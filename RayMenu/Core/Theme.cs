using UnityEngine;

namespace RayMenu.Core
{
    public class Theme
    {
        public string Name;
        public Color Background;
        public Color Panel;
        public Color Button;
        public Color ButtonHover;
        public Color ButtonActive;
        public Color Text;
        public Color Accent;
        public Color Outline;

        public Theme(
            string name,
            Color background,
            Color panel,
            Color button,
            Color buttonHover,
            Color buttonActive,
            Color text,
            Color accent,
            Color outline)
        {
            Name = name;
            Background = background;
            Panel = panel;
            Button = button;
            ButtonHover = buttonHover;
            ButtonActive = buttonActive;
            Text = text;
            Accent = accent;
            Outline = outline;
        }
    }
}
