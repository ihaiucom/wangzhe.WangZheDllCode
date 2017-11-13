using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class CAchieveInfo2
	{
		public ListView<CAchieveItem2> m_Trophies;

		public ListView<CAchieveItem2> m_AchiveItems;

		public DictionaryView<uint, CAchieveItem2> m_AchiveItemDic;

		public uint TrophyRewardInfoArrCnt;

		public CTrophyRewardInfo[] TrophyRewardInfoArr;

		public CTrophyRewardInfo LastDoneTrophyRewardInfo;

		public DictionaryView<uint, CTrophyRewardInfo> TrophyRewardDic;

		public List<uint> MostLatelyDoneAchievements;

		public CAchieveItem2[] SelectedTrophies;

		private int m_logicWorldID;

		private ulong m_playerUUID;

		private static CAchieveInfo2 _masterAchieveInfo;

		private static CAchieveInfo2 _otherAchieveInfo2;

		private static Dictionary<string, uint> worldRanks = new Dictionary<string, uint>();

		private CAchieveInfo2(int logicWorldID, ulong playerUUID)
		{
			this.m_Trophies = new ListView<CAchieveItem2>();
			this.m_AchiveItems = new ListView<CAchieveItem2>();
			this.m_AchiveItemDic = new DictionaryView<uint, CAchieveItem2>();
			this.TrophyRewardInfoArrCnt = 0u;
			this.TrophyRewardInfoArr = new CTrophyRewardInfo[100];
			this.LastDoneTrophyRewardInfo = null;
			this.TrophyRewardDic = new DictionaryView<uint, CTrophyRewardInfo>();
			this.MostLatelyDoneAchievements = new List<uint>();
			this.SelectedTrophies = new CAchieveItem2[3];
			this.m_logicWorldID = logicWorldID;
			this.m_playerUUID = playerUUID;
			this.InitLocalData();
		}

		private void InitLocalData()
		{
			GameDataMgr.achieveDatabin.Reload();
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.achieveDatabin.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.get_Current();
				ResAchievement resAchievement = current.get_Value() as ResAchievement;
				CAchieveItem2 cAchieveItem = new CAchieveItem2(ref resAchievement);
				this.m_AchiveItems.Add(cAchieveItem);
				if (!this.m_AchiveItemDic.ContainsKey(cAchieveItem.ID))
				{
					this.m_AchiveItemDic.Add(cAchieveItem.ID, cAchieveItem);
				}
			}
			this.Classify();
			this.InitLocalTrophy();
		}

		public void ClearData()
		{
			this.LastDoneTrophyRewardInfo = null;
			this.m_AchiveItems.Clear();
			this.m_AchiveItemDic.Clear();
			this.m_Trophies.Clear();
			this.MostLatelyDoneAchievements.Clear();
			Array.Clear(this.SelectedTrophies, 0, this.SelectedTrophies.Length);
			this.TrophyRewardDic.Clear();
			Array.Clear(this.TrophyRewardInfoArr, 0, this.TrophyRewardInfoArr.Length);
		}

		public static void Clear()
		{
			if (CAchieveInfo2._masterAchieveInfo != null)
			{
				CAchieveInfo2._masterAchieveInfo.ClearData();
				CAchieveInfo2._masterAchieveInfo = null;
			}
			if (CAchieveInfo2._otherAchieveInfo2 != null)
			{
				CAchieveInfo2._otherAchieveInfo2.ClearData();
				CAchieveInfo2._otherAchieveInfo2 = null;
			}
			CAchieveInfo2.worldRanks.Clear();
		}

		public static CAchieveInfo2 GetMasterAchieveInfo()
		{
			if (CAchieveInfo2._masterAchieveInfo != null)
			{
				return CAchieveInfo2._masterAchieveInfo;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "GetMasterAchieveInfo::Master Role Info Is Null");
				return CAchieveInfo2.GetAchieveInfo(0, 0uL, true);
			}
			return CAchieveInfo2.GetAchieveInfo(masterRoleInfo.logicWorldID, masterRoleInfo.playerUllUID, true);
		}

		public static CAchieveInfo2 GetAchieveInfo(int logicWorldID, ulong playerUUID, bool isMaster = false)
		{
			if (CAchieveInfo2._masterAchieveInfo != null && CAchieveInfo2._masterAchieveInfo.m_logicWorldID == logicWorldID && CAchieveInfo2._masterAchieveInfo.m_playerUUID == playerUUID)
			{
				isMaster = true;
			}
			if (isMaster)
			{
				if (CAchieveInfo2._masterAchieveInfo == null)
				{
					CAchieveInfo2._masterAchieveInfo = new CAchieveInfo2(logicWorldID, playerUUID);
				}
				return CAchieveInfo2._masterAchieveInfo;
			}
			if (CAchieveInfo2._otherAchieveInfo2 == null)
			{
				CAchieveInfo2._otherAchieveInfo2 = new CAchieveInfo2(logicWorldID, playerUUID);
				return CAchieveInfo2._otherAchieveInfo2;
			}
			if (CAchieveInfo2._otherAchieveInfo2.m_logicWorldID == logicWorldID && CAchieveInfo2._otherAchieveInfo2.m_playerUUID == playerUUID)
			{
				return CAchieveInfo2._otherAchieveInfo2;
			}
			CAchieveInfo2._otherAchieveInfo2 = new CAchieveInfo2(logicWorldID, playerUUID);
			return CAchieveInfo2._otherAchieveInfo2;
		}

		public static void AddWorldRank(int logicWorldID, ulong playerUUID, uint rank)
		{
			string text = string.Format("{0}_{1}", logicWorldID, playerUUID);
			if (CAchieveInfo2.worldRanks.ContainsKey(text))
			{
				CAchieveInfo2.worldRanks.set_Item(text, rank);
			}
			else
			{
				CAchieveInfo2.worldRanks.Add(text, rank);
			}
		}

		public uint GetWorldRank()
		{
			uint result = 0u;
			string text = string.Format("{0}_{1}", this.m_logicWorldID, this.m_playerUUID);
			if (CAchieveInfo2.worldRanks.ContainsKey(text))
			{
				CAchieveInfo2.worldRanks.TryGetValue(text, ref result);
			}
			return result;
		}

		public void OnServerAchieveInfo(ref COMDT_ACHIEVEMENT_INFO svrAchieveInfo)
		{
			int[] array = new int[61];
			int num = 0;
			while ((long)num < (long)((ulong)svrAchieveInfo.dwDoneTypeNum))
			{
				uint dwDoneType = svrAchieveInfo.astDoneData[num].dwDoneType;
				array[(int)((uint)((UIntPtr)dwDoneType))] = svrAchieveInfo.astDoneData[num].iDoneCnt;
				num++;
			}
			num = 0;
			while ((long)num < (long)((ulong)svrAchieveInfo.dwAchievementNum))
			{
				uint dwID = svrAchieveInfo.astAchievementData[num].dwID;
				uint dwDoneTime = svrAchieveInfo.astAchievementData[num].dwDoneTime;
				COM_ACHIEVEMENT_STATE bState = (COM_ACHIEVEMENT_STATE)svrAchieveInfo.astAchievementData[num].bState;
				if (this.m_AchiveItemDic.ContainsKey(dwID))
				{
					this.m_AchiveItemDic[dwID].DoneTime = dwDoneTime;
					this.m_AchiveItemDic[dwID].State = bState;
					if (this.m_AchiveItemDic[dwID].DoneType >= RES_ACHIEVE_DONE_TYPE.RES_ACHIEVE_DONE_GET_GOLD && this.m_AchiveItemDic[dwID].DoneType < (RES_ACHIEVE_DONE_TYPE)array.Length)
					{
						this.m_AchiveItemDic[dwID].DoneCnt = array[(int)this.m_AchiveItemDic[dwID].DoneType];
					}
				}
				num++;
			}
			this.OnServerTrophy(ref svrAchieveInfo.stTrophyLvlInfo);
			this.SetSelectedTrophies(ref svrAchieveInfo.ShowAchievement);
		}

		public void OnServerAchieveInfo(CSDT_SHOWACHIEVE_DETAIL[] selectedTrophyDetailss, uint trophyPoints)
		{
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.achieveDatabin.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.get_Current();
				ResAchievement resAchievement = current.get_Value() as ResAchievement;
				CAchieveItem2 cAchieveItem = new CAchieveItem2(ref resAchievement);
				this.m_AchiveItems.Add(cAchieveItem);
				if (!this.m_AchiveItemDic.ContainsKey(cAchieveItem.ID))
				{
					this.m_AchiveItemDic.Add(cAchieveItem.ID, cAchieveItem);
				}
			}
			this.OnServerTrophy(trophyPoints);
			this.SetSelectedTrophies(ref selectedTrophyDetailss, true);
		}

		private void InitLocalTrophy()
		{
			GameDataMgr.trophyDatabin.Reload();
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.trophyDatabin.GetEnumerator();
			uint num = 0u;
			uint minPoint = 0u;
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.get_Current();
				ResTrophyLvl resTrophyLvl = current.get_Value() as ResTrophyLvl;
				if (resTrophyLvl != null && (ulong)num < (ulong)((long)this.TrophyRewardInfoArr.Length))
				{
					this.TrophyRewardInfoArr[(int)((uint)((UIntPtr)num))] = new CTrophyRewardInfo(resTrophyLvl, TrophyState.UnFinish, (int)num, minPoint);
					minPoint = this.TrophyRewardInfoArr[(int)((uint)((UIntPtr)num))].MaxPoint;
				}
				if (!this.TrophyRewardDic.ContainsKey(this.TrophyRewardInfoArr[(int)((uint)((UIntPtr)num))].Cfg.dwTrophyLvl))
				{
					this.TrophyRewardDic.Add(this.TrophyRewardInfoArr[(int)((uint)((UIntPtr)num))].Cfg.dwTrophyLvl, this.TrophyRewardInfoArr[(int)((uint)((UIntPtr)num))]);
				}
				num += 1u;
			}
			this.TrophyRewardInfoArrCnt = num;
		}

		private void OnServerTrophy(uint trophyPoints)
		{
			int num = 0;
			while ((long)num < (long)((ulong)this.TrophyRewardInfoArrCnt))
			{
				if (trophyPoints < this.TrophyRewardInfoArr[num].MaxPoint)
				{
					break;
				}
				this.TrophyRewardInfoArr[num].State = TrophyState.Finished;
				this.LastDoneTrophyRewardInfo = this.TrophyRewardInfoArr[num];
				num++;
			}
		}

		private void OnServerTrophy(ref COMDT_TROPHY_INFO TrophyRewardsInfo)
		{
			this.TrophyRewardInfoArrCnt = Math.Min(this.TrophyRewardInfoArrCnt, 100u);
			if (this.TrophyRewardInfoArrCnt == 0u)
			{
				DebugHelper.Assert(false, "成就系统荣耀奖励配置为空!");
				return;
			}
			uint totalDonePoints = this.GetTotalDonePoints();
			int num = 0;
			int num2 = 0;
			while ((long)num < (long)((ulong)this.TrophyRewardInfoArrCnt))
			{
				if (num < (int)TrophyRewardsInfo.bAlreadyGetRewardNoGapLvl)
				{
					this.TrophyRewardInfoArr[num].State = TrophyState.GotRewards;
				}
				else if (num >= (int)TrophyRewardsInfo.bAlreadyGetRewardNoGapLvl && num < (int)(TrophyRewardsInfo.bAlreadyGetRewardNoGapLvl + TrophyRewardsInfo.bNum) && num2 < (int)TrophyRewardsInfo.bNum)
				{
					this.TrophyRewardInfoArr[num].State = ((TrophyRewardsInfo.szTrophyStateArray[num2] == 1) ? TrophyState.Finished : TrophyState.GotRewards);
					num2++;
				}
				else if (totalDonePoints > this.TrophyRewardInfoArr[num].MinPoint && totalDonePoints < this.TrophyRewardInfoArr[num].MaxPoint)
				{
					this.TrophyRewardInfoArr[num].State = TrophyState.OnGoing;
				}
				else if (totalDonePoints >= this.TrophyRewardInfoArr[num].MaxPoint)
				{
					this.TrophyRewardInfoArr[num].State = TrophyState.Finished;
				}
				else if (totalDonePoints <= this.TrophyRewardInfoArr[num].MinPoint)
				{
					this.TrophyRewardInfoArr[num].State = TrophyState.UnFinish;
				}
				if (this.TrophyRewardInfoArr[num].IsFinish())
				{
					this.LastDoneTrophyRewardInfo = this.TrophyRewardInfoArr[num];
				}
				num++;
			}
		}

		private void Classify()
		{
			for (int i = 0; i < this.m_AchiveItems.Count; i++)
			{
				CAchieveItem2 cAchieveItem = this.m_AchiveItems[i];
				uint prevID = cAchieveItem.PrevID;
				if (this.m_AchiveItemDic.ContainsKey(prevID))
				{
					this.m_AchiveItemDic[prevID].Next = cAchieveItem;
					cAchieveItem.Prev = this.m_AchiveItemDic[prevID];
				}
				else
				{
					this.m_Trophies.Add(cAchieveItem);
				}
			}
		}

		private void SetSelectedTrophies(ref CSDT_SHOWACHIEVE_DETAIL[] serverSelectedTrophies, bool setFinishRecursively = false)
		{
			for (int i = 0; i < this.SelectedTrophies.Length; i++)
			{
				if (this.m_AchiveItemDic.ContainsKey(serverSelectedTrophies[i].dwAchieveID))
				{
					CAchieveItem2 cAchieveItem;
					this.m_AchiveItemDic.TryGetValue(serverSelectedTrophies[i].dwAchieveID, out cAchieveItem);
					if (cAchieveItem != null)
					{
						if (setFinishRecursively)
						{
							cAchieveItem = cAchieveItem.GetHeadAndSetFinishRecursively(serverSelectedTrophies[i].dwDoneTime);
						}
						else
						{
							cAchieveItem = cAchieveItem.GetHead();
						}
						this.SelectedTrophies[i] = cAchieveItem;
					}
				}
				else
				{
					this.SelectedTrophies[i] = null;
				}
			}
		}

		private void SetSelectedTrophies(ref uint[] serverSelectedTrophies)
		{
			for (int i = 0; i < this.SelectedTrophies.Length; i++)
			{
				if (this.m_AchiveItemDic.ContainsKey(serverSelectedTrophies[i]))
				{
					CAchieveItem2 head;
					this.m_AchiveItemDic.TryGetValue(serverSelectedTrophies[i], out head);
					if (head != null)
					{
						head = head.GetHead();
						this.SelectedTrophies[i] = head;
					}
				}
				else
				{
					this.SelectedTrophies[i] = null;
				}
			}
		}

		public ListView<CTrophyRewardInfo> GetTrophyRewardInfoWithRewards()
		{
			ListView<CTrophyRewardInfo> listView = new ListView<CTrophyRewardInfo>();
			int num = 0;
			while ((long)num < (long)((ulong)this.TrophyRewardInfoArrCnt))
			{
				if (this.TrophyRewardInfoArr[num] != null && this.TrophyRewardInfoArr[num].IsRewardConfiged())
				{
					listView.Add(this.TrophyRewardInfoArr[num]);
				}
				num++;
			}
			return listView;
		}

		public CTrophyRewardInfo GetTrophyRewardInfoByIndex(int index)
		{
			if (index >= 0 && (long)index < (long)((ulong)this.TrophyRewardInfoArrCnt))
			{
				return this.TrophyRewardInfoArr[index];
			}
			if ((long)index >= (long)((ulong)this.TrophyRewardInfoArrCnt))
			{
				return this.TrophyRewardInfoArr[index - 1];
			}
			return this.TrophyRewardInfoArr[0];
		}

		public CTrophyRewardInfo GetTrophyRewardInfoByPoint(uint point)
		{
			for (int i = (int)(this.TrophyRewardInfoArrCnt - 1u); i >= 0; i--)
			{
				if (this.TrophyRewardInfoArr[i] != null && this.TrophyRewardInfoArr[i].MaxPoint <= point)
				{
					return this.TrophyRewardInfoArr[i];
				}
			}
			return null;
		}

		public uint GetTotalDonePoints()
		{
			uint num = 0u;
			for (int i = 0; i < this.m_Trophies.Count; i++)
			{
				if (this.m_Trophies[i] != null)
				{
					num += this.m_Trophies[i].GetTotalDonePoints();
				}
			}
			return num;
		}

		public bool HasRewardNotGot()
		{
			int num = 0;
			while ((long)num < (long)((ulong)this.TrophyRewardInfoArrCnt))
			{
				if (this.TrophyRewardInfoArr[num] != null && this.TrophyRewardInfoArr[num].State == TrophyState.Finished && this.TrophyRewardInfoArr[num].IsRewardConfiged())
				{
					return true;
				}
				num++;
			}
			return false;
		}

		public ListView<CAchieveItem2> GetTrophies(enTrophyState state)
		{
			ListView<CAchieveItem2> listView = new ListView<CAchieveItem2>();
			switch (state)
			{
			case enTrophyState.All:
				for (int i = 0; i < this.m_Trophies.Count; i++)
				{
					CAchieveItem2 item = this.m_Trophies[i];
					listView.Add(item);
				}
				break;
			case enTrophyState.Finish:
				for (int j = 0; j < this.m_Trophies.Count; j++)
				{
					CAchieveItem2 cAchieveItem = this.m_Trophies[j];
					if (cAchieveItem.IsFinish())
					{
						listView.Add(cAchieveItem);
					}
				}
				break;
			case enTrophyState.UnFinish:
				for (int k = 0; k < this.m_Trophies.Count; k++)
				{
					CAchieveItem2 cAchieveItem2 = this.m_Trophies[k];
					if (!cAchieveItem2.IsFinish())
					{
						listView.Add(cAchieveItem2);
					}
				}
				break;
			}
			return listView;
		}

		public void GetTrophyProgress(ref uint cur, ref uint next)
		{
			cur = this.GetTotalDonePoints();
			if (this.LastDoneTrophyRewardInfo == null)
			{
				if (this.TrophyRewardInfoArrCnt > 0u)
				{
					next = this.TrophyRewardInfoArr[0].MaxPoint;
				}
				else
				{
					next = 0u;
				}
			}
			else
			{
				uint num = this.LastDoneTrophyRewardInfo.Cfg.dwTrophyLvl;
				while (this.TrophyRewardDic.ContainsKey(num))
				{
					CTrophyRewardInfo cTrophyRewardInfo = this.TrophyRewardDic[num];
					if (cur < cTrophyRewardInfo.MaxPoint)
					{
						next = cTrophyRewardInfo.MaxPoint;
						return;
					}
					num += 1u;
				}
				next = this.LastDoneTrophyRewardInfo.MaxPoint;
			}
		}

		public CTrophyRewardInfo GetFirstTrophyRewardInfoAwardNotGot()
		{
			if (this.LastDoneTrophyRewardInfo == null)
			{
				return null;
			}
			ListView<CTrophyRewardInfo> trophyRewardInfoWithRewards = this.GetTrophyRewardInfoWithRewards();
			for (int i = 0; i < trophyRewardInfoWithRewards.Count; i++)
			{
				CTrophyRewardInfo cTrophyRewardInfo = trophyRewardInfoWithRewards[i];
				if (!cTrophyRewardInfo.HasGotAward())
				{
					return cTrophyRewardInfo;
				}
			}
			return null;
		}

		public void TrophyLevelUp(uint oldLevel, uint newLevel)
		{
			for (uint num = oldLevel + 1u; num <= newLevel; num += 1u)
			{
				if (this.TrophyRewardDic.ContainsKey(num))
				{
					this.LastDoneTrophyRewardInfo = this.TrophyRewardDic[num];
					this.LastDoneTrophyRewardInfo.State = TrophyState.Finished;
				}
			}
			uint key = newLevel + 1u;
			if (this.TrophyRewardDic.ContainsKey(key))
			{
				this.TrophyRewardDic[key].State = TrophyState.OnGoing;
			}
		}

		public void ChangeAchieveState(ref COMDT_ACHIEVEMENT_DATA data)
		{
			if (this.m_AchiveItemDic.ContainsKey(data.dwID))
			{
				this.m_AchiveItemDic[data.dwID].State = (COM_ACHIEVEMENT_STATE)data.bState;
				this.m_AchiveItemDic[data.dwID].DoneTime = data.dwDoneTime;
				this.MostLatelyDoneAchievements.Add(data.dwID);
			}
		}

		public void OnAchieveDoneDataChange(COMDT_ACHIEVEMENT_DONE_DATA doneData)
		{
			RES_ACHIEVE_DONE_TYPE dwDoneType = (RES_ACHIEVE_DONE_TYPE)doneData.dwDoneType;
			int num = (this.m_AchiveItems == null) ? 0 : this.m_AchiveItems.Count;
			for (int i = 0; i < num; i++)
			{
				if (this.m_AchiveItems != null && this.m_AchiveItems[i].DoneType == dwDoneType)
				{
					this.m_AchiveItems[i].DoneCnt = doneData.iDoneCnt;
				}
			}
		}
	}
}
