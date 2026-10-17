using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using BepInEx;

namespace RayMenu.Assets
{
    public class Calibration
    {
        public float PosX;
        public float PosY;
        public float PosZ;
        public float RotX;
        public float RotY;
        public float RotZ;
        public float Scale = 1f;
        public int SpawnMode;
        public float SavedX;
        public float SavedY;
        public float SavedZ;
        public bool PlayAudio = true;
        public bool PlayEffects = true;
    }

    public static class BundleCalibration
    {
        private static readonly Dictionary<string, Calibration> Entries =
            new Dictionary<string, Calibration>(StringComparer.OrdinalIgnoreCase);

        public static string FilePath => Path.Combine(Paths.ConfigPath, "RayMenu", "calibration.ini");

        public static Calibration Get(string bundleName)
        {
            if (string.IsNullOrEmpty(bundleName)) return new Calibration();

            Calibration calibration;
            if (Entries.TryGetValue(bundleName, out calibration)) return calibration;

            calibration = new Calibration();
            Entries[bundleName] = calibration;
            return calibration;
        }

        public static void Reset(string bundleName)
        {
            if (string.IsNullOrEmpty(bundleName)) return;
            Entries[bundleName] = new Calibration();
            Save();
        }

        public static void Load()
        {
            Entries.Clear();

            try
            {
                if (!File.Exists(FilePath)) return;

                Calibration current = null;

                foreach (string raw in File.ReadAllLines(FilePath))
                {
                    string line = raw.Trim();
                    if (line.Length == 0 || line.StartsWith("#") || line.StartsWith(";")) continue;

                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        string section = line.Substring(1, line.Length - 2).Trim();
                        current = section.Length == 0 ? null : Get(section);
                        continue;
                    }

                    if (current == null) continue;

                    int split = line.IndexOf('=');
                    if (split <= 0) continue;

                    string key = line.Substring(0, split).Trim().ToLowerInvariant();
                    string value = line.Substring(split + 1).Trim();

                    float number;
                    if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out number)) continue;

                    switch (key)
                    {
                        case "posx": current.PosX = number; break;
                        case "posy": current.PosY = number; break;
                        case "posz": current.PosZ = number; break;
                        case "rotx": current.RotX = number; break;
                        case "roty": current.RotY = number; break;
                        case "rotz": current.RotZ = number; break;
                        case "scale": current.Scale = number < 0.01f ? 0.01f : number; break;
                        case "savedx": current.SavedX = number; break;
                        case "savedy": current.SavedY = number; break;
                        case "savedz": current.SavedZ = number; break;
                        case "spawn": current.SpawnMode = (int)number; break;
                        case "sound":
                            if (bool.TryParse(value, out bool sound)) current.PlayAudio = sound;
                            break;
                        case "effects":
                            if (bool.TryParse(value, out bool effects)) current.PlayEffects = effects;
                            break;
                    }
                }
            }
            catch
            {
            }
        }

        public static void Save()
        {
            try
            {
                System.IO.Directory.CreateDirectory(Path.Combine(Paths.ConfigPath, "RayMenu"));

                var builder = new StringBuilder();
                builder.AppendLine("# Ray Menu bundle calibration");
                builder.AppendLine("# Offset in metres, rotation in degrees, scale as a multiplier.");
                builder.AppendLine();

                foreach (var pair in Entries)
                {
                    Calibration entry = pair.Value;
                    builder.AppendLine("[" + pair.Key + "]");
                    builder.AppendLine("PosX = " + entry.PosX.ToString("0.###", CultureInfo.InvariantCulture));
                    builder.AppendLine("PosY = " + entry.PosY.ToString("0.###", CultureInfo.InvariantCulture));
                    builder.AppendLine("PosZ = " + entry.PosZ.ToString("0.###", CultureInfo.InvariantCulture));
                    builder.AppendLine("RotX = " + entry.RotX.ToString("0.###", CultureInfo.InvariantCulture));
                    builder.AppendLine("RotY = " + entry.RotY.ToString("0.###", CultureInfo.InvariantCulture));
                    builder.AppendLine("RotZ = " + entry.RotZ.ToString("0.###", CultureInfo.InvariantCulture));
                    builder.AppendLine("Scale = " + entry.Scale.ToString("0.###", CultureInfo.InvariantCulture));
                    builder.AppendLine("Spawn = " + entry.SpawnMode);
                    builder.AppendLine("SavedX = " + entry.SavedX.ToString("0.###", CultureInfo.InvariantCulture));
                    builder.AppendLine("SavedY = " + entry.SavedY.ToString("0.###", CultureInfo.InvariantCulture));
                    builder.AppendLine("SavedZ = " + entry.SavedZ.ToString("0.###", CultureInfo.InvariantCulture));
                    builder.AppendLine("Sound = " + (entry.PlayAudio ? "true" : "false"));
                    builder.AppendLine("Effects = " + (entry.PlayEffects ? "true" : "false"));
                    builder.AppendLine();
                }

                File.WriteAllText(FilePath, builder.ToString());
            }
            catch
            {
            }
        }
    }
}
