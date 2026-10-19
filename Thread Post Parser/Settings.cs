using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Thread_Post_Parser
{
    /// <summary>
    /// Remembers the pasted cookie between runs. The cookie is a live session credential,
    /// so it is encrypted with DPAPI under the current Windows user and never written in the clear.
    /// </summary>
    public class Settings
    {
        public string? ProtectedCookie { get; set; }
        public string? UserAgent { get; set; }
        public string? LastUrl { get; set; }

        private static string FilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ThreadPostParser", "settings.json");

        public static Settings Load()
        {
            try
            {
                if (File.Exists(FilePath))
                    return JsonSerializer.Deserialize<Settings>(File.ReadAllText(FilePath)) ?? new Settings();
            }
            catch { /* corrupt or unreadable settings are not worth bothering the user about */ }

            return new Settings();
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
                File.WriteAllText(FilePath, JsonSerializer.Serialize(this,
                    new JsonSerializerOptions { WriteIndented = true }));
            }
            catch { /* ignore - losing the remembered cookie just means pasting it again */ }
        }

        public static void Delete()
        {
            try { if (File.Exists(FilePath)) File.Delete(FilePath); } catch { }
        }

        public string? GetCookie()
        {
            if (string.IsNullOrEmpty(ProtectedCookie)) return null;
            try
            {
                byte[] bytes = ProtectedData.Unprotect(
                    Convert.FromBase64String(ProtectedCookie), null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(bytes);
            }
            catch { return null; }
        }

        public void SetCookie(string? cookie)
        {
            if (string.IsNullOrWhiteSpace(cookie))
            {
                ProtectedCookie = null;
                return;
            }

            byte[] bytes = ProtectedData.Protect(
                Encoding.UTF8.GetBytes(cookie.Trim()), null, DataProtectionScope.CurrentUser);
            ProtectedCookie = Convert.ToBase64String(bytes);
        }
    }
}
