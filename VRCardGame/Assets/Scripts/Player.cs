using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private int lifepoints;

    [SyncVar(hook = "firstPlayerUpdated")]
    public int playerNumber;

    public Vector2 cursorPosition;

    public GameplayManager gpManager;

    private PlayingField field;
    public List<GameObject> hand;
    private Transform handLocation;

    public Text playerText;
    public Text phaseText;

    void Awake()
    {
        playerNumber = -1;
        gpManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
        field = GetComponent<PlayingField>();
        hand = new List<GameObject>();
    }

    void Start()
    {
        Cursor.visible = false;

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
            if (gpManager.player1 == -1)
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
    void Update()
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
                Debug.Log("Play card");
                field.AddMonsterCard((int)(Random.Range(0f, 1.99f)));
            }
        }

        if (Input.GetButtonDown("Attack"))
        {
            if (gpManager.GetCurrentPhase() == EGamePhase.BattlePhase && ((gpManager.isPlayerOnesTurn() && IsFirstPlayer()) || (!gpManager.isPlayerOnesTurn() && !IsFirstPlayer())))
            {
                //field.Attack();
            }
        }

        if (Input.GetButtonDown("drawcard_test"))
        {
            Debug.Log(field);
            Debug.Log(field.getDeck());
            addCardToHand(field.getDeck().DrawTop());
        }

        if (Input.GetButtonDown("playcard1_test"))
        {
            Debug.Log("playcard1_test pressed");
            playCard(0);
        }
        if (Input.GetButtonDown("playcard2_test"))
        {
            playCard(1);
        }
        if (Input.GetButtonDown("playcard3_test"))
        {
            playCard(2);
        }
    }

    //CmdAddCard

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
        if (hasAuthority)
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
        InitHand();
    }

    private void addCardToHand(int card_id)
    {
        GameObject theCard = GameObject.Find("CardDictionary").GetComponent<CardDictionary>().GetPrefabByID(card_id);
        theCard = Instantiate(theCard, handLocation.position, handLocation.rotation) as GameObject;
        hand.Add(theCard);
        fixHandCardPositions(hand.Count - 1);
    }

    private void playCard(int cardIndex)
    {
        field.AddMonsterCard(hand[cardIndex].GetComponent<MonsterCard>().cardID);

        GameObject cardToDestroy = hand[cardIndex];
        hand.RemoveAt(cardIndex);
        Destroy(cardToDestroy);
        if (hand.Count > cardIndex)
        {
            fixHandCardPositions(cardIndex);
        }
    }

    private void fixHandCardPositions(int startIndex)
    {
        for(int i=startIndex; i<hand.Count; i++)
        {
            Mesh msh = hand[i].GetComponentInChildren<MeshFilter>().mesh;
            float wid = msh.bounds.size.x * hand[i].GetComponentInChildren<MeshFilter>().transform.lossyScale.z;
            Vector3 cardPosition = handLocation.position;
            cardPosition.z += i * wid;
            hand[i].transform.position = cardPosition;
        }
    }

    private void InitHand()
    {
        handLocation = GameObject.Find("_" + (IsFirstPlayer() ? "P1" : "P2") + "_handLocation").transform;
    }
}
