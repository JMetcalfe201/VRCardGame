﻿using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PlayingField : NetworkBehaviour
{
    [SerializeField]
    private GameObject[] monsterCards;

    public SyncListInt monsterSync = new SyncListInt();

    private TransformGrid modelSpawnGrid;
    private Transform friendlyCardSpawnLocation;

    public GameObject dieEffectPrefab;

    private Player player;

    // Use this for initialization
    void Start()
    {
        monsterCards = new GameObject[5];

        if (hasAuthority)
        {
            for(int i = 0; i < 5; i++)
            {
                monsterSync.Add(-1);
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        monsterSync.Callback = OnMonsterChanged;
    }

    public void InitPlayingField()
    {
        player = GetComponent<Player>();

        modelSpawnGrid = GameObject.Find("_" + (player.IsFirstPlayer() ? "P1" : "P2") + "_FieldGrid").GetComponent<TransformGrid>();
        friendlyCardSpawnLocation = GameObject.Find("_" + (player.IsFirstPlayer() ? "P1" : "P2") + "_friendlyCards").transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddMonsterCard(int cardId)
    {
        if (isLocalPlayer && !IsFieldFull())
        {
            CmdAddMonsterCard(cardId);
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
            GetOpposingPlayingField().DestroyCard(opponentIndex);
            GetOpposingPlayingField().player.TakeLifePointsDamage(myAttack - opponentAttack);
        }
        else if(opponentAttack > myAttack)
        {
            DestroyCard(myIndex);
            player.TakeLifePointsDamage(opponentAttack - myAttack);
        }
        else
        {
            DestroyCard(myIndex);
            GetOpposingPlayingField().DestroyCard(opponentIndex);
            Debug.Log("Tie");
        }
    }

    private void DestroyCard(int index)
    {
        if (monsterCards[index] != null)
        {
            if (isClient)
            {
                Instantiate(dieEffectPrefab, monsterCards[index].GetComponent<ICard>()._3Dmodel.transform.position, Quaternion.identity);
            }
            Destroy(monsterCards[index].GetComponent<ICard>()._3Dmodel);
            Destroy(monsterCards[index]);

            monsterCards[index] = null;
        }

        if (hasAuthority)
        {
            monsterSync[index] = -1;
        }
    }

    [Command]
    private void CmdAddMonsterCard(int cardID)
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

            monsterCards[firstEmpty] = Instantiate(CardDictionary.singleton.GetPrefabByID(cardID), friendlyCardSpawnLocation.position, friendlyCardSpawnLocation.rotation) as GameObject;
            monsterCards[firstEmpty].GetComponent<ICard>()._3Dmodel = Instantiate(monsterCards[firstEmpty].GetComponent<ICard>()._3Dmodel, modelSpawnGrid.GetPositionAt(1, firstEmpty), modelSpawnGrid.transform.rotation) as GameObject;
        }
    }

    private void OnMonsterChanged(SyncListInt.Operation op, int index)
    {
        if (!hasAuthority)
        {
            if ( op == SyncList<int>.Operation.OP_SET && monsterSync[index] != -1)
            {
                monsterCards[index] = Instantiate(CardDictionary.singleton.GetPrefabByID(monsterSync[index]), friendlyCardSpawnLocation.position, friendlyCardSpawnLocation.rotation) as GameObject;
                monsterCards[index].GetComponent<ICard>()._3Dmodel = Instantiate(monsterCards[index].GetComponent<ICard>()._3Dmodel, modelSpawnGrid.GetPositionAt(1, index), modelSpawnGrid.transform.rotation) as GameObject;
            }
            else if (op == SyncList<int>.Operation.OP_SET && monsterSync[index] == -1)
            {
                if (isClient)
                {
                    Instantiate(dieEffectPrefab, monsterCards[index].GetComponent<ICard>()._3Dmodel.transform.position, Quaternion.identity);
                }
                Destroy(monsterCards[index].GetComponent<ICard>()._3Dmodel);
                Destroy(monsterCards[index]);
            }
        }
    }

    public bool IsFieldFull()
    {
        for(int i = 0; i < monsterSync.Count; i++)
        {
            if(monsterSync[i] == -1)
            {
                return false;
            }
        }

        return true;
    }

    private PlayingField GetOpposingPlayingField()
    {
        return (player.IsFirstPlayer() ? player.gpManager.p2.GetPlayingField() : player.gpManager.p1.GetPlayingField());
    }
}
