using System;
using System.Collections.Generic;
using RayMenu.Game;
using RayMenu.Mods;
using RayMenu.Settings;
using UnityEngine;

namespace RayMenu.Core
{
    public class MenuCore
    {
        public const float Width = 720f;
        public const float Height = 580f;
        public const int ButtonsPerPage = 8;

        public bool Visible;
        public MenuSettings Settings { get; }

        private readonly ModManager _mods;
        private readonly GameBridge _game;
        private readonly List<MenuPage> _pages = new List<MenuPage>();
        private int _pageIndex;
        private int _themePage;

        private GUIStyle _window;
        private GUIStyle _panel;
        private GUIStyle _sidePanel;
        private GUIStyle _bar;
        private GUIStyle _mod;
        private GUIStyle _modActive;
        private GUIStyle _category;
        private GUIStyle _categoryActive;
        private GUIStyle _arrow;
        private GUIStyle _icon;
        private GUIStyle _title;
        private GUIStyle _subtitle;
        private int _styleTheme = -1;

        private static readonly Dictionary<string, Texture2D> TexCache = new Dictionary<string, Texture2D>();
        private static Texture2D _gearTex;
        private static Texture2D _arrowTex;

        private static readonly Color White = Rgb(255, 255, 255);
        private static readonly Color Black = Rgb(0, 0, 0);
        private static readonly Color SubText = Rgb(150, 150, 150);

        public MenuCore(ModManager mods, GameBridge game)
        {
            _mods = mods;
            _game = game;
            Settings = new MenuSettings();
            Settings.Load();
            ThemeManager.Apply(Settings.ThemeIndex);
            MenuSound.Enabled = Settings.Sounds;
            SyncPresets();

            _pages.Add(new MenuPage("Main", BuildMain));
            _pages.Add(new MenuPage("Settings", BuildSettings));
            _pages.Add(new MenuPage("Themes", BuildThemes));
            if (MenuConfig.EnableCreditsPage) _pages.Add(new MenuPage("Credits", BuildCredits));
        }

        private void SyncPresets()
        {
            _mods.SpeedBoost.PresetIndex = Mathf.Clamp(Settings.SpeedPreset, 0, SpeedBoostMod.Presets.Length - 1);
            _mods.LongArms.PresetIndex = Mathf.Clamp(Settings.ArmPreset, 0, LongArmsMod.Presets.Length - 1);
        }

        public void Update()
        {
            bool pcHeld = !GameBridge.IsVrPresent && Input.GetKey(Settings.PcKey);
            bool vrHeld = _game.VrMenuButtonHeld();

            if ((pcHeld || vrHeld) && !Visible) MenuSound.Play();
            Visible = pcHeld || vrHeld;
        }

        public void Draw()
        {
            if (!Visible) return;
            EnsureStyles();

            var buttons = _pages[_pageIndex].Build();
            if (buttons.Count > ButtonsPerPage) buttons = buttons.GetRange(0, ButtonsPerPage);

            var previousMatrix = GUI.matrix;
            float scale = Settings.Scale;
            GUI.matrix = Matrix4x4.TRS(
                new Vector3((Screen.width - Width * scale) * 0.5f, (Screen.height - Height * scale) * 0.5f, 0f),
                Quaternion.identity,
                new Vector3(scale, scale, 1f));

            try
            {
                GUI.Box(new Rect(0f, 0f, Width, Height), "", _window);

                if (GUI.Button(new Rect(12f, 16f, 88f, 205f), "<", _arrow)) PrevPage();
                if (GUI.Button(new Rect(12f, 232f, 88f, 262f), ">", _arrow)) NextPage();

                GUI.Box(new Rect(118f, 12f, 430f, 482f), "", _panel);

                ShadowLabel(new Rect(118f, 22f, 430f, 52f), MenuConfig.Title + " [" + (_pageIndex + 1) + "]", _title);
                GUI.Label(new Rect(118f, 74f, 430f, 24f), MenuConfig.Subtitle, _subtitle);

                float buttonY = 106f;
                const float buttonH = 42f;
                const float buttonGap = 4f;
                for (int i = 0; i < buttons.Count; i++)
                {
                    var rect = new Rect(134f, buttonY, 398f, buttonH);
                    DrawModButton(rect, buttons[i]);
                    buttonY += buttonH + buttonGap;
                }

                GUI.Box(new Rect(556f, 12f, 152f, 482f), "", _sidePanel);
                DrawCategories();

                var gearRect = new Rect(274f, 504f, 54f, 52f);
                var arrowRect = new Rect(338f, 504f, 54f, 52f);
                if (GUI.Button(gearRect, "", _icon)) Goto("Settings");
                if (GUI.Button(arrowRect, "", _icon)) NextPage();
                DrawIcon(gearRect, GearIcon);
                DrawIcon(arrowRect, ArrowIcon);
            }
            finally
            {
                GUI.matrix = previousMatrix;
            }
        }

