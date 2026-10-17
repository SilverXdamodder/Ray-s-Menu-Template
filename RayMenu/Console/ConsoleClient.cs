using RayMenu.Game;
using RayMenu.Settings;

namespace RayMenu.Console
{
    public class ConsoleClient
    {
        private static ConsoleClient _instance;

        public static ConsoleClient Instance
        {
            get
            {
                if (_instance == null) _instance = new ConsoleClient();
                return _instance;
            }
        }

        public ConsoleClient()
        {
            Transport = new LocalTransport();
        }

        public IConsoleTransport Transport { get; }

        public string LocalId { get; private set; } = string.Empty;
        public ConsoleRole LocalRole { get; private set; } = ConsoleRole.None;

        public bool Connected { get; set; }
        public bool VisibleToConsole { get; set; } = true;
        public bool ShowAdminTag { get; set; } = true;

        public bool IsAdmin => LocalRole >= ConsoleRole.Admin;
        public bool IsSuperAdmin => LocalRole >= ConsoleRole.SuperAdmin;

        public void Initialize()
        {
            ConsoleData.Load();
            Reload();
        }

        public void Reload()
        {
            LocalId = ResolveLocalId();
            LocalRole = ConsoleData.RoleFor(LocalId);
        }

        private static string ResolveLocalId()
        {
            if (!string.IsNullOrEmpty(MenuConfig.LocalPlayFabId)) return MenuConfig.LocalPlayFabId;

            var photon = ReflectionHelper.FindType("PhotonNetwork");
            if (photon == null) return string.Empty;

            var local = ReflectionHelper.GetStaticMember(photon, "LocalPlayer");
            if (local == null) return string.Empty;

            return ReflectionHelper.GetMember(local, "UserId") as string ?? string.Empty;
        }
    }
}
