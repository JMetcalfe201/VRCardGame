using UnityEngine;
using System.Collections;

public class MonsterReborn : IEffectCard {//add one monster from player's graveyard to field

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

			int monsterIndex = owner.GetPlayingField ().GetGraveYard ().DrawMonsterCard();
			if (monsterIndex != -1) {
				owner.GetPlayingField().AddCard(owner.GetPlayingField ().GetGraveYard ().GetCardIdByIndex(monsterIndex), true);
				owner.GetPlayingField ().GetGraveYard ().removeByIndex (monsterIndex);
			}



			owner.GetPlayingField ().CmdDestroyCard (this);


		}

	}
}
