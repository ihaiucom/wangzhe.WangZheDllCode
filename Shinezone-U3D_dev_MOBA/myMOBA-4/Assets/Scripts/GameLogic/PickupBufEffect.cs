using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class PickupBufEffect : BasePickupEffect
	{
		private ResBufConfigInfo BufDropInfo;

		public PickupBufEffect(ResBufConfigInfo InBufDropInfo)
		{
			this.BufDropInfo = InBufDropInfo;
		}

		public override void OnPickup(PoolObjHandle<ActorRoot> InTarget)
		{
			DebugHelper.Assert(InTarget);
			this.ApplyBuff(InTarget);
			base.OnPickup(InTarget);
		}

		public void ApplyBuff(PoolObjHandle<ActorRoot> InTarget)
		{
			BufConsumer bufConsumer = new BufConsumer((int)this.BufDropInfo.dwBufID, InTarget, InTarget);
			bufConsumer.Use();
		}
	}
}
