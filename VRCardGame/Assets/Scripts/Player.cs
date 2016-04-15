using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class Player : NetworkBehaviour
{
    public float mouseLookClampVert = 75f;
    public float mouseLookClampHoriz = 90f;

    public GameObject playerCam;

    [SerializeField]
    CardInfoPane cardInfoPane;

    [SyncVar(hook = "OnLifePointsChanged")]
    public int lifepoints;

    [SyncVar(hook = "firstPlayerUpdated")]
    public int playerNumber;

    public Vector2 cursorPosition;

    public GameplayManager gpManager;

    private PlayingField field;
    public List<GameObject> hand;
    private Transform handLocation;

    public int attackingCard;

    public int selectionIndex;
    private bool vAxisInUse;
    private bool hAxisInUse;

    void Awake()
    {
        playerNumber = -1;
        gpManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
        field = GetComponent<PlayingField>();
        hand = new List<GameObject>();
        vAxisInUse = false;
        hAxisInUse = false;
    }

    void Start()
    {
        if (VRSettings.enabled)
        {
            Cursor.visible = false;
        }

        gpManager.EventPhaseChanged += OnPhaseChanged;

        lifepoints = 8000;

        attackingCard = -1;

        if (!isLocalPlayer)
        {
            playerCam.GetComponent<AudioListener>().enabled = false;
            playerCam.GetComponent<Camera>().tag = "Untagged";
            playerCam.GetComponent<Camera>().enabled = false;

            UpdatePlayerNumInfo();
        }
        else
        {
            CmdInitPlayer();
        }

        cardInfoPane.TurnOff();

        if (playerNumber == 1)
        {
            gpManager.p1 = this;
        }
        else
        {
            gpManager.p2 = this;
        }
        drawStartingHand();
    }

    [Command]
    private void CmdInitPlayer()
    {
        if (hasAuthority)
        {
            if (gpManager.player_1_netID == -1)
            {
                gpManager.player_1_netID = (int)netId.Value;
                playerNumber = 1;
            }
            else
            {
                gpManager.player_2_netID = (int)netId.Value;
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

        if (!VRSettings.enabled)
        {
            float lookSensitivity = 5.0f;

            playerCam.transform.localEulerAngles = playerCam.transform.localEulerAngles + new Vector3(0f, Input.GetAxis("Mouse X") * lookSensitivity, 0f);
            playerCam.transform.localEulerAngles = playerCam.transform.localEulerAngles + new Vector3(-Input.GetAxis("Mouse Y") * lookSensitivity, 0f, 0f);

            /*
            if (transform.eulerAngles.y > mouseLookClampHoriz)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, mouseLookClampHoriz, transform.eulerAngles.z);
            }
            else if (transform.eulerAngles.y < -mouseLookClampHoriz)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, -mouseLookClampHoriz, transform.eulerAngles.z);
            }
            if (transform.eulerAngles.x > mouseLookClampVert)
            {
                transform.eulerAngles = new Vector3(mouseLookClampVert, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            else if (transform.eulerAngles.x < -mouseLookClampVert)
            {
                transform.eulerAngles = new Vector3(-mouseLookClampVert, transform.eulerAngles.y, transform.eulerAngles.z);
            }
             * */
        }

        if (Input.GetButtonDown("Advance Phase"))
        {
            if ((gpManager.isPlayerOnesTurn() && IsFirstPlayer()) || (!gpManager.isPlayerOnesTurn() && !IsFirstPlayer()))
            {
                CmdgpManagerAdvancePhase();
            }

            //////////////////////
            /// Moved this stuff to "OnPhaseChanged" event below. Because of the networking gpManager.GetCurrentPhase() might not be correct right after calling CmdgpManagerAdvancePhase() since it must update over the network. Using an event make sure it isnt called until after the update is done
            /*
            if ((gpManager.isPlayerOnesTurn() && IsFirstPlayer()) || (!gpManager.isPlayerOnesTurn() && !IsFirstPlayer()))
            {
                if (gpManager.GetCurrentPhase() == EGamePhase.DrawPhase)
                {
                    addCardToHand(field.getDeck().DrawTop());
                }
                hand[selectionIndex].GetComponent<ParticleSystem>().enableEmission = false;

                CmdgpManagerAdvancePhase();

                if (gpManager.GetCurrentPhase() == EGamePhase.MainPhase1 || gpManager.GetCurrentPhase() == EGamePhase.MainPhase2)
                {
                    if (hand[selectionIndex].GetComponent<ICard>().cardtype == ECardType.MONSTER_CARD)
                    {
                        cardInfoPane.UpdateFields(hand[selectionIndex].GetComponent<MonsterCard>());
                    }
                    else if (hand[selectionIndex].GetComponent<ICard>().cardtype != ECardType.UNKNOWN)
                    {
                        cardInfoPane.UpdateFields(hand[selectionIndex].GetComponent<IEffectCard>());
                    }
                    cardInfoPane.TurnOn();

                    selectionIndex = 0;
                    if (hand[0] != null)
                    {
                        hand[0].GetComponent<ParticleSystem>().enableEmission = true;
                    }
                    else
                    {
                        // no cards in hand, player loses?
                    }
                }

            }
             */
        }

        if (gpManager.GetCurrentPhase() == EGamePhase.BattlePhase && ((gpManager.isPlayerOnesTurn() && IsFirstPlayer()) || (!gpManager.isPlayerOnesTurn() && !IsFirstPlayer())))
        {
            if (attackingCard == -1)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    attackingCard = 0;
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    attackingCard = 1;
                }

                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    attackingCard = 2;
                }

                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    attackingCard = 3;
                }

                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    attackingCard = 4;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    field.Attack(attackingCard, 0);
                    attackingCard = -1;
                }

                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    field.Attack(attackingCard, 1);
                    attackingCard = -1;
                }

                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    field.Attack(attackingCard, 2);
                    attackingCard = -1;
                }

                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    field.Attack(attackingCard, 3);
                    attackingCard = -1;
                }

                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    field.Attack(attackingCard, 4);
                    attackingCard = -1;
                }
            }
        }
        /*
        if (gpManager.GetCurrentPhase() == EGamePhase.DrawPhase && ((gpManager.isPlayerOnesTurn() && IsFirstPlayer()) || (!gpManager.isPlayerOnesTurn() && !IsFirstPlayer())))
        {
            if (Input.GetButtonDown("drawcard_test"))
            {
                Debug.Log(field);
                Debug.Log(field.getDeck());
                field.getDeck().print();
                addCardToHand(field.getDeck().DrawTop());
                CmdgpManagerAdvancePhase();
            }
        }
        */
        if (isPlayersTurn())
        {
            float v = Input.GetAxisRaw("Menu Vertical");
            float h = Input.GetAxisRaw("Menu Horizontal");

            if ((h > 0.5 || h < -0.5) && !hAxisInUse)
            {
                if (gpManager.GetCurrentPhase() == EGamePhase.MainPhase1 || gpManager.GetCurrentPhase() == EGamePhase.MainPhase2)
                {
                    if (h > 0)
                    {
                        if (hand.Count != 0)
                        {
                            hand[selectionIndex].GetComponent<ParticleSystem>().enableEmission = false;
                            selectionIndex = (selectionIndex + 1) % hand.Count;
                            hand[selectionIndex].GetComponent<ParticleSystem>().enableEmission = true;

                            if(hand[selectionIndex].GetComponent<ICard>().cardtype == ECardType.MONSTER_CARD)
                            {
                                cardInfoPane.UpdateFields(hand[selectionIndex].GetComponent<MonsterCard>());
                            }
                            else if(hand[selectionIndex].GetComponent<ICard>().cardtype != ECardType.UNKNOWN)
                            {
                                cardInfoPane.UpdateFields(hand[selectionIndex].GetComponent<IEffectCard>());
                            }

                        }
                        else
                            Debug.Log("No cards in hand");
                    }
                    else
                    {
                        if (hand.Count != 0)
                        {
                            hand[selectionIndex].GetComponent<ParticleSystem>().enableEmission = false;
                            selectionIndex = (selectionIndex - 1 + hand.Count) % hand.Count;
                            hand[selectionIndex].GetComponent<ParticleSystem>().enableEmission = true;

                            if (hand[selectionIndex].GetComponent<ICard>().cardtype == ECardType.MONSTER_CARD)
                            {
                                cardInfoPane.UpdateFields(hand[selectionIndex].GetComponent<MonsterCard>());
                            }
                            else if (hand[selectionIndex].GetComponent<ICard>().cardtype != ECardType.UNKNOWN)
                            {
                                cardInfoPane.UpdateFields(hand[selectionIndex].GetComponent<IEffectCard>());
                            }
                        }
                        else
                            Debug.Log("No cards in hand");
                    }
                }
                else if (gpManager.GetCurrentPhase() == EGamePhase.BattlePhase)
                {
                    List<int> monsterIndices = field.getMonsterIndices();
                    if (monsterIndices.Count > 0)
                    {
                        if (h > 0)
                        {
                            Debug.Log("There are monsters on field");
                            // do stuff...
                        }
                        else
                        {
                            Debug.Log("There are monsters on field");
                            // do stuff...
                        }
                    }
                    else
                    {
                        Debug.Log("No monsters on field");
                    }
                }

                Debug.Log("Selection Index: " + selectionIndex);
                hAxisInUse = true;
            }
            else if (h < 0.5 && h > -0.5)
            {
                hAxisInUse = false;
            }

            if (Input.GetButtonDown("Select"))
            {
                if (gpManager.GetCurrentPhase() == EGamePhase.MainPhase1 || gpManager.GetCurrentPhase() == EGamePhase.MainPhase2)
                {
                    if (hand.Count != 0)
                        playCard(selectionIndex);
                    else
                        Debug.Log("No cards in hand");

                    CmdgpManagerAdvancePhase();
                }
                /*
                else if (gpManager.GetCurrentPhase() == EGamePhase.BattlePhase)
                {
                    attackingCard = selectionIndex;
                    field.Attack(attackingCard, insert_index_to_attack_here);
                }
                */
            }

            if ((v > 0.5 || v < -0.5) && !vAxisInUse)
            {
                if (v > 0)
                {

                    Debug.Log("Up");
                }
                else
                {

                    Debug.Log("Down");
                }

                vAxisInUse = true;
            }
            else if (v < 0.5 && v > -0.5)
            {
                vAxisInUse = false;
            }
        }

    } // End handleInput()

    private void OnPhaseChanged(int player, EGamePhase phase)
    {
        if (phase == EGamePhase.MainPhase1 && player == playerNumber)
        {
            addCardToHand(field.getDeck().DrawTop());

            StartCoroutine(cardInfoPane.FadeIn());
        }

        if((phase == EGamePhase.MainPhase1 || phase == EGamePhase.MainPhase2) && player == playerNumber)
        {
            selectionIndex = 0;
            if (hand[0] != null)
            {
                hand[0].GetComponent<ParticleSystem>().enableEmission = true;

                if (hand[selectionIndex].GetComponent<ICard>().cardtype == ECardType.MONSTER_CARD)
                {
                    cardInfoPane.UpdateFields(hand[selectionIndex].GetComponent<MonsterCard>());
                }
                else if (hand[selectionIndex].GetComponent<ICard>().cardtype != ECardType.UNKNOWN)
                {
                    cardInfoPane.UpdateFields(hand[selectionIndex].GetComponent<IEffectCard>());
                }
            }
            else
            {
                // no cards in hand, player loses?
            }
        }

        if (phase == EGamePhase.EndPhase && ((player == 1 && IsFirstPlayer()) || (player != 1 && !IsFirstPlayer())))
        {
            StartCoroutine(cardInfoPane.FadeOut());
        }
    }

    //CmdAddCard

    [Command]
    private void CmdgpManagerAdvancePhase()
    {
        gpManager.AdvancePhase();
    }

    public void TakeLifePointsDamage(int points)
    {
        if (hasAuthority)
        {
            lifepoints -= points;

            if (lifepoints <= 0)
            {
                gpManager.CmdGameOver((int)netId.Value);
            }

            gpManager.Cmd_EventPlayerDamaged(playerNumber, points);
        }
    }

    private void OnLifePointsChanged(int lp)
    {
        if(lp < 0)
        {
            lifepoints = 0;
        }
        else
        {
            lifepoints = lp;
        }

        if(playerNumber == 2)
        {
            gpManager.scoreboard.SetBlueLifePoints(lp);
        }
        else
        {
            gpManager.scoreboard.SetRedLifePoits(lp);
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
        transform.eulerAngles = (IsFirstPlayer() ? new Vector3(0, 270f, 0) : new Vector3(0, 90f, 0));

        field.InitPlayingField();
        InitHand();
    }

    private void addCardToHand(int card_id)
    {
        GameObject theCard = GameObject.Find("CardDictionary").GetComponent<CardDictionary>().GetPrefabByID(card_id);
        theCard = Instantiate(theCard, handLocation.position, handLocation.rotation) as GameObject;
        theCard.GetComponent<ICard>().Placed(false);
        theCard.GetComponent<ICard>().Reveal();
        hand.Add(theCard);
        fixHandCardPositions(hand.Count - 1);
    }

    private void playCard(int cardIndex)
    {
        field.AddCard(hand[cardIndex].GetComponent<MonsterCard>().cardID, true);

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
        for (int i = startIndex; i < hand.Count; i++)
        {
            Mesh msh = hand[i].GetComponentInChildren<MeshFilter>().mesh;
            float wid = msh.bounds.size.x * hand[i].GetComponentInChildren<MeshFilter>().transform.lossyScale.z * 11 / 10;
            Vector3 cardPosition = handLocation.position;
            cardPosition.z += i * wid;
            hand[i].transform.position = cardPosition;
        }
    }

    private void InitHand()
    {
        handLocation = GameObject.Find("_" + (IsFirstPlayer() ? "P1" : "P2") + "_handLocation").transform;
    }

    public PlayingField GetPlayingField()
    {
        return field;
    }

    private bool isPlayersTurn()
    {
        return (gpManager.isPlayerOnesTurn() && IsFirstPlayer()) || (!gpManager.isPlayerOnesTurn() && !IsFirstPlayer());
    }

    private void drawStartingHand()
    {
        for (int i = 0; i < 4; i++)
        {
            addCardToHand(field.getDeck().DrawTop());
        }
    }
}