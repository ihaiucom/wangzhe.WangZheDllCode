using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[AddComponentMenu("MMGameTrigger/Global Trigger")]
	public class GlobalTrigger : MonoBehaviour, ITrigger
	{
		private class CDelayMatch
		{
			public CTriggerMatch TriggerMatch;

			public PoolObjHandle<ActorRoot> SrcHandle;

			public PoolObjHandle<ActorRoot> AtkerHandle;
		}

		public CTriggerMatch[] TriggerMatches = new CTriggerMatch[0];

		public GameObject[] DeactiveObjList = new GameObject[0];

		[SerializeField]
		public TriggerActionWrapper[] ActionList = new TriggerActionWrapper[0];

		private MultiValueListDictionary<uint, TriggerActionBase> TriggerActionMultiMap = new MultiValueListDictionary<uint, TriggerActionBase>();

		private DictionaryView<int, GlobalTrigger.CDelayMatch> DelayTimeSeqMap = new DictionaryView<int, GlobalTrigger.CDelayMatch>();

		public int CurGlobalVariable;

		public void UpdateLogic(int inDelta)
		{
			CTriggerMatch[] triggerMatches = this.TriggerMatches;
			for (int i = 0; i < triggerMatches.Length; i++)
			{
				CTriggerMatch cTriggerMatch = triggerMatches[i];
				if (cTriggerMatch != null && cTriggerMatch.bCoolingDown)
				{
					cTriggerMatch.m_cooldownTimer -= inDelta;
					if (cTriggerMatch.m_cooldownTimer <= 0)
					{
						cTriggerMatch.m_cooldownTimer = 0;
					}
				}
			}
		}

		public GameObject GetTriggerObj()
		{
			return base.gameObject;
		}

		public void PrepareFight()
		{
			GameObject[] deactiveObjList = this.DeactiveObjList;
			for (int i = 0; i < deactiveObjList.Length; i++)
			{
				GameObject gameObject = deactiveObjList[i];
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
		}

		public void BindSkillCancelListener()
		{
			Singleton<GameSkillEventSys>.instance.AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UseCanceled, new GameSkillEvent<DefaultSkillEventParam>(this.onUseSkillCanceled));
		}

		public void UnbindSkillCancelListener()
		{
			Singleton<GameSkillEventSys>.instance.RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UseCanceled, new GameSkillEvent<DefaultSkillEventParam>(this.onUseSkillCanceled));
		}

		private void Awake()
		{
			if (Singleton<GameEventSys>.instance != null)
			{
				Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
				Singleton<GameEventSys>.instance.AddEventHandler<SGroupDeadEventParam>(GameEventDef.Event_SpawnGroupDead, new RefAction<SGroupDeadEventParam>(this.onSpawnGroupDone));
				Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.onFightPrepare));
				Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
				Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.onFightStart));
				Singleton<GameEventSys>.instance.AddEventHandler<PoolObjHandle<ActorRoot>>(GameEventDef.Event_ActorInit, new RefAction<PoolObjHandle<ActorRoot>>(this.onActorInit));
				Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorEnterCombat, new RefAction<DefaultGameEventParam>(this.onEnterCombat));
				Singleton<GameEventSys>.instance.AddEventHandler<TalentLevelChangeParam>(GameEventDef.Event_TalentLevelChange, new RefAction<TalentLevelChangeParam>(this.onTalentLevelChange));
			}
			if (Singleton<GameSkillEventSys>.instance != null)
			{
				Singleton<GameSkillEventSys>.instance.AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_UseSkill, new GameSkillEvent<ActorSkillEventParam>(this.onUseSkill));
			}
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this.OnActorBattleCoinChanged));
			TriggerActionWrapper[] actionList = this.ActionList;
			for (int i = 0; i < actionList.Length; i++)
			{
				TriggerActionWrapper triggerActionWrapper = actionList[i];
				if (triggerActionWrapper != null)
				{
					EGlobalTriggerAct triggerType = triggerActionWrapper.TriggerType;
					triggerActionWrapper.Init(0);
					TriggerActionBase actionInternal = triggerActionWrapper.GetActionInternal();
					DebugHelper.Assert(actionInternal != null);
					this.TriggerActionMultiMap.Add((uint)triggerType, actionInternal);
				}
			}
		}

		private void DoTriggeringImpl(CTriggerMatch match, PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker)
		{
			ListView<TriggerActionBase> values = this.TriggerActionMultiMap.GetValues((uint)match.ActType, true);
			for (int i = 0; i < values.Count; i++)
			{
				TriggerActionBase triggerActionBase = values[i];
				if (triggerActionBase != null)
				{
					triggerActionBase.AppendRefObj(match.Listeners);
					triggerActionBase.TriggerEnter(src, atker, this);
				}
			}
			if (match.ActionList != null && match.ActionList.Length > 0)
			{
				int num = match.ActionList.Length;
				for (int j = 0; j < num; j++)
				{
					TriggerActionWrapper triggerActionWrapper = match.ActionList[j];
					if (triggerActionWrapper != null)
					{
						TriggerActionBase actionInternal = triggerActionWrapper.GetActionInternal();
						if (actionInternal == null)
						{
							triggerActionWrapper.Init(0);
							actionInternal = triggerActionWrapper.GetActionInternal();
							DebugHelper.Assert(actionInternal != null);
						}
						actionInternal.AppendRefObj(match.Listeners);
						actionInternal.TriggerEnter(src, atker, this);
					}
				}
			}
		}

		private void ClearDelayTimers()
		{
			DictionaryView<int, GlobalTrigger.CDelayMatch>.Enumerator enumerator = this.DelayTimeSeqMap.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, GlobalTrigger.CDelayMatch> current = enumerator.Current;
				int key = current.Key;
				Singleton<CTimerManager>.instance.RemoveTimer(key);
			}
			this.DelayTimeSeqMap.Clear();
		}

		private void OnDelayTriggerTimer(int inTimeSeq)
		{
			if (this.DelayTimeSeqMap.ContainsKey(inTimeSeq))
			{
				GlobalTrigger.CDelayMatch cDelayMatch = this.DelayTimeSeqMap[inTimeSeq];
				if (cDelayMatch != null)
				{
					this.DoTriggeringImpl(cDelayMatch.TriggerMatch, cDelayMatch.SrcHandle, cDelayMatch.AtkerHandle);
				}
				this.DelayTimeSeqMap.Remove(inTimeSeq);
			}
			Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
		}

		private void DoTriggering(CTriggerMatch match, PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker)
		{
			if (match.TriggerCountMax > 0 && ++match.m_triggeredCounter > match.TriggerCountMax)
			{
				return;
			}
			if (!match.bCoolingDown)
			{
				match.IntoCoolingDown();
				if (match.DelayTime > 0)
				{
					int num = Singleton<CTimerManager>.instance.AddTimer(match.DelayTime, 1, new CTimer.OnTimeUpHandler(this.OnDelayTriggerTimer), true);
					if (num >= 0)
					{
						DebugHelper.Assert(!this.DelayTimeSeqMap.ContainsKey(num));
						GlobalTrigger.CDelayMatch cDelayMatch = new GlobalTrigger.CDelayMatch();
						cDelayMatch.AtkerHandle = atker;
						cDelayMatch.SrcHandle = src;
						cDelayMatch.TriggerMatch = match;
						this.DelayTimeSeqMap.Add(num, cDelayMatch);
					}
				}
				else
				{
					this.DoTriggeringImpl(match, src, atker);
				}
			}
		}

		private bool FilterMatch(EGlobalGameEvent inEventType, CTriggerMatch match, PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ref SFilterMatchParam param, int inMatchIndex)
		{
			return match.EventType == inEventType && match.Condition.FilterMatch(inEventType, src, atker, ref param, match, inMatchIndex);
		}

		private void onActorDead(ref GameDeadEventParam prm)
		{
			SFilterMatchParam sFilterMatchParam = default(SFilterMatchParam);
			int num = this.TriggerMatches.Length;
			for (int i = 0; i < num; i++)
			{
				CTriggerMatch cTriggerMatch = this.TriggerMatches[i];
				if (cTriggerMatch != null)
				{
					if (this.FilterMatch(EGlobalGameEvent.ActorDead, cTriggerMatch, prm.src, prm.orignalAtker, ref sFilterMatchParam, i))
					{
						this.DoTriggering(cTriggerMatch, prm.src, prm.orignalAtker);
					}
				}
			}
		}

		private void onSpawnGroupDone(ref SGroupDeadEventParam inParam)
		{
			int num = this.TriggerMatches.Length;
			for (int i = 0; i < num; i++)
			{
				CTriggerMatch cTriggerMatch = this.TriggerMatches[i];
				if (cTriggerMatch != null)
				{
					PoolObjHandle<ActorRoot> src = new PoolObjHandle<ActorRoot>(null);
					PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
					SFilterMatchParam sFilterMatchParam = default(SFilterMatchParam);
					sFilterMatchParam.csg = inParam.csg;
					sFilterMatchParam.sg = inParam.sg;
					if (this.FilterMatch(EGlobalGameEvent.SpawnGroupDead, cTriggerMatch, src, atker, ref sFilterMatchParam, i))
					{
						this.DoTriggering(cTriggerMatch, src, atker);
					}
				}
			}
		}

		private void onFightPrepare(ref DefaultGameEventParam prm)
		{
			SFilterMatchParam sFilterMatchParam = default(SFilterMatchParam);
			int num = this.TriggerMatches.Length;
			for (int i = 0; i < num; i++)
			{
				CTriggerMatch cTriggerMatch = this.TriggerMatches[i];
				if (cTriggerMatch != null)
				{
					if (this.FilterMatch(EGlobalGameEvent.FightPrepare, cTriggerMatch, prm.src, prm.atker, ref sFilterMatchParam, i))
					{
						this.DoTriggering(cTriggerMatch, prm.src, prm.atker);
					}
				}
			}
		}

		private void onActorDamage(ref HurtEventResultInfo prm)
		{
			int num = this.TriggerMatches.Length;
			SFilterMatchParam sFilterMatchParam = default(SFilterMatchParam);
			sFilterMatchParam.hurtInfo = prm;
			for (int i = 0; i < num; i++)
			{
				CTriggerMatch cTriggerMatch = this.TriggerMatches[i];
				if (cTriggerMatch != null)
				{
					if (this.FilterMatch(EGlobalGameEvent.ActorDamage, cTriggerMatch, prm.src, prm.atker, ref sFilterMatchParam, i))
					{
						this.DoTriggering(cTriggerMatch, prm.src, prm.atker);
					}
				}
			}
		}

		private void onFightStart(ref DefaultGameEventParam prm)
		{
			int num = this.TriggerMatches.Length;
			SFilterMatchParam sFilterMatchParam = default(SFilterMatchParam);
			for (int i = 0; i < num; i++)
			{
				CTriggerMatch cTriggerMatch = this.TriggerMatches[i];
				if (cTriggerMatch != null)
				{
					if (this.FilterMatch(EGlobalGameEvent.FightStart, cTriggerMatch, prm.src, prm.atker, ref sFilterMatchParam, i))
					{
						this.DoTriggering(cTriggerMatch, prm.src, prm.atker);
					}
				}
			}
		}

		private void onUseSkill(ref ActorSkillEventParam prm)
		{
			if (!ActorHelper.IsHostCtrlActor(ref prm.src))
			{
				return;
			}
			int num = this.TriggerMatches.Length;
			SFilterMatchParam sFilterMatchParam = default(SFilterMatchParam);
			sFilterMatchParam.slot = prm.slot;
			for (int i = 0; i < num; i++)
			{
				CTriggerMatch cTriggerMatch = this.TriggerMatches[i];
				if (cTriggerMatch != null)
				{
					PoolObjHandle<ActorRoot> src = new PoolObjHandle<ActorRoot>(null);
					PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
					if (this.FilterMatch(EGlobalGameEvent.UseSkill, cTriggerMatch, src, atker, ref sFilterMatchParam, i))
					{
						this.DoTriggering(cTriggerMatch, src, atker);
					}
				}
			}
		}

		private void onUseSkillCanceled(ref DefaultSkillEventParam prm)
		{
			int num = this.TriggerMatches.Length;
			SFilterMatchParam sFilterMatchParam = default(SFilterMatchParam);
			sFilterMatchParam.slot = prm.slot;
			for (int i = 0; i < num; i++)
			{
				CTriggerMatch cTriggerMatch = this.TriggerMatches[i];
				if (cTriggerMatch != null)
				{
					PoolObjHandle<ActorRoot> src = new PoolObjHandle<ActorRoot>(null);
					PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
					if (this.FilterMatch(EGlobalGameEvent.SkillUseCanceled, cTriggerMatch, src, atker, ref sFilterMatchParam, i))
					{
						this.DoTriggering(cTriggerMatch, src, atker);
					}
				}
			}
		}

		private void onActorInit(ref PoolObjHandle<ActorRoot> inActor)
		{
			int num = this.TriggerMatches.Length;
			SFilterMatchParam sFilterMatchParam = default(SFilterMatchParam);
			for (int i = 0; i < num; i++)
			{
				CTriggerMatch cTriggerMatch = this.TriggerMatches[i];
				if (cTriggerMatch != null)
				{
					PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
					if (this.FilterMatch(EGlobalGameEvent.ActorInit, cTriggerMatch, inActor, atker, ref sFilterMatchParam, i))
					{
						this.DoTriggering(cTriggerMatch, inActor, atker);
					}
				}
			}
		}

		public void OpenTalentTip()
		{
			int num = this.TriggerMatches.Length;
			SFilterMatchParam sFilterMatchParam = default(SFilterMatchParam);
			for (int i = 0; i < num; i++)
			{
				CTriggerMatch cTriggerMatch = this.TriggerMatches[i];
				if (cTriggerMatch != null)
				{
					PoolObjHandle<ActorRoot> src = new PoolObjHandle<ActorRoot>(null);
					PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
					if (this.FilterMatch(EGlobalGameEvent.OpenTalentTip, cTriggerMatch, src, atker, ref sFilterMatchParam, i))
					{
						this.DoTriggering(cTriggerMatch, src, atker);
					}
				}
			}
		}

		public void CloseTalentTip()
		{
			int num = this.TriggerMatches.Length;
			SFilterMatchParam sFilterMatchParam = default(SFilterMatchParam);
			for (int i = 0; i < num; i++)
			{
				CTriggerMatch cTriggerMatch = this.TriggerMatches[i];
				if (cTriggerMatch != null)
				{
					PoolObjHandle<ActorRoot> src = new PoolObjHandle<ActorRoot>(null);
					PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
					if (this.FilterMatch(EGlobalGameEvent.CloseTalentTip, cTriggerMatch, src, atker, ref sFilterMatchParam, i))
					{
						this.DoTriggering(cTriggerMatch, src, atker);
					}
				}
			}
		}

		private void onEnterCombat(ref DefaultGameEventParam prm)
		{
			int num = this.TriggerMatches.Length;
			SFilterMatchParam sFilterMatchParam = default(SFilterMatchParam);
			for (int i = 0; i < num; i++)
			{
				CTriggerMatch cTriggerMatch = this.TriggerMatches[i];
				if (cTriggerMatch != null)
				{
					if (this.FilterMatch(EGlobalGameEvent.EnterCombat, cTriggerMatch, prm.src, prm.atker, ref sFilterMatchParam, i))
					{
						this.DoTriggering(cTriggerMatch, prm.src, prm.atker);
					}
				}
			}
		}

		private void onTalentLevelChange(ref TalentLevelChangeParam prm)
		{
			int num = this.TriggerMatches.Length;
			SFilterMatchParam sFilterMatchParam = default(SFilterMatchParam);
			sFilterMatchParam.intParam = prm.TalentLevel;
			for (int i = 0; i < num; i++)
			{
				CTriggerMatch cTriggerMatch = this.TriggerMatches[i];
				if (cTriggerMatch != null)
				{
					PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
					if (this.FilterMatch(EGlobalGameEvent.TalentLevelChange, cTriggerMatch, prm.src, atker, ref sFilterMatchParam, i))
					{
						this.DoTriggering(cTriggerMatch, prm.src, atker);
					}
				}
			}
		}

		private void OnActorBattleCoinChanged(PoolObjHandle<ActorRoot> actor, int changeValue, bool isIncome, PoolObjHandle<ActorRoot> target)
		{
			int num = this.TriggerMatches.Length;
			SFilterMatchParam sFilterMatchParam = default(SFilterMatchParam);
			sFilterMatchParam.intParam = actor.handle.ValueComponent.GetGoldCoinInBattle();
			for (int i = 0; i < num; i++)
			{
				CTriggerMatch cTriggerMatch = this.TriggerMatches[i];
				if (cTriggerMatch != null)
				{
					PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
					if (this.FilterMatch(EGlobalGameEvent.BattleGoldChange, cTriggerMatch, actor, atker, ref sFilterMatchParam, i))
					{
						this.DoTriggering(cTriggerMatch, actor, atker);
					}
				}
			}
		}

		private void OnDestroy()
		{
			if (Singleton<GameEventSys>.instance != null)
			{
				Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
				Singleton<GameEventSys>.instance.RmvEventHandler<SGroupDeadEventParam>(GameEventDef.Event_SpawnGroupDead, new RefAction<SGroupDeadEventParam>(this.onSpawnGroupDone));
				Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.onFightPrepare));
				Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
				Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.onFightStart));
				Singleton<GameEventSys>.instance.RmvEventHandler<PoolObjHandle<ActorRoot>>(GameEventDef.Event_ActorInit, new RefAction<PoolObjHandle<ActorRoot>>(this.onActorInit));
				Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorEnterCombat, new RefAction<DefaultGameEventParam>(this.onEnterCombat));
				Singleton<GameEventSys>.instance.RmvEventHandler<TalentLevelChangeParam>(GameEventDef.Event_TalentLevelChange, new RefAction<TalentLevelChangeParam>(this.onTalentLevelChange));
			}
			if (Singleton<GameSkillEventSys>.instance != null)
			{
				Singleton<GameSkillEventSys>.instance.RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_UseSkill, new GameSkillEvent<ActorSkillEventParam>(this.onUseSkill));
				Singleton<GameSkillEventSys>.instance.RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UseCanceled, new GameSkillEvent<DefaultSkillEventParam>(this.onUseSkillCanceled));
			}
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this.OnActorBattleCoinChanged));
			this.ClearDelayTimers();
			TriggerActionWrapper[] actionList = this.ActionList;
			for (int i = 0; i < actionList.Length; i++)
			{
				TriggerActionWrapper triggerActionWrapper = actionList[i];
				if (triggerActionWrapper != null)
				{
					triggerActionWrapper.Destroy();
				}
			}
			CTriggerMatch[] triggerMatches = this.TriggerMatches;
			for (int j = 0; j < triggerMatches.Length; j++)
			{
				CTriggerMatch cTriggerMatch = triggerMatches[j];
				if (cTriggerMatch != null)
				{
					TriggerActionWrapper[] actionList2 = cTriggerMatch.ActionList;
					for (int k = 0; k < actionList2.Length; k++)
					{
						TriggerActionWrapper triggerActionWrapper2 = actionList2[k];
						if (triggerActionWrapper2 != null)
						{
							triggerActionWrapper2.Destroy();
						}
					}
				}
			}
		}
	}
}
