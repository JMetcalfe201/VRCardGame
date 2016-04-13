using UnityEngine;
using System.Collections;

public class TrapHole : IEffectCard
{

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	// Called by playing field when placed on the field
	public override void Placed(bool onField)
	{
		base.Placed(onField);

		// Binds the function CheckCardPlaced to the event EventCardPlaced
		owner.gpManager.EventCardPlaced += CheckCardPlaced;
	}

	// Checks the conditions of the trap card
	public void CheckCardPlaced(int player, int rowIndex, int colIndex)
	{
		// Get player index
		int ownerIndex = (owner.IsFirstPlayer() ? 1 : 2);

		// If the other played the card
		if (ownerIndex != player)
		{
			// If it is a monster card. (row 0 is effect, row 1 is monster)
			if(rowIndex==1)
			{
				// Get the instance of the card from the oppenent's playingfield
				MonsterCard card = owner.GetPlayingField().GetOpposingPlayingField().GetCardByIndex(rowIndex, colIndex).GetComponent<MonsterCard>();
				if(card.attack>=1000){

				// Delete the monster cards from opponent's playing field
				
					owner.GetPlayingField().GetOpposingPlayingField().CmdForceDestroyMonsterCard(colIndex);
				}
			}
		}
	}
}
