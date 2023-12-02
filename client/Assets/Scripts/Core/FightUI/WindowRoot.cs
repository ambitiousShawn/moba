using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowRoot : MonoBehaviour
{
    protected void OnClick(GameObject go, Action<PointerEventData, object[]> clickCB, params object[] args)
    {
        ShawListener listener = GetOrAddComponent<ShawListener>(go);
        listener.onClick = clickCB;
        if (args != null)
        {
            listener.args = args;
        }
    }
    protected void OnClickDown(GameObject go, Action<PointerEventData, object[]> clickDownCB, params object[] args)
    {
        ShawListener listener = GetOrAddComponent<ShawListener>(go);
        listener.onClickDown = clickDownCB;
        if (args != null)
        {
            listener.args = args;
        }
    }
    protected void OnClickUp(GameObject go, Action<PointerEventData, object[]> clickUpCB, params object[] args)
    {
        ShawListener listener = GetOrAddComponent<ShawListener>(go);
        listener.onClickUp = clickUpCB;
        if (args != null)
        {
            listener.args = args;
        }
    }
    protected void OnDrag(GameObject go, Action<PointerEventData, object[]> dragCB, params object[] args)
    {
        ShawListener listener = GetOrAddComponent<ShawListener>(go);
        listener.onDrag = dragCB;
        if (args != null)
        {
            listener.args = args;
        }
    }

    private T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t == null)
        {
            t = go.AddComponent<T>();
        }
        return t;
    }

    // UI¼ÆÊ±Æ÷
    protected MonoTimer CreateMonoTimer(
        Action<int> cbAction,
        float intervelTime,
        int loopCount = 1,
        Action<bool, float, float> prgAction = null,
        Action endAction = null,
        float delayTime = 0
        )
    {
        MonoTimer timer = new MonoTimer(
            cbAction,
            intervelTime,
            loopCount,
            prgAction,
            endAction,
            delayTime);
        Launcher.Instance.AddMonoTimer(timer);
        return timer;
    }
}

public class ShawListener :
MonoBehaviour,
IPointerClickHandler,
IPointerDownHandler,
IPointerUpHandler,
IDragHandler
{
    public Action<PointerEventData, object[]> onClick;
    public Action<PointerEventData, object[]> onClickDown;
    public Action<PointerEventData, object[]> onClickUp;
    public Action<PointerEventData, object[]> onDrag;

    public object[] args = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(eventData, args);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        onClickDown?.Invoke(eventData, args);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        onClickUp?.Invoke(eventData, args);
    }
    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData, args);
    }
}
