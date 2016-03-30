﻿using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDbApplication.Structure
{
    class Post
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string content { get; set; }
        public IEnumerable<Comment> comments { get; set; }
        public IEnumerable<string> tags { get; set; }
        public DateTime date { get; set; }
    }
}
