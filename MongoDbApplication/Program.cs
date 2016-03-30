using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDbApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient();
            var database = client.GetDatabase("blog");
            var collection = database.GetCollection<Post>("blogPart");
        }
    }
}
