using AGE;
using Assets.Scripts.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionTextBubble : TriggerActionBase
	{
		private struct ActorRootInfo
		{
			public PoolObjHandle<ActorRoot> Actor;

			public int Id;
		}

		private ListView<TriggerActionTextBubble.ActorRootInfo> m_actorTimerMap = new ListView<TriggerActionTextBubble.ActorRootInfo>();

		public TriggerActionTextBubble(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
		}

		private bool HasActor(PoolObjHandle<ActorRoot> InActor)
		{
			return this.FindActor(InActor) != -1;
		}

		private int FindActor(PoolObjHandle<ActorRoot> InActor)
		{
			for (int i = 0; i < this.m_actorTimerMap.Count; i++)
			{
				if (this.m_actorTimerMap[i].Actor == InActor)
				{
					return i;
				}
			}
			return -1;
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			List<PoolObjHandle<ActorRoot>> list = new List<PoolObjHandle<ActorRoot>>();
			if (this.bSrc && src)
			{
				list.Add(src);
			}
			if (this.bAtker && atker)
			{
				list.Add(atker);
			}
			if (this.RefObjList != null && this.RefObjList.Length > 0)
			{
				GameObject[] refObjList = this.RefObjList;
				for (int i = 0; i < refObjList.Length; i++)
				{
					GameObject gameObject = refObjList[i];
					if (!(gameObject == null))
					{
						SpawnPoint componentInChildren = gameObject.GetComponentInChildren<SpawnPoint>();
						if (componentInChildren != null)
						{
							List<PoolObjHandle<ActorRoot>> spawnedList = componentInChildren.GetSpawnedList();
							List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = spawnedList.GetEnumerator();
							while (enumerator.MoveNext())
							{
								PoolObjHandle<ActorRoot> current = enumerator.get_Current();
								if (current)
								{
									list.Add(current);
								}
							}
						}
						else
						{
							PoolObjHandle<ActorRoot> actorRoot = ActorHelper.GetActorRoot(gameObject);
							if (actorRoot)
							{
								list.Add(actorRoot);
							}
						}
					}
				}
			}
			List<PoolObjHandle<ActorRoot>>.Enumerator enumerator2 = list.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				if (enumerator2.get_Current() && !this.HasActor(enumerator2.get_Current()))
				{
					this.SetHudText(Utility.GetBubbleText((uint)this.EnterUniqueId), enumerator2.get_Current());
					if (this.TotalTime > 0)
					{
						int id = Singleton<CTimerManager>.GetInstance().AddTimer(this.TotalTime, 1, new CTimer.OnTimeUpHandler(this.OnTimeUp), true);
						this.m_actorTimerMap.Add(new TriggerActionTextBubble.ActorRootInfo
						{
							Actor = enumerator2.get_Current(),
							Id = id
						});
					}
				}
			}
			return null;
		}

		private void SetHudText(string inText, PoolObjHandle<ActorRoot> inActor)
		{
			if (inActor && inActor.handle.HudControl != null)
			{
				inActor.handle.HudControl.SetTextHud(inText, this.Offset_x, this.Offset_y, true);
			}
		}

		private void OnTimeUp(int timersequence)
		{
			PoolObjHandle<ActorRoot> poolObjHandle = default(PoolObjHandle<ActorRoot>);
			int num = -1;
			for (int i = 0; i < this.m_actorTimerMap.Count; i++)
			{
				TriggerActionTextBubble.ActorRootInfo actorRootInfo = this.m_actorTimerMap[i];
				if (actorRootInfo.Id == timersequence)
				{
					num = i;
					poolObjHandle = actorRootInfo.Actor;
					break;
				}
			}
			if (poolObjHandle && num != -1)
			{
				this.SetHudText(string.Empty, poolObjHandle);
				this.m_actorTimerMap.RemoveAt(num);
			}
			Singleton<CTimerManager>.GetInstance().RemoveTimer(timersequence);
		}

		private void Clear()
		{
			ListView<TriggerActionTextBubble.ActorRootInfo>.Enumerator enumerator = this.m_actorTimerMap.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TriggerActionTextBubble.ActorRootInfo current = enumerator.Current;
				PoolObjHandle<ActorRoot> actor = current.Actor;
				TriggerActionTextBubble.ActorRootInfo current2 = enumerator.Current;
				int id = current2.Id;
				this.SetHudText(string.Empty, actor);
				Singleton<CTimerManager>.GetInstance().RemoveTimer(id);
			}
			this.m_actorTimerMap.Clear();
		}

		public override void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
		}

		public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
		{
			if (this.bStopWhenLeaving)
			{
				this.Clear();
			}
		}

		private void onActorDead(ref GameDeadEventParam prm)
		{
			PoolObjHandle<ActorRoot> src = prm.src;
			int num = this.FindActor(src);
			if (num != -1)
			{
				this.SetHudText(string.Empty, src);
				Singleton<CTimerManager>.GetInstance().RemoveTimer(this.m_actorTimerMap[num].Id);
				this.m_actorTimerMap.RemoveAt(num);
			}
		}

		public override void Destroy()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
		}
	}
}
