using UnityEngine;
using System.Collections;

public class MagicJammer : IEffectCard { //trap card discard one card in opponent's hand

	// Use this for initialization
	void Start () {
	
	}
	/*
	// Update is called once per frame
	void Update () {
	
	}
	public override void Placed(bool onField)
	{
		base.Placed(onField);
		if (onField == true) {
			//if opponent's hand is not empty, discard a card from his hand

			//need to complete


			owner.GetPlayingField ().DestroyCard (this);


		}

	}
	public override bool CanActivate()
	{
		int ownerIndex = (owner.IsFirstPlayer() ? 1 : 2);

		// If the other played the card
		if (ownerIndex != player) {
			if (owner.gpManager.GetCurrentPhase() == EGamePhase.BattlePhase) {
				return true;
			}
		}

		return false;

	}
     */
}
