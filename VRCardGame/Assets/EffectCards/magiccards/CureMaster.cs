using UnityEngine;
using System.Collections;

public class CureMaster : IEffectCard {//discard one card in hand and destroy all monsters on opponent's field

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


			owner.TakeLifePointsDamage (-1000);
            for (int i = 0; i < 5; i++)
            {
                owner.GetPlayingField().GetOpposingPlayingField().CmdForceDestroyMonsterCard(i);
            }

            owner.GetPlayingField ().CmdDestroyCard (this);


		}

	}
}
