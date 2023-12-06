using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 场景逻辑单元血条的基类
/// </summary>
public abstract class SceneHPItem : MonoBehaviour
{
    public RectTransform rect;
    public Image ImgPrg;

    protected bool IsFriend;
    Transform root;
    public int OriginHP;

    // 血量变动时触发的UI更新事件(初始化时赋值)
    public static Action<MainLogicUnit, int> OnHPChangedViewEvent;

    public virtual void InitItem(MainLogicUnit unit, Transform root, int hp)
    {
        ETeamType selfTeam = FightManager.Instance.GetCurrentUserTeam();
        IsFriend = unit.IsTeam(selfTeam);

        // 初始化血条
        ImgPrg.fillAmount = 1;
        this.root = root;
        OriginHP = hp;
        OnHPChangedViewEvent?.Invoke(unit, hp);
    }

    public virtual void SetStateIcon(EAbnormalState state, bool show) { }

    // 由HPPanel带动帧更新
    public void RefreshBarPos()
    {
        float scaleRate = 1.0f * ClientConfig.ScreenStandardHeight / Screen.height;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(root.position);
        rect.anchoredPosition = screenPos * scaleRate;
    }
}