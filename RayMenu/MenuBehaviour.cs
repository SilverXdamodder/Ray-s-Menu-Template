using RayMenu.Core;
using RayMenu.Game;
using RayMenu.Mods;
using UnityEngine;

namespace RayMenu
{
    public class MenuBehaviour : MonoBehaviour
    {
        public MenuCore Menu;
        public ModManager Mods;
        public GameBridge Game;

        private void Update()
        {
            Game.EnsureResolved();
            Menu.Update();
            Mods.Update();
        }

        private void OnGUI()
        {
            Menu.Draw();
        }

        private void OnDestroy()
        {
            Mods.DisableAll();
        }
    }
}
