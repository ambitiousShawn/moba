
using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ���ܰ�ť
/// </summary>
public partial class SkillItem : CommonListenerRoot
{
    public Image ImgCycle;
    public Image ImgSkillIcon;
    public Image ImgCD;
    public Text Txt_CD;

    public Image ImgPoint;
    public Image ImgForbid;
    public Transform EffectRoot;

    int skillIndex; // �������
    SkillConfig skillConf; // ��������
    float pointDis; // �����϶�λ��
    Vector2 startPos; // ��ʼ��קλ��

    HeroView heroView;

    public void InitSkillItem(SkillConfig skillConf, int skillIndex)
    {
        EffectRoot.gameObject.SetActive(false);

        // heroView = BattleSys.Instance.GetSelfHero().mainViewUnit as HeroView;
        this.skillIndex = skillIndex;
        this.skillConf = skillConf;

        pointDis = Screen.height * 1.0f / ClientConfig.ScreenStandardHeight * ClientConfig.SkillOPDis;
        if (!skillConf.isNormalAttack)
        {
            // ������ͨ����������С����
            ImgSkillIcon.sprite = AssetsSvc.Instance.LoadSprite("ResImages/PlayWnd/" + skillConf.iconName);
            ImgCD.gameObject.SetActive(false);
            Txt_CD.gameObject.SetActive(false);

            OnClickDown(ImgSkillIcon.gameObject, (evt, args) =>
            {
                startPos = evt.position;
                ImgCycle.gameObject.SetActive(true);
                ImgPoint.gameObject.SetActive(true);
                ShowSkillAtkRange(true);

                if (skillConf.releaseModeType == EReleaseModeType.Position)
                {
                    // ��Ҫѡ��λ�õļ���
                    // heroView.SetSkillGuide(skillIndex, true, EReleaseModeType.Position, Vector3.zero);

                }
                else if (skillConf.releaseModeType == EReleaseModeType.Direction)
                {
                    // ��Ҫѡ������ļ���
                    // heroView.SetSkillGuide(skillIndex, true, EReleaseModeType.Direction, Vector3.zero);
                }
            });
            OnDrag(ImgSkillIcon.gameObject, (evt, args) =>
            {
                Vector2 dir = evt.position - startPos;
                float len = dir.magnitude;
                if (len > pointDis)
                {
                    Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);
                    ImgPoint.transform.position = startPos + clampDir;
                }
                else
                {
                    ImgPoint.transform.position = evt.position;
                }

                if (skillConf.releaseModeType == EReleaseModeType.Position)
                {
                    // ��Χ����ָ��
                    if (dir == Vector2.zero)
                    {
                        return;
                    }
                    // dir = BattleSys.Instance.SkillDisMultipler * dir;
                    Vector2 clampDir = Vector2.ClampMagnitude(dir, skillConf.targetConf.selectRange);
                    Vector3 clampDirVector3 = new Vector3(clampDir.x, 0, clampDir.y);
                    clampDirVector3 = Quaternion.Euler(0, 45, 0) * clampDirVector3;
                    // heroView.SetSkillGuide(skillIndex, true, EReleaseModeType.Position, clampDirVector3);
                }
                else if (skillConf.releaseModeType == EReleaseModeType.Direction)
                {
                    // ������ָ��
                    Vector3 dirVector3 = new Vector3(dir.x, 0, dir.y);
                    dirVector3 = Quaternion.Euler(0, 45, 0) * dirVector3;
                    // heroView.SetSkillGuide(skillIndex, true, EReleaseModeType.Direction, dirVector3.normalized);
                }
                else
                {
                    LogCore.Warn(skillConf.releaseModeType.ToString());
                }

                if (len >= ClientConfig.SkillCancelDis)
                {
                    // SetState(BattleSys.Instance.playWnd.imgCancelSkill);
                }
                else
                {
                    // SetState(BattleSys.Instance.playWnd.imgCancelSkill, false);
                }
            });

            OnClickUp(ImgSkillIcon.gameObject, (evt, args) =>
            {
                Vector2 dir = evt.position - startPos;
                ImgPoint.transform.position = transform.position;

                ImgCycle.gameObject.SetActive(false);
                ImgPoint.gameObject.SetActive(false);

                // SetState(BattleSys.Instance.playWnd.imgCancelSkill, false);
                ShowSkillAtkRange(false);

                if (dir.magnitude >= ClientConfig.SkillCancelDis)
                {
                    LogCore.Log("ȡ�������ͷ�");
                    // heroView.DisableSkillGuide(skillIndex);
                    return;
                }
                if (skillConf.releaseModeType == EReleaseModeType.Click)
                {
                    LogCore.Log("ֱ���ͷż���");
                    ClickSkillItem();
                }
                else if (skillConf.releaseModeType == EReleaseModeType.Position)
                {
                    // dir = BattleSys.Instance.SkillDisMultipler * dir;
                    Vector2 clampDir = Vector2.ClampMagnitude(dir, skillConf.targetConf.selectRange);
                    LogCore.Log("Pos Info:" + clampDir.ToString());
                    // heroView.DisableSkillGuide(skillIndex);
                    Vector3 clampDirVector3 = new Vector3(clampDir.x, 0, clampDir.y);
                    clampDirVector3 = Quaternion.Euler(0, 45, 0) * clampDirVector3;

                    ClickSkillItem(clampDirVector3);
                }
                else if (skillConf.releaseModeType == EReleaseModeType.Direction)
                {
                    if (dir == Vector2.zero)
                    {
                        return;
                    }
                    // heroView.DisableSkillGuide(skillIndex);

                    Vector3 dirVector3 = new Vector3(dir.x, 0, dir.y);
                    dirVector3 = Quaternion.Euler(0, 45, 0) * dirVector3;
                    ClickSkillItem(dirVector3);
                }
                else
                {
                    LogCore.Warn("Skill Release has cancel!");
                }
                ShowEffect();
            });
        }
        else
        {
            // ��ͨ����
            OnClickDown(ImgSkillIcon.gameObject, (evt, args) =>
            {
                // ��ʾ��Χͼ��
                ShowSkillAtkRange(true);
                ClickSkillItem();
            });

            OnClickUp(ImgSkillIcon.gameObject, (evt, args) =>
            {
                ShowSkillAtkRange(false);
                ShowEffect();
            });
        }
    }

    Coroutine co = null;
    void ShowEffect()
    {
        if (co != null)
        {
            StopCoroutine(co);
            EffectRoot.gameObject.SetActive(false);
        }
        EffectRoot.gameObject.SetActive(true);
        co = StartCoroutine(DisableEffect());
    }

    IEnumerator DisableEffect()
    {
        yield return new WaitForSeconds(0.5f);
        EffectRoot.gameObject.SetActive(false);
    }

    private void ShowSkillAtkRange(bool state) 
    {
        if (skillConf.targetConf != null)
        {
            // heroView.SetAtkSkillRange(state, skillConf.targetConf.selectRange);     
        }
    }

    // �������ͼ��
    public void ClickSkillItem()
    {
        // BattleSys.Instance.SendSkillKey(skillConf.skillID, Vector2.zero);
    }

    /// <summary>
    /// ���������ͷŷ���
    /// </summary>
    /// <param name="arg"></param>
    public void ClickSkillItem(Vector3 arg)
    {
        // BattleSys.Instance.SendSkillKey(skillConf.skillID, arg);
    }

    public void SetForbidState(bool state)
    {
        ImgForbid.gameObject.SetActive(state);
    }
}
