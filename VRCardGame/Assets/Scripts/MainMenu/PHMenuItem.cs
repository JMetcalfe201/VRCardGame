using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.Networking;

public class PHMenuItem : MonoBehaviour
{
    public UnityEvent action;
    public UnityEvent actionLeft;
    public UnityEvent actionRight;

    public void Select()
    {
        if(action != null)
            action.Invoke();
    }

    public void Left()
    {
        if(actionLeft != null)
            actionLeft.Invoke();
        Debug.Log("Left");
    }

    public void Right()
    {
        if (actionRight != null)
            actionRight.Invoke();
        Debug.Log("Right");
    }
}
