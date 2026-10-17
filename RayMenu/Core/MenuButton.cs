using System;

namespace RayMenu.Core
{
    public class MenuButton
    {
        public Func<string> Label;
        public Action OnClick;
        public Func<bool> IsActive;

        public MenuButton(string label, Action onClick, Func<bool> isActive = null)
            : this(() => label, onClick, isActive)
        {
        }

        public MenuButton(Func<string> label, Action onClick, Func<bool> isActive = null)
        {
            Label = label;
            OnClick = onClick;
            IsActive = isActive;
        }
    }
}
