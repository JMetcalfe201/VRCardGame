using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayingFieldTestBench : NetworkBehaviour
{
    private PlayingField field;

	// Use this for initialization
	void Start () 
    {
        field = GetComponent<PlayingField>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (isLocalPlayer)
        {
            HandleInput();
        }
	}

    private void HandleInput()
    {
        if(!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            field.AddCard(0, true);
        }

        if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            field.AddCard(1, true);
        }
    }
}

