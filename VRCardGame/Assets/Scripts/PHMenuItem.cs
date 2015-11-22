using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.Networking;

public class PHMenuItem : MonoBehaviour
{
    public UnityEvent action;

    public void Select()
    {
        action.Invoke();
    }
}
