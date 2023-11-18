using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class LuaCallFunc : MonoBehaviour
{
    public static LuaCallFunc Instance;

    private void Awake()
    {
        Instance = this;
    }

    #region 战斗模块通用API
    /// <summary>
    /// 初始化场景碰撞环境
    /// </summary>
    /// <returns>存储所有场景碰撞体的容器</returns>
    public List<ShawColliderBase> InitCollisionEnv()
    {
        // 生成碰撞配置
        Transform transEnvRoot = GameObject.Find("MapRoot/EnvCollider").transform;
        if (transEnvRoot == null ) 
        {
            LogCore.Error($"未在场景中找到 {0}", "MapRoot/EnvCollider");
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
                mPos = new ShawVector3((ShawInt)pos.x, (ShawInt)pos.y, (ShawInt)pos.z)
            };
            cfg.mName = trans.name;
            cfg.mSize = new ShawVector3((ShawInt)scale.x, (ShawInt)scale.y, (ShawInt)scale.z);
            cfg.mType = ColliderType.Box;
            cfg.mAxis = new ShawVector3[3];
            cfg.mAxis[0] = new ShawVector3((ShawInt)right.x, (ShawInt)right.y, (ShawInt)right.z);
            cfg.mAxis[1] = new ShawVector3((ShawInt)up.x, (ShawInt)up.y, (ShawInt)up.z);
            cfg.mAxis[2] = new ShawVector3((ShawInt)forward.x, (ShawInt)forward.y, (ShawInt)forward.z);

            envColliCfgLst.Add(cfg);
        }

        CapsuleCollider[] cylindderArr = transEnvRoot.GetComponentsInChildren<CapsuleCollider>();
        for (int i = 0; i < cylindderArr.Length; i++)
        {
            Transform trans = cylindderArr[i].transform;
            UnityEngine.Vector3 pos = trans.position;
            ColliderConfig cfg = new ColliderConfig
            {
                mPos = new ShawVector3((ShawInt)pos.x, (ShawInt)pos.y, (ShawInt)pos.z)
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

        return logicEnv.GetAllEnvColliders();
    }
    #endregion
}