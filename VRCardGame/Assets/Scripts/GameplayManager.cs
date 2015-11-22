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

    public EGamePhase GetPhase()
    {
        return currentPhase;
    }
}
