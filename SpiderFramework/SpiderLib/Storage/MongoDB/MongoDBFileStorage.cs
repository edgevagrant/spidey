using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SpiderFramework.Storage
{
    public class MongoDBFileStorage:IFileStorage
    {
        string connectionString = null;
        string collectionName = null;
        string dbname = null;
        MongoClient client;

        public MongoDBFileStorage(string connectionString, string collectionName)
        {
            this.connectionString = connectionString;
            this.collectionName = collectionName;
            this.dbname = MongoUrl.Create(this.connectionString).DatabaseName;
            client = new MongoClient(this.connectionString);
        }
        public string Add(string path, Stream stream)
        {
            return Add(path, stream, null);
        }
        public string Add(string path, Stream stream, string contentType)
        {
            return Add(path, stream, contentType, null);
        }
        public string Add(string path, Stream stream, string contentType, string category)
        {
            var gridFs = client.GetServer().GetDatabase(this.dbname).GridFS;
            var options = new MongoGridFSCreateOptions();
            if (contentType != null) 
            {
                options.ContentType = contentType;
            }
            if (category != null)
            {
                options.Metadata = new MongoDB.Bson.BsonDocument();
                options.Metadata.Add(category, BsonValue.Create(category));
            }
            options.UploadDate = DateTime.Now;
            var result = gridFs.Upload(stream, path, options);
            return result.Id.ToString();
        }

        public MongoGridFSStream Open(string path)
        {
            var gridFs = client.GetServer().GetDatabase(this.dbname).GridFS;
            return gridFs.Open(path, FileMode.Open, FileAccess.Read);
        }

        public void Delete(string path)
        {
            var gridFs = client.GetServer().GetDatabase(this.dbname).GridFS;
            gridFs.Delete(path);
        }

        public bool Exists(string path)
        {
            var gridFs = client.GetServer().GetDatabase(this.dbname).GridFS;
            return gridFs.Exists(path);
        }
    }
}
