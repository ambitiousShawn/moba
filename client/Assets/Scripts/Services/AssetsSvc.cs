using ShawnFramework.ShawHotUpdate;
using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XLua;

namespace ShawnFramework.CommonModule
{
    /// <summary>
    /// 资产管理方式
    /// </summary>
    [LuaCallCSharp]
    public enum EAssetsType
    {
        Resources = 0,
        AssetBundle = 1,
        Addressable = 2,
    }

    /// <summary>
    /// 资产管理
    /// </summary>
    [LuaCallCSharp]
    public class AssetsSvc : MonoBehaviour
    {
        public static AssetsSvc Instance;

        public void InitService()
        {
            Instance = this;
        }

        private void Update()
        {
            prgCB?.Invoke();
        }

        private Dictionary<string, GameObject> m_PanelCache = new Dictionary<string, GameObject>();
        /// <summary>
        /// 资源加载通用方式
        /// </summary>
        /// <param name="arg1">if Resources:pathWithoutName >>> if AssetBundle:packageName </param>
        /// <param name="assetName">if Resources:assetName >>> if AssetBundle:assetName </param>
        /// <param name="assetsType">资源管理的方式</param>
        /// <param name="cache">是否启用缓存</param>
        /// <returns></returns>
        public GameObject LoadUIPrefab(string arg1, string assetName, int type, bool cache = false)
        {
            GameObject panel = null;
            EAssetsType assetsType = (EAssetsType)type;
            if (m_PanelCache.TryGetValue(assetName, out panel))
            {
                return panel;
            }
            switch (assetsType)
            {
                case EAssetsType.Resources:
                    panel = Resources.Load<GameObject>($"{arg1}/{assetName}");
                    break;
                case EAssetsType.AssetBundle:
                    panel = AssetBundleMgr.Instance.LoadAsset<GameObject>(arg1, assetName);
                    break;
                case EAssetsType.Addressable:
                    break;
            }

            return panel;
        }

