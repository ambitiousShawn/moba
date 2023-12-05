/*using ShawnFramework.CommonModule;
using ShawnFramework.ShawLog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EJumpType
{
    None,
    SkillDamage,
    BuffDamage,
    Cure,
    SlowSpeed,
}

public enum EJumpAnim
{
    None,
    LeftCurve,
    RightCurve,
    CenterUp,
}

public class JumpUpdateInfo
{
    public int jumpVal;
    public Vector2 pos;
    public EJumpType jumpType;
    public EJumpAnim jumpAni;
}

public class SceneJumpItem : MonoBehaviour
{
    public RectTransform rect;
    public Animator anim;
    public Text text_info;

    public int MaxFont;
    public int MinFont;
    public int MaxFontValue;
    public Color SkillDamageColor;
    public Color BuffDamageColor;
    public Color CureDamageColor;
    public Color SlowSpeedColor;

    private JumpNumPool ownerPool;

    public void Init(JumpNumPool ownerPool)
    {
        this.ownerPool = ownerPool;
    }

    public void Show(JumpUpdateInfo info)
    {
        int fontSize = (int)Mathf.Clamp(info.jumpVal * 1.0f / MaxFontValue, MinFont, MaxFont);
        text_info.fontSize = fontSize;
        rect.anchoredPosition = info.pos;

        switch (info.jumpType)
        {
            case EJumpType.SkillDamage:
                text_info.text = info.jumpVal.ToString();
                text_info.color = SkillDamageColor;
                break;
            case EJumpType.BuffDamage:
                text_info.text = info.jumpVal.ToString();
                text_info.color = BuffDamageColor;
                break;
            case EJumpType.Cure:
                text_info.text = "+" + info.jumpVal;
                text_info.color = CureDamageColor;
                break;
            case EJumpType.SlowSpeed:
                text_info.text = "减速";
                text_info.color = SlowSpeedColor;
                break;
            default:
                break;
        }

        switch (info.jumpAni)
        {
            case EJumpAnim.LeftCurve:
                anim.Play("JumpLeft", 0);
                break;
            case EJumpAnim.RightCurve:
                anim.Play("JumpRight", 0);
                break;
            case EJumpAnim.CenterUp:
                anim.Play("JumpCenter", 0);
                break;
            default:
                break;
        }

        StartCoroutine(Recycle());
    }

    IEnumerator Recycle()
    {
        yield return new WaitForSeconds(0.75f);
        anim.Play("Empty");
        ownerPool.PushOne(this);
    }
}

public class JumpNumPool
{
    Transform poolRoot;
    private Queue<SceneJumpItem> jumpNumQue;

    public JumpNumPool(int count, Transform poolRoot)
    {
        this.poolRoot = poolRoot;
        jumpNumQue = new Queue<SceneJumpItem>();

        for (int i = 0; i < count; i++)
        {
            PushOne(CreateOne());
        }
    }

    int index = 0;
    int Index
    {
        get
        {
            return ++index;
        }
    }
    SceneJumpItem CreateOne()
    {
        GameObject go = AssetsSvc.Instance.LoadPrefab("", "UIPrefab/DynamicItem/SceneJumpItem", 0);
        go.name = "JumpNum_" + Index;
        go.transform.SetParent(poolRoot);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        SceneJumpItem jn = go.GetComponent<SceneJumpItem>();
        jn.Init(this);
        return jn;
    }

    public SceneJumpItem PopOne()
    {
        if (jumpNumQue.Count > 0)
        {
            return jumpNumQue.Dequeue();
        }
        else
        {
            LogCore.Warn("飘字超额，动态调整上限");
            PushOne(CreateOne());
            return PopOne();
        }
    }

    public void PushOne(SceneJumpItem jn)
    {
        jumpNumQue.Enqueue(jn);
    }
}
*/