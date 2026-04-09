using System.Diagnostics;
using System.Windows.Forms;

namespace Thread_Post_Parser
{
    public partial class Main : Form
    {
        private List<string> htmlPages = new List<string>();

        public Main()
        {
            InitializeComponent();
        }

        private void btnCopyHtml_Click(object sender, EventArgs e)
        {
            using (var form = new Html())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var data = form.ParsedPosts;

                    // Bind to DataGridView
                    dgData.DataSource = data;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dgData.DataSource == null)
            {
                MessageBox.Show("No data to save.");
                return;
            }

            // Get data from DataGridView
            var data = dgData.DataSource as List<PostData>;

            if (data == null || data.Count == 0)
            {
                MessageBox.Show("No data to save.");
                return;
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            string filePath = Path.Combine(desktopPath, $"output_{timestamp}.csv");

            // Helper to escape CSV fields
            string Escape(string s)
            {
                if (string.IsNullOrEmpty(s)) return "\"\"";

                s = s.Replace("\"", "\"\""); // escape quotes
                return $"\"{s}\"";           // wrap in quotes
            }

            // Build CSV
            var lines = new List<string>();

            // Header
            lines.Add("Date,Name,Post");

            // Rows
            lines.AddRange(data.Select(p =>
                $"{Escape(p.Date)},{Escape(p.Name)},{Escape(p.Post)}"
            ));

            File.WriteAllLines(filePath, lines);

            // Open file
            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            });

            MessageBox.Show($"Saved to:\n{filePath}");
        }
    }
}
