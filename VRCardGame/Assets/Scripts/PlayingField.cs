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

    private TransformGrid modelSpawnGrid;
    private TransformGrid friendlyCardSpawnLocation;

    public GameObject dieEffectPrefab;

    public Player player;

    private Deck playerDeck;
    private Deck graveyard;

    // Use this for initialization
    void Awake()
    {
        monsterCards = new GameObject[5];
        effectCards = new GameObject[monsterCards.Length];

        for (int i = 0; i < monsterCards.Length; i++)
        {
            monsterCards[i] = null;
            effectCards[i] = null;
        }

        playerDeck = new Deck();
        graveyard = new Deck();

        // add some sort of load deck contents by integer
        for (int i = 0; i < 15; i++)
        {
            playerDeck.addCardTop(Random.Range(0, CardDictionary.singleton.cardList.Count));
        }
        playerDeck.Shuffle();
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

    public void CmdDestroyCard(ICard card)
    {
        if(card.cardtype == ECardType.MONSTER_CARD)
        {
            for(int i = 0; i < monsterCards.Length; i++)
            {
                if(card == monsterCards[i])
                {
                    DestroyMonsterCard(i);
                    return;
                }
            }
        }
        else if(card.cardtype != ECardType.UNKNOWN)
        {
            for(int i = 0; i < effectCards.Length; i++)
            {
                if(card == effectCards[i])
                {
                    DestroyEffectCard(i);
                    return;
                }
            }
        }
    }

    public bool HasMonsterCards()
    {
        for(int i = 0; i < monsterCards.Length; i++)
        {
            if(monsterCards[i] != null)
            {
                return true;
            }
        }

        return false;
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

    [Server]
    private void DestroyMonsterCard(int index)
    {
        if (monsterCards[index] != null)
        {
            if (isClient)
            {
                Instantiate(dieEffectPrefab, monsterCards[index].GetComponent<ICard>()._3DmonsterModel.transform.position, Quaternion.identity);
            }

            int cardID = monsterCards[index].GetComponent<ICard>().cardID;

            Destroy(monsterCards[index].GetComponent<ICard>()._3DmonsterModel);
            Destroy(monsterCards[index]);

            monsterCards[index] = null;
           
            player.gpManager.Cmd_EventCardDestroyed(player.playerNumber, 1, index);

            if (hasAuthority)
            {
                RpcDestroyMonsterCard(index);
                CmdSendCardToGraveyard(cardID);
            }
        }
    }

    [ClientRpc]
    private void RpcDestroyMonsterCard(int index)
    {
        if (monsterCards[index] != null)
        {
            Destroy(monsterCards[index].GetComponent<ICard>()._3DmonsterModel);
            Destroy(monsterCards[index]);

            monsterCards[index] = null;
        }
        else { Debug.Log("Tried to destroy null card???"); }
    }

    [Server]
    private void DestroyEffectCard(int index)
    {
        if (effectCards[index] != null)
        {
            if (isClient)
            {
                Instantiate(dieEffectPrefab, effectCards[index].GetComponent<ICard>()._3DmonsterModel.transform.position, Quaternion.identity);
            }

            int cardID = effectCards[index].GetComponent<ICard>().cardID;

            Destroy(effectCards[index].GetComponent<ICard>()._3DmonsterModel);
            Destroy(effectCards[index]);

            effectCards[index] = null;

            player.gpManager.Cmd_EventCardDestroyed(player.playerNumber, 0, index);

            if (hasAuthority)
            {
                RpcEffectMonsterCard(index);
                CmdSendCardToGraveyard(cardID);
            }
        }
    }

    [ClientRpc]
    private void RpcEffectMonsterCard(int index)
    {
        Destroy(effectCards[index].GetComponent<ICard>()._3DmonsterModel);
        Destroy(effectCards[index]);

        effectCards[index] = null;
    }

    [Command]
    public void CmdForceDestroyMonsterCard(int index)
    {
        DestroyMonsterCard(index);
    }

    [Command]
    public void CmdForceDestroyEffectCard(int index)
    {
        DestroyEffectCard(index);
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

    public void AddCard(int cardId, bool revealed)
    {
        if (CardDictionary.singleton.GetCardType(cardId) == ECardType.MONSTER_CARD)
        {
            Debug.LogError("AddingCard: " + cardId + " Revealed: " + revealed);
            AddMonsterCard(cardId, revealed);
        }
        else if (CardDictionary.singleton.GetCardType(cardId) != ECardType.UNKNOWN)
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
        if (isLocalPlayer && !IsFieldFull(ECardType.MAGIC_CARD))
        {
            CmdAddEffectCard(cardId, revealed);
        }
    }

    [Command]
    private void CmdAddMonsterCard(int cardID, bool attackMode)
    {
        if (hasAuthority)
        {
            int firstEmpty = 0;
            for (; firstEmpty < monsterCards.Length; firstEmpty++)
            {
                if(monsterCards[firstEmpty] == null)
                {
                    break;
                }
            }

            if(firstEmpty >= monsterCards.Length)
            {
                Debug.Log("error: tried to add monster card to a full field");
                return;
            }

            /*
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
             * */
            
            RpcAddMonsterCard(cardID, attackMode, firstEmpty);

            player.gpManager.Cmd_EventCardPlaced(player.playerNumber, 1, firstEmpty);
        }
    }
    
    [ClientRpc]
    private void RpcAddMonsterCard(int cardID, bool attackMode, int index)
    {
        monsterCards[index] = Instantiate(CardDictionary.singleton.GetPrefabByID(cardID), friendlyCardSpawnLocation.GetPositionAt(1, index), friendlyCardSpawnLocation.transform.rotation) as GameObject;
        MonsterCard card = monsterCards[index].GetComponent<MonsterCard>();
        card.owner = player;
        card.Placed(true);
        card.SetMonsterModelTransform(modelSpawnGrid.GetPositionAt(1, index), modelSpawnGrid.transform.rotation.eulerAngles);

        if (attackMode)
        {
            card.Reveal();
            card.SetAttackMode();
        }
    }

    [Command]
    private void CmdAddEffectCard(int cardID, bool activate)
    {
        if (hasAuthority)
        {
            int firstEmpty = 0;
            for (; firstEmpty < effectCards.Length; firstEmpty++)
            {
                if (effectCards[firstEmpty] == null)
                {
                    break;
                }
            }

            if (firstEmpty >= effectCards.Length)
            {
                Debug.Log("error: tried to add effect card to full field");
                return;
            }

            /*
            effectCards[firstEmpty] = Instantiate(CardDictionary.singleton.GetPrefabByID(cardID), friendlyCardSpawnLocation.GetPositionAt(0, firstEmpty), friendlyCardSpawnLocation.transform.rotation) as GameObject;
            ICard card = effectCards[firstEmpty].GetComponent<ICard>();
            card._3DmonsterModel = Instantiate(effectCards[firstEmpty].GetComponent<ICard>()._3DmonsterModel, modelSpawnGrid.GetPositionAt(0, firstEmpty), modelSpawnGrid.transform.rotation) as GameObject;
            
            if(activate)
            {
                card.Reveal();
                
                // Activate Card's effect
            }
             * */

            RpcAddEffectCard(cardID, activate, firstEmpty);

            player.gpManager.Cmd_EventCardPlaced(player.playerNumber, 0, firstEmpty);
        }
    }

    [ClientRpc]
    private void RpcAddEffectCard(int cardID, bool activate, int index)
    {
        effectCards[index] = Instantiate(CardDictionary.singleton.GetPrefabByID(cardID), friendlyCardSpawnLocation.GetPositionAt(0, index), friendlyCardSpawnLocation.transform.rotation) as GameObject;
        ICard card = effectCards[index].GetComponent<ICard>();
        card._3DmonsterModel = Instantiate(effectCards[index].GetComponent<ICard>()._3DmonsterModel, modelSpawnGrid.GetPositionAt(0, index), modelSpawnGrid.transform.rotation) as GameObject;
        card.owner = player;

        if (activate)
        {
            card.Reveal();

            // Activate Card's effect
        }
    }

    public bool IsFieldFull(ECardType type)
    {
        if (type == ECardType.MONSTER_CARD)
        {
            for (int i = 0; i < monsterCards.Length; i++)
            {
                if (monsterCards[i] == null)
                {
                    return false;
                }
            }
        }
        else if(type != ECardType.UNKNOWN)
        {
            for (int i = 0; i < effectCards.Length; i++)
            {
                if (effectCards[i] == null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public int GetCardIDByIndex(int row, int col)
    {
        if(row == 0)
        {
            return effectCards[col].GetComponent<ICard>().cardID;
        }
        else
        {
            return monsterCards[col].GetComponent<ICard>().cardID;
        }
    }

    public GameObject GetCardByIndex(int row, int col)
    {
        if(row == 0)
        {
            return effectCards[col];
        }
        else
        {
            return monsterCards[col];
        }
    }

    public PlayingField GetOpposingPlayingField()
    {
        return (player.IsFirstPlayer() ? player.gpManager.p2.GetPlayingField() : player.gpManager.p1.GetPlayingField());
    }

    public Deck getDeck()
    {
        return playerDeck;
    }

    
    public List<GameObject> getMonsterCards()
    {
        List<GameObject> monsters = new List<GameObject>();
        for (int i = 0; i < 5; i++)
        {
            if(monsterCards[i] != null)
            {
                monsters.Add(monsterCards[i]);
            }
        }
        return monsters;
    }
    /*
    public List<int> getMonsterIndices()
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            if (monsterCards[i] != null)
            {
                indices.Add(i);
            }
        }
        return indices;
    }
    */
    public int getIndexByMonsterCard(GameObject monster)
    {
        for (int i = 0; i < 5; i++)
        {
            if (monsterCards[i] == monster)
            {
                return i;
            }
        }
        return -1;
    }

    public void setMonstersCanAttack(bool boolParam)
    {
        for(int i=0; i<5; i++)
        {
            if (monsterCards[i])
            {
                monsterCards[i].GetComponent<MonsterCard>().canAttack = boolParam;
            }
        }
    }

    public Deck GetGraveYard()
    {
        return graveyard;
    }
}
