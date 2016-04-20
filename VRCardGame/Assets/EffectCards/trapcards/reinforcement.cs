using UnityEngine;
using System.Collections;

public class reinforcement : IEffectCard {//a trap card adding 500 atk to a monster in the playing field and doing an attack from an opponent's monster to that monster

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
			while (col < 5) {
				if (owner.GetPlayingField ().GetOpposingPlayingField ().GetCardByIndex (1, col) != null) {
					int ownerCol = 0;
					
					while (ownerCol < 5) {
						
						if (owner.GetPlayingField ().GetCardByIndex (1, ownerCol) != null) {
							MonsterCard mycard = owner.GetPlayingField ().GetCardByIndex (1, ownerCol).GetComponent<MonsterCard> ();
							MonsterCard opcard = owner.GetPlayingField ().GetOpposingPlayingField ().GetCardByIndex (1, col).GetComponent<MonsterCard> ();
							int difference = mycard.attack +500 - opcard.attack;
							if (difference > 0) {
								owner.GetPlayingField ().GetOpposingPlayingField ().CmdForceDestroyMonsterCard (col);
								owner.GetPlayingField ().GetOpposingPlayingField ().player.TakeLifePointsDamage (difference);
							} else {
								if (difference == 0) {
									owner.GetPlayingField ().GetOpposingPlayingField ().CmdForceDestroyMonsterCard (col);
									owner.GetPlayingField ().CmdForceDestroyMonsterCard (ownerCol);

								} else {
									owner.GetPlayingField ().CmdForceDestroyMonsterCard (ownerCol);
									owner.TakeLifePointsDamage (0 - difference);
								}
							}
							
							destroyed = true;
							ownerCol += 5;
							col += 5;
						} else {
							ownerCol++;
						}
					}


				}
				col++;
			}
			if (destroyed == true) {
				owner.GetPlayingField ().CmdDestroyCard (this);
			}

		}

	}
	public override bool CanActivate()
	{
        return owner.gpManager.GetCurrentPhase() == EGamePhase.BattlePhase;
	}
}
