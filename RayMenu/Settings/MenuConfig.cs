using System;
using System.Globalization;
using System.IO;
using System.Text;
using BepInEx;
using UnityEngine;

namespace RayMenu.Settings
{
    public static class MenuConfig
    {
        public static string Title = "RAY MENU";
        public static string Subtitle = "V1.0.0";
        public static string CreditName = "SilverX";
        public static string GitHubUrl = "https://github.com/SilverXdamodder";
        public static int DefaultTheme = 0;
        public static float DefaultScale = 1f;
        public static float DefaultOpacity = 0.95f;
        public static bool DefaultSounds = true;
        public static KeyCode DefaultPcKey = KeyCode.Q;
        public static int CornerRadius = 8;
        public static bool EnableCreditsPage = true;
        public static bool EnableAssetsPage = true;
        public static string BundlesFolder = string.Empty;
        public static bool EnableConsole = true;
        public static string ServerDataUrl = "https://silverxserverdata.rf.gd/?i=1";
        public static string LocalPlayFabId = string.Empty;

        public static string Directory => Path.Combine(Paths.ConfigPath, "RayMenu");
        public static string FilePath => Path.Combine(Directory, "settings.ini");

        public static void Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    Save();
                    return;
                }

                foreach (string raw in File.ReadAllLines(FilePath))
                {
                    string line = raw.Trim();
                    if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";")) continue;

                    int split = line.IndexOf('=');
                    if (split <= 0) continue;

                    string key = line.Substring(0, split).Trim();
                    string value = line.Substring(split + 1).Trim();
                    Apply(key, value);
                }
            }
            catch
            {
            }
        }

        private static void Apply(string key, string value)
        {
            switch (key.ToLowerInvariant())
            {
                case "title":
                    Title = value;
                    break;
                case "subtitle":
                    Subtitle = value;
                    break;
                case "creditname":
                    CreditName = value;
                    break;
                case "githuburl":
                    GitHubUrl = value;
                    break;
                case "defaulttheme":
                    if (int.TryParse(value, out int theme)) DefaultTheme = theme;
                    break;
                case "defaultscale":
                    if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float scale)) DefaultScale = scale;
                    break;
                case "defaultopacity":
                    if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float opacity)) DefaultOpacity = opacity;
                    break;
                case "defaultsounds":
                    if (bool.TryParse(value, out bool sounds)) DefaultSounds = sounds;
                    break;
                case "defaultpckey":
                    if (Enum.TryParse(value, true, out KeyCode key2)) DefaultPcKey = key2;
                    break;
                case "cornerradius":
                    if (int.TryParse(value, out int radius)) CornerRadius = Mathf.Clamp(radius, 0, 24);
                    break;
                case "enablecreditspage":
                    if (bool.TryParse(value, out bool credits)) EnableCreditsPage = credits;
                    break;
                case "enableassetspage":
                    if (bool.TryParse(value, out bool assets)) EnableAssetsPage = assets;
                    break;
                case "bundlesfolder":
                    BundlesFolder = value;
                    break;
                case "enableconsole":
                    if (bool.TryParse(value, out bool console)) EnableConsole = console;
                    break;
                case "serverdataurl":
                    ServerDataUrl = value;
                    break;
                case "localplayfabid":
                    LocalPlayFabId = value;
                    break;
            }
        }

        public static void Save()
        {
            try
            {
                System.IO.Directory.CreateDirectory(Directory);
                File.WriteAllText(FilePath, BuildText());
            }
            catch
            {
            }
        }

        private static string BuildText()
        {
            var builder = new StringBuilder();
            builder.AppendLine("# Ray Menu configuration");
            builder.AppendLine("# Edit these values and restart the game to apply them.");
            builder.AppendLine();
            builder.AppendLine("Title = " + Title);
            builder.AppendLine("Subtitle = " + Subtitle);
            builder.AppendLine("CreditName = " + CreditName);
            builder.AppendLine("GitHubUrl = " + GitHubUrl);
            builder.AppendLine("DefaultTheme = " + DefaultTheme);
            builder.AppendLine("DefaultScale = " + DefaultScale.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("DefaultOpacity = " + DefaultOpacity.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine("DefaultSounds = " + (DefaultSounds ? "true" : "false"));
            builder.AppendLine("DefaultPcKey = " + DefaultPcKey);
            builder.AppendLine("CornerRadius = " + CornerRadius);
            builder.AppendLine("EnableCreditsPage = " + (EnableCreditsPage ? "true" : "false"));
            builder.AppendLine("EnableAssetsPage = " + (EnableAssetsPage ? "true" : "false"));
            builder.AppendLine("BundlesFolder = " + BundlesFolder);
            builder.AppendLine("EnableConsole = " + (EnableConsole ? "true" : "false"));
            builder.AppendLine("ServerDataUrl = " + ServerDataUrl);
            builder.AppendLine("LocalPlayFabId = " + LocalPlayFabId);
            return builder.ToString();
        }
    }
}