        private void DrawCategories()
        {
            const float gap = 6f;
            const float top = 26f;
            float available = 482f - top - 26f;
            int count = _pages.Count;
            float height = Mathf.Min(44f, (available - gap * (count - 1)) / count);

            float y = top;
            for (int i = 0; i < count; i++)
            {
                var rect = new Rect(566f, y, 132f, height);
                if (GUI.Button(rect, _pages[i].Name, i == _pageIndex ? _categoryActive : _category))
                {
                    _pageIndex = i;
                    _themePage = 0;
                    MenuSound.Play();
                }
                y += height + gap;
            }
        }

        private void DrawModButton(Rect rect, MenuButton button)
        {
            bool active = button.IsActive != null && button.IsActive();
            string label = button.Label != null ? button.Label() : string.Empty;
            if (GUI.Button(rect, label, active ? _modActive : _mod))
            {
                MenuSound.Play();
                button.OnClick?.Invoke();
            }
        }

        private void PrevPage()
        {
            if (_pageIndex <= 0) return;
            _pageIndex--;
            _themePage = 0;
            MenuSound.Play();
        }

        private void NextPage()
        {
            if (_pageIndex >= _pages.Count - 1) return;
            _pageIndex++;
            _themePage = 0;
            MenuSound.Play();
        }

        private void Goto(string pageName)
        {
            for (int i = 0; i < _pages.Count; i++)
            {
                if (_pages[i].Name == pageName)
                {
                    if (_pageIndex != i) MenuSound.Play();
                    _pageIndex = i;
                    _themePage = 0;
                    return;
                }
            }
        }

        private List<MenuButton> BuildMain()
        {
            return new List<MenuButton>
            {
                new MenuButton(
                    () => "Speed Boost: " + OnOff(_mods.SpeedBoost.Enabled),
                    () => ToggleMod(_mods.SpeedBoost),
                    () => _mods.SpeedBoost.Enabled),
                new MenuButton(
                    () => "Long Arms: " + OnOff(_mods.LongArms.Enabled),
                    () => ToggleMod(_mods.LongArms),
                    () => _mods.LongArms.Enabled),
                new MenuButton(
                    () => "Speed  x" + _mods.SpeedBoost.Multiplier.ToString("0.0"),
                    () =>
                    {
                        _mods.SpeedBoost.PresetIndex = (_mods.SpeedBoost.PresetIndex + 1) % SpeedBoostMod.Presets.Length;
                        Save();
                    },
                    () => false),
                new MenuButton(
                    () => "Arm Length: " + _mods.LongArms.Length.ToString("0.0"),
                    () =>
                    {
                        _mods.LongArms.PresetIndex = (_mods.LongArms.PresetIndex + 1) % LongArmsMod.Presets.Length;
                        Save();
                    },
                    () => false),
                new MenuButton("Reset Mods", ResetMods, () => false),
                new MenuButton("Settings  >", () => Goto("Settings"), () => false),
                new MenuButton("Themes  >", () => Goto("Themes"), () => false),
                new MenuButton("Close", () => Visible = false, () => false),
            };
        }

