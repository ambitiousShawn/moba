using UnityEngine;

namespace ShawnFramework.CommonModule
{
    public class AudioSvc : MonoBehaviour
    {
        public static AudioSvc Instance;
        [Header("播放BGM")]
        public bool TurnOnBGM;
        [Header("播放音效")]
        public bool TurnOnSoundEffect;

        private AudioSource _bgm;
        private AudioSource _uiSound;
        public void InitService()
        {
            Instance = this;

            _bgm = new GameObject("BGMAudio").AddComponent<AudioSource>();
            _uiSound = new GameObject("UISoundAudio").AddComponent<AudioSource>();
        }

        /// <summary>
        /// 播放BGM
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isLoop"></param>
        public void PlayBGM(string name, bool isLoop = true)
        {
            if (!TurnOnBGM)
                return;

            AudioClip clip = AssetsSvc.Instance.LoadAudioClip("Audio/" + name);
            if (clip == null || _bgm.clip.name != clip.name)
            {
                _bgm.clip = clip;
                _bgm.loop = isLoop;
                _bgm.Play();
            }
        }

        /// <summary>
        /// 播放UI音效
        /// </summary>
        /// <param name="name"></param>
        public void PlayUISound(string name)
        {
            if (!TurnOnSoundEffect)
                return;

            AudioClip clip = AssetsSvc.Instance.LoadAudioClip(name);
            _uiSound.clip = clip;
            _uiSound.Play();
        }
    }
}