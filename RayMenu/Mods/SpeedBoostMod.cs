using RayMenu.Game;

namespace RayMenu.Mods
{
    public class SpeedBoostMod : IMod
    {
        public static readonly float[] Presets = { 1.5f, 2f, 3f, 4f, 5f, 7.5f, 10f };

        private const float BaseJumpSpeed = 6.5f;
        private const float BaseJumpMultiplier = 1.1f;

        private readonly GameBridge _game;

        public SpeedBoostMod(GameBridge game)
        {
            _game = game;
        }

        public string Name => "Speed Boost";
        public bool Enabled { get; set; }
        public int PresetIndex { get; set; } = 2;
        public float Multiplier => Presets[PresetIndex];

        public void OnUpdate()
        {
            var player = _game.Player;
            if (player == null)
            {
                _game.EnsureResolved();
                return;
            }

            ReflectionHelper.TrySetMember(player, "maxJumpSpeed", BaseJumpSpeed * Multiplier);
            ReflectionHelper.TrySetMember(player, "jumpMultiplier", BaseJumpMultiplier * Multiplier);
        }

        public void OnDisable()
        {
            var player = _game.Player;
            if (player == null) return;

            ReflectionHelper.TrySetMember(player, "maxJumpSpeed", BaseJumpSpeed);
            ReflectionHelper.TrySetMember(player, "jumpMultiplier", BaseJumpMultiplier);
        }
    }
}
