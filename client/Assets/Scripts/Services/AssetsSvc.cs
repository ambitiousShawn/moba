using ShawnFramework.ShawHotUpdate;
using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using XLua;
using XLua.CSObjectWrap;

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

        #region UI预制体
        private Dictionary<string, GameObject> m_PanelCache = new Dictionary<string, GameObject>();
        /// <summary>
        /// UI预制体加载通用方式
        /// </summary>
        /// <param name="packageName">if Resources:pathWithoutName >>> if AssetBundle:packageName </param>
        /// <param name="assetName">if Resources:assetName >>> if AssetBundle:assetName </param>
        /// <param name="assetsType">资源管理的方式</param>
        /// <param name="cache">是否启用缓存</param>
        /// <returns></returns>
        public GameObject LoadUIPrefab(string packageName, string assetName, int type, bool cache = false)
        {
            GameObject panel = null;
            EAssetsType assetsType = (EAssetsType)type;
            if (cache && m_PanelCache.TryGetValue(assetName, out panel))
            {
                return panel;
            }
            switch (assetsType)
            {
                case EAssetsType.Resources:
                    panel = Instantiate(Resources.Load<GameObject>($"{assetName}"));
                    break;
                case EAssetsType.AssetBundle:
                    panel = AssetBundleMgr.Instance.LoadAsset<GameObject>(packageName, assetName);
                    break;
                case EAssetsType.Addressable:
                    break;
            }
            if (cache)
            {
                m_PanelCache.Add(assetName, panel);
            }
            return panel;
        }
        #endregion

        # region 精灵图像
                private Dictionary<string, Sprite> m_SpriteCache = new Dictionary<string, Sprite>();
                /// <summary>
                /// 加载精灵图像
                /// </summary>
                /// <param name="packageName"></param>
                /// <param name="assetName"></param>
                /// <param name="type"></param>
                /// <param name="cache"></param>
                /// <returns></returns>
                public Sprite LoadSprite(string packageName, string assetName, int type, bool cache = false)
                {
                    Sprite sprite = null;
                    EAssetsType assetsType = (EAssetsType)type;
                    if (cache && m_SpriteCache.TryGetValue(assetName, out sprite))
                    {
                        return sprite;   
                    }
                    switch (assetsType)
                    {
                        case EAssetsType.Resources:
                            sprite = Resources.Load<Sprite>(assetName);
                            break;
                        case EAssetsType.AssetBundle:
                            sprite = AssetBundleMgr.Instance.LoadAsset<Sprite>(packageName, assetName);
                            break;
                        case EAssetsType.Addressable:
                            break;
                    }

                    if (cache)
                    {
                        m_SpriteCache.Add(assetName, sprite);
                    }

                    return sprite;
                }
        #endregion

        #region 场景
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
        #endregion

        #region 加载预制体
        private Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>();
        /// <summary>
        /// 加载预制体模型
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cache"></param>
        public GameObject LoadPrefab(string packageName, string assetName, int type, bool cache = false)
        {
            GameObject prefab = null;
            EAssetsType assetsType = (EAssetsType)type;
            if (cache && goDic.TryGetValue(assetName, out prefab))
            {
                return prefab;
            }
            
            switch (assetsType)
            {
                case EAssetsType.Resources:
                    prefab = Instantiate(Resources.Load<GameObject>($"{assetName}"));
                    break;
                case EAssetsType.AssetBundle:
                    prefab = AssetBundleMgr.Instance.LoadAsset<GameObject>(packageName, assetName);
                    break;
                case EAssetsType.Addressable:
                    break;
            }
            if (cache)
            {
                goDic.Add(assetName, prefab);
            }

            return prefab;
        }
        #endregion

        #region 音频切片
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
        #endregion

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

                        // 防御塔
                        towerIDArr = new int[] { 1001, 1002, 2001, 2002 },
                        towerPosArr = new ShawVector3[]
                        {
                            new ShawVector3(-(ShawInt)12.6f, 0, -(ShawInt)0.2f),
                            new ShawVector3(-(ShawInt)24.1f, 0, -(ShawInt)0.2f),
                            new ShawVector3((ShawInt)12.6f, 0, -(ShawInt)0.2f),
                            new ShawVector3((ShawInt)24.1f, 0, -(ShawInt)0.2f),
                        },

                        // 小兵生成
                        soldierBornDelay = 15000,
                        soldierBornInterval = 2000,
                        soldierWaveInterval = 50000,
                        blueSoldierIDArr = new int[] { 1003, 1003, 1004, 1004 },
                        blueSoldierPosArr = new ShawVector3[]
                        {
                            new ShawVector3(-22,0,-(ShawInt)1.7f),
                            new ShawVector3(-22,0,(ShawInt)1.7f),
                            new ShawVector3(-22,0,-(ShawInt)1.7f),
                            new ShawVector3(-22,0,(ShawInt)1.7f),
                        },
                        redSoldierIDArr = new int[] { 2003, 2003, 2004, 2004 },
                        redSoldierPosArr = new ShawVector3[]
                        {
                            new ShawVector3(22,0,-(ShawInt)1.7f),
                            new ShawVector3(22,0,(ShawInt)1.7f),
                            new ShawVector3(22,0,-(ShawInt)1.7f),
                            new ShawVector3(22,0,(ShawInt)1.7f),
                        },
                    };
                default:
                    return null;
            }
        }

        /// <summary>
        /// 根据heroID获取英雄配置数据
        /// </summary>
        /// <param name="unitID"></param>
        /// <returns></returns>
        public UnitConfig GetUnitConfigByID(int unitID)
        {
            switch (unitID)
            {
                case 101:
                    return new UnitConfig
                    {
                        unitID = 101,
                        unitName = "德玛西亚之力",
                        resName = "galen",

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
                        resName = "ashe",

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
                case 1001:
                    return new UnitConfig
                    {
                        unitID = 1001,
                        unitName = "蓝方一塔",
                        resName = "blueTower",
                        hp = 400,
                        defense = 0,
                        colliCfg = new ColliderConfig
                        {
                            mType = ColliderType.Cylinder,
                            mRadius = (ShawInt)0.25f,
                        },
                        skillArr = new int[] { 10010 }
                    };
                case 1002:
                    return new UnitConfig
                    {
                        unitID = 1002,
                        unitName = "蓝方水晶",
                        resName = "blueCrystal",
                        hp = 800,
                        defense = 0,
                        colliCfg = new ColliderConfig
                        {
                            mType = ColliderType.Cylinder,
                            mRadius = (ShawInt)1f,
                        },
                        skillArr = new int[] { 10020 }
                    };
                case 1003:
                    return new UnitConfig
                    {
                        unitID = 1003,
                        unitName = "蓝方近战小兵",
                        resName = "soldier_blue_1",
                        hp = 500,
                        defense = 0,
                        moveSpeed = 2,
                        colliCfg = new ColliderConfig
                        {
                            mType = ColliderType.Cylinder,
                            mRadius = (ShawInt)0.25f,
                        },
                        skillArr = new int[] { 10030 }
                    };
                case 1004:
                    return new UnitConfig
                    {
                        unitID = 1004,
                        unitName = "蓝方远程小兵",
                        resName = "soldier_blue_2",
                        hp = 300,
                        defense = 0,
                        moveSpeed = 2,
                        colliCfg = new ColliderConfig
                        {
                            mType = ColliderType.Cylinder,
                            mRadius = (ShawInt)0.25f,
                        },
                        skillArr = new int[] { 10040 }
                    };
                case 2001:
                    return new UnitConfig
                    {
                        unitID = 2001,
                        unitName = "红方一塔",
                        resName = "redTower",
                        hp = 400,
                        defense = 0,
                        colliCfg = new ColliderConfig
                        {
                            mType = ColliderType.Cylinder,
                            mRadius = (ShawInt)0.25f,
                        },
                        skillArr = new int[] { 20010 }
                    };
                case 2002:
                    return new UnitConfig
                    {
                        unitID = 2002,
                        unitName = "红方水晶",
                        resName = "redCrystal",
                        hp = 800,
                        defense = 0,
                        colliCfg = new ColliderConfig
                        {
                            mType = ColliderType.Cylinder,
                            mRadius = (ShawInt)1f,
                        },
                        skillArr = new int[] { 20020 }
                    };
                case 2003:
                    return new UnitConfig
                    {
                        unitID = 2003,
                        unitName = "红方近战小兵",
                        resName = "soldier_red_1",
                        hp = 500,
                        defense = 0,
                        moveSpeed = 2,
                        colliCfg = new ColliderConfig
                        {
                            mType = ColliderType.Cylinder,
                            mRadius = (ShawInt)0.25f,
                        },
                        skillArr = new int[] { 20030 }
                    };
                case 2004:
                    return new UnitConfig
                    {
                        unitID = 2004,
                        unitName = "红方远程小兵",
                        resName = "soldier_red_2",
                        hp = 300,
                        defense = 0,
                        moveSpeed = 2,
                        colliCfg = new ColliderConfig
                        {
                            mType = ColliderType.Cylinder,
                            mRadius = (ShawInt)0.25f,
                        },
                        skillArr = new int[] { 20040 }
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
                // 蓝方塔
                case 10010:
                    return new SkillConfig
                    {
                        skillID = 10010,
                        iconName = null,
                        animName = null,
                        releaseModeType = EReleaseModeType.None,
                        //最近的敌方目标
                        targetConf = new TargetConfig
                        {
                            skillTargetType = ESkillTargetType.Enemy,
                            selectRuleType = ESelectRuleType.TargetClosestSingle,
                            targetUnits = new EUnitType[]
                            {
                                EUnitType.Hero,
                                EUnitType.Soldier,
                            },
                            selectRange = 6f,
                            searchDis = 0f,
                        },
                        // bulletCfg = new BulletCfg
                        // {
                        //     bulletType = BulletTypeEnum.SkillTarget,//技能锁定的目标
                        //     bulletName = "蓝方防御塔攻击子弹",
                        //     resPath = "tower_ska_bullet",
                        //     bulletSpeed = 1f,
                        //     bulletSize = 0.1f,
                        //     bulletHeight = 4f,//子弹出发点高度，如果是方向指向技能，则子弹一直保持这个高度
                        //     bulletOffset = 0,
                        //     bulletDelay = 0,
                        // },
                        cdTime = 0,
                        spellTime = 1000,//施法时间（技能前摇）
                        isNormalAttack = true,
                        skillTime = 2000,
                        damage = 50,
                    };
                // 蓝方水晶
                case 10020:
                    return new SkillConfig
                    {
                        skillID = 10020,
                        iconName = null,
                        animName = null,
                        releaseModeType = EReleaseModeType.None,
                        //最近的敌方目标
                        targetConf = new TargetConfig
                        {
                            skillTargetType = ESkillTargetType.Enemy,
                            selectRuleType = ESelectRuleType.TargetClosestSingle,
                            targetUnits = new EUnitType[]
                            {
                                EUnitType.Hero,
                                EUnitType.Soldier,
                            },
                            selectRange = 6f,
                            searchDis = 0f,
                        },
                        // bulletCfg = new BulletCfg
                        // {
                        //     bulletType = BulletTypeEnum.SkillTarget,//技能锁定的目标
                        //     bulletName = "蓝方水晶攻击子弹",
                        //     resPath = "tower_ska_bullet",
                        //     bulletSpeed = 1f,
                        //     bulletSize = 0.1f,
                        //     bulletHeight = 2.5f,
                        //     bulletOffset = 0,
                        //     bulletDelay = 0,
                        // },
                        cdTime = 0,
                        spellTime = 1000,//施法时间（技能前摇）
                        isNormalAttack = true,
                        skillTime = 2000,
                        damage = 100,
                    };
                // 红方塔
                case 20010:
                    return new SkillConfig
                    {
                        skillID = 20010,
                        iconName = null,
                        animName = null,
                        releaseModeType = EReleaseModeType.None,
                        //最近的敌方目标
                        targetConf = new TargetConfig
                        {
                            skillTargetType = ESkillTargetType.Enemy,
                            selectRuleType = ESelectRuleType.TargetClosestSingle,
                            targetUnits = new EUnitType[]
                            {
                                EUnitType.Hero,
                                EUnitType.Soldier,
                            },
                            selectRange = 6f,
                            searchDis = 0f,
                        },
                        // bulletCfg = new BulletCfg
                        // {
                        //     bulletType = BulletTypeEnum.SkillTarget,//技能锁定的目标
                        //     bulletName = "蓝方防御塔攻击子弹",
                        //     resPath = "tower_ska_bullet",
                        //     bulletSpeed = 1f,
                        //     bulletSize = 0.1f,
                        //     bulletHeight = 4f,//子弹出发点高度，如果是方向指向技能，则子弹一直保持这个高度
                        //     bulletOffset = 0,
                        //     bulletDelay = 0,
                        // },
                        cdTime = 0,
                        spellTime = 1000,//施法时间（技能前摇）
                        isNormalAttack = true,
                        skillTime = 2000,
                        damage = 50,
                    };
                // 红方水晶
                case 20020:
                    return new SkillConfig
                    {
                        skillID = 20020,
                        iconName = null,
                        animName = null,
                        releaseModeType = EReleaseModeType.None,
                        //最近的敌方目标
                        targetConf = new TargetConfig
                        {
                            skillTargetType = ESkillTargetType.Enemy,
                            selectRuleType = ESelectRuleType.TargetClosestSingle,
                            targetUnits = new EUnitType[]
                            {
                                EUnitType.Hero,
                                EUnitType.Soldier,
                            },
                            selectRange = 6f,
                            searchDis = 0f,
                        },
                        // bulletCfg = new BulletCfg
                        // {
                        //     bulletType = BulletTypeEnum.SkillTarget,//技能锁定的目标
                        //     bulletName = "蓝方水晶攻击子弹",
                        //     resPath = "tower_ska_bullet",
                        //     bulletSpeed = 1f,
                        //     bulletSize = 0.1f,
                        //     bulletHeight = 2.5f,
                        //     bulletOffset = 0,
                        //     bulletDelay = 0,
                        // },
                        cdTime = 0,
                        spellTime = 1000,//施法时间（技能前摇）
                        isNormalAttack = true,
                        skillTime = 2000,
                        damage = 100,
                    };
                // 蓝方近战小兵
                case 10030:
                    return new SkillConfig
                    {
                        skillID = 10030,
                        iconName = null,
                        animName = "attack",
                        releaseModeType = EReleaseModeType.None,
                        //最近的敌方目标
                        targetConf = new TargetConfig
                        {
                            skillTargetType = ESkillTargetType.Enemy,
                            selectRuleType = ESelectRuleType.TargetClosestSingle,
                            targetUnits = new EUnitType[] 
                            {
                                EUnitType.Hero,
                                EUnitType.Soldier,
                                EUnitType.Tower,
                            },
                            selectRange = 1.5f,
                            searchDis = 5f,
                        },
                        cdTime = 0,
                        spellTime = 400,//施法时间（技能前摇）
                        isNormalAttack = true,
                        skillTime = 1200,
                        damage = 20,
                    };
                // 蓝方远程小兵
                case 10040:
                    return new SkillConfig
                    {
                        skillID = 10040,
                        iconName = null,
                        animName = "attack",
                        releaseModeType = EReleaseModeType.None,
                        //最近的敌方目标
                        targetConf = new TargetConfig
                        {
                            skillTargetType = ESkillTargetType.Enemy,
                            selectRuleType = ESelectRuleType.TargetClosestSingle,
                            targetUnits = new EUnitType[]
                            {
                                EUnitType.Hero,
                                EUnitType.Soldier,
                                EUnitType.Tower,
                            },
                            selectRange = 4f,
                            searchDis = 7f,
                        },
                        // bulletCfg = new BulletCfg
                        // {
                        //     bulletType = BulletTypeEnum.SkillTarget,//技能锁定的目标
                        //     bulletName = "蓝方防远程小兵攻击子弹",
                        //     resPath = "bluesoldier_ska_bullet",
                        //     bulletSpeed = 0.5f,
                        //     bulletSize = 0.1f,
                        //     bulletHeight = 0.6f,//子弹出发点高度，如果是方向指向技能，则子弹一直保持这个高度
                        //     bulletOffset = 0,
                        //     bulletDelay = 0,
                        // },
                        cdTime = 0,
                        spellTime = 400,//施法时间（技能前摇）
                        isNormalAttack = true,
                        skillTime = 1200,
                        damage = 30,
                    };
                // 红方近战小兵
                case 20030:
                    return new SkillConfig
                    {
                        skillID = 20030,
                        iconName = null,
                        animName = "attack",
                        releaseModeType = EReleaseModeType.None,
                        //最近的敌方目标
                        targetConf = new TargetConfig
                        {
                            skillTargetType = ESkillTargetType.Enemy,
                            selectRuleType = ESelectRuleType.TargetClosestSingle,
                            targetUnits = new EUnitType[]
                            {
                                EUnitType.Hero,
                                EUnitType.Soldier,
                                EUnitType.Tower,
                            },
                            selectRange = 1.5f,
                            searchDis = 5f,
                        },
                        cdTime = 0,
                        spellTime = 400,//施法时间（技能前摇）
                        isNormalAttack = true,
                        skillTime = 1200,
                        damage = 20,
                    };
                // 红方远程小兵
                case 20040:
                    return new SkillConfig
                    {
                        skillID = 20040,
                        iconName = null,
                        animName = "attack",
                        releaseModeType = EReleaseModeType.None,
                        //最近的敌方目标
                        targetConf = new TargetConfig
                        {
                            skillTargetType = ESkillTargetType.Enemy,
                            selectRuleType = ESelectRuleType.TargetClosestSingle,
                            targetUnits = new EUnitType[]
                            {
                                EUnitType.Hero,
                                EUnitType.Soldier,
                                EUnitType.Tower,
                            },
                            selectRange = 4f,
                            searchDis = 7f,
                        },
                        // bulletCfg = new BulletCfg
                        // {
                        //     bulletType = BulletTypeEnum.SkillTarget,//技能锁定的目标
                        //     bulletName = "蓝方防远程小兵攻击子弹",
                        //     resPath = "bluesoldier_ska_bullet",
                        //     bulletSpeed = 0.5f,
                        //     bulletSize = 0.1f,
                        //     bulletHeight = 0.6f,//子弹出发点高度，如果是方向指向技能，则子弹一直保持这个高度
                        //     bulletOffset = 0,
                        //     bulletDelay = 0,
                        // },
                        cdTime = 0,
                        spellTime = 400,//施法时间（技能前摇）
                        isNormalAttack = true,
                        skillTime = 1200,
                        damage = 30,
                    };
                    
            }
            return null;
        }
    }
}