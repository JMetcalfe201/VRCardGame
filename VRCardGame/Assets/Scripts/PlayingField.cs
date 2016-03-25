using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PlayingField : NetworkBehaviour
{
    [SerializeField]
    private GameObject[] monsterCards;
    [SerializeField]
    private GameObject[] effectCards;

    public SyncListInt monsterSync = new SyncListInt();
    public SyncListInt effectSync = new SyncListInt();

    private TransformGrid modelSpawnGrid;
    private TransformGrid friendlyCardSpawnLocation;

    public GameObject dieEffectPrefab;

    private Player player;

    private Deck playerDeck;
    private Deck graveyard;

    // Use this for initialization
    void Start()
    {
        monsterCards = new GameObject[5];
        effectCards = new GameObject[5];

        playerDeck = new Deck();
        graveyard = new Deck();

        // add some sort of load deck contents by integer
        for (int i = 0; i < 15; i++)
        {
            playerDeck.addCardTop(Random.Range(0, 4));
        }
        playerDeck.Shuffle();

        if (hasAuthority)
        {
            for (int i = 0; i < 5; i++)
            {
                monsterSync.Add(-1);
                effectSync.Add(-1);
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        monsterSync.Callback = OnMonsterChanged;
        effectSync.Callback = OnEffectChanged;
    }

    public void InitPlayingField()
    {
        player = GetComponent<Player>();

        modelSpawnGrid = GameObject.Find("_" + (player.IsFirstPlayer() ? "P1" : "P2") + "_FieldGrid").GetComponent<TransformGrid>();

        if (isLocalPlayer)
        {
            friendlyCardSpawnLocation = GameObject.Find("_" + (player.IsFirstPlayer() ? "P1" : "P2") + "_friendlyCardsGrid").GetComponent<TransformGrid>();
        }
        else
        {
            friendlyCardSpawnLocation = GameObject.Find("_" + (player.IsFirstPlayer() ? "P2" : "P1") + "_enemyCardsGrid").GetComponent<TransformGrid>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCard(int cardId, bool revealed)
    {
        if(CardDictionary.singleton.GetCardType(cardId) == ECardType.MONSTER_CARD)
        {
            Debug.LogError("AddingCard: " + cardId + " Revealed: " + revealed);
            AddMonsterCard(cardId, revealed);
        }
        else if(CardDictionary.singleton.GetCardType(cardId) != ECardType.UNKNOWN)
        {
            AddEffectCard(cardId, revealed);
        }
        else
        {
            Debug.LogError("Cannot play a card of type ECardType.UNKOWN");
        }
    }

    private void AddMonsterCard(int cardId, bool revealed)
    {
        if (isLocalPlayer && !IsFieldFull(ECardType.MONSTER_CARD))
        {
            Debug.LogError("AddingCard: " + cardId + " Revealed: " + revealed);
            CmdAddMonsterCard(cardId, revealed);
        }
    }

    private void AddEffectCard(int cardId, bool revealed)
    {
        if(isLocalPlayer && !IsFieldFull(ECardType.MAGIC_CARD))
        {
            CmdAddEffectCard(cardId, revealed);
        }
    }

    public void Attack(int myIndex, int opponentIndex)
    {
        if (isLocalPlayer)
        {
            if (monsterCards[myIndex] != null && GetOpposingPlayingField().monsterCards[opponentIndex] != null)
            {
                CmdAttack(myIndex, opponentIndex);
            }
            else
            {
                Debug.Log("One of the slots is empty.");
            }
        }
    }

    [Command]
    private void CmdAttack(int myIndex, int opponentIndex)
    {
        int myAttack = monsterCards[myIndex].GetComponent<MonsterCard>().attack;
        int opponentAttack = GetOpposingPlayingField().monsterCards[opponentIndex].GetComponent<MonsterCard>().attack;

        if(myAttack > opponentAttack)
        {
            GetOpposingPlayingField().DestroyMonsterCard(opponentIndex);
            GetOpposingPlayingField().player.TakeLifePointsDamage(myAttack - opponentAttack);
        }
        else if(opponentAttack > myAttack)
        {
            DestroyMonsterCard(myIndex);
            player.TakeLifePointsDamage(opponentAttack - myAttack);
        }
        else
        {
            DestroyMonsterCard(myIndex);
            GetOpposingPlayingField().DestroyMonsterCard(opponentIndex);
        }
    }

    private void DestroyMonsterCard(int index)
    {
        if (monsterCards[index] != null)
        {
            if (isClient)
            {
                Instantiate(dieEffectPrefab, monsterCards[index].GetComponent<ICard>()._3DmonsterModel.transform.position, Quaternion.identity);
            }
            Destroy(monsterCards[index].GetComponent<ICard>()._3DmonsterModel);
            Destroy(monsterCards[index]);

            monsterCards[index] = null;
           
            player.gpManager.Cmd_EventCardDestroyed(player.playerNumber, 1, index);
        }

        if (hasAuthority)
        {
            CmdSendCardToGraveyard(monsterSync[index]);
            monsterSync[index] = -1;
        }
    }

    private void DestroyEffectCard(int index)
    {
        if (effectCards[index] != null)
        {
            if (isClient)
            {
                Instantiate(dieEffectPrefab, effectCards[index].GetComponent<ICard>()._3DmonsterModel.transform.position, Quaternion.identity);
            }
            Destroy(effectCards[index].GetComponent<ICard>()._3DmonsterModel);
            Destroy(effectCards[index]);

            effectCards[index] = null;

            player.gpManager.Cmd_EventCardDestroyed(player.playerNumber, 0, index);
        }

        if (hasAuthority)
        {
            CmdSendCardToGraveyard(effectSync[index]);
            effectSync[index] = -1;
        }
    }

    [Command]
    private void CmdSendCardToGraveyard(int id)
    {
        // Add card to graveyard
        graveyard.addCardTop(id);

        // Replicate to clients
        RpcSendCardToGraveyard(id);
    }

    [ClientRpc]
    private void RpcSendCardToGraveyard(int id)
    {
        graveyard.addCardTop(id);
    }

    [Command]
    private void CmdAddMonsterCard(int cardID, bool attackMode)
    {
        if (hasAuthority)
        {
            int firstEmpty = 0;
            for (; firstEmpty < monsterSync.Count; firstEmpty++)
            {
                if(monsterSync[firstEmpty] == -1)
                {
                    break;
                }
            }

            if(firstEmpty >= monsterSync.Count)
            {
                return;
            }

            monsterSync[firstEmpty] = cardID;

            monsterCards[firstEmpty] = Instantiate(CardDictionary.singleton.GetPrefabByID(cardID), friendlyCardSpawnLocation.GetPositionAt(1, firstEmpty), friendlyCardSpawnLocation.transform.rotation) as GameObject;
            MonsterCard card = monsterCards[firstEmpty].GetComponent<MonsterCard>();
            card.Placed(true);
            card.SetMonsterModelTransform(modelSpawnGrid.GetPositionAt(1, firstEmpty), modelSpawnGrid.transform.rotation.eulerAngles);

            if(attackMode)
            {
                card.Reveal();
            }
            else
            {
                card.SetDefenseMode();
            }

            player.gpManager.Cmd_EventCardPlaced(player.playerNumber, 1, firstEmpty);
        }
    }

    [Command]
    private void CmdAddEffectCard(int cardID, bool activate)
    {
        if (hasAuthority)
        {
            int firstEmpty = 0;
            for (; firstEmpty < effectSync.Count; firstEmpty++)
            {
                if (effectSync[firstEmpty] == -1)
                {
                    break;
                }
            }

            if (firstEmpty >= effectSync.Count)
            {
                return;
            }

            effectSync[firstEmpty] = cardID;

            effectCards[firstEmpty] = Instantiate(CardDictionary.singleton.GetPrefabByID(cardID), friendlyCardSpawnLocation.GetPositionAt(0, firstEmpty), friendlyCardSpawnLocation.transform.rotation) as GameObject;
            ICard card = effectCards[firstEmpty].GetComponent<ICard>();
            card._3DmonsterModel = Instantiate(effectCards[firstEmpty].GetComponent<ICard>()._3DmonsterModel, modelSpawnGrid.GetPositionAt(0, firstEmpty), modelSpawnGrid.transform.rotation) as GameObject;
            
            if(activate)
            {
                card.Reveal();
                
                // Activate Card's effect
            }

            player.gpManager.Cmd_EventCardPlaced(player.playerNumber, 0, firstEmpty);
        }
    }

    private void OnMonsterChanged(SyncListInt.Operation op, int index)
    {
        if (!hasAuthority)
        {
            if ( op == SyncList<int>.Operation.OP_SET && monsterSync[index] != -1)
            {
                monsterCards[index] = Instantiate(CardDictionary.singleton.GetPrefabByID(monsterSync[index]), friendlyCardSpawnLocation.GetPositionAt(1, index), friendlyCardSpawnLocation.transform.rotation) as GameObject;
                ICard card = monsterCards[index].GetComponent<ICard>();
                card.Placed(true);
                card.SetMonsterModelTransform(modelSpawnGrid.GetPositionAt(1, index), modelSpawnGrid.transform.rotation.eulerAngles);
                card.Reveal();
            }
            else if (op == SyncList<int>.Operation.OP_SET && monsterSync[index] == -1)
            {
                if (isClient)
                {
                    Instantiate(dieEffectPrefab, monsterCards[index].GetComponent<ICard>()._3DmonsterModel.transform.position, Quaternion.identity);
                }
                Destroy(monsterCards[index].GetComponent<ICard>()._3DmonsterModel);
                Destroy(monsterCards[index]);
            }
        }
    }

    private void OnEffectChanged(SyncListInt.Operation op, int index)
    {
        if (!hasAuthority)
        {
            if (op == SyncList<int>.Operation.OP_SET && effectSync[index] != -1)
            {
                effectCards[index] = Instantiate(CardDictionary.singleton.GetPrefabByID(effectSync[index]), friendlyCardSpawnLocation.GetPositionAt(0, index), friendlyCardSpawnLocation.transform.rotation) as GameObject;
                ICard card = effectCards[index].GetComponent<ICard>();
                card.Placed(true);
                card.SetMonsterModelTransform(modelSpawnGrid.GetPositionAt(1, index), modelSpawnGrid.transform.rotation.eulerAngles);
            }
            else if (op == SyncList<int>.Operation.OP_SET && effectSync[index] == -1)
            {
                if (isClient)
                {
                    Instantiate(dieEffectPrefab, effectCards[index].GetComponent<ICard>()._3DmonsterModel.transform.position, Quaternion.identity);
                }
                Destroy(effectCards[index].GetComponent<ICard>()._3DmonsterModel);
                Destroy(effectCards[index]);
            }
        }
    }

    public bool IsFieldFull(ECardType type)
    {
        if (type == ECardType.MONSTER_CARD)
        {
            for (int i = 0; i < monsterSync.Count; i++)
            {
                if (monsterSync[i] == -1)
                {
                    return false;
                }
            }
        }
        else if(type != ECardType.UNKNOWN)
        {
            for (int i = 0; i < effectSync.Count; i++)
            {
                if (effectSync[i] == -1)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private PlayingField GetOpposingPlayingField()
    {
        return (player.IsFirstPlayer() ? player.gpManager.p2.GetPlayingField() : player.gpManager.p1.GetPlayingField());
    }

    public Deck getDeck()
    {
        return playerDeck;
    }
}
