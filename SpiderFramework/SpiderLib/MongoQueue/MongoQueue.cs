using System;
using System.Threading;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;

namespace SpiderFramework.Queue
{
    public class MongoQueue<T> : IPublish<T>, ISubscribe<T>, IDisposable where T : class
    {
        private static ILog Log = LogManager.GetLogger(typeof(MongoQueue<T>));

        private readonly MongoDatabase _database;
        private readonly MongoCollection<BsonDocument> _position;   // used to record the current position
        private readonly IMongoQuery _positionQuery;
        private readonly MongoCollection<MongoMessage<T>> _queue;   // the collection for the messages
        private readonly string _queueName = typeof(T).Name;        // name of collection (based on type name)
        MongoServer server;

        private IEnumerator<MongoMessage<T>> _enumerator; // our cursor enumerator
        private ObjectId _lastId = ObjectId.Empty;                  // the last _id read from the queue
        private bool _startedReading;                               // initial query on an empty collection is a special case

        public void Disconnect()
        {
            if (server != null
                && (server.State == MongoServerState.Connected|| server.State== MongoServerState.Connecting))
                server.Disconnect();
        }

        public MongoQueue(string connectionString, long queueSize)
        {
            // our queue name will be the same as the message class
            MongoClient client = new MongoClient(connectionString);
            server = client.GetServer();
            server.Connect();
            string dbname = MongoUrl.Create(connectionString).DatabaseName;
            _database = server.GetDatabase(dbname);

            if (!_database.CollectionExists(_queueName))
            {
                try
                {
                    Log.InfoFormat("Creating queue '{0}' size {1}", _queueName, queueSize);

					var options = CollectionOptions
						.SetCapped(true)        // use a capped collection so space is pre-allocated and re-used
						.SetAutoIndexId(false)  // we don't need the default _id index that MongoDB normally created automatically
                        .SetMaxSize(queueSize); // limit the size of the collection and pre-allocated the space to this number of bytes
						

                    _database.CreateCollection(_queueName, options);
                }
                catch
                {
                    // assume that any exceptions are because the collection already exists ...
                }
            }

            // get the queue collection for our messages
            _queue = _database.GetCollection<MongoMessage<T>>(_queueName);

            // check if we already have a 'last read' position to start from
            _position = _database.GetCollection("_queueIndex");
            var last = _position.FindOneById(_queueName);
            if (last != null)
                _lastId = last["last"].AsObjectId;

            _positionQuery = Query.EQ("_id", _queueName);
        }

        #region IPublish<T> Members

        public void Send(T message)
        {
            // sending a message is easy - we just insert it into the collection 
            // it will be given a new sequential Id and also be written to the end (of the capped collection)

            Log.Debug(string.Format("Sending {0} ({1})", _queueName, message));
            _queue.Insert(new MongoMessage<T>(message),WriteConcern.Acknowledged);
        }

        #endregion

        #region ISubscribe<T> Members

        public T Receive()
        {
            // for reading, we give the impression to the client that we provide a single message at a time
            // which means we maintain a cursor and enumerator in the background and hide it from the caller
            lock (this)
            {
                if (_enumerator == null)
                    _enumerator = InitializeCursor();

                // there is no end when you need to sit and wait for messages to arrive
                try
                {
                    // do we have a message waiting? 
                    // this may block on the server for a few seconds but will return as soon as something is available
                    if (_enumerator.MoveNext())
                    {
                        // yes - record the current position and return it to the client
                        _startedReading = true;
                        _lastId = _enumerator.Current.Id;
                        _position.Update(_positionQuery, Update.Set("last", _lastId), UpdateFlags.Upsert, WriteConcern.Acknowledged);

                        var message = _enumerator.Current.Message;
                        var delay = DateTime.UtcNow - _enumerator.Current.Enqueued;
                        Log.Debug(string.Format("Received {0} after {1}", _queueName, delay));
                        return message;
                    }
                    _enumerator.Dispose();
                    return null;
                }
                catch (Exception ex)
                {
                    // cursor died or was killed
                    _enumerator.Dispose();
                    return null;
                }
            }
        }

        #endregion

        private IEnumerator<MongoMessage<T>> InitializeCursor()
        {
            Log.Debug(string.Format("Initializing Cursor from {0}", _lastId));

            var cursor = _queue
                .Find(Query.GT("_id", _lastId))
                .SetFlags(
                    QueryFlags.AwaitData |
                    QueryFlags.NoCursorTimeout |
                    QueryFlags.TailableCursor
                )
                .SetSortOrder(SortBy.Ascending("$natural"));

            return cursor.GetEnumerator();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}