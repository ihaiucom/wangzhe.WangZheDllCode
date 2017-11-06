using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public class BasePickupEffect : IPickupEffect
	{
		private DropItem Item;

		public virtual void Bind(DropItem item)
		{
			this.Item = item;
		}

		public virtual bool CanPickup(PoolObjHandle<ActorRoot> InTarget)
		{
			return true;
		}

		public virtual void OnPickup(PoolObjHandle<ActorRoot> InTarget)
		{
			Singleton<DropItemMgr>.instance.RemoveItem(this.Item);
			this.Item.Destroy();
		}
	}
}
