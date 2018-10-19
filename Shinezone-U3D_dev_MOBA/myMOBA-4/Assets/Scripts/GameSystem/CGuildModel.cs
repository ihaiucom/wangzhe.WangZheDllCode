using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Assets.Scripts.GameSystem
{
	public class CGuildModel : Singleton<CGuildModel>
	{
		public const string APPLIED_GUILD_DIC_BIN_FILE_PRE = "applyed_guild_02_";

		public const string INVITED_FRIEND_DIC_BIN_FILE_PRE = "invited_friend_02_";

		public const byte REQUEST_PREPARE_GUILD_LIST_CACHE_TIME = 10;

		public const byte REQUEST_GUILD_LIST_CACHE_TIME = 10;

		public const byte REQUEST_APPLICANT_LIST_CACHE_TIME = 10;

		public const int STATE_SYNC_INTERVAL = 5000;

		public const int RankpointRankTimeLimit = 300;

		public int m_LastPrepareGuildListRequestTime;

		public int m_LastGuildListRequestTime;

		public int m_LastApplicantListRequestTime;

		public bool m_IsInited;

		private DictionaryView<ulong, GuildInfo> m_GuildDic;

		private DictionaryView<ulong, PrepareGuildInfo> m_PrepareGuildDic;

		private Dictionary<ulong, stAppliedGuildInfo> m_AppliedGuildDic;

		private bool m_IsLocalDataInited;

		private Dictionary<ulong, stApplicantInfo> m_ApplicantDic;

		private Dictionary<ulong, stInvitedFriend> m_InvitedFriendDic;

		private PrepareGuildInfo m_CurrentPrepareGuildInfo;

		private GuildInfo m_CurrentGuildInfo;

		public ulong m_InvitePlayerUuid;

		public ulong m_InviteGuildUuid;

		public int m_InviteGuildLogicWorldId;

		public string m_InvitGuildName;

		public COM_PLAYER_GUILD_STATE m_PlayerGuildLastState;

		public int m_LastRequestJoinPrepareGuildTime;

		public int m_LastRequestJoinGuildTime;

		public int m_RequestJoinGuildCnt;

		public uint m_PrepareGuildOldestRequestTime;

		private Dictionary<ulong, stRecommendInfo> m_recommendInfoDic;

		private Dictionary<ulong, int> m_recommendTimeInfoDic;

		private Dictionary<ulong, int> m_inviteTimeInfoDic;

		public bool m_isApplyListReceived;

		public bool m_isRecommendListReceived;

		public CGuildSystem.enPlatformGroupStatus PlatformGroupStatus = CGuildSystem.enPlatformGroupStatus.Resolve;

		public bool IsSelfInPlatformGroup;

		public bool IsLocalDataInited
		{
			get
			{
				return this.m_IsLocalDataInited;
			}
			set
			{
				this.m_IsLocalDataInited = value;
			}
		}

		public PrepareGuildInfo CurrentPrepareGuildInfo
		{
			get
			{
				return this.m_CurrentPrepareGuildInfo;
			}
			set
			{
				this.m_CurrentPrepareGuildInfo = value;
			}
		}

		public GuildInfo CurrentGuildInfo
		{
			get
			{
				return this.m_CurrentGuildInfo;
			}
			set
			{
				this.m_CurrentGuildInfo = value;
			}
		}

		public RES_GUILD_DONATE_TYPE CurrentDonateType
		{
			get;
			set;
		}

		public List<KeyValuePair<ulong, MemberRankInfo>> RankpointMemberInfoList
		{
			get;
			set;
		}

		public ListView<RankpointRankInfo>[] RankpointRankInfoLists
		{
			get;
			set;
		}

		public bool[] RankpointRankGottens
		{
			get;
			set;
		}

		public int[] RankpointRankLastGottenTimes
		{
			get;
			set;
		}

		public int RequestApplyListPageId
		{
			get;
			set;
		}

		public int RequestRecommendListPageId
		{
			get;
			set;
		}

		public GuildMemInfo CurrentSelectedMemberInfo
		{
			get;
			set;
		}

		public CGuildModel()
		{
			this.m_CurrentPrepareGuildInfo = new PrepareGuildInfo();
			this.m_CurrentGuildInfo = new GuildInfo();
			this.m_PlayerGuildLastState = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL;
			this.m_PrepareGuildOldestRequestTime = 4294967295u;
			this.m_GuildDic = new DictionaryView<ulong, GuildInfo>();
			this.m_PrepareGuildDic = new DictionaryView<ulong, PrepareGuildInfo>();
			this.m_ApplicantDic = new Dictionary<ulong, stApplicantInfo>();
			this.m_AppliedGuildDic = new Dictionary<ulong, stAppliedGuildInfo>();
			this.m_InvitedFriendDic = new Dictionary<ulong, stInvitedFriend>();
			this.m_IsLocalDataInited = false;
			this.m_IsInited = false;
			this.m_InvitePlayerUuid = 0uL;
			this.m_InviteGuildUuid = 0uL;
			this.m_InviteGuildLogicWorldId = 0;
			this.m_InvitGuildName = null;
			this.m_recommendInfoDic = new Dictionary<ulong, stRecommendInfo>();
			this.m_recommendTimeInfoDic = new Dictionary<ulong, int>();
			this.m_inviteTimeInfoDic = new Dictionary<ulong, int>();
			this.RankpointMemberInfoList = new List<KeyValuePair<ulong, MemberRankInfo>>();
			this.RankpointRankInfoLists = new ListView<RankpointRankInfo>[4];
			for (int i = 0; i < this.RankpointRankInfoLists.Length; i++)
			{
				this.RankpointRankInfoLists[i] = new ListView<RankpointRankInfo>();
			}
			this.RankpointRankGottens = new bool[4];
			this.RankpointRankLastGottenTimes = new int[4];
		}

		public void ResetCurrentPrepareGuildInfo()
		{
			if (this.m_CurrentPrepareGuildInfo != null)
			{
				this.m_CurrentPrepareGuildInfo.Reset();
			}
		}

		public void ResetCurrentGuildInfo()
		{
			if (this.m_CurrentGuildInfo != null)
			{
				this.m_CurrentGuildInfo.Reset();
			}
		}

		public void SetPlatformGroupStatus(CGuildSystem.enPlatformGroupStatus status, bool isSelfInPlatformGroup = false)
		{
			this.PlatformGroupStatus = status;
			this.IsSelfInPlatformGroup = isSelfInPlatformGroup;
			if (Singleton<CGuildSystem>.GetInstance().IsOpenPlatformGroupFunc())
			{
				Singleton<EventRouter>.GetInstance().BroadCastEvent<CGuildSystem.enPlatformGroupStatus, bool>("Guild_PlatformGroup_Status_Change", this.PlatformGroupStatus, this.IsSelfInPlatformGroup);
			}
		}

		public void InitLoginData()
		{
			if (this.m_IsInited)
			{
				return;
			}
			this.m_PlayerGuildLastState = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState;
			this.m_IsInited = true;
		}

		private void WriteAppliedGuildDicToBinFile()
		{
			string cachePath = CFileManager.GetCachePath("applyed_guild_02_" + Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID);
			FileStream fileStream = null;
			try
			{
				if (!CFileManager.IsFileExist(cachePath))
				{
					fileStream = new FileStream(cachePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
				}
				else
				{
					fileStream = new FileStream(cachePath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
				}
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(fileStream, this.m_AppliedGuildDic);
				fileStream.Flush();
				fileStream.Close();
				fileStream.Dispose();
			}
			catch (Exception var_3_74)
			{
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream.Dispose();
				}
			}
		}

		private void ReadAppliedGuildDicFromBinFile()
		{
			string cachePath = CFileManager.GetCachePath("applyed_guild_02_" + Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID);
			FileStream fileStream = null;
			if (!CFileManager.IsFileExist(cachePath))
			{
				return;
			}
			try
			{
				fileStream = new FileStream(cachePath, FileMode.Open, FileAccess.Read, FileShare.None);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				this.m_AppliedGuildDic = (Dictionary<ulong, stAppliedGuildInfo>)binaryFormatter.Deserialize(fileStream);
				fileStream.Close();
				fileStream.Dispose();
			}
			catch (Exception var_3_65)
			{
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream.Dispose();
				}
			}
		}

		private void WriteInvitedFriendDicToBinFile()
		{
			string cachePath = CFileManager.GetCachePath("invited_friend_02_" + Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID);
			FileStream fileStream = null;
			try
			{
				if (!CFileManager.IsFileExist(cachePath))
				{
					fileStream = new FileStream(cachePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
				}
				else
				{
					fileStream = new FileStream(cachePath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
				}
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(fileStream, this.m_InvitedFriendDic);
				fileStream.Flush();
				fileStream.Close();
				fileStream.Dispose();
			}
			catch (Exception var_3_74)
			{
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream.Dispose();
				}
			}
		}

		private void ReadInvitedFriendDicFromBinFile()
		{
			string cachePath = CFileManager.GetCachePath("invited_friend_02_" + Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID);
			FileStream fileStream = null;
			if (!CFileManager.IsFileExist(cachePath))
			{
				return;
			}
			try
			{
				fileStream = new FileStream(cachePath, FileMode.Open, FileAccess.Read, FileShare.None);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				this.m_InvitedFriendDic = (Dictionary<ulong, stInvitedFriend>)binaryFormatter.Deserialize(fileStream);
				fileStream.Close();
				fileStream.Dispose();
			}
			catch (Exception var_3_65)
			{
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream.Dispose();
				}
			}
		}

		private void InitLocalData()
		{
			this.ReadAppliedGuildDicFromBinFile();
			this.ReadInvitedFriendDicFromBinFile();
			this.m_IsLocalDataInited = true;
		}

		public void UpdateGuildState(int seq)
		{
			Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = this.m_PlayerGuildLastState;
		}

		public void SetPlayerGuildStateToTemp()
		{
			Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_WAIT_RESULT;
			Singleton<CTimerManager>.GetInstance().AddTimer(5000, 1, new CTimer.OnTimeUpHandler(this.UpdateGuildState));
		}

		public bool IsInGuildStep()
		{
			if (!this.m_IsInited)
			{
				this.InitLoginData();
			}
			return Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState != COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL;
		}

		public void AddGuildInfo(GuildInfo info)
		{
			if (!this.m_GuildDic.ContainsKey(info.briefInfo.uulUid))
			{
				this.m_GuildDic.Add(info.briefInfo.uulUid, info);
			}
		}

		public void AddGuildInfoList(ListView<GuildInfo> guildInfoList)
		{
			int count = guildInfoList.Count;
			for (int i = 0; i < count; i++)
			{
				this.AddGuildInfo(guildInfoList[i]);
			}
		}

		public void ClearGuildInfoList()
		{
			this.m_GuildDic.Clear();
		}

		public int GetGuildInfoCount()
		{
			return this.m_GuildDic.Count;
		}

		public GuildInfo GetGuildInfoByIndex(int idx)
		{
			if (idx >= 0 && idx < this.m_GuildDic.Count)
			{
				int num = 0;
				DictionaryView<ulong, GuildInfo>.Enumerator enumerator = this.m_GuildDic.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (num == idx)
					{
						KeyValuePair<ulong, GuildInfo> current = enumerator.Current;
						return current.Value;
					}
					num++;
				}
			}
			return null;
		}

		public void AddPrepareGuildInfo(PrepareGuildInfo info)
		{
			try
			{
				this.m_PrepareGuildDic.Add(info.stBriefInfo.uulUid, info);
			}
			catch (Exception)
			{
			}
		}

		public void AddPrepareGuildInfoList(ListView<PrepareGuildInfo> prepareGuildInfoList)
		{
			int i = 0;
			int count = prepareGuildInfoList.Count;
			while (i < count)
			{
				this.AddPrepareGuildInfo(prepareGuildInfoList[i]);
				i++;
			}
		}

		public void ClearPrepareGuildInfoList()
		{
			this.m_PrepareGuildDic.Clear();
		}

		public int GetPrepareGuildInfoCount()
		{
			return this.m_PrepareGuildDic.Count;
		}

		public PrepareGuildInfo GetPrepareGuildInfoByIndex(int idx)
		{
			if (idx >= 0 && idx < this.m_PrepareGuildDic.Count)
			{
				int num = 0;
				DictionaryView<ulong, PrepareGuildInfo>.Enumerator enumerator = this.m_PrepareGuildDic.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (num == idx)
					{
						KeyValuePair<ulong, PrepareGuildInfo> current = enumerator.Current;
						return current.Value;
					}
					num++;
				}
			}
			return null;
		}

		public byte AddAppliedGuildInfo(stAppliedGuildInfo info, bool persist = true)
		{
			byte result;
			try
			{
				this.m_AppliedGuildDic.Add(info.stBriefInfo.uulUid, info);
				if (persist)
				{
					this.WriteAppliedGuildDicToBinFile();
				}
				result = 1;
			}
			catch (Exception)
			{
				result = 0;
			}
			return result;
		}

		public void RemoveAppliedGuildInfo(ulong guildUid)
		{
			this.m_AppliedGuildDic.Remove(guildUid);
			this.WriteAppliedGuildDicToBinFile();
		}

		public Dictionary<ulong, stAppliedGuildInfo> GetAppliedGuildDic()
		{
			if (!this.m_IsInited)
			{
				this.InitLoginData();
			}
			if (!this.m_IsLocalDataInited)
			{
				this.InitLocalData();
			}
			return this.m_AppliedGuildDic;
		}

		public stAppliedGuildInfo GetAppliedGuildInfoByUid(ulong uulUid)
		{
			if (!this.m_IsInited)
			{
				this.InitLoginData();
			}
			if (!this.m_IsLocalDataInited)
			{
				this.InitLocalData();
			}
			stAppliedGuildInfo result = default(stAppliedGuildInfo);
			uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey(9u).dwConfValue;
			int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
			if (!this.m_AppliedGuildDic.ContainsKey(uulUid))
			{
				return result;
			}
			this.m_AppliedGuildDic.TryGetValue(uulUid, out result);
			if (result.stBriefInfo.uulUid == 0uL)
			{
				return result;
			}
			if ((ulong)(result.dwApplyTime + dwConfValue) < (ulong)((long)currentUTCTime))
			{
				this.m_AppliedGuildDic.Remove(result.stBriefInfo.uulUid);
				this.WriteAppliedGuildDicToBinFile();
				return default(stAppliedGuildInfo);
			}
			return result;
		}

		public void ClearAppliedGuildDic()
		{
			if (this.m_AppliedGuildDic != null)
			{
				this.m_AppliedGuildDic.Clear();
			}
		}

		public List<stApplicantInfo> GetApplicants()
		{
			List<stApplicantInfo> list = new List<stApplicantInfo>(this.m_ApplicantDic.Count);
			Dictionary<ulong, stApplicantInfo>.Enumerator enumerator = this.m_ApplicantDic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				List<stApplicantInfo> arg_32_0 = list;
				KeyValuePair<ulong, stApplicantInfo> current = enumerator.Current;
				arg_32_0.Add(current.Value);
			}
			return list;
		}

		public int GetApplicantsCount()
		{
			return (this.m_ApplicantDic != null) ? this.m_ApplicantDic.Count : 0;
		}

		public stApplicantInfo GetApplicantByUid(ulong uid)
		{
			stApplicantInfo result = default(stApplicantInfo);
			if (this.m_ApplicantDic.ContainsKey(uid))
			{
				this.m_ApplicantDic.TryGetValue(uid, out result);
			}
			return result;
		}

		public void AddApplicants(List<stApplicantInfo> applicants)
		{
			int count = applicants.Count;
			for (int i = 0; i < count; i++)
			{
				this.AddApplicant(applicants[i]);
			}
		}

		public void AddApplicant(stApplicantInfo applicant)
		{
			if (!this.m_ApplicantDic.ContainsKey(applicant.stBriefInfo.uulUid))
			{
				try
				{
					this.m_ApplicantDic.Add(applicant.stBriefInfo.uulUid, applicant);
				}
				catch (Exception)
				{
				}
			}
		}

		public void RemoveApplicant(ulong uulUid)
		{
			if (this.m_ApplicantDic.ContainsKey(uulUid))
			{
				try
				{
					this.m_ApplicantDic.Remove(uulUid);
				}
				catch (Exception)
				{
				}
				return;
			}
		}

		public byte AddInvitedFriend(stInvitedFriend friend, bool persist = true)
		{
			if (!this.m_InvitedFriendDic.ContainsKey(friend.uulUid))
			{
				try
				{
					this.m_InvitedFriendDic.Add(friend.uulUid, friend);
					if (persist)
					{
						this.WriteInvitedFriendDicToBinFile();
					}
					byte result = 1;
					return result;
				}
				catch (Exception)
				{
					byte result = 0;
					return result;
				}
				return 0;
			}
			return 0;
		}

		public Dictionary<ulong, stInvitedFriend> GetInvitedFriendDic()
		{
			if (!this.m_IsInited)
			{
				this.InitLoginData();
			}
			if (!this.m_IsLocalDataInited)
			{
				this.InitLocalData();
			}
			return this.m_InvitedFriendDic;
		}

		public stInvitedFriend GetInvitedFriendByUid(ulong uulUid)
		{
			if (!this.m_IsInited)
			{
				this.InitLoginData();
			}
			if (!this.m_IsLocalDataInited)
			{
				this.InitLocalData();
			}
			stInvitedFriend result = default(stInvitedFriend);
			if (this.m_InvitedFriendDic.ContainsKey(uulUid))
			{
				this.m_InvitedFriendDic.TryGetValue(uulUid, out result);
			}
			return result;
		}

		public void ClearInvitedFriendDic()
		{
			if (this.m_InvitedFriendDic != null)
			{
				this.m_InvitedFriendDic.Clear();
			}
		}

		public void AddRecommendInfo(stRecommendInfo info)
		{
			if (!this.m_recommendInfoDic.ContainsKey(info.uid))
			{
				this.m_recommendInfoDic.Add(info.uid, info);
			}
		}

		public void AddRecommendTimeInfo(ulong uid, int recommendTime)
		{
			if (!this.m_recommendTimeInfoDic.ContainsKey(uid))
			{
				this.m_recommendTimeInfoDic.Add(uid, recommendTime);
			}
		}

		public void ClearRecommendTimeInfo()
		{
			if (this.m_recommendTimeInfoDic != null)
			{
				this.m_recommendTimeInfoDic.Clear();
			}
		}

		public void AddInviteTimeInfo(ulong uid, int inviteTime)
		{
			if (!this.m_inviteTimeInfoDic.ContainsKey(uid))
			{
				this.m_inviteTimeInfoDic.Add(uid, inviteTime);
			}
		}

		public void ClearInviteTimeInfo()
		{
			if (this.m_inviteTimeInfoDic != null)
			{
				this.m_inviteTimeInfoDic.Clear();
			}
		}

		public void AddRecommendInfoList(List<stRecommendInfo> infoList)
		{
			for (int i = 0; i < infoList.Count; i++)
			{
				this.AddRecommendInfo(infoList[i]);
			}
		}

		public void RemoveRecommendInfo(ulong uulUid)
		{
			if (this.m_recommendInfoDic.ContainsKey(uulUid))
			{
				this.m_recommendInfoDic.Remove(uulUid);
			}
		}

		public List<stRecommendInfo> GetRecommendInfo()
		{
			List<stRecommendInfo> list = new List<stRecommendInfo>(this.m_recommendInfoDic.Count);
			Dictionary<ulong, stRecommendInfo>.Enumerator enumerator = this.m_recommendInfoDic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				List<stRecommendInfo> arg_32_0 = list;
				KeyValuePair<ulong, stRecommendInfo> current = enumerator.Current;
				arg_32_0.Add(current.Value);
			}
			return list;
		}

		public int GetRecommendTimeInfoByUid(ulong uid)
		{
			return (!this.m_recommendTimeInfoDic.ContainsKey(uid)) ? -1 : this.m_recommendTimeInfoDic[uid];
		}

		public int GetInviteTimeInfoByUid(ulong uid)
		{
			return (!this.m_inviteTimeInfoDic.ContainsKey(uid)) ? -1 : this.m_inviteTimeInfoDic[uid];
		}

		public int GetRecommendInfoCount()
		{
			return (this.m_recommendInfoDic != null) ? this.m_recommendInfoDic.Count : 0;
		}

		public stRecommendInfo GetRecommendInfoByUid(ulong uulUid)
		{
			stRecommendInfo result = default(stRecommendInfo);
			if (this.m_recommendInfoDic.ContainsKey(uulUid))
			{
				this.m_recommendInfoDic.TryGetValue(uulUid, out result);
				return result;
			}
			return result;
		}

		public GuildMemInfo GetGuildMemberInfoByUid(ulong uid)
		{
			for (int i = 0; i < this.CurrentGuildInfo.listMemInfo.Count; i++)
			{
				if (this.CurrentGuildInfo.listMemInfo[i].stBriefInfo.uulUid == uid)
				{
					return this.CurrentGuildInfo.listMemInfo[i];
				}
			}
			return null;
		}

		public GuildMemInfo GetGuildMemberInfoByName(string name)
		{
			for (int i = 0; i < this.CurrentGuildInfo.listMemInfo.Count; i++)
			{
				if (this.CurrentGuildInfo.listMemInfo[i].stBriefInfo.sName == name)
				{
					return this.CurrentGuildInfo.listMemInfo[i];
				}
			}
			return null;
		}

		public GuildMemInfo GetPlayerGuildMemberInfo()
		{
			for (int i = 0; i < this.CurrentGuildInfo.listMemInfo.Count; i++)
			{
				if (this.CurrentGuildInfo.listMemInfo[i].stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
				{
					return this.CurrentGuildInfo.listMemInfo[i];
				}
			}
			return null;
		}

		public void SetGuildMatchMemberInfo(SCPKG_GUILD_MATCH_MEMBER_CHG_NTF ntf)
		{
			for (int i = 0; i < (int)ntf.bCnt; i++)
			{
				for (int j = 0; j < this.CurrentGuildInfo.listMemInfo.Count; j++)
				{
					if (this.CurrentGuildInfo.listMemInfo[j].stBriefInfo.uulUid == ntf.astChgInfo[i].ullUid)
					{
						this.CurrentGuildInfo.listMemInfo[j].GuildMatchInfo.ullTeamLeaderUid = ntf.astChgInfo[i].ullTeamLeaderUid;
						this.CurrentGuildInfo.listMemInfo[j].GuildMatchInfo.bContinueWin = ntf.astChgInfo[i].bContinueWin;
						this.CurrentGuildInfo.listMemInfo[j].GuildMatchInfo.bIsReady = ntf.astChgInfo[i].bIsReady;
					}
				}
			}
		}

		public void SetGuildMatchMemberInfo(SCPKG_CHG_GUILD_MATCH_LEADER_NTF ntf)
		{
			for (int i = 0; i < this.CurrentGuildInfo.listMemInfo.Count; i++)
			{
				if (this.CurrentGuildInfo.listMemInfo[i].stBriefInfo.uulUid == ntf.ullUid)
				{
					this.CurrentGuildInfo.listMemInfo[i].GuildMatchInfo.bIsLeader = ntf.bIsLeader;
					this.CurrentGuildInfo.listMemInfo[i].GuildMatchInfo.ullTeamLeaderUid = ntf.ullTeamLeaderUid;
					this.CurrentGuildInfo.listMemInfo[i].GuildMatchInfo.bContinueWin = ntf.bContinueWin;
					return;
				}
			}
		}

		public void SetGuildMatchMemberReadyState(SCPKG_SET_GUILD_MATCH_READY_NTF ntf)
		{
			for (int i = 0; i < (int)ntf.bCnt; i++)
			{
				for (int j = 0; j < this.CurrentGuildInfo.listMemInfo.Count; j++)
				{
					if (this.CurrentGuildInfo.listMemInfo[j].stBriefInfo.uulUid == ntf.astInfo[i].ullUid)
					{
						this.CurrentGuildInfo.listMemInfo[j].GuildMatchInfo.bIsReady = ntf.astInfo[i].bIsReady;
					}
				}
			}
		}

		public void SetGuildMatchScore(SCPKG_GUILD_MATCH_SCORE_CHG_NTF ntf)
		{
			this.CurrentGuildInfo.GuildMatchInfo.dwScore = ntf.dwGuildScore;
			this.CurrentGuildInfo.GuildMatchInfo.dwWeekScore = ntf.dwGuildWeekScore;
			this.CurrentGuildInfo.RankInfo.totalRankPoint = ntf.dwTotalRankPoint;
			for (int i = 0; i < ntf.astChgInfo.Length; i++)
			{
				for (int j = 0; j < this.CurrentGuildInfo.listMemInfo.Count; j++)
				{
					if (this.CurrentGuildInfo.listMemInfo[j].stBriefInfo.uulUid == ntf.astChgInfo[i].ullUid)
					{
						this.CurrentGuildInfo.listMemInfo[j].GuildMatchInfo.dwScore = ntf.astChgInfo[i].dwScore;
						this.CurrentGuildInfo.listMemInfo[j].GuildMatchInfo.dwWeekScore = ntf.astChgInfo[i].dwWeekScore;
						this.CurrentGuildInfo.listMemInfo[j].GuildMatchInfo.bContinueWin = ntf.astChgInfo[i].bContinueWin;
						this.CurrentGuildInfo.listMemInfo[j].GuildMatchInfo.bWeekMatchCnt = ntf.astChgInfo[i].bWeekMatchCnt;
					}
				}
			}
		}
	}
}
