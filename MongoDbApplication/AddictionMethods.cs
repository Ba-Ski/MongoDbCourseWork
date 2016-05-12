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
        private const string English = "qwertyuiop[]asdfghjkl;'zxcvbnm,.QWERTYUIOP{}ASDFGHJKL:\"ZXCVBNM<>?";
        private const string Russian = "йцукенгшщзхъфывапролджэячсмитьбюЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮ,";

        public static string convertEngToRus(string input)
        {
            var result = new StringBuilder(input.Length);
            int index;
            foreach (var symbol in input)
                result.Append((index = Russian.IndexOf(symbol)) != -1 ? English[index] :
                symbol);
            return result.ToString();
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