using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �����߼���ԪѪ���Ļ���
/// </summary>
public abstract class SceneHPItem : MonoBehaviour
{
    public RectTransform rect;
    public Image ImgPrg;

    protected bool IsFriend;
    Transform root;
    public int OriginHP;

    // Ѫ���䶯ʱ������UI�����¼�(��ʼ��ʱ��ֵ)
    public static Action<MainLogicUnit, int> OnHPChangedViewEvent;

    public virtual void InitItem(MainLogicUnit unit, Transform root, int hp)
    {
        ETeamType selfTeam = FightManager.Instance.GetCurrentUserTeam();
        IsFriend = unit.IsTeam(selfTeam);

        // ��ʼ��Ѫ��
        ImgPrg.fillAmount = 1;
        this.root = root;
        OriginHP = hp;
        OnHPChangedViewEvent?.Invoke(unit, hp);
    }

    public virtual void SetStateIcon(EAbnormalState state, bool show) { }

    // ��HPPanel����֡����
    public void RefreshBarPos()
    {
        float scaleRate = 1.0f * ClientConfig.ScreenStandardHeight / Screen.height;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(root.position);
        rect.anchoredPosition = screenPos * scaleRate;
    }
}