        private List<MenuButton> BuildSettings()
        {
            return new List<MenuButton>
            {
                new MenuButton(
                    () => "Scale -  " + Settings.Scale.ToString("0.00"),
                    () => Settings.Scale = Mathf.Clamp(Settings.Scale - 0.05f, 0.6f, 1.6f),
                    () => false),
                new MenuButton(
                    () => "Scale +  " + Settings.Scale.ToString("0.00"),
                    () => Settings.Scale = Mathf.Clamp(Settings.Scale + 0.05f, 0.6f, 1.6f),
                    () => false),
                new MenuButton(
                    () => "Opacity -  " + Mathf.RoundToInt(Settings.Opacity * 100f) + "%",
                    () => Settings.Opacity = Mathf.Clamp(Settings.Opacity - 0.05f, 0.4f, 1f),
                    () => false),
                new MenuButton(
                    () => "Opacity +  " + Mathf.RoundToInt(Settings.Opacity * 100f) + "%",
                    () => Settings.Opacity = Mathf.Clamp(Settings.Opacity + 0.05f, 0.4f, 1f),
                    () => false),
                new MenuButton(
                    () => "Sounds: " + OnOff(Settings.Sounds),
                    () =>
                    {
                        Settings.Sounds = !Settings.Sounds;
                        MenuSound.Enabled = Settings.Sounds;
                    },
                    () => Settings.Sounds),
                new MenuButton(
                    () => "PC Key: " + Settings.KeyName,
                    () => Settings.CycleKey(1),
                    () => false),
                new MenuButton("Save Settings", Save, () => false),
                new MenuButton("< Back", () => Goto("Main"), () => false),
            };
        }

        private List<MenuButton> BuildThemes()
        {
            const int perPage = 6;
            var list = new List<MenuButton>();
            int start = _themePage * perPage;

            for (int i = 0; i < perPage; i++)
            {
                int index = start + i;
                if (index < ThemeManager.Themes.Count)
                {
                    int captured = index;
                    list.Add(new MenuButton(
                        () => ThemeManager.Themes[captured].Name,
                        () =>
                        {
                            ThemeManager.Apply(captured);
                            Save();
                        },
                        () => captured == ThemeManager.Index));
                }
                else
                {
                    list.Add(new MenuButton(string.Empty, null, () => false));
                }
            }

            bool canPrev = _themePage > 0;
            bool canNext = start + perPage < ThemeManager.Themes.Count;
            list.Add(new MenuButton("< Prev", canPrev ? new Action(() => _themePage--) : null, () => false));
            list.Add(new MenuButton("Next >", canNext ? new Action(() => _themePage++) : null, () => false));
            return list;
        }

        private List<MenuButton> BuildCredits()
        {
            return new List<MenuButton>
            {
                new MenuButton(MenuConfig.CreditName + "  -  Developer", () => OpenUrl(MenuConfig.GitHubUrl), () => false),
                new MenuButton(MenuConfig.GitHubUrl, () => OpenUrl(MenuConfig.GitHubUrl), () => false),
                new MenuButton("Open GitHub  >", () => OpenUrl(MenuConfig.GitHubUrl), () => false),
                new MenuButton("< Back", () => Goto("Main"), () => false),
                new MenuButton(string.Empty, null, () => false),
                new MenuButton(string.Empty, null, () => false),
                new MenuButton(string.Empty, null, () => false),
                new MenuButton(string.Empty, null, () => false),
            };
        }

        private static void OpenUrl(string url)
        {
            try
            {
                Application.OpenURL(url);
            }
            catch
            {
            }
        }

        private void ToggleMod(IMod mod)
        {
            mod.Enabled = !mod.Enabled;
            if (!mod.Enabled) mod.OnDisable();
            Save();
        }

        private void ResetMods()
        {
            _mods.SpeedBoost.Enabled = false;
            _mods.SpeedBoost.OnDisable();
            _mods.SpeedBoost.PresetIndex = 2;

            _mods.LongArms.Enabled = false;
            _mods.LongArms.OnDisable();
            _mods.LongArms.PresetIndex = 2;

            Save();
        }

