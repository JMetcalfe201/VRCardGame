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

            if (owner.GetPlayingField().getMonsterCards().Count < 5)
            {
                int monsterIndex = owner.GetPlayingField().GetGraveYard().drawFirstMonsterID();  // also removes from graveyard
                if (monsterIndex != -1)
                {
                    owner.GetPlayingField().AddCard(monsterIndex, true);
                    owner.GetPlayingField().CmdDestroyCard(this);
                }
            }
            else
            {
                // ADD USER PROMPT HERE
                Debug.Log("Can't use card, field is full");
            }
		}
	}
}
