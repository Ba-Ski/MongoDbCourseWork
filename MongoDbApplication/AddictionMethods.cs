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
            Regex regex = new Regex("^.*" + input + ".*$");
            var name = collection.Aggregate().Match(c => regex.IsMatch(c.nick)).ToListAsync().Result;
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

    }
}