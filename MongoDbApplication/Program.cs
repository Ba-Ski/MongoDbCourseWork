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
            var parser = new HabrParser(RequestsToBase.tryInsertPost, RequestsToBase.tryInsertUser);
            try
            {
                parser.parse();

            }catch (AccessViolationException ex)
            {
                Console.WriteLine("[Parsing error]: " + ex.Message);
            }
               catch(Exception ex)
            {
                Console.WriteLine("[Some exception]: " + ex.Message);
            }
            //    var posts = parser.parse();
            //    using (StreamWriter sw = new StreamWriter("posts.json"))
            //    {
            //        foreach (var item in posts.Item1)
            //        {
            //            RequestsToBase.insertPost(item).GetAwaiter().GetResult();
            //            sw.WriteLine(item.ToJson());
            //            sw.WriteLine();
            //        }
            //    }
            //    using (StreamWriter sw = new StreamWriter("users.json"))
            //    {
            //        foreach (var item in posts.Item2)
            //        {
            //            RequestsToBase.insertUser(item).GetAwaiter().GetResult();
            //            sw.WriteLine(item.ToJson());
            //            sw.WriteLine();
            //        }
            //    }

            //}
            //catch (ApplicationException ex)
            //{
            //    Console.WriteLine("[Parsing error]: " + ex.Message);
            //}
            #region old one
            //for (int i = 0; i < 10; i++)
            //{
            //    RequestsToBase.insertUser(new User
            //    {
            //        name = "Вася" + i
            //    }).GetAwaiter().GetResult();
            //}
            /*RequestsToBase.insertUser(new User
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
            //Console.WriteLine("Enter a search pattern ");


            // var userList = AddictionMethods.getUserByNameWithRegex(Console.ReadLine());
            // if ((userList == null) || (userList.Count == 0))
            //     Console.WriteLine("No users were found");
            // else
            //     foreach (var user in userList)
            //     {
            //         Console.WriteLine("Nick " + user.nick);
            //         Console.WriteLine("rDatefsd " + user.registerDate);
            //     }
            #endregion
            #region Task Algo Fuzzy search
            //Console.WriteLine("Enter a string pattern, that will be insert in \".*pattern.*\"");
            //var users = AddictionMethods.getUserByNIckWithRegex(Console.ReadLine());
            //double maxMatch = 0;
            //Console.WriteLine("Enter a string, that will be original for comparing with");
            //string line = Console.ReadLine(); ;
            //Dictionary<string, double> matches = new Dictionary<string, double>();
            //foreach (var user in users)
            //{
            //    if (!matches.ContainsKey(user.nick))
            //    {
            //        double match = AlgoSearch.LevenshteinDist(user.nick, line);
            //        if (maxMatch < match) maxMatch = match;
            //        matches.Add(user.nick, match);
            //    }
            //}
            //if (matches.Count != 0)
            //{
            //    foreach (var d in matches)
            //        Console.WriteLine("Name: {0} distance: {1}", d.Key, d.Value);
            //}
            //else Console.WriteLine("There are no elements for comparing with");
            #endregion

            #region Task Algo Fuzzy search (2)
            AddictionMethods.task2FindWithLevinstein();

            #endregion
            Console.ReadKey();
        }



    }
}
