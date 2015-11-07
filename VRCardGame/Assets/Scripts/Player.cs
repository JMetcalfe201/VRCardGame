using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private int lifepoints;

    [SyncVar]
    private bool isFirstPlayer;

    public Vector2 cursorPosition;
    public GameplayManager gpManager;

    public Text playerText;

	// Use this for initialization
	void Start ()
    {
	    if(!isLocalPlayer)
        {
            GetComponent<AudioListener>().enabled = false;
            GetComponent<Camera>().tag = "Untagged";
            GetComponent<Camera>().enabled = false;
        }

        gpManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();

        isFirstPlayer = (NetworkManager.singleton.numPlayers == 1);

        if (isLocalPlayer)
        {
            playerText = GameObject.Find("PlayerText").GetComponent<Text>();
            playerText.text = "You are player: " + (isFirstPlayer ? 1 : 2);
        }
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
        if (!VRSettings.enabled)
        {
            float lookSensitivity = 5.0f;

            transform.eulerAngles = transform.eulerAngles + new Vector3(0f, Input.GetAxis("Mouse X") * lookSensitivity, 0f);
            transform.eulerAngles = transform.eulerAngles + new Vector3(-Input.GetAxis("Mouse Y") * lookSensitivity, 0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if ((gpManager.isPlayerOnesTurn() && isFirstPlayer) || (!gpManager.isPlayerOnesTurn() && !isFirstPlayer))
            {
                Debug.Log("Turn: " + gpManager.isPlayerOnesTurn() + "Player: " + isFirstPlayer);

                CmdgpManagerAdvancePhase();
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleNetworkGUI();
        }
    }

    [Command]
    private void CmdgpManagerAdvancePhase()
    {
        gpManager.AdvancePhase();
    }

    private void ToggleNetworkGUI()
    {
        NetworkManagerHUD hud = NetworkManager.singleton.GetComponent<NetworkManagerHUD>();
        hud.showGUI = !hud.showGUI;
    }

    public void ChangeLifePoints(int points)
    {
        if(hasAuthority)
        {
            lifepoints += points;
        }
    }
}
