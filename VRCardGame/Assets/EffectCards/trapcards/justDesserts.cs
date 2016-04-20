using UnityEngine;
using System.Collections;

public class justDesserts : IEffectCard {//a trap card dealing damage according to the number of opponent's monster cards on field

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

			int col=0;
			int cardCount = 0;
			while (col < 5) {
				if (owner.GetPlayingField ().GetOpposingPlayingField ().GetCardByIndex (1, col) != null) {
						cardCount++;
				}
				col++;
			}
			if (cardCount!=0){
						owner.GetPlayingField ().GetOpposingPlayingField ().player.TakeLifePointsDamage(500*cardCount);
						owner.GetPlayingField ().CmdDestroyCard (this);
			}
		}

	}
	public override bool CanActivate()
	{
        return owner.gpManager.GetCurrentPhase() == EGamePhase.BattlePhase;
	}
}