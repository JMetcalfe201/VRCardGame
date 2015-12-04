using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayingFieldTestBench : NetworkBehaviour
{
    private PlayingFieldOneSlot field;
    //private PlayingField field;

	// Use this for initialization
	void Start () 
    {
        field = GetComponent<PlayingFieldOneSlot>();
        //field = GetComponent<PlayingField>();
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
            field.AddMonsterCard();
            //field.setMonsterCardByIndex(field.tempMonsterCard, 0);
        }
    }
}

