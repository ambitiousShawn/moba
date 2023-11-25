using UnityEngine;

/// <summary>
/// ������ͼ��Ԫ
/// </summary>
public abstract class BaseViewUnit : MonoBehaviour
{
    public Transform RotationRoot; // ��ת���ڵ�

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
        RotationRoot.rotation = Quaternion.FromToRotation(Vector3.forward, logicUnit.LogicDir.ConvertViewVector3()); // ��ת����
    }
}

/// <summary>
/// ����ͼ��Ԫ
/// </summary>
public abstract class MainViewUnit : BaseViewUnit
{
    float animMoveSpeedBase;
    MainLogicUnit mainLogicUnit = null;
    public override void Init(BaseLogicUnit logicUnit)
    {
        base.Init(logicUnit);
        mainLogicUnit = logicUnit as MainLogicUnit;
        // ��������
        animMoveSpeedBase = mainLogicUnit.LogicMoveSpeed.RawFloat;
    }
}