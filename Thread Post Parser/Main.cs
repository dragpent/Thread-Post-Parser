using System.Diagnostics;
using System.Windows.Forms;

namespace Thread_Post_Parser
{
    public partial class Main : Form
    {
        private readonly Settings settings = Settings.Load();
        private BrowserPageSource? browser;
        private CancellationTokenSource? cts;

        public Main()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // The designed size is the floor; anchored controls grow with anything larger.
            MinimumSize = Size;

            txtUrl.Text = settings.LastUrl ?? "";

            string? saved = settings.GetCookie();
            if (!string.IsNullOrEmpty(saved))
            {
                txtCookie.Text = saved;
                lbStatus.Text = "Saved cookie loaded. Paste a topic URL and hit Fetch All.";
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            browser?.Dispose();
            base.OnFormClosed(e);
        }

        private List<PostData> CurrentData => dgData.DataSource as List<PostData> ?? new List<PostData>();

        /// <summary>Boots the embedded browser on first use and seeds whatever cookie is in the box.</summary>
        private async Task<BrowserPageSource> GetBrowserAsync()
        {
            browser ??= new BrowserPageSource();
            await browser.InitializeAsync(txtCookie.Text);
            return browser;
        }

        private async void btnFetch_Click(object sender, EventArgs e)
        {
            if (!PostParser.TryParseTopicUrl(txtUrl.Text, out int topicId, out int forumId))
            {
                MessageBox.Show(
                    "That does not look like a d2jsp topic URL.\n\n" +
                    "Expected something like:\nhttps://forums.d2jsp.org/topic.php?t=110498378&f=104",
                    "Bad URL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            settings.LastUrl = txtUrl.Text.Trim();
            settings.SetCookie(chkRemember.Checked ? txtCookie.Text : null);
            settings.Save();

            cts = new CancellationTokenSource();
            SetBusy(true);

            try
            {
                lbStatus.Text = "Starting browser engine…";
                var engine = await GetBrowserAsync();

                // Load page one for real, so we can tell a Cloudflare check or a logged-out
                // session apart from "this thread simply has no posts" before the loop starts.
                lbStatus.Text = "Opening topic…";
                string first = await engine.NavigateAndReadAsync(
                    ThreadFetcher.BuildUrl(topicId, forumId, 0), cts.Token);

                if (BrowserPageSource.IsCloudflareChallenge(first))
                {
                    engine.Reveal();
                    MessageBox.Show(this,
                        "Cloudflare is showing a verification check.\n\n" +
                        "Complete it in the browser window that just opened, wait for the topic to appear, " +
                        "then click OK here.",
                        "Verification needed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    first = await engine.NavigateAndReadAsync(
                        ThreadFetcher.BuildUrl(topicId, forumId, 0), cts.Token);
                    engine.Conceal();
                }

                if (PostParser.IsAccessDenied(first) || PostParser.ParsePage(first).Count == 0)
                {
                    if (PostParser.IsAccessDenied(first))
                    {
                        engine.Reveal();
                        MessageBox.Show(this,
                            "d2jsp will not show this topic to the current session — the cookie is missing, " +
                            "wrong or expired.\n\n" +
                            "Either log in inside the browser window that just opened, or paste a fresh Cookie " +
                            "header from Chrome. Click OK here once the topic is visible in that window.",
                            "Not logged in", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        first = await engine.NavigateAndReadAsync(
                            ThreadFetcher.BuildUrl(topicId, forumId, 0), cts.Token);
                        engine.Conceal();

                        // A manual login leaves usable cookies behind; keep them for next time.
                        await RememberSessionCookiesAsync(engine);

                        if (PostParser.IsAccessDenied(first))
                        {
                            lbStatus.Text = "Access denied — still not logged in.";
                            return;
                        }
                    }
                }

                var progress = new Progress<string>(s => lbStatus.Text = s);
                var fetcher = new ThreadFetcher(engine);
                var result = await fetcher.FetchAllAsync(topicId, forumId, progress, cts.Token);

                dgData.DataSource = result.Posts;

                if (result.AccessDenied)
                {
                    lbStatus.Text = "Access denied — the session is not logged in.";
                    return;
                }

                int users = UserListWriter.Tally(result.Posts).Count;
                lbStatus.Text = result.Error == null
                    ? $"Done: {result.PagesFetched} pages, {result.Posts.Count} posts, {users} unique users."
                    : $"Stopped early: {result.Error} ({result.Posts.Count} posts kept)";

                if (result.Error != null)
                    MessageBox.Show(result.Error, "Fetch incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (OperationCanceledException)
            {
                lbStatus.Text = "Cancelled.";
            }
            catch (Exception ex)
            {
                lbStatus.Text = "Failed.";
                MessageBox.Show(ex.Message, "Fetch failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                browser?.Conceal();
                SetBusy(false);
                cts?.Dispose();
                cts = null;
            }
        }

        private async void btnSignIn_Click(object sender, EventArgs e)
        {
            SetBusy(true);
            try
            {
                var engine = await GetBrowserAsync();

                string target = PostParser.TryParseTopicUrl(txtUrl.Text, out int t, out int f)
                    ? ThreadFetcher.BuildUrl(t, f, 0)
                    : "https://forums.d2jsp.org/login.php";

                engine.Reveal();
                await engine.NavigateAndReadAsync(target, CancellationToken.None);

                MessageBox.Show(this,
                    "Log into d2jsp in the browser window, then click OK here.\n\n" +
                    "The session is kept in this app's own profile, so you should only need to do this once.",
                    "Sign in", MessageBoxButtons.OK, MessageBoxIcon.Information);

                await RememberSessionCookiesAsync(engine);
                engine.Conceal();

                lbStatus.Text = "Signed-in session saved. Hit Fetch All.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sign in failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task RememberSessionCookiesAsync(BrowserPageSource engine)
        {
            try
            {
                string header = await engine.GetCookieHeaderAsync();
                if (string.IsNullOrWhiteSpace(header)) return;

                txtCookie.Text = header;
                if (chkRemember.Checked)
                {
                    settings.SetCookie(header);
                    settings.Save();
                }
            }
            catch { /* the browser profile keeps the session anyway */ }
        }

        private void btnStop_Click(object sender, EventArgs e) => cts?.Cancel();

        private void SetBusy(bool busy)
        {
            btnFetch.Enabled = !busy;
            btnSignIn.Enabled = !busy;
            btnSave.Enabled = !busy;
            btnStop.Enabled = busy;
            UseWaitCursor = busy;
        }

        private void lbCookieHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(
                "In Chrome, while logged into d2jsp:\n\n" +
                "1. Open the topic.\n" +
                "2. Press F12, go to the Network tab.\n" +
                "3. Reload the page (F5).\n" +
                "4. Click the first request (topic.php).\n" +
                "5. Under Headers > Request Headers, find \"Cookie\".\n" +
                "6. Right-click it, Copy value, and paste it here.\n\n" +
                "The cookie is your live login session — it is stored encrypted under your Windows account " +
                "and only ever sent to forums.d2jsp.org. It will expire eventually; paste a fresh one, or use " +
                "\"Open browser / sign in\" instead.",
                "Getting the cookie", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var data = CurrentData;

            if (data.Count == 0)
            {
                MessageBox.Show("No data to save.");
                return;
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string label = PostParser.TryParseTopicUrl(txtUrl.Text, out int topicId, out _)
                ? $"topic_{topicId}"
                : "thread";

            var (uniquePath, multiPath) = UserListWriter.Write(data, desktopPath, label);

            var tally = UserListWriter.Tally(data);
            int repeats = tally.Count(u => u.Count > 1);

            MessageBox.Show(
                $"{tally.Count} unique users ({repeats} posted more than once).\n\n" +
                $"Saved:\n{uniquePath}\n{multiPath}",
                "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Process.Start(new ProcessStartInfo
            {
                FileName = uniquePath,
                UseShellExecute = true
            });
        }
    }
}
