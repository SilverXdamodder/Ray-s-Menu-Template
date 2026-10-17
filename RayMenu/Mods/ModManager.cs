using System.Collections.Generic;
using RayMenu.Game;

namespace RayMenu.Mods
{
    public class ModManager
    {
        public SpeedBoostMod SpeedBoost { get; }
        public LongArmsMod LongArms { get; }
        public IReadOnlyList<IMod> Mods { get; }

        public ModManager(GameBridge game)
        {
            SpeedBoost = new SpeedBoostMod(game);
            LongArms = new LongArmsMod(game);
            Mods = new List<IMod> { SpeedBoost, LongArms };
        }

        public void Update()
        {
            for (int i = 0; i < Mods.Count; i++)
            {
                if (Mods[i].Enabled) Mods[i].OnUpdate();
            }
        }

        public void DisableAll()
        {
            for (int i = 0; i < Mods.Count; i++)
            {
                if (!Mods[i].Enabled) continue;
                Mods[i].Enabled = false;
                Mods[i].OnDisable();
            }
        }
    }
}
