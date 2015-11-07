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
    public EGamePhase currentPhase;

    [SyncVar(hook = "OnFirstPlayersTurnChange")]
    private bool firstPlayersTurn;

    public Text phaseText;

    // Use this for initialization
    void Start()
    {
        currentPhase = EGamePhase.DrawPhase;
        firstPlayersTurn = true;

        phaseText.text = "It is player " + (firstPlayersTurn ? 1 : 2) + "'s turn. Phase: " + currentPhase.ToString();

        NetworkManager.singleton.GetComponent<NetworkManagerHUD>().showGUI = false;
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
        UpdateUI();
    }

    private void OnFirstPlayersTurnChange(bool newTurn)
    {
        firstPlayersTurn = newTurn;
        UpdateUI();
    }

    private void UpdateUI()
    {
        phaseText.text = "It is player " + (firstPlayersTurn ? 1 : 2) + "'s turn. Phase: " + currentPhase.ToString();
    }

    public EGamePhase GetCurrentPhase()
    {
        return currentPhase;
    }

    public bool isPlayerOnesTurn()
    {
        return firstPlayersTurn;
    }
}
