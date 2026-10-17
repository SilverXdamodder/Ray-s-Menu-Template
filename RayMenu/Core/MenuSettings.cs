using RayMenu.Settings;
using UnityEngine;

namespace RayMenu.Core
{
    public class MenuSettings
    {
        public float Scale = MenuConfig.DefaultScale;
        public float Opacity = MenuConfig.DefaultOpacity;
        public bool Sounds = MenuConfig.DefaultSounds;
        public KeyCode PcKey = MenuConfig.DefaultPcKey;
        public int ThemeIndex = MenuConfig.DefaultTheme;
        public int SpeedPreset = 2;
        public int ArmPreset = 2;

        private static readonly KeyCode[] KeyCycle =
        {
            KeyCode.Q, KeyCode.E, KeyCode.F1, KeyCode.F2, KeyCode.F3,
            KeyCode.F4, KeyCode.F5, KeyCode.BackQuote, KeyCode.Tab, KeyCode.LeftAlt
        };

        public string KeyName => PcKey.ToString();

        public void Load()
        {
            Scale = PlayerPrefs.GetFloat("RayMenu.Scale", Scale);
            Opacity = PlayerPrefs.GetFloat("RayMenu.Opacity", Opacity);
            Sounds = PlayerPrefs.GetInt("RayMenu.Sounds", Sounds ? 1 : 0) != 0;
            PcKey = (KeyCode)PlayerPrefs.GetInt("RayMenu.PcKey", (int)PcKey);
            ThemeIndex = PlayerPrefs.GetInt("RayMenu.Theme", ThemeIndex);
            SpeedPreset = PlayerPrefs.GetInt("RayMenu.Speed", SpeedPreset);
            ArmPreset = PlayerPrefs.GetInt("RayMenu.Arm", ArmPreset);
        }

        public void Save()
        {
            PlayerPrefs.SetFloat("RayMenu.Scale", Scale);
            PlayerPrefs.SetFloat("RayMenu.Opacity", Opacity);
            PlayerPrefs.SetInt("RayMenu.Sounds", Sounds ? 1 : 0);
            PlayerPrefs.SetInt("RayMenu.PcKey", (int)PcKey);
            PlayerPrefs.SetInt("RayMenu.Theme", ThemeIndex);
            PlayerPrefs.SetInt("RayMenu.Speed", SpeedPreset);
            PlayerPrefs.SetInt("RayMenu.Arm", ArmPreset);
            PlayerPrefs.Save();
        }

        public void CycleKey(int direction)
        {
            int i = System.Array.IndexOf(KeyCycle, PcKey);
            if (i < 0) i = 0;
            i = (i + direction) % KeyCycle.Length;
            if (i < 0) i += KeyCycle.Length;
            PcKey = KeyCycle[i];
        }
    }
}
