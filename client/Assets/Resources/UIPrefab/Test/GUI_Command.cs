using GameProtocol;
using ShawnFramework.CommonModule;
using ShawnFramework.ShawMath;
using ShawnFramework.ShawnPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

public class GUI_Command : MonoBehaviour
{
    public Button btn_quick_select;
    public Button btn_quick_sndSelect;
    private LuaEnv luaEnv;

    void Start()
    {
        luaEnv = LuaManager.Instance.GlobalLuaEnv;
        btn_quick_select.onClick.AddListener(() =>
        {
            UserData userData = new UserData();
            userData.id = (uint)Random.Range(100000, 999999);
            userData.name = "Test_" + userData.id;
            userData.heroDatas = new List<HeroData>
            {
                new HeroData(){ heroId = 101 },
                new HeroData(){ heroId = 102 },
            };
            Launcher.Instance.UserData = userData;

            Launcher.Instance.RoomID = (uint)Random.Range(10000, 99999);
            luaEnv.DoString("WindowManager:close_all() \n WindowManager:open('ugui_selectpanel')");
        });

        btn_quick_sndSelect.onClick.AddListener(() =>
        {
            GameMsg msg = new GameMsg()
            {
                cmd = CMD.SndSelect,
                sndSelect = new SndSelect
                {
                    roomID = 999,
                    heroID = 101,
                }
            };
            NetSvc.Instance.SendMsg(msg);
        });
    }
}
