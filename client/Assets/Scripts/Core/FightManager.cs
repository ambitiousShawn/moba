using GameProtocol;
using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class FightManager : MonoBehaviour
{
    public static FightManager Instance;

    Transform cameraFollowTarget = null; // 相机跟随目标
    public bool IsFightTick = false;    // 是否开始战斗打点

    void Awake()
    {
        Instance = this;
    }

    List<ShawColliderBase> colliderLst; // 存储环境中所有碰撞体
    List<HeroLogic> heroList = new List<HeroLogic>();
    
    /// <summary>
    /// 外部获取所有环境碰撞
    /// </summary>
    /// <returns></returns>
    public List<ShawColliderBase> GetAllEnvColliders()
    {
        return colliderLst;
    }

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
}
