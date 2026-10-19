namespace Thread_Post_Parser
{
    public class FetchResult
    {
        public List<PostData> Posts { get; set; } = new();
        public int PagesFetched { get; set; }
        public bool AccessDenied { get; set; }
        public string? Error { get; set; }
    }

    /// <summary>
    /// Where topic HTML comes from. In the app this is a real Chromium engine (see BrowserPageSource):
    /// forums.d2jsp.org sits behind Cloudflare, which rejects .NET's and Schannel curl's TLS fingerprint
    /// outright, so a plain HttpClient only ever gets the "Just a moment..." challenge page back.
    /// </summary>
    public interface IPageSource
    {
        Task<string> GetAsync(string url, CancellationToken token);
    }

    /// <summary>
    /// Walks a d2jsp topic page by page until a page comes back with no posts on it.
    /// </summary>
    public class ThreadFetcher
    {
        public const string DefaultUserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36";

        private const int DefaultStep = 25;
        private const int MaxPages = 400;   // ~10k posts, a hard stop so a parsing surprise cannot loop forever
        private const int DelayMs = 600;    // be polite to the forum

        private readonly IPageSource source;

        public ThreadFetcher(IPageSource source) => this.source = source;

        public static string BuildUrl(int topicId, int forumId, int offset) =>
            $"https://forums.d2jsp.org/topic.php?t={topicId}" +
            (forumId > 0 ? $"&f={forumId}" : "") +
            (offset > 0 ? $"&o={offset}" : "");

        public async Task<FetchResult> FetchAllAsync(
            int topicId, int forumId, IProgress<string>? progress, CancellationToken token)
        {
            var result = new FetchResult();
            var all = new List<PostData>();
            int step = DefaultStep;
            int? totalPosts = null;

            for (int page = 0; page < MaxPages; page++)
            {
                token.ThrowIfCancellationRequested();

                int offset = page * step;
                string url = BuildUrl(topicId, forumId, offset);

                string html;
                try
                {
                    html = await source.GetAsync(url, token);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    result.Error = $"Request failed at offset {offset}: {ex.Message}";
                    break;
                }

                var posts = PostParser.ParsePage(html);

                if (posts.Count == 0)
                {
                    // First page empty and the shell says no access -> it is the session, not the end of the thread.
                    if (page == 0 && PostParser.IsAccessDenied(html))
                    {
                        result.AccessDenied = true;
                        result.Error = "d2jsp refused the request: not logged in, or the cookie has expired.";
                    }
                    break; // ran off the end of the thread
                }

                // Page one tells us the real page size and how long the thread is.
                if (page == 0)
                {
                    step = PostParser.ParsePageStep(html) ?? posts.Count;
                    if (step <= 0) step = DefaultStep;

                    int? replies = PostParser.ParseReplyCount(html);
                    if (replies.HasValue) totalPosts = replies.Value + 1;
                }

                all.AddRange(posts);
                result.PagesFetched = page + 1;

                progress?.Report(totalPosts.HasValue
                    ? $"Page {page + 1} (offset {offset}): {posts.Count} posts — {all.Count} of ~{totalPosts} collected"
                    : $"Page {page + 1} (offset {offset}): {posts.Count} posts — {all.Count} collected");

                await Task.Delay(DelayMs, token);
            }

            result.Posts = PostParser.Deduplicate(all);
            return result;
        }
    }
}
