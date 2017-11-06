using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CFriendModel
	{
		public enum FriendType
		{
			GameFriend = 1,
			RequestFriend,
			Recommend,
			SNS,
			BlackList,
			Mentor,
			Apprentice,
			MentorRecommend,
			MentorRequestList
		}

		public enum enVerifyDataSet
		{
			Friend,
			Mentor,
			Count
		}

		public struct stBlackName
		{
			public ulong ullUid;

			public uint dwLogicWorldId;

			public string name;

			public byte bIsOnline;

			public uint dwLastLoginTime;

			public byte bGender;

			public string szHeadUrl;

			public uint dwPvpLvl;

			public uint dwMentorLv;
		}

		public struct stFriendIntimacy
		{
			public ulong ullUid;

			public uint dwLogicWorldID;

			public uint dwLastIntimacyTime;

			public ushort wIntimacyValue;

			public CFriendModel.EIntimacyType type;

			public bool bFreeze;

			public byte bIntimacyType;

			public uint dwTerminateTime;

			public stFriendIntimacy(ulong ullUid, uint dwLogicWorldId, uint dwLastIntimacyTime, ushort wIntimacyValue, byte bIntimacyType, uint dwTerminateTime)
			{
				this.ullUid = ullUid;
				this.dwLogicWorldID = dwLogicWorldId;
				this.dwLastIntimacyTime = dwLastIntimacyTime;
				this.wIntimacyValue = wIntimacyValue;
				this.bIntimacyType = bIntimacyType;
				this.dwTerminateTime = dwTerminateTime;
				this.bFreeze = UT.IsFreeze(dwLastIntimacyTime);
				this.type = Singleton<CFriendContoller>.instance.model.CalcType((int)wIntimacyValue);
			}
		}

		public enum EIntimacyType
		{
			None = -1,
			Low = 2,
			Middle,
			High,
			full
		}

		public enum LBSGenderType
		{
			Both,
			Nan,
			Nv
		}

		public class FriendInGame
		{
			public ulong ullUid;

			public uint dwLogicWorldID;

			public string nickName;

			public uint startTime;

			public uint antiDisturbBits;

			public COM_ACNT_GAME_STATE State
			{
				get;
				set;
			}

			public string NickName
			{
				get
				{
					if (CFriendModel.RemarkNames.ContainsKey(this.ullUid))
					{
						string empty = string.Empty;
						CFriendModel.RemarkNames.TryGetValue(this.ullUid, ref empty);
						if (!string.IsNullOrEmpty(empty))
						{
							return empty;
						}
					}
					return this.nickName;
				}
			}

			public FriendInGame(ulong uid, uint worldID, COM_ACNT_GAME_STATE state, uint startTime, string nickName = "", uint antiDisturbBits = 0u)
			{
				this.ullUid = uid;
				this.dwLogicWorldID = worldID;
				this.State = state;
				this.nickName = CUIUtility.RemoveEmoji(nickName);
				this.startTime = startTime;
				this.antiDisturbBits = antiDisturbBits;
			}

			public bool IsUseTGA()
			{
				return (this.antiDisturbBits & 2u) > 0u;
			}
		}

		public ulong ullSvrCurSec;

		private DictionaryView<uint, ListView<COMDT_FRIEND_INFO>> _map = new DictionaryView<uint, ListView<COMDT_FRIEND_INFO>>();

		public CFriendHeartData HeartData = new CFriendHeartData();

		public CFriendReCallData SnsReCallData = new CFriendReCallData();

		private ListView<CFriendModel.FriendInGame> gameStateList = new ListView<CFriendModel.FriendInGame>();

		public static Dictionary<ulong, string> RemarkNames = new Dictionary<ulong, string>();

		public static Dictionary<stFriendByUUIDAndLogicID, bool> IsNoAskForDic = new Dictionary<stFriendByUUIDAndLogicID, bool>();

		public string Guild_Invite_txt;

		public string Guild_Has_Invited_txt;

		public string Guild_Recommend_txt;

		public string Guild_Has_Recommended_txt;

		private ListView<COMDT_FRIEND_INFO> cache_OnlineFriends_Results = new ListView<COMDT_FRIEND_INFO>();

		private ListView<COMDT_FRIEND_INFO> cache_chatFriends_Results = new ListView<COMDT_FRIEND_INFO>();

		private List<stFriendVerifyContent>[] m_friendVerifyList = new List<stFriendVerifyContent>[2];

		private List<string> m_preconfigVerifyContentList = new List<string>();

		public string friend_static_text = string.Empty;

		public FriendReserve friendReserve = new FriendReserve();

		public CFriendRecruit friendRecruit = new CFriendRecruit();

		private List<CFriendModel.stBlackName> m_blackList_friend = new List<CFriendModel.stBlackName>();

		public int freezeDayCount;

		public int cond1;

		public int cond2;

		public uint fullIntimacy;

		private List<CFriendModel.stFriendIntimacy> m_stFriendIntimacyList = new List<CFriendModel.stFriendIntimacy>();

		public CFriendRelationship FRData = new CFriendRelationship();

		public IntimacyRelationView.IntimacyTipsMgr intimacyTipsMgr = new IntimacyRelationView.IntimacyTipsMgr();

		private uint IntimacyRequestValue;

		private uint IntimacyMaxValue;

		private ListView<CSDT_LBS_USER_INFO> m_LBSList = new ListView<CSDT_LBS_USER_INFO>();

		private ListView<CSDT_LBS_USER_INFO> m_LBSListNv = new ListView<CSDT_LBS_USER_INFO>();

		private ListView<CSDT_LBS_USER_INFO> m_LBSListNan = new ListView<CSDT_LBS_USER_INFO>();

		public bool m_shareLocation;

		public string searchLBSZero = string.Empty;

		private uint _fileter = 3u;

		private ListView<COMDT_FRIEND_INFO> _rankingFriend = new ListView<COMDT_FRIEND_INFO>();

		private static COM_APOLLO_TRANK_SCORE_TYPE _lastSortTpye;

		public bool bShowUID
		{
			get;
			set;
		}

		public uint fileter
		{
			get
			{
				return this._fileter;
			}
			set
			{
				this._fileter = value;
			}
		}

		public bool EnableShareLocation
		{
			get
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
				if (masterRoleInfo == null)
				{
					return false;
				}
				uint num = 4u;
				uint num2 = masterRoleInfo.snsSwitchBits & num;
				return num2 > 0u;
			}
			set
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
				if (masterRoleInfo == null)
				{
					return;
				}
				uint num = 4u;
				if (value)
				{
					masterRoleInfo.snsSwitchBits |= num;
				}
				else
				{
					masterRoleInfo.snsSwitchBits &= ~num;
				}
			}
		}

		public CFriendModel()
		{
			Array values = Enum.GetValues(typeof(CFriendModel.FriendType));
			for (int i = 0; i < values.get_Length(); i++)
			{
				this._map.Add((uint)((int)values.GetValue(i)), new ListView<COMDT_FRIEND_INFO>());
			}
			for (int j = 0; j < 2; j++)
			{
				this.m_friendVerifyList[j] = new List<stFriendVerifyContent>();
			}
			this.Guild_Invite_txt = Singleton<CTextManager>.GetInstance().GetText("Guild_Invite");
			this.Guild_Has_Invited_txt = Singleton<CTextManager>.GetInstance().GetText("Guild_Has_Invited");
			this.Guild_Recommend_txt = Singleton<CTextManager>.GetInstance().GetText("Guild_Recommend");
			this.Guild_Has_Recommended_txt = Singleton<CTextManager>.GetInstance().GetText("Guild_Has_Recommended");
		}

		public List<CFriendModel.stBlackName> GetBlackList()
		{
			return this.m_blackList_friend;
		}

		public CFriendModel.stBlackName Convert2BlackName(COMDT_FRIEND_INFO info)
		{
			return new CFriendModel.stBlackName
			{
				ullUid = info.stUin.ullUid,
				dwLogicWorldId = info.stUin.dwLogicWorldId,
				name = UT.Bytes2String(info.szUserName),
				bIsOnline = info.bIsOnline,
				dwLastLoginTime = info.dwLastLoginTime,
				bGender = info.bGender,
				szHeadUrl = UT.Bytes2String(info.szHeadUrl),
				dwPvpLvl = info.dwPvpLvl,
				dwMentorLv = info.dwMasterLvl
			};
		}

		public CFriendModel.stBlackName Convert2BlackName(COMDT_CHAT_PLAYER_INFO info, byte bGender, uint dwLastLoginTime)
		{
			return new CFriendModel.stBlackName
			{
				ullUid = info.ullUid,
				dwLogicWorldId = (uint)info.iLogicWorldID,
				name = UT.Bytes2String(info.szName),
				bGender = bGender,
				szHeadUrl = UT.Bytes2String(info.szHeadUrl),
				dwPvpLvl = info.dwLevel,
				dwLastLoginTime = dwLastLoginTime
			};
		}

		public string GetName(ulong ullUid, uint dwLogicWorldId)
		{
			COMDT_FRIEND_INFO cOMDT_FRIEND_INFO;
			COMDT_CHAT_PLAYER_INFO cOMDT_CHAT_PLAYER_INFO;
			this.GetUser(ullUid, dwLogicWorldId, out cOMDT_FRIEND_INFO, out cOMDT_CHAT_PLAYER_INFO);
			if (cOMDT_FRIEND_INFO != null)
			{
				return UT.Bytes2String(cOMDT_FRIEND_INFO.szUserName);
			}
			if (cOMDT_CHAT_PLAYER_INFO != null)
			{
				return UT.Bytes2String(cOMDT_CHAT_PLAYER_INFO.szName);
			}
			return string.Empty;
		}

		public void GetUser(ulong ullUid, uint dwLogicWorldId, out COMDT_FRIEND_INFO friendInfo, out COMDT_CHAT_PLAYER_INFO chatInfo)
		{
			friendInfo = this.GetGameOrSnsFriend(ullUid, dwLogicWorldId);
			if (friendInfo == null)
			{
				chatInfo = Singleton<CChatController>.instance.model.Get_Palyer_Info(ullUid, dwLogicWorldId);
			}
			else
			{
				chatInfo = null;
			}
		}

		public bool IsBlack(ulong ullUid, uint dwLogicWorldId)
		{
			int friendBlackIndex = this.GetFriendBlackIndex(ullUid, dwLogicWorldId);
			return friendBlackIndex != -1;
		}

		public string GetBlackName(ulong ullUid, uint dwLogicWorldId)
		{
			for (int i = 0; i < this.m_blackList_friend.get_Count(); i++)
			{
				CFriendModel.stBlackName stBlackName = this.m_blackList_friend.get_Item(i);
				if (stBlackName.ullUid == ullUid && stBlackName.dwLogicWorldId == dwLogicWorldId)
				{
					return stBlackName.name;
				}
			}
			return string.Empty;
		}

		public void AddFriendBlack(COMDT_FRIEND_INFO info)
		{
			if (info == null)
			{
				return;
			}
			int friendBlackIndex = this.GetFriendBlackIndex(info.stUin.ullUid, info.stUin.dwLogicWorldId);
			if (friendBlackIndex != -1)
			{
				this.m_blackList_friend.RemoveAt(friendBlackIndex);
			}
			this.m_blackList_friend.Add(this.Convert2BlackName(info));
		}

		public void AddFriendBlack(COMDT_CHAT_PLAYER_INFO info, byte bGender, uint dwLastLoginTime)
		{
			if (info == null)
			{
				return;
			}
			int friendBlackIndex = this.GetFriendBlackIndex(info.ullUid, (uint)info.iLogicWorldID);
			if (friendBlackIndex != -1)
			{
				this.m_blackList_friend.RemoveAt(friendBlackIndex);
			}
			this.m_blackList_friend.Add(this.Convert2BlackName(info, bGender, dwLastLoginTime));
		}

		public void RemoveFriendBlack(COMDT_FRIEND_INFO info)
		{
			int friendBlackIndex = this.GetFriendBlackIndex(info.stUin.ullUid, info.stUin.dwLogicWorldId);
			if (friendBlackIndex != -1)
			{
				this.m_blackList_friend.RemoveAt(friendBlackIndex);
			}
		}

		public void RemoveFriendBlack(ulong ullUid, uint dwLogicWorldID)
		{
			int friendBlackIndex = this.GetFriendBlackIndex(ullUid, dwLogicWorldID);
			if (friendBlackIndex != -1)
			{
				this.m_blackList_friend.RemoveAt(friendBlackIndex);
			}
		}

		private int GetFriendBlackIndex(ulong ullUid, uint dwLogicWorldID)
		{
			for (int i = 0; i < this.m_blackList_friend.get_Count(); i++)
			{
				CFriendModel.stBlackName stBlackName = this.m_blackList_friend.get_Item(i);
				if (stBlackName.ullUid == ullUid && stBlackName.dwLogicWorldId == dwLogicWorldID)
				{
					return i;
				}
			}
			return -1;
		}

		public CFriendModel.EIntimacyType CalcType(int value)
		{
			if (value <= this.cond1)
			{
				return CFriendModel.EIntimacyType.Low;
			}
			if (value > this.cond1 && value <= this.cond2)
			{
				return CFriendModel.EIntimacyType.Middle;
			}
			if (value > this.cond2 && (long)value < (long)((ulong)this.fullIntimacy))
			{
				return CFriendModel.EIntimacyType.High;
			}
			return CFriendModel.EIntimacyType.full;
		}

		public uint GetIntimacyRequestValue()
		{
			if (this.IntimacyRequestValue == 0u)
			{
				this.IntimacyRequestValue = GameDataMgr.globalInfoDatabin.GetDataByKey(331u).dwConfValue;
			}
			return this.IntimacyRequestValue;
		}

		public uint GetIntimacyMaxValue()
		{
			if (this.IntimacyMaxValue == 0u)
			{
				this.IntimacyMaxValue = GameDataMgr.globalInfoDatabin.GetDataByKey(195u).dwConfValue;
			}
			return this.IntimacyMaxValue;
		}

		public string GetPrefixString(int value, COM_INTIMACY_STATE state)
		{
			COM_INTIMACY_STATE state2 = COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL;
			if (IntimacyRelationViewUT.IsRelaState(state))
			{
				state2 = state;
			}
			if (IntimacyRelationViewUT.IsRelaStateDeny(state))
			{
				state2 = IntimacyRelationViewUT.GetStateByDenyState(state);
			}
			RelationConfig relaTextCfg = this.FRData.GetRelaTextCfg(state2);
			if (relaTextCfg == null)
			{
				return string.Empty;
			}
			uint intimacyMaxValue = Singleton<CFriendContoller>.instance.model.GetIntimacyMaxValue();
			int nxtLevelValue = IntimacyRelationViewUT.GetNxtLevelValue(value);
			if ((long)value >= (long)((ulong)intimacyMaxValue))
			{
				return relaTextCfg.IntimRela_TypeColor_Prefix_Max;
			}
			string intimRela_TypeColor_Prefix_Normal = relaTextCfg.IntimRela_TypeColor_Prefix_Normal;
			return string.Format(intimRela_TypeColor_Prefix_Normal, value, nxtLevelValue);
		}

		public bool GetFriendIntimacy(ulong ullUid, uint dwLogicWorldID, out ushort wIntimacyValue, out CFriendModel.EIntimacyType type, out bool bFreeze)
		{
			for (int i = 0; i < this.m_stFriendIntimacyList.get_Count(); i++)
			{
				CFriendModel.stFriendIntimacy stFriendIntimacy = this.m_stFriendIntimacyList.get_Item(i);
				if (stFriendIntimacy.ullUid == ullUid && stFriendIntimacy.dwLogicWorldID == dwLogicWorldID)
				{
					wIntimacyValue = stFriendIntimacy.wIntimacyValue;
					type = stFriendIntimacy.type;
					bFreeze = stFriendIntimacy.bFreeze;
					return true;
				}
			}
			wIntimacyValue = 0;
			type = CFriendModel.EIntimacyType.None;
			bFreeze = false;
			return false;
		}

		public void AddFriendIntimacy(ulong ullUid, uint dwLogicWorldID, uint lastTime, ushort wIntimacyValue, byte bIntimacyType, uint dwTerminateTime)
		{
			int friendIntimacyIndex = this.GetFriendIntimacyIndex(ullUid, dwLogicWorldID);
			if (friendIntimacyIndex != -1)
			{
				this.m_stFriendIntimacyList.RemoveAt(friendIntimacyIndex);
			}
			this.m_stFriendIntimacyList.Add(new CFriendModel.stFriendIntimacy(ullUid, dwLogicWorldID, lastTime, wIntimacyValue, bIntimacyType, dwTerminateTime));
		}

		public void AddFriendIntimacy(COMDT_ACNT_UNIQ uniq, COMDT_INTIMACY_DATA data)
		{
			if (uniq == null || data == null)
			{
				return;
			}
			this.AddFriendIntimacy(uniq.ullUid, uniq.dwLogicWorldId, data.dwLastIntimacyTime, data.wIntimacyValue, data.bIntimacyState, data.dwTerminateTime);
			this.FRData.ProcessFriendList(uniq, data);
		}

		public void RemoveIntimacy(ulong ullUid, uint dwLogicWorldID)
		{
			int friendIntimacyIndex = this.GetFriendIntimacyIndex(ullUid, dwLogicWorldID);
			if (friendIntimacyIndex != -1)
			{
				this.m_stFriendIntimacyList.RemoveAt(friendIntimacyIndex);
			}
			this.FRData.Remove(ullUid, dwLogicWorldID);
		}

		private int GetFriendIntimacyIndex(ulong ullUid, uint dwLogicWorldID)
		{
			for (int i = 0; i < this.m_stFriendIntimacyList.get_Count(); i++)
			{
				CFriendModel.stFriendIntimacy stFriendIntimacy = this.m_stFriendIntimacyList.get_Item(i);
				if (stFriendIntimacy.ullUid == ullUid && stFriendIntimacy.dwLogicWorldID == dwLogicWorldID)
				{
					return i;
				}
			}
			return -1;
		}

		public void LoadPreconfigVerifyContentList()
		{
			int num = 0;
			bool flag = true;
			while (flag)
			{
				string text = string.Format("FriendVerify_Text_{0}", num);
				string text2 = Singleton<CTextManager>.instance.GetText(text);
				if (string.Equals(text, text2))
				{
					flag = false;
				}
				else
				{
					if (!this.m_preconfigVerifyContentList.Contains(text2))
					{
						this.m_preconfigVerifyContentList.Add(text2);
					}
					num++;
				}
			}
			this.friend_static_text = Singleton<CTextManager>.instance.GetText("FriendVerify_StaticText");
		}

		public void LoadIntimacyConfig()
		{
			if (!int.TryParse(Singleton<CTextManager>.instance.GetText("Intimacy_FreezeDay"), ref this.freezeDayCount))
			{
				DebugHelper.Assert(false, "---Intimacy  Intimacy_FreezeDay 对应的配置项 好像不是整数哦， check out");
			}
			if (!int.TryParse(Singleton<CTextManager>.instance.GetText("Intimacy_CondLow"), ref this.cond1))
			{
				DebugHelper.Assert(false, "---Intimacy  Intimacy_CondLow 对应的配置项 好像不是整数哦， check out");
			}
			if (!int.TryParse(Singleton<CTextManager>.instance.GetText("Intimacy_CondMiddle"), ref this.cond2))
			{
				DebugHelper.Assert(false, "---Intimacy  Intimacy_CondMiddle 对应的配置项 好像不是整数哦， check out");
			}
			this.friendReserve.Receive_Reserve_Tip = Singleton<CTextManager>.instance.GetText("Receive_Reserve_Tip");
			this.friendReserve.Reserve_TabShortCut = Singleton<CTextManager>.instance.GetText("Reserve_TabShortCut");
			this.friendReserve.Reserve_TabReserve = Singleton<CTextManager>.instance.GetText("Reserve_TabReserve");
			this.friendReserve.Reserve_Success = Singleton<CTextManager>.instance.GetText("Reserve_Success");
			this.friendReserve.Reserve_Failed = Singleton<CTextManager>.instance.GetText("Reserve_Failed");
			this.friendReserve.Reserve_TabReserve = Singleton<CTextManager>.instance.GetText("Reserve_Wait4Rsp");
			this.fullIntimacy = 999u;
		}

		public string GetRandomVerifyContent()
		{
			if (this.m_preconfigVerifyContentList.get_Count() == 0)
			{
				return null;
			}
			int num = Random.Range(0, this.m_preconfigVerifyContentList.get_Count() - 1);
			return this.m_preconfigVerifyContentList.get_Item(num);
		}

		public string GetFriendVerifyContent(ulong ullUid, uint dwLogicWorldID, CFriendModel.enVerifyDataSet verifyType)
		{
			for (int i = 0; i < this.m_friendVerifyList[(int)verifyType].get_Count(); i++)
			{
				stFriendVerifyContent stFriendVerifyContent = this.m_friendVerifyList[(int)verifyType].get_Item(i);
				if (stFriendVerifyContent.ullUid == ullUid && stFriendVerifyContent.dwLogicWorldID == dwLogicWorldID)
				{
					return stFriendVerifyContent.content;
				}
			}
			return null;
		}

		public COMDT_FRIEND_SOURCE GetFriendVerifySource(ulong ullUid, uint dwLogicWorldID, CFriendModel.enVerifyDataSet verifyType)
		{
			for (int i = 0; i < this.m_friendVerifyList[(int)verifyType].get_Count(); i++)
			{
				stFriendVerifyContent stFriendVerifyContent = this.m_friendVerifyList[(int)verifyType].get_Item(i);
				if (stFriendVerifyContent.ullUid == ullUid && stFriendVerifyContent.dwLogicWorldID == dwLogicWorldID)
				{
					return stFriendVerifyContent.friendSource;
				}
			}
			return null;
		}

		public void AddFriendVerifyContent(ulong ullUid, uint dwLogicWorldID, string content, CFriendModel.enVerifyDataSet verifyType, COMDT_FRIEND_SOURCE friendSource, int mentor_type = 0)
		{
			int friendVerifyIndex = this.GetFriendVerifyIndex(ullUid, dwLogicWorldID, verifyType);
			if (friendVerifyIndex != -1)
			{
				this.m_friendVerifyList[(int)verifyType].RemoveAt(friendVerifyIndex);
			}
			this.m_friendVerifyList[(int)verifyType].Add(new stFriendVerifyContent(ullUid, dwLogicWorldID, content, friendSource, mentor_type));
		}

		public void RemoveFriendVerifyContent(ulong ullUid, uint dwLogicWorldID, CFriendModel.enVerifyDataSet verifyType)
		{
			int friendVerifyIndex = this.GetFriendVerifyIndex(ullUid, dwLogicWorldID, verifyType);
			if (friendVerifyIndex != -1)
			{
				this.m_friendVerifyList[(int)verifyType].RemoveAt(friendVerifyIndex);
			}
		}

		private int GetFriendVerifyIndex(ulong ullUid, uint dwLogicWorldID, CFriendModel.enVerifyDataSet verifyType)
		{
			for (int i = 0; i < this.m_friendVerifyList[(int)verifyType].get_Count(); i++)
			{
				stFriendVerifyContent stFriendVerifyContent = this.m_friendVerifyList[(int)verifyType].get_Item(i);
				if (stFriendVerifyContent.ullUid == ullUid && stFriendVerifyContent.dwLogicWorldID == dwLogicWorldID)
				{
					return i;
				}
			}
			return -1;
		}

		public stFriendVerifyContent GetFriendVerifyData(ulong ullUid, uint dwLogicWorldID, CFriendModel.enVerifyDataSet verifyType)
		{
			for (int i = 0; i < this.m_friendVerifyList[(int)verifyType].get_Count(); i++)
			{
				stFriendVerifyContent stFriendVerifyContent = this.m_friendVerifyList[(int)verifyType].get_Item(i);
				if (stFriendVerifyContent.ullUid == ullUid && stFriendVerifyContent.dwLogicWorldID == dwLogicWorldID)
				{
					return stFriendVerifyContent;
				}
			}
			return null;
		}

		public byte GetLBSGenterFilter()
		{
			if (this.fileter == 3u)
			{
				return 0;
			}
			if (this.fileter == 2u)
			{
				return 2;
			}
			if (this.fileter == 1u)
			{
				return 1;
			}
			return 0;
		}

		public uint NegFlag(uint value, int flag)
		{
			uint num = 1u << flag;
			if ((value & num) != 0u)
			{
				value &= ~num;
			}
			else
			{
				value |= num;
			}
			return value;
		}

		public ListView<CSDT_LBS_USER_INFO> GetCurrentLBSList()
		{
			if (this.fileter == 3u)
			{
				return this.GetLBSList(CFriendModel.LBSGenderType.Both);
			}
			if (this.fileter == 2u)
			{
				return this.GetLBSList(CFriendModel.LBSGenderType.Nv);
			}
			if (this.fileter == 1u)
			{
				return this.GetLBSList(CFriendModel.LBSGenderType.Nan);
			}
			return null;
		}

		public ListView<CSDT_LBS_USER_INFO> GetLBSList(CFriendModel.LBSGenderType type = CFriendModel.LBSGenderType.Both)
		{
			if (type == CFriendModel.LBSGenderType.Both)
			{
				return this.m_LBSList;
			}
			if (type == CFriendModel.LBSGenderType.Nan)
			{
				return this.m_LBSListNan;
			}
			if (type == CFriendModel.LBSGenderType.Nv)
			{
				return this.m_LBSListNv;
			}
			return null;
		}

		public int GetLBSListIndex(ulong ullUid, uint dwLogicWorldId, CFriendModel.LBSGenderType type = CFriendModel.LBSGenderType.Both)
		{
			ListView<CSDT_LBS_USER_INFO> lBSList = this.GetLBSList(type);
			if (lBSList == null)
			{
				return -1;
			}
			for (int i = 0; i < lBSList.Count; i++)
			{
				CSDT_LBS_USER_INFO cSDT_LBS_USER_INFO = lBSList[i];
				if (cSDT_LBS_USER_INFO != null && cSDT_LBS_USER_INFO.stLbsUserInfo.stUin.ullUid == ullUid && cSDT_LBS_USER_INFO.stLbsUserInfo.stUin.dwLogicWorldId == dwLogicWorldId)
				{
					return i;
				}
			}
			return -1;
		}

		public int GetLBSListIndex(ulong ullUid, uint dwLogicWorldId, ListView<CSDT_LBS_USER_INFO> list)
		{
			if (list == null)
			{
				return -1;
			}
			for (int i = 0; i < list.Count; i++)
			{
				CSDT_LBS_USER_INFO cSDT_LBS_USER_INFO = list[i];
				if (cSDT_LBS_USER_INFO != null && cSDT_LBS_USER_INFO.stLbsUserInfo.stUin.ullUid == ullUid && cSDT_LBS_USER_INFO.stLbsUserInfo.stUin.dwLogicWorldId == dwLogicWorldId)
				{
					return i;
				}
			}
			return -1;
		}

		public CSDT_LBS_USER_INFO GetLBSUserInfo(ulong ullUid, uint dwLogicWorldId, CFriendModel.LBSGenderType type = CFriendModel.LBSGenderType.Both)
		{
			ListView<CSDT_LBS_USER_INFO> lBSList = this.GetLBSList(type);
			if (lBSList == null)
			{
				return null;
			}
			for (int i = 0; i < lBSList.Count; i++)
			{
				CSDT_LBS_USER_INFO cSDT_LBS_USER_INFO = lBSList[i];
				if (cSDT_LBS_USER_INFO != null && cSDT_LBS_USER_INFO.stLbsUserInfo.stUin.ullUid == ullUid && cSDT_LBS_USER_INFO.stLbsUserInfo.stUin.dwLogicWorldId == dwLogicWorldId)
				{
					return cSDT_LBS_USER_INFO;
				}
			}
			return null;
		}

		public void SortLBSFriend()
		{
			this.m_LBSList.Sort(new Comparison<CSDT_LBS_USER_INFO>(CFriendModel.LBSDataSort));
			this.m_LBSListNan.Sort(new Comparison<CSDT_LBS_USER_INFO>(CFriendModel.LBSDataSort));
			this.m_LBSListNv.Sort(new Comparison<CSDT_LBS_USER_INFO>(CFriendModel.LBSDataSort));
		}

		public void ClearLBSData()
		{
			this.m_LBSList.Clear();
			this.m_LBSListNan.Clear();
			this.m_LBSListNv.Clear();
		}

		public void RemoveLBSUser(ulong ullUid, uint dwLogicWorldId)
		{
			this.RemoveLBSUser(ullUid, dwLogicWorldId, this.m_LBSList);
			this.RemoveLBSUser(ullUid, dwLogicWorldId, this.m_LBSListNan);
			this.RemoveLBSUser(ullUid, dwLogicWorldId, this.m_LBSListNv);
		}

		private void RemoveLBSUser(ulong ullUid, uint dwLogicWorldId, ListView<CSDT_LBS_USER_INFO> list)
		{
			if (list == null)
			{
				return;
			}
			int lBSListIndex = this.GetLBSListIndex(ullUid, dwLogicWorldId, CFriendModel.LBSGenderType.Both);
			if (lBSListIndex != -1)
			{
				list.RemoveAt(lBSListIndex);
			}
		}

		public void AddLBSUser(CSDT_LBS_USER_INFO info)
		{
			this.AddLBSUser(info, this.m_LBSList);
			if (info.stLbsUserInfo.bGender == 2)
			{
				this.AddLBSUser(info, this.m_LBSListNv);
			}
			if (info.stLbsUserInfo.bGender == 1)
			{
				this.AddLBSUser(info, this.m_LBSListNan);
			}
		}

		private void AddLBSUser(CSDT_LBS_USER_INFO info, ListView<CSDT_LBS_USER_INFO> list)
		{
			if (info == null || list == null)
			{
				return;
			}
			int lBSListIndex = this.GetLBSListIndex(info.stLbsUserInfo.stUin.ullUid, info.stLbsUserInfo.stUin.dwLogicWorldId, list);
			if (lBSListIndex != -1)
			{
				list.RemoveAt(lBSListIndex);
			}
			list.Add(info);
		}

		private static void FindAll(ListView<COMDT_FRIEND_INFO> InSearch, Predicate<COMDT_FRIEND_INFO> match, ListView<COMDT_FRIEND_INFO> outputList)
		{
			if (InSearch == null || outputList == null)
			{
				return;
			}
			outputList.Clear();
			for (int i = 0; i < InSearch.Count; i++)
			{
				if (match.Invoke(InSearch[i]))
				{
					outputList.Add(InSearch[i]);
				}
			}
		}

		public ListView<COMDT_FRIEND_INFO> GetOnlineFriendList()
		{
			ListView<COMDT_FRIEND_INFO> list = this.GetList(CFriendModel.FriendType.GameFriend);
			CFriendModel.FindAll(list, new Predicate<COMDT_FRIEND_INFO>(CFriendModel.OnlineFinder), this.cache_OnlineFriends_Results);
			return this.cache_OnlineFriends_Results;
		}

		public ListView<COMDT_FRIEND_INFO> GetOnlineFriendAndSnsFriendList()
		{
			ListView<COMDT_FRIEND_INFO> list = this.GetList(CFriendModel.FriendType.GameFriend);
			ListView<COMDT_FRIEND_INFO> list2 = this.GetList(CFriendModel.FriendType.SNS);
			this.cache_OnlineFriends_Results.Clear();
			for (int i = 0; i < list2.Count; i++)
			{
				if (CFriendModel.OnlineFinder(list2[i]))
				{
					this.cache_OnlineFriends_Results.Add(list2[i]);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				if (CFriendModel.OnlineFinder(list[j]))
				{
					bool flag = false;
					for (int k = 0; k < this.cache_OnlineFriends_Results.Count; k++)
					{
						if (this.cache_OnlineFriends_Results[k].stUin.ullUid == list[j].stUin.ullUid)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						this.cache_OnlineFriends_Results.Add(list[j]);
					}
				}
			}
			return this.cache_OnlineFriends_Results;
		}

		private int GetItemIndex(ulong uid, uint worldID, ListView<COMDT_FRIEND_INFO> targetList)
		{
			if (targetList == null)
			{
				return -1;
			}
			for (int i = 0; i < targetList.Count; i++)
			{
				COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = targetList[i];
				if (cOMDT_FRIEND_INFO != null && cOMDT_FRIEND_INFO.stUin.ullUid == uid && cOMDT_FRIEND_INFO.stUin.dwLogicWorldId == worldID)
				{
					return i;
				}
			}
			return -1;
		}

		public ListView<COMDT_FRIEND_INFO> GetValidChatFriendList()
		{
			this.cache_chatFriends_Results.Clear();
			ListView<COMDT_FRIEND_INFO> list = this.GetList(CFriendModel.FriendType.GameFriend);
			ListView<COMDT_FRIEND_INFO> list2 = this.GetList(CFriendModel.FriendType.SNS);
			ListView<COMDT_FRIEND_INFO> list3 = this.GetList(CFriendModel.FriendType.Mentor);
			ListView<COMDT_FRIEND_INFO> list4 = this.GetList(CFriendModel.FriendType.Apprentice);
			if (list2 != null)
			{
				for (int i = 0; i < list2.Count; i++)
				{
					COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = list2[i];
					if (cOMDT_FRIEND_INFO != null && CFriendModel.OnlineFinder(cOMDT_FRIEND_INFO) && this.GetItemIndex(cOMDT_FRIEND_INFO.stUin.ullUid, cOMDT_FRIEND_INFO.stUin.dwLogicWorldId, this.cache_chatFriends_Results) == -1)
					{
						this.cache_chatFriends_Results.Add(cOMDT_FRIEND_INFO);
					}
				}
			}
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					COMDT_FRIEND_INFO cOMDT_FRIEND_INFO2 = list[j];
					if (cOMDT_FRIEND_INFO2 != null && CFriendModel.OnlineFinder(cOMDT_FRIEND_INFO2) && this.GetItemIndex(cOMDT_FRIEND_INFO2.stUin.ullUid, cOMDT_FRIEND_INFO2.stUin.dwLogicWorldId, this.cache_chatFriends_Results) == -1)
					{
						this.cache_chatFriends_Results.Add(cOMDT_FRIEND_INFO2);
					}
				}
			}
			if (list3 != null)
			{
				for (int k = 0; k < list3.Count; k++)
				{
					COMDT_FRIEND_INFO cOMDT_FRIEND_INFO3 = list3[k];
					if (cOMDT_FRIEND_INFO3 != null && this.GetItemIndex(cOMDT_FRIEND_INFO3.stUin.ullUid, cOMDT_FRIEND_INFO3.stUin.dwLogicWorldId, this.cache_chatFriends_Results) == -1)
					{
						this.cache_chatFriends_Results.Add(cOMDT_FRIEND_INFO3);
					}
				}
			}
			if (list4 != null)
			{
				for (int l = 0; l < list4.Count; l++)
				{
					COMDT_FRIEND_INFO cOMDT_FRIEND_INFO4 = list4[l];
					if (cOMDT_FRIEND_INFO4 != null && this.GetItemIndex(cOMDT_FRIEND_INFO4.stUin.ullUid, cOMDT_FRIEND_INFO4.stUin.dwLogicWorldId, this.cache_chatFriends_Results) == -1)
					{
						this.cache_chatFriends_Results.Add(cOMDT_FRIEND_INFO4);
					}
				}
			}
			ListView<CChatChannel> friendChannelList = Singleton<CChatController>.instance.model.channelMgr.FriendChannelList;
			if (friendChannelList != null)
			{
				for (int m = 0; m < friendChannelList.Count; m++)
				{
					CChatChannel cChatChannel = friendChannelList[m];
					if (cChatChannel != null && cChatChannel.HasAnyValidChatEntity() && this.GetItemIndex(cChatChannel.ullUid, cChatChannel.dwLogicWorldId, this.cache_chatFriends_Results) == -1)
					{
						COMDT_FRIEND_INFO gameOrSnsFriend = this.GetGameOrSnsFriend(cChatChannel.ullUid, cChatChannel.dwLogicWorldId);
						if (gameOrSnsFriend != null)
						{
							this.cache_chatFriends_Results.Add(gameOrSnsFriend);
						}
					}
				}
			}
			return this.cache_chatFriends_Results;
		}

		public bool IsAnyFriendExist(bool requre_bOnline)
		{
			ListView<COMDT_FRIEND_INFO> list = this.GetList(CFriendModel.FriendType.GameFriend);
			if (!requre_bOnline)
			{
				return list.Count > 0;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].bIsOnline == 1)
				{
					return true;
				}
			}
			return false;
		}

		public static bool OnlineFinder(COMDT_FRIEND_INFO a)
		{
			return a.bIsOnline == 1;
		}

		public static bool IsOnSnsSwitch(uint switchBits, COM_REFUSE_TYPE type)
		{
			return ((ulong)switchBits & 1uL << (int)(type & (COM_REFUSE_TYPE)31)) != 0uL;
		}

		public void SortGameFriend()
		{
			ListView<COMDT_FRIEND_INFO> list = this.GetList(CFriendModel.FriendType.GameFriend);
			if (list != null)
			{
				list.Sort(new Comparison<COMDT_FRIEND_INFO>(CFriendModel.FriendDataSort));
			}
		}

		public void SortSNSFriend()
		{
			ListView<COMDT_FRIEND_INFO> list = this.GetList(CFriendModel.FriendType.SNS);
			if (list != null)
			{
				list.Sort(new Comparison<COMDT_FRIEND_INFO>(CFriendModel.FriendDataSort));
			}
		}

		public void SortShower(CFriendModel.FriendType type)
		{
			ListView<COMDT_FRIEND_INFO> list = this.GetList(type);
			if (list != null)
			{
				if (type == CFriendModel.FriendType.Mentor)
				{
					list.Sort(new Comparison<COMDT_FRIEND_INFO>(CFriendModel.MentorDataSort));
				}
				else
				{
					list.Sort(new Comparison<COMDT_FRIEND_INFO>(CFriendModel.FriendDataSort));
				}
			}
		}

		public bool IsFriendOfflineOnline(ulong ullUid, uint dwLogicWorldID)
		{
			ListView<COMDT_FRIEND_INFO> list = this.GetList(CFriendModel.FriendType.GameFriend);
			ListView<COMDT_FRIEND_INFO> list2 = this.GetList(CFriendModel.FriendType.SNS);
			ListView<COMDT_FRIEND_INFO> list3 = this.GetList(CFriendModel.FriendType.Mentor);
			ListView<COMDT_FRIEND_INFO> list4 = this.GetList(CFriendModel.FriendType.Apprentice);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = list[i];
					if (cOMDT_FRIEND_INFO != null && cOMDT_FRIEND_INFO.stUin.ullUid == ullUid && cOMDT_FRIEND_INFO.stUin.dwLogicWorldId == dwLogicWorldID)
					{
						return cOMDT_FRIEND_INFO.bIsOnline == 1;
					}
				}
			}
			if (list2 != null)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					COMDT_FRIEND_INFO cOMDT_FRIEND_INFO2 = list2[j];
					if (cOMDT_FRIEND_INFO2 != null && cOMDT_FRIEND_INFO2.stUin.ullUid == ullUid && cOMDT_FRIEND_INFO2.stUin.dwLogicWorldId == dwLogicWorldID)
					{
						return cOMDT_FRIEND_INFO2.bIsOnline == 1;
					}
				}
			}
			if (list3 != null)
			{
				for (int k = 0; k < list3.Count; k++)
				{
					COMDT_FRIEND_INFO cOMDT_FRIEND_INFO3 = list3[k];
					if (cOMDT_FRIEND_INFO3 != null && cOMDT_FRIEND_INFO3.stUin.ullUid == ullUid && cOMDT_FRIEND_INFO3.stUin.dwLogicWorldId == dwLogicWorldID)
					{
						return cOMDT_FRIEND_INFO3.bIsOnline == 1;
					}
				}
			}
			if (list4 != null)
			{
				for (int l = 0; l < list4.Count; l++)
				{
					COMDT_FRIEND_INFO cOMDT_FRIEND_INFO4 = list4[l];
					if (cOMDT_FRIEND_INFO4 != null && cOMDT_FRIEND_INFO4.stUin.ullUid == ullUid && cOMDT_FRIEND_INFO4.stUin.dwLogicWorldId == dwLogicWorldID)
					{
						return cOMDT_FRIEND_INFO4.bIsOnline == 1;
					}
				}
			}
			return false;
		}

		public bool IsFriendInGamingState(ulong ullUid, uint dwLogicWorldID, COM_ACNT_GAME_STATE State)
		{
			for (int i = 0; i < this.gameStateList.Count; i++)
			{
				CFriendModel.FriendInGame friendInGame = this.gameStateList[i];
				if (friendInGame.ullUid == ullUid && friendInGame.dwLogicWorldID == dwLogicWorldID)
				{
					return friendInGame.State == State;
				}
			}
			return false;
		}

		public COM_ACNT_GAME_STATE GetFriendInGamingState(ulong ullUid, uint dwLogicWorldID)
		{
			for (int i = 0; i < this.gameStateList.Count; i++)
			{
				CFriendModel.FriendInGame friendInGame = this.gameStateList[i];
				if (friendInGame != null && friendInGame.ullUid == ullUid && friendInGame.dwLogicWorldID == dwLogicWorldID)
				{
					return friendInGame.State;
				}
			}
			return COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE;
		}

		public CFriendModel.FriendInGame GetFriendInGaming(ulong ullUid, uint dwLogicWorldID)
		{
			for (int i = 0; i < this.gameStateList.Count; i++)
			{
				CFriendModel.FriendInGame friendInGame = this.gameStateList[i];
				if (friendInGame != null && friendInGame.ullUid == ullUid && friendInGame.dwLogicWorldID == dwLogicWorldID)
				{
					return friendInGame;
				}
			}
			return null;
		}

		public void SetFriendGameState(ulong ullUid, uint dwLogicWorldID, COM_ACNT_GAME_STATE State, uint startTime, string nickName = "", bool IgnoreWorld_id = false, bool needSetAntiDisturbBits = false, uint antiDisturbBits = 0u)
		{
			for (int i = 0; i < this.gameStateList.Count; i++)
			{
				CFriendModel.FriendInGame friendInGame = this.gameStateList[i];
				bool flag;
				if (!IgnoreWorld_id)
				{
					flag = (friendInGame.ullUid == ullUid && friendInGame.dwLogicWorldID == dwLogicWorldID);
				}
				else
				{
					flag = (friendInGame.ullUid == ullUid);
				}
				if (flag)
				{
					if (friendInGame.State != COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE && State == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE)
					{
						COMDT_FRIEND_INFO info = this.GetInfo(CFriendModel.FriendType.GameFriend, ullUid, dwLogicWorldID);
						if (info == null)
						{
							info = this.GetInfo(CFriendModel.FriendType.SNS, ullUid, dwLogicWorldID);
						}
						if (info != null)
						{
							string text = UT.Bytes2String(info.szUserName);
							if (this.friendReserve.sendReserve_accepted.Contains(text))
							{
								if (!Singleton<BattleLogic>.GetInstance().isRuning)
								{
									string text2 = Singleton<CTextManager>.instance.GetText("sendReserve_accepted_Idle_Tip");
									string strContent = string.Format(text2, text);
									Singleton<CUIManager>.instance.OpenMessageBox(strContent, false);
								}
								this.friendReserve.sendReserve_accepted.Remove(text);
							}
						}
					}
					friendInGame.State = State;
					friendInGame.dwLogicWorldID = dwLogicWorldID;
					friendInGame.startTime = startTime;
					if (needSetAntiDisturbBits)
					{
						friendInGame.antiDisturbBits = antiDisturbBits;
					}
					return;
				}
			}
			this.gameStateList.Add(new CFriendModel.FriendInGame(ullUid, dwLogicWorldID, State, startTime, nickName, antiDisturbBits));
		}

		public static int MentorDataSort(COMDT_FRIEND_INFO l, COMDT_FRIEND_INFO r)
		{
			if (CFriendContoller.m_mentorInfo == null)
			{
				return CFriendModel.FriendDataSort(l, r);
			}
			if (l == null || r == null)
			{
				return 0;
			}
			if (l.stUin.dwLogicWorldId == CFriendContoller.m_mentorInfo.stMaster.stFriendInfo.stUin.dwLogicWorldId && l.stUin.ullUid == CFriendContoller.m_mentorInfo.stMaster.stFriendInfo.stUin.ullUid)
			{
				return -1;
			}
			if (r.stUin.dwLogicWorldId == CFriendContoller.m_mentorInfo.stMaster.stFriendInfo.stUin.dwLogicWorldId && r.stUin.ullUid == CFriendContoller.m_mentorInfo.stMaster.stFriendInfo.stUin.ullUid)
			{
				return 1;
			}
			return CFriendModel.FriendDataSort(l, r);
		}

		public static int FriendDataSort(COMDT_FRIEND_INFO l, COMDT_FRIEND_INFO r)
		{
			if (l == r)
			{
				return 0;
			}
			if (l == null || r == null)
			{
				return 0;
			}
			if (r.bIsOnline != l.bIsOnline)
			{
				return (int)(r.bIsOnline - l.bIsOnline);
			}
			CFriendModel model = Singleton<CFriendContoller>.instance.model;
			stFriendByUUIDAndLogicID stFriendByUUIDAndLogicID = new stFriendByUUIDAndLogicID(l.stUin.ullUid, l.stUin.dwLogicWorldId, CFriendModel.FriendType.GameFriend);
			bool flag;
			CFriendModel.IsNoAskForDic.TryGetValue(stFriendByUUIDAndLogicID, ref flag);
			ushort num;
			CFriendModel.EIntimacyType eIntimacyType;
			bool flag2;
			model.GetFriendIntimacy(r.stUin.ullUid, r.stUin.dwLogicWorldId, out num, out eIntimacyType, out flag2);
			ushort num2;
			CFriendModel.EIntimacyType eIntimacyType2;
			bool flag3;
			model.GetFriendIntimacy(l.stUin.ullUid, l.stUin.dwLogicWorldId, out num2, out eIntimacyType2, out flag3);
			if (num2 != num)
			{
				return (int)(num - num2);
			}
			if (r.stGameVip.dwCurLevel != l.stGameVip.dwCurLevel)
			{
				return (int)(r.stGameVip.dwCurLevel - l.stGameVip.dwCurLevel);
			}
			if (r.dwPvpLvl != l.dwPvpLvl)
			{
				return (int)(r.dwPvpLvl - l.dwPvpLvl);
			}
			if (flag)
			{
				return 1;
			}
			return -1;
		}

		private static COM_INTIMACY_STATE GetState(ulong ulluid, uint worldID)
		{
			CFR cfr = Singleton<CFriendContoller>.instance.model.FRData.GetCfr(ulluid, worldID);
			if (cfr == null)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL;
			}
			return cfr.state;
		}

		public static int LBSDataSort(CSDT_LBS_USER_INFO l, CSDT_LBS_USER_INFO r)
		{
			int bLogicGrade = (int)CLadderSystem.GetGradeDataByShowGrade((int)l.bGradeOfRank).bLogicGrade;
			int bLogicGrade2 = (int)CLadderSystem.GetGradeDataByShowGrade((int)r.bGradeOfRank).bLogicGrade;
			if (l == r)
			{
				return 0;
			}
			if (l == null || r == null)
			{
				return 0;
			}
			if (r.dwDistance != l.dwDistance)
			{
				return (int)(l.dwDistance - r.dwDistance);
			}
			if (bLogicGrade2 != bLogicGrade)
			{
				return bLogicGrade2 - bLogicGrade;
			}
			if (r.stLbsUserInfo.stGameVip.dwCurLevel == l.stLbsUserInfo.stGameVip.dwCurLevel)
			{
				return (int)(r.stLbsUserInfo.dwPvpLvl - l.stLbsUserInfo.dwPvpLvl);
			}
			return (int)(r.stLbsUserInfo.stGameVip.dwCurLevel - l.stLbsUserInfo.stGameVip.dwCurLevel);
		}

		public static int FriendDataSortForChatFriendList(COMDT_FRIEND_INFO l, COMDT_FRIEND_INFO r)
		{
			CChatChannel friendChannel = Singleton<CChatController>.instance.model.channelMgr.GetFriendChannel(r.stUin.ullUid, r.stUin.dwLogicWorldId);
			CChatChannel friendChannel2 = Singleton<CChatController>.instance.model.channelMgr.GetFriendChannel(l.stUin.ullUid, l.stUin.dwLogicWorldId);
			ushort num;
			CFriendModel.EIntimacyType eIntimacyType;
			bool flag;
			Singleton<CFriendContoller>.instance.model.GetFriendIntimacy(r.stUin.ullUid, r.stUin.dwLogicWorldId, out num, out eIntimacyType, out flag);
			ushort num2;
			CFriendModel.EIntimacyType eIntimacyType2;
			bool flag2;
			Singleton<CFriendContoller>.instance.model.GetFriendIntimacy(l.stUin.ullUid, l.stUin.dwLogicWorldId, out num2, out eIntimacyType2, out flag2);
			int num3 = friendChannel.GetUnreadCount() - friendChannel2.GetUnreadCount();
			int num4 = (int)(num - num2);
			int num5 = (int)(r.dwPvpLvl - l.dwPvpLvl);
			int num6 = (int)(r.stGameVip.dwCurLevel - l.stGameVip.dwCurLevel);
			if (num3 > 0)
			{
				return 1000;
			}
			if (num3 < 0)
			{
				return -1000;
			}
			if (num4 > 0)
			{
				return 100;
			}
			if (num4 < 0)
			{
				return -100;
			}
			if (num5 > 0)
			{
				return 10;
			}
			if (num5 < 0)
			{
				return -10;
			}
			if (num6 > 0)
			{
				return 1;
			}
			if (num6 < 0)
			{
				return -1;
			}
			return 0;
		}

		public int GetDataCount(CFriendModel.FriendType type)
		{
			return this.GetList(type).Count;
		}

		public void FilterRecommendFriends()
		{
			ListView<COMDT_FRIEND_INFO> listView = this._map[3u];
			if (listView == null)
			{
				return;
			}
			for (int i = 0; i < listView.Count; i++)
			{
				if (this.GetInfo(CFriendModel.FriendType.GameFriend, listView[i].stUin.ullUid, listView[i].stUin.dwLogicWorldId) != null)
				{
					listView.RemoveAt(i);
					i--;
				}
			}
		}

		public COMDT_FRIEND_INFO GetInfoAtIndex(CFriendModel.FriendType type, int index)
		{
			ListView<COMDT_FRIEND_INFO> list = this.GetList(type);
			if (list == null)
			{
				return null;
			}
			if (index >= 0 && index < list.Count)
			{
				return list[index];
			}
			return null;
		}

		public void Add(CFriendModel.FriendType type, COMDT_FRIEND_INFO data, bool ingore_worldid = false)
		{
			this.AddRankingFriend(type, data);
			ListView<COMDT_FRIEND_INFO> list = this.GetList(type);
			if (list == null)
			{
				return;
			}
			int index = this.getIndex(data, list, ingore_worldid);
			if (index == -1)
			{
				UT.Add2List<COMDT_FRIEND_INFO>(data, list);
			}
			else
			{
				list[index] = data;
			}
			if (type == CFriendModel.FriendType.RequestFriend || type == CFriendModel.FriendType.MentorRequestList)
			{
				Singleton<EventRouter>.instance.BroadCastEvent("Friend_LobbyIconRedDot_Refresh");
			}
		}

		public void Remove(CFriendModel.FriendType type, COMDT_FRIEND_INFO data)
		{
			this.RemoveRankingFriend(type, data);
			ListView<COMDT_FRIEND_INFO> list = this.GetList(type);
			if (list == null)
			{
				return;
			}
			list.Remove(data);
			if (type == CFriendModel.FriendType.RequestFriend || type == CFriendModel.FriendType.MentorRequestList)
			{
				Singleton<EventRouter>.instance.BroadCastEvent("Friend_LobbyIconRedDot_Refresh");
			}
			if (type == CFriendModel.FriendType.Recommend || type == CFriendModel.FriendType.MentorRequestList)
			{
				Singleton<EventRouter>.instance.BroadCastEvent("Friend_RecommandFriend_Refresh");
			}
		}

		public void Remove(CFriendModel.FriendType type, ulong ullUid, uint dwLogicWorldId)
		{
			COMDT_FRIEND_INFO info = this.GetInfo(type, ullUid, dwLogicWorldId);
			this.Remove(type, info);
		}

		public COMDT_FRIEND_INFO Remove(CFriendModel.FriendType type, COMDT_ACNT_UNIQ uniq)
		{
			COMDT_FRIEND_INFO info = this.GetInfo(type, uniq);
			this.Remove(type, info);
			if (type == CFriendModel.FriendType.GameFriend)
			{
				this.RemoveIntimacy(uniq.ullUid, uniq.dwLogicWorldId);
			}
			return info;
		}

		public void Clear(CFriendModel.FriendType type)
		{
			this.GetList(type).Clear();
		}

		public void ClearAll()
		{
			DictionaryView<uint, ListView<COMDT_FRIEND_INFO>>.Enumerator enumerator = this._map.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, ListView<COMDT_FRIEND_INFO>> current = enumerator.Current;
				ListView<COMDT_FRIEND_INFO> value = current.get_Value();
				if (value != null)
				{
					value.Clear();
				}
			}
			this.gameStateList.Clear();
			this._rankingFriend.Clear();
			this.SnsReCallData.Clear();
			this.HeartData.Clear();
			for (int i = 0; i < 2; i++)
			{
				this.m_friendVerifyList[i].Clear();
			}
			this.m_stFriendIntimacyList.Clear();
			this.m_LBSList.Clear();
			this.searchLBSZero = string.Empty;
			CFriendModel.RemarkNames.Clear();
			this.FRData.Clear();
			this.friendRecruit.Clear();
			this.friendReserve.Clear();
		}

		public COMDT_FRIEND_INFO GetInfo(CFriendModel.FriendType type, COMDT_ACNT_UNIQ uniq)
		{
			return this.getFriendInfo(uniq.ullUid, uniq.dwLogicWorldId, this.GetList(type));
		}

		public COMDT_FRIEND_INFO GetInfo(CFriendModel.FriendType type, ulong ullUid, uint dwLogicWorldID)
		{
			ListView<COMDT_FRIEND_INFO> list = this.GetList(type);
			if (list == null)
			{
				return null;
			}
			return this.getFriendInfo(ullUid, dwLogicWorldID, list);
		}

		public COMDT_FRIEND_INFO GetGameOrSnsFriend(ulong ullUid, uint dwLogicWorldID)
		{
			COMDT_FRIEND_INFO friendInfo = this.getFriendInfo(ullUid, dwLogicWorldID, this.GetList(CFriendModel.FriendType.GameFriend));
			if (friendInfo == null)
			{
				friendInfo = this.getFriendInfo(ullUid, dwLogicWorldID, this.GetList(CFriendModel.FriendType.SNS));
			}
			if (friendInfo == null)
			{
				friendInfo = this.getFriendInfo(ullUid, dwLogicWorldID, this.GetList(CFriendModel.FriendType.Mentor));
			}
			if (friendInfo == null)
			{
				friendInfo = this.getFriendInfo(ullUid, dwLogicWorldID, this.GetList(CFriendModel.FriendType.Apprentice));
			}
			return friendInfo;
		}

		public bool IsContain(CFriendModel.FriendType type, ulong ullUid, uint dwLogicWorldID)
		{
			ListView<COMDT_FRIEND_INFO> list = this.GetList(type);
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = list[i];
				if (cOMDT_FRIEND_INFO != null && cOMDT_FRIEND_INFO.stUin.ullUid == ullUid && cOMDT_FRIEND_INFO.stUin.dwLogicWorldId == dwLogicWorldID)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsContain(CFriendModel.FriendType type, COMDT_FRIEND_INFO data)
		{
			ListView<COMDT_FRIEND_INFO> list = this.GetList(type);
			return list != null && list.Contains(data);
		}

		public ListView<COMDT_FRIEND_INFO> GetList(CFriendModel.FriendType type)
		{
			ListView<COMDT_FRIEND_INFO> result;
			if (this._map.TryGetValue((uint)type, out result))
			{
				return result;
			}
			return null;
		}

		public string GetAllFriendsIDs()
		{
			ListView<COMDT_FRIEND_INFO> listView = this._map[1u];
			string text = string.Empty;
			bool flag = true;
			foreach (COMDT_FRIEND_INFO current in listView)
			{
				if (!flag)
				{
					text += '~';
					flag = false;
				}
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					current.stUin.dwLogicWorldId,
					",",
					current.stUin.ullUid
				});
			}
			return text;
		}

		public ListView<COMDT_FRIEND_INFO> GetAllFriend(bool containMentorApprentice = true)
		{
			ListView<COMDT_FRIEND_INFO> listView = this._map[1u];
			ListView<COMDT_FRIEND_INFO> collection = this._map[4u];
			ListView<COMDT_FRIEND_INFO> listView2 = this._map[6u];
			ListView<COMDT_FRIEND_INFO> listView3 = this._map[7u];
			ListView<COMDT_FRIEND_INFO> listView4 = new ListView<COMDT_FRIEND_INFO>();
			listView4.AddRange(collection);
			for (int i = 0; i < listView.Count; i++)
			{
				if (!Singleton<CFriendContoller>.instance.FilterSameFriend(listView[i], listView4))
				{
					listView4.Add(listView[i]);
				}
			}
			if (containMentorApprentice)
			{
				for (int j = 0; j < listView2.Count; j++)
				{
					if (!Singleton<CFriendContoller>.instance.FilterSameFriend(listView2[j], listView4))
					{
						listView4.Add(listView2[j]);
					}
				}
				for (int k = 0; k < listView3.Count; k++)
				{
					if (!Singleton<CFriendContoller>.instance.FilterSameFriend(listView3[k], listView4))
					{
						listView4.Add(listView3[k]);
					}
				}
			}
			return listView4;
		}

		private COMDT_FRIEND_INFO getFriendInfo(ulong ullUid, uint dwLogicWorldID, ListView<COMDT_FRIEND_INFO> list)
		{
			if (list == null)
			{
				return null;
			}
			COMDT_FRIEND_INFO result = null;
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = list[i];
				if (cOMDT_FRIEND_INFO != null && cOMDT_FRIEND_INFO.stUin.ullUid == ullUid && cOMDT_FRIEND_INFO.stUin.dwLogicWorldId == dwLogicWorldID)
				{
					result = cOMDT_FRIEND_INFO;
					break;
				}
			}
			return result;
		}

		private int getIndex(COMDT_FRIEND_INFO info, ListView<COMDT_FRIEND_INFO> list, bool ingore_worldid = false)
		{
			if (list == null)
			{
				return -1;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (UT.BEqual_ACNT_UNIQ(list[i].stUin, info.stUin, ingore_worldid))
				{
					return i;
				}
			}
			return -1;
		}

		public bool IsGameFriend(ulong ullUid, uint logicWorldId)
		{
			ListView<COMDT_FRIEND_INFO> listView = this._map[1u];
			for (int i = 0; i < listView.Count; i++)
			{
				if (listView[i].stUin.ullUid == ullUid && Utility.IsSamePlatform(listView[i].stUin.dwLogicWorldId, logicWorldId))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsSnsFriend(ulong ullUid, uint logicWorldId)
		{
			ListView<COMDT_FRIEND_INFO> listView = this._map[4u];
			for (int i = 0; i < listView.Count; i++)
			{
				if (listView[i].stUin.ullUid == ullUid && listView[i].stUin.dwLogicWorldId == logicWorldId)
				{
					return true;
				}
			}
			return false;
		}

		public COMDT_FRIEND_INFO getFriendByName(string friendName, CFriendModel.FriendType friendType)
		{
			ListView<COMDT_FRIEND_INFO> listView = this._map[(uint)friendType];
			for (int i = 0; i < listView.Count; i++)
			{
				if (Utility.UTF8Convert(listView[i].szUserName) == friendName)
				{
					return listView[i];
				}
			}
			return null;
		}

		public void SetGameFriendGuildState(ulong uid, COM_PLAYER_GUILD_STATE guildState)
		{
			ListView<COMDT_FRIEND_INFO> list = this.GetList(CFriendModel.FriendType.GameFriend);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].stUin.ullUid == uid)
				{
					list[i].bGuildState = (byte)guildState;
				}
			}
		}

		public void AddRankingFriend(CFriendModel.FriendType type, COMDT_FRIEND_INFO data)
		{
			if (type != CFriendModel.FriendType.SNS && type != CFriendModel.FriendType.GameFriend)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < this._rankingFriend.Count; i++)
			{
				if (this._rankingFriend[i].stUin.ullUid == data.stUin.ullUid && this._rankingFriend[i].stUin.dwLogicWorldId == data.stUin.dwLogicWorldId)
				{
					this._rankingFriend[i] = data;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this._rankingFriend.Add(data);
			}
		}

		public void RemoveRankingFriend(CFriendModel.FriendType type, COMDT_FRIEND_INFO data)
		{
			if (type != CFriendModel.FriendType.SNS && type != CFriendModel.FriendType.GameFriend)
			{
				return;
			}
			for (int i = 0; i < this._rankingFriend.Count; i++)
			{
				if (this._rankingFriend[i].stUin.ullUid == data.stUin.ullUid && this._rankingFriend[i].stUin.dwLogicWorldId == data.stUin.dwLogicWorldId)
				{
					this._rankingFriend.RemoveAt(i);
					break;
				}
			}
		}

		public ListView<COMDT_FRIEND_INFO> GetSortedRankingFriendList(COM_APOLLO_TRANK_SCORE_TYPE sortType)
		{
			this.SetSortType(sortType);
			this._rankingFriend.Sort(new Comparison<COMDT_FRIEND_INFO>(CFriendModel.RankingFriendSort));
			return this._rankingFriend;
		}

		public void SetSortType(COM_APOLLO_TRANK_SCORE_TYPE sortTpye)
		{
			CFriendModel._lastSortTpye = sortTpye;
		}

		private static int RankingFriendSort(COMDT_FRIEND_INFO l, COMDT_FRIEND_INFO r)
		{
			int lastSortTpye = (int)CFriendModel._lastSortTpye;
			if (CFriendModel._lastSortTpye != COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_LADDER_POINT)
			{
				return -l.RankVal[lastSortTpye].CompareTo(r.RankVal[lastSortTpye]);
			}
			ulong num = (ulong)l.dwRankClass;
			ulong num2 = (ulong)r.dwRankClass;
			if (num > 0uL && num2 == 0uL)
			{
				return -1;
			}
			if (num == 0uL && num2 > 0uL)
			{
				return 1;
			}
			num = (ulong)CLadderSystem.GetGradeDataByShowGrade((int)l.stRankShowGrade.bGrade).bLogicGrade;
			num2 = (ulong)CLadderSystem.GetGradeDataByShowGrade((int)r.stRankShowGrade.bGrade).bLogicGrade;
			if (num2 == num)
			{
				num = (ulong)l.stRankShowGrade.dwScore;
				num2 = (ulong)l.stRankShowGrade.dwScore;
			}
			if (num2 == num)
			{
				num = (ulong)l.dwPvpLvl;
				num2 = (ulong)r.dwPvpLvl;
			}
			if (num2 == num)
			{
				num = l.stUin.ullUid;
				num2 = r.stUin.ullUid;
			}
			return -num.CompareTo(num2);
		}
	}
}
