using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BepInEx;

namespace RayMenu.Console
{
    public static class ConsoleData
    {
        private static readonly object Gate = new object();
        private static readonly List<ConsoleUser> Users = new List<ConsoleUser>();

        public static string Folder => Path.Combine(Paths.ConfigPath, "RayMenu");
        public static string FilePath => Path.Combine(Folder, "console.txt");

        public static IReadOnlyList<ConsoleUser> All
        {
            get
            {
                lock (Gate)
                {
                    return Users.ToArray();
                }
            }
        }

        public static int Count
        {
            get
            {
                lock (Gate)
                {
                    return Users.Count;
                }
            }
        }

        public static ConsoleRole RoleFor(string playFabId)
        {
            if (string.IsNullOrEmpty(playFabId)) return ConsoleRole.None;

            lock (Gate)
            {
                for (int i = 0; i < Users.Count; i++)
                {
                    if (Users[i].PlayFabId == playFabId) return Users[i].Role;
                }
            }
            return ConsoleRole.None;
        }

        public static string NameFor(string playFabId)
        {
            if (string.IsNullOrEmpty(playFabId)) return string.Empty;

            lock (Gate)
            {
                for (int i = 0; i < Users.Count; i++)
                {
                    if (Users[i].PlayFabId == playFabId) return Users[i].Name;
                }
            }
            return string.Empty;
        }

        public static void Load()
        {
            var parsed = new List<ConsoleUser>();

            try
            {
                if (File.Exists(FilePath))
                {
                    foreach (string raw in File.ReadAllLines(FilePath))
                    {
                        string line = raw.Trim();
                        if (line.Length == 0 || line.StartsWith("#")) continue;

                        string[] parts = line.Split(
                            new[] { ' ', '\t', ',', '=' },
                            StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 2) continue;

                        if (!Enum.TryParse(parts[0], true, out ConsoleRole role)) continue;
                        if (role == ConsoleRole.None) continue;

                        var user = new ConsoleUser
                        {
                            Role = role,
                            PlayFabId = parts[1]
                        };
                        if (parts.Length > 2) user.Name = parts[2];
                        parsed.Add(user);
                    }
                }
                else
                {
                    SaveTemplate();
                }
            }
            catch
            {
            }

            lock (Gate)
            {
                Users.Clear();
                Users.AddRange(parsed);
            }
        }

        public static void RefreshFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return;

            Task.Run(() =>
            {
                try
                {
                    string text;
                    using (var http = new HttpClient())
                    {
                        http.Timeout = TimeSpan.FromSeconds(10);
                        text = http.GetStringAsync(url).GetAwaiter().GetResult();
                    }

                    System.IO.Directory.CreateDirectory(Folder);
                    File.WriteAllText(FilePath, text);
                    Load();
                }
                catch
                {
                }
            });
        }

        private static void SaveTemplate()
        {
            try
            {
                System.IO.Directory.CreateDirectory(Folder);
                File.WriteAllText(
                    FilePath,
                    "# Ray Menu console access" + Environment.NewLine +
                    "# Format: <Role> <PlayFabId> [Name]" + Environment.NewLine +
                    "# Roles: Admin, SuperAdmin" + Environment.NewLine +
                    Environment.NewLine +
                    "# SuperAdmin 0000000000000000 SilverX" + Environment.NewLine +
                    "# Admin 0000000000000000 ExampleUser" + Environment.NewLine);
            }
            catch
            {
            }
        }
    }
}
