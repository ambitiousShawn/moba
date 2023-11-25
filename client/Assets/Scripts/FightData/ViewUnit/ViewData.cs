using UnityEngine;

/// <summary>
/// 基本视图单元
/// </summary>
public abstract class BaseViewUnit : MonoBehaviour
{
    public Transform RotationRoot; // 旋转根节点

    BaseLogicUnit logicUnit = null;
    public virtual void Init(BaseLogicUnit logicUnit)
    {
        this.logicUnit = logicUnit;
        gameObject.name = logicUnit.unitName + "_" + gameObject.name;
        transform.position = logicUnit.LogicPos.ConvertViewVector3();
        if (RotationRoot == null)
        {
            RotationRoot = transform;
        }
        RotationRoot.rotation = Quaternion.FromToRotation(Vector3.forward, logicUnit.LogicDir.ConvertViewVector3()); // 旋转矫正
    }
}

/// <summary>
/// 主视图单元
/// </summary>
public abstract class MainViewUnit : BaseViewUnit
{
    float animMoveSpeedBase;
    MainLogicUnit mainLogicUnit = null;
    public override void Init(BaseLogicUnit logicUnit)
    {
        base.Init(logicUnit);
        mainLogicUnit = logicUnit as MainLogicUnit;
        // 基础移速
        animMoveSpeedBase = mainLogicUnit.LogicMoveSpeed.RawFloat;
    }
}