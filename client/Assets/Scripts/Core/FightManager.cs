using GameProtocol;
using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Tutorial;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class FightManager : MonoBehaviour
{
    public static FightManager Instance;

    public UGUI_PlayPanel playWnd; // Lua�и�ֵ

    public float SkillDisMultipler = 0.03f; // ���ܾ���ĳ˷�ϵ��

    int waveIndex = 0; // С������

    
    public Transform transEnvRoot; // ��ײ����ڵ�

    void Awake()
    {
        Instance = this;
    }

    List<ShawColliderBase> colliderLst;
    List<HeroLogic> heroList;
    List<TowerLogic> towerList;
    List<SoldierLogic> soldierList;
    List<LogicTimer> timerList;
    List<BulletLogic> bulletList;
    /// <summary>
    /// �� BattleSystem ���ã���ʼ�� ��ײ������Ӣ�ۣ�����С����UI��
    /// </summary>
    public void InitAll(List<BattleHeroData> battleHeroDatas, MapConfig config)
    {
        heroList = new List<HeroLogic>();
        towerList = new List<TowerLogic>();
        soldierList = new List<SoldierLogic>();
        timerList = new List<LogicTimer>();
        bulletList = new List<BulletLogic>();

        // ��ײ����
        InitCollisionEnv();
        // ������
        InitTower(config);
        // Ӣ��
        InitHero(battleHeroDatas, config);
        // С��
        waveIndex++;
        void CreateSoldierWave()
        {
            CreateSoldierBatch(config, ETeamType.Blue);
            CreateSoldierBatch(config, ETeamType.Red);
        }
        LogicTimer timer = new LogicTimer(CreateSoldierWave, config.soldierBornDelay, config.soldierWaveInterval);
        timerList.Add(timer); // ��������������С�������񣬲������߼�֡����
    }


    #region �����
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
            transCameraRoot.position = cameraFollowTarget.position;
        }
    }
    #endregion

    #region ��ʼ����Ϸʵ��
    
    // ��ʼ��������ײ����
    void InitCollisionEnv()
    {
        // ������ײ����
        transEnvRoot = GameObject.Find("MapRoot/EnvCollider").transform;
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

    // ��ʼ��������
    void InitTower(MapConfig mapCfg)
    {
        int sep = mapCfg.towerIDArr.Length / 2;
        TowerLogic[] blueTeamTower = new TowerLogic[sep];
        TowerLogic[] redTeamTower = new TowerLogic[sep];
        for (int i = 0; i < mapCfg.towerIDArr.Length; i++)
        {
            TowerData td = new TowerData
            {
                towerID = mapCfg.towerIDArr[i],
                towerIndex = i,
                unitCfg = AssetsSvc.Instance.GetUnitConfigByID(mapCfg.towerIDArr[i])
            };

            TowerLogic tower;
            if (i < sep)
            {
                td.teamType = ETeamType.Blue;
                td.bornPos = mapCfg.towerPosArr[i];
                tower = new TowerLogic(td);
                blueTeamTower[i] = tower;
            }
            else
            {
                td.teamType = ETeamType.Red;
                td.bornPos = mapCfg.towerPosArr[i];
                tower = new TowerLogic(td);
                redTeamTower[i - sep] = tower;
            }
            tower.LogicInit();
            towerList.Add(tower);
        }

        CalcSkillSelectTarget.blueTeamTower = blueTeamTower;
        CalcSkillSelectTarget.redTeamTower = redTeamTower;
    }

    // ��ʼ��Ӣ�۵�λ
    void InitHero(List<BattleHeroData> battleHeroDatas, MapConfig mapCfg)
    {
        int cnt = battleHeroDatas.Count / 2;
        HeroLogic[] blueTeamHero = new HeroLogic[cnt];
        HeroLogic[] redTeamHero = new HeroLogic[cnt];
        for (int i = 0; i < cnt; i++)
        {
            HeroData hd = new HeroData
            {
                heroID = battleHeroDatas[i].heroID,
                posIndex = i,
                userName = battleHeroDatas[i].userName,
                unitCfg = AssetsSvc.Instance.GetUnitConfigByID(battleHeroDatas[i].heroID),
            };
            HeroLogic hero;
            if (i < cnt)
            {
                hd.teamType = ETeamType.Blue;
                hd.bornPos = mapCfg.blueBornPos;
                hero = new HeroLogic(hd);
                blueTeamHero[i] = hero;
            }
            else
            {
                hd.teamType = ETeamType.Red;
                hd.bornPos = mapCfg.redBornPos;
                hero = new HeroLogic(hd);
                redTeamHero[i - cnt] = hero;
            }
            hero.LogicInit();
            heroList.Add(hero);
        }
        CalcSkillSelectTarget.blueTeamHero = blueTeamHero;
        CalcSkillSelectTarget.redTeamHero = redTeamHero;
    }

    // ����һ��С��
    void CreateSoldierBatch(MapConfig config, ETeamType team)
    {
        int[] idArr;
        ShawVector3[] posArr;
        if (team == ETeamType.Blue)
        {
            idArr = config.blueSoldierIDArr;
            posArr = config.blueSoldierPosArr;
        }
        else
        {
            idArr = config.redSoldierIDArr;
            posArr = config.redSoldierPosArr;
        }

        for (int i = 0; i < idArr.Length; i++)
        {
            SoldierData sd = new SoldierData
            {
                soldierID = idArr[i],
                waveIndex = waveIndex,
                orderIndex = i,
                soldierName = "soldier_" + idArr[i],
                teamType = team,
                bornPos = posArr[i],
                unitCfg = AssetsSvc.Instance.GetUnitConfigByID(idArr[i]),
            };

            LogicTimer timer = new LogicTimer(() => {
                SoldierLogic soldier = new SoldierLogic(sd);
                soldier.LogicInit();
                if (sd.teamType == ETeamType.Blue)
                {
                    CalcSkillSelectTarget.blueTeamSoldier.Add(soldier);
                }
                else
                {
                    CalcSkillSelectTarget.redTeamSoldier.Add(soldier);
                }
                soldierList.Add(soldier);
            }, (i / 2) * config.soldierBornInterval);
            timerList.Add(timer);
        }
    }

    #endregion

    #region ��Ϸ���й�����
    public void Tick()
    {
        // �ӵ�
        for (int i = bulletList.Count - 1; i >= 0; --i)
        {
            if (bulletList[i].state == SubUnitState.None)
            {
                bulletList[i].LogicUninit();
                bulletList.RemoveAt(i);
            }
            else
            {
                bulletList[i].LogicTick();
            }
        }

        // Ӣ��
        for (int i = 0; i < heroList.Count; ++i)
        {
            heroList[i].LogicTick();
        }

        // ������
        for (int i = towerList.Count - 1; i >= 0; --i)
        {
            TowerLogic tower = towerList[i];
            if (tower.stateType != EUnitStateType.Dead)
            {
                tower.LogicTick();
            }
            // else
            // {
            //     towerList[i].LogicUnInit();
            //     towerList.RemoveAt(i);
            // }
        }

        // С��
        for (int i = soldierList.Count - 1; i >= 0; --i)
        {
            SoldierLogic soldier = soldierList[i];
            if (soldier.stateType != EUnitStateType.Dead)
            {
                soldier.LogicTick();
            }
            else
            {
                if (soldier.IsTeam(ETeamType.Blue))
                {
                    int index = CalcSkillSelectTarget.blueTeamSoldier.IndexOf(soldier);
                    CalcSkillSelectTarget.blueTeamSoldier.RemoveAt(index);
                }
                else
                {
                    int index = CalcSkillSelectTarget.redTeamSoldier.IndexOf(soldier);
                    CalcSkillSelectTarget.redTeamSoldier.RemoveAt(index);
                }
                soldier.LogicUninit();
                soldierList.RemoveAt(i);
            }
        }

        // ȫ�ּ�ʱ��
        for (int i = timerList.Count - 1; i >= 0; --i)
        {
            LogicTimer timer = timerList[i];
            if (timer.Enable)
            {
                timer.TickTimer();
            }
            else
            {
                timerList.RemoveAt(i);
            }
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
    public void SendSkillOperation(int skillID, UnityEngine.Vector3 vec)
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

    // ĳ�����ܽ���CD
    public void EnterCDState(int skilldID, int cdTime)
    {
        playWnd.EnterCDState(skilldID, cdTime);
    }
    #endregion

    public void UnInit()
    {
        heroList.Clear();
        towerList.Clear();
        soldierList.Clear();
        bulletList.Clear();
        CalcSkillSelectTarget.blueTeamSoldier.Clear();
        CalcSkillSelectTarget.redTeamSoldier.Clear();
    }

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

    /// <summary>
    /// �����ӵ�������������֡���µȲ���
    /// </summary>
    /// <param name="bullet"></param>
    public void AddBullet(BulletLogic bullet)
    {
        bulletList.Add(bullet);
    }
    #endregion
}
