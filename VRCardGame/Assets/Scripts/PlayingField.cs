using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/*
public class PlayingField : NetworkBehaviour
{
    public GameObject[] monsterCards;
    public GameObject[] effectCards;
    private Deck playerDeck;
    private Deck graveyard;
    private Player player;

    public SyncListInt monsterSyncList = new SyncListInt();
    public SyncListInt effectSyncList = new SyncListInt();

    public GameObject tempMonsterCard;
    public GameObject tempEffectCard;

    public GameObject tempFriendlySpawnCard;
    public GameObject tempHostileSpawnCard;
    public GameObject tempFriendlySpawnModel;

    void Awake()
    {
        monsterSyncList.Callback = OnMonsterChanged;
        effectSyncList.Callback = OnEffectChanged;
    }

    void Start()
    {
        monsterCards = new GameObject[5];
        effectCards = new GameObject[5];
        playerDeck = new Deck();
        graveyard = new Deck();

        player = GetComponent<Player>();

        if(player.IsFirstPlayer())
        {
            tempFriendlySpawnCard = GameObject.Find("_P1_friendlyCards");
            tempHostileSpawnCard = GameObject.Find("_P1_hostileCards");
            tempFriendlySpawnModel = GameObject.Find("_P1_Monsters");
        }
        else
        {
            tempFriendlySpawnCard = GameObject.Find("_P2_friendlyCards");
            tempHostileSpawnCard = GameObject.Find("_P2_hostileCards");
            tempFriendlySpawnModel = GameObject.Find("_P2_Monsters");
        }

        if (hasAuthority)
        {
            Debug.Log("Is local player");


            for (int i = 0; i < 5; i++)
            {
                monsterSyncList.Add(-1);
                effectSyncList.Add(-1);
            }
        }

        Debug.LogError(player.netId + " sync list on " + (Application.isEditor ? "editor " : "standalone ") + "contains " + monsterSyncList.Count + " things");
    }

    public Deck getDeck()
    {
        return playerDeck;
    }

    public MonsterCard getMonsterCardByIndex(int i)
    {
        if (i > -1 && i < 5)
        {
            return monsterCards[i].GetComponent<MonsterCard>();
        }
        else
        {
            Debug.Log("Error: index out of range");
            return null;
        }
    }

    public IEffectCard getEffectCardByIndex(int i)
    {
        if (i > -1 && i < 5)
        {
            return effectCards[i].GetComponent<IEffectCard>();
        }
        else
        {
            Debug.Log("Error: index out of range");
            return null;
        }
    }

    public bool setMonsterCardByIndex(GameObject c, int i)
    {
        Debug.Log(player.name + " is placing " + c.GetComponent<ICard>().cardName);

        if (i > -1 && i < 5)
        {
            if (null == monsterCards[i])
            {
                monsterCards[i] = Instantiate(c, tempFriendlySpawnCard.transform.position, tempFriendlySpawnCard.transform.rotation) as GameObject;
                monsterCards[i].GetComponent<MonsterCard>()._3Dmodel = Instantiate(monsterCards[i].GetComponent<MonsterCard>()._3Dmodel, tempFriendlySpawnModel.transform.position, tempFriendlySpawnModel.transform.rotation) as GameObject;

                if(hasAuthority)
                {
                    Debug.LogError(player.name + " is spawning " + c.name + " on " + (Application.isEditor ? "editor" : "standalone"));
                    monsterSyncList[i] = c.GetComponent<MonsterCard>().cardID;
                }

                Debug.Log("Card placed");
                return true;
            }
            else
            {
                Debug.Log("Cannot place card: spot is full");
                return false;
            }
        }
        else
        {
            Debug.Log("Cannot place card: index out of range");
            return false;
        }
    }

    public bool setEffectCardByIndex(GameObject c, int i)
    {
        if (i > -1 && i < 5)
        {
            if (null == effectCards[i])
            {
                effectCards[i] = NetworkManager.Instantiate(c, tempFriendlySpawnCard.transform.position, tempFriendlySpawnCard.transform.rotation) as GameObject;

                if(hasAuthority)
                {
                    effectSyncList[i] = c.GetComponent<IEffectCard>().cardID;
                }

                Debug.Log("Card placed");
                return true;
            }
            else
            {
                Debug.Log("Cannot place card: spot is full");
                return false;
            }
        }
        else
        {
            Debug.Log("Cannot place card: index out of range");
            return false;
        }
    }

    public GameObject removeMonsterCardByIndex(int i)
    {
        Debug.Log("Remove");

        if(i >= 0 && i < 5)
        {
            if(monsterCards[i] != null)
            {
                GameObject temp = monsterCards[i];
                monsterCards[i] = null;

                if(hasAuthority)
                {
                    monsterSyncList[i] = -1;
                }

                return temp;
            }
            else
            {
                Debug.Log("No card to remove at " + i);
                return null;
            }
        }
        else
        {
            Debug.Log("Index out of range");
            return null;
        }
    }

    public GameObject removeEffectCardByIndex(int i)
    {
        if (i >= 0 && i < 5)
        {
            if (effectCards[i] != null)
            {
                GameObject temp = effectCards[i];
                effectCards[i] = null;

                if(hasAuthority)
                {
                    effectSyncList[i] = -1;
                }

                return temp;
            }
            else
            {
                Debug.Log("No card to remove at " + i);
                return null;
            }
        }
        else
        {
            Debug.Log("Index out of range");
            return null;
        }
    }

    // for testing/debugging
    public void print()
    {
        string outputString = "SyncListMonster: ";
        for (int i = 0; i < 5; i++)
        {
            outputString += monsterSyncList[i] + " ";
        }

        outputString += "\nMonster Cards:\t|";
        for (int i = 0; i < 5; i++)
        {
            outputString += (monsterCards[i] != null ? monsterCards[i].GetComponent<ICard>().cardName : "null") + "\t|\t";
        }
        outputString += "\nEffect Cards:\t|";
        for (int i = 0; i < 5; i++)
        {
            outputString += (effectCards[i] != null ? effectCards[i].GetComponent<ICard>().cardName : "null") + "\t|\t";
        }
        Debug.LogError(outputString);
    }

    private void OnMonsterChanged(SyncListInt.Operation op, int index)
    {
        //Debug.LogError("Callback triggered " + player.name);
        if (SyncListInt.Operation.OP_SET == op)
        {
            if (monsterSyncList[index] == -1)
            {
                //removeMonsterCardByIndex(index);
            }
            else
            {
                //MonsterCard card = CardDictionary.GetCardRefFromID(monsterSyncList[index]);
                GameObject card = tempMonsterCard;
                setMonsterCardByIndex(card, index);
            }
        }
    }

    private void OnEffectChanged(SyncListInt.Operation op, int index)
    {
        if (SyncListInt.Operation.OP_SET == op && !hasAuthority)
        {
            if (effectSyncList[index] == -1)
            {
               //removeEffectCardByIndex(index);
            }
            else
            {
                //IEffectCard card = CardDictionary.GetCardRefFromID(monsterSyncList[index]) as IEffectCard;
                //GameObject card = tempEffectCard;
                //setEffectCardByIndex(card, index);
            }
        }
    }
}
*/
public class PlayingField : NetworkBehaviour
{
    public SyncListInt monsterSyncList = new SyncListInt();
    public GameObject tempMonsterCard;
    public Player player;

    void Awake()
    {
        monsterSyncList.Callback = OnMonsterChanged;
    }

    void Start()
    {
        player = GetComponent<Player>();

        if (hasAuthority)
        {
            CmdInitSyncLists();
        }

        Debug.LogError("NetID " + netId + "'s sync list on " + (Application.isEditor ? "editor " : "standalone ") + "contains " + monsterSyncList.Count + " things");
    }

    [Command]
    private void CmdInitSyncLists()
    {
        for (int i = 0; i < 5; i++)
        {
            monsterSyncList.Add(-1);
        }
    }

    public void setMonsterCardByIndex(GameObject card, int i)
    {
        CmdsetMonsterCardByIndex(card, i);
    }

    [Command]
    private void CmdsetMonsterCardByIndex(GameObject card, int i)
    {
        monsterSyncList[i] = card.GetComponent<ICard>().cardID;
    }

    private void OnMonsterChanged(SyncListInt.Operation op, int index)
    {
        if (SyncListInt.Operation.OP_SET == op)
        {
            if (monsterSyncList[index] != -1)
            {
                Debug.LogError("Synced");
            }
        }
    }
}