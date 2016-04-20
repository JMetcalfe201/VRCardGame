using UnityEngine;
using System.Collections;

public class DarkHole : IEffectCard {//destroy all monsters on field

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
			for (int i = 0; i < 5; i++) {
				owner.GetPlayingField ().GetOpposingPlayingField ().CmdForceDestroyMonsterCard (i);
				owner.GetPlayingField ().CmdForceDestroyMonsterCard (i);

			}
			owner.GetPlayingField ().CmdDestroyCard (this);


		}

	}
}
