/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SceneHPItem : WindowRoot
{
    public RectTransform rect;
    public Image ImgPrg;

    protected bool isFriend;
    Transform root;
    int hpVal;

    public virtual void InitItem(MainLogicUnit unit, Transform root, int hp)
    {
        ETeamType selfTeam = FightManager.Instance.GetCurrentUserTeam();
        isFriend = unit.IsTeam(selfTeam);

        ImgPrg.fillAmount = 1;
        this.root = root;
        hpVal = hp;
    }

    public void RefreshHPPrg(int val)
    {
        gameObject.SetActive(val != 0);
        ImgPrg.fillAmount = val * 1.0f / hpVal;
    }

    public virtual void SetStateInfo(EAbnormalState state, bool show) { }

    class JumpPack
    {
        public SceneJumpItem sji;
        public JumpUpdateInfo jui;
    }
    Queue<JumpPack> packQueue = new Queue<JumpPack>();

    public void AddHPJumpToQueue(SceneJumpItem sjn, JumpUpdateInfo jui)
    {
        packQueue.Enqueue(new JumpPack { sji = sjn, jui = jui });
    }

    public float JumpInterval;
    bool canShow = true;
    float timeCounter = 0;

    public void Check()
    {
        if (packQueue.Count > 0 && canShow)
        {
            JumpPack pack = packQueue.Dequeue();
            pack.sji.Show(pack.jui);
        }
        if (canShow == false)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter > JumpInterval / 1000)
            {
                timeCounter = 0;
                canShow = true;
            }
        }

        if (root != null)
        {
            float scaleRate = 1.0f * ClientConfig.ScreenStandardHeight / Screen.height;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(root.position);
            rect.anchoredPosition = screenPos * scaleRate;
        }
    }
}*/