using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDbApplication.Structure
{
    public class Comment
    {
        public string author { get; set; }
        public string content { get; set; }
        public DateTime date { get; set; }
        public int nestingLevel { get; set; }
        //[BsonIgnoreIfNull]
        //public IEnumerable<Comment> comments { get; set; }
    }
}
