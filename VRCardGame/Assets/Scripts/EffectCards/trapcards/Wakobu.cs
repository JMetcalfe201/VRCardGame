using UnityEngine;
using System.Collections;

public class Wakobu : IEffectCard {//a trap card to prevent opponent attack for a turn

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public override void Placed(bool onField)
	{
		base.Placed(onField);
		if (onField == true) {

			// Binds the function CheckCardPlaced to the event EventCardPlaced
			owner.CmdgpManagerAdvancePhase ();
			owner.GetPlayingField ().DestroyCard (this);
		}

	}
	public override bool CanActivate()
	{
		int ownerIndex = (owner.IsFirstPlayer() ? 1 : 2);

		// If the other played the card
		if (ownerIndex != player) {
			if (owner.gpManager.GetCurrentPhase == EGamePhase.BattlePhase) {
				return true;
			}
		}

		return false;

	}
}
