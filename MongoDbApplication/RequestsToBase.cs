using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbApplication.Structure;

namespace MongoDbApplication
{
    class RequestsToBase
    {
        public static async Task insertUser(User user)
        {
            var blogContext = new BlogContext(); 
            await blogContext.Users.InsertOneAsync(user);
        }

        public static async Task insertPost(Post post)
        {
            var blogContext = new BlogContext();
            await blogContext.Posts.InsertOneAsync(post);
        }

        public static async Task<List<Post>> findPost(BsonDocument filter)
        {
            var blogContext = new BlogContext();
            var posts = await blogContext.Posts.Find(filter).ToListAsync();
            return posts;
        }

        public static async Task updatePerson(Post post)
        {
            var blogContext = new BlogContext();
            var result = await blogContext.Posts.UpdateOneAsync(
                new BsonDocument("author", "Vasya"),
                post.ToBsonDocument());
                //new BsonDocument
                //{
                //    {"title","knowledge"},
                //    {"author", "Kate"},

                //});
            Console.WriteLine("Найдено по соответствию: {0}; обновлено: {1}",
                result.MatchedCount, result.ModifiedCount);
        }

        public static async Task DeletePerson(BsonDocument doc)
        {
            var blogContext = new BlogContext();
            var filter = doc;
            await blogContext.Posts.DeleteOneAsync(filter);
        }
    }
}
