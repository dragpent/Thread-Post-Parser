namespace Thread_Post_Parser
{
    public class UserTally
    {
        public string Name { get; set; } = "";
        public int Count { get; set; }
    }

    /// <summary>Turns the parsed posts into the two deliverables: every unique poster, and the repeat posters.</summary>
    public static class UserListWriter
    {
        /// <summary>
        /// Groups posters case-insensitively (d2jsp names are unique regardless of case)
        /// and keeps the spelling of the first post seen.
        /// </summary>
        public static List<UserTally> Tally(IEnumerable<PostData> posts) =>
            posts.Where(p => !string.IsNullOrWhiteSpace(p.Name))
                 .GroupBy(p => p.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                 .Select(g => new UserTally { Name = g.First().Name.Trim(), Count = g.Count() })
                 .OrderBy(u => u.Name, StringComparer.OrdinalIgnoreCase)
                 .ToList();

        /// <summary>
        /// Writes unique-users and multi-posters next to each other, timestamped.
        /// Returns the two paths written.
        /// </summary>
        public static (string uniquePath, string multiPath) Write(
            IEnumerable<PostData> posts, string directory, string label)
        {
            var tally = Tally(posts);
            string stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            string uniquePath = Path.Combine(directory, $"{label}_{stamp}_unique-users.txt");
            string multiPath = Path.Combine(directory, $"{label}_{stamp}_multi-posters.txt");

            File.WriteAllLines(uniquePath, tally.Select(u => u.Name));

            var repeats = tally.Where(u => u.Count > 1)
                               .OrderByDescending(u => u.Count)
                               .ThenBy(u => u.Name, StringComparer.OrdinalIgnoreCase)
                               .Select(u => $"{u.Name}\t{u.Count}");

            File.WriteAllLines(multiPath, repeats);

            return (uniquePath, multiPath);
        }
    }
}
