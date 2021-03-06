﻿
using UnityEngine;
using System.Collections;

public class MirrorForce : IEffectCard {//a trap card destroy a monster card from opponent's playing field when the opponent is in their battle field

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

			bool destroyed=false;
			int col=0;
			while(destroyed==false&&col<5){
				if (owner.GetPlayingField ().GetOpposingPlayingField ().GetCardByIndex (1, col) != null) {
					owner.GetPlayingField().GetOpposingPlayingField().CmdForceDestroyMonsterCard(col);
					destroyed = true;
				}
				col++;

			}

			owner.GetPlayingField ().CmdDestroyCard (this);
		}

	}
	public override bool CanActivate()
	{
        // Check if the other player has a monster card to destroy
        return owner.GetPlayingField().GetOpposingPlayingField().HasMonsterCards() && owner.gpManager.GetCurrentPhase() == EGamePhase.BattlePhase;
	}
}
