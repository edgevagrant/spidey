using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SpiderFramework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderTasks.cnblogs
{
    public class CnblogsNews : IMongoDBEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }


        [BsonIgnore]
        public string Key
        {
            get { return Url; }
        }
    }
}
