using ShawnFramework.UGUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UGUI_Tip : PanelRoot
{
    public Image img_bg;
    public Text text_info;
    public Animator anim;

    private bool isPlaying = false;
    private Queue<string> _tipsQue = new Queue<string>();
    protected override void InitPanel()
    {
        base.InitPanel();
        SetActive(img_bg, false);
        _tipsQue.Clear();
    }

    public void AddTipToQueue(string info)
    {
        _tipsQue.Enqueue(info);
    }

    void Update()
    {
        // print("ha");
        if (_tipsQue.Count > 0 && !isPlaying)
        {
            string info = _tipsQue.Dequeue();
            isPlaying = true;
            text_info.text = info;
            SetActive(img_bg, true);
            transform.SetAsLastSibling();
            anim.Play("TipsShow", 0, 0) ;
        }
    }

    public void AnimPlayDone()
    {
        SetActive(img_bg, false);
        isPlaying = false;
    }
}
