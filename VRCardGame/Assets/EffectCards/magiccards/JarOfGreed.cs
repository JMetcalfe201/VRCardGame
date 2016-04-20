using UnityEngine;
using System.Collections;

public class JarOfGreed : IEffectCard {//draw 1 card

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
			owner.GetPlayingField ().CmdDestroyCard (this);


		}

	}
}
