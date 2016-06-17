using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MongoDB.Driver;
using MongoDbApplication.Structure;

namespace MongoDbApplication
{
    public class BlogContext
    {
        public const string CONNECTION_STRING_NAME = "mongodb://localhost";
        public const string DATABASE_NAME = "blog";
        public const string POSTS_COLLECTION_NAME = "posts";
        public const string USERS_COLLECTION_NAME = "users";

        // This is ok... Normally, these or the entire BlogContext
        // would be put into an IoC container. We aren't using one,
        // so we'll just keep them statically here as they are 
        // thread-safe.
        private static readonly IMongoClient _client;
        private static readonly IMongoDatabase _database;

        static BlogContext()
        {
            //var connectionString = ConfigurationManager.ConnectionStrings[CONNECTION_STRING_NAME].ConnectionString;
            _client = new MongoClient(CONNECTION_STRING_NAME);
            _database = _client.GetDatabase(DATABASE_NAME);
        }

        public IMongoClient Client
        {
            get { return _client; }
        }

        public IMongoDatabase Database
        {
            get { return _database; }
        }

        public IMongoCollection<Post> Posts
        {
            get { return _database.GetCollection<Post>(POSTS_COLLECTION_NAME); }
        }

        public IMongoCollection<User> Users
        {
            get { return _database.GetCollection<User>(USERS_COLLECTION_NAME); }
        }

    }
}
