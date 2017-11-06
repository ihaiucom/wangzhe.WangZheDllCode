using AGE;
using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionSoldierLine : TriggerActionBase
	{
		public TriggerActionSoldierLine(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
		}

		private void UpdateSoldierRegionAvail(bool bAvailable)
		{
			if (Singleton<BattleLogic>.GetInstance().mapLogic != null && this.RefObjList != null)
			{
				for (int i = 0; i < this.RefObjList.Length; i++)
				{
					GameObject gameObject = this.RefObjList[i];
					if (!(gameObject == null))
					{
						SoldierRegion component = gameObject.GetComponent<SoldierRegion>();
						if (!(component == null))
						{
							Singleton<BattleLogic>.GetInstance().mapLogic.EnableSoldierRegion(bAvailable, component);
						}
					}
				}
			}
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			this.UpdateSoldierRegionAvail(this.bEnable);
			return null;
		}

		public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
		{
			if (this.bStopWhenLeaving)
			{
				this.UpdateSoldierRegionAvail(!this.bEnable);
			}
		}
	}
}
