using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // The designed size is the floor; anchored controls grow with anything larger.
            MinimumSize = Size;
        }

        private void btnSaveAndAdd_Click(object sender, EventArgs e)
        {
            StashCurrentPage();
        }

        private void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            StashCurrentPage();

            var results = new List<PostData>();
            foreach (var html in htmlPages)
                results.AddRange(PostParser.ParsePage(html));

            ParsedPosts = PostParser.Deduplicate(results);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void StashCurrentPage()
        {
            if (string.IsNullOrWhiteSpace(htmlTextBox.Text)) return;

            htmlPages.Add(htmlTextBox.Text);
            htmlTextBox.Clear();

            lbCount.Text = htmlPages.Count.ToString() + " html pages added.";
            lbCount.Visible = true;
        }
    }
}
