using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using MongoDbApplication.Structure;
using System.Text.RegularExpressions;

namespace MongoDbApplication.Habr
{
    static class HabrPostParser
    {
        private static string basePath = "https://habrahabr.ru";
        private static IDictionary<string, User> _users;

        public static Post parsePost(string adress, IDictionary<string, User> users)
        {
            _users = users;
            try
            {
                var root = HabrParser.inicializePage(adress);

                string postTitle = root.Descendants("span")
                    .Where(x => x.GetAttributeValue("class", "").Equals("post_title"))
                    .Single().InnerText;

                List<string> postTags = new List<string>();
                var hubs = root.Descendants("a")
                    .Where(x => x.GetAttributeValue("class", "").Equals("hub "));
                foreach (var hub in hubs)
                {
                    postTags.Add(hub.InnerText);
                }

                string postContent = root.Descendants("div")
                    .Where(x => x.GetAttributeValue("class", "").Equals("content html_format"))
                    .Single().InnerHtml;

                var postAuthor = root.Descendants("a")
                    .Where(x => x.GetAttributeValue("class", "").Equals("author-info__nickname") ||
                        x.GetAttributeValue("class", "").Equals("post-type__value post-type__value_author"))
                    .First().InnerText.Substring(1);

                var authorRef = root.Descendants("a")
                    .Where(x => x.GetAttributeValue("class", "").Equals("author-info__nickname") ||
                        x.GetAttributeValue("class", "").Equals("post-type__value post-type__value_author"))
                    .First().GetAttributeValue("href", "");

                if (!_users.ContainsKey(postAuthor) && authorRef != "")
                {
                    _users.Add(postAuthor, getUserInfo(basePath + authorRef));
                }

                var dateStr = root.Descendants("div")
                    .Where(x => x.GetAttributeValue("class", "").Equals("published"))
                    .Single().InnerText;
                var postDate = toDate(dateStr);

                var commentsRoot = root.Descendants("ul")
                     .Where(x => x.GetAttributeValue("id", "").Equals("comments-list")).Single();
                IEnumerable<Comment> postComments = reqursiveCommentGet(commentsRoot, 0);

                Post post = new Post
                {
                    title = postTitle,
                    author = postAuthor,
                    content = postContent,
                    comments = postComments,
                    tags = postTags,
                    date = postDate,
                };
                return post;
           }
            catch (Exception ex)
            {
                Console.WriteLine("postParse error: " + ex.Message);
                return null;
            }
        }

        private static IEnumerable<Comment> reqursiveCommentGet(HtmlNode parent, int nestingLevel)
        {
            List<Comment> comments = new List<Comment>();
            try
            {
                var commentNodes = parent.ChildNodes
                    .Where(x => x.Name == "li" && x.GetAttributeValue("class", "").Equals("comment_item"));

                foreach (var item in commentNodes)
                {

                var authorSpan = item.Descendants("span")
                        .Where(x => x.GetAttributeValue("class", "").Equals("comment-item__user-info"))
                        .FirstOrDefault();

                string author;

                if (authorSpan == null)
                {
                    Console.WriteLine("НЛО");
                    continue;
                }
                else
                {
                    author = authorSpan.GetAttributeValue("data-user-login", "");

                    if (author == "") continue;

                    var authorRef = item.Descendants("a")
                        .Where(x => x.GetAttributeValue("class", "").Equals("comment-item__username"))
                        .First().GetAttributeValue("href", "");

                    if (!_users.ContainsKey(author) && authorRef != "")
                    {
                        _users.Add(author, getUserInfo(authorRef));
                    }
                }

                    var content = item.Descendants("div")
                            .Where(x => x.GetAttributeValue("class", "").Contains("message html_format "))
                            .First().InnerHtml;

                    var date = toDate(item.Descendants().Where(x => x.Name == "time").First().InnerText);

                    var replySection = item.Descendants("ul")
                            .Where(x => x.GetAttributeValue("class", "").Equals("reply_comments"))
                            .First();
                    IEnumerable<Comment> cmmnts = null;
                    if (replySection.HasChildNodes == true && replySection.ChildNodes.Count != 1)
                    {
                        cmmnts = reqursiveCommentGet(replySection, nestingLevel + 1);
                    }

                    Comment comment = new Comment
                    {
                        author = author,
                        content = content,
                        date = date,
                        nestingLevel = nestingLevel,
                    };
                    comments.Add(comment);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("reqursiveCommentGet error: " + ex.Message);
                return null;
            }
            return comments;
        }

        private static DateTime toDate(string str)
        {
            DateTime date;

            try
            {
                IFormatProvider culture = new System.Globalization.CultureInfo("ru-RU", true);
                var toCut = @"в\s+";
                var cutDate = @"((\d{1,2} [а-я]{3,8}( \d{4})?)|сегодня|вчера) в\s+\d{2}:\d{2}";
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
            }
            catch (FormatException ex)
            {
                throw new FormatException("Time parsing exception: " + ex.Message);
            }
            return date;
        }

        private static User getUserInfo(string userProfilePath)
        {
            User user;
            try {
                var root = HabrParser.inicializePage(userProfilePath);

                var userNick = root.Descendants("a")
                        .Where(x => x.GetAttributeValue("class", "").Equals("author-info__nickname"))
                        .First().InnerText.Substring(1);

                var nameNode = root.Descendants("a")
                        .Where(x => x.GetAttributeValue("class", "").Equals("author-info__name"))
                        .FirstOrDefault();

                var userName = nameNode == null ? null : nameNode.InnerText.Trim();

                
                var profileNode = root.Descendants("div")
                    .Where(x => x.GetAttributeValue("class", "").Equals("user_profile")).First();

                var registerRegex = @"Зарегистрирован";
                Regex regex = new Regex(registerRegex);

                var registerDateStrLst =
                        from dl in profileNode.ChildNodes
                        from dt in dl.Elements("dt")
                        where regex.IsMatch(dt.InnerText.Trim())
                        select dl;

                var registerDateStr = registerDateStrLst.First().Element("dd").InnerText;
                var userRegisterDate = toDate(registerDateStr);

                var fromCellLst =
                    from dl in profileNode.ChildNodes
                    from dt in dl.Elements("dt")
                    where dt.InnerText.Trim() == "Откуда:"
                    select dl;
                var fromCell = fromCellLst.FirstOrDefault();

                IList<string> userFrom = new List<string>();
                if (fromCell != null)
                {
                    var places = fromCell.Element("dd");
                    foreach (var item in places.Elements("a"))
                    {
                        userFrom.Add(item.InnerText.Trim());
                    }
                }
                user = new User
                {
                    nick = userNick,
                    registerDate = userRegisterDate,
                    name = userName,
                    from = userFrom.ToArray(),
                };
            }catch(Exception ex)
            {
                throw new ApplicationException("user info parsing exception: " + ex.Message);
            }
            return user;
        }
    }
}
