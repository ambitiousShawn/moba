
using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 技能按钮
/// </summary>
public partial class SkillItem : WindowRoot
{
    public Image ImgCycle;
    public Image ImgSkillIcon;
    public Image ImgCD;
    public Text TextCD;

    public Image ImgPoint;
    public Image ImgForbid;
    public Transform EffectRoot;

    int skillIndex; // 技能序号
    SkillConfig skillConf; // 技能配置
    float pointDis; // 限制拖动位移
    Vector2 startPos = Vector2.zero; // 开始拖拽位置

    HeroView heroView;

    public void InitSkillItem(SkillConfig skillConf, int skillIndex)
    {
        EffectRoot.gameObject.SetActive(false);

        heroView = FightManager.Instance.GetSelfHero(Launcher.Instance.SelfIndex).mainViewUnit as HeroView;
        this.skillIndex = skillIndex;
        this.skillConf = skillConf;

        pointDis = Screen.height * 1.0f / ClientConfig.ScreenStandardHeight * ClientConfig.SkillOPDis;
        if (!skillConf.isNormalAttack)
        {
            // 不是普通攻击，开启小轮盘
            ImgSkillIcon.sprite = null;
            ImgCD.gameObject.SetActive(false);
            TextCD.gameObject.SetActive(false);

            OnClickDown(ImgSkillIcon.gameObject, (evt, args) =>
            {
                startPos = evt.position;
                ImgCycle.gameObject.SetActive(true);
                ImgPoint.gameObject.SetActive(true);
                ShowSkillAtkRange(true);

                if (skillConf.releaseModeType == EReleaseModeType.Position)
                {
                    // 需要选定位置的技能
                    heroView.SetSkillGuide(skillIndex, true, EReleaseModeType.Position, Vector3.zero);

                }
                else if (skillConf.releaseModeType == EReleaseModeType.Direction)
                {
                    // 需要选定方向的技能
                    heroView.SetSkillGuide(skillIndex, true, EReleaseModeType.Direction, Vector3.zero);
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
                    if (dir == Vector2.zero)
                    {
                        return;
                    }
                    dir = FightManager.Instance.SkillDisMultipler * dir;
                    Vector2 clampDir = Vector2.ClampMagnitude(dir, skillConf.targetConf.selectRange);
                    Vector3 clampDirVector3 = new Vector3(clampDir.x, 0, clampDir.y);
                    clampDirVector3 = Quaternion.Euler(0, 45, 0) * clampDirVector3;
                    heroView.SetSkillGuide(skillIndex, true, EReleaseModeType.Position, clampDirVector3);
                }
                else if (skillConf.releaseModeType== EReleaseModeType.Direction)
                {
                    Vector3 dirVector3 = new Vector3(dir.x, 0, dir.y);
                    dirVector3 = Quaternion.Euler(0, 45, 0) * dirVector3;
                    heroView.SetSkillGuide(skillIndex, true, EReleaseModeType.Direction, dirVector3.normalized);
                }
                else
                {
                    LogCore.Warn(skillConf.releaseModeType.ToString());
                }

                if (len >= ClientConfig.SkillCancelDis)
                {
                    FightManager.Instance.playWnd.imgCancelSkill.gameObject.SetActive(true);
                }
                else
                {
                    FightManager.Instance.playWnd.imgCancelSkill.gameObject.SetActive(false) ;
                }
            });

            OnClickUp(ImgSkillIcon.gameObject, (evt, args) =>
            {
                Vector2 dir = evt.position - startPos;
                ImgPoint.transform.position = transform.position;

                ImgCycle.gameObject.SetActive(false);
                ImgPoint.gameObject.SetActive(false);

                FightManager.Instance.playWnd.imgCancelSkill.gameObject.SetActive(false);
                ShowSkillAtkRange(false);

                if (dir.magnitude >= ClientConfig.SkillCancelDis)
                {
                    LogCore.Log("取消技能释放");
                    heroView.DisableSkillGuide(skillIndex);
                    return;
                }
                if (skillConf.releaseModeType == EReleaseModeType.Click)
                {
                    LogCore.Log("直接释放技能");
                    ClickSkillItem();
                }
                else if (skillConf.releaseModeType == EReleaseModeType.Position)
                {
                    dir = FightManager.Instance.SkillDisMultipler * dir;
                    Vector2 clampDir = Vector2.ClampMagnitude(dir, skillConf.targetConf.selectRange);
                    LogCore.Log("Pos Info:" + clampDir.ToString());
                    heroView.DisableSkillGuide(skillIndex);
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
                    heroView.DisableSkillGuide(skillIndex);

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
            // 普通攻击
            OnClickDown(ImgSkillIcon.gameObject, (evt, args) =>
            {
                // 显示范围图标
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
            heroView.SetAtkSkillRange(state, skillConf.targetConf.selectRange);     
        }
    }

    // 点击技能图标
    public void ClickSkillItem()
    {
        FightManager.Instance.SendSkillOperation(skillConf.skillID, Vector2.zero);
    }

    /// <summary>
    /// 附带技能释放方向
    /// </summary>
    /// <param name="arg"></param>
    public void ClickSkillItem(Vector3 arg)
    {
        FightManager.Instance.SendSkillOperation(skillConf.skillID, arg);
    }

    public void SetForbidState(bool state)
    {
        ImgForbid.gameObject.SetActive(state);
    }

    // 检查是否是当前技能
    public bool CheckSkillID(int skillID)
    {
        return skillConf.skillID == skillID;
    }

    // 技能进入CD状态
    public void EnterCDState(int cdTime)
    {
        int sec = cdTime / 1000;
        int ms = cdTime % 1000;
        CreateMonoTimer(
            (loopCount) => {
                TextCD.text = (sec - loopCount).ToString();
            },
            1000,
            sec,
            (isDelay, loopPrg, allPrg) => {
                ImgCD.fillAmount = 1 - allPrg;
            },
            () => {
                ImgCD.gameObject.SetActive(false);
                TextCD.gameObject.SetActive(false);
                ShowEffect();
                ImgSkillIcon.raycastTarget = true;
            },
            ms);

        ImgCD.gameObject.SetActive(true);
        TextCD.gameObject.SetActive(true);
        TextCD.text = sec.ToString();
        ImgSkillIcon.raycastTarget = false;
    }
}
