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

    //public int attackingCard;
    private int attackingCardIndex;
    private int attackeeCardIndex;
    private GameObject attackingCardObject;

    public int selectionIndex;
    private bool vAxisInUse;
    private bool hAxisInUse;
    public List<GameObject> selectionItems;
    private bool showInfoFlag;
    private bool attackingFlag;
    private enum selectedArea { handCards, monsterCards, effectCards };

    void Awake()
    {
        playerNumber = -1;
        gpManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
        field = GetComponent<PlayingField>();
        hand = new List<GameObject>();
        vAxisInUse = false;
        hAxisInUse = false;
        showInfoFlag = true;
        selectionIndex = 0;
        selectionItems = new List<GameObject>();
    }

    void Start()
    {
        if (VRSettings.enabled)
        {
            Cursor.visible = false;
        }

        gpManager.EventPhaseChanged += OnPhaseChanged;

        lifepoints = 8000;

        //attackingCard = -1;
        attackingCardIndex = -1;
        attackingCardObject = null;

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

        loadHandIntoSelectionItems();

        hand[selectionIndex].GetComponent<ParticleSystem>().enableEmission = true;
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
        }

        if (Input.GetButtonDown("Show Info"))
        {
            if (showInfoFlag)
            {
                StartCoroutine(cardInfoPane.FadeIn());
                showInfoFlag = false;
            }
            else
            {
                StartCoroutine(cardInfoPane.FadeOut());
                showInfoFlag = true;
            }
            Debug.Log("Show Info");
        }

        //float v = Input.GetAxisRaw("Menu Vertical");
        float h = Input.GetAxisRaw("Menu Horizontal");

        if (attackingFlag)
        {
            handleAttackInput(h);
        }
        else
        {
            handleHandInput(h);
        }

        /*
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
        */


    } // End handleInput()

    private void handleAttackInput(float h)
    {
        if (playerNumber == 1)
        {
            h *= -1;
        }
        if (Input.GetButtonDown("Back"))
        {
            attackingFlag = false;
            loadMonstersIntoSelectionItems();
        }

        if ((h > 0.5 || h < -0.5) && !hAxisInUse)
        {
            if (selectionItems.Count != 0)
            {
                if (h > 0)
                {
                        selectionItems[selectionIndex].GetComponent<ParticleSystem>().enableEmission = false;
                        selectionIndex = (selectionIndex + 1) % selectionItems.Count;
                        selectionItems[selectionIndex].GetComponent<ParticleSystem>().enableEmission = true;
                }
                else
                {
                        selectionItems[selectionIndex].GetComponent<ParticleSystem>().enableEmission = false;
                        selectionIndex = (selectionIndex - 1) % selectionItems.Count;
                        if (selectionIndex < 0) { selectionIndex += selectionItems.Count; }
                        selectionItems[selectionIndex].GetComponent<ParticleSystem>().enableEmission = true;
                }
                updateCardInfoPane();
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
            GameObject attackingMonster = field.GetCardByIndex(1, attackingCardIndex);
            attackingMonster.GetComponent<MonsterCard>().canAttack = false;
            if (selectionItems.Count != 0)
            {
                attackeeCardIndex = field.GetOpposingPlayingField().getIndexByMonsterCard(selectionItems[selectionIndex]);
                field.Attack(attackingCardIndex, attackeeCardIndex);
            }
            else
            {
                field.GetOpposingPlayingField().player.TakeLifePointsDamage(attackingMonster.GetComponent<MonsterCard>().attack);
            }
            loadMonstersIntoSelectionItems();
            selectionItems[selectionIndex].GetComponent<ParticleSystem>().enableEmission = true;
            attackingFlag = false;
        }
    }

    private void handleHandInput(float h)
    {
        if (playerNumber != 1 || (playerNumber == 1 && (gpManager.GetCurrentPhase() == EGamePhase.BattlePhase)))
            { h *= -1; }
        if ((h > 0.5 || h < -0.5) && !hAxisInUse)
        {
            if (h > 0)
            {
                if (selectionItems.Count != 0)
                {
                    selectionItems[selectionIndex].GetComponent<ParticleSystem>().enableEmission = false;
                    selectionIndex = (selectionIndex + 1) % selectionItems.Count;
                    selectionItems[selectionIndex].GetComponent<ParticleSystem>().enableEmission = true;

                    updateCardInfoPane();

                }
                else
                    Debug.Log("No cards in hand");
            }
            else
            {
                if (selectionItems.Count != 0)
                {
                    selectionItems[selectionIndex].GetComponent<ParticleSystem>().enableEmission = false;
                    selectionIndex = (selectionIndex - 1) % selectionItems.Count;
                    if (selectionIndex < 0) { selectionIndex += selectionItems.Count; }
                    selectionItems[selectionIndex].GetComponent<ParticleSystem>().enableEmission = true;

                    updateCardInfoPane();
                }
                else
                    Debug.Log("No cards in hand");
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
            if ((gpManager.GetCurrentPhase() == EGamePhase.MainPhase1 || gpManager.GetCurrentPhase() == EGamePhase.MainPhase2) && isPlayersTurn())
            {
                if (selectionItems.Count != 0)
                {
                    if (field.getMonsterCards().Count < 5)
                    {
                        playCard(selectionIndex);
                        selectionIndex = 0;
                        Debug.Log("Play Card");
                        CmdgpManagerAdvancePhase();
                    }
                    else
                    {
                        Debug.Log("Can't play card, field is full");
                    }
                }
                else { Debug.Log("No cards in hand"); } 

            }
            else if (gpManager.GetCurrentPhase() == EGamePhase.BattlePhase)
            {
                if (selectionItems.Count > 0)
                {
                    attackingCardObject = selectionItems[selectionIndex];
                    if (attackingCardObject.GetComponent<MonsterCard>().canAttack)
                    {
                        attackingFlag = true; // move on to selecting enemy monster to attack
                        attackingCardIndex = field.getIndexByMonsterCard(attackingCardObject);
                        loadAttackeeMonstersIntoSelectionItems();
                        Debug.Log("Chose Attacking Card " + attackingCardObject.GetComponent<MonsterCard>().cardName);
                    }
                    else
                    {
                        Debug.Log("That monster can't attack yet");
                    }
                }
                else
                {
                    Debug.Log("No monster to select");
                }
            }
        }
    }

    private void OnPhaseChanged(int player, EGamePhase phase)
    {
        if (phase == EGamePhase.MainPhase1 && player == playerNumber)
        {
            addCardToHand(field.getDeck().DrawTop());
            loadHandIntoSelectionItems();
            hand[selectionIndex].GetComponent<ParticleSystem>().enableEmission = true;
            // Removed, press "Show Info" to display info pane now
            //StartCoroutine(cardInfoPane.FadeIn());
        }
        else if (phase == EGamePhase.BattlePhase && player == playerNumber)
        {
            loadMonstersIntoSelectionItems();
            if(selectionItems.Count > 0)
            {
                selectionItems[selectionIndex].GetComponent<ParticleSystem>().enableEmission = true;
            }
        }
        else if (phase == EGamePhase.MainPhase2 && player == playerNumber)
        {
            loadHandIntoSelectionItems();
            hand[selectionIndex].GetComponent<ParticleSystem>().enableEmission = true;
            attackingFlag = false;
        }
        else if(phase == EGamePhase.EndPhase)
        {
            field.setMonstersCanAttack(true);
        }

        // Removed so that card info display may be accessed at any time
        /*
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
        */
    }

    //CmdAddCard

    [Command]
    public void CmdgpManagerAdvancePhase()
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

    public void addCardToHand(int card_id)
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
        selectionItems.RemoveAt(cardIndex);
        Destroy(cardToDestroy);
        if (hand.Count > cardIndex) { fixHandCardPositions(cardIndex); }
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

    private void loadHandIntoSelectionItems()
    {
        emptySelectionItems();
        if (hand.Count == 0) { Debug.Log("No cards in hand"); }
        else
        {
            for(int i=0; i<hand.Count; i++)
            {
                selectionItems.Add(hand[i]);
            }
        }
        selectionIndex = 0;
        updateCardInfoPane();
    }

    private void loadMonstersIntoSelectionItems()
    {
        emptySelectionItems();
        List<GameObject> monsters = field.getMonsterCards();
        {
            for(int i=0; i<monsters.Count; i++)
            {
                selectionItems.Add(monsters[i]);
            }
        }
        selectionIndex = 0;
        updateCardInfoPane();
    }

    private void loadEffectCardsIntoSelectionItems()
    {
        emptySelectionItems();
        List<GameObject> effectCards = field.getEffectCards();
        {
            for (int i = 0; i < effectCards.Count; i++)
            {
                selectionItems.Add(effectCards[i]);
            }
        }
        selectionIndex = 0;
        updateCardInfoPane();
    }

    private void emptySelectionItems()
    {
        while (selectionItems.Count != 0)
        {
            if (selectionItems[0] != null)
            {
                selectionItems[0].GetComponent<ParticleSystem>().enableEmission = false;
            }
            selectionItems.RemoveAt(0);
        }
    }

    private void loadAttackeeMonstersIntoSelectionItems()
    {
        emptySelectionItems();
        PlayingField oppField = field.GetOpposingPlayingField();
        for(int i=0; i<5; i++)
        {
            GameObject monster = oppField.GetCardByIndex(1, i);
            if (monster != null)
            {
                selectionItems.Add(monster);
            }
        }
        selectionIndex = 0;
        updateCardInfoPane();
        if(selectionItems.Count > 0)
        {
            selectionItems[0].GetComponent<ParticleSystem>().enableEmission = true;
        }
    }

    private void updateCardInfoPane()
    {
        if (selectionItems[selectionIndex].GetComponent<ICard>().cardtype == ECardType.MONSTER_CARD)
        {
            cardInfoPane.UpdateFields(selectionItems[selectionIndex].GetComponent<MonsterCard>());
        }
        else if (selectionItems[selectionIndex].GetComponent<ICard>().cardtype != ECardType.UNKNOWN)
        {
            cardInfoPane.UpdateFields(selectionItems[selectionIndex].GetComponent<IEffectCard>());
        }
    }
}