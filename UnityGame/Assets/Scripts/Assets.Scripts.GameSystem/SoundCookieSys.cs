using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.Sound;
using System;

namespace Assets.Scripts.GameSystem
{
	public class SoundCookieSys : Singleton<SoundCookieSys>
	{
		private bool bIsLuandouPlayMode;

		private bool bIsClassic5V5Mode;

		private bool bIsBaronActived;

		private PoolObjHandle<ActorRoot> BaronActor;

		private int FadeOutTime = 5000;

		public override void Init()
		{
			base.Init();
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.onFightStart));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.OnFightOver));
		}

		public override void UnInit()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.onFightStart));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.OnFightOver));
			base.UnInit();
		}

		private void OnAchievementEvent(KillDetailInfo DetailInfo)
		{
			if (this.bIsLuandouPlayMode)
			{
				this.OnLuanDouNotify(ref DetailInfo);
			}
		}

		private void OnLuanDouNotify(ref KillDetailInfo InKillInfoRef)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer != null && hostPlayer.Captain && InKillInfoRef.Killer)
			{
				if (InKillInfoRef.Type == KillDetailInfoType.Info_Type_DestroyTower)
				{
					this.PlayBattleEvent("Set_Theme");
				}
				else if (InKillInfoRef.Type == KillDetailInfoType.Info_Type_QuataryKill)
				{
					if (InKillInfoRef.Killer.handle.IsSelfCamp(hostPlayer.Captain))
					{
						this.PlayBattleEvent("Trigger_FourKill");
					}
					else
					{
						this.PlayBattleEvent("Trigger_BeFiveKill");
					}
				}
				else if (InKillInfoRef.Type == KillDetailInfoType.Info_Type_PentaKill)
				{
					if (InKillInfoRef.Killer.handle.IsSelfCamp(hostPlayer.Captain))
					{
						this.PlayBattleEvent("Trigger_FiveKill");
					}
					else
					{
						this.PlayBattleEvent("Trigger_BeFiveKill");
					}
				}
			}
		}

		private void PlayBattleEvent(string InSoundEvent)
		{
			Singleton<CSoundManager>.instance.PlayBattleSound2D(InSoundEvent);
		}

		private void onFightStart(ref DefaultGameEventParam prm)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			this.bIsLuandouPlayMode = curLvelContext.IsLuanDouPlayMode();
			this.bIsClassic5V5Mode = curLvelContext.IsNorma5v5PlayMode();
			if (this.bIsLuandouPlayMode)
			{
			}
		}

		private void OnFightOver(ref DefaultGameEventParam prm)
		{
			if (this.bIsLuandouPlayMode)
			{
			}
		}

		public void OnBaronAttacked(MonsterWrapper InBaron, ActorRoot InAtacker)
		{
			if (!this.bIsClassic5V5Mode || !InAtacker.IsHostCamp())
			{
				return;
			}
			bool isAttacked = InBaron.m_isAttacked;
			if (!this.bIsBaronActived && isAttacked)
			{
				this.PlayBattleEvent("Play_Music_Dalong_S1");
				this.BaronActor = InBaron.GetActor();
				this.bIsBaronActived = true;
				this.RegistBaronEvents();
			}
		}

		private void RegistBaronEvents()
		{
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorExitCombat, new RefAction<DefaultGameEventParam>(this.OnExitCombat));
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
			Singleton<CTimerManager>.instance.AddTimer(this.FadeOutTime, 1, new CTimer.OnTimeUpHandler(this.OnFadeOut));
		}

		private void UnRegistBaronEvents()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorExitCombat, new RefAction<DefaultGameEventParam>(this.OnExitCombat));
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
			Singleton<CTimerManager>.instance.RemoveTimer(new CTimer.OnTimeUpHandler(this.OnFadeOut));
		}

		private void OnExitCombat(ref DefaultGameEventParam prm)
		{
			if (prm.src == this.BaronActor && this.bIsBaronActived)
			{
				this.BaronActor = default(PoolObjHandle<ActorRoot>);
				this.bIsBaronActived = false;
				this.PlayBattleEvent("Set_Music_DaLong_S2_Cease");
				this.UnRegistBaronEvents();
			}
		}

		private void OnActorDead(ref GameDeadEventParam prm)
		{
			if (prm.src == this.BaronActor && this.bIsBaronActived)
			{
				this.BaronActor = default(PoolObjHandle<ActorRoot>);
				this.bIsBaronActived = false;
				this.UnRegistBaronEvents();
			}
		}

		private void OnFadeOut(int timerSequence)
		{
			this.PlayBattleEvent("Set_Music_DaLong_S2");
			Singleton<CTimerManager>.instance.RemoveTimer(new CTimer.OnTimeUpHandler(this.OnFadeOut));
		}
	}
}
