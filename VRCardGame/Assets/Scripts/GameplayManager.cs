using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public enum EGamePhase
{
    DrawPhase,
    MainPhase1,
    BattlePhase,
    MainPhase2,
    EndPhase,
}

public class GameplayManager : NetworkBehaviour
{
    [SyncVar(hook = "OnCurrentPhaseChange")]
    private EGamePhase currentPhase;

    [SyncVar(hook = "OnFirstPlayersTurnChange")]
    private bool firstPlayersTurn;

    [SyncVar]
    public int player_1_netID;
    [SyncVar]
    public int player_2_netID;

    public Player p1;
    public Player p2;

    public static GameplayManager singleton = null;

    // Events
    public delegate void FieldEventDelegate(int player, int cardIndex);
    public delegate void PlayerEventDelegate(int player, int lifepointDamage);

    [SyncEvent]
    public event FieldEventDelegate EventCardPlaced;
    [SyncEvent]
    public event FieldEventDelegate EventCardDestroyed;
    [SyncEvent]
    public event FieldEventDelegate EventCardRevealed;
    [SyncEvent]
    public event PlayerEventDelegate EventPlayerDamaged;

    void Awake()
    {
        player_1_netID = -1;
        player_2_netID = -1;
    }

    // Use this for initialization
    void Start()
    {
        if(singleton != null)
        {
            Destroy(gameObject);
        }
        else
        {
            singleton = this;
        }

        currentPhase = EGamePhase.DrawPhase;
        firstPlayersTurn = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void AdvancePhase()
    {
        if(hasAuthority)
        {
            if(currentPhase < EGamePhase.EndPhase)
            {
                currentPhase++;
            }
            else
            {
                firstPlayersTurn = !firstPlayersTurn;
                currentPhase = EGamePhase.DrawPhase;
            }
        }
    }

    // Handler for phase change
    private void OnCurrentPhaseChange(EGamePhase newPhase)
    {
        currentPhase = newPhase;

        foreach(Player p in GameObject.FindObjectsOfType<Player>())
        {
            if(p.isLocalPlayer)
            {
                p.UpdateUI();
            }
        }
    }

    public void GameOver(Player winner)
    {
        Debug.LogError(winner.playerText.text + " is the winner!");
    }

    private void OnFirstPlayersTurnChange(bool newTurn)
    {
        firstPlayersTurn = newTurn;

        foreach (Player p in GameObject.FindObjectsOfType<Player>())
        {
            if (p.isLocalPlayer)
            {
                p.UpdateUI();
            }
        }
    }

    public EGamePhase GetCurrentPhase()
    {
        return currentPhase;
    }

    public bool isPlayerOnesTurn()
    {
        return firstPlayersTurn;
    }

    public bool GetFirstPlayerTurn()
    {
        return firstPlayersTurn;
    }

    //Event Callers: these are commands so that the events will be called on the server instance of this script. Since these are "SyncEvents" they will automatically be called on all other instances of this script.
    [Command]
    public void CmdEventCardPlaced(int player, int cardIndex)
    {
        EventCardPlaced(player, cardIndex);
    }
    
    [Command]
    public void CmdEventCardDestroyed(int player, int cardIndex)
    {
        EventCardDestroyed(player, cardIndex);
    }

    [Command]
    public void CmdEventCardRevealed(int player, int cardIndex)
    {
        EventCardRevealed(player, cardIndex);
    }

    [Command]
    public void CmdEventPlayerDamaged(int player, int lifepointDamage)
    {
        EventPlayerDamaged(player, lifepointDamage);
    }
}
