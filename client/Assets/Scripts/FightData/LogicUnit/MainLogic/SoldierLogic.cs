using ShawnFramework.ShawMath;
/// <summary>
/// Ӣ���߼���
/// </summary>
public class SoldierLogic : MainLogicUnit
{
    public int soldierID; // С��ID
    public int waveIndex; // ������
    public int orderIndex; // ��ǰ���εĵڼ���
    public string soldierName; 

    public SoldierLogic(SoldierData ud) : base(ud)
    {
        soldierID = ud.soldierID;
        waveIndex = ud.waveIndex;
        orderIndex = ud.orderIndex;

        unitType = EUnitType.Soldier;
        unitName = $"{ud.unitCfg.unitName}_w:{waveIndex}_o:{orderIndex}";
        pathPrefix = "charactor";
    }


    ShawInt sqrSearchDis; // ���������ƽ��
    TargetConfig cfg;        // Ŀ��������Ϣ
    public override void LogicInit()
    {
        base.LogicInit();
        sqrSearchDis = (ShawInt)skillArr[0].config.targetConf.searchDis * (ShawInt)skillArr[0].config.targetConf.searchDis;
        cfg = skillArr[0].config.targetConf;    //  �����ͨ�����ļ�������
        
        // ���ְ���Ĭ�Ϲ����н�
        if (IsTeam(ETeamType.Blue))
        {
            InputDir = ShawVector3.right;
        }
        else
        {
            InputDir = ShawVector3.left;
        }
    }

    private int AITickInterval = 5;
    private int AITickIntervalCounter = 0;
    public override void LogicTick()
    {
        base.LogicTick();
        
        if (AITickIntervalCounter < AITickInterval)
        {
            AITickIntervalCounter += 1;
            return;
        }
        AITickIntervalCounter = 0;

        // ÿ5���߼�ִ֡��һ��AI
        if (CanReleaseSkill(unitData.unitCfg.skillArr[0]))
        {
            MainLogicUnit target = CalcSkillSelectTarget.FindSingleTargetByConfig(this, cfg, ShawVector3.zero);
            
            if (target != null)
            {
                // ����Ŀ�꣬��ʼ��ͨ����
                InputDir = ShawVector3.zero;    // ��ͣ����
                skillArr[0].ReleaseSkill(ShawVector3.zero);
            }
            else
            {
                // ������Ŀ��
                target = CalcSkillSelectTarget.FindMinDistanceEnemyTarget(this, cfg);
                if (target != null)
                {
                    // ��ǰ��������ĵ�λ�ƶ�
                    ShawVector3 offset = target.LogicPos - LogicPos;
                    ShawInt sqrDistance = offset.sqrMagnitude;
                    if (sqrDistance < sqrSearchDis)
                    {
                        // �ҵ����Ŀ�꣬����Ŀ���ƶ�
                        InputDir = offset.normalized;
                    }
                    else
                    {
                        // δ�ҵ����Ŀ�꣬˳����ǰ�ƶ�
                        if (IsTeam(ETeamType.Blue))
                        {
                            InputDir = ShawVector3.right;
                        }
                        else
                        {
                            InputDir = ShawVector3.left;
                        }
                    }
                }
            }
        }
    }
}