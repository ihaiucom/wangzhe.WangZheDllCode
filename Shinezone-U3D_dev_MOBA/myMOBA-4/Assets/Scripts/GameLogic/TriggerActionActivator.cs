using AGE;
using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionActivator : TriggerActionBase
	{
		public TriggerActionActivator(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			GameObject[] refObjList = this.RefObjList;
			if (refObjList != null && refObjList.Length > 0)
			{
				GameObject[] array = refObjList;
				for (int i = 0; i < array.Length; i++)
				{
					GameObject gameObject = array[i];
					if (!(gameObject == null))
					{
						gameObject.CustomSetActive(this.bEnable);
						FuncRegion component = gameObject.GetComponent<FuncRegion>();
						if (component != null)
						{
							if (this.bEnable)
							{
								component.Startup();
							}
							else
							{
								component.Stop();
							}
						}
					}
				}
			}
			if (this.bSrc && src)
			{
				src.handle.gameObject.CustomSetActive(this.bEnable);
				FuncRegion component2 = src.handle.gameObject.GetComponent<FuncRegion>();
				if (component2 != null)
				{
					if (this.bEnable)
					{
						component2.Startup();
					}
					else
					{
						component2.Stop();
					}
				}
			}
			if (this.bAtker && atker)
			{
				atker.handle.gameObject.CustomSetActive(this.bEnable);
				FuncRegion component3 = atker.handle.gameObject.GetComponent<FuncRegion>();
				if (component3 != null)
				{
					if (this.bEnable)
					{
						component3.Startup();
					}
					else
					{
						component3.Stop();
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
			if (refObjList != null && refObjList.Length > 0)
			{
				GameObject[] array = refObjList;
				for (int i = 0; i < array.Length; i++)
				{
					GameObject gameObject = array[i];
					if (!(gameObject == null))
					{
						gameObject.CustomSetActive(!this.bEnable);
					}
				}
			}
			if (this.bSrc && src)
			{
				src.handle.gameObject.CustomSetActive(!this.bEnable);
			}
			if (this.bAtker && inTrigger != null)
			{
				inTrigger.GetTriggerObj().CustomSetActive(!this.bEnable);
			}
		}
	}
}
