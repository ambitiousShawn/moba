using GameProtocol;
using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using ShawnFramework.ShawMath;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UGUI_PlayPanel : WindowRoot
{
    private void OnEnable()
    {
        InitMove();
        InitMiniMap();

        RegisterMoveEvts();
    }

    private void Update()
    {
        MoveTick();
    }

    #region 移动模块
    public Image imgCancelSkill;
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;
    public Transform ArrowRoot;

    float pointDis = 135;
    Vector2 startPos = Vector2.zero;
    Vector2 defaultPos = Vector2.zero;

    private void InitMove()
    {
        pointDis = Screen.height * 1.0f / ClientConfig.ScreenStandardHeight * ClientConfig.ScreenOPDis;
        ArrowRoot.gameObject.SetActive(false);
        defaultPos = imgDirBg.transform.position;
    }

    void RegisterMoveEvts()
    {
        ArrowRoot.gameObject.SetActive(false);

        OnClickDown(imgTouch.gameObject, (PointerEventData evt, object[] args) => {
            startPos = evt.position;
            Debug.Log($"evt.pos:{evt.position}");
            imgDirPoint.color = new Color(1, 1, 1, 1f);
            imgDirBg.transform.position = evt.position;
        });
        OnClickUp(imgTouch.gameObject, (PointerEventData evt, object[] args) => {
            imgDirBg.transform.position = defaultPos;
            imgDirPoint.color = new Color(1, 1, 1, 0.5f);
            imgDirPoint.transform.localPosition = Vector2.zero;
            ArrowRoot.gameObject.SetActive(false);

            InputMoveKey(Vector2.zero);
        });
        OnDrag(imgTouch.gameObject, (PointerEventData evt, object[] args) => {
            Vector2 dir = evt.position - startPos;
            float len = dir.magnitude;
            if (len > pointDis)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);
                imgDirPoint.transform.position = startPos + clampDir;
            }
            else
            {
                imgDirPoint.transform.position = evt.position;
            }

            if (dir != Vector2.zero)
            {
                ArrowRoot.gameObject.SetActive(true);
                float angle = Vector2.SignedAngle(new Vector2(1, 0), dir);
                ArrowRoot.localEulerAngles = new Vector3(0, 0, angle);
            }

            InputMoveKey(dir.normalized);
        });
    }

    private Vector2 lastKeyDir = Vector2.zero;
    private void MoveTick()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 keyDir = new Vector2(h, v);

        if (keyDir != lastKeyDir)
        {
            if (h != 0 ||  v != 0)
            {
                keyDir = keyDir.normalized;
            }
            InputMoveKey(keyDir);
            lastKeyDir = keyDir;
        }
    }

    private Vector2 lastStickDir = Vector2.zero;
    private void InputMoveKey(Vector2 dir)
    {
        if (!dir.Equals(lastStickDir))
        {
            Vector3 dirVector3 = new Vector3(dir.x, 0, dir.y);
            dirVector3 = Quaternion.Euler(0, 45, 0) * dirVector3;
            ShawVector3 logicDir = ShawVector3.zero;
            if (dir != Vector2.zero)
            {
                isUIInput = true;
                logicDir.x = (ShawInt)dirVector3.x;
                logicDir.y = (ShawInt)dirVector3.y;
                logicDir.z = (ShawInt)dirVector3.z;
            }
            else
            {
                isUIInput = false;
            }

            // Action<ShawVector3> snd_opkey = LuaManager.Instance.GlobalLuaEnv.Global.Get<Action<ShawVector3>>("SendMoveOperation"); // 调用lua的全局响应函数
            // snd_opkey?.Invoke(logicDir);
            FightManager.Instance.SendMoveOperation(logicDir);

            lastStickDir = dir;
        }
       
    }
    #endregion

    #region 技能模块
    public SkillItem skillItemA;
    public SkillItem skillItem1;
    public SkillItem skillItem2;
    public SkillItem skillItem3;

    public Transform ImgInfoRoot;
    public void InitSkillInfo()
    {
        BattleHeroData self = Launcher.Instance.BattleHeroDatas[Launcher.Instance.SelfIndex];
        UnitConfig heroCfg = AssetsSvc.Instance.GetUnitConfigByID(self.heroID);
        int[] skillArr = heroCfg.skillArr; // 拿到技能配置

        skillItemA.InitSkillItem(AssetsSvc.Instance.GetSkillConfigByID(skillArr[0]), 0);
        skillItem1.InitSkillItem(AssetsSvc.Instance.GetSkillConfigByID(skillArr[1]), 1);
        skillItem2.InitSkillItem(AssetsSvc.Instance.GetSkillConfigByID(skillArr[2]), 2);
        skillItem3.InitSkillItem(AssetsSvc.Instance.GetSkillConfigByID(skillArr[3]), 3);

        SetForbidState(false);
        ImgInfoRoot.gameObject.SetActive(true);
    }

    /// <summary>
    /// 被控制时无法释放技能
    /// </summary>
    /// <param name="state"></param>
    void SetForbidState(bool state)
    {
        skillItem1.SetForbidState(state);
        skillItem2.SetForbidState(state);
        skillItem3.SetForbidState(state);
    }

    // 某个技能进入CD
    public void EnterCDState(int skillID, int cdTime)
    {
        if (skillItemA.CheckSkillID(skillID))
        {
            skillItemA.EnterCDState(cdTime);
        }
        else if (skillItem1.CheckSkillID(skillID))
        {
            skillItem1.EnterCDState(cdTime);
        }
        else if (skillItem2.CheckSkillID(skillID))
        {
            skillItem2.EnterCDState(cdTime);
        }
        else if (skillItem3.CheckSkillID(skillID))
        {
            skillItem3.EnterCDState(cdTime);
        }
        else
        {
            LogCore.Error($"skill id{skillID} enter cd error.");
        }
    }
    #endregion

    #region 小地图模块
    public Transform mapIconRoot;
    public Transform mapHeroIconRoot;

    private Dictionary<MainLogicUnit, MinimapItem> unitMapitemDic; // 场景对象与地图图标的映射
    private Vector3 refPosition = Vector3.zero; // 相对位置

    void InitMiniMap()
    {
        unitMapitemDic = new Dictionary<MainLogicUnit, MinimapItem>();
    }
    void UnInitMiniMap()
    {
        for (int i = mapHeroIconRoot.childCount - 1; i >= 0; --i)
        {
            Destroy(mapHeroIconRoot.GetChild(i).gameObject);
        }
        for (int i = mapIconRoot.childCount - 1; i >= 0; --i)
        {
            if (mapIconRoot.GetChild(i).name != "bgRoad")
            {
                Destroy(mapIconRoot.GetChild(i).gameObject);
            }
        }
        if (unitMapitemDic != null)
        {
            unitMapitemDic.Clear();
        }
    }

    public void AddMiniIconItemInfo(MainLogicUnit unit)
    {
        if (unitMapitemDic.ContainsKey(unit))
        {
            LogCore.Error(unit.unitName + "minimap item is already exist.");
            return;
        }
        else
        {
            if (gameObject.activeSelf == false)
            {
                return;
            }
            GameObject go = null;
            if (unit.unitType == EUnitType.Hero)
            {
                go = AssetsSvc.Instance.LoadPrefab("", "UIPrefab/DynamicItem/ItemMapIconHero", 0);
                go.transform.SetParent(mapHeroIconRoot);
            }
            else
            {
                go = AssetsSvc.Instance.LoadPrefab("", "UIPrefab/DynamicItem/ItemMapIcon", 0);
                go.transform.SetParent(mapIconRoot);
            }
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            MinimapItem item = go.GetComponent<MinimapItem>();
            item.InitItem(unit, refPosition);
            unitMapitemDic.Add(unit, item);
        }
    }

    public void RmvMapIconItemInfo(MainLogicUnit key)
    {
        if (unitMapitemDic.TryGetValue(key, out MinimapItem item))
        {
            Destroy(item.gameObject);
            unitMapitemDic.Remove(key);
        }
    }
    #endregion

    bool isUIInput = false;
    public bool IsUIInput()
    {
        return isUIInput;
    }
}
