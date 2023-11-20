using GameProtocol;
using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public static FightManager Instance;

    void Awake()
    {
        Instance = this;
    }

    List<ShawColliderBase> colliderLst; // �洢������������ײ��

    /// <summary>
    /// ��ʼ��������ײ����
    /// </summary>
    /// <returns>�洢���г�����ײ�������</returns>
    public void InitCollisionEnv()
    {
        // ������ײ����
        Transform transEnvRoot = GameObject.Find("MapRoot/EnvCollider").transform;
        if (transEnvRoot == null)
        {
            LogCore.Error($"δ�ڳ������ҵ� {0}", "MapRoot/EnvCollider");
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


    public void InitHero(List<BattleHeroData> battleHeroDatas, MapConfig mapCfg)
    {

    }
    /// <summary>
    /// �ⲿ��ȡ���л�����ײ
    /// </summary>
    /// <returns></returns>
    public List<ShawColliderBase> GetAllEnvColliders()
    {
        return colliderLst;
    }
}
