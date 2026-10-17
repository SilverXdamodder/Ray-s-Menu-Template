using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace RayMenu.Core
{
    public static class ThemeManager
    {
        public static readonly List<Theme> Themes = new List<Theme>
        {
            new Theme("Default",         Hex("3A3A3A"), Hex("D9D9D9"), Hex("000000"), Hex("1A1A1A"), Hex("FFFFFF"), Hex("FFFFFF"), Hex("E8E8E8"), Hex("000000")),
            new Theme("Midnight Purple", Hex("1A1030"), Hex("E8E0FF"), Hex("2A1A3E"), Hex("3A2456"), Hex("A259FF"), Hex("FFFFFF"), Hex("D9CCFF"), Hex("12081F")),
            new Theme("Neon Green",      Hex("0B1A0B"), Hex("E6FFE6"), Hex("0E2A12"), Hex("16401C"), Hex("39FF14"), Hex("EAFFEA"), Hex("C6FFC6"), Hex("052005")),
            new Theme("Cyberpunk",       Hex("12021A"), Hex("F3E0FF"), Hex("24003A"), Hex("360057"), Hex("FF00A0"), Hex("FFFFFF"), Hex("FFD9F2"), Hex("08000F")),
            new Theme("Blood Red",       Hex("1A0505"), Hex("FFE2E2"), Hex("330A0A"), Hex("4A1010"), Hex("FF2A2A"), Hex("FFEFEF"), Hex("FFC9C9"), Hex("0F0303")),
            new Theme("Ocean Blue",      Hex("041A2E"), Hex("DCF2FF"), Hex("0A2A46"), Hex("0F3A5E"), Hex("00B4D8"), Hex("FFFFFF"), Hex("BFE9FF"), Hex("020D17")),
            new Theme("Sunset Orange",   Hex("2A1105"), Hex("FFE8D6"), Hex("3E1A0A"), Hex("54250E"), Hex("FF7A00"), Hex("FFF3EA"), Hex("FFD3AD"), Hex("150802")),
            new Theme("Monochrome",      Hex("101010"), Hex("E6E6E6"), Hex("1E1E1E"), Hex("2E2E2E"), Hex("FFFFFF"), Hex("FFFFFF"), Hex("D0D0D0"), Hex("000000")),
            new Theme("Forest",          Hex("0C1A10"), Hex("E4F7E9"), Hex("16301D"), Hex("1F4228"), Hex("2ECC71"), Hex("E9FBEF"), Hex("C3EFCF"), Hex("050D07")),
            new Theme("Gold",            Hex("1A1600"), Hex("FFF6D6"), Hex("2E2A0A"), Hex("40380E"), Hex("FFD700"), Hex("FFF8E1"), Hex("FFEBA6"), Hex("0D0B00")),
        };

        public static int Index;

        public static Theme Current => Themes[Mathf.Clamp(Index, 0, Themes.Count - 1)];

        public static void Apply(int index) => Index = Mathf.Clamp(index, 0, Themes.Count - 1);

        public static Color Hex(string hex)
        {
            if (hex.StartsWith("#")) hex = hex.Substring(1);
            byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
    }
}
