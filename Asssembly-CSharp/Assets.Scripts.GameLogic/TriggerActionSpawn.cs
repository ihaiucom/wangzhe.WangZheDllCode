using AGE;
using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionSpawn : TriggerActionBase
	{
		public TriggerActionSpawn(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			GameObject[] refObjList = this.RefObjList;
			for (int i = 0; i < refObjList.Length; i++)
			{
				GameObject gameObject = refObjList[i];
				if (!(gameObject == null))
				{
					SpawnGroup component = gameObject.GetComponent<SpawnGroup>();
					if (!(component == null))
					{
						component.TriggerStartUp();
					}
				}
			}
			return null;
		}
	}
}
