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

        // 注册UI更新事件
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
    /// 某逻辑实体血条发生变动时触发
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
            //判断单位类型，实例化对应预制体
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
