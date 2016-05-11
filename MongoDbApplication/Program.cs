using System;
using System.IO;
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
                using (StreamWriter sw = new StreamWriter("posts.json")) {
                    foreach (var item in posts.Item1)
                    {
                        RequestsToBase.insertPost(item).GetAwaiter().GetResult();
                        sw.WriteLine(item.ToJson());
                        sw.WriteLine();
                    }
                }
                using (StreamWriter sw = new StreamWriter("users.json"))
                {
                    foreach (var item in posts.Item2)
                    {
                        RequestsToBase.insertUser(item).GetAwaiter().GetResult();
                        sw.WriteLine(item.ToJson());
                        sw.WriteLine();
                    }
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

            //AddictionMethods.getAggregateData();
            //Console.WriteLine(AddictionMethods.convertEngToRus(Console.ReadLine()));
            /* Console.WriteLine("Enter a search pattern ");


             var userList = AddictionMethods.getUserByNameWithRegex(Console.ReadLine());
             if (userList == null)
                 Console.WriteLine("No users were found");
             else
                 foreach (var user in userList)
                 {
                     Console.WriteLine("Nick " + user.nick);
                     Console.WriteLine("rDatefsd " + user.registerDate);
                 }
                 */
            #endregion
            Console.ReadKey();
        }



    }
}
