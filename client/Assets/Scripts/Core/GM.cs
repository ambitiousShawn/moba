using GameProtocol;
using ShawnFramework.CommonModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GM 指令测试系统
/// </summary>
public class GMSys : MonoBehaviour
{

    public static GMSys Instance;
    public bool EnableGM = false; // 测试模块是否开启

    private uint frameID = 0;
    private void OnGUI()
    {
        if (GUILayout.Button("快速战斗GM"))
        {
            StartSimulate();
        }
    }

    void Start()
    {
        Instance = this;
    }

    public void StartSimulate()
    {        
        EnableGM = true;
        StartCoroutine(BattleSimulate());
    }

    public IEnumerator BattleSimulate()
    {
        SimulateLoadRes();
        yield return new WaitForSeconds(2f);
        SimulateBattleStart();
    }

    void SimulateLoadRes()
    {
        GameMsg msg = new GameMsg
        {
            cmd = CMD.NtfLoadRes,
            ntfLoadRes = new NtfLoadRes
            {
                mapID = 101,
                battleHeroDatas = new List<BattleHeroData>
                {
                    new BattleHeroData
                    {
                        heroID = 101,
                        userName = "Bot1"
                    },
                    new BattleHeroData
                    {
                        heroID = 102,
                        userName = "Bot2",
                    }
                },
                posIndex = 0,
            },
        };
        Action<GameMsg> ntf_loadres = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("NtfLoadResCallBack"); // 调用lua的全局响应函数
        ntf_loadres?.Invoke(msg);
    }

    void SimulateBattleStart()
    {
        GameMsg msg = new GameMsg
        {
            cmd = CMD.RspBattleStart,
        };
        Action<GameMsg> rsp_battlestart = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("RspBattleStartCallBack"); // 调用lua的全局响应函数
        rsp_battlestart?.Invoke(msg);
    }

    /// <summary>
    /// 模拟服务器接收消息
    /// </summary>
    /// <param name="msg"></param>
    public void SimulateServerRevMsg(GameMsg msg)
    {
        switch (msg.cmd)
        {
            case CMD.SndOpKey:
                // 仅处理一帧的最后一次操作，前面的会被覆盖
                UpdateOpKey(msg.sndOpKey.opKey);
                break;
            default:
                break;
        }
    }

    private List<OpKey> opKeyList = new List<OpKey>();
    private void UpdateOpKey(OpKey key)
    {
        opKeyList.Add(key);
    }

    /// <summary>
    /// 设为每秒15帧的标准逻辑帧
    /// </summary>
    private void FixedUpdate()
    {
        ++frameID;
        GameMsg msg = new GameMsg
        {
            cmd = CMD.NtfOpKey,
            ntfOpKey = new NtfOpKey
            {
                frameID = frameID,
                keyList = new List<OpKey>(),
            }
        };
        
        int count = opKeyList.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                OpKey key = opKeyList[i];
                msg.ntfOpKey.keyList.Add(key);
            }
        }
        opKeyList.Clear();
        NetSvc.Instance.AddToMsgQueue(msg);
    }
}
