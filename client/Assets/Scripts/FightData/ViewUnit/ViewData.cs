using ShawnFramework.ShawMath;
using UnityEngine;

/// <summary>
/// 基本视图单元
/// </summary>
public abstract class BaseViewUnit : MonoBehaviour
{
    // 一些配置数据
    [Header("平滑转向")]
    public bool EnableSmoothRotate = true;             // 开启平滑转向
    public float RotateSmoothValue = 10;               // 旋转平滑参数
    public float AngleMultiplier = 8;                  // 角度变化幅度

    [Header("平滑移动")]
    public bool EnablePredictPos = true;               // 开启预测位置移动算法
    public bool EnableSmoothMove = true;               // 开启平滑移动
    public int MaxPredictCnt = 15;                     // 最大预测位置次数     
    public float MoveSmoothValue = 10;                 // 移动平滑幅度

    Transform RotationRoot; // 旋转根节点
    BaseLogicUnit logicUnit = null; // 逻辑实体
    public virtual void Init(BaseLogicUnit logicUnit)
    {
        this.logicUnit = logicUnit;
        gameObject.name = logicUnit.unitName + "_" + gameObject.name;
        transform.position = logicUnit.LogicPos.ConvertViewVector3();
        if (RotationRoot == null)
        {
            RotationRoot = transform;
        }
        RotationRoot.rotation = CalcRotation(logicUnit.LogicDir.ConvertViewVector3()); // 旋转矫正
    }

    protected virtual void Update()
    {
        UpdateDirection();
        UpdatePosition();
    }

    protected Vector3 viewTargetPos = Vector3.zero;
    int predictCount; // 当前预测帧数
    // 更新位置
    void UpdatePosition()
    {
        if (EnablePredictPos)
        {
            if (logicUnit.isPosChanged)
            {
                // 逻辑帧更新，目标位置更新到实际值
                viewTargetPos = logicUnit.LogicPos.ConvertViewVector3();
                logicUnit.isPosChanged = false;
                predictCount = 0;
            }
            else
            {
                // 逻辑帧未更新，表现端需要预测
                if (predictCount > MaxPredictCnt) return;

                Vector3 predictDeltaPos = Time.deltaTime * logicUnit.LogicMoveSpeed.RawFloat * logicUnit.LogicDir.ConvertViewVector3();
                viewTargetPos += predictDeltaPos;
                ++ predictCount;
            }

            // 平滑移动
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
            // 强行以15帧(逻辑帧)更新位置
            transform.position = logicUnit.LogicPos.ConvertViewVector3();
        }
    }

    protected Vector3 viewTargetDir = Vector3.zero; // 表现目标方向(逻辑方向)
    // 更新表现方向
    void UpdateDirection()
    {
        // 需要转向
        if (logicUnit.IsDirChanged)
        {
            viewTargetDir = GetUnitViewDir();
            logicUnit.IsDirChanged = false;
        }
        if (EnableSmoothRotate)
        {
            // 平滑转向
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
            // 非平滑转向
            RotationRoot.rotation = CalcRotation(viewTargetDir);
        }
    }

    // 计算 世界坐标的 forward 轴与当前 dir 的方向偏移
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
/// 主视图单元
/// </summary>
public abstract class MainViewUnit : BaseViewUnit
{
    public Transform skillRange; // 普攻技能范围

    float animMoveSpeedBase;
    MainLogicUnit mainLogicUnit = null;
    public override void Init(BaseLogicUnit logicUnit)
    {
        base.Init(logicUnit);
        mainLogicUnit = logicUnit as MainLogicUnit;
        // 基础移速
        animMoveSpeedBase = mainLogicUnit.LogicMoveSpeed.RawFloat;

        // 将该表现单元加入小地图
        FightManager.Instance.playWnd.AddMiniIconItemInfo(mainLogicUnit);
    }

    /// <summary>
    /// 设置并显示技能指示范围
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

    /// 更新释放技能时的方向
    public void UpdateSkillRotation(ShawVector3 skillRotation)
    {
        viewTargetDir = skillRotation.ConvertViewVector3();
    }
}