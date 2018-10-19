using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class CPlayerSoulLevelStat
	{
		private class SoulLevelDetail
		{
			public uint playerId;

			public uint[] changeTime;

			public SoulLevelDetail(uint playerId)
			{
				this.playerId = playerId;
				this.changeTime = new uint[ValueProperty.GetMaxSoulLvl()];
			}
		}

		private List<CPlayerSoulLevelStat.SoulLevelDetail> playerSoulLevelDetail = new List<CPlayerSoulLevelStat.SoulLevelDetail>();

		public void StartRecord()
		{
			this.Clear();
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnSoulLvlChange));
		}

		public void Clear()
		{
			this.playerSoulLevelDetail.Clear();
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnSoulLvlChange));
		}

		private void OnSoulLvlChange(PoolObjHandle<ActorRoot> act, int curSoulLevel)
		{
			if (curSoulLevel > ValueProperty.GetMaxSoulLvl() || curSoulLevel == 0)
			{
				return;
			}
			if (act)
			{
				CPlayerSoulLevelStat.SoulLevelDetail soulLevelDetail = null;
				uint playerId = act.handle.TheActorMeta.PlayerId;
				bool flag = false;
				for (int i = 0; i < this.playerSoulLevelDetail.Count; i++)
				{
					if (this.playerSoulLevelDetail[i].playerId == playerId)
					{
						flag = true;
						soulLevelDetail = this.playerSoulLevelDetail[i];
						break;
					}
				}
				if (!flag)
				{
					soulLevelDetail = new CPlayerSoulLevelStat.SoulLevelDetail(playerId);
					this.playerSoulLevelDetail.Add(soulLevelDetail);
				}
				if (curSoulLevel <= ValueProperty.GetMaxSoulLvl() && curSoulLevel > 0)
				{
					soulLevelDetail.changeTime[curSoulLevel - 1] = (uint)Singleton<FrameSynchr>.instance.LogicFrameTick;
				}
			}
		}

		public uint GetPlayerLevelChangedTime(uint playerId, uint soulLevel)
		{
			for (int i = 0; i < this.playerSoulLevelDetail.Count; i++)
			{
				if (this.playerSoulLevelDetail[i].playerId == playerId && (ulong)soulLevel <= (ulong)((long)ValueProperty.GetMaxSoulLvl()) && soulLevel > 0u)
				{
					return this.playerSoulLevelDetail[i].changeTime[(int)((UIntPtr)(soulLevel - 1u))];
				}
			}
			return 0u;
		}

		public int GetPlayerSoulLevelAtTime(uint playerID, int time)
		{
			CPlayerSoulLevelStat.SoulLevelDetail soulLevelDetail = null;
			for (int i = 0; i < this.playerSoulLevelDetail.Count; i++)
			{
				if (this.playerSoulLevelDetail[i].playerId == playerID)
				{
					soulLevelDetail = this.playerSoulLevelDetail[i];
				}
			}
			int result = 0;
			if (soulLevelDetail != null)
			{
				for (int j = 0; j < soulLevelDetail.changeTime.Length; j++)
				{
					if ((ulong)soulLevelDetail.changeTime[j] > (ulong)((long)time))
					{
						break;
					}
					result = j + 1;
				}
			}
			return result;
		}
	}
}
