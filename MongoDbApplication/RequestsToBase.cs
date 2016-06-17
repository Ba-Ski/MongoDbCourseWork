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

        public static async Task<bool> tryInsertUser(User user)
        {
            var blogContext = new BlogContext();
            var filter = Builders<User>.Filter.Eq(x => x.nick, user.nick);
            var document = await blogContext.Users.Find(filter).FirstOrDefaultAsync();
            if (document == null)
            {
                await blogContext.Users.InsertOneAsync(user);
                return true;
            }
            return false;
        }

        public static async Task<bool> tryInsertPost(Post post)
        {
            var blogContext = new BlogContext();
            var builder = Builders<Post>.Filter;
            var filter = builder.Eq(x => x.title, post.title) & builder.Eq(x => x.author, post.author);
            var document = await blogContext.Posts.Find(filter).FirstOrDefaultAsync();
            if (document == null)
            {
                await blogContext.Posts.InsertOneAsync(post);
                return true;
            }
            return false;
        }

        public static async Task insertPost(Post post)
        {
            var blogContext = new BlogContext();
            await blogContext.Posts.InsertOneAsync(post);
        }

        public static async Task<List<Post>> findPost(string author)
        {
            var blogContext = new BlogContext();
            var filter = Builders<Post>.Filter.Eq("author", author);
            var posts = await blogContext.Posts.Find(filter).ToListAsync();
            return posts;
        }

        public static async Task<List<User>> findUserByNick(string nick)
        {
            var blogContext = new BlogContext();
            var filter = Builders<User>.Filter.Eq("nick", nick);
            var users = await blogContext.Users.Find(filter).ToListAsync();
            return users;
        }

        public static async Task<List<User>> findUserByName(string name)
        {
            var blogContext = new BlogContext();
            var filter = Builders<User>.Filter.Eq("name", name);
            var users = await blogContext.Users.Find(filter).ToListAsync();
            return users;
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

        public static async Task<int> getCommentsCountByUserTag(string nick)
        {
            throw new NotImplementedException();
        }

        public static async Task<int> getPostsCountByUser(string nick)
        {
            throw new NotImplementedException();
        }

        //public static async Task<List<Post>> getPostsbyTag(string tag)
        //{
        //    var blogContext = new BlogContext();
        //    var filter = Builders<Post>.Filter.Eq("tags", tag);
        //    var posts = await blogContext.Users.Find(filter).ToListAsync();
        //    return posts;
        //}

        public static async Task<Dictionary<string, List<Post>>> getPostsbyTags(string tag)
        {
            throw new NotImplementedException();
        }

        public static async void getPostsByDate()
        {
            var blogContext = new BlogContext();
            var collection = blogContext.Users;
            var userCollection = await collection.Aggregate()
                .Match(new BsonDocument { { "nick", new BsonDocument { { "$gte", "xv" }, { "$lt", "xz" } } } })
                .Group(new BsonDocument { { "_id", new BsonDocument { { "nick", "$nick" }, { "registerDate", "$registerDate" } } } })
                .Project(new BsonDocument { { "Nick", "$_id.nick" }, { "rDate", "$_id.registerDate" } })
                .ToListAsync();
            foreach (var human in userCollection)
            {
                Console.WriteLine("Nick:\t" + human.GetValue("Nick"));
                Console.WriteLine("rDate:\t" + human.GetValue("rDate"));
                Console.WriteLine();
            }
        }

        public static async Task<List<string>> levenshteinSearch(string name) {
            string search = @"
                function levensteinSearch (key) {
            if(this.hasOwnProperty(""name"")){
                var users = db.users.find({ 'name': { $exists: true } }, { name: 1, _id: 0});
                    var name
                for (index = 0; index < users.length; index++)
                    {
                        var b = key;
                        var a = users[index];
                        if (a.length === 0) return b.length;
                        if (b.length === 0) return a.length;

                        var matrix = [];

                        // increment along the first column of each row
                        var i;
                        for (i = 0; i <= b.length; i++)
                        {
                            matrix[i] = [i];
                        }

                        // increment each column in the first row
                        var j;
                        for (j = 0; j <= a.length; j++)
                        {
                            matrix[0][j] = j;
                        }

                        // Fill in the rest of the matrix
                        for (i = 1; i <= b.length; i++)
                        {
                            for (j = 1; j <= a.length; j++)
                            {
                                if (b.charAt(i - 1) == a.charAt(j - 1))
                                {
                                    matrix[i][j] = matrix[i - 1][j - 1];
                                }
                                else {
                                    matrix[i][j] = Math.min(matrix[i - 1][j - 1] + 1, // substitution
                                                            Math.min(matrix[i][j - 1] + 1, // insertion
                                                                    matrix[i - 1][j] + 1)); // deletion
                                }
                            }
                        }
                        users.distance = matrix[b.length][a.length];
                    }
                    users.sort(function(a, b) { return a[""disr""] - b[""dist""] });
                    return users.slice(0, 10);
                }
            }";

            var blogContext = new BlogContext();
            var result = blogContext.Databas

        }
        //public static async Task<List<string>> DLSearch(string name)
        //{
        //    string map = @"
        //        function DL() {
        //            var source = this.name;
        //            var target = scope.key;
        //            if (!source || source.length === 0)
        //                if (!target || target.length === 0)
        //                return 0;
        //                else
        //                return target.length;
        //            else if (!target)
        //                return source.length;
        //            var sourceLength = source.length;
        //            var targetLength = target.length;
        //            var score = [];
        //            var INF = sourceLength + targetLength;
        //            score[0] = [INF];
        //            for (var i=0 ; i <= sourceLength ; i++) { score[i + 1] = []; score[i + 1][1] = i; score[i + 1][0] = INF; }
        //            for (var i=0 ; i <= targetLength ; i++) { score[1][i + 1] = i; score[0][i + 1] = INF; }
        //            var sd = {};
        //            var combinedStrings = source + target;
        //            var combinedStringsLength = combinedStrings.length;
        //            for(var i=0 ; i < combinedStringsLength ; i++) {
        //                var letter = combinedStrings[i];
        //                if (!sd.hasOwnProperty(letter))
        //                sd[letter] = 0;
        //            }
        //            for (var i=1 ; i <= sourceLength ; i++) {
        //                var DB = 0;
        //                for (var j=1 ; j <= targetLength ; j++) {
        //                var i1 = sd[target[j - 1]];
        //                var j1 = DB;
        //                if (source[i - 1] == target[j - 1]) {
        //                    score[i + 1][j + 1] = score[i][j];
        //                    DB = j;
        //                }
        //                else
        //                    score[i + 1][j + 1] = Math.min(score[i][j], Math.min(score[i + 1][j], score[i][j + 1])) + 1;
        //                score[i + 1][j + 1] = Math.min(score[i + 1][j + 1], score[i1][j1] + (i - i1 - 1) + 1 + (j - j1 - 1));
        //                }
        //                sd[source[i - 1]] = i;
        //            }
        //            empit(score[sourceLength + 1][targetLength + 1], this.name);
        //        }";

        //    var blogContext = new BlogContext();
        //    var result = blogContext.E

        //}
    }
}
