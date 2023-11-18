using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Tip : MonoBehaviour
{
    public Image img_bg;
    public Text text_info;
    public Animator anim;

    private bool isPlaying = false;
    private Queue<string> _tipsQue = new Queue<string>();
    private void Awake()
    {
        img_bg.gameObject.SetActive(false);
        _tipsQue.Clear();
    }

    public void AddTipToQueue(string info)
    {
        _tipsQue.Enqueue(info);
    }

    void Update()
    {
        if (_tipsQue.Count > 0 && !isPlaying)
        {
            string info = _tipsQue.Dequeue();
            isPlaying = true;
            text_info.text = info;
            img_bg.gameObject.SetActive(true);
            transform.SetAsLastSibling();
            anim.Play("TipsShow", 0, 0) ;
        }
    }

    public void AnimPlayDone()
    {
        img_bg.gameObject.SetActive(false);
        isPlaying = false;
    }
}
