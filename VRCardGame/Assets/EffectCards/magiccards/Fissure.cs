using UnityEngine;
using System.Collections;

public class Fissure : IEffectCard {//destroy lowest attack monster from opponent's field

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
			int atkp = 5000;
			int colIndex = -1;
			for (int i = 0; i < 5; i++) {
				MonsterCard card = owner.GetPlayingField ().GetOpposingPlayingField ().GetCardByIndex(1, i).GetComponent<MonsterCard>();
				if (card != null) {
					if (card.attack < atkp) {
						atkp = card.attack;
						colIndex = i;
					}
				}
			}
			if (colIndex != -1) {
				owner.GetPlayingField ().GetOpposingPlayingField ().CmdForceDestroyMonsterCard (colIndex);
			}
			owner.GetPlayingField ().CmdDestroyCard (this);
		

		}

	}
}
