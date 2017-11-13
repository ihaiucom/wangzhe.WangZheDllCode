using AGE;
using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionBuff : TriggerActionBase
	{
		public TriggerActionBuff(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			int enterUniqueId = this.EnterUniqueId;
			GameObject[] refObjList = this.RefObjList;
			RefParamOperator refParamOperator = new RefParamOperator();
			if (enterUniqueId > 0)
			{
				if (refObjList != null)
				{
					int num = refObjList.Length;
					for (int i = 0; i < num; i++)
					{
						GameObject gameObject = refObjList[i];
						if (!(gameObject == null))
						{
							PoolObjHandle<ActorRoot> actorRoot = ActorHelper.GetActorRoot(gameObject);
							if (actorRoot)
							{
								BufConsumer bufConsumer = new BufConsumer(enterUniqueId, actorRoot, actorRoot);
								if (bufConsumer.Use())
								{
									refParamOperator = new RefParamOperator();
									string name = string.Format("TriggerActionBuffTar_{0}", i);
									refParamOperator.AddRefParam(name, bufConsumer.buffSkill);
								}
							}
						}
					}
				}
				if (this.bSrc && src)
				{
					BufConsumer bufConsumer2 = new BufConsumer(enterUniqueId, src, atker);
					if (bufConsumer2.Use())
					{
						refParamOperator = new RefParamOperator();
						refParamOperator.AddRefParam("TriggerActionBuffSrc", bufConsumer2.buffSkill);
					}
				}
				if (this.bAtker && atker)
				{
					BufConsumer bufConsumer3 = new BufConsumer(enterUniqueId, atker, src);
					if (bufConsumer3.Use())
					{
						refParamOperator = new RefParamOperator();
						refParamOperator.AddRefParam("TriggerActionBuffAtker", bufConsumer3.buffSkill);
					}
				}
			}
			return refParamOperator;
		}

		public override void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			int updateUniqueId = this.UpdateUniqueId;
			if (updateUniqueId > 0)
			{
				BufConsumer bufConsumer = new BufConsumer(updateUniqueId, src, atker);
				bufConsumer.Use();
			}
		}

		public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
		{
			int leaveUniqueId = this.LeaveUniqueId;
			if (leaveUniqueId > 0)
			{
				BufConsumer bufConsumer = new BufConsumer(leaveUniqueId, src, new PoolObjHandle<ActorRoot>(null));
				bufConsumer.Use();
			}
			int enterUniqueId = this.EnterUniqueId;
			if (this.bStopWhenLeaving && enterUniqueId > 0 && inTrigger != null)
			{
				AreaEventTrigger areaEventTrigger = inTrigger as AreaEventTrigger;
				if (areaEventTrigger != null)
				{
					RefParamOperator refParamOperator = areaEventTrigger._inActors.get_Item(src.handle.ObjID).refParams[this];
					if (refParamOperator != null)
					{
						ListView<string> listView = new ListView<string>();
						GameObject[] refObjList = this.RefObjList;
						if (refObjList != null)
						{
							int num = refObjList.Length;
							for (int i = 0; i < num; i++)
							{
								listView.Add(string.Format("TriggerActionBuffTar_{0}", i));
							}
						}
						if (this.bSrc)
						{
							listView.Add("TriggerActionBuffSrc");
						}
						if (this.bAtker)
						{
							listView.Add("TriggerActionBuffAtker");
						}
						ListView<string>.Enumerator enumerator = listView.GetEnumerator();
						while (enumerator.MoveNext())
						{
							string current = enumerator.Current;
							if (!string.IsNullOrEmpty(current))
							{
								BuffFense refParamObject = refParamOperator.GetRefParamObject<BuffFense>(current);
								if (refParamObject != null)
								{
									refParamObject.Stop();
								}
							}
						}
					}
				}
			}
		}
	}
}
