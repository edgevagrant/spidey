using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using log4net;

namespace SpiderFramework.Storage
{
    public class MongoDBEntityStorage<T>:IDisposable where T:IMongoDBEntity
    {
        ILog logger = LogManager.GetLogger(typeof(MongoDBEntityStorage<T>));
        string dbname = "";
        string collectionName = "";
        MongoServer server=null;
        string keyName = "";
        public MongoDBEntityStorage(string connectionString, string collectionName, string keyFieldName)
        {
            this.dbname = MongoUrl.Create(connectionString).DatabaseName;
            this.collectionName = collectionName;
            MongoClient client=new MongoClient(connectionString);
            this.server = client.GetServer();
            this.keyName = keyFieldName;
        }
        public void Connect()
        {
            if (server != null)
                server.Connect();
        }
        public void Disconnect()
        {
            if (server != null&& server.State== MongoServerState.Connected)
                server.Disconnect();
        }
        public bool Exists(string keyValue)
        {
            MongoDatabase db = server.GetDatabase(dbname);
            var coll = db.GetCollection<T>(collectionName);
            return coll.Count(Query.EQ(keyName, keyValue)) > 0;
        }
        public bool Save(T entity)
        {
            MongoDatabase db = server.GetDatabase(dbname);
            var coll = db.GetCollection<T>(collectionName);
            if (coll.Count(Query.EQ(keyName, entity.Key)) > 0)
            {
                return false;
            }
            WriteConcernResult smr = coll.Insert(entity, WriteConcern.Acknowledged);
            if (smr.ErrorMessage != null)
            {
                logger.Error(smr.ErrorMessage);
                return false;
            }
            return true;
        }
        public T Load(string keyValue)
        {
            MongoDatabase db = server.GetDatabase(dbname);
            var coll = db.GetCollection<T>(collectionName);
            if (coll.Count(Query.EQ(keyName, keyValue)) == 0)
            {
                return default(T);
            }
            return coll.FindOne(Query.EQ(keyName, keyValue));
        }

        public void Dispose()
        {
            this.Disconnect();
        }
    }
}
