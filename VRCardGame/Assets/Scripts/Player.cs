using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

using PlayingFieldClass;
using DeckClass;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private int lifepoints;

    [SyncVar]
    private bool isFirstPlayer;

    public Vector2 cursorPosition;
    public GameplayManager gpManager;

    public PlayingField field;

    public Text playerText;
    public Text phaseText;

    void Awake()
    {
        gpManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
    }

    void Start()
    {
        if (!isLocalPlayer)
        {
            GetComponent<AudioListener>().enabled = false;
            GetComponent<Camera>().tag = "Untagged";
            GetComponent<Camera>().enabled = false;
        }

        isFirstPlayer = (NetworkManager.singleton.numPlayers == 1);

        if (isLocalPlayer)
        {
            playerText.text = "You are player: " + (isFirstPlayer ? 1 : 2);
            UpdateUI();
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
                CmdgpManagerAdvancePhase();
            }
        }

        // testing deck functionality
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            CardTestType newcard = new CardTestType(Random.Range(1, 60));
            field.getDeck().addCardTop(newcard);
            field.getDeck().print();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CardTestType newcard = new CardTestType(Random.Range(1, 60));
            field.getDeck().addCardBottom(newcard);
            field.getDeck().print();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CardTestType drawnCard = field.getDeck().DrawTop();
            Debug.Log("Drew: " + drawnCard.getid());
            field.getDeck().print();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CardTestType drawnCard = field.getDeck().DrawBottom();
            Debug.Log("Drew: " + drawnCard.getid());
            field.getDeck().print();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            field.getDeck().Shuffle();
            Debug.Log("Shuffled deck");
            field.getDeck().print();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            field.setMonsterCardByIndex(field.getDeck().DrawTop(), 0);
            field.print();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            field.setEffectCardByIndex(field.getDeck().DrawTop(), 1);
            field.print();
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
}
