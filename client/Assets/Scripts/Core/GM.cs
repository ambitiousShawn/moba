using GameProtocol;
using ShawnFramework.CommonModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GM ָ�����ϵͳ
/// </summary>
public class GMSys : MonoBehaviour
{

    public static GMSys Instance;
    public bool EnableGM = false; // ����ģ���Ƿ���

    private uint frameID = 0;
    private void OnGUI()
    {
        if (GUILayout.Button("����ս��GM"))
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
        Action<GameMsg> ntf_loadres = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("NtfLoadResCallBack"); // ����lua��ȫ����Ӧ����
        ntf_loadres?.Invoke(msg);
    }

    void SimulateBattleStart()
    {
        GameMsg msg = new GameMsg
        {
            cmd = CMD.RspBattleStart,
        };
        Action<GameMsg> rsp_battlestart = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<GameMsg>>("RspBattleStartCallBack"); // ����lua��ȫ����Ӧ����
        rsp_battlestart?.Invoke(msg);
    }

    /// <summary>
    /// ģ�������������Ϣ
    /// </summary>
    /// <param name="msg"></param>
    public void SimulateServerRevMsg(GameMsg msg)
    {
        switch (msg.cmd)
        {
            case CMD.SndOpKey:
                // ������һ֡�����һ�β�����ǰ��Ļᱻ����
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
    /// ��Ϊÿ��15֡�ı�׼�߼�֡
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
