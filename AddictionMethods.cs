using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDbApplication.Structure;

namespace MongoDbApplication
{
    class AddictionMethods
    {
        const string English = "qwertyuiop[]asdfghjkl;'zxcvbnm,.QWERTYUIOP{}ASDFGHJKL:\"ZXCVBNM<>?";
        const string Russian = "йцукенгшщзхъфывапролджэячсмитьбюЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮ,";
        public static string convertEngToRus(string input)
        {
            var result = new StringBuilder(input.Length);
            int index;
            foreach (var symbol in input)
                result.Append((index = Russian.IndexOf(symbol)) != -1 ? English[index] :
                symbol);
            return result.ToString();
        }
        public static async void getAggregateData()
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
        public static List<User> getUserByNameWithRegex(string input)
        {

            if (isEnglish(input))
            {
                input = convertEngToRus(input);
            }

            var blogContext = new BlogContext();
            var collection = blogContext.Users;
            Regex regex = new Regex(".*" + input + ".*");
            var name = collection.Aggregate().Match(c => regex.IsMatch(c.name)).ToListAsync().Result;
            return name;

        }
        public static bool isEnglish(string s)
        {
            s = s.ToLower();
            byte[] b = System.Text.Encoding.Default.GetBytes(s);
            int angl_count = 0, rus_count = 0;
            foreach (byte bt in b)
            {
                if ((bt >= 97) && (bt <= 122)) angl_count++;
                if ((bt >= 224) && (bt <= 255)) rus_count++;
            }
            if (angl_count > rus_count) return false;
            else return true;
        }
        public static List<User> getUser(string s)
        {
            var blogContext = new BlogContext();
            var collection = blogContext.Users;
            var result = collection.Aggregate().Match(new BsonDocument { { "name", s } }).ToListAsync().Result;
            return result;
        }
        public static void task2FindWithLevinstein()
        {
            Console.WriteLine("What user would you like to find?");
            Dictionary<string, double> matches = new Dictionary<string, double>();
            string line2 = Console.ReadLine();
            if (isEnglish(line2))
            {
                line2 =convertEngToRus(line2);
            }
            var users2 = getUser(line2);
            if ((users2 != null) && (users2.Count != 0))
            {
                foreach (var user in users2)
                {
                    Console.WriteLine("User with nick {1} and name {0} was registred at {2}", user.name, user.nick, user.registerDate);
                }
            }
            else
            {
                double minmatch = 100;
                var users3 = getUserByNameWithRegex(line2);
                if ((users3 != null) && (users3.Count != 0))
                {
                    foreach (var user in users3)
                    {
                        if (!matches.ContainsKey(user.name))
                        {
                            double match = AlgoSearch.LevenshteinDist(user.name, line2);
                            matches.Add(user.name, match);
                        }
                    }
                }
                if (matches.Count != 0)
                {
                    matches = matches.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                    foreach (var i in matches)
                    {
                        if (i.Value < minmatch) minmatch = i.Value;
                    }
                    Console.WriteLine("Maybe you've mistaked and you wanted to find somebody from the list below ?");
                    foreach (var i in matches)
                    {
                        if (i.Value == minmatch)
                            Console.WriteLine("{0}", i.Key);
                    }
                }
                else
                    Console.WriteLine("Seems you've made and mistake and there are no one who can be similar to your entered name");
            }
        }

        }
}