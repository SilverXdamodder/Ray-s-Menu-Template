using System;
using System.Collections.Generic;

namespace RayMenu.Core
{
    public class MenuPage
    {
        public string Name;
        public Func<List<MenuButton>> Build;

        public MenuPage(string name, Func<List<MenuButton>> build)
        {
            Name = name;
            Build = build;
        }
    }
}
