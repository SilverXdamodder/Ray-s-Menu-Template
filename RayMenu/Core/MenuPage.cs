using System;
using System.Collections.Generic;
using RayMenu.Console;

namespace RayMenu.Core
{
    public class MenuPage
    {
        public string Name;
        public Func<List<MenuButton>> Build;
        public ConsoleRole RequiredRole;

        public MenuPage(string name, Func<List<MenuButton>> build, ConsoleRole requiredRole = ConsoleRole.None)
        {
            Name = name;
            Build = build;
            RequiredRole = requiredRole;
        }
    }
}
