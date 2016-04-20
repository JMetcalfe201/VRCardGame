using UnityEngine;
using System.Collections;

public class EnchantedJavelin : IEffectCard {//increse your lp by the atk of the highest atk monster on field

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


			int col = 0;
			int increase = 0;
			while(col<5){
				MonsterCard opcard = owner.GetPlayingField ().GetOpposingPlayingField ().GetCardByIndex (1, col).GetComponent<MonsterCard>();
				if (opcard != null) {
					if(opcard.attack>increase){
						increase = opcard.attack;
					}

				}
				MonsterCard mycard = owner.GetPlayingField ().GetCardByIndex (1, col).GetComponent<MonsterCard>();
				if (mycard != null) {
					if (mycard.attack > increase) {
						increase = mycard.attack;
					}
				}

			}

			owner.TakeLifePointsDamage (0 - increase);
			owner.GetPlayingField ().CmdDestroyCard (this);


		}

	}
}
