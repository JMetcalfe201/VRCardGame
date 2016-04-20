﻿using UnityEngine;
using System.Collections;

public class magicCylinder : IEffectCard {//a trap card to attack opponent during opponent's battle field with one of opponent's monster

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
					MonsterCard card = owner.GetPlayingField().GetOpposingPlayingField().GetCardByIndex(1, col).GetComponent<MonsterCard>();
					owner.GetPlayingField ().GetOpposingPlayingField ().player.TakeLifePointsDamage(card.attack);
					destroyed = true;
				}
				col++;

			}

			owner.GetPlayingField ().CmdDestroyCard (this);
		}

	}
	public override bool CanActivate()
    {
        return owner.gpManager.GetCurrentPhase() == EGamePhase.BattlePhase;
	}
}
