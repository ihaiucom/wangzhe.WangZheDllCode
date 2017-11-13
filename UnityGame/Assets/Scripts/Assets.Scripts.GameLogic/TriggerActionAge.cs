using AGE;
using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionAge : TriggerActionBase
	{
		private ListView<Action> m_duraActsPrivate = new ListView<Action>();

		public TriggerActionAge(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
		}

		private ListView<Action> PlayAgeActionPrivate(AreaEventTrigger.EActTiming inTiming, GameObject inSrc, GameObject inAtker)
		{
			return this.PlayAgeActionShared(inTiming, this.TimingActionsInter, new ActionStopDelegate(this.OnActionStopedPrivate), this.m_duraActsPrivate, inSrc, inAtker);
		}

		protected static Action PlayAgeActionShared(string inActionName, string inHelperName, GameObject inSrc, GameObject inAtker, int inHelperIndex = -1, ActionStopDelegate inCallback = null)
		{
			return DialogueProcessor.PlayAgeAction(inActionName, inHelperName, inSrc, inAtker, inCallback, inHelperIndex);
		}

		protected virtual ListView<Action> PlayAgeActionShared(AreaEventTrigger.EActTiming inTiming, AreaEventTrigger.STimingAction[] inTimingActs, ActionStopDelegate inCallback, ListView<Action> outDuraActs, GameObject inSrc, GameObject inAtker)
		{
			ListView<Action> listView = new ListView<Action>();
			for (int i = 0; i < inTimingActs.Length; i++)
			{
				AreaEventTrigger.STimingAction sTimingAction = inTimingActs[i];
				if (sTimingAction.Timing == inTiming)
				{
					ActionStopDelegate actionStopDelegate = null;
					if (inTiming == AreaEventTrigger.EActTiming.EnterDura)
					{
						actionStopDelegate = inCallback;
					}
					Action action = TriggerActionAge.PlayAgeActionShared(sTimingAction.ActionName, sTimingAction.HelperName, inSrc, inAtker, sTimingAction.HelperIndex, inCallback);
					if (action != null)
					{
						listView.Add(action);
						if (actionStopDelegate != null)
						{
							outDuraActs.Add(action);
						}
					}
				}
			}
			return listView;
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			GameObject inSrc = src ? src.handle.gameObject : null;
			GameObject gameObject = (inTrigger != null) ? inTrigger.GetTriggerObj() : null;
			if (!gameObject)
			{
				gameObject = (atker ? atker.handle.gameObject : null);
			}
			this.PlayAgeActionPrivate(AreaEventTrigger.EActTiming.Enter, inSrc, gameObject);
			ListView<Action> listView = this.PlayAgeActionPrivate(AreaEventTrigger.EActTiming.EnterDura, inSrc, gameObject);
			int num = this.RefObjList.Length;
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					this.PlayAgeActionPrivate(AreaEventTrigger.EActTiming.Enter, this.RefObjList[i], this.RefObjList[i]);
					listView.AddRange(this.PlayAgeActionPrivate(AreaEventTrigger.EActTiming.EnterDura, this.RefObjList[i], this.RefObjList[i]));
				}
			}
			RefParamOperator refParamOperator = new RefParamOperator();
			refParamOperator.AddRefParam("TriggerActionAgeEnterDura", listView);
			return refParamOperator;
		}

		public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
		{
			GameObject inSrc = src ? src.handle.gameObject : null;
			GameObject inAtker = (inTrigger != null) ? inTrigger.GetTriggerObj() : null;
			this.PlayAgeActionPrivate(AreaEventTrigger.EActTiming.Leave, inSrc, inAtker);
			AreaEventTrigger areaEventTrigger = inTrigger as AreaEventTrigger;
			if (areaEventTrigger != null)
			{
				RefParamOperator refParamOperator = areaEventTrigger._inActors.get_Item(src.handle.ObjID).refParams[this];
				if (refParamOperator != null)
				{
					ListView<Action> refParamObject = refParamOperator.GetRefParamObject<ListView<Action>>("TriggerActionAgeEnterDura");
					if (refParamObject != null)
					{
						ListView<Action>.Enumerator enumerator = refParamObject.GetEnumerator();
						while (enumerator.MoveNext())
						{
							enumerator.Current.Stop(false);
						}
					}
				}
			}
			int num = this.RefObjList.Length;
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					this.PlayAgeActionPrivate(AreaEventTrigger.EActTiming.Leave, this.RefObjList[i], this.RefObjList[i]);
				}
			}
		}

		public override void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			GameObject inSrc = src ? src.handle.gameObject : null;
			GameObject inAtker = (inTrigger != null) ? inTrigger.GetTriggerObj() : null;
			this.PlayAgeActionPrivate(AreaEventTrigger.EActTiming.Update, inSrc, inAtker);
			int num = this.RefObjList.Length;
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					this.PlayAgeActionPrivate(AreaEventTrigger.EActTiming.Update, this.RefObjList[i], this.RefObjList[i]);
				}
			}
		}

		private void OnActionStopedPrivate(ref PoolObjHandle<Action> action)
		{
			if (!action)
			{
				return;
			}
			action.handle.onActionStop -= new ActionStopDelegate(this.OnActionStopedPrivate);
			this.m_duraActsPrivate.Remove(action.handle);
		}
	}
}
