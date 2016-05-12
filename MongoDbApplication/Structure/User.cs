using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDbApplication.Structure
{
    public class User //TODO add a carma and a rating
    {
        [BsonId]
        public ObjectId id { get; set; }
        [BsonRequired]
        public string nick { get; set; }
        [BsonRequired]
        public DateTime registerDate { get; set; }
        [BsonIgnoreIfNull]
        public string name { get; set; }
        [BsonIgnoreIfNull]
        public string[] from { get; set; }
    }
}
