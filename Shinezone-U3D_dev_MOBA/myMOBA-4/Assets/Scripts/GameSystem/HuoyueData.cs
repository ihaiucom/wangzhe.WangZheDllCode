using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class HuoyueData
	{
		public List<ushort> have_get_list_day = new List<ushort>();

		public List<ushort> have_get_list_week = new List<ushort>();

		public uint day_curNum;

		public uint week_curNum;

		public DictionaryView<ushort, CUseable> useable_cfg = new DictionaryView<ushort, CUseable>();

		public List<ushort> day_huoyue_list = new List<ushort>();

		public ushort week_reward1;

		public ushort week_reward2;

		public uint week_reward1_cost;

		public uint week_reward2_cost;

		public uint max_dayhuoyue_num;

		public void Clear()
		{
			this.day_curNum = (this.week_curNum = 0u);
			this.have_get_list_day.Clear();
			this.have_get_list_week.Clear();
		}

		public ResHuoYueDuReward GetRewardCfg(ushort id)
		{
			ResHuoYueDuReward result;
			GameDataMgr.huoyueduDict.TryGetValue(id, out result);
			return result;
		}

		public CUseable GetUsable(ushort id)
		{
			CUseable cUseable = null;
			this.useable_cfg.TryGetValue(id, out cUseable);
			if (cUseable == null)
			{
				ResHuoYueDuReward rewardCfg = this.GetRewardCfg(id);
				ResDT_HuoYueDuReward_PeriodInfo resDT_HuoYueDuReward_PeriodInfo = this.IsInTime(rewardCfg);
				if (resDT_HuoYueDuReward_PeriodInfo == null)
				{
					cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, rewardCfg.dwRewardID, 0);
				}
				else
				{
					cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, resDT_HuoYueDuReward_PeriodInfo.dwRewardID, (int)resDT_HuoYueDuReward_PeriodInfo.dwRewardNum);
				}
			}
			return cUseable;
		}

		public ResDT_HuoYueDuReward_PeriodInfo IsInTime(ResHuoYueDuReward cfg)
		{
			for (int i = 0; i < cfg.astPeriodInfo.Length; i++)
			{
				ResDT_HuoYueDuReward_PeriodInfo resDT_HuoYueDuReward_PeriodInfo = cfg.astPeriodInfo[i];
				int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
				if ((long)currentUTCTime >= (long)((ulong)resDT_HuoYueDuReward_PeriodInfo.dwStartTimeGen) && (long)currentUTCTime <= (long)((ulong)resDT_HuoYueDuReward_PeriodInfo.dwEndTimeGen))
				{
					return resDT_HuoYueDuReward_PeriodInfo;
				}
			}
			return null;
		}

		public void ParseHuoyuedu(ResHuoYueDuReward Cfg)
		{
			if (Cfg == null)
			{
				return;
			}
			if (Cfg.bHuoYueDuType == 1 && !this.day_huoyue_list.Contains(Cfg.wID))
			{
				this.day_huoyue_list.Add(Cfg.wID);
			}
			if (Cfg.bHuoYueDuType == 2)
			{
				if (this.week_reward1 == 0)
				{
					this.week_reward1 = Cfg.wID;
					this.week_reward1_cost = Cfg.dwHuoYueDu;
				}
				else if (this.week_reward2 == 0)
				{
					this.week_reward2 = Cfg.wID;
					this.week_reward2_cost = Cfg.dwHuoYueDu;
				}
			}
			this.max_dayhuoyue_num = this.Calc_Day_Max_Num();
		}

		private uint Calc_Day_Max_Num()
		{
			uint num = 0u;
			for (int i = 0; i < this.day_huoyue_list.Count; i++)
			{
				ResHuoYueDuReward resHuoYueDuReward = null;
				GameDataMgr.huoyueduDict.TryGetValue(this.day_huoyue_list[i], out resHuoYueDuReward);
				if (resHuoYueDuReward != null && resHuoYueDuReward.dwHuoYueDu > num)
				{
					num = resHuoYueDuReward.dwHuoYueDu;
				}
			}
			return num;
		}

		public void Set(RES_HUOYUEDU_TYPE type, uint num, int length, ushort[] ary)
		{
			if (type == RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY)
			{
				this.day_curNum = num;
			}
			else if (type == RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_WEEK)
			{
				this.week_curNum = num;
			}
			List<ushort> list = (type != RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY) ? this.have_get_list_week : this.have_get_list_day;
			list.Clear();
			for (int i = 0; i < length; i++)
			{
				list.Add(ary[i]);
			}
			this.PrintInfo(type);
		}

		public void Get_Reward(RES_HUOYUEDU_TYPE type, ushort id)
		{
			List<ushort> list = (type != RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY) ? this.have_get_list_week : this.have_get_list_day;
			DebugHelper.Assert(!list.Contains(id));
			list.Add(id);
		}

		public void PrintInfo(RES_HUOYUEDU_TYPE type)
		{
			List<ushort> list = (type != RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY) ? this.have_get_list_week : this.have_get_list_day;
			uint num = (type != RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY) ? this.week_curNum : this.day_curNum;
			string arg = string.Concat(new object[]
			{
				"---ctask 活跃度数据: type:",
				type,
				",值:",
				num,
				",已领取奖励: "
			});
			for (int i = 0; i < list.Count; i++)
			{
				arg = arg + list[i] + ", ";
			}
		}

		public bool BAlready_Reward(RES_HUOYUEDU_TYPE type, ushort id)
		{
			List<ushort> list = (type != RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY) ? this.have_get_list_week : this.have_get_list_day;
			return list.Contains(id);
		}
	}
}
