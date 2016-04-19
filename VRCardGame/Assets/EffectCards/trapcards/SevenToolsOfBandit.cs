using UnityEngine;
using System.Collections;

public class SevenToolsOfBandit : IEffectCard  //block a trap card and take 1000 lp damage
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
			// If it is an effect card. (row 0 is effect, row 1 is monster)
			if (rowIndex == 0)
			{
				// Get the instance of the card from the opponent's playingfield
				IEffectCard card = owner.GetPlayingField().GetOpposingPlayingField().GetCardByIndex(rowIndex, colIndex).GetComponent<IEffectCard>();



				// If the card is revlead (activiated)
				if (card.revealed)
				{
					if (card.cardtype == ECardType.TRAP_CARD) {
						// Call block
						card.Block ();//should destroy the trap card
						owner.TakeLifePointsDamage (1000);
						owner.GetPlayingField ().DestroyCard (this);
					}
				}
			}

		}
	}
}
