using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class CFriendRecruit
	{
		public enum RewardState
		{
			Normal,
			Keling,
			Getted
		}

		public class RecruitReward
		{
			public ushort rewardID;

			public bool bActive;

			public ResRecruitmentReward cfg;

			public CFriendRecruit.RewardState state;

			public RecruitReward(ushort rewardId, CFriendRecruit.RewardState state)
			{
				this.rewardID = rewardId;
				this.state = state;
				this.cfg = Singleton<CFriendContoller>.instance.model.friendRecruit.GetCfgReward(rewardId);
			}

			public void Clear()
			{
				this.cfg = null;
			}
		}

		public class RecruitData
		{
			public COM_RECRUITMENT_TYPE type;

			private COMDT_FRIEND_INFO _userInfo;

			public ListView<CFriendRecruit.RecruitReward> RewardList = new ListView<CFriendRecruit.RecruitReward>();

			public COMDT_FRIEND_INFO userInfo
			{
				get
				{
					return this._userInfo;
				}
				set
				{
					this._userInfo = value;
					Singleton<EventRouter>.GetInstance().BroadCastEvent("FRDataChange");
				}
			}

			public ulong ullUid
			{
				get
				{
					return (this.userInfo != null) ? this.userInfo.stUin.ullUid : 0uL;
				}
			}

			public uint dwLogicWorldId
			{
				get
				{
					return (this.userInfo != null) ? this.userInfo.stUin.dwLogicWorldId : 0u;
				}
			}

			public RecruitData(COMDT_FRIEND_INFO userInfo, COM_RECRUITMENT_TYPE type)
			{
				this.userInfo = userInfo;
				this.type = type;
			}

			public void Clear()
			{
				this._userInfo = null;
				for (int i = 0; i < this.RewardList.Count; i++)
				{
					CFriendRecruit.RecruitReward recruitReward = this.RewardList[i];
					if (recruitReward != null)
					{
						recruitReward.Clear();
					}
				}
				this.RewardList.Clear();
			}

			public bool IsEqual(ulong ullUid, uint dwLogicWorldId)
			{
				return this.userInfo != null && this.userInfo.stUin.ullUid == ullUid && this.userInfo.stUin.dwLogicWorldId == dwLogicWorldId;
			}

			public void ResetReward()
			{
				for (int i = 0; i < this.RewardList.Count; i++)
				{
					CFriendRecruit.RecruitReward recruitReward = this.RewardList[i];
					recruitReward.state = CFriendRecruit.RewardState.Normal;
				}
				Singleton<EventRouter>.GetInstance().BroadCastEvent("FRDataChange");
			}

			public void SetReward(ushort rewardID, CFriendRecruit.RewardState state)
			{
				CFriendRecruit.RecruitReward reward = this.GetReward(rewardID);
				if (reward == null)
				{
					this.RewardList.Add(new CFriendRecruit.RecruitReward(rewardID, state));
				}
				else
				{
					reward.state = state;
				}
				Singleton<EventRouter>.GetInstance().BroadCastEvent("FRDataChange");
			}

			public CFriendRecruit.RecruitReward GetReward(ushort rewardID)
			{
				return CFriendRecruit.GetReward(rewardID, this.RewardList);
			}

			public bool IsGetAllReward()
			{
				for (int i = 0; i < this.RewardList.Count; i++)
				{
					CFriendRecruit.RecruitReward recruitReward = this.RewardList[i];
					if (recruitReward != null && recruitReward.state != CFriendRecruit.RewardState.Getted && recruitReward.state != CFriendRecruit.RewardState.Keling)
					{
						return false;
					}
				}
				return true;
			}
		}

		public static string DataChange = "FriendRecruitDataChange";

		private ListView<CFriendRecruit.RecruitData> m_zhaoMuZhe = new ListView<CFriendRecruit.RecruitData>();

		private CFriendRecruit.RecruitData m_beiZhaoMuZhe;

		private uint m_dwRecruiterRewardBits;

		private ListView<ResRecruitmentReward> m_rewardConfig = new ListView<ResRecruitmentReward>();

		public static int Max_ZhaoMuzheCount = 4;

		public static int Max_ZhaoMuzheRewarCount = 3;

		public static int Max_BeiZhaoMuZheCount = 1;

		public static int Max_BeiZhaoMuZheRewardCount = 4;

		public DictionaryView<ushort, CUseable> useable_cfg = new DictionaryView<ushort, CUseable>();

		public CFriendRecruit.RecruitReward SuperReward
		{
			get;
			private set;
		}

		public void Clear()
		{
		}

		public void SetRecruiterRewardBits(uint m_dwRecruiterRewardBits)
		{
			this.m_dwRecruiterRewardBits = m_dwRecruiterRewardBits;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			for (int i = 0; i < this.m_beiZhaoMuZhe.RewardList.Count; i++)
			{
				CFriendRecruit.RecruitReward recruitReward = this.m_beiZhaoMuZhe.RewardList[i];
				if (this.HasGetBeiZhaoMuZheReward((int)recruitReward.cfg.bRewardBit))
				{
					recruitReward.state = CFriendRecruit.RewardState.Getted;
				}
				else if (this.m_beiZhaoMuZhe.userInfo != null && masterRoleInfo != null && masterRoleInfo.PvpLevel >= recruitReward.cfg.dwLevel)
				{
					recruitReward.state = CFriendRecruit.RewardState.Keling;
				}
				else
				{
					recruitReward.state = CFriendRecruit.RewardState.Normal;
				}
			}
		}

		public void SetBITS(RES_RECRUIMENT_BITS type, bool bGetted)
		{
			if (bGetted)
			{
				uint num = 1u << (int)type;
				this.m_dwRecruiterRewardBits |= num;
			}
			else
			{
				uint num2 = 1u << (int)type;
				num2 = ~num2;
				this.m_dwRecruiterRewardBits &= num2;
			}
		}

		public bool HasGetBeiZhaoMuZheReward(RES_RECRUIMENT_BITS type)
		{
			if (type == RES_RECRUIMENT_BITS.RES_RECRUIMENT_BITS_NONE)
			{
				return false;
			}
			uint num = 1u << (int)type;
			return (num & this.m_dwRecruiterRewardBits) != 0u;
		}

		public bool HasGetBeiZhaoMuZheReward(int v)
		{
			return this.HasGetBeiZhaoMuZheReward((RES_RECRUIMENT_BITS)v);
		}

		public void LoadConfig()
		{
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.recruimentReward.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.Current;
				ResRecruitmentReward resRecruitmentReward = (ResRecruitmentReward)current.Value;
				if (!this.m_rewardConfig.Contains(resRecruitmentReward))
				{
					this.m_rewardConfig.Add(resRecruitmentReward);
				}
				if (resRecruitmentReward.bRecruimentType == 1 && resRecruitmentReward.bRewardBit == 5)
				{
					this.SuperReward = null;
					this.SuperReward = new CFriendRecruit.RecruitReward(resRecruitmentReward.wID, CFriendRecruit.RewardState.Normal);
				}
			}
			this.InitData();
			for (int i = 0; i < this.m_zhaoMuZhe.Count; i++)
			{
				CFriendRecruit.RecruitData data = this.m_zhaoMuZhe[i];
				this.ParseConfig(data);
			}
			this.ParseConfig(this.m_beiZhaoMuZhe);
		}

		public void InitData()
		{
			for (int i = 0; i < this.m_zhaoMuZhe.Count; i++)
			{
				CFriendRecruit.RecruitData recruitData = this.m_zhaoMuZhe[i];
				if (recruitData != null)
				{
					recruitData.Clear();
				}
			}
			this.m_zhaoMuZhe.Clear();
			if (this.m_beiZhaoMuZhe != null)
			{
				this.m_beiZhaoMuZhe.Clear();
			}
			this.m_beiZhaoMuZhe = null;
			for (int j = 0; j < CFriendRecruit.Max_ZhaoMuzheCount; j++)
			{
				this.m_zhaoMuZhe.Add(new CFriendRecruit.RecruitData(null, COM_RECRUITMENT_TYPE.COM_RECRUITMENT_ACTIVE));
			}
			this.m_beiZhaoMuZhe = new CFriendRecruit.RecruitData(null, COM_RECRUITMENT_TYPE.COM_RECRUITMENT_PASSIVE);
		}

		public void ParseFriend(COMDT_FRIEND_INFO stFriendInfo, COMDT_RECRUITMENT_DATA stRecruitmentInfo)
		{
			if (stFriendInfo == null || stRecruitmentInfo == null)
			{
				return;
			}
			if (stRecruitmentInfo.bRecruitmentType == 1)
			{
				COMDT_RECRUITMENT_ACTIVE stRecruitmentActive = stRecruitmentInfo.stRecruitmentInfo.stRecruitmentActive;
				if (this.GetZhaoMuZhe(stFriendInfo.stUin.ullUid, stFriendInfo.stUin.dwLogicWorldId) == null)
				{
					this.SetZhaoMuZhe(stFriendInfo);
				}
				int num = 0;
				while ((long)num < (long)((ulong)stRecruitmentActive.dwActiveRewardNum))
				{
					ushort rewardID = stRecruitmentActive.ActiveRewardList[num];
					this.SetZhaoMuZheRewardData(stFriendInfo, rewardID, CFriendRecruit.RewardState.Getted);
					num++;
				}
			}
			else if (stRecruitmentInfo.bRecruitmentType == 2)
			{
				this.m_beiZhaoMuZhe.userInfo = stFriendInfo;
			}
		}

		public void Check()
		{
			for (int i = 0; i < this.m_zhaoMuZhe.Count; i++)
			{
				CFriendRecruit.RecruitData data = this.m_zhaoMuZhe[i];
				this.CheckCanGetReward(data);
			}
			this.CheckBeiZhaoMuZheReward();
			if (this.GetZhaoMuZhe_RewardProgress() == this.GetZhaoMuZhe_RewardTotalCount())
			{
				this.SuperReward.state = CFriendRecruit.RewardState.Keling;
			}
		}

		public void ParseConfig(CFriendRecruit.RecruitData data)
		{
			for (int i = 0; i < this.m_rewardConfig.Count; i++)
			{
				ResRecruitmentReward resRecruitmentReward = this.m_rewardConfig[i];
				if ((byte)data.type == resRecruitmentReward.bRecruimentType)
				{
					CFriendRecruit.RecruitReward reward = data.GetReward(resRecruitmentReward.wID);
					if (reward == null || reward.state != CFriendRecruit.RewardState.Getted)
					{
						data.SetReward(resRecruitmentReward.wID, CFriendRecruit.RewardState.Normal);
					}
				}
			}
		}

		public void CheckCanGetReward(CFriendRecruit.RecruitData data)
		{
			for (int i = 0; i < data.RewardList.Count; i++)
			{
				CFriendRecruit.RecruitReward recruitReward = data.RewardList[i];
				if (recruitReward.state == CFriendRecruit.RewardState.Normal)
				{
					ResRecruitmentReward cfgReward = this.GetCfgReward(recruitReward.rewardID);
					if (cfgReward != null && data.userInfo != null && data.userInfo.dwPvpLvl >= cfgReward.dwLevel)
					{
						recruitReward.state = CFriendRecruit.RewardState.Keling;
					}
				}
			}
		}

		public void CheckBeiZhaoMuZheReward()
		{
			if (this.m_beiZhaoMuZhe == null || this.m_beiZhaoMuZhe.userInfo == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			for (int i = 0; i < this.m_beiZhaoMuZhe.RewardList.Count; i++)
			{
				CFriendRecruit.RecruitReward recruitReward = this.m_beiZhaoMuZhe.RewardList[i];
				if (recruitReward.state == CFriendRecruit.RewardState.Normal)
				{
					ResRecruitmentReward cfgReward = this.GetCfgReward(recruitReward.rewardID);
					if (cfgReward != null && masterRoleInfo != null && masterRoleInfo.PvpLevel >= cfgReward.dwLevel)
					{
						recruitReward.state = CFriendRecruit.RewardState.Keling;
					}
				}
			}
		}

		public ResRecruitmentReward GetCfgReward(ushort rewardID)
		{
			for (int i = 0; i < this.m_rewardConfig.Count; i++)
			{
				ResRecruitmentReward resRecruitmentReward = this.m_rewardConfig[i];
				if (resRecruitmentReward.wID == rewardID)
				{
					return resRecruitmentReward;
				}
			}
			return null;
		}

		public void SetZhaoMuZheRewardData(COMDT_FRIEND_INFO friendData, ushort rewardID, CFriendRecruit.RewardState state)
		{
			CFriendRecruit.RecruitData zhaoMuZhe = this.GetZhaoMuZhe(friendData.stUin.ullUid, friendData.stUin.dwLogicWorldId);
			if (zhaoMuZhe != null)
			{
				zhaoMuZhe.SetReward(rewardID, state);
			}
			else
			{
				CFriendRecruit.RecruitData validRecruitData = this.GetValidRecruitData();
				if (validRecruitData != null)
				{
					validRecruitData.userInfo = friendData;
					validRecruitData.SetReward(rewardID, state);
				}
			}
		}

		public CFriendRecruit.RecruitData GetValidRecruitData()
		{
			for (int i = 0; i < this.m_zhaoMuZhe.Count; i++)
			{
				CFriendRecruit.RecruitData recruitData = this.m_zhaoMuZhe[i];
				if (recruitData != null && recruitData.userInfo == null)
				{
					return recruitData;
				}
			}
			return null;
		}

		public void SetZhaoMuZhe(COMDT_FRIEND_INFO info)
		{
			CFriendRecruit.RecruitData zhaoMuZhe = this.GetZhaoMuZhe(info.stUin.ullUid, info.stUin.dwLogicWorldId);
			if (zhaoMuZhe != null)
			{
				zhaoMuZhe.userInfo = info;
				return;
			}
			CFriendRecruit.RecruitData validRecruitData = this.GetValidRecruitData();
			if (validRecruitData != null)
			{
				validRecruitData.userInfo = info;
			}
		}

		public CFriendRecruit.RecruitReward GetRecruitReward(ulong ullUid, uint dwLogicWorldId, ushort rewardID)
		{
			CFriendRecruit.RecruitData zhaoMuZhe = this.GetZhaoMuZhe(ullUid, dwLogicWorldId);
			if (zhaoMuZhe != null)
			{
				return zhaoMuZhe.GetReward(rewardID);
			}
			if (this.m_beiZhaoMuZhe.IsEqual(ullUid, dwLogicWorldId))
			{
				return this.m_beiZhaoMuZhe.GetReward(rewardID);
			}
			return null;
		}

		public CFriendRecruit.RecruitData GetZhaoMuZhe(ulong ullUid, uint dwLogicWorldId)
		{
			for (int i = 0; i < this.m_zhaoMuZhe.Count; i++)
			{
				CFriendRecruit.RecruitData recruitData = this.m_zhaoMuZhe[i];
				if (recruitData.IsEqual(ullUid, dwLogicWorldId))
				{
					return recruitData;
				}
			}
			return null;
		}

		public CFriendRecruit.RecruitData GetRecruitData(ulong ullUid, uint dwLogicWorldId)
		{
			CFriendRecruit.RecruitData zhaoMuZhe = this.GetZhaoMuZhe(ullUid, dwLogicWorldId);
			if (zhaoMuZhe != null)
			{
				return zhaoMuZhe;
			}
			if (this.m_beiZhaoMuZhe.IsEqual(ullUid, dwLogicWorldId))
			{
				return this.m_beiZhaoMuZhe;
			}
			return null;
		}

		public void RemoveRecruitData(ulong ullUid, uint dwLogicWorldId)
		{
			CFriendRecruit.RecruitData recruitData = this.GetRecruitData(ullUid, dwLogicWorldId);
			if (recruitData != null)
			{
				recruitData.ResetReward();
				recruitData.userInfo = null;
			}
		}

		public void SetBeiZhaoMuZheRewardData(COMDT_FRIEND_INFO friendData)
		{
			this.m_beiZhaoMuZhe.userInfo = friendData;
		}

		public ListView<CFriendRecruit.RecruitData> GetZhaoMuZheRewardList()
		{
			return this.m_zhaoMuZhe;
		}

		public CFriendRecruit.RecruitData GetBeiZhaoMuZhe()
		{
			return this.m_beiZhaoMuZhe;
		}

		public static CFriendRecruit.RecruitReward GetReward(ushort rewardID, ListView<CFriendRecruit.RecruitReward> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				CFriendRecruit.RecruitReward recruitReward = list[i];
				if (recruitReward != null && recruitReward.rewardID == rewardID)
				{
					return recruitReward;
				}
			}
			return null;
		}

		public int GetZhaoMuZhe_RewardExp()
		{
			ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(306u);
			return (int)((dataByKey == null) ? 4294967295u : (dataByKey.dwConfValue / 100u));
		}

		public int GetZhaoMuZhe_RewardGold()
		{
			ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(307u);
			return (int)((dataByKey == null) ? 4294967295u : (dataByKey.dwConfValue / 100u));
		}

		public int GetBeiZhaoMuZhe_RewardExp()
		{
			ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(306u);
			return (int)((dataByKey == null) ? 4294967295u : (dataByKey.dwConfValue / 100u));
		}

		public int GetBeiZhaoMuZhe_RewardGold()
		{
			ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(307u);
			return (int)((dataByKey == null) ? 4294967295u : (dataByKey.dwConfValue / 100u));
		}

		public int GetZhaoMuZhe_RewardProgress()
		{
			int num = 0;
			for (int i = 0; i < this.m_zhaoMuZhe.Count; i++)
			{
				CFriendRecruit.RecruitData recruitData = this.m_zhaoMuZhe[i];
				if (recruitData != null && recruitData.IsGetAllReward())
				{
					num++;
				}
			}
			return num;
		}

		public int GetZhaoMuZhe_RewardTotalCount()
		{
			return 4;
		}

		public CUseable GetUsable(ushort id)
		{
			CUseable cUseable = null;
			this.useable_cfg.TryGetValue(id, out cUseable);
			if (cUseable == null)
			{
				ResRecruitmentReward cfgReward = this.GetCfgReward(id);
				cUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, cfgReward.dwRewardID, 0);
				this.useable_cfg.Add(id, cUseable);
			}
			return cUseable;
		}
	}
}
