/*using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using ShawnFramework.ShawUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPWnd : WindowRoot
{
    public Transform HPItemRoot;
    public Transform JumpInfoRoot;
    public int JumpCnt;
    public SceneJumpItem HPItemPrefab;

    private Dictionary<MainLogicUnit, SceneHPItem> itemDic;
    JumpNumPool pool;

    private void OnEnable()
    {
        itemDic = new Dictionary<MainLogicUnit, SceneHPItem>();
        pool = new JumpNumPool(JumpCnt, JumpInfoRoot);
    }

    public void AddHPItemInfo(MainLogicUnit unit, Transform trans, int hp)
    {
        if (itemDic.ContainsKey(unit))
        {
            LogCore.Error(unit.unitName + " hp item is already exist.");
        }
        else
        {
            //判断单位类型
            string path = GetItemPath(unit.unitType);
            GameObject go = AssetsSvc.Instance.LoadPrefab("", path, 0);
            go.transform.SetParent(HPItemRoot);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            SceneHPItem ih = go.GetComponent<SceneHPItem>();
            ih.InitItem(unit, trans, hp);
            itemDic.Add(unit, ih);
        }
    }
    private void Update()
    {
        foreach (var item in itemDic)
        {
            item.Value.Check();
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
}
*/