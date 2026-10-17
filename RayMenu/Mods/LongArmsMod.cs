using RayMenu.Game;

namespace RayMenu.Mods
{
    public class LongArmsMod : IMod
    {
        public static readonly float[] Presets = { 2f, 2.5f, 3f, 4f, 5f, 6f, 8f };

        private const float BaseArmLength = 1.5f;

        private readonly GameBridge _game;

        public LongArmsMod(GameBridge game)
        {
            _game = game;
        }

        public string Name => "Long Arms";
        public bool Enabled { get; set; }
        public int PresetIndex { get; set; } = 2;
        public float Length => Presets[PresetIndex];

        public void OnUpdate()
        {
            var player = _game.Player;
            if (player == null)
            {
                _game.EnsureResolved();
                return;
            }

            ReflectionHelper.TrySetMember(player, "maxArmLength", Length);
        }

        public void OnDisable()
        {
            var player = _game.Player;
            if (player == null) return;

            ReflectionHelper.TrySetMember(player, "maxArmLength", BaseArmLength);
        }
    }
}
