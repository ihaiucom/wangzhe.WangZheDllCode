using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	public class HeroProficiency
	{
		public ObjWrapper m_wrapper;

		private int showTimeInterv;

		private bool m_needDeginShow = true;

		private int m_showTimes;

		public void Init(ObjWrapper wrapper)
		{
			this.m_wrapper = wrapper;
			this.showTimeInterv = 0;
			this.m_needDeginShow = true;
			this.m_showTimes = 0;
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_Odyssey, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_OdysseyBeStopped, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.RmvEventHandler<SettleEventParam>(GameEventDef.Event_SettleComplete, new RefAction<SettleEventParam>(this.OnSettleCompleteShow));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_Odyssey, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_OdysseyBeStopped, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.AddEventHandler<SettleEventParam>(GameEventDef.Event_SettleComplete, new RefAction<SettleEventParam>(this.OnSettleCompleteShow));
		}

		public void UnInit()
		{
			this.m_wrapper = null;
			this.showTimeInterv = 0;
			this.m_needDeginShow = true;
			this.m_showTimes = 0;
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_Odyssey, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_OdysseyBeStopped, new RefAction<DefaultGameEventParam>(this.OnShouldShowProficiencyEffect));
			Singleton<GameEventSys>.instance.RmvEventHandler<SettleEventParam>(GameEventDef.Event_SettleComplete, new RefAction<SettleEventParam>(this.OnSettleCompleteShow));
		}

		public void OnShouldShowProficiencyEffect(ref DefaultGameEventParam prm)
		{
			if (prm.orignalAtker == this.m_wrapper.actorPtr)
			{
				this.m_showTimes++;
				this.ShowProficiencyEffect();
			}
		}

		public void OnSettleCompleteShow(ref SettleEventParam prm)
		{
			if (this != null && this.m_wrapper != null && this.m_wrapper.actor != null)
			{
				COM_PLAYERCAMP hostPlayerCamp = Singleton<GamePlayerCenter>.instance.hostPlayerCamp;
				if (hostPlayerCamp == this.m_wrapper.actor.TheActorMeta.ActorCamp && prm.isWin)
				{
					this.m_showTimes++;
					this.ShowProficiencyEffect();
				}
				else if (hostPlayerCamp != this.m_wrapper.actor.TheActorMeta.ActorCamp && !prm.isWin)
				{
					this.m_showTimes++;
					this.ShowProficiencyEffect();
				}
			}
		}

		public void ShowProficiencyEffect()
		{
			ActorServerData actorServerData = default(ActorServerData);
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
			if (actorDataProvider.GetActorServerData(ref this.m_wrapper.actor.TheActorMeta, ref actorServerData))
			{
				bool flag = this.m_wrapper.actor.HudControl.PlayProficiencyAni(actorServerData.TheProficiencyInfo.Level);
				if (flag)
				{
					this.m_showTimes--;
				}
			}
		}

		public void UpdateLogic(int nDelta)
		{
			if (this.m_needDeginShow && Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsMobaMode())
			{
				this.showTimeInterv += nDelta;
				if (this.showTimeInterv > 5000)
				{
					this.m_showTimes++;
					this.ShowProficiencyEffect();
					this.showTimeInterv = 0;
					this.m_needDeginShow = false;
				}
			}
			if (this.m_showTimes > 0)
			{
				this.ShowProficiencyEffect();
			}
		}
	}
}
