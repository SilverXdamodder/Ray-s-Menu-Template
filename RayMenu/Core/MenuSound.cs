using UnityEngine;

namespace RayMenu.Core
{
    public static class MenuSound
    {
        public static bool Enabled = true;

        private static AudioSource _source;
        private static AudioClip _click;

        public static void Play()
        {
            if (!Enabled) return;
            try
            {
                if (_source == null)
                {
                    var host = new GameObject("RayMenuAudio");
                    Object.DontDestroyOnLoad(host);
                    _source = host.AddComponent<AudioSource>();
                    _source.volume = 0.25f;
                    _source.playOnAwake = false;
                }
                if (_click == null) _click = CreateClick();
                _source.PlayOneShot(_click);
            }
            catch
            {
            }
        }

        private static AudioClip CreateClick()
        {
            const int rate = 44100;
            const float duration = 0.05f;
            int count = (int)(rate * duration);
            var data = new float[count];
            for (int i = 0; i < count; i++)
            {
                float t = (float)i / rate;
                float fade = 1f - (float)i / count;
                data[i] = Mathf.Sin(2f * Mathf.PI * 1200f * t) * 0.5f * fade;
            }
            var clip = AudioClip.Create("RayMenuClick", count, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
