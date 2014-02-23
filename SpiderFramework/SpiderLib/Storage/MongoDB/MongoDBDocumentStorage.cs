using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization;
using log4net;

namespace SpiderFramework.Storage
{
    
    public class MongoDBDocumentStorage:IDocumentStorage
    {
        string connectionString = "";
        string collectionName = "";
        MongoClient client;
        
        public MongoDBDocumentStorage(string connectionString, string collectionName)
        {
            this.Init(connectionString, collectionName);
        }
        private void Init(string connectionString, string collectionName)
        {
            this.connectionString = connectionString;
            this.collectionName = collectionName;
            client = new MongoClient(this.connectionString);
        }
        public MongoDBDocumentStorage(string connectionString)
        {
            string[] conn = connectionString.Split('|');
            if (conn.Length < 2)
                throw new ArgumentException("connection string is invalid: " + connectionString);

            this.connectionString = conn[0];
            this.collectionName = conn[1];
            this.Init(this.connectionString, this.collectionName);
        }

        public string DefaultPath
        {
            get;
            set;
        }

        public void Connect()
        {
            client.GetServer().Connect();
        }
        public void Disconnect()
        {
            if(client!=null)
                client.GetServer().Disconnect();
        }
        public bool DocumentExists(string url)
        {
            string dbname = MongoUrl.Create(this.connectionString).DatabaseName;
            MongoDatabase db = client.GetServer().GetDatabase(dbname);
			var coll = db.GetCollection<IDocument>(collectionName);
            return coll.Count(Query.EQ("Url", url)) > 0;
        }
        ILog logger = LogManager.GetLogger(typeof(MongoDBDocumentStorage));
		public bool SaveDocument(IDocument document, bool rewrite)
        {
            string dbname = MongoUrl.Create(this.connectionString).DatabaseName;
            MongoDatabase db = client.GetServer().GetDatabase(dbname);

			var coll = db.GetCollection<IDocument>(collectionName);
            if (coll.Count(Query.EQ("Url", document.Url)) > 0)
            {
                if (rewrite)
                {
                    WriteConcernResult smr = coll.Remove(Query.EQ("Url", document.Url), WriteConcern.Acknowledged);
                    if (smr.HasLastErrorMessage)
                    {
                        Console.WriteLine(smr.ErrorMessage);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            WriteConcernResult smr2 = coll.Insert(document, WriteConcern.Acknowledged);
            if (smr2.HasLastErrorMessage)
            {
                Console.WriteLine(smr2.ErrorMessage);
            }
            return !smr2.HasLastErrorMessage;        
        }
		public bool SaveDocument(IDocument document)
        {
            return SaveDocument(document, false);
        }


		public IDocument LoadDocument(string url)
        {
            MongoDatabase db = client.GetServer().GetDatabase(connectionString);
			var coll = db.GetCollection<IDocument>(collectionName);
            if (coll.Count(Query.EQ("Url", url)) == 0)
            {
                return null;
            }
            return coll.FindOne(Query.EQ("Url", url));
        }

		public List<IDocument> LoadDocuments()
        {
            BsonClassMap.RegisterClassMap<HtmlContent>();
            MongoDatabase db = client.GetServer().GetDatabase(connectionString);
			var coll = db.GetCollection<IDocument>(collectionName);

			var list = coll.FindAllAs<IDocument>();
			return list.ToList<IDocument>();
        }
    }
}
