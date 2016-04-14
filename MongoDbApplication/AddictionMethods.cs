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
                result.Append((index = English.IndexOf(symbol)) != -1 ? Russian[index] :
                symbol);
            return result.ToString();
        }
        public static async void getAggregateData()
        {
            var blogContext = new BlogContext();
            var collection = blogContext.Users;
            var userCollection = await collection.Aggregate()
                .Match(new BsonDocument { { "name", new BsonDocument { { "$gte", "ИТМО" }, { "$lt", "ИТМОЯ" } } } })
                .Group(new BsonDocument { { "_id", new BsonDocument { { "name", "$name" }, { "email", "$email" } } } })
                .Project(new BsonDocument { { "Name", "$_id.name" }, { "Email", "$_id.email" } })
                .ToListAsync();
            foreach (var human in userCollection)
            {
                Console.WriteLine("Name:\t" + human.GetValue("Name"));
                Console.WriteLine("Email:\t" + human.GetValue("Email"));
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
            if (angl_count > rus_count) return true;
            else return false;
        }

    }
}