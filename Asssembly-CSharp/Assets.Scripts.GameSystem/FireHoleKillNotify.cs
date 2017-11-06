using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	public class FireHoleKillNotify
	{
		private int first_count;

		private int second_count;

		private int third_count;

		private bool bFrist_Notifyed;

		private bool bSecond_Notifyed;

		private bool bThird_Notifyed;

		public FireHoleKillNotify()
		{
			Singleton<EventRouter>.instance.AddEventHandler(EventID.BATTLE_KDA_CHANGED, new Action(this.OnBattleKDAChanged));
			if (!int.TryParse(Singleton<CTextManager>.instance.GetText("FireHole_First_Notify_Num"), ref this.first_count))
			{
				DebugHelper.Assert(false, "--- 2爷 你配的 FireHole_First_Notify_Num 好像不是整数哦， check out");
			}
			if (!int.TryParse(Singleton<CTextManager>.instance.GetText("FireHole_Second_Notify_Num"), ref this.second_count))
			{
				DebugHelper.Assert(false, "--- 2爷 你配的 FireHole_Second_Notify_Num 好像不是整数哦， check out");
			}
			if (!int.TryParse(Singleton<CTextManager>.instance.GetText("FireHole_Third_Notify_Num"), ref this.third_count))
			{
				DebugHelper.Assert(false, "--- 2爷 你配的 FireHole_Third_Notify_Num 好像不是整数哦， check out");
			}
			this.bFrist_Notifyed = (this.bSecond_Notifyed = (this.bThird_Notifyed = false));
		}

		public void Clear()
		{
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BATTLE_KDA_CHANGED, new Action(this.OnBattleKDAChanged));
		}

		private void OnBattleKDAChanged()
		{
			bool bSelfCamp_Notify3;
			if (!this.bFrist_Notifyed)
			{
				bool bSelfCamp_Notify;
				if (this._check(this.first_count, out bSelfCamp_Notify))
				{
					this.bFrist_Notifyed = true;
					this._broadcast(bSelfCamp_Notify, KillDetailInfoType.Info_Type_FireHole_first);
				}
			}
			else if (!this.bSecond_Notifyed)
			{
				bool bSelfCamp_Notify2;
				if (this._check(this.second_count, out bSelfCamp_Notify2))
				{
					this.bSecond_Notifyed = true;
					this._broadcast(bSelfCamp_Notify2, KillDetailInfoType.Info_Type_FireHole_second);
				}
			}
			else if (!this.bThird_Notifyed && this._check(this.third_count, out bSelfCamp_Notify3))
			{
				this.bThird_Notifyed = true;
				this._broadcast(bSelfCamp_Notify3, KillDetailInfoType.Info_Type_FireHole_third);
			}
		}

		public void _broadcast(bool bSelfCamp_Notify, KillDetailInfoType type)
		{
			KillDetailInfo killDetailInfo = new KillDetailInfo();
			killDetailInfo.bSelfCamp = bSelfCamp_Notify;
			killDetailInfo.Type = type;
			Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, killDetailInfo);
		}

		private bool _check(int count, out bool bSelfCamp_Notify)
		{
			int campKillCount = this.getCampKillCount(true);
			int campKillCount2 = this.getCampKillCount(false);
			if (campKillCount >= count)
			{
				bSelfCamp_Notify = true;
				return true;
			}
			if (campKillCount2 >= count)
			{
				bSelfCamp_Notify = false;
				return true;
			}
			bSelfCamp_Notify = false;
			return false;
		}

		private int getCampKillCount(bool bSelfCamp)
		{
			COM_PLAYERCAMP camp = bSelfCamp ? Singleton<GamePlayerCenter>.instance.hostPlayerCamp : this.getEnemyCamp();
			CPlayerKDAStat playerKDAStat = Singleton<BattleStatistic>.instance.m_playerKDAStat;
			return playerKDAStat.GetTeamKillNum(camp);
		}

		private COM_PLAYERCAMP getEnemyCamp()
		{
			if (Singleton<GamePlayerCenter>.instance.hostPlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
			{
				return COM_PLAYERCAMP.COM_PLAYERCAMP_2;
			}
			if (Singleton<GamePlayerCenter>.instance.hostPlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
			{
				return COM_PLAYERCAMP.COM_PLAYERCAMP_1;
			}
			DebugHelper.Assert(false, "getEnemyCamp error check out");
			return COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
		}
	}
}
