using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Net;
using HtmlAgilityPack;
using MongoDbApplication.Structure;
using System.Text.RegularExpressions;

namespace MongoDbApplication.Habr
{

    static class HabrParser
    {
        private static string PATH = "https://habrahabr.ru/hubs/page";
        private static int HUB_PAGE_NUMBER = 7;
        private static IDictionary<string, Post> _posts;
        private static IDictionary<string, User> _users;

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

            //for (int i = 1; i <= HUB_PAGE_NUMBER; i++)
            //{
            //    var root = inicializePage(PATH + i);

            //    var hubs = root.SelectNodes("//div[(@id = 'hubs')] //div[(@class = 'title')]");

            //    string hubPath;

            //    foreach (var item in hubs)
            //    {
            //        hubPath = item.Element("a").GetAttributeValue("href", "");
            //        if (hubPath != "")
            //            parseHub(hubPath);

            //    }

            //}
            Thread thread1 = new Thread(() => parseThread(1, 2));
            Thread thread2 = new Thread(() => parseThread(3, 4));
            Thread thread3 = new Thread(() => parseThread(4, 5));
            Thread thread4 = new Thread(() => parseThread(6, 7));

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();

            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();

            Tuple<IEnumerable<Post>, IEnumerable<User>> pair =
                new Tuple<IEnumerable<Post>, IEnumerable<User>>(_posts.Values, _users.Values);

            return pair;

        }

        private static void parseThread(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                var root = inicializePage(PATH + i);

                var hubs = root.SelectNodes("//div[(@id = 'hubs')] //div[(@class = 'title')]");

                string hubPath;

                foreach (var item in hubs)
                {
                    hubPath = item.Element("a").GetAttributeValue("href", "");
                    if (hubPath != "")
                        parseHub(hubPath);

                }

            }
        }

        private static void parseHub(string hubPath)
        {

            var root = inicializePage(hubPath);

            var lastPagePathNode = root.SelectSingleNode("//a[(@title = 'Последняя страница')]");
            string lastPagePath;

            if (lastPagePathNode == null)
            {
                lastPagePath = root.SelectSingleNode("//ul[(@id = 'nav-pages')] //a[last()]")
                    .GetAttributeValue("href", null);
            }
            else
            {
                lastPagePath = lastPagePathNode.GetAttributeValue("href", null);
            }

            if (lastPagePath == null) return;

            Regex regex = new Regex(@"[0-9]+");
            var pagCount = int.Parse(regex.Match(lastPagePath).Value);


            for (int i = 1; i <= pagCount; i++)
            {
                parseArticles(hubPath + "page" + i);
            }


        }

        private static void parseArticles(string pagePath)
        {

            var root = inicializePage(pagePath);

            var articles = root.SelectNodes("//div[contains(@class,'posts shortcuts_items')] //a[@class = 'post_title']");
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
                        _posts.Add(postAdress, post);
                    }
                }

            }
        }

    }
}
