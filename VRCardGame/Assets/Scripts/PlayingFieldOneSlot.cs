using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayingFieldOneSlot : NetworkBehaviour 
{
    [SerializeField]
    private GameObject monsterCard;
    [SyncVar(hook="OnMonsterChanged")]
    public int monsterSync;

    private GameObject monsterPrefab;
    public GameObject redEyes;
    public GameObject darkMag;
    private Transform modelSpawnLocation;
    private Transform friendlyCardSpawnLocation;

    public GameObject dieEffectPrefab;

    private Player player;

	// Use this for initialization
	void Start () 
    {
        if(hasAuthority)
        {
            monsterSync = -1;
        }
	}

    public void InitPlayingField()
    {
        player = GetComponent<Player>();

        monsterPrefab = (player.IsFirstPlayer() ? redEyes : darkMag);

        modelSpawnLocation = GameObject.Find("_" + (player.IsFirstPlayer() ? "P1" : "P2") + "_Monsters").transform;
        friendlyCardSpawnLocation = GameObject.Find("_" + (player.IsFirstPlayer() ? "P1" : "P2") + "_friendlyCards").transform;
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void AddMonsterCard()
    {
        if (isLocalPlayer && monsterCard == null)
        {
            CmdAddMonsterCard();
        }
    }

    public void Attack()
    {
        if(isLocalPlayer && monsterCard != null)
        {
            CmdAttack();
        }
    }

    [Command]
    private void CmdAttack()
    {
        uint opponentNetID = (uint)(player.IsFirstPlayer() ? player.gpManager.player_2_netID : player.gpManager.player_1_netID);

        NetworkIdentity id = null;

        if(NetworkServer.objects.TryGetValue(new NetworkInstanceId(opponentNetID), out id))
        {
            PlayingFieldOneSlot oppenentPlayingField = id.gameObject.GetComponent<PlayingFieldOneSlot>();

            oppenentPlayingField.GetAttacked(monsterCard.GetComponent<MonsterCard>().attack);
        }
    }

    private void GetAttacked(int attack)
    {
        if (attack > monsterCard.GetComponent<MonsterCard>().attack)
        {
            DestroyCard();
        }
    }

    private void DestroyCard()
    {
        if (monsterCard != null)
        {
            if(isClient)
            {
                Instantiate(dieEffectPrefab, monsterCard.GetComponent<ICard>()._3Dmodel.transform.position, Quaternion.identity);
            }
            Destroy(monsterCard.GetComponent<ICard>()._3Dmodel);
            Destroy(monsterCard);

            monsterCard = null;
        }

        if (hasAuthority)
        {
            monsterSync = -1;
        }
    }

    [Command]
    private void CmdAddMonsterCard()
    {
        if (hasAuthority)
        {
            monsterSync = monsterPrefab.GetComponent<ICard>().cardID;

            monsterCard = Instantiate(monsterPrefab, friendlyCardSpawnLocation.position, friendlyCardSpawnLocation.rotation) as GameObject;
            monsterCard.GetComponent<ICard>()._3Dmodel = Instantiate(monsterCard.GetComponent<ICard>()._3Dmodel, modelSpawnLocation.position, modelSpawnLocation.rotation) as GameObject;
        }
    }

    private void OnMonsterChanged(int newVal)
    {
        if (!hasAuthority)
        {
            if (monsterSync == -1 && newVal != -1)
            {
                monsterCard = Instantiate(monsterPrefab, friendlyCardSpawnLocation.position, friendlyCardSpawnLocation.rotation) as GameObject;
                monsterCard.GetComponent<ICard>()._3Dmodel = Instantiate(monsterCard.GetComponent<ICard>()._3Dmodel, modelSpawnLocation.position, modelSpawnLocation.rotation) as GameObject;
            }
            else if (monsterSync != -1 && newVal == -1)
            {
                DestroyCard();
            }
        }

        monsterSync = newVal;
    }
}
