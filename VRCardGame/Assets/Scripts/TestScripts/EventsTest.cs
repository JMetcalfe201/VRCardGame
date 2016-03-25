using UnityEngine;
using System.Collections;

public class EventsTest : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GameplayManager.singleton.EventCardDestroyed += CardDestroyed;
        GameplayManager.singleton.EventCardPlaced += CardPlayed;
        GameplayManager.singleton.EventPlayerDamaged += PlayerHurt;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CardDestroyed(int player, int slotY, int slotX)
    {
        Debug.Log("Player " + player + "'s card in slot " + slotX + " was destroyed.");
    }

    public void CardPlayed(int player, int slotY, int slotX)
    {
        Debug.Log("Player " + player + " played a card in slot " + slotX + ".");
    }

    public void PlayerHurt(int player, int points)
    {
        Debug.Log("Player " + player + " has taken " + points + " points of damage.");
    }
}
