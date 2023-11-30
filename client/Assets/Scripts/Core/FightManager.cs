using GameProtocol;
using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class FightManager : MonoBehaviour
{
    public static FightManager Instance;

    public UGUI_PlayPanel playWnd; // Lua中赋值

    public float SkillDisMultipler = 0.03f; // 技能距离的乘法系数

    void Awake()
    {
        Instance = this;
    }

    #region 初始化相关(包括相机跟随)
    
    List<ShawColliderBase> colliderLst; // 存储环境中所有碰撞体
    /// <summary>
    /// 初始化场景碰撞环境
    /// </summary>
    /// <returns>存储所有场景碰撞体的容器</returns>
    public void InitCollisionEnv()
    {
        // 生成碰撞配置
        Transform transEnvRoot = GameObject.Find("MapRoot/EnvCollider").transform;
        if (transEnvRoot == null)
        {
            LogCore.Error($"未在场景中找到\"MapRoot/EnvCollider\" ");
        }

        List<ColliderConfig> envColliCfgLst = new List<ColliderConfig>();
        BoxCollider[] boxArr = transEnvRoot.GetComponentsInChildren<BoxCollider>();
        for (int i = 0; i < boxArr.Length; i++)
        {
            Transform trans = boxArr[i].transform;
            UnityEngine.Vector3 pos = trans.position;
            UnityEngine.Vector3 scale = trans.localScale / 2;
            UnityEngine.Vector3 right = trans.right;
            UnityEngine.Vector3 up = trans.up;
            UnityEngine.Vector3 forward = trans.forward;
            ColliderConfig cfg = new ColliderConfig
            {
                mPos = new ShawVector3(pos),
            };
            cfg.mName = trans.name;
            cfg.mSize = new ShawVector3(scale);
            cfg.mType = ColliderType.Box;
            cfg.mAxis = new ShawVector3[3];
            cfg.mAxis[0] = new ShawVector3(right);
            cfg.mAxis[1] = new ShawVector3(up);
            cfg.mAxis[2] = new ShawVector3(forward);

            envColliCfgLst.Add(cfg);
        }

        CapsuleCollider[] cylindderArr = transEnvRoot.GetComponentsInChildren<CapsuleCollider>();
        for (int i = 0; i < cylindderArr.Length; i++)
        {
            Transform trans = cylindderArr[i].transform;
            UnityEngine.Vector3 pos = trans.position;
            ColliderConfig cfg = new ColliderConfig
            {
                mPos = new ShawVector3(pos)
            };
            cfg.mName = trans.name;
            cfg.mType = ColliderType.Box;
            cfg.mRadius = (ShawInt)(trans.localScale.x / 2);

            envColliCfgLst.Add(cfg);
        }

        EnvColliders logicEnv = new EnvColliders
        {
            colliderConfigLst = envColliCfgLst
        };

        logicEnv.Init();

        colliderLst = logicEnv.GetAllEnvColliders();
    }


    List<HeroLogic> heroList = new List<HeroLogic>();
    /// <summary>
    /// 初始化英雄单位
    /// </summary>
    /// <param name="battleHeroDatas"></param>
    /// <param name="mapCfg"></param>
    public void InitHero(List<BattleHeroData> battleHeroDatas, MapConfig mapCfg)
    {
        int cnt = battleHeroDatas.Count / 2;
        for (int i = 0; i < cnt; i++)
        {
            HeroData hd = new HeroData
            {
                heroID = battleHeroDatas[i].heroID,
                posIndex = i,
                userName = battleHeroDatas[i].userName,
                unitCfg = AssetsSvc.Instance.GetHeroConfigByID(battleHeroDatas[i].heroID),
            };
            HeroLogic hero;
            if (i < cnt)
            {
                hd.teamType = ETeamType.Blue;
                hd.bornPos = mapCfg.blueBornPos;
                hero = new HeroLogic(hd);
            }
            else
            {
                hd.teamType = ETeamType.Red;
                hd.bornPos = mapCfg.redBornPos;
                hero = new HeroLogic(hd);
            }
            hero.LogicInit();
            heroList.Add(hero);
        }
    }

    Transform transCameraRoot = null;
    Transform cameraFollowTarget = null; // 相机跟随目标
    /// <summary>
    /// 初始化相机
    /// </summary>
    public void InitCamera(int posIndex)
    {
        cameraFollowTarget = heroList[posIndex].mainViewUnit.transform;
    }

    private void LateUpdate()
    {
        // 相机跟随
        if (cameraFollowTarget == null) 
        {
            transCameraRoot = GameObject.Find("MapRoot/CameraRoot").transform;
        }
        else
        {
            transCameraRoot.position =  cameraFollowTarget.position;
        }
    }

    #endregion

    #region 游戏运行过程中
    public void Tick()
    {
        for (int i = 0; i < heroList.Count; i++)
        {
            heroList[i].LogicTick();
        }
    }

    public void InputKey(List<OpKey> keyList)
    {
        for (int i = 0; i < keyList.Count; i++)
        {
            OpKey key = keyList[i];
            MainLogicUnit hero = heroList[key.opIndex];
            hero.InputKey(key);
        }
    }

    uint keyID = 0;
    public uint KeyID => ++keyID;
    // 移动操作请求
    public void SendMoveOperation(ShawVector3 logicDir)
    {
        GameMsg msg = new()
        {
            cmd = CMD.SndOpKey,
            sndOpKey = new SndOpKey
            {
                roomID = Launcher.Instance.RoomID,
                opKey = new OpKey
                {
                    opIndex = Launcher.Instance.SelfIndex,
                    keyType = EKeyType.Move,
                    moveKey = new MoveKey
                    {
                        x = logicDir.x.ScaledValue,
                        z = logicDir.z.ScaledValue,
                        keyID = keyID,
                    },
                },
            }
        };
    
        NetSvc.Instance.SendMsg(msg);
    }

    // 技能操作请求
    public void SendSkillOperation(int skillID, Vector3 vec)
    {
        GameMsg msg = new GameMsg
        {
            cmd = CMD.SndOpKey,
            sndOpKey = new SndOpKey
            {
                roomID = Launcher.Instance.RoomID,
                opKey = new OpKey
                {
                    opIndex = Launcher.Instance.SelfIndex,
                    keyType = EKeyType.Skill,
                    skillKey = new SkillKey
                    {
                        skillID = (uint)skillID,
                        x = ((ShawInt)vec.x).ScaledValue,
                        z = ((ShawInt)vec.z).ScaledValue,
                    }
                }
            }
        };

        NetSvc.Instance.SendMsg(msg);
    }
    #endregion

    #region API Func
    /// <summary>
    /// 获取当前客户端的逻辑实体
    /// </summary>
    /// <param name="posIndex"></param>
    /// <returns></returns>
    public MainLogicUnit GetSelfHero(int posIndex)
    {
        return heroList[posIndex];
    }

    /// <summary>
    /// 外部获取所有环境碰撞
    /// </summary>
    /// <returns></returns>
    public List<ShawColliderBase> GetAllEnvColliders()
    {
        return colliderLst;
    }
    #endregion
}
