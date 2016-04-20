using UnityEngine;
using System.Collections;

public class RaigekiBreak : IEffectCard {//discard one card in hand and destroy one monster on opponent's field

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

			if (owner.hand.Count > 0) {
				owner.hand.RemoveAt (0);
				int col = 0;
				while(col<5){
					if (owner.GetPlayingField ().GetOpposingPlayingField ().GetCardByIndex (1, col) != null) {
						owner.GetPlayingField ().GetOpposingPlayingField ().CmdForceDestroyMonsterCard (col);
						col += 5;
					}
					col++;

				}
			}


			owner.GetPlayingField ().CmdDestroyCard (this);


		}

	}
}