        private void Save()
        {
            Settings.ThemeIndex = ThemeManager.Index;
            Settings.SpeedPreset = _mods.SpeedBoost.PresetIndex;
            Settings.ArmPreset = _mods.LongArms.PresetIndex;
            Settings.Save();
        }

        private static string OnOff(bool value) => value ? "ON" : "OFF";

        private void EnsureStyles()
        {
            if (_title != null && _styleTheme == ThemeManager.Index) return;
            var theme = ThemeManager.Current;
            int r = MenuConfig.CornerRadius;

            _window = Rounded(theme.Background, theme.Outline, 1, r + 4);
            _panel = Rounded(theme.Panel, theme.Outline, 1, r);
            _sidePanel = Rounded(theme.Outline, theme.Outline, 0, r);
            _bar = Rounded(theme.Outline, theme.Outline, 0, r);

            _mod = Rounded(theme.Button, theme.Outline, 2, r);
            _mod.fontSize = 22;
            _mod.fontStyle = FontStyle.BoldAndItalic;
            _mod.normal.textColor = theme.Text;
            _mod.hover.textColor = theme.Text;
            _mod.active.textColor = theme.Text;

            _modActive = Rounded(theme.ButtonActive, theme.Outline, 2, r);
            _modActive.fontSize = 22;
            _modActive.fontStyle = FontStyle.BoldAndItalic;
            _modActive.normal.textColor = Contrast(theme.ButtonActive);
            _modActive.hover.textColor = Contrast(theme.ButtonActive);
            _modActive.active.textColor = Contrast(theme.ButtonActive);

            _category = Rounded(theme.Accent, theme.Outline, 2, r);
            _category.fontSize = 18;
            _category.fontStyle = FontStyle.Bold;
            _category.normal.textColor = Contrast(theme.Accent);
            _category.hover.textColor = Contrast(theme.Accent);
            _category.active.textColor = Contrast(theme.Accent);

            _categoryActive = Rounded(theme.Accent, theme.Outline, 4, r);
            _categoryActive.fontSize = 18;
            _categoryActive.fontStyle = FontStyle.Bold;
            _categoryActive.normal.textColor = Contrast(theme.Accent);
            _categoryActive.hover.textColor = Contrast(theme.Accent);
            _categoryActive.active.textColor = Contrast(theme.Accent);

            _arrow = Rounded(theme.Outline, theme.Outline, 0, r);
            _arrow.fontSize = 54;
            _arrow.fontStyle = FontStyle.Bold;
            _arrow.normal.textColor = Contrast(theme.Outline);
            _arrow.hover.textColor = Contrast(theme.Outline);
            _arrow.active.textColor = Contrast(theme.Outline);

            _icon = Rounded(theme.Outline, theme.Outline, 0, r);

            _title = new GUIStyle(GUI.skin.label)
            {
                fontSize = 40,
                fontStyle = FontStyle.BoldAndItalic,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = false
            };
            _title.normal.textColor = White;

            _subtitle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = false
            };
            _subtitle.normal.textColor = SubText;

            _styleTheme = ThemeManager.Index;
        }

        private static GUIStyle Rounded(Color fill, Color border, int borderWidth, int radius)
        {
            var style = new GUIStyle(GUI.skin.button)
            {
                border = new RectOffset(radius, radius, radius, radius),
                padding = new RectOffset(6, 6, 6, 6),
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
            var texture = RoundedTexture(fill, border, borderWidth, radius);
            style.normal.background = texture;
            style.hover.background = texture;
            style.active.background = texture;
            style.focused.background = texture;
            style.onNormal.background = texture;
            style.onHover.background = texture;
            return style;
        }

        private static Texture2D RoundedTexture(Color fill, Color border, int borderWidth, int radius)
        {
            string key = fill.r + "_" + fill.g + "_" + fill.b + "_" + fill.a + "|"
                + border.r + "_" + border.g + "_" + border.b + "_" + border.a + "|"
                + borderWidth + "_" + radius;
            if (TexCache.TryGetValue(key, out var cached) && cached != null) return cached;

            int size = radius * 2 + 2;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = 0f;
                    float dy = 0f;
                    if (x < radius) dx = radius - x - 0.5f;
                    else if (x >= size - radius) dx = x - (size - radius) + 0.5f;
                    if (y < radius) dy = radius - y - 0.5f;
                    else if (y >= size - radius) dy = y - (size - radius) + 0.5f;

                    float distance = Mathf.Sqrt(dx * dx + dy * dy);
                    if (distance > radius)
                    {
                        texture.SetPixel(x, y, new Color(0f, 0f, 0f, 0f));
                        continue;
                    }
                    texture.SetPixel(x, y, distance > radius - borderWidth ? border : fill);
                }
            }

