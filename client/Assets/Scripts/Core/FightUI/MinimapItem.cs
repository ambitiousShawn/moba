using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using Tutorial;
using UnityEngine;
using UnityEngine.UI;

public class MinimapItem : WindowRoot
{
    public float scaler = 7; // 尺寸为7最合适
    public RectTransform rect;
    public Image imgIcon;
    public Image imgFrame;

    MainLogicUnit unit;     // 对应逻辑实体
    Vector3 refPos;         // 相对位置

    public void InitItem(MainLogicUnit unit, Vector3 refPos)
    {
        this.unit = unit;
        this.refPos = refPos;

        rect.localEulerAngles = new Vector3(0, 0, -45);
        switch (unit.unitType)
        {
            case EUnitType.Hero:
                Sprite sprite = AssetsSvc.Instance.LoadSprite("minimap", $"{unit.unitData.unitCfg.resName}_mapIcon", 1);
                imgIcon.sprite = sprite;
                if (unit.IsTeam(ETeamType.Blue))
                {
                    sprite = AssetsSvc.Instance.LoadSprite("minimap", "blueHeroMapFrame", 1);
                    imgFrame.sprite = sprite;
                }
                else
                {
                    sprite = AssetsSvc.Instance.LoadSprite("minimap", "redHeroMapFrame", 1);
                    imgFrame.sprite = sprite;
                }
                imgFrame.SetNativeSize();
                break;
            default:
                LogCore.Error("Unknow unitType.");
                break;
        }
    }

    private void Update()
    {
        Vector3 offset = unit.LogicPos.ConvertViewVector3() - refPos;
        rect.localPosition = new Vector3(offset.x, offset.z, 0) * scaler;
    }

    private void OnDisable()
    {
        unit = null;
        refPos = Vector3.zero;
    }
}