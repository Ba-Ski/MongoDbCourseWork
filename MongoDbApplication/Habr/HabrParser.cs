using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;
using MongoDbApplication.Structure;
using System.Text.RegularExpressions;

namespace MongoDbApplication.Habr
{

    static class HabrParser
    {
        private static string Path = "https://habrahabr.ru/hubs/page";
        private static int HubPageNumber;
        private static int TaskCount = 4;
        private static ConcurrentDictionary<string, Post> _posts;
        private static ConcurrentDictionary<string, User> _users;

        public static HtmlNode inicializePage(string path)
        {
            var html = new HtmlDocument();
            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            html.LoadHtml(webClient.DownloadString(path));
            var root = html.DocumentNode;
            if (root == null) throw new ApplicationException("page loading error");

            return root;
        }

        public static Tuple<IEnumerable<Post>, IEnumerable<User>> parse()
        {
            _posts = new ConcurrentDictionary<string, Post>();
            _users = new ConcurrentDictionary<string, User>();

            var root = inicializePage(Path + 1);

            var pageNumNode = root.SelectSingleNode("//ul[(@id = 'nav-pages')] //a[last()]");

            if (pageNumNode != null)
                HubPageNumber = int.Parse(pageNumNode.InnerText);
            else {
                pageNumNode = null;
                throw new ApplicationException("count of hubs pages not found");
            }

            List<KeyValuePair<string, int>> pathCountPairs = new List<KeyValuePair<string, int>>();
            int postsCount = 0;
            for (int i = 1; i <= HubPageNumber; i++)
            {
                root = inicializePage(Path + i);

                var counts = root.SelectNodes("//div[(@id = 'hubs')] //div[(@class = 'stat')] /a[last()]");
                var hubsPaths = root.SelectNodes("//div[(@id = 'hubs')] //div[(@class = 'title')] /a[1]");

                int hubPostsCount;
                string hubPath;

                if(counts.Count != hubsPaths.Count)
                {
                    throw new ApplicationException("can't check posts count in hubs");
                }

                for (int j = 0; j < counts.Count; j++)
                {
                    hubPostsCount = parseHubPostsCount(counts[j].InnerText);
                    hubPath = hubsPaths[j].GetAttributeValue("href", "");
                    if (hubPath != "")
                    {
                        pathCountPairs.Add(new KeyValuePair<string, int>(hubPath, hubPostsCount));
                        postsCount += hubPostsCount;
                    }
                    else
                    {
                        Console.WriteLine("So strange");
                    }
                }
            }

            pathCountPairs.Sort((x, y) => x.Value.CompareTo(y.Value));
            int perThread = postsCount / TaskCount;
            int count = 0;
            int n = 0;
            List<string>[] hubsPerTask = new List<string>[TaskCount];
            for (int i = 0; i < TaskCount; i++)
            {
                hubsPerTask[i] = new List<string>();
            }
            
            for (int i = pathCountPairs.Count - 1; i >= 0; i--)
            {
                count += pathCountPairs[i].Value;
                hubsPerTask[n].Add(pathCountPairs[i].Key);
                if (count >= perThread && n < 3)
                {  
                    count = 0;
                    n++;
                }
                
            }

            Task task1 = Task.Factory.StartNew (() => parseThread(hubsPerTask[0]));
            Task task2 = Task.Factory.StartNew(() => parseThread(hubsPerTask[1]));
            Task task3 = Task.Factory.StartNew(() => parseThread(hubsPerTask[2]));
            Task task4 = Task.Factory.StartNew(() => parseThread(hubsPerTask[3]));

            Task.WaitAll(task1, task2, task3, task4);

            Tuple<IEnumerable<Post>, IEnumerable<User>> pair =
                new Tuple<IEnumerable<Post>, IEnumerable<User>>(_posts.Values, _users.Values);

            return pair;

        }

        private static int parseHubPostsCount(string countStr)
        {
            string kSstr    = @"(\d+,\d|\d+)k\s+\w+";
            string kLessStr = @"\d+\s+\w+";
            string number   = @"(\d+,\d|\d+)";
            int count = 0;
            try
            {

                if (Regex.IsMatch(countStr, kSstr))
                {
                    count = (int)(double.Parse(Regex.Match(countStr, number).Value) * 1000);
                }
                else if (Regex.IsMatch(countStr, kLessStr))
                {
                    count = int.Parse(Regex.Match(countStr, number).Value);
                }
                else
                {
                    count = 0;
                    throw new ApplicationException("count parse error"); // is it good decision to throw exception?
                }

                return count;
            }
            catch(Exception ex)
            {
                throw new FormatException("Posts count parsing error: " + ex.Message);
            }
        }

        private static void parseThread(List<string> hubs)
        {
                foreach (var item in hubs)
                {
                        parseHub(item + "/all");
                }
        }

        private static void parseHub(string hubPath)
        {

            var root = inicializePage(hubPath);

            var lastPagePathNode = root.SelectSingleNode("//a[(@title = 'Последняя страница')]");
            string lastPagePath;

            if (lastPagePathNode == null)
            {
                var lastLiNode = root.SelectSingleNode("//ul[(@id = 'nav-pages')] //a[last()]");
                if (lastLiNode != null)
                    lastPagePath = lastLiNode.GetAttributeValue("href", null);
                else
                    lastPagePath = null;
            }
            else
            {
                lastPagePath = lastPagePathNode.GetAttributeValue("href", null);
            }

            if (lastPagePath == null) return;

            Regex regex = new Regex(@"[0-9]+");
            var pageCount = int.Parse(regex.Match(lastPagePath).Value);


            for (int i = 1; i <= pageCount; i++)
            {
                parseArticles(hubPath + "page" + i);
            }


        }

        private static void parseArticles(string pagePath)
        {

            var root = inicializePage(pagePath);

            var articles = root.SelectNodes("//div[contains(@class,'posts shortcuts_items')] //a[@class = 'post_title']");
            if (articles == null) return;
            string postAdress;

            foreach (var item in articles)
            {

                postAdress = item.GetAttributeValue("href", null);

                if (postAdress == null) throw new ApplicationException("article preview doesn't have href");

                if (!_posts.ContainsKey(postAdress))
                {
                    var post = HabrPostParser.parsePost(postAdress, _users);

                    if (post != null)
                    {
                        _posts.TryAdd(postAdress, post);
                    }
                }

            }
        }

    }
}
