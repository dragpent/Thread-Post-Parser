# Thread Post Parser

A small Windows desktop app that walks a [d2jsp](https://forums.d2jsp.org) forum topic from start to
finish, collects every post, and writes out who posted — deduplicated, in thread order.

Point it at a topic URL, hit **Fetch All**, and it pages through the whole thread on its own.

## What it produces

**Save user lists** writes two timestamped files to your Desktop:

| File | Contents |
| --- | --- |
| `topic_<id>_<stamp>_unique-users.txt` | One name per line, deduplicated |
| `topic_<id>_<stamp>_multi-posters.txt` | `name<TAB>count` for anyone who posted more than once |

`unique-users.txt` is ordered by **where each person first appears in the thread**, not alphabetically,
so you can read the file and the thread top-to-bottom together and check names off as you go.
`multi-posters.txt` is ordered by post count, highest first, since its job is spotting repeat posters.

Names are matched case-insensitively (d2jsp names are unique regardless of case) and the spelling from
the user's first post is the one kept.

## Why it embeds a browser

forums.d2jsp.org sits behind Cloudflare, which fingerprints the **TLS handshake** rather than just
inspecting headers. Every HTTP stack on Windows is built on Schannel, so `HttpClient`, `WebClient` and
even Windows' own bundled `curl.exe` all get a `403` "Just a moment..." challenge page back — no
combination of user-agent, headers or cookies changes that.

So the app fetches through an embedded **WebView2** (real Chromium) instead. Page one is a genuine
navigation; the remaining pages are pulled with an in-page `fetch()` on the same origin, which reuses
the browser's cookies and TLS without re-rendering images and scripts for every page.

The browser window stays hidden during normal runs. It is only shown when a human actually has to do
something — clear a Cloudflare interstitial, or log in.

## Getting access

Members-only sections (anything that answers *"You do not have permission to view this topic"* when
logged out) need a signed-in session. Two ways:

1. **Open browser / sign in** — log into d2jsp once in the window that appears. The app keeps its own
   persistent Chromium profile, so the session survives restarts. This is the durable option.
2. **Paste a cookie** — in Chrome: `F12` → Network → reload → click the `topic.php` request →
   Request Headers → right-click `Cookie` → Copy value → paste into the Cookie box. Expires eventually
   and has to be re-pasted.

Cloudflare's own `cf_clearance` and `__cf_bm` cookies are not worth copying: they are bound to Chrome's
TLS fingerprint and your IP, and the embedded browser clears Cloudflare without them.

With **Remember** ticked, the cookie is stored in
`%LOCALAPPDATA%\ThreadPostParser\settings.json`, encrypted with DPAPI under your Windows account —
never in plain text. The browser profile lives in `%LOCALAPPDATA%\ThreadPostParser\WebView2`.

## How pagination works

d2jsp pages a topic with an `&o=<offset>` parameter:

```
topic.php?t=110498378&f=104          first page
topic.php?t=110498378&f=104&o=25     second page
topic.php?t=110498378&f=104&o=250    past the end — HTTP 200, full page shell, zero posts
```

Running off the end returns a normal `200` page with no post blocks in it, so **"stop when a page
yields zero posts"** is the termination rule.

The page size is **not** a fixed 25. It is a per-account preference — logged out, the same thread pages
by 10. The app reads the real step off the pager links (`div.pg`) on page one instead of assuming, and
falls back to probing if a thread has no pager at all. The reply count in the page header is used for
the progress readout.

Requests are spaced 600 ms apart, with a hard ceiling of 400 pages so a parsing surprise can't loop
forever. Posts are deduplicated on `date + name + body`, so overlapping pages can't inflate counts.

## Building

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download) and the
[WebView2 Runtime](https://developer.microsoft.com/microsoft-edge/webview2/) (already present on most
Windows 10/11 installs — Edge ships it).

```
dotnet build
dotnet run
```

Dependencies: `HtmlAgilityPack`, `Microsoft.Web.WebView2`, `System.Security.Cryptography.ProtectedData`.

## Layout

| File | Role |
| --- | --- |
| `Main.cs` / `Main.Designer.cs` | The window: URL, cookie, fetch, grid, save |
| `BrowserPageSource.cs` | WebView2 fetch engine, cookie seeding, sign-in window |
| `ThreadFetcher.cs` | Pagination loop and the stop condition |
| `PostParser.cs` | Topic-page HTML → posts; pager, reply count, URL parsing |
| `UserListWriter.cs` | Tallying and the two output files |
| `Settings.cs` | DPAPI-encrypted cookie, last URL |

`ThreadFetcher` talks to an `IPageSource` interface rather than to WebView2 directly, so the pagination
logic can be driven from saved HTML in a test harness without a browser.

## Troubleshooting

**"Access denied — still not logged in."** The session isn't valid for that forum. Use
*Open browser / sign in*, or paste a fresh cookie.

**A Cloudflare check appears.** The browser window opens on its own; complete the check, wait for the
topic, then click OK. It is normally a one-off per profile.

**Fewer posts than expected.** Check the status line for the detected page step. If a thread uses an
unusual pager layout, the step detection is the first place to look.
