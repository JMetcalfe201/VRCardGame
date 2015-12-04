using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class NetworkMenuRelay : MonoBehaviour
{
    NetworkManager manager;

    void Start()
    {
        manager = GetComponent<NetworkManager>();
    }

    public void StartHost()
    {
        manager.StartHost();
    }

    public void StartClient()
    {
        manager.networkAddress = GameObject.Find("IPField").GetComponent<InputField>().text;
        manager.StartClient();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
