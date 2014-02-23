using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpiderFramework.Storage;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SpiderTasks.smzdm
{
    public class CouponEntity:IMongoDBEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }
        public DateTime Time { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public string TargetUrl { get; set; }

        [BsonIgnore]
        public string Key
        {
            get { return Title; }
        }
    }
}
