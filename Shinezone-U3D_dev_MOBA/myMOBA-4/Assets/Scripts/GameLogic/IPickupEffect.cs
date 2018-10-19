using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public interface IPickupEffect
	{
		void Bind(DropItem item);

		bool CanPickup(PoolObjHandle<ActorRoot> InTarget);

		void OnPickup(PoolObjHandle<ActorRoot> InTarget);
	}
}
