using AGE;
using Assets.Scripts.Common;
using Pathfinding;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionBlockSwitcher : TriggerActionBase
	{
		public TriggerActionBlockSwitcher(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			GameObject[] refObjList = this.RefObjList;
			for (int i = 0; i < refObjList.Length; i++)
			{
				GameObject gameObject = refObjList[i];
				if (gameObject != null)
				{
					NavmeshCut componentInChildren = gameObject.GetComponentInChildren<NavmeshCut>();
					if (componentInChildren)
					{
						componentInChildren.enabled = this.bEnable;
					}
				}
			}
			return null;
		}

		public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
		{
			if (!this.bStopWhenLeaving)
			{
				return;
			}
			GameObject[] refObjList = this.RefObjList;
			for (int i = 0; i < refObjList.Length; i++)
			{
				GameObject gameObject = refObjList[i];
				if (gameObject != null)
				{
					NavmeshCut componentInChildren = gameObject.GetComponentInChildren<NavmeshCut>();
					if (componentInChildren)
					{
						componentInChildren.enabled = !this.bEnable;
					}
				}
			}
		}
	}
}
