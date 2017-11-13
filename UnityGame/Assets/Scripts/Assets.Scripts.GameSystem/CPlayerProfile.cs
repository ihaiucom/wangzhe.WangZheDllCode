using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CPlayerProfile
	{
		private string _playerName;

		private string _playerHeadUrl;

		public string m_personSign;

		private uint _playerLevel;

		private uint _playerExp;

		private uint _playerNeedExp;

		private uint _power;

		private uint _pvpLevel;

		private uint _playerPvpExp;

		private int _5V5TotalCount;

		private int _5V5WinCount;

		private int _3V3TotalCount;

		private int _3V3WinCount;

		private int _1V1TotalCount;

		private int _1V1WinCount;

		private int _2V2TotalCount;

		private int _2V2WinCount;

		public int _guildTotalCount;

		public int _guildWinCount;

		private int _vsAiTotalCount;

		private int _vsAiWinCount;

		private int _doubleKillCount;

		private int _trippleKillCount;

		private int _quataryKillCount;

		private int _pentaKillCount;

		private int _holyShitCount;

		private int _mvpCnt;

		private int _loseMvpCount;

		private int _rankTotalCount;

		private int _rankWinCount;

		private int _entertainmentTotalCount;

		private int _entertainmentWinCount;

		private byte _rankGrade;

		private byte _rankHistoryHighestGrade;

		private uint _rankClass;

		private uint _rankHistoryHighestClass;

		public uint _wangZheCnt;

		private int _heroCnt;

		private int _skinCnt;

		private bool _isOnLine;

		private COM_PRIVILEGE_TYPE _privilegeType;

		private COM_SNSGENDER _gender;

		public ListView<COMDT_MOST_USED_HERO_INFO> _mostUsedHeroList;

		public SCPKG_GAME_VIP_NTF m_vipInfo = new SCPKG_GAME_VIP_NTF();

		public ulong m_uuid;

		public int m_iLogicWorldId;

		private int _selectedHonorId;

		private Dictionary<int, COMDT_HONORINFO> _honorDic = new Dictionary<int, COMDT_HONORINFO>();

		public uint qqVipMask;

		public uint creditScore;

		public int sumDelCreditValue;

		public uint mostDelCreditType;

		public int _geiLiDuiYou;

		public int _keJingDuiShou;

		public bool _haveExtraCoin;

		public int _coinExpireHours;

		public uint _coinWinCount;

		public bool _haveExtraExp;

		public int _expExpireHours;

		public uint _expWinCount;

		public uint _trophyRewardInfoLevel;

		public uint _trophyRank;

		public CAchieveItem2[] _selectedTrophies = new CAchieveItem2[3];

		public COMDT_ACNT_MASTER_INFO _mentorInfo;

		public COMDT_FRIEND_CARD _socialCardInfo;

		public COMDT_STATISTIC_DATA_EXTRA_DETAIL pvpExtraDetail;

		public CSDT_STATISTIC_DATA_EXTRA_RADAR_DETAIL pvpAbilityDetail;

		public bool isMasterData;

		public ulong m_userPrivacyBits;

		public SCPKG_GET_INTIMACY_RELATION_RSP cacheIntimacyRelationRsp;

		public string GuildName
		{
			get;
			set;
		}

		public COM_PLAYER_GUILD_STATE GuildState
		{
			get;
			set;
		}

		private void ResetData()
		{
			this.isMasterData = false;
			this.m_uuid = 0uL;
			this.m_iLogicWorldId = 0;
			this.m_vipInfo = new SCPKG_GAME_VIP_NTF();
			this._playerName = null;
			this._playerHeadUrl = null;
			this.m_personSign = null;
			this._playerLevel = 0u;
			this._playerExp = 0u;
			this._playerNeedExp = 0u;
			this._power = 0u;
			this._pvpLevel = 0u;
			this._playerPvpExp = 0u;
			this._doubleKillCount = 0;
			this._trippleKillCount = 0;
			this._quataryKillCount = 0;
			this._pentaKillCount = 0;
			this._holyShitCount = 0;
			this._mvpCnt = 0;
			this._loseMvpCount = 0;
			this._rankGrade = 0;
			this._rankHistoryHighestGrade = 0;
			this._rankClass = 0u;
			this._rankHistoryHighestClass = 0u;
			this._wangZheCnt = 0u;
			this.GuildName = null;
			this.GuildState = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL;
			this._5V5TotalCount = 0;
			this._5V5WinCount = 0;
			this._3V3TotalCount = 0;
			this._3V3WinCount = 0;
			this._2V2TotalCount = 0;
			this._2V2WinCount = 0;
			this._1V1TotalCount = 0;
			this._1V1WinCount = 0;
			this._guildTotalCount = 0;
			this._guildWinCount = 0;
			this._vsAiTotalCount = 0;
			this._vsAiWinCount = 0;
			this._privilegeType = COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
			this._heroCnt = 0;
			this._skinCnt = 0;
			if (this._mostUsedHeroList == null)
			{
				this._mostUsedHeroList = new ListView<COMDT_MOST_USED_HERO_INFO>();
			}
			else
			{
				this._mostUsedHeroList.Clear();
			}
			this._selectedHonorId = 0;
			this._honorDic.Clear();
			this._haveExtraCoin = false;
			this._coinExpireHours = 0;
			this._coinWinCount = 0u;
			this._haveExtraExp = false;
			this._expExpireHours = 0;
			this._expWinCount = 0u;
			this.pvpExtraDetail = null;
			this.pvpAbilityDetail = null;
			this._trophyRewardInfoLevel = 0u;
			this._trophyRank = 0u;
			Array.Clear(this._selectedTrophies, 0, this._selectedTrophies.Length);
			this._mentorInfo = null;
			this.cacheIntimacyRelationRsp = null;
		}

		public void ConvertRoleInfoData(CRoleInfo roleInfo)
		{
			if (roleInfo == null)
			{
				this.ResetData();
				return;
			}
			this.ResetData();
			this.isMasterData = (roleInfo == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo());
			this.m_uuid = roleInfo.playerUllUID;
			this.m_iLogicWorldId = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID;
			this.m_vipInfo = roleInfo.GetNobeInfo();
			this._gender = roleInfo.m_gender;
			this._privilegeType = roleInfo.m_privilegeType;
			this._playerName = roleInfo.Name;
			this.m_personSign = roleInfo.PersonSign;
			this._playerHeadUrl = roleInfo.HeadUrl;
			this._playerLevel = roleInfo.Level;
			this._playerExp = roleInfo.Exp;
			this._playerNeedExp = roleInfo.NeedExp;
			this._power = roleInfo.BattlePower;
			this._pvpLevel = roleInfo.PvpLevel;
			this._playerPvpExp = roleInfo.PvpExp;
			this._rankGrade = roleInfo.m_rankGrade;
			this._rankHistoryHighestGrade = roleInfo.m_rankHistoryHighestGrade;
			this._rankClass = roleInfo.m_rankClass;
			this._rankHistoryHighestClass = roleInfo.m_rankHistoryHighestClass;
			this._wangZheCnt = roleInfo.m_WangZheCnt;
			this.GuildName = roleInfo.m_baseGuildInfo.name;
			this.GuildState = roleInfo.m_baseGuildInfo.guildState;
			this.creditScore = roleInfo.creditScore;
			this.sumDelCreditValue = roleInfo.sumDelCreditValue;
			this.mostDelCreditType = roleInfo.mostDelCreditType;
			this.m_userPrivacyBits = roleInfo.m_userPrivacyBits;
			int num = 0;
			while ((long)num < (long)((ulong)roleInfo.pvpDetail.stKVDetail.dwNum))
			{
				COMDT_STATISTIC_KEY_VALUE_INFO cOMDT_STATISTIC_KEY_VALUE_INFO = roleInfo.pvpDetail.stKVDetail.astKVDetail[num];
				RES_STATISTIC_SETTLE_DATA_TYPE dwKey = (RES_STATISTIC_SETTLE_DATA_TYPE)cOMDT_STATISTIC_KEY_VALUE_INFO.dwKey;
				switch (dwKey)
				{
				case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_MVP_CNT:
					this._mvpCnt = (int)cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue;
					break;
				case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_LOSE_SOUL:
					this._loseMvpCount = (int)cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue;
					break;
				case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_GODLIKE_CNT:
					this._holyShitCount = (int)cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue;
					break;
				case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_DOUBLE_KILL_CNT:
					this._doubleKillCount = (int)cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue;
					break;
				case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_TRIPLE_KILL_CNT:
					this._trippleKillCount = (int)cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue;
					break;
				default:
					if (dwKey != RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_QUATARY_KILL_CNT)
					{
						if (dwKey == RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_PENTA_KILL_CNT)
						{
							this._pentaKillCount = (int)cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue;
						}
					}
					else
					{
						this._quataryKillCount = (int)cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue;
					}
					break;
				}
				num++;
			}
			this._5V5TotalCount = (int)roleInfo.pvpDetail.stFiveVsFiveInfo.dwTotalNum;
			this._5V5WinCount = (int)roleInfo.pvpDetail.stFiveVsFiveInfo.dwWinNum;
			this._3V3TotalCount = (int)roleInfo.pvpDetail.stThreeVsThreeInfo.dwTotalNum;
			this._3V3WinCount = (int)roleInfo.pvpDetail.stThreeVsThreeInfo.dwWinNum;
			this._2V2TotalCount = (int)roleInfo.pvpDetail.stTwoVsTwoInfo.dwTotalNum;
			this._2V2WinCount = (int)roleInfo.pvpDetail.stTwoVsTwoInfo.dwWinNum;
			this._1V1TotalCount = (int)roleInfo.pvpDetail.stOneVsOneInfo.dwTotalNum;
			this._1V1WinCount = (int)roleInfo.pvpDetail.stOneVsOneInfo.dwWinNum;
			this._guildTotalCount = (int)roleInfo.pvpDetail.stGuildMatch.dwTotalNum;
			this._guildWinCount = (int)roleInfo.pvpDetail.stGuildMatch.dwWinNum;
			this._vsAiTotalCount = (int)roleInfo.pvpDetail.stVsMachineInfo.dwTotalNum;
			this._vsAiWinCount = (int)roleInfo.pvpDetail.stVsMachineInfo.dwWinNum;
			this._rankTotalCount = (int)roleInfo.pvpDetail.stLadderInfo.dwTotalNum;
			this._rankWinCount = (int)roleInfo.pvpDetail.stLadderInfo.dwWinNum;
			this._entertainmentTotalCount = (int)roleInfo.pvpDetail.stEntertainmentInfo.dwTotalNum;
			this._entertainmentWinCount = (int)roleInfo.pvpDetail.stEntertainmentInfo.dwWinNum;
			this._heroCnt = roleInfo.GetHaveHeroCount(false);
			this._skinCnt = roleInfo.GetHeroSkinCount(false);
			this._isOnLine = true;
			if (this._mostUsedHeroList == null)
			{
				this._mostUsedHeroList = new ListView<COMDT_MOST_USED_HERO_INFO>();
			}
			else
			{
				this._mostUsedHeroList.Clear();
			}
			int num2 = (int)Mathf.Min(roleInfo.MostUsedHeroDetail.dwHeroNum, (float)roleInfo.MostUsedHeroDetail.astHeroInfoList.Length);
			for (int i = 0; i < num2; i++)
			{
				this._mostUsedHeroList.Add(roleInfo.MostUsedHeroDetail.astHeroInfoList[i]);
			}
			this.SortMostUsedHeroList();
			this._selectedHonorId = roleInfo.selectedHonorID;
			Dictionary<int, COMDT_HONORINFO>.Enumerator enumerator = roleInfo.honorDic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, COMDT_HONORINFO> current = enumerator.get_Current();
				int key = current.get_Key();
				Dictionary<int, COMDT_HONORINFO> honorDic = this._honorDic;
				KeyValuePair<int, COMDT_HONORINFO> current2 = enumerator.get_Current();
				if (!honorDic.ContainsKey(current2.get_Key()))
				{
					Dictionary<int, COMDT_HONORINFO> honorDic2 = this._honorDic;
					int num3 = key;
					KeyValuePair<int, COMDT_HONORINFO> current3 = enumerator.get_Current();
					honorDic2.Add(num3, current3.get_Value());
				}
			}
			this._geiLiDuiYou = roleInfo.GeiLiDuiYou;
			this._keJingDuiShou = roleInfo.KeJingDuiShou;
			this._haveExtraCoin = roleInfo.HaveExtraCoin();
			if (this._haveExtraCoin)
			{
				this._coinExpireHours = roleInfo.GetCoinExpireHours();
				this._coinWinCount = roleInfo.GetCoinWinCount();
			}
			this._haveExtraExp = roleInfo.HaveExtraExp();
			if (this._haveExtraExp)
			{
				this._expExpireHours = roleInfo.GetExpExpireHours();
				this._expWinCount = roleInfo.GetExpWinCount();
			}
			this.pvpExtraDetail = roleInfo.pvpDetail.stMultiExtraDetail;
			this.pvpAbilityDetail = roleInfo.pvpDetail.stRadarDetail;
			CAchieveInfo2 achieveInfo = CAchieveInfo2.GetAchieveInfo(roleInfo.logicWorldID, roleInfo.playerUllUID, false);
			if (achieveInfo != null)
			{
				if (achieveInfo.LastDoneTrophyRewardInfo != null)
				{
					this._trophyRewardInfoLevel = achieveInfo.LastDoneTrophyRewardInfo.Cfg.dwTrophyLvl;
				}
				else
				{
					DebugHelper.Assert(false, "cheieveInfo.LastDoneTrophyRewardInfo is null,wordID{0},uid{1}", new object[]
					{
						roleInfo.logicWorldID,
						roleInfo.playerUllUID
					});
				}
				this._trophyRank = achieveInfo.GetWorldRank();
				if (achieveInfo.SelectedTrophies != null)
				{
					Array.Copy(achieveInfo.SelectedTrophies, this._selectedTrophies, achieveInfo.SelectedTrophies.Length);
				}
				else
				{
					DebugHelper.Assert(false, "cheieveInfo.SelectedTrophies is null,wordID{0},uid{1}", new object[]
					{
						roleInfo.logicWorldID,
						roleInfo.playerUllUID
					});
				}
				this._mentorInfo = Utility.DeepCopyByReflection<COMDT_ACNT_MASTER_INFO>(roleInfo.m_mentorInfo);
				this._socialCardInfo = Utility.DeepCopyByReflection<COMDT_FRIEND_CARD>(roleInfo.m_socialFriendCard);
			}
			else
			{
				DebugHelper.Assert(false, "cheieveInfo is null,wordID{0},uid{1}", new object[]
				{
					roleInfo.logicWorldID,
					roleInfo.playerUllUID
				});
			}
		}

		public bool IsPrivacyOpen(COM_USER_PRIVACY_MASK mask)
		{
			return (this.m_userPrivacyBits & 1uL << (int)(mask & (COM_USER_PRIVACY_MASK)31)) > 0uL;
		}

		public void SetPrivacyBit(bool bOpen, COM_USER_PRIVACY_MASK mask)
		{
			if (bOpen)
			{
				this.m_userPrivacyBits |= 1uL << (int)(mask & (COM_USER_PRIVACY_MASK)31);
			}
			else
			{
				this.m_userPrivacyBits &= ~(1uL << (int)(mask & (COM_USER_PRIVACY_MASK)31));
			}
		}

		public void ConvertServerDetailData(CSDT_ACNT_DETAIL_INFO detailInfo)
		{
			this.isMasterData = false;
			this.ResetData();
			this._doubleKillCount = 0;
			this._trippleKillCount = 0;
			this._quataryKillCount = 0;
			this._pentaKillCount = 0;
			this._holyShitCount = 0;
			this._mvpCnt = 0;
			this._loseMvpCount = 0;
			if (detailInfo == null)
			{
				return;
			}
			this._playerName = StringHelper.UTF8BytesToString(ref detailInfo.szAcntName);
			this.m_personSign = StringHelper.UTF8BytesToString(ref detailInfo.szSignatureInfo);
			this.m_uuid = detailInfo.ullUid;
			this.m_iLogicWorldId = detailInfo.iLogicWorldId;
			this.m_vipInfo = new SCPKG_GAME_VIP_NTF();
			this.m_vipInfo.stGameVipClient = detailInfo.stGameVip;
			this._playerHeadUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref detailInfo.szOpenUrl);
			this._playerLevel = detailInfo.dwLevel;
			ResAcntExpInfo dataByKey = GameDataMgr.acntExpDatabin.GetDataByKey(this._playerLevel);
			this._playerNeedExp = dataByKey.dwNeedExp;
			this._playerExp = detailInfo.dwExp;
			this.creditScore = detailInfo.dwCreditValue;
			this.sumDelCreditValue = detailInfo.iSumDelCreditValue * -1;
			this.mostDelCreditType = detailInfo.dwMostDelCreditType;
			this._power = detailInfo.dwPower;
			this._pvpLevel = detailInfo.dwPvpLevel;
			this._playerPvpExp = detailInfo.dwPvpExp;
			this._gender = (COM_SNSGENDER)detailInfo.bGender;
			this._privilegeType = (COM_PRIVILEGE_TYPE)detailInfo.bPrivilege;
			this._rankGrade = detailInfo.bGradeOfRank;
			this._rankHistoryHighestGrade = detailInfo.bMaxGradeOfRank;
			this._rankClass = detailInfo.dwCurClassOfRank;
			this._rankHistoryHighestClass = detailInfo.stRankInfo.dwTopClassOfRank;
			this._wangZheCnt = detailInfo.dwWangZheCnt;
			this.GuildName = StringHelper.UTF8BytesToString(ref detailInfo.stGuildInfo.szGuildName);
			this.GuildState = (COM_PLAYER_GUILD_STATE)detailInfo.stGuildInfo.bGuildState;
			this.qqVipMask = detailInfo.dwQQVIPMask;
			int num = 0;
			while ((long)num < (long)((ulong)detailInfo.stStatistic.stKVDetail.dwNum))
			{
				COMDT_STATISTIC_KEY_VALUE_INFO cOMDT_STATISTIC_KEY_VALUE_INFO = detailInfo.stStatistic.stKVDetail.astKVDetail[num];
				RES_STATISTIC_SETTLE_DATA_TYPE dwKey = (RES_STATISTIC_SETTLE_DATA_TYPE)cOMDT_STATISTIC_KEY_VALUE_INFO.dwKey;
				switch (dwKey)
				{
				case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_MVP_CNT:
					this._mvpCnt = (int)cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue;
					break;
				case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_LOSE_SOUL:
					this._loseMvpCount = (int)cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue;
					break;
				case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_GODLIKE_CNT:
					this._holyShitCount = (int)cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue;
					break;
				case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_DOUBLE_KILL_CNT:
					this._doubleKillCount = (int)cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue;
					break;
				case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_TRIPLE_KILL_CNT:
					this._trippleKillCount = (int)cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue;
					break;
				default:
					if (dwKey != RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_QUATARY_KILL_CNT)
					{
						if (dwKey == RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_PENTA_KILL_CNT)
						{
							this._pentaKillCount = (int)cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue;
						}
					}
					else
					{
						this._quataryKillCount = (int)cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue;
					}
					break;
				}
				num++;
			}
			this._5V5TotalCount = (int)detailInfo.stStatistic.stFiveVsFiveInfo.dwTotalNum;
			this._5V5WinCount = (int)detailInfo.stStatistic.stFiveVsFiveInfo.dwWinNum;
			this._3V3TotalCount = (int)detailInfo.stStatistic.stThreeVsThreeInfo.dwTotalNum;
			this._3V3WinCount = (int)detailInfo.stStatistic.stThreeVsThreeInfo.dwWinNum;
			this._2V2TotalCount = (int)detailInfo.stStatistic.stTwoVsTwoInfo.dwTotalNum;
			this._2V2WinCount = (int)detailInfo.stStatistic.stTwoVsTwoInfo.dwWinNum;
			this._1V1TotalCount = (int)detailInfo.stStatistic.stOneVsOneInfo.dwTotalNum;
			this._1V1WinCount = (int)detailInfo.stStatistic.stOneVsOneInfo.dwWinNum;
			this._guildTotalCount = (int)detailInfo.stStatistic.stGuildMatch.dwTotalNum;
			this._guildWinCount = (int)detailInfo.stStatistic.stGuildMatch.dwWinNum;
			this._vsAiTotalCount = (int)detailInfo.stStatistic.stVsMachineInfo.dwTotalNum;
			this._vsAiWinCount = (int)detailInfo.stStatistic.stVsMachineInfo.dwWinNum;
			this._rankTotalCount = (int)detailInfo.stStatistic.stLadderInfo.dwTotalNum;
			this._rankWinCount = (int)detailInfo.stStatistic.stLadderInfo.dwWinNum;
			this._entertainmentTotalCount = (int)detailInfo.stStatistic.stEntertainmentInfo.dwTotalNum;
			this._entertainmentWinCount = (int)detailInfo.stStatistic.stEntertainmentInfo.dwWinNum;
			this._heroCnt = (int)detailInfo.stMostUsedHero.dwTotalHeroNum;
			this._skinCnt = (int)detailInfo.stMostUsedHero.dwTotalSkinNum;
			this._isOnLine = (detailInfo.bIsOnline != 0);
			if (this._mostUsedHeroList == null)
			{
				this._mostUsedHeroList = new ListView<COMDT_MOST_USED_HERO_INFO>();
			}
			else
			{
				this._mostUsedHeroList.Clear();
			}
			int num2 = (int)Mathf.Min(detailInfo.stMostUsedHero.dwHeroNum, (float)detailInfo.stMostUsedHero.astHeroInfoList.Length);
			for (int i = 0; i < num2; i++)
			{
				this._mostUsedHeroList.Add(detailInfo.stMostUsedHero.astHeroInfoList[i]);
			}
			this.SortMostUsedHeroList();
			if (detailInfo.stHonorInfo == null || detailInfo.stHonorInfo.bHonorCnt < 6)
			{
				Singleton<CRoleInfoManager>.instance.InsertHonorOnDuplicateUpdate(ref this._honorDic, 1, 0);
				Singleton<CRoleInfoManager>.instance.InsertHonorOnDuplicateUpdate(ref this._honorDic, 2, 0);
				Singleton<CRoleInfoManager>.instance.InsertHonorOnDuplicateUpdate(ref this._honorDic, 6, 0);
				Singleton<CRoleInfoManager>.instance.InsertHonorOnDuplicateUpdate(ref this._honorDic, 4, 0);
				Singleton<CRoleInfoManager>.instance.InsertHonorOnDuplicateUpdate(ref this._honorDic, 5, 0);
				Singleton<CRoleInfoManager>.instance.InsertHonorOnDuplicateUpdate(ref this._honorDic, 3, 0);
			}
			if (detailInfo.stHonorInfo != null)
			{
				for (int j = 0; j < (int)detailInfo.stHonorInfo.bHonorCnt; j++)
				{
					COMDT_HONORINFO cOMDT_HONORINFO = detailInfo.stHonorInfo.astHonorInfo[j];
					switch (cOMDT_HONORINFO.iHonorID)
					{
					case 1:
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
						Singleton<CRoleInfoManager>.instance.InsertHonorOnDuplicateUpdate(ref this._honorDic, cOMDT_HONORINFO.iHonorID, cOMDT_HONORINFO.iHonorPoint);
						break;
					}
				}
				COMDT_HONORINFO cOMDT_HONORINFO2 = new COMDT_HONORINFO();
				if (this._honorDic.TryGetValue(detailInfo.stHonorInfo.iCurUseHonorID, ref cOMDT_HONORINFO2))
				{
					if (cOMDT_HONORINFO2.iHonorLevel > 0)
					{
						this._selectedHonorId = detailInfo.stHonorInfo.iCurUseHonorID;
					}
					else
					{
						this._selectedHonorId = 0;
					}
				}
			}
			this._geiLiDuiYou = (int)detailInfo.stLikeNum.dwTeammateNum;
			this._keJingDuiShou = (int)detailInfo.stLikeNum.dwOpponentNum;
			this._haveExtraCoin = false;
			this._haveExtraExp = false;
			this.pvpExtraDetail = detailInfo.stStatistic.stMultiExtraDetail;
			this.pvpAbilityDetail = detailInfo.stStatistic.stRadarDetail;
			CAchieveInfo2 achieveInfo = CAchieveInfo2.GetAchieveInfo(detailInfo.iLogicWorldId, detailInfo.ullUid, false);
			achieveInfo.OnServerAchieveInfo(detailInfo.astShowAchievement, detailInfo.dwAchieveMentScore);
			this._trophyRewardInfoLevel = achieveInfo.LastDoneTrophyRewardInfo.Cfg.dwTrophyLvl;
			this._trophyRank = achieveInfo.GetWorldRank();
			Array.Copy(achieveInfo.SelectedTrophies, this._selectedTrophies, achieveInfo.SelectedTrophies.Length);
			this._mentorInfo = Utility.DeepCopyByReflection<COMDT_ACNT_MASTER_INFO>(detailInfo.stAcntMasterInfo);
			this.m_userPrivacyBits = detailInfo.ullUserPrivacyBits;
			this._socialCardInfo = Utility.DeepCopyByReflection<COMDT_FRIEND_CARD>(detailInfo.stFriendCard);
		}

		public static float Divide(uint a, uint b)
		{
			if (b == 0u)
			{
				return 0f;
			}
			return a / b;
		}

		public static float Divide(float a, float b)
		{
			if (b == 0f)
			{
				return 0f;
			}
			return a / b;
		}

		public static float Divide(ulong a, uint b)
		{
			if (b == 0u)
			{
				return 0f;
			}
			return a * 1000uL / (ulong)b / 1000f;
		}

		public static string Round(float value)
		{
			if (Math.Abs(value % 1f) < 1.401298E-45f)
			{
				return ((int)value).ToString("D");
			}
			return value.ToString("0.0");
		}

		public string Name()
		{
			return this._playerName;
		}

		public string HeadUrl()
		{
			return this._playerHeadUrl;
		}

		public uint Exp()
		{
			return this._playerExp;
		}

		public uint NeedExp()
		{
			return this._playerNeedExp;
		}

		public string HeadPath()
		{
			return string.Empty;
		}

		public uint FightingPower()
		{
			return this._power;
		}

		public uint PvpLevel()
		{
			return this._pvpLevel;
		}

		public uint PvpExp()
		{
			return this._playerPvpExp;
		}

		public uint PvpNeedExp()
		{
			ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint)((byte)this.PvpLevel()));
			if (dataByKey != null)
			{
				return dataByKey.dwNeedExp;
			}
			return 0u;
		}

		public byte GetRankGrade()
		{
			return this._rankGrade;
		}

		public byte GetRankClass()
		{
			return (byte)this._rankClass;
		}

		public byte GetHistoryHighestRankGrade()
		{
			return this._rankHistoryHighestGrade;
		}

		public uint GetHistoryHighestRankClass()
		{
			return this._rankHistoryHighestClass;
		}

		public int Pvp5V5TotalGameCnt()
		{
			return this._5V5TotalCount;
		}

		public int Pvp5V5WinGameCnt()
		{
			return this._5V5WinCount;
		}

		public float Pvp5V5Wins()
		{
			return CPlayerProfile.Divide((uint)this.Pvp5V5WinGameCnt(), (uint)this.Pvp5V5TotalGameCnt());
		}

		public int Pvp3V3TotalGameCnt()
		{
			return this._3V3TotalCount;
		}

		public int Pvp3V3WinGameCnt()
		{
			return this._3V3WinCount;
		}

		public float Pvp3V3Wins()
		{
			return CPlayerProfile.Divide((uint)this.Pvp3V3WinGameCnt(), (uint)this.Pvp3V3TotalGameCnt());
		}

		public int Pvp2V2TotalGameCnt()
		{
			return this._2V2TotalCount;
		}

		public int Pvp2V2WinGameCnt()
		{
			return this._2V2WinCount;
		}

		public float Pvp2V2Wins()
		{
			return CPlayerProfile.Divide((uint)this.Pvp2V2WinGameCnt(), (uint)this.Pvp2V2TotalGameCnt());
		}

		public int Pvp1V1TotalGameCnt()
		{
			return this._1V1TotalCount;
		}

		public int Pvp1V1WinGameCnt()
		{
			return this._1V1WinCount;
		}

		public float Pvp1V1Wins()
		{
			return CPlayerProfile.Divide((uint)this.Pvp1V1WinGameCnt(), (uint)this.Pvp1V1TotalGameCnt());
		}

		public int PvpGuildTotalGameCnt()
		{
			return this._guildTotalCount;
		}

		public int PvpGuildWinGameCnt()
		{
			return this._guildWinCount;
		}

		public float PvpGuildWins()
		{
			return CPlayerProfile.Divide((uint)this._guildWinCount, (uint)this._guildTotalCount);
		}

		public int PvmTotalGameCnt()
		{
			return this._vsAiTotalCount;
		}

		public int PvmWinGameCnt()
		{
			return this._vsAiWinCount;
		}

		public float PvmWins()
		{
			return CPlayerProfile.Divide((uint)this.PvmWinGameCnt(), (uint)this.PvmTotalGameCnt());
		}

		public int RankTotalGameCnt()
		{
			return this._rankTotalCount;
		}

		public int RankWinGameCnt()
		{
			return this._rankWinCount;
		}

		public float RankWins()
		{
			return CPlayerProfile.Divide((uint)this.RankWinGameCnt(), (uint)this.RankTotalGameCnt());
		}

		public int EntertainmentTotalGameCnt()
		{
			return this._entertainmentTotalCount;
		}

		public int EntertainmentWinGameCnt()
		{
			return this._entertainmentWinCount;
		}

		public float EntertainmentWins()
		{
			return CPlayerProfile.Divide((uint)this.EntertainmentWinGameCnt(), (uint)this.EntertainmentTotalGameCnt());
		}

		public uint DoubleKill()
		{
			return (uint)this._doubleKillCount;
		}

		public uint TripleKill()
		{
			return (uint)this._trippleKillCount;
		}

		public uint QuataryKill()
		{
			return (uint)this._quataryKillCount;
		}

		public uint PentaKill()
		{
			return (uint)this._pentaKillCount;
		}

		public uint HolyShit()
		{
			return (uint)this._holyShitCount;
		}

		public uint MVPCnt()
		{
			return (uint)this._mvpCnt;
		}

		public uint LoseSoulCnt()
		{
			return (uint)this._loseMvpCount;
		}

		public int HeroCnt()
		{
			return this._heroCnt;
		}

		public int SkinCnt()
		{
			return this._skinCnt;
		}

		public bool IsOnLine()
		{
			return this._isOnLine;
		}

		public COM_PRIVILEGE_TYPE PrivilegeType()
		{
			return this._privilegeType;
		}

		public COM_SNSGENDER Gender()
		{
			return this._gender;
		}

		public ListView<COMDT_MOST_USED_HERO_INFO> MostUsedHeroList()
		{
			return this._mostUsedHeroList;
		}

		public int GetSelectedHonorId()
		{
			return this._selectedHonorId;
		}

		public Dictionary<int, COMDT_HONORINFO> GetHonorDic()
		{
			return this._honorDic;
		}

		private void SortMostUsedHeroList()
		{
			if (this._mostUsedHeroList != null)
			{
				this._mostUsedHeroList.Sort(delegate(COMDT_MOST_USED_HERO_INFO b, COMDT_MOST_USED_HERO_INFO a)
				{
					if (a == null && b == null)
					{
						return 0;
					}
					if (a != null && b == null)
					{
						return 1;
					}
					if (a == null && b != null)
					{
						return -1;
					}
					if (a.dwGameWinNum + a.dwGameLoseNum > b.dwGameWinNum + b.dwGameLoseNum)
					{
						return 1;
					}
					if (a.dwGameWinNum + a.dwGameLoseNum == b.dwGameWinNum + b.dwGameLoseNum)
					{
						return 0;
					}
					return -1;
				});
			}
		}
	}
}
