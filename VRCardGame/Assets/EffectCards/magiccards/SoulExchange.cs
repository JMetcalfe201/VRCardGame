using UnityEngine;
using System.Collections;

public class SoulExchange : IEffectCard {//discard one card in hand and destroy all monsters on opponent's field

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
			while (col < 5) {
				MonsterCard opcard = owner.GetPlayingField ().GetOpposingPlayingField ().GetCardByIndex (1, col).GetComponent<MonsterCard>();
				if (opcard != null) {
					owner.GetPlayingField().AddCard(opcard.cardID, true);//may need to be fixed, intend to put the card in my own playing field
					owner.GetPlayingField ().GetOpposingPlayingField ().CmdForceDestroyMonsterCard (col);
					// ???
                    //col += 5;

				}
                col++;
			}



			owner.GetPlayingField ().CmdDestroyCard (this);


		}

	}
}
