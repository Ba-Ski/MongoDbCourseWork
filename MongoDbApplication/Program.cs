using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbApplication.Structure;
using MongoDbApplication.Habr;

namespace MongoDbApplication
{
    class Program
    {
        static void Main(string[] args)
        {

             try { 
                 var posts = HabrParser.parse();

                 foreach (var item in posts.Item1)
                 {
                     RequestsToBase.insertPost(item).GetAwaiter().GetResult();

                 }
                 foreach (var item in posts.Item2)
                {
                    RequestsToBase.insertUser(item).GetAwaiter().GetResult();
                }
             }
             catch(ApplicationException ex)
             {
                 Console.WriteLine("[Parsing error]: " + ex.Message);
             }
            #region old one
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

            /*RequestsToBase.insertUser(new User
            {
                name = "ITMO",
                email = "ITMO@yandex.ru",
                password = "i am cool guy",
            }).GetAwaiter().GetResult();*/
            #endregion
            #region Iliya one
            //for (int i = 0; i < 10; i++)
            //{
            //    RequestsToBase.insertUser(new User
            //    {
            //        name = "ИТМО" + i.ToString(),
            //        email = "ИТМО" + i.ToString() + "@yandex.ru",
            //        password = "i am cool guy" + i.ToString(),
            //    }).GetAwaiter().GetResult();
            //}

            //AddictionMethods.getAggregateData();
            //Console.WriteLine(AddictionMethods.convertEngToRus(Console.ReadLine()));
            /*var userList = AddictionMethods.getUserByNameWithRegex(Console.ReadLine());
            foreach(var user in userList )
            {
                Console.WriteLine("Name "+user.name);
                Console.WriteLine("Email " + user.email);
            }
            */
            #endregion 
            Console.ReadKey();
        }

        
    
    }
}
