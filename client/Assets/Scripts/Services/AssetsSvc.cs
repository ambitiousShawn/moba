using System.Collections.Generic;
using UnityEngine;

namespace ShawnFramework.CommonModule
{
    /// <summary>
    /// �ʲ�����
    /// </summary>
    public class AssetsSvc : MonoBehaviour
    {
        public static AssetsSvc Instance;

        public EAssetsType type = EAssetsType.Resources;
        public void InitService()
        {
            Instance = this;
        }

        private Dictionary<string, AudioClip> _cacheDic = new Dictionary<string, AudioClip>();
        public AudioClip LoadAudioClip(string path)
        {
            AudioClip audio = null;
            if (_cacheDic.TryGetValue(path, out audio))
                return audio;

            // TODO:��Դ����
            switch (type)
            {
                case EAssetsType.Resources:
                    audio = Resources.Load<AudioClip>(path);
                    break;
                case EAssetsType.AssetBundle:
                    break;
                case EAssetsType.Addressable:
                    break;
            }

            _cacheDic.Add(path, audio);

            return audio;
        }
    }

    public enum EAssetsType
    {
        Resources,
        AssetBundle,
        Addressable,
    }
}