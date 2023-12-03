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

    public UGUI_PlayPanel playWnd; // Lua中赋值

    public float SkillDisMultipler = 0.03f; // 技能距离的乘法系数

    int waveIndex = 0; // 小兵波数

    
    public Transform transEnvRoot; // 碰撞体根节点

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
    /// 给 BattleSystem 调用，初始化 碰撞环境，英雄，塔，小兵，UI等
    /// </summary>
    public void InitAll(List<BattleHeroData> battleHeroDatas, MapConfig config)
    {
        heroList = new List<HeroLogic>();
        towerList = new List<TowerLogic>();
        soldierList = new List<SoldierLogic>();
        timerList = new List<LogicTimer>();
        bulletList = new List<BulletLogic>();

        // 碰撞环境
        InitCollisionEnv();
        // 防御塔
        InitTower(config);
        // 英雄
        InitHero(battleHeroDatas, config);
        // 小兵
        waveIndex++;
        void CreateSoldierWave()
        {
            CreateSoldierBatch(config, ETeamType.Blue);
            CreateSoldierBatch(config, ETeamType.Red);
        }
        LogicTimer timer = new LogicTimer(CreateSoldierWave, config.soldierBornDelay, config.soldierWaveInterval);
        timerList.Add(timer); // 创建周期性生成小兵的任务，并加入逻辑帧更新
    }


    #region 摄相机
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
            transCameraRoot.position = cameraFollowTarget.position;
        }
    }
    #endregion

    #region 初始化游戏实体
    
    // 初始化场景碰撞环境
    void InitCollisionEnv()
    {
        // 生成碰撞配置
        transEnvRoot = GameObject.Find("MapRoot/EnvCollider").transform;
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

    // 初始化防御塔
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

    // 初始化英雄单位
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

    // 创建一波小兵
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

    #region 游戏运行过程中
    public void Tick()
    {
        // 子弹
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

        // 英雄
        for (int i = 0; i < heroList.Count; ++i)
        {
            heroList[i].LogicTick();
        }

        // 防御塔
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

        // 小兵
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

        // 全局计时器
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

    // 某个技能进入CD
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

    /// <summary>
    /// 加入子弹到容器，参与帧更新等操作
    /// </summary>
    /// <param name="bullet"></param>
    public void AddBullet(BulletLogic bullet)
    {
        bulletList.Add(bullet);
    }
    #endregion
}
