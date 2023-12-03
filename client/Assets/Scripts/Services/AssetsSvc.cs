using ShawnFramework.ShawHotUpdate;
using ShawnFramework.ShawLog;
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
    /// �ʲ�����ʽ
    /// </summary>
    [LuaCallCSharp]
    public enum EAssetsType
    {
        Resources = 0,
        AssetBundle = 1,
        Addressable = 2,
    }

    /// <summary>
    /// �ʲ�����
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

        #region UIԤ����
        private Dictionary<string, GameObject> m_PanelCache = new Dictionary<string, GameObject>();
        /// <summary>
        /// UIԤ�������ͨ�÷�ʽ
        /// </summary>
        /// <param name="packageName">if Resources:pathWithoutName >>> if AssetBundle:packageName </param>
        /// <param name="assetName">if Resources:assetName >>> if AssetBundle:assetName </param>
        /// <param name="assetsType">��Դ����ķ�ʽ</param>
        /// <param name="cache">�Ƿ����û���</param>
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

        # region ����ͼ��
                private Dictionary<string, Sprite> m_SpriteCache = new Dictionary<string, Sprite>();
                /// <summary>
                /// ���ؾ���ͼ��
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

        #region ����
        private Action prgCB = null;
        /// <summary>
        /// �첽���س���
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

        #region ����Ԥ����
        private Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>();
        /// <summary>
        /// ����Ԥ����ģ��
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

        #region ��Ƶ��Ƭ
        private Dictionary<string, AudioClip> _cacheDic = new Dictionary<string, AudioClip>();
        /// <summary>
        /// ������Ƶ��Ƭ
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AudioClip LoadAudioClip(string path)
        {
            AudioClip audio = null;
            if (_cacheDic.TryGetValue(path, out audio))
                return audio;

            // TODO:��Դ����
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
        /// ����mapID��ȡ��ͼ������Ϣ
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

                        // ������
                        towerIDArr = new int[] { 1001, 1002, 2001, 2002 },
                        towerPosArr = new ShawVector3[]
                        {
                            new ShawVector3(-(ShawInt)12.6f, 0, -(ShawInt)0.2f),
                            new ShawVector3(-(ShawInt)24.1f, 0, -(ShawInt)0.2f),
                            new ShawVector3((ShawInt)12.6f, 0, -(ShawInt)0.2f),
                            new ShawVector3((ShawInt)24.1f, 0, -(ShawInt)0.2f),
                        },

                        // С������
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
        /// ����heroID��ȡӢ����������
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
                        unitName = "��������֮��",
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
                        unitName = "��������",
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
                        unitName = "����һ��",
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
                        unitName = "����ˮ��",
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
                        unitName = "������սС��",
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
                        unitName = "����Զ��С��",
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
                        unitName = "�췽һ��",
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
                        unitName = "�췽ˮ��",
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
                        unitName = "�췽��սС��",
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
                        unitName = "�췽Զ��С��",
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
        /// ����skillID��ȡ������������
        /// </summary>
        /// <param name="skillID"></param>
        /// <returns></returns>
        public SkillConfig GetSkillConfigByID(int skillID)
        {
            switch (skillID)
            {
                // ����
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
                        },
                        bulletConf = null,

                        buffIDArr = null,
                        audio_start = "arthur_ska_rls",
                        audio_work = null,
                        audio_hit = "arthur_ska_hit"
                    };
                case 1011:
                    /// <summary>
                    /// �²д��
                    /// �� ������ 3s������30�������٣���ǿ����һ���չ���
                    /// �������˺�������Ĭ����Ŀ��1.5s
                    /// ͬʱ���Ŀ�꣬����5s��
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

                        buffIDArr = new int[] { 10110, 10111 },
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

                        buffIDArr = new int[] { 10120 },
                    };
                case 1013:
                    // ������������
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
                        },

                        buffIDArr = new int[] { 10130, 10131, 10132, 10133 },
                    };
                case 1014:
                    return new SkillConfig
                    {
                        skillID = 1010,
                        animName = "sk1_atk",
                        releaseModeType = EReleaseModeType.None,
                        //����ĵз�Ŀ��
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
                        },
                        bulletConf = null,
                        cdTime = 0,
                        spellTime = 800,//ʩ��ʱ�䣨����ǰҡ��
                        isNormalAttack = true,
                        skillTime = 1400,
                        damage = 90,
                        //��Ĭbuff
                        buffIDArr = new int[] { 10140, 10141, 10142 },

                        //audio
                        audio_start = null,
                        audio_work = null,
                        audio_hit = "arthur_sk1_hit",
                    };
                // ��ϣ
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
                // ������
                case 10010:
                    return new SkillConfig
                    {
                        skillID = 10010,
                        iconName = null,
                        animName = null,
                        releaseModeType = EReleaseModeType.None,
                        //����ĵз�Ŀ��
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
                        //     bulletType = BulletTypeEnum.SkillTarget,//����������Ŀ��
                        //     bulletName = "���������������ӵ�",
                        //     resPath = "tower_ska_bullet",
                        //     bulletSpeed = 1f,
                        //     bulletSize = 0.1f,
                        //     bulletHeight = 4f,//�ӵ�������߶ȣ�����Ƿ���ָ���ܣ����ӵ�һֱ��������߶�
                        //     bulletOffset = 0,
                        //     bulletDelay = 0,
                        // },
                        cdTime = 0,
                        spellTime = 1000,//ʩ��ʱ�䣨����ǰҡ��
                        isNormalAttack = true,
                        skillTime = 2000,
                        damage = 50,
                    };
                // ����ˮ��
                case 10020:
                    return new SkillConfig
                    {
                        skillID = 10020,
                        iconName = null,
                        animName = null,
                        releaseModeType = EReleaseModeType.None,
                        //����ĵз�Ŀ��
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
                        //     bulletType = BulletTypeEnum.SkillTarget,//����������Ŀ��
                        //     bulletName = "����ˮ�������ӵ�",
                        //     resPath = "tower_ska_bullet",
                        //     bulletSpeed = 1f,
                        //     bulletSize = 0.1f,
                        //     bulletHeight = 2.5f,
                        //     bulletOffset = 0,
                        //     bulletDelay = 0,
                        // },
                        cdTime = 0,
                        spellTime = 1000,//ʩ��ʱ�䣨����ǰҡ��
                        isNormalAttack = true,
                        skillTime = 2000,
                        damage = 100,
                    };
                // �췽��
                case 20010:
                    return new SkillConfig
                    {
                        skillID = 20010,
                        iconName = null,
                        animName = null,
                        releaseModeType = EReleaseModeType.None,
                        //����ĵз�Ŀ��
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
                        //     bulletType = BulletTypeEnum.SkillTarget,//����������Ŀ��
                        //     bulletName = "���������������ӵ�",
                        //     resPath = "tower_ska_bullet",
                        //     bulletSpeed = 1f,
                        //     bulletSize = 0.1f,
                        //     bulletHeight = 4f,//�ӵ�������߶ȣ�����Ƿ���ָ���ܣ����ӵ�һֱ��������߶�
                        //     bulletOffset = 0,
                        //     bulletDelay = 0,
                        // },
                        cdTime = 0,
                        spellTime = 1000,//ʩ��ʱ�䣨����ǰҡ��
                        isNormalAttack = true,
                        skillTime = 2000,
                        damage = 50,
                    };
                // �췽ˮ��
                case 20020:
                    return new SkillConfig
                    {
                        skillID = 20020,
                        iconName = null,
                        animName = null,
                        releaseModeType = EReleaseModeType.None,
                        //����ĵз�Ŀ��
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
                        //     bulletType = BulletTypeEnum.SkillTarget,//����������Ŀ��
                        //     bulletName = "����ˮ�������ӵ�",
                        //     resPath = "tower_ska_bullet",
                        //     bulletSpeed = 1f,
                        //     bulletSize = 0.1f,
                        //     bulletHeight = 2.5f,
                        //     bulletOffset = 0,
                        //     bulletDelay = 0,
                        // },
                        cdTime = 0,
                        spellTime = 1000,//ʩ��ʱ�䣨����ǰҡ��
                        isNormalAttack = true,
                        skillTime = 2000,
                        damage = 100,
                    };
                // ������սС��
                case 10030:
                    return new SkillConfig
                    {
                        skillID = 10030,
                        iconName = null,
                        animName = "attack",
                        releaseModeType = EReleaseModeType.None,
                        //����ĵз�Ŀ��
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
                        spellTime = 400,//ʩ��ʱ�䣨����ǰҡ��
                        isNormalAttack = true,
                        skillTime = 1200,
                        damage = 20,
                    };
                // ����Զ��С��
                case 10040:
                    return new SkillConfig
                    {
                        skillID = 10040,
                        iconName = null,
                        animName = "attack",
                        releaseModeType = EReleaseModeType.None,
                        //����ĵз�Ŀ��
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
                        //     bulletType = BulletTypeEnum.SkillTarget,//����������Ŀ��
                        //     bulletName = "������Զ��С�������ӵ�",
                        //     resPath = "bluesoldier_ska_bullet",
                        //     bulletSpeed = 0.5f,
                        //     bulletSize = 0.1f,
                        //     bulletHeight = 0.6f,//�ӵ�������߶ȣ�����Ƿ���ָ���ܣ����ӵ�һֱ��������߶�
                        //     bulletOffset = 0,
                        //     bulletDelay = 0,
                        // },
                        cdTime = 0,
                        spellTime = 400,//ʩ��ʱ�䣨����ǰҡ��
                        isNormalAttack = true,
                        skillTime = 1200,
                        damage = 30,
                    };
                // �췽��սС��
                case 20030:
                    return new SkillConfig
                    {
                        skillID = 20030,
                        iconName = null,
                        animName = "attack",
                        releaseModeType = EReleaseModeType.None,
                        //����ĵз�Ŀ��
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
                        spellTime = 400,//ʩ��ʱ�䣨����ǰҡ��
                        isNormalAttack = true,
                        skillTime = 1200,
                        damage = 20,
                    };
                // �췽Զ��С��
                case 20040:
                    return new SkillConfig
                    {
                        skillID = 20040,
                        iconName = null,
                        animName = "attack",
                        releaseModeType = EReleaseModeType.None,
                        //����ĵз�Ŀ��
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
                        //     bulletType = BulletTypeEnum.SkillTarget,//����������Ŀ��
                        //     bulletName = "������Զ��С�������ӵ�",
                        //     resPath = "bluesoldier_ska_bullet",
                        //     bulletSpeed = 0.5f,
                        //     bulletSize = 0.1f,
                        //     bulletHeight = 0.6f,//�ӵ�������߶ȣ�����Ƿ���ָ���ܣ����ӵ�һֱ��������߶�
                        //     bulletOffset = 0,
                        //     bulletDelay = 0,
                        // },
                        cdTime = 0,
                        spellTime = 400,//ʩ��ʱ�䣨����ǰҡ��
                        isNormalAttack = true,
                        skillTime = 1200,
                        damage = 30,
                    };
                    
            }
            return null;
        }

        /// <summary>
        /// ����buffID��ȡ����Ӧ��Buff����
        /// </summary>
        /// <param name="buffID"></param>
        /// <returns></returns>
        public BuffConfig GetBuffConfigByID(int buffID)
        {
            switch (buffID)
            {
                case 10100:
                    return new HPCureBuffConfig
                    {
                        //ͨ��buff����
                        buffID = 10100,
                        buffName = "��������",
                        buffType = EBuffType.HPCure,

                        attacher = EAttachType.Caster,
                        impacter = null,

                        buffDelay = 0,
                        buffInterval = 2000,
                        buffDuration = -1,

                        //ר������
                        cureHPpercent = 2,
                    };
                //Arthur1����
                case 10110://���ټ���
                    return new MoveSpeedBuffConfig
                    {
                        //ͨ��buff����
                        buffID = 10110,
                        buffName = "����",
                        buffType = EBuffType.MoveSpeedUp,

                        attacher = EAttachType.Caster,
                        impacter = null,

                        buffDelay = 0,
                        buffInterval = 0,
                        buffDuration = 3000,

                        //ר�����ԣ�����30%
                        amount = 30,
                    };
                case 10111:
                    return new CommonModifySkillBuffConfig
                    {
                        //ͨ��buff����
                        buffID = 10111,
                        buffName = "�滻�չ�",
                        buffType = EBuffType.ModifySkill,

                        attacher = EAttachType.Caster,
                        impacter = null,

                        buffDelay = 0,
                        buffInterval = 0,
                        buffDuration = 3000,

                        //ר������
                        originalID = 1010,
                        replaceID = 1014
                    };
                // case 10140:
                //     return ResBuffConfigs.buff_10140;
                // case 10141:
                //     return ResBuffConfigs.buff_10141;
                // case 10142:
                //     return ResBuffConfigs.buff_10142;
                // //Arthur2����
                // case 10120:
                //     return ResBuffConfigs.buff_10120;
                // //Arthur3����
                // case 10130:
                //     return ResBuffConfigs.buff_10130;
                // case 10131:
                //     return ResBuffConfigs.buff_10131;
                // case 10132:
                //     return ResBuffConfigs.buff_10132;
                // case 10133:
                //     return ResBuffConfigs.buff_10133;
                // //Houyi��������
                // case 10200:
                //     return ResBuffConfigs.buff_10200;
                // case 10201:
                //     return ResBuffConfigs.buff_10201;
                // case 10250:
                //     return ResBuffConfigs.buff_10250;
                // //Houyi1����
                // case 10210://�����滻
                //     return ResBuffConfigs.buff_10210;
                // case 10240://scatter
                //     return ResBuffConfigs.buff_10240;
                // case 10260://mixed
                //     return ResBuffConfigs.buff_10260;
                // //Houyi2����
                // case 10220:
                //     return ResBuffConfigs.buff_10220;
                // case 10221:
                //     return ResBuffConfigs.buff_10221;
                // case 10222:
                //     return ResBuffConfigs.buff_10222;
                // case 10223:
                //     return ResBuffConfigs.buff_10223;
                // //Houyi3����
                // case 10230:
                //     return ResBuffConfigs.buff_10230;
                // case 10231:
                //     return ResBuffConfigs.buff_10231;
                // //ͨ��
                // case 90000:
                //     return ResBuffConfigs.buff_90000;
                default:
                    break;
            }
            LogCore.Error("Get Buff Config Failed,buffID:" + buffID);
            return null;
        }

        public BuffLogic CreateBuff(MainLogicUnit source, MainLogicUnit owner, Skill skill, int buffID, object[] args)
        {
            BuffConfig cfg = GetBuffConfigByID(buffID);
            switch (cfg.buffType)
            {
                // case EBuffType.MoveAttack:
                //     return new MoveAttackBuff(source, owner, skill, buffID, args);
                case EBuffType.MoveSpeedUp:
                     return new MoveSpeedUpBuff(source, owner, skill, buffID, args);
                // case EBuffType.MoveSpeed_DynamicGroup:
                //     return new MoveSpeedBuff_DynamicGroup(source, owner, skill, buffID, args);
                case EBuffType.ModifySkill:
                     return new CommonModifySkillBuff(source, owner, skill, buffID, args);
                // case EBuffType.Silense:
                //     return new SilenseBuff_Single(source, owner, skill, buffID, args);
                // case EBuffType.ArthurMark:
                //     return new ArthurMarkBuff(source, owner, skill, buffID, args);
                case EBuffType.HPCure:
                    return new HPCureBuff(source, owner, skill, buffID, args);
                // case EBuffType.Knockup_Group:
                //     return new KnockUpBuff_Group(source, owner, skill, buffID, args);
                // 
                // case EBuffType.Damage_DynamicGroup:
                //     return new DamageBuff_DynamicGroup(source, owner, skill, buffID, args);
                // case EBuffType.TargetFlashMove:
                //     return new TargetFlashMoveBuff(source, owner, skill, buffID, args);
                // case EBuffType.ExecuteDamage:
                //     return new ExecuteDamageBuff(source, owner, skill, buffID, args);
                // case EBuffType.Damage_StaticGroup:
                //     return new DamageBuff_StaticGroup(source, owner, skill, buffID, args);
                // 
                // case EBuffType.HouyiPasvAttackSpeed:
                //     return new HouyiPasvAttackSpeedBuff(source, owner, skill, buffID, args);
                // case EBuffType.HouyiPasvSkillModify:
                //     return new HouyiMultipleSkillModifyBuff(source, owner, skill, buffID, args);
                // case EBuffType.HouyiPasvMultiArrow:
                //     return new HouyiMultipleArrowBuff(source, owner, skill, buffID, args);
                // case EBuffType.HouyiActiveSkillModify:
                //     return new HouyiScatterSkillModifyBuff(source, owner, skill, buffID, args);
                // case EBuffType.Scatter:
                //     return new HouyiScatterArrowBuff(source, owner, skill, buffID, args);
                // case EBuffType.HouyiMixedMultiScatter:
                //     return new HouyiMixedMultiScatterBuff(source, owner, skill, buffID, args);
                // case EBuffType.MoveSpeed_StaticGroup:
                //     return new MoveSpeedBuff_StaticGroup(source, owner, skill, buffID, args);
                // 
                // case EBuffType.Stun_Single_DynamicTime:
                //     return new StunBuff_DynamicTime(source, owner, skill, buffID, args);
                // //TOADD
                // case EBuffType.None:
                default:
                    LogCore.Error("Create Buff Failed,BuffID:" + buffID);
                    return null;
            }
        }

        public BulletLogic CreateBullet(MainLogicUnit source, MainLogicUnit target, Skill skill)
        {
            switch (skill.config.bulletConf.bulletType)
            {
                case EBulletType.SkillTarget:
                    return new TargetBullet(source, target, skill);
                case EBulletType.UIDirection:
                    // return new DirectionBullet(source, skill);
                case EBulletType.UIPosition:
                case EBulletType.BuffSearch:
                default:
                    LogCore.Error("Create Bullet Error.");
                    return null;
            }
        }
    }
}