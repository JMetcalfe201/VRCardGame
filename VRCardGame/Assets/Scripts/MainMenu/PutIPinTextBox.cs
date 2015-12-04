using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public class PutIPinTextBox : MonoBehaviour 
{
    public TextMesh text;
	// Use this for initialization
	void Start () 
    {
        StartCoroutine(SetIP());
	}

    IEnumerator SetIP()
    {
        Network.Connect("www.google.com");

        yield return new WaitForSeconds(0.25f);

        text.text = "IP: " + Network.player.externalIP;
        Network.Disconnect();
    }
}
