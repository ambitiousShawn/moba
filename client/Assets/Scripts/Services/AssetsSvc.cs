using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XLua;

namespace ShawnFramework.CommonModule
{
    /// <summary>
    /// 资产管理
    /// </summary>
    [LuaCallCSharp]
    public class AssetsSvc : MonoBehaviour
    {
        public static AssetsSvc Instance;

        public int test = 5;

        public EAssetsType type = EAssetsType.Resources;
        public void InitService()
        {
            Instance = this;
        }

        private void Update()
        {
            prgCB?.Invoke();
        }

        private Action prgCB = null;
        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="loadRate"></param>
        /// <param name="loaded"></param>
        public void LoadSceneAsync(string sceneName, Action<float> loadRate, Action loaded)
        {
            AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(sceneName);
            prgCB = () =>
            {
                float progress = sceneAsync.progress;
                loadRate?.Invoke(progress);
                if (progress == 1)
                {
                    loaded?.Invoke();
                    prgCB = null;
                    sceneAsync = null;
                }
            };
        }


        private Dictionary<string, AudioClip> _cacheDic = new Dictionary<string, AudioClip>();
        /// <summary>
        /// 加载音频切片
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AudioClip LoadAudioClip(string path)
        {
            AudioClip audio = null;
            if (_cacheDic.TryGetValue(path, out audio))
                return audio;

            // TODO:资源加载
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

        /// <summary>
        /// 根据heroID获取英雄配置数据
        /// </summary>
        /// <param name="heroID"></param>
        /// <returns></returns>
        public HeroConfig GetHeroConfigByID(uint heroID)
        {
            switch (heroID)
            {
                case 101:
                    return new HeroConfig
                    {
                        heroID = 101,
                        heroName = "德玛西亚之力",
                        resName = "Galen",

                        hp = 800,
                        defense = 0,
                        attack = 80,
                        moveSpeed = 3,
                        colliCfg = new ColliderConfig
                        {
                            mType = ColliderType.Cylinder,
                            mRadius = (ShawInt)0.5f
                        },
                        skillArr = new[]
                        {
                            1010,
                            1011,
                            1012,
                            1013
                        },
                    };
                case 102:
                    return new HeroConfig
                    {
                        heroID = 102,
                        heroName = "寒冰射手",
                        resName = "Ashe",

                        hp = 650,
                        defense = 0,
                        attack = 120,
                        moveSpeed = 3,
                        colliCfg = new ColliderConfig
                        {
                            mType = ColliderType.Cylinder,
                            mRadius = (ShawInt)0.5f
                        },
                        skillArr = new[]
                        {
                            1020, 1021, 1022, 1023
                        }
                    };
            }
            return null;
        }

        public enum EAssetsType
        {
            Resources,
            AssetBundle,
            Addressable,
        }
    }
}