        private Dictionary<string, Sprite> m_SpriteCache = new Dictionary<string, Sprite>();
        /// <summary>
        /// 加载精灵图像
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public Sprite LoadSprite(string path, bool cache = false)
        {
            Sprite sprite = null;
            if (!m_SpriteCache.TryGetValue(path, out sprite))
            {
                sprite = Resources.Load<Sprite>(path);
                if (cache)
                {
                    m_SpriteCache.Add(path, sprite);
                }
            }
            return sprite;
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

        private Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>();
        /// <summary>
        /// 加载预制体模型
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cache"></param>
        public GameObject LoadPrefab(string path, bool cache = true)
        {
            GameObject prefab = null;
            if (!goDic.TryGetValue(path, out prefab))
            {
                prefab = Resources.Load<GameObject>(path);
                if (cache)
                {
                    goDic.Add(path, prefab);
                }
            }
            GameObject go = null;
            if (prefab != null)
            {
                go = Instantiate(prefab);
            }
            return go;
        }

        /// <summary>
        /// 根据mapID获取地图配置信息
        /// </summary>
        /// <param name="mapID"></param>
        /// <returns></returns>
        public MapConfig GetMapConfigByID(int mapID)
        {
            switch (mapID)
            {
                case 101:
                    return new MapConfig
                    {
                        mapID = 101,
                        blueBornPos = new ShawVector3(-27, 0, 0),
                        redBornPos = new ShawVector3(-27, 0, 0),
                        soldierBornDelay = 15000,
                        soldierBornInterval = 2000,
                        soldierWaveInterval = 50000,
                    };
                default:
                    return null;
            }
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
/*            switch (type)
            {
                case EAssetsType.Resources:
                    audio = Resources.Load<AudioClip>(path);
                    break;
                case EAssetsType.AssetBundle:
                    break;
                case EAssetsType.Addressable:
                    break;
            }*/

            _cacheDic.Add(path, audio);

            return audio;
        }

        /// <summary>
        /// 根据heroID获取英雄配置数据
        /// </summary>
        /// <param name="heroID"></param>
        /// <returns></returns>
        public UnitConfig GetHeroConfigByID(int heroID)
        {
            switch (heroID)
            {
                case 101:
                    return new UnitConfig
                    {
                        unitID = 101,
                        unitName = "德玛西亚之力",
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
                    return new UnitConfig
                    {
                        unitID = 102,
                        unitName = "寒冰射手",
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

        /// <summary>
        /// 根据skillID获取技能配置数据
        /// </summary>
        /// <param name="skillID"></param>
        /// <returns></returns>
        public SkillConfig GetSkillConfigByID(int skillID)
        {
            switch (skillID)
            {
                // 盖伦
                case 1010:
                    return new SkillConfig
                    {
                        skillID = 1010,
                        iconName = null,
                        animName = "atk",
                        cdTime = 0,
                        spellTime = 800,
                        isNormalAttack = true,
                        skillTime = 1400,
                        damage = 45,

                        releaseModeType = EReleaseModeType.Click,
                        targetConf = new TargetConfig
                        {
                            skillTargetType = ESkillTargetType.Enemy,
                            selectRuleType = ESelectRuleType.TargetClosestSingle,
                            targetUnits = new EUnitType[]
                            {
                                EUnitType.Hero,
                                EUnitType.Soldier,
                                EUnitType.Tower
                            },
                            selectRange = 2f,
                            searchDis = 10f,
                        }
                    };
                case 1011:
                    /// <summary>
                    /// 致残打击
                    /// 在 接下来 3s内提升30％的移速，并强化下一次普攻，
                    /// 增加其伤害，并沉默命中目标1.5s
                    /// 同时标记目标，持续5s。
                    /// </summary>
                    return new SkillConfig
                    {
                        skillID = 1011,
                        iconName = "arthur_sk1",
                        animName = null,
                        cdTime = 5000,
                        spellTime = 0,
                        isNormalAttack = false,
                        skillTime = 0,
                        damage = 0,

                        releaseModeType = EReleaseModeType.Click,
                        targetConf = null,
                    };
                case 1012:
                    return new SkillConfig
                    {
                        skillID = 1012,
                        iconName = "arther_sk2",
                        animName = null,
                        cdTime = 5000,
                        spellTime = 0,
                        isNormalAttack = false,
                        skillTime = 0,
                        damage = 0,

                        releaseModeType = EReleaseModeType.Click,
                        targetConf = null,
                    };
                case 1013:
                    // 德玛西亚正义
                    return new SkillConfig
                    {
                        skillID = 1013,
                        iconName = "arthur_sk3",
                        animName = "sk3",
                        cdTime = 10000,
                        spellTime = 100,
                        isNormalAttack = false,
                        skillTime = 0,
                        damage = 0,

                        releaseModeType = EReleaseModeType.Click,
                        targetConf = new TargetConfig
                        {
                            skillTargetType = ESkillTargetType.Enemy,
                            selectRuleType = ESelectRuleType.TargetClosestSingle,
                            targetUnits = new EUnitType[]
                            {
                                EUnitType.Hero
                            },
                            selectRange = 4f,
                            searchDis = 4f,
                        }
                    };
                // 艾希
                case 1020:
                    return new SkillConfig
                    {
                        skillID = 1020,
                        iconName = null,
                        animName = "atk",
                        releaseModeType = EReleaseModeType.Click,

                        targetConf = new TargetConfig
                        {
                            skillTargetType = ESkillTargetType.Enemy,
                            selectRuleType = ESelectRuleType.TargetClosestSingle,
                            targetUnits = new EUnitType[]
                            {
                                EUnitType.Hero,
                                EUnitType.Tower,
                                EUnitType.Soldier,
                            },
                            selectRange = 5f,
                            searchDis = 15f,
                        },
                        cdTime = 0,
                        spellTime = 550,
                        isNormalAttack = true,
                        skillTime = 1400,
                        damage = 50,
                    };
                case 1021:
                    return new SkillConfig
                    {
                        skillID = 1021,
                        iconName = "houyi_sk1",
                        animName = null,
                        releaseModeType = EReleaseModeType.Click,

                        targetConf = null,
                        cdTime = 5000,
                        spellTime = 0,
                        isNormalAttack = false,
                        skillTime = 0,
                        damage = 0,
                    };
                case 1022:
                    return new SkillConfig
                    {
                        skillID = 1022,
                        iconName = "houyi_sk2",
                        animName = "sk2",
                        releaseModeType = EReleaseModeType.Position,

                        targetConf = new TargetConfig
                        {
                            skillTargetType = ESkillTargetType.Dynamic,
                            selectRange = 6,
                        },
                        cdTime = 5000,
                        spellTime = 630,
                        isNormalAttack = false,
                        skillTime = 1200,
                        damage = 0,
                    };
                case 1023:
                    return new SkillConfig
                    {
                        skillID = 1023,
                        iconName = "houyi_sk3",
                        animName = "sk3",
                        releaseModeType = EReleaseModeType.Direction,

                        targetConf = null,
                        cdTime = 8000,
                        spellTime = 230,
                        isNormalAttack = false,
                        skillTime = 800,
                        damage = 0,
                        buffIDArr = new int[] { 10230, 10231 },
                    };
            }
            return null;
        }
    }
}