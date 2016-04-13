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
    class Program
    {
        static void Main(string[] args)
        {

            /* try { 
                 IEnumerable<Post> posts = HabrParser.getPosts();

                 foreach (var item in posts)
                 {
                     RequestsToBase.insertPost(item).GetAwaiter().GetResult();

                 }
             }
             catch(ApplicationException ex)
             {
                 Console.WriteLine("[Parsing error]: " + ex.Message);
             }*/

            /* RequestsToBase.insertUser(new User
             {
                 name = "denis_l_eryomin",
                 email = "denis_l_eryomin@yandex.ru",
                 password = "i am cool guy",
             }).GetAwaiter().GetResult();

             RequestsToBase.insertUser(new User
             {
                 name = "arodygin",
                 email = "arodygin@yandex.ru",
                 password = "i am cool guy",
             }).GetAwaiter().GetResult();

             RequestsToBase.insertUser(new User
             {
                 name = "sidristij",
                 email = "sidristij@yandex.ru",
                 password = "i am cool guy",
             }).GetAwaiter().GetResult();

             RequestsToBase.insertUser(new User
             {
                 name = "kosatchev",
                 email = "kosatchev@yandex.ru",
                 password = "i am cool guy",
             }).GetAwaiter().GetResult();*/

            /*var posts = RequestsToBase.findPost(new BsonDocument
            {
                {"author" , "kosatchev" }
            }).GetAwaiter().GetResult();

            foreach (Post post in posts)
            {
                Console.WriteLine("{0} - {1} ({2})", post.Id, post.title, post.author);
            }*/

            RequestsToBase.insertUser(new User
            {
                name = "ITMO",
                email = "ITMO@yandex.ru",
                password = "i am cool guy",
            }).GetAwaiter().GetResult();



            Console.ReadKey();
        }

        
    
    }
}
