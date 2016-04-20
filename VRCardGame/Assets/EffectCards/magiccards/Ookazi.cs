using UnityEngine;
using System.Collections;

public class Ookazi : IEffectCard {//discard one card in hand and destroy all monsters on opponent's field

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


			owner.GetPlayingField ().GetOpposingPlayingField ().player.TakeLifePointsDamage (800);
	

			owner.GetPlayingField ().CmdDestroyCard (this);


		}

	}
}
