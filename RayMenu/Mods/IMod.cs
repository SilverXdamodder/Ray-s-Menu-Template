namespace RayMenu.Mods
{
    public interface IMod
    {
        string Name { get; }
        bool Enabled { get; set; }
        void OnUpdate();
        void OnDisable();
    }
}
