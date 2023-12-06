using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class HPPanel : WindowRoot
{
    public Transform HPItemRoot;
    public Transform JumpInfoRoot;
    public int JumpCnt;

    private Dictionary<MainLogicUnit, SceneHPItem> itemDic;

    private void OnEnable()
    {
        itemDic = new Dictionary<MainLogicUnit, SceneHPItem>();

        // ע��UI�����¼�
        SceneHPItem.OnHPChangedViewEvent += OnHPChangedViewEventFunc;
    }

    private void OnDisable()
    {
        SceneHPItem.OnHPChangedViewEvent -= OnHPChangedViewEventFunc;

        for (int i = HPItemRoot.childCount - 1; i >= 0; --i)
        {
            Destroy(HPItemRoot.GetChild(i).gameObject);
        }
        for (int i = HPItemRoot.childCount - 1; i >= 0; --i)
        {
            Destroy(HPItemRoot.GetChild(i).gameObject);
        }

        if (itemDic != null)
        {
            itemDic.Clear();
        }
    }

    private void Update()
    {
        foreach (var item in itemDic)
        {
            item.Value.RefreshBarPos();
        }
    }

    /// <summary>
    /// ĳ�߼�ʵ��Ѫ�������䶯ʱ����
    /// </summary>
    /// <param name="item"></param>
    /// <param name="curVal"></param>
    /// <param name="sumVal"></param>
    void OnHPChangedViewEventFunc(MainLogicUnit unit, int curVal)
    {
        SceneHPItem item = null;
        if (itemDic.TryGetValue(unit, out item))
        {
            item.gameObject.SetActive(curVal != 0);
            item.ImgPrg.fillAmount = curVal * 1.0f / item.OriginHP;
        }
    }

    public void AddHPItemInfo(MainLogicUnit unit, Transform trans, int hp)
    {
        if (itemDic.ContainsKey(unit))
        {
            LogCore.Error(unit.unitName + " hp item is already exist.");
        }
        else
        {
            //�жϵ�λ���ͣ�ʵ������ӦԤ����
            string path = GetItemPath(unit.unitType);
            GameObject go = AssetsSvc.Instance.LoadPrefab("", path, 0);
            go.transform.SetParent(HPItemRoot);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            SceneHPItem hpItem = go.GetComponent<SceneHPItem>();
            hpItem.InitItem(unit, trans, hp);

            itemDic.Add(unit, hpItem);
        }
    }

    string GetItemPath(EUnitType unitType)
    {
        string path = "";
        switch (unitType)
        {
            case EUnitType.Hero:
                path = "UIPrefab/DynamicItem/ItemHPHero";
                break;
            case EUnitType.Soldier:
                path = "UIPrefab/DynamicItem/ItemHPSoldier";
                break;
            case EUnitType.Tower:
                path = "UIPrefab/DynamicItem/ItemHPTower";
                break;
            default:
                break;
        }
        return path;
    }

    public void RemoveHPItem(MainLogicUnit unit)
    {
        if (itemDic.TryGetValue(unit, out SceneHPItem item))
        {
            Destroy(item.gameObject);
            itemDic.Remove(unit);
        }
    }
}
