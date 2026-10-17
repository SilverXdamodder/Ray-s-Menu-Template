using BepInEx;
using RayMenu.Console;
using RayMenu.Core;
using RayMenu.Game;
using RayMenu.Mods;
using RayMenu.Settings;

namespace RayMenu
{
    [BepInPlugin(Guid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Guid = "com.ray.raymenu";
        public const string PluginName = "Ray Menu";
        public const string PluginVersion = "1.0.0";

        private void Awake()
        {
            MenuConfig.Load();
            ConsoleClient.Instance.Initialize();

            var game = new GameBridge(Logger);
            var mods = new ModManager(game);
            var menu = new MenuCore(mods, game);

            var behaviour = gameObject.AddComponent<MenuBehaviour>();
            behaviour.Menu = menu;
            behaviour.Mods = mods;
            behaviour.Game = game;

            Logger.LogInfo(PluginName + " v" + PluginVersion + " loaded. Hold [" + menu.Settings.KeyName + "] (PC) or Left Y (VR) to open.");
        }
    }
}
