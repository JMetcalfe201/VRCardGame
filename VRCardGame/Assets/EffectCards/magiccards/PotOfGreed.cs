using UnityEngine;
using System.Collections;

public class PotOfGreed : IEffectCard {//draw 2 cards

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
			

			owner.addCardToHand (owner.GetPlayingField ().getDeck ().DrawTop ());
			owner.addCardToHand (owner.GetPlayingField ().getDeck ().DrawTop ());
			owner.GetPlayingField ().CmdDestroyCard (this);


		}

	}
}
