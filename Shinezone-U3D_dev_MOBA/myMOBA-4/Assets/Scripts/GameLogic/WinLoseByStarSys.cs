using Assets.Scripts.Framework;
using ResData;
using System;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.GameLogic
{
	public class WinLoseByStarSys : Singleton<WinLoseByStarSys>
	{
		public IStarEvaluation WinnerEvaluation;

		public IStarEvaluation LoserEvaluation;

		public event OnEvaluationChangedDelegate OnEvaluationChanged;

		public event OnEvaluationChangedDelegate OnFailureEvaluationChanged;

		public bool bStarted
		{
			get;
			private set;
		}

		public uint CurLevelTimeDuration
		{
			get;
			private set;
		}

		public bool isSuccess
		{
			get
			{
				return this.WinnerEvaluation != null && this.WinnerEvaluation.status == StarEvaluationStatus.Success;
			}
		}

		public bool isFailure
		{
			get
			{
				return this.LoserEvaluation != null && this.LoserEvaluation.status == StarEvaluationStatus.Success;
			}
		}

		public void StartFight()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null && Singleton<WinLoseByStarSys>.instance.Reset(curLvelContext, false))
			{
				WinLoseByStarSys expr_27 = Singleton<WinLoseByStarSys>.instance;
				expr_27.OnEvaluationChanged = (OnEvaluationChangedDelegate)Delegate.Combine(expr_27.OnEvaluationChanged, new OnEvaluationChangedDelegate(BattleLogic.OnWinStarSysChanged));
				WinLoseByStarSys expr_4D = Singleton<WinLoseByStarSys>.instance;
				expr_4D.OnFailureEvaluationChanged = (OnEvaluationChangedDelegate)Delegate.Combine(expr_4D.OnFailureEvaluationChanged, new OnEvaluationChangedDelegate(BattleLogic.OnLoseStarSysChanged));
				Singleton<WinLoseByStarSys>.instance.Start();
			}
		}

		public void EndGame()
		{
			WinLoseByStarSys expr_05 = Singleton<WinLoseByStarSys>.instance;
			expr_05.OnEvaluationChanged = (OnEvaluationChangedDelegate)Delegate.Remove(expr_05.OnEvaluationChanged, new OnEvaluationChangedDelegate(BattleLogic.OnWinStarSysChanged));
			WinLoseByStarSys expr_2B = Singleton<WinLoseByStarSys>.instance;
			expr_2B.OnFailureEvaluationChanged = (OnEvaluationChangedDelegate)Delegate.Remove(expr_2B.OnFailureEvaluationChanged, new OnEvaluationChangedDelegate(BattleLogic.OnLoseStarSysChanged));
			Singleton<WinLoseByStarSys>.instance.Clear();
		}

		public void Start()
		{
			if (this.WinnerEvaluation != null)
			{
				this.WinnerEvaluation.Start();
			}
			if (this.LoserEvaluation != null)
			{
				this.LoserEvaluation.Start();
			}
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_PostActorDead, new RefAction<GameDeadEventParam>(this.OnActorDeath));
			Singleton<GameEventSys>.instance.AddEventHandler<SCampScoreUpdateParam>(GameEventDef.Event_CampScoreUpdated, new RefAction<SCampScoreUpdateParam>(this.OnCampScoreUpdated));
			this.bStarted = true;
		}

		public void Clear()
		{
			if (this.WinnerEvaluation != null)
			{
				this.WinnerEvaluation.Dispose();
				this.WinnerEvaluation = null;
			}
			if (this.LoserEvaluation != null)
			{
				this.LoserEvaluation.Dispose();
				this.LoserEvaluation = null;
			}
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_PostActorDead, new RefAction<GameDeadEventParam>(this.OnActorDeath));
			Singleton<GameEventSys>.instance.RmvEventHandler<SCampScoreUpdateParam>(GameEventDef.Event_CampScoreUpdated, new RefAction<SCampScoreUpdateParam>(this.OnCampScoreUpdated));
			this.CurLevelTimeDuration = 0u;
			this.bStarted = false;
		}

		private void OnActorDeath(ref GameDeadEventParam prm)
		{
			if (this.WinnerEvaluation != null)
			{
				this.WinnerEvaluation.OnActorDeath(ref prm);
			}
			if (this.LoserEvaluation != null)
			{
				this.LoserEvaluation.OnActorDeath(ref prm);
			}
		}

		private void OnCampScoreUpdated(ref SCampScoreUpdateParam prm)
		{
			if (this.WinnerEvaluation != null)
			{
				this.WinnerEvaluation.OnCampScoreUpdated(ref prm);
			}
			if (this.LoserEvaluation != null)
			{
				this.LoserEvaluation.OnCampScoreUpdated(ref prm);
			}
		}

		public bool Reset(SLevelContext levelContext, bool bMultiGame)
		{
			this.Clear();
			bool result = false;
			if (levelContext.IsMobaModeWithOutGuide())
			{
				this.CurLevelTimeDuration = levelContext.m_timeDuration;
				if (levelContext.m_addWinCondStarId != 0u)
				{
					ResEvaluateStarInfo dataByKey = GameDataMgr.addWinLoseCondDatabin.GetDataByKey(levelContext.m_addWinCondStarId);
					DebugHelper.Assert(dataByKey != null);
					if (dataByKey != null)
					{
						this.WinnerEvaluation = this.CreateStar(dataByKey);
						DebugHelper.Assert(this.WinnerEvaluation != null, "我擦，怎会没有？");
						result = true;
					}
				}
				if (levelContext.m_addLoseCondStarId != 0u)
				{
					ResEvaluateStarInfo dataByKey2 = GameDataMgr.addWinLoseCondDatabin.GetDataByKey(levelContext.m_addLoseCondStarId);
					DebugHelper.Assert(dataByKey2 != null);
					if (dataByKey2 != null)
					{
						this.LoserEvaluation = this.CreateStar(dataByKey2);
						DebugHelper.Assert(this.LoserEvaluation != null, "我擦，怎会没有？");
						result = true;
					}
				}
			}
			return result;
		}

		private IStarEvaluation CreateStar(ResEvaluateStarInfo ConditionDetail)
		{
			StarEvaluation starEvaluation = new StarEvaluation();
			starEvaluation.OnChanged += new OnEvaluationChangedDelegate(this.OnEvaluationChangedInner);
			starEvaluation.Index = 0;
			starEvaluation.Initialize(ConditionDetail);
			return starEvaluation;
		}

		private void OnEvaluationChangedInner(IStarEvaluation InStarEvaluation, IStarCondition InStarCondition)
		{
			if (InStarEvaluation == this.WinnerEvaluation)
			{
				if (this.OnEvaluationChanged != null)
				{
					this.OnEvaluationChanged(InStarEvaluation, InStarCondition);
				}
			}
			else if (InStarEvaluation == this.LoserEvaluation && this.OnFailureEvaluationChanged != null)
			{
				this.OnFailureEvaluationChanged(InStarEvaluation, InStarCondition);
			}
		}
	}
}
