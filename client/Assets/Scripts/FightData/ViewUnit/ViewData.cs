using ShawnFramework.ShawMath;
using UnityEngine;

/// <summary>
/// ������ͼ��Ԫ
/// </summary>
public abstract class BaseViewUnit : MonoBehaviour
{
    // һЩ��������
    [Header("ƽ��ת��")]
    public bool EnableSmoothRotate = true;             // ����ƽ��ת��
    public float RotateSmoothValue = 10;               // ��תƽ������
    public float AngleMultiplier = 8;                  // �Ƕȱ仯����

    [Header("ƽ���ƶ�")]
    public bool EnablePredictPos = true;               // ����Ԥ��λ���ƶ��㷨
    public bool EnableSmoothMove = true;               // ����ƽ���ƶ�
    public int MaxPredictCnt = 15;                     // ���Ԥ��λ�ô���     
    public float MoveSmoothValue = 10;                 // �ƶ�ƽ������

    Transform RotationRoot; // ��ת���ڵ�
    BaseLogicUnit logicUnit = null; // �߼�ʵ��
    public virtual void Init(BaseLogicUnit logicUnit)
    {
        this.logicUnit = logicUnit;
        gameObject.name = logicUnit.unitName + "_" + gameObject.name;
        transform.position = logicUnit.LogicPos.ConvertViewVector3();
        if (RotationRoot == null)
        {
            RotationRoot = transform;
        }
        RotationRoot.rotation = CalcRotation(logicUnit.LogicDir.ConvertViewVector3()); // ��ת����
    }

    protected virtual void Update()
    {
        UpdateDirection();
        UpdatePosition();
    }

    protected Vector3 viewTargetPos = Vector3.zero;
    int predictCount; // ��ǰԤ��֡��
    // ����λ��
    void UpdatePosition()
    {
        if (EnablePredictPos)
        {
            if (logicUnit.isPosChanged)
            {
                // �߼�֡���£�Ŀ��λ�ø��µ�ʵ��ֵ
                viewTargetPos = logicUnit.LogicPos.ConvertViewVector3();
                logicUnit.isPosChanged = false;
                predictCount = 0;
            }
            else
            {
                // �߼�֡δ���£����ֶ���ҪԤ��
                if (predictCount > MaxPredictCnt) return;

                Vector3 predictDeltaPos = Time.deltaTime * logicUnit.LogicMoveSpeed.RawFloat * logicUnit.LogicDir.ConvertViewVector3();
                viewTargetPos += predictDeltaPos;
                ++ predictCount;
            }

            // ƽ���ƶ�
            if (EnableSmoothMove)
            {
                transform.position = Vector3.Lerp(transform.position, viewTargetPos, Time.deltaTime * MoveSmoothValue);
            }
            else
            {
                transform.position = viewTargetPos;
            }
        }
        else
        {
            // ǿ����15֡(�߼�֡)����λ��
            transform.position = logicUnit.LogicPos.ConvertViewVector3();
        }
    }

    protected Vector3 viewTargetDir = Vector3.zero; // ����Ŀ�귽��(�߼�����)
    // ���±��ַ���
    void UpdateDirection()
    {
        // ��Ҫת��
        if (logicUnit.IsDirChanged)
        {
            viewTargetDir = GetUnitViewDir();
            logicUnit.IsDirChanged = false;
        }
        if (EnableSmoothRotate)
        {
            // ƽ��ת��
            float threshold = Time.deltaTime * RotateSmoothValue;
            float angle = Vector3.Angle(RotationRoot.forward, viewTargetDir);
            float angleMult = (angle / 180) * AngleMultiplier;

            if (viewTargetDir != Vector3.zero)
            {
                Vector3 innerDir = Vector3.Lerp(RotationRoot.forward, viewTargetDir, threshold * angleMult);
                RotationRoot.rotation = CalcRotation(innerDir);
            }
        }
        else
        {
            // ��ƽ��ת��
            RotationRoot.rotation = CalcRotation(viewTargetDir);
        }
    }

    // ���� ��������� forward ���뵱ǰ dir �ķ���ƫ��
    protected Quaternion CalcRotation(Vector3 targetDir)
    {
        return Quaternion.FromToRotation(Vector3.forward, targetDir);
    }

    protected virtual Vector3 GetUnitViewDir()
    {
        return logicUnit.LogicDir.ConvertViewVector3();
    }
}

/// <summary>
/// ����ͼ��Ԫ
/// </summary>
public abstract class MainViewUnit : BaseViewUnit
{
    public Transform skillRange; // �չ����ܷ�Χ

    float animMoveSpeedBase;
    MainLogicUnit mainLogicUnit = null;
    public override void Init(BaseLogicUnit logicUnit)
    {
        base.Init(logicUnit);
        mainLogicUnit = logicUnit as MainLogicUnit;
        // ��������
        animMoveSpeedBase = mainLogicUnit.LogicMoveSpeed.RawFloat;

        // ���ñ��ֵ�Ԫ����С��ͼ
        FightManager.Instance.playWnd.AddMiniIconItemInfo(mainLogicUnit);
    }

    /// <summary>
    /// ���ò���ʾ����ָʾ��Χ
    /// </summary>
    /// <param name="state"></param>
    /// <param name="range"></param>
    public void SetAtkSkillRange(bool state, float range = 2.5f)
    {
        if (skillRange != null)
        {
            range += mainLogicUnit.unitData.unitCfg.colliCfg.mRadius.RawFloat;
            skillRange.localScale = new Vector3(range / 2.5f, range / 2.5f);
            skillRange.gameObject.SetActive(state);
        }
    }

    /// �����ͷż���ʱ�ķ���
    public void UpdateSkillRotation(ShawVector3 skillRotation)
    {
        viewTargetDir = skillRotation.ConvertViewVector3();
    }
}