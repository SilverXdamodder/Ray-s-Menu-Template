using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using UnityEngine;

namespace RayMenu.Assets
{
    public static class BundleLibrary
    {
        private static readonly List<string> Files = new List<string>();
        private static readonly Dictionary<string, AssetBundle> Bundles = new Dictionary<string, AssetBundle>();
        private static readonly List<GameObject> Spawned = new List<GameObject>();

        private static string _folder;

        public static string Folder
        {
            get
            {
                if (!string.IsNullOrEmpty(Settings.MenuConfig.BundlesFolder)) return Settings.MenuConfig.BundlesFolder;
                return _folder ?? (_folder = ResolveDefault());
            }
        }

        private static string ResolveDefault()
        {
            string config = Path.Combine(Paths.ConfigPath, "RayMenu", "Bundles");
            if (System.IO.Directory.Exists(config)) return config;

            string beside = BesidePlugin();
            if (beside != null && System.IO.Directory.Exists(beside)) return beside;

            return config;
        }

        private static string BesidePlugin()
        {
            try
            {
                string location = typeof(BundleLibrary).Assembly.Location;
                if (string.IsNullOrEmpty(location)) return null;

                string directory = Path.GetDirectoryName(location);
                return directory == null ? null : Path.Combine(directory, "Bundles");
            }
            catch
            {
                return null;
            }
        }

        public static int BundleCount => Files.Count;
        public static int SpawnedCount => Spawned.Count;
        public static int SelectedIndex { get; private set; }
        public static string LastError { get; private set; } = string.Empty;

        public static string SelectedName
        {
            get
            {
                if (Files.Count == 0) return "none";
                if (SelectedIndex < 0) SelectedIndex = Files.Count - 1;
                if (SelectedIndex >= Files.Count) SelectedIndex = 0;
                return Path.GetFileNameWithoutExtension(Files[SelectedIndex]);
            }
        }

        public static void Refresh()
        {
            Files.Clear();
            SelectedIndex = 0;

            try
            {
                if (!System.IO.Directory.Exists(Folder))
                {
                    System.IO.Directory.CreateDirectory(Folder);
                    return;
                }

                foreach (string path in System.IO.Directory.GetFiles(Folder))
                {
                    string extension = Path.GetExtension(path).ToLowerInvariant();
                    if (extension == ".manifest" || extension == ".txt" || extension == ".ini") continue;
                    Files.Add(path);
                }

                Files.Sort(StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception e)
            {
                LastError = e.Message;
            }
        }

        public static void CycleSelection()
        {
            if (Files.Count == 0) return;
            SelectedIndex = (SelectedIndex + 1) % Files.Count;
        }

        public static GameObject SpawnSelected()
        {
            if (Files.Count == 0)
            {
                LastError = "No bundles in " + Folder;
                return null;
            }

            try
            {
                string path = Files[SelectedIndex];
                AssetBundle bundle = GetBundle(path);
                if (bundle == null)
                {
                    LastError = "Could not load " + Path.GetFileName(path);
                    return null;
                }

                string assetName = Path.GetFileNameWithoutExtension(path);
                GameObject prefab = bundle.LoadAsset<GameObject>(assetName);

                if (prefab == null)
                {
                    var all = bundle.LoadAllAssets<GameObject>();
                    if (all != null && all.Length > 0) prefab = all[0];
                }

                if (prefab == null)
                {
                    LastError = "No prefab found in " + assetName;
                    return null;
                }

                GameObject instance = UnityEngine.Object.Instantiate(prefab);
                instance.name = assetName;
                Spawned.Add(instance);
                LastError = string.Empty;
                return instance;
            }
            catch (Exception e)
            {
                LastError = e.Message;
                return null;
            }
        }

        public static void ApplyPlayback(GameObject instance, bool audio, bool effects)
        {
            if (instance == null) return;

            try
            {
                AudioSource[] sources = instance.GetComponentsInChildren<AudioSource>(true);
                for (int i = 0; i < sources.Length; i++)
                {
                    if (audio) sources[i].Play();
                    else sources[i].Stop();
                }
            }
            catch
            {
            }

            try
            {
                ParticleSystem[] systems = instance.GetComponentsInChildren<ParticleSystem>(true);
                for (int i = 0; i < systems.Length; i++)
                {
                    if (effects) systems[i].Play();
                    else systems[i].Stop();
                }
            }
            catch
            {
            }
        }

        public static void Clear()
        {
            for (int i = 0; i < Spawned.Count; i++)
            {
                if (Spawned[i] != null) UnityEngine.Object.Destroy(Spawned[i]);
            }
            Spawned.Clear();
        }

        public static void UnloadAll()
        {
            Clear();

            foreach (var pair in Bundles)
            {
                if (pair.Value != null) pair.Value.Unload(false);
            }
            Bundles.Clear();
        }

        private static AssetBundle GetBundle(string path)
        {
            AssetBundle bundle;
            if (Bundles.TryGetValue(path, out bundle) && bundle != null) return bundle;

            bundle = AssetBundle.LoadFromFile(path);
            Bundles[path] = bundle;
            return bundle;
        }
    }
}
