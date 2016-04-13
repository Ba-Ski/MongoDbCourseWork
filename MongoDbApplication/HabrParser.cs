using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;
using MongoDbApplication.Structure;
using System.Text.RegularExpressions;

namespace MongoDbApplication
{

    static class HabrParser
    {
        private static string PATH = "https://habrahabr.ru/interesting/page";
        private static int PAGE_NUMBER = /*100*/ 3;

        public static IEnumerable<Post> getPosts()
        {
            List<Post> posts = new List<Post>();

            for (int i = 1; i <= PAGE_NUMBER; i++)
            {
                var html = new HtmlDocument();
                html.LoadHtml(new WebClient().DownloadString(PATH + i));
                var root = html.DocumentNode;
                if (root == null) throw new ApplicationException("page loading error");

                var articles = root.Descendants().Where(
                    x => x.Name == "div" && x.Attributes.Contains("class") && x.Attributes["class"].Value.Contains("shortcuts_item")
                    && x.Attributes["class"].Value.Contains("post "));

                foreach (var item in articles)
                {
                    string postAdress;

                    try
                    {
                       postAdress = item.Descendants()
                        .Where(x => x.Name == "a" && x.GetAttributeValue("class", "")
                        .Equals("post_title")).Single().GetAttributeValue("href", null);
                    }
                    catch (Exception ex)
                    {
                        postAdress = null;
                    }
                    
                    if (postAdress == null) throw new ApplicationException("article preview doesn't have href");

                    var post = parsePost(postAdress);

                    if (post != null)
                    {
                        posts.Add(post);
                    }
                    
                }
            }

            return posts;

        }
        private static Post parsePost(string adress)
        {
            try {
                var html = new HtmlDocument();
                var webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                html.LoadHtml(webClient.DownloadString(adress));
                var root = html.DocumentNode;
                if (root == null) throw new ApplicationException("page loading error");

                string postTitle = root.Descendants()
                    .Where(x => x.Name == "span" && x.GetAttributeValue("class", "").Equals("post_title"))
                    .Single().InnerText;

                List<string> postTags = new List<string>();
                var hubs = root.Descendants().Where(x => x.Name == "a" && x.GetAttributeValue("class", "").Equals("hub "));
                foreach (var hub in hubs)
                {
                    postTags.Add(hub.InnerText);
                }

                string postContent = root.Descendants()
                    .Where(x => x.Name == "div" && x.GetAttributeValue("class", "").Equals("content html_format"))
                    .Single().InnerHtml;

                var postAuthor = root.Descendants()
                    .Where(x => x.Name == "a" && ( x.GetAttributeValue("class", "").Equals("author-info__nickname") ||
                        x.GetAttributeValue("class", "").Equals("post-type__value post-type__value_author")))
                    .First().InnerText.Substring(1);

                var dateStr = root.Descendants()
                    .Where(x => x.Name == "div" && x.GetAttributeValue("class", "").Equals("published"))
                    .Single().InnerText;
                var postDate = toDate(dateStr);

                var commentsRoot = root.Descendants()
                     .Where(x => x.Name == "ul" && x.GetAttributeValue("id", "").Equals("comments-list")).Single();
                IEnumerable<Comment> postComments = reqursiveCommentGet(commentsRoot);

                Post post = new Post
                {
                    title    = postTitle,
                    author   = postAuthor,
                    content  = postContent,
                    comments = postComments,
                    tags     = postTags,
                    date     = postDate,
                };
                return post;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private static IEnumerable<Comment> reqursiveCommentGet(HtmlNode parent)
        {
            List<Comment> comments = new List<Comment>();
            try {
                var commentNodes = parent.ChildNodes
                    .Where(x => x.Name == "li" && x.GetAttributeValue("class", "").Equals("comment_item"));

                foreach (var item in commentNodes)
                {

                    var author = item.Descendants()
                            .Where(x => x.Name == "span" && x.GetAttributeValue("class", "").Equals("comment-item__user-info"))
                            .First().GetAttributeValue("data-user-login", "");
                    if (author == "") continue;

                    var content = item.Descendants()
                            .Where(x => x.Name == "div" && x.GetAttributeValue("class", "").Contains("message html_format "))
                            .First().InnerHtml;

                    var date = toDate(item.Descendants().Where(x => x.Name == "time").First().InnerText);

                    var replySection = item.Descendants()
                            .Where(x => x.Name == "ul" && x.GetAttributeValue("class", "").Equals("reply_comments"))
                            .First();
                    IEnumerable<Comment> cmmnts = null;
                    if (replySection.HasChildNodes == true && replySection.ChildNodes.Count != 1)
                    {
                        cmmnts = reqursiveCommentGet(replySection);
                    }
                    
                    Comment comment = new Comment
                    {
                        author = author,
                        content = content,
                        date = date,
                        comments = cmmnts,
                    };
                    comments.Add(comment);
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return comments;
        }

        private static DateTime toDate(string str)
        {
            DateTime date;

            try {
                IFormatProvider culture = new System.Globalization.CultureInfo("ru-RU", true);
                var toCut = @"в \b";
                var cutDate = @"((\d{1,2} [а-я]{3,8}( \d{4})?)|сегодня|вчера) в \d{2}:\d{2}";
                var dateStr = Regex.Match(str, cutDate, RegexOptions.IgnoreCase).Value;
                dateStr = Regex.Replace(dateStr, toCut, "", RegexOptions.IgnoreCase).Trim();
                string[] words = dateStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch (words[0])
                {
                    case "сегодня":
                        dateStr = DateTime.Today.ToString("d") + " " + words[words.Length - 1];
                        date = DateTime.Parse(dateStr);
                        break;
                    case "вчера":
                        dateStr = DateTime.Today.AddDays(-1).ToString("d") + " " + words[words.Length - 1];
                        date = DateTime.Parse(dateStr);
                        break;
                    default:
                        if (Regex.IsMatch(words[2], @"\d{4}"))
                        {
                            date = DateTime.ParseExact(dateStr, "d MMMM yyyy H:mm", culture);
                        }
                        else
                        {
                            dateStr = words[0] + " " + words[1] + " " + DateTime.Today.ToString("yyyy") + " " + words[words.Length - 1];
                            date = DateTime.Parse(dateStr);

                        }

                        break;

                }
            }catch(FormatException ex)
            {
                throw new FormatException("Time parsing exception: " + ex.Message);
            }
            return date;
        }
    }
}
