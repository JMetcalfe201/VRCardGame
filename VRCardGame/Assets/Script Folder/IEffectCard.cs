using System;

namespace Cards
{
	abstract class IEffectCard
	{
		event OnActivate();
		public bool CanActivate()
		{

		}
	}
}
