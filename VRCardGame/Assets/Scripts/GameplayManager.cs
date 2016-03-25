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
    public delegate void FieldEventDelegate(int player, int cardIndexRow, int cardIndexCol);
    public delegate void PlayerEventDelegate(int player, int lifepointDamage);

    public event FieldEventDelegate EventCardPlaced;
    public event FieldEventDelegate EventCardDestroyed;
    public event FieldEventDelegate EventCardRevealed;
    public event PlayerEventDelegate EventPlayerDamaged;

    void Awake()
    {
        player_1_netID = -1;
        player_2_netID = -1;

        if (singleton != null)
        {
            Destroy(gameObject);
        }
        else
        {
            singleton = this;
        }
    }

    // Use this for initialization
    void Start()
    {
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
    public void Cmd_EventCardPlaced(int player, int cardIndexRow, int cardIndexCol)
    {
        Rpc_EventCardPlaced(player, cardIndexRow, cardIndexCol);
    }
    
    [Command]
    public void Cmd_EventCardDestroyed(int player, int cardIndexRow, int cardIndexCol)
    {
        Rpc_EventCardDestroyed(player, cardIndexRow, cardIndexCol);
    }

    [Command]
    public void Cmd_EventCardRevealed(int player, int cardIndexRow, int cardIndexCol)
    {
        Rpc_EventCardRevealed(player, cardIndexRow, cardIndexCol);
    }

    [Command]
    public void Cmd_EventPlayerDamaged(int player, int lifepointDamage)
    {
        Rpc_EventPlayerDamaged(player, lifepointDamage);
    }

    [ClientRpc]
    private void Rpc_EventCardPlaced(int player, int cardIndexRow, int cardIndexCol)
    {
        EventCardPlaced(player, cardIndexRow, cardIndexCol);
    }

    [ClientRpc]
    private void Rpc_EventCardDestroyed(int player, int cardIndexRow, int cardIndexCol)
    {
        EventCardDestroyed(player, cardIndexRow, cardIndexCol);
    }

    [ClientRpc]
    private void Rpc_EventCardRevealed(int player, int cardIndexRow, int cardIndexCol)
    {
        EventCardRevealed(player, cardIndexRow, cardIndexCol);
    }

    [ClientRpc]
    private void Rpc_EventPlayerDamaged(int player, int lifepointDamage)
    {
        EventPlayerDamaged(player, lifepointDamage);
    }
}
