using UnityEngine;
/// <summary>
/// Ӣ����ͼ��
/// </summary>
public class HeroView : MainViewUnit
{
    [Header("�����ͷ�ָ��")]
    public Transform sk1;
    public Transform sk2;
    public Transform sk3;

    HeroLogic heroLogic; // ���������߼�ʵ��
    public override void ViewInit(BaseLogicUnit logicUnit)
     {
        base.ViewInit(logicUnit);

        heroLogic = logicUnit as HeroLogic;

        sk1 = transform.Find("sk1");
        sk2 = transform.Find("sk2");
        sk3 = transform.Find("sk3");

        skillRange.gameObject.SetActive(false);
        if (sk1 != null)
        {
            sk1.gameObject.SetActive(false);
        }
        if (sk2 != null)
        {
            sk2.gameObject.SetActive(false);
        }
        if (sk3 != null)
        {
            sk3.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ʧ���Ӧ��ŵļ���ָʾ
    /// </summary>
    /// <param name="skillIndex"></param>
    public void DisableSkillGuide(int skillIndex)
    {
        switch (skillIndex)
        {
            case 1:
                if (sk1 != null)
                {
                    sk1.gameObject.SetActive(false);
                }
                break;
            case 2:
                if (sk2 != null)
                {
                    sk2.gameObject.SetActive(false);
                }
                break;
            case 3:
                if (sk3 != null)
                {
                    sk3.gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// �����Ӧ�ļ���ָʾ��������λ�ò���
    /// </summary>
    /// <param name="skillIndex"></param>
    /// <param name="state"></param>
    /// <param name="modeType"></param>
    /// <param name="vector"></param>
    public void SetSkillGuide(int skillIndex, bool state, EReleaseModeType modeType, Vector3 vector)
    {
        switch (skillIndex)
        {
            case 1:
                sk1.gameObject.SetActive(state);
                if (state)
                {
                    UpdateSkillGuide(sk1, modeType, vector);
                }
                break;
            case 2:
                if (sk2 == null) return;
                sk2.gameObject.SetActive(state);
                if (state)
                {
                    UpdateSkillGuide(sk2, modeType, vector);
                }
                break;
            case 3:
                sk3.gameObject.SetActive(state);
                if (state)
                {
                    UpdateSkillGuide(sk3, modeType, vector);
                }
                break;
            default:
                break;
        }
    }

    void UpdateSkillGuide(Transform sk, EReleaseModeType modeType, Vector3 vector)
    {
        if (modeType == EReleaseModeType.Position)
        {
            sk.localPosition = vector;
        }
        else
        {
            float angle = Vector2.SignedAngle(new Vector2(vector.x, vector.z), new Vector2(0, 1));
            sk.localEulerAngles = new Vector3(0, angle, 0);
        }
    }
}