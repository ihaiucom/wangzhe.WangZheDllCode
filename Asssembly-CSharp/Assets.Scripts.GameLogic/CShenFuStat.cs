using Assets.Scripts.Framework;
using CSProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class CShenFuStat
	{
		public struct ShenFuRecord
		{
			public COM_PLAYERCAMP playerCamp;

			public uint playerId;

			public uint shenFuId;

			public uint onEffectTime;

			public ShenFuRecord(COM_PLAYERCAMP playerCamp, uint playerId, uint shenFuId, uint onEffectTime)
			{
				this.playerCamp = playerCamp;
				this.playerId = playerId;
				this.shenFuId = shenFuId;
				this.onEffectTime = onEffectTime;
			}
		}

		private List<CShenFuStat.ShenFuRecord> m_recordList = new List<CShenFuStat.ShenFuRecord>();

		public void StartRecord()
		{
			this.Clear();
			Singleton<EventRouter>.instance.AddEventHandler<COM_PLAYERCAMP, uint, uint>(EventID.BATTLE_SHENFU_EFFECT_CHANGED, new Action<COM_PLAYERCAMP, uint, uint>(this.OnShenFuEffect));
		}

		public void Clear()
		{
			this.m_recordList.Clear();
			Singleton<EventRouter>.instance.RemoveEventHandler<COM_PLAYERCAMP, uint, uint>(EventID.BATTLE_SHENFU_EFFECT_CHANGED, new Action<COM_PLAYERCAMP, uint, uint>(this.OnShenFuEffect));
		}

		private void OnShenFuEffect(COM_PLAYERCAMP playerCamp, uint playerId, uint shenFuId)
		{
			this.m_recordList.Add(new CShenFuStat.ShenFuRecord(playerCamp, playerId, shenFuId, (uint)Singleton<FrameSynchr>.instance.LogicFrameTick));
		}

		public List<CShenFuStat.ShenFuRecord> GetShenFuRecord(uint playerId)
		{
			List<CShenFuStat.ShenFuRecord> list = new List<CShenFuStat.ShenFuRecord>();
			for (int i = 0; i < this.m_recordList.get_Count(); i++)
			{
				if (this.m_recordList.get_Item(i).playerId == playerId)
				{
					list.Add(this.m_recordList.get_Item(i));
				}
			}
			return list;
		}

		public List<CShenFuStat.ShenFuRecord> GetShenFuRecord(uint playerId, uint shenFuId)
		{
			List<CShenFuStat.ShenFuRecord> list = new List<CShenFuStat.ShenFuRecord>();
			for (int i = 0; i < this.m_recordList.get_Count(); i++)
			{
				if (this.m_recordList.get_Item(i).playerId == playerId && this.m_recordList.get_Item(i).shenFuId == shenFuId)
				{
					list.Add(this.m_recordList.get_Item(i));
				}
			}
			return list;
		}

		public List<CShenFuStat.ShenFuRecord> GetShenFuRecord(COM_PLAYERCAMP playerCamp)
		{
			List<CShenFuStat.ShenFuRecord> list = new List<CShenFuStat.ShenFuRecord>();
			for (int i = 0; i < this.m_recordList.get_Count(); i++)
			{
				if (this.m_recordList.get_Item(i).playerCamp == playerCamp)
				{
					list.Add(this.m_recordList.get_Item(i));
				}
			}
			return list;
		}

		public List<CShenFuStat.ShenFuRecord> GetShenFuRecord(COM_PLAYERCAMP playerCamp, uint shenFuId)
		{
			List<CShenFuStat.ShenFuRecord> list = new List<CShenFuStat.ShenFuRecord>();
			for (int i = 0; i < this.m_recordList.get_Count(); i++)
			{
				if (this.m_recordList.get_Item(i).playerCamp == playerCamp && this.m_recordList.get_Item(i).shenFuId == shenFuId)
				{
					list.Add(this.m_recordList.get_Item(i));
				}
			}
			return list;
		}
	}
}
