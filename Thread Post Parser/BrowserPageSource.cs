using System.Text.Json;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Thread_Post_Parser
{
    /// <summary>
    /// Fetches topic pages through an embedded Chromium (WebView2).
    /// Cloudflare fingerprints the TLS handshake, so anything built on Schannel - HttpClient,
    /// WebClient, even Windows' own curl.exe - gets a 403 challenge page no matter what headers
    /// or cookies it sends. A real browser engine is the only thing that gets through.
    ///
    /// The window stays hidden for normal runs and is only shown when a human has to step in
    /// (Cloudflare interstitial, or a login).
    /// </summary>
    public class BrowserPageSource : Form, IPageSource
    {
        private readonly WebView2 web = new();
        private TaskCompletionSource<string>? pendingFetch;
        private bool ready;

        private static string ProfileDir => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ThreadPostParser", "WebView2");

        public BrowserPageSource()
        {
            Text = "d2jsp — sign in / verify, then close this window";
            ClientSize = new Size(1000, 760);
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = false;

            web.Dock = DockStyle.Fill;
            Controls.Add(web);

            // Created but kept off-screen so the control gets a handle without flashing at the user.
            Opacity = 0;
            Location = new Point(-32000, -32000);
        }

        /// <summary>Boots Chromium against a persistent profile and seeds any pasted cookie.</summary>
        public async Task InitializeAsync(string? cookieHeader)
        {
            if (!ready)
            {
                Show();                       // needed for the handle; still invisible (Opacity 0, off-screen)
                Directory.CreateDirectory(ProfileDir);

                var env = await CoreWebView2Environment.CreateAsync(null, ProfileDir);
                await web.EnsureCoreWebView2Async(env);

                var core = web.CoreWebView2;
                core.Settings.UserAgent = ThreadFetcher.DefaultUserAgent;
                core.Settings.AreDefaultContextMenusEnabled = true;
                core.WebMessageReceived += OnWebMessage;

                ready = true;
            }

            if (!string.IsNullOrWhiteSpace(cookieHeader))
                ApplyCookieHeader(cookieHeader);
        }

        /// <summary>Splits a pasted "a=1; b=2" Cookie header into the browser's own cookie jar.</summary>
        private void ApplyCookieHeader(string cookieHeader)
        {
            var mgr = web.CoreWebView2.CookieManager;

            foreach (var part in cookieHeader.Split(';'))
            {
                int eq = part.IndexOf('=');
                if (eq <= 0) continue;

                string name = part[..eq].Trim();
                string value = part[(eq + 1)..].Trim();
                if (name.Length == 0) continue;

                var cookie = mgr.CreateCookie(name, value, "forums.d2jsp.org", "/");
                cookie.IsSecure = true;
                cookie.Expires = DateTime.Now.AddDays(30);
                mgr.AddOrUpdateCookie(cookie);
            }
        }

        private void OnWebMessage(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try { pendingFetch?.TrySetResult(e.TryGetWebMessageAsString() ?? ""); }
            catch { pendingFetch?.TrySetResult(""); }
        }

        /// <summary>Loads a URL as a real navigation and hands back the rendered DOM.</summary>
        public async Task<string> NavigateAndReadAsync(string url, CancellationToken token)
        {
            var done = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            void Handler(object? s, CoreWebView2NavigationCompletedEventArgs e) => done.TrySetResult(true);

            web.CoreWebView2.NavigationCompleted += Handler;
            try
            {
                web.CoreWebView2.Navigate(url);
                using var reg = token.Register(() => done.TrySetCanceled());
                await done.Task.WaitAsync(TimeSpan.FromSeconds(60), token);
            }
            finally
            {
                web.CoreWebView2.NavigationCompleted -= Handler;
            }

            await Task.Delay(250, token); // let any late DOM work settle
            return await ReadDomAsync();
        }

        private async Task<string> ReadDomAsync()
        {
            string json = await web.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML");
            return JsonSerializer.Deserialize<string>(json) ?? "";
        }

        /// <summary>
        /// Pulls a page with an in-page fetch() instead of a navigation: same cookies, same TLS,
        /// but none of the image and script loading a full page render would do.
        /// </summary>
        public async Task<string> GetAsync(string url, CancellationToken token)
        {
            if (!ready) throw new InvalidOperationException("Browser engine not initialized.");

            // The first page has to be a real navigation so the in-page fetches are same-origin.
            if (web.Source == null || !web.Source.Host.Contains("d2jsp.org", StringComparison.OrdinalIgnoreCase))
                return await NavigateAndReadAsync(url, token);

            pendingFetch = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            string js = $@"
                fetch({JsonSerializer.Serialize(url)}, {{ credentials: 'include' }})
                    .then(r => r.text())
                    .then(t => window.chrome.webview.postMessage(t))
                    .catch(e => window.chrome.webview.postMessage('__FETCH_ERROR__' + e));";

            await web.CoreWebView2.ExecuteScriptAsync(js);

            using var reg = token.Register(() => pendingFetch.TrySetCanceled());
            string html = await pendingFetch.Task.WaitAsync(TimeSpan.FromSeconds(45), token);
            pendingFetch = null;

            if (html.StartsWith("__FETCH_ERROR__", StringComparison.Ordinal))
                throw new HttpRequestException(html["__FETCH_ERROR__".Length..]);

            return html;
        }

        public static bool IsCloudflareChallenge(string html) =>
            html.Contains("Just a moment", StringComparison.OrdinalIgnoreCase) ||
            html.Contains("cf-browser-verification", StringComparison.OrdinalIgnoreCase) ||
            html.Contains("challenge-platform", StringComparison.OrdinalIgnoreCase);

        /// <summary>Brings the hidden browser on screen so the user can log in or clear a challenge.</summary>
        public void Reveal()
        {
            Opacity = 1;
            ShowInTaskbar = true;
            StartPosition = FormStartPosition.CenterScreen;
            Location = new Point(
                Math.Max(0, (Screen.PrimaryScreen!.WorkingArea.Width - Width) / 2),
                Math.Max(0, (Screen.PrimaryScreen!.WorkingArea.Height - Height) / 2));
            Show();
            BringToFront();
            Activate();
        }

        public void Conceal()
        {
            Opacity = 0;
            ShowInTaskbar = false;
            Location = new Point(-32000, -32000);
        }

        /// <summary>Current cookies for the forum, as a Cookie header - so a manual login can be remembered.</summary>
        public async Task<string> GetCookieHeaderAsync()
        {
            var cookies = await web.CoreWebView2.CookieManager.GetCookiesAsync("https://forums.d2jsp.org/");
            return string.Join("; ", cookies.Select(c => $"{c.Name}={c.Value}"));
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // The user closing the sign-in window should only hide it; Main owns the lifetime.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Conceal();
                return;
            }
            base.OnFormClosing(e);
        }
    }
}
