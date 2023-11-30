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

    public UGUI_PlayPanel playWnd; // Lua�и�ֵ

    public float SkillDisMultipler = 0.03f; // ���ܾ���ĳ˷�ϵ��

    void Awake()
    {
        Instance = this;
    }

    #region ��ʼ�����(�����������)
    
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
            LogCore.Error($"δ�ڳ������ҵ�\"MapRoot/EnvCollider\" ");
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
    /// ��ʼ��Ӣ�۵�λ
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
    Transform cameraFollowTarget = null; // �������Ŀ��
    /// <summary>
    /// ��ʼ�����
    /// </summary>
    public void InitCamera(int posIndex)
    {
        cameraFollowTarget = heroList[posIndex].mainViewUnit.transform;
    }

    private void LateUpdate()
    {
        // �������
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

    #region ��Ϸ���й�����
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
    // �ƶ���������
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

    // ���ܲ�������
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
    /// ��ȡ��ǰ�ͻ��˵��߼�ʵ��
    /// </summary>
    /// <param name="posIndex"></param>
    /// <returns></returns>
    public MainLogicUnit GetSelfHero(int posIndex)
    {
        return heroList[posIndex];
    }

    /// <summary>
    /// �ⲿ��ȡ���л�����ײ
    /// </summary>
    /// <returns></returns>
    public List<ShawColliderBase> GetAllEnvColliders()
    {
        return colliderLst;
    }
    #endregion
}
