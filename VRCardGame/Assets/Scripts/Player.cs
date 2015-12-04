using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private int lifepoints;

    [SyncVar(hook = "firstPlayerUpdated")]
    public int playerNumber;

    public Vector2 cursorPosition;
    
    public GameplayManager gpManager;

    private PlayingFieldOneSlot field;

    public Text playerText;
    public Text phaseText;

    void Awake()
    {
        playerNumber = -1;
        gpManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
        field = GetComponent<PlayingFieldOneSlot>();
    }

    void Start()
    {
        if (!isLocalPlayer)
        {
            GetComponent<AudioListener>().enabled = false;
            GetComponent<Camera>().tag = "Untagged";
            GetComponent<Camera>().enabled = false;

            UpdatePlayerNumInfo();
        }
        else
        {
            CmdInitPlayer();
            UpdateUI();
        }
    }

    [Command]
    private void CmdInitPlayer()
    {
        if (hasAuthority)
        {
            if(gpManager.player1 == -1)
            {
                gpManager.player1 = (int)netId.Value;
                playerNumber = 1;
            }
            else
            {
                gpManager.player2 = (int)netId.Value;
                playerNumber = 2;
            }
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
        /*
        if (!VRSettings.enabled)
        {
            float lookSensitivity = 5.0f;

            transform.eulerAngles = transform.eulerAngles + new Vector3(0f, Input.GetAxis("Mouse X") * lookSensitivity, 0f);
            transform.eulerAngles = transform.eulerAngles + new Vector3(-Input.GetAxis("Mouse Y") * lookSensitivity, 0f, 0f);
        }
         */

        if (Input.GetButtonDown("Advance Phase"))
        {

            if ((gpManager.isPlayerOnesTurn() && IsFirstPlayer()) || (!gpManager.isPlayerOnesTurn() && !IsFirstPlayer()))
            {
                CmdgpManagerAdvancePhase();
            }
        }

        if (Input.GetButtonDown("Play Card"))
        {
            if (gpManager.GetCurrentPhase() == EGamePhase.MainPhase1 && ((gpManager.isPlayerOnesTurn() && IsFirstPlayer()) || (!gpManager.isPlayerOnesTurn() && !IsFirstPlayer())))
            {
                field.AddMonsterCard();
            }
        }

        if (Input.GetButtonDown("Attack"))
        {
            if(gpManager.GetCurrentPhase() == EGamePhase.BattlePhase && ((gpManager.isPlayerOnesTurn() && IsFirstPlayer()) || (!gpManager.isPlayerOnesTurn() && !IsFirstPlayer())))
            {
                field.Attack();
            }
        }
    }

    public void UpdateUI()
    {
        if (isLocalPlayer)
        {
            phaseText.text = "It is player " + (gpManager.GetFirstPlayerTurn() ? 1 : 2) + "'s turn. Phase: " + gpManager.GetCurrentPhase().ToString();
        }
    }

    [Command]
    private void CmdgpManagerAdvancePhase()
    {
        gpManager.AdvancePhase();
    }

    public void ChangeLifePoints(int points)
    {
        if(hasAuthority)
        {
            lifepoints += points;
        }
    }

    public bool IsFirstPlayer()
    {
        return playerNumber == 1;
    }

    private void firstPlayerUpdated(int newVal)
    {
        playerNumber = newVal;

        if (isLocalPlayer)
        {
            UpdatePlayerNumInfo();
        }
    }

    private void UpdatePlayerNumInfo()
    {
        gameObject.name = "Player " + playerNumber;
        playerText.text = "You are player: " + playerNumber;
        transform.eulerAngles = (IsFirstPlayer() ? new Vector3(0, 270f, 0) : new Vector3(0, 90f, 0));

        field.InitPlayingField();
    }
}
