using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack; // for HtmlDecode

namespace Thread_Post_Parser
{
    public partial class Html : Form
    {
        private List<string> htmlPages = new List<string>();
        public List<PostData> ParsedPosts { get; private set; } = new List<PostData>();
        public Html()
        {
            InitializeComponent();
        }

        private void btnSaveAndAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(htmlTextBox.Text))
            {
                htmlPages.Add(htmlTextBox.Text);
                htmlTextBox.Clear();

                lbCount.Text = htmlPages.Count.ToString() + " html pages added.";
                lbCount.Visible = true;
            }
        }

        private void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            var results = new List<PostData>();

            foreach (var html in htmlPages)
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var posts = doc.DocumentNode.SelectNodes("//div[contains(@class,'ppc')]");
                if (posts == null) continue;

                foreach (var post in posts)
                {
                    // NAME
                    var nameNode = post.SelectSingleNode(".//div[contains(@class,'pU')]//a[@name]");
                    string name = nameNode?
                        .ChildNodes
                        .Where(n => n.NodeType == HtmlNodeType.Text)
                        .Select(n => n.InnerText.Trim())
                        .FirstOrDefault();

                    // DATE
                    var dateNode = post.SelectSingleNode(".//span[starts-with(@id,'td')]");
                    string date = dateNode?.InnerText.Trim();

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

                        // 🧹 Remove signatures
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
                        var tempDoc = new HtmlAgilityPack.HtmlDocument();
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
                        !string.IsNullOrWhiteSpace(date) &&
                        !string.IsNullOrWhiteSpace(content))
                    {
                        results.Add(new PostData
                        {
                            Date = date,
                            Name = name,
                            Post = content
                        });
                    }
                }
            }

            // Deduplicate
            ParsedPosts = results
                .GroupBy(p => $"{p.Date}|{p.Name}|{p.Post}")
                .Select(g => g.First())
                .ToList();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