            texture.Apply();
            TexCache[key] = texture;
            return texture;
        }

        private void ShadowLabel(Rect rect, string text, GUIStyle style)
        {
            var previous = style.normal.textColor;
            style.normal.textColor = Black;
            GUI.Label(new Rect(rect.x + 2f, rect.y + 2f, rect.width, rect.height), text, style);
            style.normal.textColor = previous;
            GUI.Label(rect, text, style);
        }

        private static void DrawIcon(Rect rect, Texture2D icon)
        {
            if (icon == null) return;
            float pad = rect.height * 0.22f;
            float size = rect.height - pad * 2f;
            var position = new Rect(rect.x + (rect.width - size) * 0.5f, rect.y + pad, size, size);
            var previous = GUI.color;
            GUI.color = Color.white;
            GUI.DrawTexture(position, icon);
            GUI.color = previous;
        }

        private static Color Contrast(Color color)
        {
            float luminance = 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
            return luminance > 0.6f ? Black : White;
        }

        private static Color Rgb(int r, int g, int b) => new Color(r / 255f, g / 255f, b / 255f, 1f);

        private static Texture2D GearIcon
        {
            get
            {
                if (_gearTex != null) return _gearTex;
                const int size = 64;
                _gearTex = new Texture2D(size, size, TextureFormat.RGBA32, false)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };

                float center = (size - 1) / 2f;
                float outer = size * 0.46f;
                float inner = size * 0.30f;
                float hole = size * 0.12f;

                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float dx = x - center;
                        float dy = y - center;
                        float distance = Mathf.Sqrt(dx * dx + dy * dy);
                        float angle = Mathf.Atan2(dy, dx);
                        float tooth = (Mathf.Cos(angle * 8f) + 1f) * 0.5f;
                        float reach = Mathf.Lerp(inner, outer, tooth);
                        bool on = distance <= reach && distance >= hole;
                        _gearTex.SetPixel(x, y, on ? White : new Color(0f, 0f, 0f, 0f));
                    }
                }

                _gearTex.Apply();
                return _gearTex;
            }
        }

        private static Texture2D ArrowIcon
        {
            get
            {
                if (_arrowTex != null) return _arrowTex;
                const int size = 64;
                _arrowTex = new Texture2D(size, size, TextureFormat.RGBA32, false)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };

                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float nx = (float)x / size;
                        float ny = (float)y / size;
                        bool tail = nx >= 0.12f && nx <= 0.55f && ny >= 0.42f && ny <= 0.58f;
                        bool head = InTriangle(nx, ny, 0.50f, 0.18f, 0.50f, 0.82f, 0.90f, 0.50f);
                        _arrowTex.SetPixel(x, y, tail || head ? White : new Color(0f, 0f, 0f, 0f));
                    }
                }

                _arrowTex.Apply();
                return _arrowTex;
            }
        }

        private static bool InTriangle(float px, float py, float ax, float ay, float bx, float by, float cx, float cy)
        {
            float d1 = (px - bx) * (ay - by) - (ax - bx) * (py - by);
            float d2 = (px - cx) * (by - cy) - (bx - cx) * (py - cy);
            float d3 = (px - ax) * (cy - ay) - (cx - ax) * (py - ay);
            bool negative = d1 < 0f || d2 < 0f || d3 < 0f;
            bool positive = d1 > 0f || d2 > 0f || d3 > 0f;
            return !(negative && positive);
        }
    }
}
