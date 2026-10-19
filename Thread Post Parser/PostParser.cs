using System.Text.RegularExpressions;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace Thread_Post_Parser
{
    /// <summary>
    /// Everything that knows how to read a d2jsp topic page.
    /// Shared by the manual HTML paste form and the automatic fetcher.
    /// </summary>
    public static class PostParser
    {
        /// <summary>Pulls every post out of a single topic page. Empty list = page has no posts.</summary>
        public static List<PostData> ParsePage(string html)
        {
            var results = new List<PostData>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var posts = doc.DocumentNode.SelectNodes("//div[contains(@class,'ppc')]");
            if (posts == null) return results;

            foreach (var post in posts)
            {
                // NAME
                var nameNode = post.SelectSingleNode(".//div[contains(@class,'pU')]//a[@name]");
                string? name = nameNode?
                    .ChildNodes
                    .Where(n => n.NodeType == HtmlNodeType.Text)
                    .Select(n => n.InnerText.Trim())
                    .FirstOrDefault();

                name = System.Net.WebUtility.HtmlDecode(name ?? "").Trim();

                // DATE
                var dateNode = post.SelectSingleNode(".//span[starts-with(@id,'td')]");
                string date = dateNode?.InnerText.Trim() ?? "";

                // POST CONTENT
                var postNode = post.SelectSingleNode(".//div[starts-with(@id,'tp')]");
                string content = "";

                if (postNode != null)
                {
                    // Remove ONLY quote headers (quote1), keep actual quoted text (quote2)
                    var quoteHeaders = postNode.SelectNodes(".//div[contains(@class,'quote1')]");
                    if (quoteHeaders != null)
                    {
                        foreach (var qh in quoteHeaders)
                            qh.Remove();
                    }

                    // Remove signatures
                    var sig = postNode.SelectSingleNode(".//div[contains(@class,'sig')]");
                    sig?.Remove();

                    // Convert <br> to newline
                    string htmlContent = postNode.InnerHtml
                        .Replace("<br>", "\n")
                        .Replace("<br/>", "\n")
                        .Replace("<br />", "\n");

                    // Decode HTML entities
                    htmlContent = System.Net.WebUtility.HtmlDecode(htmlContent);

                    // Strip remaining HTML
                    var tempDoc = new HtmlDocument();
                    tempDoc.LoadHtml(htmlContent);
                    content = tempDoc.DocumentNode.InnerText;

                    // Remove ^ symbols
                    content = content.Replace("^", "");

                    // Clean lines
                    content = string.Join("\n",
                        content.Split('\n')
                            .Select(l => l.Trim())
                            .Where(l => !string.IsNullOrWhiteSpace(l))
                    ).Trim();
                }

                if (!string.IsNullOrWhiteSpace(name) &&
                    !string.IsNullOrWhiteSpace(date))
                {
                    results.Add(new PostData
                    {
                        Date = date,
                        Name = name,
                        Post = content
                    });
                }
            }

            return results;
        }

        /// <summary>Drops posts that are byte-for-byte the same date+name+body.</summary>
        public static List<PostData> Deduplicate(IEnumerable<PostData> posts) =>
            posts.GroupBy(p => $"{p.Date}|{p.Name}|{p.Post}")
                 .Select(g => g.First())
                 .ToList();

        /// <summary>
        /// The posts-per-page step, read off the pager links rather than assumed.
        /// d2jsp exposes this as an account preference, so it is 10 for guests and 25 for most logged-in users.
        /// Returns null when the topic is a single page (no pager).
        /// </summary>
        public static int? ParsePageStep(string html)
        {
            var offsets = Regex.Matches(html, @"topic\.php\?t=\d+&(?:amp;)?f=\d+&(?:amp;)?o=(\d+)")
                               .Select(m => int.Parse(m.Groups[1].Value))
                               .Distinct()
                               .OrderBy(o => o)
                               .ToList();

            if (offsets.Count < 2) return null;

            // Smallest gap between adjacent page offsets is the page size.
            int step = offsets.Zip(offsets.Skip(1), (a, b) => b - a).Where(d => d > 0).DefaultIfEmpty(0).Min();
            return step > 0 ? step : null;
        }

        /// <summary>Reply count from the "Views: n Replies: n" strip, for progress reporting. Null if absent.</summary>
        public static int? ParseReplyCount(string html)
        {
            var m = Regex.Match(html, @"Replies:\s*([\d,]+)");
            if (!m.Success) return null;
            return int.TryParse(m.Groups[1].Value.Replace(",", ""), out int n) ? n : null;
        }

        /// <summary>True when d2jsp served the "you must be logged in / no permission" shell instead of the topic.</summary>
        public static bool IsAccessDenied(string html) =>
            html.Contains("You do not have permission", StringComparison.OrdinalIgnoreCase) ||
            html.Contains("Invalid Action", StringComparison.OrdinalIgnoreCase) ||
            html.Contains("You must be logged in", StringComparison.OrdinalIgnoreCase);

        /// <summary>Pulls the topic id and forum id out of any d2jsp topic URL.</summary>
        public static bool TryParseTopicUrl(string url, out int topicId, out int forumId)
        {
            topicId = 0;
            forumId = 0;
            if (string.IsNullOrWhiteSpace(url)) return false;

            var t = Regex.Match(url, @"[?&]t=(\d+)");
            var f = Regex.Match(url, @"[?&]f=(\d+)");

            if (!t.Success) return false;
            topicId = int.Parse(t.Groups[1].Value);
            if (f.Success) forumId = int.Parse(f.Groups[1].Value);
            return true;
        }
    }
}
