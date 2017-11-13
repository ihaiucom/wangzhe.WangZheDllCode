using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CRoleInfo
	{
		public struct stCoinGetInfoDaily
		{
			public uint GetCntDaily;

			public uint LimitCntDaily;
		}

		private const int GET_FREEHERO_DELAY = 15;

		private const string GAMER_REDDOT = "GAMER_REDDOT";

		private uint m_level;

		private uint m_exp;

		private uint m_pvpLevel;

		private uint m_pvpExp;

		private uint m_needExp;

		private uint m_goldCoin;

		private ulong m_dianquan;

		private uint m_diamond;

		private uint m_JiFen;

		private uint m_burningCoin;

		private uint m_arenaCoin;

		private uint m_skinCoin;

		private uint m_symbolCoin;

		private uint m_maxActionPoint;

		private uint m_curActionPoint;

		private CrypticInt32 m_titleId;

		private CrypticInt32 m_headId;

		private string m_HeadUrl;

		private byte m_MaterialDirectBuyLimit;

		private uint m_expPool;

		private string m_Name;

		private string m_personSign;

		public stShopBuyDrawInfo[] m_freeDrawInfo;

		public int m_coinDrawIndex;

		public uint m_DiamondOpenBoxCnt;

		public uint m_payLevel;

		private uint m_vipFlags;

		private bool m_vipFlagsValid;

		private int m_vipReadTimer;

		private byte m_gameDifficult;

		private uint m_firstLoginTime;

		private long m_firstLoginZeroDay;

		public static int m_globalRefreshTimerSeq = -1;

		public uint m_initHeroId;

		public uint snsSwitchBits;

		public uint creditScore;

		public int sumDelCreditValue;

		public uint mostDelCreditType;

		private int m_newDayTimer = -1;

		private static float s_upToLoginSec;

		private static int s_sysTime;

		public int getFriendCoinCnt;

		public int GeiLiDuiYou;

		public int KeJingDuiShou;

		public uint m_goldWeekCur;

		public uint m_goldWeekLimit = 100u;

		public uint m_goldWeekMask;

		public uint m_expWeekCur;

		public uint m_expWeekLimit = 100u;

		private bool m_showGameRedDot;

		private uint m_otherStateBits;

		public enROLEINFO_TYPE m_roleType = enROLEINFO_TYPE.PLAYER;

		private CUseableContainer m_itemContainer;

		private DictionaryView<uint, CHeroInfo> heroDic = new DictionaryView<uint, CHeroInfo>();

		private Dictionary<uint, ulong> heroSkinDic = new Dictionary<uint, ulong>();

		public Dictionary<uint, uint> heroExperienceSkinDic = new Dictionary<uint, uint>();

		public List<COMDT_FREEHERO_DETAIL> freeHeroList = new List<COMDT_FREEHERO_DETAIL>();

		public ListView<COMDT_FREEHERO_INFO> freeHeroSymbolList = new ListView<COMDT_FREEHERO_INFO>();

		public uint freeHeroExpireTime;

		public List<uint> battleHeroList = new List<uint>();

		public PVE_ADV_COMPLETE_INFO[] pveLevelDetail = new PVE_ADV_COMPLETE_INFO[4];

		public CSDT_PVPDETAIL_INFO pvpDetail = new CSDT_PVPDETAIL_INFO();

		public Dictionary<int, COMDT_HONORINFO> honorDic = new Dictionary<int, COMDT_HONORINFO>();

		public int selectedHonorID;

		private BitArray GuidedStateBits;

		private BitArray NewbieAchieveBits;

		private bool bNewbieAchieveChanged;

		public COMDT_MOBA_INFO acntMobaInfo = new COMDT_MOBA_INFO();

		private BitArray _clientBits;

		private bool _clientBitsChanged;

		private Dictionary<int, stInBattleLevelBits> inBattleNewbieBits;

		public uint m_skillPoint;

		public uint m_maxSkillPt;

		public int m_skillPtRefreshSec;

		public uint m_updateTimeMSec;

		public byte m_rankGrade;

		public uint m_rankScore;

		public byte m_rankHistoryHighestGrade;

		public byte m_rankSeasonHighestGrade;

		public uint m_rankClass;

		public uint m_rankHistoryHighestClass;

		public uint m_rankSeasonHighestClass;

		public ulong m_rankCurSeasonStartTime;

		public uint m_WangZheCnt;

		public CSymbolInfo m_symbolInfo = new CSymbolInfo();

		private int GetFreeHeroTimer;

		public GuildBaseInfo m_baseGuildInfo;

		public GuildExtInfo m_extGuildInfo;

		public List<uint> m_arenaDefHeroList = new List<uint>();

		public COM_PRIVILEGE_TYPE m_privilegeType;

		public COM_SNSGENDER m_gender;

		public CLicenseInfo m_licenseInfo = new CLicenseInfo();

		public COMDT_ACNT_MASTER_INFO m_mentorInfo;

		public ulong m_userPrivacyBits;

		public COMDT_FRIEND_CARD m_socialFriendCard;

		public CRoleInfo.stCoinGetInfoDaily[] m_coinGetInfoDaily = new CRoleInfo.stCoinGetInfoDaily[3];

		public int m_nHeroSkinCount;

		public COMDT_CHGNAME_CD chgNameCD = new COMDT_CHGNAME_CD();

		public COMDT_RECENT_USED_HERO recentUseHero = new COMDT_RECENT_USED_HERO();

		public COM_ACNT_OLD_TYPE m_AcntOldType;

		private uint _extraTimeCoin;

		private uint _extraWinCoin;

		private uint _extraTimeExp;

		private uint _extraWinExp;

		private uint _extraWinCoinValue;

		private uint _extraWinExpValue;

		private uint _extraCoinHours;

		private uint _extraExpHours;

		private COMDT_MOST_USED_HERO_DETAIL stMostUsedHero = new COMDT_MOST_USED_HERO_DETAIL();

		public CCustomRcmdEquipInfo m_rcmdEquipInfo;

		private SCPKG_GAME_VIP_NTF m_vipInfo = new SCPKG_GAME_VIP_NTF();

		public bool IsNoAskFor;

		public uint SendAskforReqTime;

		public uint SendAskforReqCnt;

		private uint _nextFirstWinAvailableTime;

		private uint _firstWinLv = 1u;

		public uint dailyPvpCnt
		{
			get;
			set;
		}

		public uint OtherStatebBits
		{
			get
			{
				return this.m_otherStateBits;
			}
			set
			{
				this.m_otherStateBits = value;
			}
		}

		public bool ShowGameRedDot
		{
			get
			{
				return this.m_showGameRedDot;
			}
			set
			{
				ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
				if (accountInfo == null)
				{
					return;
				}
				if (this.m_showGameRedDot != value)
				{
					this.m_showGameRedDot = value;
					if (value)
					{
						if (PlayerPrefs.HasKey("GAMER_REDDOT"))
						{
							string @string = PlayerPrefs.GetString("GAMER_REDDOT");
							if (@string.Contains(accountInfo.OpenId))
							{
								string[] array = @string.Split(new char[]
								{
									'_'
								});
								long secondsFromUtcStart = 0L;
								if (array != null)
								{
									int i = 0;
									int num = array.Length;
									while (i < num)
									{
										if (array[i] == accountInfo.OpenId && long.TryParse(array[i + 1], ref secondsFromUtcStart))
										{
											long secondsFromUtcStart2 = (long)CRoleInfo.GetCurrentUTCTime();
											this.m_showGameRedDot = !Utility.IsSameWeek(secondsFromUtcStart, secondsFromUtcStart2);
											break;
										}
										i++;
									}
								}
							}
						}
					}
					else
					{
						long num2 = (long)CRoleInfo.GetCurrentUTCTime();
						string text = string.Empty;
						if (PlayerPrefs.HasKey("GAMER_REDDOT"))
						{
							text = PlayerPrefs.GetString("GAMER_REDDOT");
							if (text.Contains(accountInfo.OpenId))
							{
								string[] array2 = text.Split(new char[]
								{
									'_'
								});
								if (array2 != null)
								{
									int j = 0;
									int num3 = array2.Length;
									while (j < num3)
									{
										if (array2[j] == accountInfo.OpenId)
										{
											array2[j + 1] = num2.ToString();
											break;
										}
										j++;
									}
									text = string.Join("_", array2);
								}
							}
							else
							{
								text = string.Format("{0}_{1}_{2}", text, accountInfo.OpenId, num2);
							}
						}
						else
						{
							text = string.Format("{0}_{1}", accountInfo.OpenId, num2);
						}
						PlayerPrefs.SetString("GAMER_REDDOT", text);
						PlayerPrefs.Save();
					}
					Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.GAMER_REDDOT_CHANGE);
				}
			}
		}

		public bool bCanRecvCoin
		{
			get
			{
				uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(17u).dwConfValue;
				return (long)this.getFriendCoinCnt < (long)((ulong)dwConfValue);
			}
		}

		public bool bFirstLoginToday
		{
			get;
			private set;
		}

		public uint BattlePower
		{
			get
			{
				ListView<CHeroInfo> listView = new ListView<CHeroInfo>(this.heroDic.Count);
				DictionaryView<uint, CHeroInfo>.Enumerator enumerator = this.heroDic.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ListView<CHeroInfo> listView2 = listView;
					KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
					listView2.Add(current.get_Value());
				}
				ushort num = (ushort)listView.Count;
				uint num2 = 0u;
				for (ushort num3 = 0; num3 < num; num3 += 1)
				{
					CHeroInfo cHeroInfo = listView[(int)num3];
					num2 = this.UInt32ChgAdjust(num2, cHeroInfo.GetCombatEft());
				}
				return num2;
			}
		}

		public int logicWorldID
		{
			get;
			set;
		}

		public ulong playerUllUID
		{
			get;
			set;
		}

		public uint Level
		{
			get
			{
				return this.m_level;
			}
		}

		public uint Exp
		{
			get
			{
				return this.m_exp;
			}
			set
			{
				this.m_exp = value;
				while (this.m_exp >= this.m_needExp)
				{
					this.m_exp -= this.m_needExp;
					this.SetLevel(this.UInt32ChgAdjust(this.Level, 1), CS_ACNT_UPDATE_FROMTYPE.CS_ACNT_UPDATE_FROMTYPE_NULL);
				}
			}
		}

		public uint NeedExp
		{
			get
			{
				return this.m_needExp;
			}
		}

		public uint PvpLevel
		{
			get
			{
				return this.m_pvpLevel;
			}
		}

		public uint PvpExp
		{
			get
			{
				return this.m_pvpExp;
			}
			set
			{
				this.m_pvpExp = value;
			}
		}

		public uint PvpNeedExp
		{
			get
			{
				ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint)((byte)this.m_pvpLevel));
				if (dataByKey != null)
				{
					return dataByKey.dwNeedExp;
				}
				return 0u;
			}
		}

		public uint GoldCoin
		{
			get
			{
				return this.m_goldCoin;
			}
			set
			{
				this.m_goldCoin = value;
				Singleton<EventRouter>.instance.BroadCastEvent("MasterCurrencyChanged");
			}
		}

		public ulong DianQuan
		{
			get
			{
				return this.m_dianquan;
			}
			set
			{
				this.m_dianquan = value;
				Singleton<EventRouter>.instance.BroadCastEvent("MasterAttributesChanged");
				Singleton<EventRouter>.instance.BroadCastEvent("MasterCurrencyChanged");
			}
		}

		public uint JiFen
		{
			get
			{
				return this.m_JiFen;
			}
		}

		public uint Diamond
		{
			get
			{
				return this.m_diamond;
			}
			set
			{
				this.m_diamond = value;
				Singleton<EventRouter>.instance.BroadCastEvent("MasterAttributesChanged");
				Singleton<EventRouter>.instance.BroadCastEvent("MasterCurrencyChanged");
			}
		}

		public uint ExpPool
		{
			get
			{
				return this.m_expPool;
			}
			set
			{
				this.m_expPool = value;
			}
		}

		public uint BurningCoin
		{
			get
			{
				return this.m_burningCoin;
			}
			set
			{
				this.m_burningCoin = value;
			}
		}

		public uint ArenaCoin
		{
			get
			{
				return this.m_arenaCoin;
			}
			set
			{
				this.m_arenaCoin = value;
			}
		}

		public uint SymbolCoin
		{
			get
			{
				return this.m_symbolCoin;
			}
			set
			{
				this.m_symbolCoin = value;
			}
		}

		public uint SkinCoin
		{
			get
			{
				return this.m_skinCoin;
			}
			set
			{
				this.m_skinCoin = value;
			}
		}

		public uint MaxActionPoint
		{
			get
			{
				return this.m_maxActionPoint;
			}
		}

		public uint CurActionPoint
		{
			get
			{
				return this.m_curActionPoint;
			}
			set
			{
				this.m_curActionPoint = value;
			}
		}

		public int TitleId
		{
			get
			{
				return this.m_titleId;
			}
		}

		public int HeadId
		{
			get
			{
				return this.m_headId;
			}
		}

		public string HeadUrl
		{
			get
			{
				return this.m_HeadUrl;
			}
			set
			{
				this.m_HeadUrl = value;
			}
		}

		public byte MaterialDirectBuyLimit
		{
			get
			{
				return this.m_MaterialDirectBuyLimit;
			}
			set
			{
				this.m_MaterialDirectBuyLimit = ((value < 0) ? 0 : value);
			}
		}

		public string Name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		public string PersonSign
		{
			get
			{
				return this.m_personSign;
			}
			set
			{
				this.m_personSign = value;
			}
		}

		public COMDT_MOST_USED_HERO_DETAIL MostUsedHeroDetail
		{
			get
			{
				return this.stMostUsedHero;
			}
		}

		public COM_ACNT_NEWBIE_TYPE GameDifficult
		{
			get
			{
				return (COM_ACNT_NEWBIE_TYPE)this.m_gameDifficult;
			}
			set
			{
				this.m_gameDifficult = (byte)value;
			}
		}

		public uint AccountRegisterTime
		{
			get
			{
				return this.m_firstLoginTime;
			}
		}

		public long AccountRegisterTime_ZeroDay
		{
			get
			{
				return this.m_firstLoginZeroDay;
			}
		}

		public CRoleInfo(enROLEINFO_TYPE type, ulong uuID, int logicWorldID = 0)
		{
			this.playerUllUID = uuID;
			this.logicWorldID = logicWorldID;
			this.m_roleType = type;
			this.m_itemContainer = new CUseableContainer(enCONTAINER_TYPE.ITEM);
			this.m_baseGuildInfo = new GuildBaseInfo();
			this.m_extGuildInfo = new GuildExtInfo();
			this.m_vipFlags = 0u;
			this.m_vipFlagsValid = false;
			if (this.m_licenseInfo == null)
			{
				this.m_licenseInfo = new CLicenseInfo();
			}
			this.m_licenseInfo.InitLicenseCfgInfo();
			for (int i = 0; i < 3; i++)
			{
				this.m_coinGetInfoDaily[i] = default(CRoleInfo.stCoinGetInfoDaily);
			}
			this.m_showGameRedDot = false;
		}

		public void SetNobeInfo(SCPKG_GAME_VIP_NTF info)
		{
			this.m_vipInfo = info;
		}

		public SCPKG_GAME_VIP_NTF GetNobeInfo()
		{
			return this.m_vipInfo;
		}

		public void Clear()
		{
			this.getFriendCoinCnt = 0;
			this.heroDic.Clear();
			this.heroSkinDic.Clear();
			this.heroExperienceSkinDic.Clear();
			this.GetUseableContainer(enCONTAINER_TYPE.ITEM).Clear();
			this.honorDic.Clear();
			CAchieveInfo2.Clear();
		}

		public bool IsGuidedStateSet(int inIndex)
		{
			return this.GuidedStateBits.Get(inIndex);
		}

		public bool IsTrainingLevelFin()
		{
			bool flag = this.IsGuidedStateSet(83);
			flag &= this.IsGuidedStateSet(84);
			flag &= this.IsGuidedStateSet(85);
			flag &= this.IsGuidedStateSet(98);
			return flag & this.IsGuidedStateSet(0);
		}

		public void SetGuidedStateSet(int inIndex, bool bOpen)
		{
			this.GuidedStateBits.Set(inIndex, bOpen);
			if (bOpen)
			{
				NewbieGuideSkipConditionType newbieGuideSkipConditionType = NewbieGuideCheckSkipConditionUtil.TranslateToSkipCond(inIndex);
				if (newbieGuideSkipConditionType != NewbieGuideSkipConditionType.Invalid)
				{
					MonoSingleton<NewbieGuideManager>.GetInstance().CheckSkipCondition(newbieGuideSkipConditionType, new uint[0]);
				}
			}
		}

		public void InitGuidedStateBits(COMDT_NEWBIE_STATUS_BITS bits)
		{
			int num = bits.BitsDetail.Length * 64;
			bool[] array = new bool[num];
			for (int i = 0; i < num; i++)
			{
				int num2 = i / 64;
				int num3 = i % 64;
				array[i] = ((bits.BitsDetail[num2] & 1uL << num3) > 0uL);
			}
			this.GuidedStateBits = new BitArray(array);
		}

		public bool IsNewbieAchieveSet(int inIndex)
		{
			return inIndex < this.NewbieAchieveBits.get_Count() && this.NewbieAchieveBits.Get(inIndex);
		}

		public void SetNewbieAchieve(int inIndex, bool bOpen, bool bSync = false)
		{
			if (inIndex >= this.NewbieAchieveBits.get_Count())
			{
				return;
			}
			if (this.IsNewbieAchieveSet(inIndex) != bOpen)
			{
				this.NewbieAchieveBits.Set(inIndex, bOpen);
				this.bNewbieAchieveChanged = true;
			}
			if (bSync)
			{
				this.SyncNewbieAchieveToSvr(false);
			}
		}

		public void InitNewbieAchieveBits(COMDT_CLIENT_BITS bits)
		{
			int num = bits.BitsDetail.Length * 64;
			bool[] array = new bool[num];
			for (int i = 0; i < num; i++)
			{
				int num2 = i / 64;
				int num3 = i % 64;
				array[i] = ((bits.BitsDetail[num2] & 1uL << num3) > 0uL);
			}
			this.NewbieAchieveBits = new BitArray(array);
		}

		public void InitClientBits(COMDT_NEWCLIENT_BITS bits)
		{
			int num = bits.BitsDetail.Length * 64;
			bool[] array = new bool[num];
			for (int i = 0; i < num; i++)
			{
				int num2 = i / 64;
				int num3 = i % 64;
				array[i] = ((bits.BitsDetail[num2] & 1uL << num3) > 0uL);
			}
			this._clientBits = new BitArray(array);
		}

		public void SyncNewbieAchieveToSvr(bool resetOldPlayerGuided = false)
		{
			if (this.bNewbieAchieveChanged || resetOldPlayerGuided)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4000u);
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < 64; j++)
					{
						if (!resetOldPlayerGuided || i != 0 || j != 17)
						{
							cSPkg.stPkgData.stUpdateClientBitsNtf.stClientBits.BitsDetail[i] += (this.NewbieAchieveBits.get_Item(64 * i + j) ? (1uL << j) : 0uL);
						}
					}
				}
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
				this.bNewbieAchieveChanged = false;
			}
		}

		public bool IsClientBitsSet(int inIndex)
		{
			return this._clientBits.Get(inIndex);
		}

		public void SetClientBits(int inIndex, bool bOpen, bool bSync = false)
		{
			if (this.IsClientBitsSet(inIndex) != bOpen)
			{
				this._clientBits.Set(inIndex, bOpen);
				this._clientBitsChanged = true;
			}
			if (bSync)
			{
				this.SyncClientBitsToSvr();
			}
		}

		public void SyncClientBitsToSvr()
		{
			if (this._clientBitsChanged)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4003u);
				for (int i = 0; i < 5; i++)
				{
					for (int j = 0; j < 64; j++)
					{
						cSPkg.stPkgData.stUpdNewClientBits.stClientBits.BitsDetail[i] += (this._clientBits.get_Item(64 * i + j) ? (1uL << j) : 0uL);
					}
				}
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
				this._clientBitsChanged = false;
			}
		}

		public bool IsOldPlayer()
		{
			return this.GuidedStateBits != null && this.GuidedStateBits.get_Item(105);
		}

		public bool IsOldPlayerGuided()
		{
			return this.NewbieAchieveBits != null && this.NewbieAchieveBits.get_Item(17);
		}

		private void SetInBattleNewbieBit(int iLevelId, uint ageId, bool bIsLastAge)
		{
			stInBattleLevelBits stInBattleLevelBits;
			if (!this.inBattleNewbieBits.TryGetValue(iLevelId, ref stInBattleLevelBits))
			{
				stInBattleLevelBits = default(stInBattleLevelBits);
				stInBattleLevelBits.bReportFinished = (bIsLastAge ? 1 : 0);
				stInBattleLevelBits.finishedDetail = new List<uint>();
				stInBattleLevelBits.finishedDetail.Add(ageId);
				this.inBattleNewbieBits.Add(iLevelId, stInBattleLevelBits);
			}
			else
			{
				stInBattleLevelBits.bReportFinished = (bIsLastAge ? 1 : 0);
				stInBattleLevelBits.finishedDetail.Add(ageId);
			}
		}

		public void InitInBattleNewbieBits(COMDT_INBATTLE_NEWBIE_BITS_DETAIL inBattleBits)
		{
			this.inBattleNewbieBits = new Dictionary<int, stInBattleLevelBits>();
			for (int i = 0; i < (int)inBattleBits.bLevelNum; i++)
			{
				COMDT_INBATTLE_NEWBIE_BITS_INFO cOMDT_INBATTLE_NEWBIE_BITS_INFO = inBattleBits.astLevelDetail[i];
				if (!this.inBattleNewbieBits.ContainsKey(cOMDT_INBATTLE_NEWBIE_BITS_INFO.iLevelID))
				{
					stInBattleLevelBits stInBattleLevelBits = default(stInBattleLevelBits);
					stInBattleLevelBits.bReportFinished = cOMDT_INBATTLE_NEWBIE_BITS_INFO.bReportFinished;
					stInBattleLevelBits.finishedDetail = new List<uint>();
					for (int j = 0; j < (int)cOMDT_INBATTLE_NEWBIE_BITS_INFO.bFinishedNum; j++)
					{
						stInBattleLevelBits.finishedDetail.Add(cOMDT_INBATTLE_NEWBIE_BITS_INFO.FinishedDetail[j]);
					}
					this.inBattleNewbieBits.Add(cOMDT_INBATTLE_NEWBIE_BITS_INFO.iLevelID, stInBattleLevelBits);
				}
			}
		}

		public void ReqSetInBattleNewbieBit(uint id, bool bIsLastAge, int time = 0)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (!Singleton<BattleLogic>.GetInstance().isRuning)
			{
				return;
			}
			if (curLvelContext != null && curLvelContext.IsGameTypeGuide())
			{
				stInBattleLevelBits stInBattleLevelBits;
				if (this.inBattleNewbieBits.TryGetValue(curLvelContext.m_mapID, ref stInBattleLevelBits))
				{
					if (stInBattleLevelBits.bReportFinished > 0)
					{
						return;
					}
					if (stInBattleLevelBits.finishedDetail.Contains(id))
					{
						return;
					}
				}
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4309u);
				cSPkg.stPkgData.stDyeInBattleNewbieBitReq.iLevelID = curLvelContext.m_mapID;
				cSPkg.stPkgData.stDyeInBattleNewbieBitReq.dwFinishedAgeID = id;
				cSPkg.stPkgData.stDyeInBattleNewbieBitReq.dwFinishTime = (uint)time;
				cSPkg.stPkgData.stDyeInBattleNewbieBitReq.bIsLastAge = (bIsLastAge ? 1 : 0);
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
				this.SetInBattleNewbieBit(curLvelContext.m_mapID, id, bIsLastAge);
			}
		}

		public uint GetFirstHeroId()
		{
			return this.m_initHeroId;
		}

		public uint GetGuideLevel2FadeHeroId()
		{
			uint result = 0u;
			if (GameDataMgr.globalInfoDatabin != null)
			{
				ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(102u);
				if (dataByKey != null)
				{
					result = dataByKey.dwConfValue;
				}
			}
			return result;
		}

		public CUseableContainer GetUseableContainer(enCONTAINER_TYPE type)
		{
			CUseableContainer result = null;
			if (type == enCONTAINER_TYPE.ITEM)
			{
				result = this.m_itemContainer;
			}
			return result;
		}

		public void SetPvELevelInfo(ref COMDT_ACNT_LEVEL_COMPLETE_DETAIL detail, uint ServerTime)
		{
			CAdventureSys.LEVEL_DIFFICULT_OPENED = (int)detail.bLastOpenDiffType;
			CAdventureSys.CHAPTER_NUM = Math.Min((int)detail.bChapterNum, GameDataMgr.chapterInfoDatabin.Count());
			for (int i = 0; i < CAdventureSys.LEVEL_DIFFICULT_OPENED; i++)
			{
				this.pveLevelDetail[i] = new PVE_ADV_COMPLETE_INFO();
			}
			for (int j = 0; j < CAdventureSys.CHAPTER_NUM; j++)
			{
				COMDT_CHAPTER_COMPLETE_INFO cOMDT_CHAPTER_COMPLETE_INFO = detail.astChapterDetail[j];
				for (int k = 0; k < CAdventureSys.LEVEL_DIFFICULT_OPENED; k++)
				{
					PVE_CHAPTER_COMPLETE_INFO pVE_CHAPTER_COMPLETE_INFO = this.pveLevelDetail[k].ChapterDetailList[j];
					pVE_CHAPTER_COMPLETE_INFO.bLevelNum = cOMDT_CHAPTER_COMPLETE_INFO.bLevelNum;
					pVE_CHAPTER_COMPLETE_INFO.bIsGetBonus = cOMDT_CHAPTER_COMPLETE_INFO.astDiffDetail[k].bGetBonus;
				}
				for (byte b = 0; b < cOMDT_CHAPTER_COMPLETE_INFO.bLevelNum; b += 1)
				{
					for (int l = 0; l < CAdventureSys.LEVEL_DIFFICULT_OPENED; l++)
					{
						PVE_LEVEL_COMPLETE_INFO pVE_LEVEL_COMPLETE_INFO = this.pveLevelDetail[l].ChapterDetailList[j].LevelDetailList[(int)b];
						COMDT_LEVEL_COMPLETE_INFO cOMDT_LEVEL_COMPLETE_INFO = cOMDT_CHAPTER_COMPLETE_INFO.astLevelDetail[(int)b];
						pVE_LEVEL_COMPLETE_INFO.iLevelID = cOMDT_LEVEL_COMPLETE_INFO.iLevelID;
						pVE_LEVEL_COMPLETE_INFO.levelStatus = cOMDT_LEVEL_COMPLETE_INFO.astDiffDetail[l].bLevelStatus;
						pVE_LEVEL_COMPLETE_INFO.bStarBits = cOMDT_LEVEL_COMPLETE_INFO.astDiffDetail[l].bStarBits;
					}
				}
			}
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.PVE_LEVEL_DETAIL_CHANGED);
		}

		public void SetHeroInfo(SCPKG_ACNTHEROINFO_NTY ntfHeroInfo)
		{
			Debug.Log("SetHeroInfo..." + ntfHeroInfo.stHeroInfo.dwHeroNum);
			this.heroDic.Clear();
			int num = 0;
			while ((long)num < (long)((ulong)ntfHeroInfo.stHeroInfo.dwHeroNum))
			{
				this.InitHero(ntfHeroInfo.stHeroInfo.astHeroInfoList[num]);
				num++;
			}
			this.InitBattleHeroList(ntfHeroInfo.stBattleListInfo);
			this.InitAllHeroSkin(ntfHeroInfo.stSkinInfo);
			this.InitAllHeroExperienceSkin(ntfHeroInfo.stLimitSkinInfo);
			this.m_initHeroId = ntfHeroInfo.stHeroCtrlInfo.dwInitHeroID;
			this.m_arenaDefHeroList.Clear();
			if (ntfHeroInfo.stHeroCtrlInfo.stBattleListOfArena.wHeroCnt > 0)
			{
				this.m_arenaDefHeroList.AddRange(ntfHeroInfo.stHeroCtrlInfo.stBattleListOfArena.BattleHeroList);
			}
			if (this.m_rcmdEquipInfo == null)
			{
				this.m_rcmdEquipInfo = new CCustomRcmdEquipInfo();
			}
			this.m_rcmdEquipInfo.InitializeCustomRecommendEquip(ntfHeroInfo.stSelfDefineEquipInfo);
			this.m_nHeroSkinCount = this.GetHeroSkinCount(false);
		}

		public void SetHeroSelSkillID(uint heroID, uint selSkillID)
		{
			CHeroInfo heroInfo = this.GetHeroInfo(heroID, true);
			if (heroInfo != null)
			{
				heroInfo.skillInfo.SelSkillID = selSkillID;
			}
			else if (this.IsFreeHero(heroID))
			{
				COMDT_FREEHERO_INFO cOMDT_FREEHERO_INFO = this.GetFreeHeroSymbol(heroID);
				if (cOMDT_FREEHERO_INFO != null)
				{
					cOMDT_FREEHERO_INFO.dwSkillID = selSkillID;
				}
				else
				{
					cOMDT_FREEHERO_INFO = new COMDT_FREEHERO_INFO();
					cOMDT_FREEHERO_INFO.dwHeroID = heroID;
					cOMDT_FREEHERO_INFO.dwSkillID = selSkillID;
					this.freeHeroSymbolList.Add(cOMDT_FREEHERO_INFO);
				}
			}
		}

		private void InitBattleHeroList(COMDT_BATTLELIST_LIST battleHeroList)
		{
		}

		private void InitAllHeroSkin(COMDT_HERO_SKIN_LIST heroSkinList)
		{
			this.heroSkinDic.Clear();
			int num = 0;
			while ((long)num < (long)((ulong)heroSkinList.dwHeroNum))
			{
				if (!this.heroSkinDic.ContainsKey(heroSkinList.astHeroSkinList[num].dwHeroID))
				{
					this.heroSkinDic.Add(heroSkinList.astHeroSkinList[num].dwHeroID, heroSkinList.astHeroSkinList[num].ullSkinBits);
				}
				num++;
			}
		}

		private void InitAllHeroExperienceSkin(COMDT_HERO_LIMIT_SKIN_LIST heroExperienceSkinList)
		{
			this.heroExperienceSkinDic.Clear();
			int num = 0;
			while ((long)num < (long)((ulong)heroExperienceSkinList.dwNum))
			{
				if (!this.heroExperienceSkinDic.ContainsKey(heroExperienceSkinList.astSkinList[num].dwSkinID))
				{
					this.heroExperienceSkinDic.Add(heroExperienceSkinList.astSkinList[num].dwSkinID, heroExperienceSkinList.astSkinList[num].dwDeadLine);
				}
				num++;
			}
		}

		public void InitHero(COMDT_HEROINFO heroInfo)
		{
			if (GameDataMgr.heroDatabin.GetDataByKey(heroInfo.stCommonInfo.dwHeroID) == null)
			{
				return;
			}
			if (!this.heroDic.ContainsKey(heroInfo.stCommonInfo.dwHeroID))
			{
				CHeroInfo value = new CHeroInfo();
				this.heroDic.Add(heroInfo.stCommonInfo.dwHeroID, value);
			}
			this.heroDic[heroInfo.stCommonInfo.dwHeroID].Init(this.playerUllUID, heroInfo);
		}

		public void SetHeroSkinData(COMDT_HERO_SKIN heroSkin)
		{
			if (!this.heroSkinDic.ContainsKey(heroSkin.dwHeroID))
			{
				this.heroSkinDic.Add(heroSkin.dwHeroID, heroSkin.ullSkinBits);
			}
			else
			{
				this.heroSkinDic.set_Item(heroSkin.dwHeroID, heroSkin.ullSkinBits);
			}
		}

		public DictionaryView<uint, CHeroInfo> GetHeroInfoDic()
		{
			return this.heroDic;
		}

		public bool GetHeroInfo(uint id, out CHeroInfo info, bool isIncludeValidExperienceHero = false)
		{
			if (isIncludeValidExperienceHero)
			{
				if (this.IsOwnHero(id) || this.IsValidExperienceHero(id))
				{
					info = this.heroDic[id];
					return true;
				}
			}
			else if (this.IsOwnHero(id))
			{
				info = this.heroDic[id];
				return true;
			}
			info = null;
			return false;
		}

		public CHeroInfo GetHeroInfo(uint id, bool isIncludeValidExperienceHero = false)
		{
			if (isIncludeValidExperienceHero)
			{
				if (this.IsOwnHero(id) || this.IsValidExperienceHero(id))
				{
					return this.heroDic[id];
				}
			}
			else if (this.IsOwnHero(id))
			{
				return this.heroDic[id];
			}
			return null;
		}

		public bool IsCanUseHero(uint heroId)
		{
			return this.IsHaveHero(heroId, true) || this.IsFreeHero(heroId);
		}

		public bool IsHaveHero(uint id, bool isIncludeValidExperienceHero)
		{
			if (isIncludeValidExperienceHero)
			{
				return this.IsOwnHero(id) || this.IsValidExperienceHero(id);
			}
			return this.IsOwnHero(id);
		}

		public int GetHaveHeroCount(bool isIncludeValidExperienceHero)
		{
			int num = 0;
			DictionaryView<uint, CHeroInfo>.Enumerator enumerator = this.heroDic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (isIncludeValidExperienceHero)
				{
					KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
					if (!this.IsOwnHero(current.get_Key()))
					{
						KeyValuePair<uint, CHeroInfo> current2 = enumerator.Current;
						if (!this.IsValidExperienceHero(current2.get_Key()))
						{
							continue;
						}
					}
					num++;
				}
				else
				{
					KeyValuePair<uint, CHeroInfo> current3 = enumerator.Current;
					if (this.IsOwnHero(current3.get_Key()))
					{
						num++;
					}
				}
			}
			return num;
		}

		public int GetHaveHeroCountWithoutBanHeroID(bool isIncludeValidExperienceHero, byte mapType, uint mapID)
		{
			ResBanHeroConf dataByKey = GameDataMgr.banHeroBin.GetDataByKey(GameDataMgr.GetDoubleKey((uint)mapType, mapID));
			List<uint> list = new List<uint>();
			if (dataByKey != null)
			{
				for (int i = 0; i < dataByKey.BanHero.Length; i++)
				{
					if (dataByKey.BanHero[i] != 0u)
					{
						list.Add(dataByKey.BanHero[i]);
					}
				}
			}
			int num = 0;
			DictionaryView<uint, CHeroInfo>.Enumerator enumerator = this.heroDic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (isIncludeValidExperienceHero)
				{
					KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
					if (!this.IsOwnHero(current.get_Key()))
					{
						KeyValuePair<uint, CHeroInfo> current2 = enumerator.Current;
						if (!this.IsValidExperienceHero(current2.get_Key()))
						{
							continue;
						}
					}
					List<uint> list2 = list;
					KeyValuePair<uint, CHeroInfo> current3 = enumerator.Current;
					if (!list2.Contains(current3.get_Key()))
					{
						num++;
					}
				}
				else
				{
					KeyValuePair<uint, CHeroInfo> current4 = enumerator.Current;
					if (this.IsOwnHero(current4.get_Key()))
					{
						List<uint> list3 = list;
						KeyValuePair<uint, CHeroInfo> current5 = enumerator.Current;
						if (!list3.Contains(current5.get_Key()))
						{
							num++;
						}
					}
				}
			}
			return num;
		}

		public bool IsHaveHeroSkin(uint heroId, uint skinId, bool isIncludeValidExperienceSkin = false)
		{
			if (this.heroSkinDic.ContainsKey(heroId))
			{
				ulong num = this.heroSkinDic.get_Item(heroId);
				if ((num & 1uL << (int)skinId) > 0uL)
				{
					return true;
				}
			}
			return isIncludeValidExperienceSkin && this.IsValidExperienceSkin(heroId, skinId);
		}

		public bool IsHaveHeroSkin(uint skinUniId, bool isIncludeValidExperienceSkin = false)
		{
			uint heroId;
			uint skinId;
			CSkinInfo.ResolveHeroSkin(skinUniId, out heroId, out skinId);
			return this.IsHaveHeroSkin(heroId, skinId, isIncludeValidExperienceSkin);
		}

		public int GetHeroSkinCount(bool isIncludeValidExperienceHero)
		{
			int num = 0;
			Dictionary<uint, ulong>.Enumerator enumerator = this.heroSkinDic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, ulong> current = enumerator.get_Current();
				ulong value = current.get_Value();
				for (int i = 1; i < 20; i++)
				{
					if ((value & 1uL << i) > 0uL)
					{
						num++;
					}
				}
			}
			if (isIncludeValidExperienceHero)
			{
				num += this.heroExperienceSkinDic.get_Count();
			}
			return num;
		}

		public bool IsCanUseSkin(uint heroId, uint skinId)
		{
			return this.IsCanUseHero(heroId) && (skinId == 0u || this.IsHaveHeroSkin(heroId, skinId, true));
		}

		public bool CheckHeroBuyable(uint inHeroId, RES_SHOPBUY_COINTYPE coinType)
		{
			IHeroData heroData = CHeroDataFactory.CreateHeroData(inHeroId);
			if (heroData == null)
			{
				return false;
			}
			if (heroData.bPlayerOwn)
			{
				return false;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			ResHeroShop resHeroShop = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(inHeroId, out resHeroShop);
			if (masterRoleInfo == null || resHeroShop == null)
			{
				return false;
			}
			switch (coinType)
			{
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
				if (masterRoleInfo.DianQuan >= (ulong)resHeroShop.dwBuyCoupons)
				{
					return true;
				}
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
				if (masterRoleInfo.GoldCoin >= resHeroShop.dwBuyCoin)
				{
					return true;
				}
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
				if (masterRoleInfo.BurningCoin >= resHeroShop.dwBuyBurnCoin)
				{
					return true;
				}
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
				if (masterRoleInfo.ArenaCoin >= resHeroShop.dwBuyArenaCoin)
				{
					return true;
				}
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND:
				if (masterRoleInfo.Diamond >= resHeroShop.dwBuyDiamond)
				{
					return true;
				}
				break;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_MIXPAY:
				if (masterRoleInfo.DianQuan + (ulong)masterRoleInfo.Diamond >= (ulong)resHeroShop.dwBuyDiamond)
				{
					return true;
				}
				break;
			}
			return false;
		}

		public void SetHeroExp(uint id, int level, int exp)
		{
			CHeroInfo cHeroInfo;
			if (this.GetHeroInfo(id, out cHeroInfo, false))
			{
				cHeroInfo.mActorValue.actorLvl = level;
				cHeroInfo.mActorValue.actorExp = exp;
			}
		}

		public void OnHeroInfoUpdate(SCPKG_NTF_HERO_INFO_UPD svrHeroInfoUp)
		{
			CHeroInfo cHeroInfo;
			if (this.GetHeroInfo(svrHeroInfoUp.dwHeroID, out cHeroInfo, true))
			{
				cHeroInfo.OnHeroInfoUpdate(svrHeroInfoUp);
			}
		}

		public void SetSkillPoint(uint pt, bool bRefreshTime = false, bool bDispathEvent = false)
		{
			this.m_skillPoint = pt;
			if (bRefreshTime)
			{
				if ((long)(this.m_skillPtRefreshSec * 1000) > (long)((ulong)-1))
				{
					this.m_updateTimeMSec = 4294967295u;
				}
				else
				{
					this.m_updateTimeMSec = (uint)(this.m_skillPtRefreshSec * 1000);
				}
			}
			if (bDispathEvent)
			{
				Singleton<EventRouter>.instance.BroadCastEvent<uint>("SkillPointChange", this.m_skillPoint);
			}
		}

		public void UpdateLogic(int delta)
		{
			this.m_updateTimeMSec = this.UInt32ChgAdjust(this.m_updateTimeMSec, -delta);
		}

		private void SetLevel(uint newLevel, CS_ACNT_UPDATE_FROMTYPE lvlUpType)
		{
			uint level = this.m_level;
			this.m_level = newLevel;
			ResAcntExpInfo dataByKey = GameDataMgr.acntExpDatabin.GetDataByKey(this.m_level);
			this.m_needExp = dataByKey.dwNeedExp;
			if (level > 0u && this.m_level > level && Singleton<GameStateCtrl>.GetInstance().GetCurrentState() is LobbyState)
			{
				if (lvlUpType == CS_ACNT_UPDATE_FROMTYPE.CS_ACNT_UPDATE_FROMTYPE_SWEEP)
				{
					CAdventureView.SetMopupLevelUp(level, this.m_level);
				}
				else
				{
					CUIEvent cUIEvent = new CUIEvent();
					cUIEvent.m_eventID = enUIEventID.Settle_OpenLvlUp;
					cUIEvent.m_eventParams.tag = (int)level;
					cUIEvent.m_eventParams.tag2 = (int)this.m_level;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				}
			}
		}

		public void SetPvpLevel(uint value)
		{
			this.m_pvpLevel = value;
			Singleton<EventRouter>.instance.BroadCastEvent("Chat_PlayerLevel_Set");
		}

		public bool CheckCoinEnough(RES_SHOPBUY_COINTYPE coinType, uint targetValue)
		{
			switch (coinType)
			{
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
				return this.DianQuan >= (ulong)targetValue;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
				return this.GoldCoin >= targetValue;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
				return this.BurningCoin >= targetValue;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
				return this.ArenaCoin >= targetValue;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SKINCOIN:
				return this.SkinCoin >= targetValue;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SYMBOLCOIN:
				return this.SymbolCoin >= targetValue;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND:
				return this.Diamond >= targetValue;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_MIXPAY:
				return (ulong)this.Diamond + this.DianQuan >= (ulong)targetValue;
			}
			return false;
		}

		public static int GetCurrentUTCTime()
		{
			int num = (int)(Time.realtimeSinceStartup - CRoleInfo.s_upToLoginSec);
			return CRoleInfo.s_sysTime + num;
		}

		public static int GetElapseSecondsSinceLogin()
		{
			return (int)(Time.realtimeSinceStartup - CRoleInfo.s_upToLoginSec);
		}

		public void SetMentorInfo(COMDT_ACNT_MASTER_INFO mentorInfo)
		{
			this.m_mentorInfo = Utility.DeepCopyByReflection<COMDT_ACNT_MASTER_INFO>(mentorInfo);
		}

		public void SetAttributes(SCPKG_CMD_GAMELOGINRSP rsp)
		{
			this.playerUllUID = rsp.ullGameAcntUid;
			this.m_firstLoginTime = rsp.dwFirstLoginTime;
			this.m_firstLoginZeroDay = Utility.GetZeroBaseSecond((long)((ulong)this.m_firstLoginTime));
			this.SetLevel(rsp.dwLevel, CS_ACNT_UPDATE_FROMTYPE.CS_ACNT_UPDATE_FROMTYPE_NULL);
			this.SetPvpLevel(rsp.dwPvpLevel);
			this.m_exp = rsp.dwExp;
			this.m_pvpExp = rsp.dwPvpExp;
			this.m_dianquan = 0uL;
			this.m_goldCoin = rsp.stCoinList.CoinCnt[1];
			this.m_burningCoin = rsp.stCoinList.CoinCnt[2];
			this.m_arenaCoin = rsp.stCoinList.CoinCnt[3];
			this.SkinCoin = rsp.stCoinList.CoinCnt[4];
			this.SymbolCoin = rsp.stCoinList.CoinCnt[5];
			this.m_diamond = rsp.stCoinList.CoinCnt[6];
			this.m_JiFen = rsp.stCoinList.CoinCnt[7];
			this.m_maxActionPoint = rsp.dwMaxActionPoint;
			this.m_curActionPoint = rsp.dwCurActionPoint;
			this.m_titleId = (int)rsp.dwTitleId;
			this.m_headId = (int)rsp.dwHeadId;
			this.m_Name = StringHelper.UTF8BytesToString(ref rsp.szName);
			this.m_HeadUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref rsp.szHeadUrl);
			this.m_personSign = StringHelper.UTF8BytesToString(ref rsp.szSignatureInfo);
			this.m_privilegeType = (COM_PRIVILEGE_TYPE)rsp.bPrivilege;
			this.m_gameDifficult = rsp.bAcntNewbieType;
			this.m_gender = (COM_SNSGENDER)rsp.bGender;
			this.getFriendCoinCnt = (int)rsp.bGetCoinNums;
			this.m_AcntOldType = (COM_ACNT_OLD_TYPE)rsp.bAcntOldType;
			this.m_mentorInfo = Utility.DeepCopyByReflection<COMDT_ACNT_MASTER_INFO>(rsp.stAcntMasterInfo);
			this.m_userPrivacyBits = rsp.ullUserPrivacyBits;
			this.m_socialFriendCard = rsp.stFriendCard;
			this.GeiLiDuiYou = (int)rsp.stLikeNum.dwTeammateNum;
			this.KeJingDuiShou = (int)rsp.stLikeNum.dwOpponentNum;
			this.snsSwitchBits = rsp.dwRefuseFriendBits;
			Debug.Log("--- LBS， get server snsSwitchBits:" + this.snsSwitchBits);
			MonoSingleton<GPSSys>.instance.Clear();
			UT.CheckGPS();
			this.creditScore = rsp.dwCreditValue;
			this.sumDelCreditValue = rsp.iSumDelCreditValue * -1;
			this.mostDelCreditType = rsp.dwMostDelCreditType;
			this.ExpPool = rsp.dwHeroPoolExp;
			Singleton<CChatController>.GetInstance().model.sysData.lastTimeStamp = rsp.dwServerCurTimeSec;
			this.setShopBuyRcd(ref rsp.stShopBuyRcd);
			this.SetGlobalRefreshTimer(rsp.dwServerCurTimeSec, false);
			this.SetPvELevelInfo(ref rsp.stPveProgress, rsp.dwServerCurTimeSec);
			this.m_skillPoint = rsp.dwSkillPoint;
			if ((ulong)(rsp.dwSPUpdTimeSec * 1000u) > (ulong)-1)
			{
				this.m_updateTimeMSec = 4294967295u;
			}
			else
			{
				this.m_updateTimeMSec = rsp.dwSPUpdTimeSec * 1000u;
			}
			this.m_maxSkillPt = GameDataMgr.globalInfoDatabin.GetDataByKey(26u).dwConfValue;
			this.m_skillPtRefreshSec = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(24u).dwConfValue;
			this.SetGuildData(ref rsp);
			this.SetExtraCoinAndExp(rsp.stPropMultiple);
			this.stMostUsedHero = rsp.stMostUsedHero;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				masterRoleInfo.SetVipFlags(rsp.dwQQVipInfo);
			}
			DateTime dateTime = Utility.ToUtcTime2Local((long)((ulong)rsp.dwLastLoginTime));
			if (Utility.ToUtcTime2Local((long)((ulong)rsp.dwServerCurTimeSec)).get_Date() == dateTime.get_Date())
			{
				this.bFirstLoginToday = false;
			}
			else
			{
				this.bFirstLoginToday = true;
			}
			this.SetFirstWinRemainingTime(rsp.dwNextFirstWinSec);
			this.SetFirstWinLevelLimit(rsp.dwFirstWinPvpLvl);
			uint num = Utility.GetNewDayDeltaSec(CRoleInfo.GetCurrentUTCTime()) * 1000u;
			CTimer timer = Singleton<CTimerManager>.instance.GetTimer(this.m_newDayTimer);
			if (timer == null)
			{
				this.m_newDayTimer = Singleton<CTimerManager>.instance.AddTimer((int)num, 1, new CTimer.OnTimeUpHandler(this.OnNewDayNtf));
			}
			else
			{
				timer.ResetTotalTime((int)num);
				timer.Reset();
				timer.Resume();
			}
			this.m_licenseInfo.SetSvrLicenseData(rsp.stLicense);
			Singleton<CTaskSys>.instance.SetCardExpireTime(RES_PROP_VALFUNC_TYPE.RES_PROP_VALFUNC_MONTH_CARD, rsp.stMonthWeekCardInfo.dwMonthExpireTimeStamp);
			Singleton<CTaskSys>.instance.SetCardExpireTime(RES_PROP_VALFUNC_TYPE.RES_PROP_VALFUNC_WEEK_CARD, rsp.stMonthWeekCardInfo.dwWeekExpireTimeStamp);
			Singleton<CTaskSys>.instance.InitReport();
			if (rsp.stPasswdInfo.bPswdState == 0)
			{
				Singleton<CSecurePwdSystem>.GetInstance().EnableStatus = PwdStatus.Disable;
			}
			else
			{
				Singleton<CSecurePwdSystem>.GetInstance().EnableStatus = PwdStatus.Enable;
			}
			if (rsp.stPasswdInfo.bPswdCloseState == 0)
			{
				Singleton<CSecurePwdSystem>.GetInstance().CloseStatus = PwdCloseStatus.Close;
				Singleton<CSecurePwdSystem>.GetInstance().CloseTime = 0u;
			}
			else
			{
				DateTime dateTime2 = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
				if ((Utility.ToUtcTime2Local((long)((ulong)rsp.stPasswdInfo.dwPswdCloseTime)) - dateTime2).get_TotalSeconds() > 0.0)
				{
					Singleton<CSecurePwdSystem>.GetInstance().CloseStatus = PwdCloseStatus.Open;
					Singleton<CSecurePwdSystem>.GetInstance().CloseTime = rsp.stPasswdInfo.dwPswdCloseTime;
				}
				else
				{
					Singleton<CSecurePwdSystem>.GetInstance().EnableStatus = PwdStatus.Disable;
					Singleton<CSecurePwdSystem>.GetInstance().CloseStatus = PwdCloseStatus.Close;
					Singleton<CSecurePwdSystem>.GetInstance().CloseTime = 0u;
				}
			}
			this.chgNameCD = rsp.stChgNameCD;
			this.recentUseHero = rsp.stRecentUsedHero;
			this.acntMobaInfo = rsp.stMobaInfo;
			this.IsNoAskFor = (rsp.bNoAskforFlag > 0);
			this.SendAskforReqTime = rsp.dwSendAskforReqTime;
			this.SendAskforReqCnt = rsp.dwSendAskforReqCnt;
			this.m_otherStateBits = rsp.dwOtherStateBits;
		}

		private void OnNewDayNtf(int timerSequence)
		{
			uint totalTime = Utility.GetNewDayDeltaSec(CRoleInfo.GetCurrentUTCTime()) * 1000u;
			CTimer timer = Singleton<CTimerManager>.instance.GetTimer(timerSequence);
			timer.ResetTotalTime((int)totalTime);
			timer.Reset();
			timer.Resume();
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.NEWDAY_NTF);
		}

		public void SetGlobalRefreshTimer(uint serverTime, bool force = false)
		{
			if (!force && CRoleInfo.m_globalRefreshTimerSeq != -1)
			{
				return;
			}
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref CRoleInfo.m_globalRefreshTimerSeq);
			DateTime dateTime = Utility.ToUtcTime2Local((long)((ulong)serverTime));
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(10u).dwConfValue;
			int num = Utility.Hours2Second((int)(dwConfValue / 100u)) + Utility.Minutes2Seconds((int)(dwConfValue % 100u));
			int num2 = (num - Convert.ToInt32(dateTime.get_TimeOfDay().get_TotalSeconds()) > 0) ? (num - Convert.ToInt32(dateTime.get_TimeOfDay().get_TotalSeconds())) : (num - Convert.ToInt32(dateTime.get_TimeOfDay().get_TotalSeconds()) + 86400);
			CRoleInfo.m_globalRefreshTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer(num2 * 1000, 1, new CTimer.OnTimeUpHandler(this.OnGlobalRefreshTimerEnd));
		}

		private void OnGlobalRefreshTimerEnd(int seq)
		{
			CRoleInfo.m_globalRefreshTimerSeq = -1;
			this.SendAskforReqCnt = 0u;
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.GLOBAL_REFRESH_TIME);
			this.SetGlobalRefreshTimer((uint)CRoleInfo.GetCurrentUTCTime(), false);
		}

		public static void SetServerTime(int serverTime)
		{
			CRoleInfo.s_sysTime = serverTime;
			CRoleInfo.s_upToLoginSec = Time.realtimeSinceStartup;
		}

		private void setShopBuyRcd(ref CSDT_ACNT_SHOPBUY_INFO rcd)
		{
			CRoleInfo.SetServerTime(rcd.iGameSysTime);
			this.m_freeDrawInfo = new stShopBuyDrawInfo[rcd.astShopDrawInfo.Length];
			for (int i = 0; i < this.m_freeDrawInfo.Length; i++)
			{
				this.m_freeDrawInfo[i].dwLeftFreeDrawCD = CRoleInfo.s_sysTime + rcd.astShopDrawInfo[i].iLeftFreeSec;
				this.m_freeDrawInfo[i].dwLeftFreeDrawCnt = rcd.astShopDrawInfo[i].iLeftFreeCnt;
			}
			this.m_coinDrawIndex = (int)rcd.bCurCoinDrawStep;
			this.m_DiamondOpenBoxCnt = rcd.dwOpenBoxByCouponsCnt;
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(86u).dwConfValue;
			if (dwConfValue - rcd.dwDirectBuyItemCnt >= 0u)
			{
				this.m_MaterialDirectBuyLimit = (byte)((uint)((byte)dwConfValue) - rcd.dwDirectBuyItemCnt);
			}
			else
			{
				this.m_MaterialDirectBuyLimit = 0;
			}
			if (rcd.stSymbolDrawCommon != null)
			{
				if (rcd.stSymbolDrawCommon.bSubDrawCnt > 0)
				{
					for (byte b = 0; b < rcd.stSymbolDrawCommon.bSubDrawCnt; b += 1)
					{
						COMDT_DRAWCNT_SUBINFO cOMDT_DRAWCNT_SUBINFO = rcd.stSymbolDrawCommon.astSubDrawInfo[(int)b];
						if (cOMDT_DRAWCNT_SUBINFO != null)
						{
							switch (cOMDT_DRAWCNT_SUBINFO.iSubType)
							{
							case 2:
								CMallSymbolGiftController.SymbolGiftCommonDrawedCntInfo[cOMDT_DRAWCNT_SUBINFO.iSubType] = cOMDT_DRAWCNT_SUBINFO.iDrawCnt;
								break;
							case 3:
								CMallSymbolGiftController.SymbolGiftCommonDrawedCntInfo[cOMDT_DRAWCNT_SUBINFO.iSubType] = cOMDT_DRAWCNT_SUBINFO.iDrawCnt;
								break;
							case 4:
								CMallSymbolGiftController.SymbolGiftCommonDrawedCntInfo[cOMDT_DRAWCNT_SUBINFO.iSubType] = cOMDT_DRAWCNT_SUBINFO.iDrawCnt;
								break;
							}
						}
					}
				}
				int iFreeDrawTotalCnt = rcd.stSymbolDrawCommon.iFreeDrawTotalCnt;
				if (iFreeDrawTotalCnt >= 0)
				{
					CMallSymbolGiftController.SymbolGiftCommonDrawedCntInfo[1] = iFreeDrawTotalCnt;
				}
			}
			if (rcd.stSymbolDrawSenior != null)
			{
				if (rcd.stSymbolDrawSenior.bSubDrawCnt > 0)
				{
					for (byte b2 = 0; b2 < rcd.stSymbolDrawSenior.bSubDrawCnt; b2 += 1)
					{
						COMDT_DRAWCNT_SUBINFO cOMDT_DRAWCNT_SUBINFO2 = rcd.stSymbolDrawSenior.astSubDrawInfo[(int)b2];
						if (cOMDT_DRAWCNT_SUBINFO2 != null)
						{
							switch (cOMDT_DRAWCNT_SUBINFO2.iSubType)
							{
							case 2:
								CMallSymbolGiftController.SymbolGiftSeniorDrawedCntInfo[cOMDT_DRAWCNT_SUBINFO2.iSubType] = cOMDT_DRAWCNT_SUBINFO2.iDrawCnt;
								break;
							case 3:
								CMallSymbolGiftController.SymbolGiftSeniorDrawedCntInfo[cOMDT_DRAWCNT_SUBINFO2.iSubType] = cOMDT_DRAWCNT_SUBINFO2.iDrawCnt;
								break;
							case 4:
								CMallSymbolGiftController.SymbolGiftSeniorDrawedCntInfo[cOMDT_DRAWCNT_SUBINFO2.iSubType] = cOMDT_DRAWCNT_SUBINFO2.iDrawCnt;
								break;
							}
						}
					}
				}
				int iFreeDrawTotalCnt2 = rcd.stSymbolDrawSenior.iFreeDrawTotalCnt;
				if (iFreeDrawTotalCnt2 >= 0)
				{
					CMallSymbolGiftController.SymbolGiftSeniorDrawedCntInfo[1] = iFreeDrawTotalCnt2;
				}
			}
		}

		public stShopBuyDrawInfo GetFreeDrawInfo(COM_SHOP_DRAW_TYPE drawType)
		{
			if (drawType < (COM_SHOP_DRAW_TYPE)this.m_freeDrawInfo.Length)
			{
				return this.m_freeDrawInfo[(int)drawType];
			}
			return default(stShopBuyDrawInfo);
		}

		public void OnUpdate(SCPKG_NTF_ACNT_INFO_UPD ntfAcntInfoUpd)
		{
			for (int i = 0; i < ntfAcntInfoUpd.iAcntUpdNum; i++)
			{
				this.OnUpdate(ref ntfAcntInfoUpd.astAcntUpdInfo[i]);
			}
			Singleton<EventRouter>.instance.BroadCastEvent("MasterAttributesChanged");
		}

		public void OnUpdate(ref SCDT_NTF_ACNT_INFO_UPD ntfAcntInfoUpd)
		{
			switch (ntfAcntInfoUpd.bUpdType)
			{
			case 1:
				this.SetLevel(this.UInt32ChgAdjust(this.Level, ntfAcntInfoUpd.iUpdValChg), CS_ACNT_UPDATE_FROMTYPE.CS_ACNT_UPDATE_FROMTYPE_NULL);
				break;
			case 2:
				this.m_exp = this.UInt32ChgAdjust(this.m_exp, ntfAcntInfoUpd.iUpdValChg);
				break;
			case 3:
				this.m_maxActionPoint = this.UInt32ChgAdjust(this.m_maxActionPoint, ntfAcntInfoUpd.iUpdValChg);
				break;
			case 4:
				this.m_curActionPoint = this.UInt32ChgAdjust(this.m_curActionPoint, ntfAcntInfoUpd.iUpdValChg);
				break;
			case 6:
				this.m_diamond = this.UInt32ChgAdjust(this.m_diamond, ntfAcntInfoUpd.iUpdValChg);
				break;
			case 8:
				this.m_skillPoint = this.UInt32ChgAdjust(this.m_skillPoint, ntfAcntInfoUpd.iUpdValChg);
				this.SetSkillPoint(this.m_skillPoint, true, true);
				break;
			case 10:
				this.SetPvpLevel(this.UInt32ChgAdjust(this.m_pvpLevel, ntfAcntInfoUpd.iUpdValChg));
				break;
			case 11:
				this.m_pvpExp = this.UInt32ChgAdjust(this.m_pvpExp, ntfAcntInfoUpd.iUpdValChg);
				break;
			case 12:
				this.m_goldCoin = this.UInt32ChgAdjust(this.m_goldCoin, ntfAcntInfoUpd.iUpdValChg);
				break;
			case 13:
				this.m_burningCoin = this.UInt32ChgAdjust(this.m_burningCoin, ntfAcntInfoUpd.iUpdValChg);
				break;
			case 14:
				this.m_arenaCoin = this.UInt32ChgAdjust(this.m_arenaCoin, ntfAcntInfoUpd.iUpdValChg);
				break;
			case 15:
				this.m_expPool = this.UInt32ChgAdjust(this.m_expPool, ntfAcntInfoUpd.iUpdValChg);
				if (ntfAcntInfoUpd.bFromType == 1)
				{
					Singleton<CUIManager>.GetInstance().OpenTips(string.Format("经验池增加{0}", ntfAcntInfoUpd.iUpdValChg), false, 1.5f, null, new object[0]);
				}
				break;
			case 16:
				this.m_skinCoin = this.UInt32ChgAdjust(this.m_skinCoin, ntfAcntInfoUpd.iUpdValChg);
				break;
			case 17:
				this.m_symbolCoin = this.UInt32ChgAdjust(this.m_symbolCoin, ntfAcntInfoUpd.iUpdValChg);
				Singleton<EventRouter>.instance.BroadCastEvent("MasterSymbolCoinChanged");
				break;
			case 18:
			{
				HuoyueData huoyue_data = Singleton<CTaskSys>.instance.model.huoyue_data;
				huoyue_data.day_curNum = (huoyue_data.week_curNum = this.UInt32ChgAdjust(huoyue_data.day_curNum, ntfAcntInfoUpd.iUpdValChg));
				Singleton<EventRouter>.instance.BroadCastEvent("TASK_HUOYUEDU_Change");
				break;
			}
			case 19:
				this.GeiLiDuiYou = (int)this.UInt32ChgAdjust((uint)this.GeiLiDuiYou, ntfAcntInfoUpd.iUpdValChg);
				break;
			case 20:
				this.KeJingDuiShou = (int)this.UInt32ChgAdjust((uint)this.KeJingDuiShou, ntfAcntInfoUpd.iUpdValChg);
				break;
			case 21:
				this.m_JiFen = this.UInt32ChgAdjust(this.m_JiFen, ntfAcntInfoUpd.iUpdValChg);
				Singleton<EventRouter>.instance.BroadCastEvent("MasterJifenChanged");
				break;
			}
		}

		public void OnLevelUp(SCPKG_NTF_ACNT_LEVELUP ntfAcntLevelUp)
		{
			this.m_maxActionPoint = ntfAcntLevelUp.dwNewMaxAP;
			this.m_curActionPoint = ntfAcntLevelUp.dwNewCurAP;
			this.SetLevel(ntfAcntLevelUp.dwNewLevel, (CS_ACNT_UPDATE_FROMTYPE)ntfAcntLevelUp.bFromType);
			this.m_exp = ntfAcntLevelUp.dwNewExp;
			Singleton<EventRouter>.instance.BroadCastEvent("MasterAttributesChanged");
		}

		public void OnPvpLevelUp(SCPKG_NTF_ACNT_PVPLEVELUP ntfAcntPvpLevelUp)
		{
			int pvpLevel = (int)this.m_pvpLevel;
			this.SetPvpLevel(ntfAcntPvpLevelUp.dwNewLevel);
			this.m_pvpExp = ntfAcntPvpLevelUp.dwNewExp;
			this.m_symbolInfo.SetSymbolPageMaxLevel();
			this.m_symbolInfo.SetSymbolPageCount((int)ntfAcntPvpLevelUp.bSymbolPageCnt);
			Singleton<EventRouter>.instance.BroadCastEvent("MasterPvpLevelChanged");
			Singleton<EventRouter>.instance.BroadCastEvent("MasterAttributesChanged");
			if (pvpLevel > 0 && (ulong)this.m_pvpLevel > (ulong)((long)pvpLevel) && Singleton<GameStateCtrl>.GetInstance().GetCurrentState() is LobbyState && ntfAcntPvpLevelUp.bFromType != 0)
			{
				CUIEvent cUIEvent = new CUIEvent();
				cUIEvent.m_eventID = enUIEventID.Settle_OpenLvlUp;
				cUIEvent.m_eventParams.tag = pvpLevel;
				cUIEvent.m_eventParams.tag2 = (int)this.m_pvpLevel;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
			}
		}

		public static void GetPlayerPreLevleAndExp(uint inDltExp, out int preLevel, out uint preExp)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			preLevel = (int)masterRoleInfo.Level;
			preExp = masterRoleInfo.Exp;
			uint num = inDltExp;
			while (num > preExp)
			{
				if (preLevel == 1)
				{
					break;
				}
				num -= preExp;
				preLevel--;
				preExp = GameDataMgr.acntExpDatabin.GetDataByKey((uint)preLevel).dwNeedExp;
			}
			preExp -= num;
		}

		public static void GetHeroPreLevleAndExp(uint HeroId, uint inDltExp, out int preLevel, out uint preExp)
		{
			preExp = 0u;
			preLevel = 0;
			CHeroInfo cHeroInfo;
			if (!Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().heroDic.TryGetValue(HeroId, out cHeroInfo))
			{
				return;
			}
			preLevel = cHeroInfo.mActorValue.actorLvl;
			preExp = (uint)cHeroInfo.mActorValue.actorExp;
			uint num = inDltExp;
			while (num > preExp)
			{
				if (preLevel == 1)
				{
					break;
				}
				num -= preExp;
				preLevel--;
				preExp = GameDataMgr.heroLvlUpDatabin.GetDataByKey((uint)preLevel).dwExp;
			}
			preExp -= num;
		}

		public void SetExtraCoinAndExp(COMDT_PROP_MULTIPLE data)
		{
			if (data == null)
			{
				return;
			}
			this._extraTimeCoin = 0u;
			this._extraWinCoin = 0u;
			this._extraTimeExp = 0u;
			this._extraWinExp = 0u;
			this._extraWinCoinValue = 0u;
			this._extraWinExpValue = 0u;
			this._extraCoinHours = 0u;
			this._extraExpHours = 0u;
			for (int i = 0; i < (int)data.wCnt; i++)
			{
				COMDT_PROP_MULTIPLE_INFO cOMDT_PROP_MULTIPLE_INFO = data.astMultipleInfo[i];
				byte bMultipleType = cOMDT_PROP_MULTIPLE_INFO.bMultipleType;
				if (bMultipleType != 2)
				{
					if (bMultipleType == 3)
					{
						if (cOMDT_PROP_MULTIPLE_INFO.bTimeType == 1)
						{
							this._extraTimeCoin = cOMDT_PROP_MULTIPLE_INFO.dwRatio;
							this._extraCoinHours = cOMDT_PROP_MULTIPLE_INFO.dwTimeValue;
						}
						else if (cOMDT_PROP_MULTIPLE_INFO.bTimeType == 2)
						{
							this._extraWinCoin = cOMDT_PROP_MULTIPLE_INFO.dwRatio;
							this._extraWinCoinValue = cOMDT_PROP_MULTIPLE_INFO.dwTimeValue;
						}
					}
				}
				else if (cOMDT_PROP_MULTIPLE_INFO.bTimeType == 1)
				{
					this._extraTimeExp = cOMDT_PROP_MULTIPLE_INFO.dwRatio;
					this._extraExpHours = cOMDT_PROP_MULTIPLE_INFO.dwTimeValue;
				}
				else if (cOMDT_PROP_MULTIPLE_INFO.bTimeType == 2)
				{
					this._extraWinExp = cOMDT_PROP_MULTIPLE_INFO.dwRatio;
					this._extraWinExpValue = cOMDT_PROP_MULTIPLE_INFO.dwTimeValue;
				}
			}
			this.UpdateCoinAndExpValidTime();
		}

		public bool HaveExtraCoin()
		{
			return this._extraTimeCoin > 0u || this._extraWinCoin > 0u;
		}

		public bool HaveExtraExp()
		{
			return this._extraTimeExp > 0u || this._extraWinExp > 0u;
		}

		public void UpdateCoinAndExpValidTime()
		{
			if (this._extraTimeCoin > 0u && this.GetExpireHours(this._extraCoinHours) == 0)
			{
				this._extraTimeCoin = 0u;
			}
			if (this._extraTimeExp > 0u && this.GetExpireHours(this._extraExpHours) == 0)
			{
				this._extraTimeExp = 0u;
			}
		}

		public int GetCoinExpireHours()
		{
			return this.GetExpireHours(this._extraCoinHours);
		}

		public int GetExpExpireHours()
		{
			return this.GetExpireHours(this._extraExpHours);
		}

		public uint GetCoinWinCount()
		{
			return this._extraWinCoinValue;
		}

		public uint GetExpWinCount()
		{
			return this._extraWinExpValue;
		}

		private int GetExpireHours(uint value)
		{
			return UT.CalcDeltaHorus((uint)CRoleInfo.GetCurrentUTCTime(), value);
		}

		public void SetGuildData(ref SCPKG_CMD_GAMELOGINRSP rsp)
		{
			this.m_baseGuildInfo.guildState = (COM_PLAYER_GUILD_STATE)rsp.stGuildBaseInfo.bGuildState;
			this.m_baseGuildInfo.uulUid = rsp.stGuildBaseInfo.ullGuildID;
			this.m_baseGuildInfo.logicWorldId = rsp.stGuildBaseInfo.iGuildLogicWorldID;
			this.m_extGuildInfo.bApplyJoinGuildNum = rsp.stGuildExtInfo.bApplyJoinGuildNum;
			this.m_extGuildInfo.dwClearApplyJoinGuildNumTime = rsp.stGuildExtInfo.dwClearApplyJoinGuildNumTime;
			this.m_extGuildInfo.dwLastCreateGuildTime = rsp.stGuildExtInfo.dwLastCreateGuildTime;
			this.m_extGuildInfo.dwLastQuitGuildTime = rsp.stGuildExtInfo.dwLastQuitGuildTime;
			this.m_extGuildInfo.bSendGuildMailCnt = rsp.stGuildExtInfo.bSendGuildMailCnt;
		}

		public void SetFreeHeroInfo(COMDT_FREEHERO stFreeHero)
		{
			uint num = this.freeHeroExpireTime;
			this.freeHeroExpireTime = stFreeHero.dwDeadline;
			this.freeHeroList.Clear();
			int i = 0;
			while (i < (int)stFreeHero.stFreeHeroList.wFreeCnt)
			{
				if (!CSysDynamicBlock.bLobbyEntryBlocked)
				{
					goto IL_71;
				}
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(stFreeHero.stFreeHeroList.astFreeHeroDetail[i].dwFreeHeroID);
				if (dataByKey == null || dataByKey.bIOSHide != 1)
				{
					goto IL_71;
				}
				IL_68:
				i++;
				continue;
				IL_71:
				this.freeHeroList.Add(stFreeHero.stFreeHeroList.astFreeHeroDetail[i]);
				goto IL_68;
			}
			if (this.freeHeroExpireTime != 0u && num != this.freeHeroExpireTime)
			{
				if (this.GetFreeHeroTimer != 0)
				{
					Singleton<CTimerManager>.GetInstance().RemoveTimer(this.GetFreeHeroTimer);
				}
				int num2 = (int)((ulong)this.freeHeroExpireTime - (ulong)((long)CRoleInfo.GetCurrentUTCTime()) + 15uL);
				this.GetFreeHeroTimer = Singleton<CTimerManager>.GetInstance().AddTimer(num2 * 1000, 1, new CTimer.OnTimeUpHandler(Singleton<CPvPHeroShop>.GetInstance().GetFreeHero));
			}
		}

		public void SetFreeHeroSymbol(COMDT_FREEHERO_INACNT stFreeHeroSymbol)
		{
			this.freeHeroSymbolList.Clear();
			for (int i = 0; i < (int)stFreeHeroSymbol.bHeroCnt; i++)
			{
				this.freeHeroSymbolList.Add(stFreeHeroSymbol.astHeroInfo[i]);
			}
		}

		public COMDT_FREEHERO_INFO GetFreeHeroSymbol(uint heroId)
		{
			for (int i = 0; i < this.freeHeroSymbolList.Count; i++)
			{
				if (this.freeHeroSymbolList[i].dwHeroID == heroId)
				{
					return this.freeHeroSymbolList[i];
				}
			}
			return null;
		}

		public byte GetFreeHeroSymbolId(uint heroId)
		{
			for (int i = 0; i < this.freeHeroSymbolList.Count; i++)
			{
				if (this.freeHeroSymbolList[i].dwHeroID == heroId)
				{
					return this.freeHeroSymbolList[i].bSymbolPageWear;
				}
			}
			return 0;
		}

		public uint GetFreeHeroWearSkinId(uint heroId)
		{
			for (int i = 0; i < this.freeHeroSymbolList.Count; i++)
			{
				if (this.freeHeroSymbolList[i].dwHeroID == heroId)
				{
					return (uint)this.freeHeroSymbolList[i].wSkinID;
				}
			}
			return 0u;
		}

		public void SetFreeHeroWearSkinId(uint heroId, uint skinId)
		{
			if (this.IsFreeHero(heroId))
			{
				for (int i = 0; i < this.freeHeroSymbolList.Count; i++)
				{
					if (this.freeHeroSymbolList[i].dwHeroID == heroId)
					{
						this.freeHeroSymbolList[i].wSkinID = (ushort)skinId;
						return;
					}
				}
				COMDT_FREEHERO_INFO cOMDT_FREEHERO_INFO = new COMDT_FREEHERO_INFO();
				cOMDT_FREEHERO_INFO.dwHeroID = heroId;
				cOMDT_FREEHERO_INFO.wSkinID = (ushort)skinId;
				this.freeHeroSymbolList.Add(cOMDT_FREEHERO_INFO);
			}
		}

		public void SetFreeHeroSymbolId(uint heroId, byte symbolId)
		{
			for (int i = 0; i < this.freeHeroSymbolList.Count; i++)
			{
				if (this.freeHeroSymbolList[i].dwHeroID == heroId)
				{
					this.freeHeroSymbolList[i].bSymbolPageWear = symbolId;
					return;
				}
			}
			COMDT_FREEHERO_INFO cOMDT_FREEHERO_INFO = new COMDT_FREEHERO_INFO();
			cOMDT_FREEHERO_INFO.dwHeroID = heroId;
			cOMDT_FREEHERO_INFO.bSymbolPageWear = symbolId;
			this.freeHeroSymbolList.Add(cOMDT_FREEHERO_INFO);
		}

		public void SetHeroSymbolPageIdx(uint heroId, int pageIdx)
		{
			CHeroInfo cHeroInfo;
			if (this.GetHeroInfo(heroId, out cHeroInfo, true))
			{
				cHeroInfo.OnSymbolPageChange(pageIdx);
			}
			else
			{
				this.SetFreeHeroSymbolId(heroId, (byte)pageIdx);
			}
		}

		public bool IsFreeHero(uint heroId)
		{
			for (int i = 0; i < this.freeHeroList.get_Count(); i++)
			{
				if (this.freeHeroList.get_Item(i).dwFreeHeroID == heroId)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsCreditFreeHero(uint heroId)
		{
			for (int i = 0; i < this.freeHeroList.get_Count(); i++)
			{
				if (this.freeHeroList.get_Item(i).dwFreeHeroID == heroId)
				{
					return this.freeHeroList.get_Item(i).dwCreditLevel > 0u;
				}
			}
			return false;
		}

		public bool IsFreeHeroAndSelfDontHave(uint heroId)
		{
			bool result = true;
			if (this.heroDic.ContainsKey(heroId))
			{
				result = false;
			}
			else if (!this.IsFreeHero(heroId))
			{
				result = false;
			}
			return result;
		}

		public HeroSkinState GetHeroSkinState(uint heroId, uint skinId)
		{
			if (this.IsHaveHero(heroId, false))
			{
				if (this.IsHaveHeroSkin(heroId, skinId, false))
				{
					if (this.GetHeroWearSkinId(heroId) == skinId)
					{
						return HeroSkinState.NormalHero_NormalSkin_Wear;
					}
					return HeroSkinState.NormalHero_NormalSkin_Own;
				}
				else
				{
					if (!this.IsValidExperienceSkin(heroId, skinId))
					{
						return HeroSkinState.NormalHero_Skin_NotOwn;
					}
					if (this.GetHeroWearSkinId(heroId) == skinId)
					{
						return HeroSkinState.NormalHero_LimitSkin_Wear;
					}
					return HeroSkinState.NormalHero_LimitSkin_Own;
				}
			}
			else if (this.IsValidExperienceHero(heroId) || this.IsFreeHero(heroId))
			{
				if (this.IsHaveHeroSkin(heroId, skinId, false) || skinId == 0u)
				{
					if (this.GetHeroWearSkinId(heroId) == skinId)
					{
						return HeroSkinState.LimitHero_NormalSkin_Wear;
					}
					return HeroSkinState.LimitHero_NormalSkin_Own;
				}
				else
				{
					if (!this.IsValidExperienceSkin(heroId, skinId))
					{
						return HeroSkinState.LimitHero_Skin_NotOwn;
					}
					if (this.GetHeroWearSkinId(heroId) == skinId)
					{
						return HeroSkinState.LimitHero_LimitSkin_Wear;
					}
					return HeroSkinState.LimitHero_LimitSkin_Own;
				}
			}
			else
			{
				if (skinId == 0u)
				{
					return HeroSkinState.NoHero_Skin_Wear;
				}
				if (this.IsHaveHeroSkin(heroId, skinId, false))
				{
					return HeroSkinState.NoHero_NormalSkin_Own;
				}
				if (this.IsValidExperienceSkin(heroId, skinId))
				{
					return HeroSkinState.NoHero_LimitSkin_Own;
				}
				return HeroSkinState.NoHero_Skin_NotOwn;
			}
		}

		public bool IsCanBuySkinButNotHaveHero(uint heroId, uint skinId)
		{
			return CSkinInfo.IsCanBuy(heroId, skinId) && !this.IsHaveHero(heroId, false);
		}

		public uint GetHeroWearSkinId(uint heroId)
		{
			if (this.heroDic.ContainsKey(heroId))
			{
				return this.heroDic[heroId].m_skinInfo.GetWearSkinId();
			}
			if (this.IsFreeHero(heroId))
			{
				return this.GetFreeHeroWearSkinId(heroId);
			}
			return 0u;
		}

		public void OnAddHeroSkin(uint heroId, uint skinId)
		{
			ulong num = 1uL << (int)skinId;
			if (this.heroSkinDic.ContainsKey(heroId))
			{
				Dictionary<uint, ulong> dictionary;
				ulong num2 = (dictionary = this.heroSkinDic).get_Item(heroId);
				dictionary.set_Item(heroId, num2 | num);
			}
			else
			{
				this.heroSkinDic.Add(heroId, (ulong)skinId);
				this.heroSkinDic.set_Item(heroId, num);
			}
		}

		public void OnGmAddAllSkin()
		{
			DictionaryView<uint, CHeroInfo>.Enumerator enumerator = this.heroDic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
				uint key = current.get_Key();
				if (this.heroSkinDic.ContainsKey(key))
				{
					this.heroSkinDic.set_Item(key, 18446744073709551615uL);
				}
				else
				{
					this.heroSkinDic.Add(key, 18446744073709551615uL);
				}
			}
		}

		public void OnWearHeroSkin(uint heroId, uint skinId)
		{
			if (this.heroDic.ContainsKey(heroId))
			{
				this.heroDic[heroId].OnHeroSkinWear(skinId);
			}
			if (this.IsFreeHero(heroId))
			{
				this.SetFreeHeroWearSkinId(heroId, skinId);
			}
		}

		public uint UInt32ChgAdjust(uint bs, int chg)
		{
			if (chg >= 0)
			{
				if ((ulong)bs + (ulong)((long)chg) >= (ulong)-1)
				{
					return 4294967295u;
				}
				return bs + (uint)chg;
			}
			else
			{
				uint num = (uint)Math.Abs(chg);
				if (bs <= num)
				{
					return 0u;
				}
				return bs - num;
			}
		}

		private void SetVipFlags(uint flags)
		{
			this.m_vipFlags = flags;
			this.m_vipFlagsValid = true;
			if (this.HasVip(64))
			{
				this.ShowGameRedDot = true;
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent("VipInfoHadSet");
		}

		public bool HasVip(int vipBitFlag)
		{
			return this.m_vipFlagsValid && (this.m_vipFlags & (uint)vipBitFlag) > 0u;
		}

		public ResPVPRatio GetCurDailyAdd()
		{
			ResPVPRatio result = null;
			DictionaryView<uint, ResPVPRatio>.Enumerator enumerator = GameDataMgr.pvpRatioDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, ResPVPRatio> current = enumerator.Current;
				uint key = current.get_Key();
				KeyValuePair<uint, ResPVPRatio> current2 = enumerator.Current;
				result = current2.get_Value();
				if (this.dailyPvpCnt <= key)
				{
					break;
				}
			}
			return result;
		}

		public bool IsOwnHero(uint heroId)
		{
			CHeroInfo cHeroInfo;
			return this.heroDic.TryGetValue(heroId, out cHeroInfo) && !cHeroInfo.IsExperienceHero();
		}

		public bool IsExperienceHero(uint heroId)
		{
			CHeroInfo cHeroInfo;
			return this.heroDic.TryGetValue(heroId, out cHeroInfo);
		}

		public bool IsValidExperienceHero(uint heroId)
		{
			CHeroInfo cHeroInfo;
			return this.heroDic.TryGetValue(heroId, out cHeroInfo) && cHeroInfo.IsValidExperienceHero();
		}

		public int GetExperienceHeroLeftTime(uint heroId)
		{
			CHeroInfo cHeroInfo;
			if (this.heroDic.TryGetValue(heroId, out cHeroInfo) && cHeroInfo.IsExperienceHero())
			{
				return (int)(cHeroInfo.m_experienceDeadLine - (uint)CRoleInfo.GetCurrentUTCTime());
			}
			return 0;
		}

		public bool IsValidExperienceSkin(uint heroId, uint skinId)
		{
			uint skinCfgId = CSkinInfo.GetSkinCfgId(heroId, skinId);
			return this.heroExperienceSkinDic.ContainsKey(skinCfgId) && (long)CRoleInfo.GetCurrentUTCTime() < (long)((ulong)this.heroExperienceSkinDic.get_Item(skinCfgId));
		}

		public int GetExperienceSkinLeftTime(uint heroId, uint skinId)
		{
			uint skinCfgId = CSkinInfo.GetSkinCfgId(heroId, skinId);
			if (this.heroExperienceSkinDic.ContainsKey(skinCfgId))
			{
				return (int)(this.heroExperienceSkinDic.get_Item(skinCfgId) - (uint)CRoleInfo.GetCurrentUTCTime());
			}
			return 0;
		}

		public string GetHeroSkinPic(uint heroId)
		{
			return CSkinInfo.GetHeroSkinPic(heroId, this.GetHeroWearSkinId(heroId));
		}

		public int GetExperienceHeroValidDays(uint heroId)
		{
			CHeroInfo cHeroInfo;
			if (this.heroDic.TryGetValue(heroId, out cHeroInfo))
			{
				return CHeroInfo.GetExperienceHeroOrSkinValidDays(cHeroInfo.m_experienceDeadLine);
			}
			return 0;
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

		public static bool IsPrivacyOpen(ulong userPrivacyBits, COM_USER_PRIVACY_MASK mask)
		{
			return (userPrivacyBits & 1uL << (int)(mask & (COM_USER_PRIVACY_MASK)31)) > 0uL;
		}

		[MessageHandler(4201)]
		public static void OnVipInfoRsp(CSPkg msg)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				masterRoleInfo.SetVipFlags(msg.stPkgData.stQQVIPInfoRsp.dwVIPFlag);
			}
		}

		[MessageHandler(4500)]
		public static void OnDailyCheckDataNtf(CSPkg msg)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				masterRoleInfo.dailyPvpCnt = msg.stPkgData.stDailyCheckDataNtf.dwDailyPvpCnt;
			}
		}

		[MessageHandler(5501)]
		public static void OnCoinLimitNtf(CSPkg msg)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				COMDT_PROFIT_LIMIT stProfitLimit = msg.stPkgData.stProfitLimitNtf.stProfitLimit;
				uint goldWeekMask = masterRoleInfo.m_goldWeekMask;
				int num = 0;
				while ((long)num < (long)((ulong)stProfitLimit.dwProfitNum))
				{
					uint dwProfitType = stProfitLimit.astProfitUnion[num].dwProfitType;
					if (dwProfitType != 1u)
					{
						if (dwProfitType == 2u)
						{
							masterRoleInfo.m_goldWeekCur = stProfitLimit.astProfitUnion[num].stProfitDetail.stCoinLimit.dwCurCoin;
							int num2 = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(90u).dwConfValue;
							num2 += stProfitLimit.astProfitUnion[num].stProfitDetail.stCoinLimit.iExtraCoin;
							if (num2 < 0)
							{
								num2 = 0;
							}
							masterRoleInfo.m_goldWeekLimit = (uint)num2;
							masterRoleInfo.m_goldWeekMask = stProfitLimit.astProfitUnion[num].stProfitDetail.stCoinLimit.dwCoinMask;
						}
					}
					else
					{
						masterRoleInfo.m_expWeekCur = stProfitLimit.astProfitUnion[num].stProfitDetail.stExpLimit.dwCurExp;
						uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(91u).dwConfValue;
						masterRoleInfo.m_expWeekLimit = dwConfValue;
					}
					num++;
				}
				if ((goldWeekMask & 16u) == 0u && (masterRoleInfo.m_goldWeekMask & 16u) != 0u)
				{
					uint dwConfValue2 = GameDataMgr.globalInfoDatabin.GetDataByKey(276u).dwConfValue;
					uint dwConfValue3 = GameDataMgr.globalInfoDatabin.GetDataByKey(277u).dwConfValue;
					Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Settlement_WEEK_Gold_Tips_ActivePoint", new string[]
					{
						dwConfValue2.ToString(),
						dwConfValue3.ToString()
					}), false, 1.5f, null, new object[0]);
				}
			}
		}

		[MessageHandler(5207)]
		public static void ReciveCreditScoreInfo(CSPkg msg)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				masterRoleInfo.creditScore = msg.stPkgData.stNtfAcntCreditValue.dwCreditValue;
				masterRoleInfo.sumDelCreditValue = msg.stPkgData.stNtfAcntCreditValue.iSumDelCreditValue * -1;
				masterRoleInfo.mostDelCreditType = msg.stPkgData.stNtfAcntCreditValue.dwMostDelCreditType;
			}
		}

		public void SetFirstWinRemainingTime(uint time)
		{
			this._nextFirstWinAvailableTime = time;
		}

		public void SetFirstWinLevelLimit(uint level)
		{
			this._firstWinLv = Math.Max(this._firstWinLv, level);
		}

		public bool IsFirstWinOpen()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			return masterRoleInfo.PvpLevel >= this._firstWinLv;
		}

		public int GetCurFirstWinRemainingTimeSec()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				uint currentUTCTime = (uint)CRoleInfo.GetCurrentUTCTime();
				if (this._nextFirstWinAvailableTime >= currentUTCTime)
				{
					return (int)(this._nextFirstWinAvailableTime - currentUTCTime);
				}
			}
			return -1;
		}

		[MessageHandler(1281)]
		public static void OnNextFirstWinTimeChange(CSPkg msg)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				CSPKG_NEXTFIRSTWINSEC_NTF stNextFirstWinSecNtf = msg.stPkgData.stNextFirstWinSecNtf;
				masterRoleInfo.SetFirstWinRemainingTime(stNextFirstWinSecNtf.dwNextFirstWinSec);
			}
		}

		public void SetCoinGetCntData(SCPKG_COINGETPATH_RSP coinData)
		{
			if (coinData == null)
			{
				return;
			}
			int num = 0;
			while ((long)num < (long)((ulong)coinData.dwPathNum))
			{
				if (coinData.astPathCoin[num].dwPath >= 0u && coinData.astPathCoin[num].dwPath < 3u)
				{
					this.m_coinGetInfoDaily[(int)((uint)((UIntPtr)coinData.astPathCoin[num].dwPath))].GetCntDaily = coinData.astPathCoin[num].dwCoin;
				}
				num++;
			}
		}

		public void SetCoinGetLimitDailyCnt()
		{
			ResGlobalInfo resGlobalInfo = new ResGlobalInfo();
			if (GameDataMgr.svr2CltCfgDict.TryGetValue(1u, out resGlobalInfo))
			{
				this.m_coinGetInfoDaily[0].LimitCntDaily = resGlobalInfo.dwConfValue;
			}
			if (GameDataMgr.svr2CltCfgDict.TryGetValue(2u, out resGlobalInfo))
			{
				this.m_coinGetInfoDaily[1].LimitCntDaily = resGlobalInfo.dwConfValue;
			}
			uint num = 0u;
			if (GameDataMgr.svr2CltCfgDict.TryGetValue(3u, out resGlobalInfo))
			{
				num = resGlobalInfo.dwConfValue;
			}
			if (GameDataMgr.svr2CltCfgDict.TryGetValue(4u, out resGlobalInfo))
			{
				this.m_coinGetInfoDaily[2].LimitCntDaily = num * resGlobalInfo.dwConfValue;
			}
		}

		public void GetCoinDailyInfo(RES_COIN_GET_PATH_TYPE pathType, out uint getCnt, out uint limitCnt)
		{
			getCnt = 0u;
			limitCnt = 0u;
			switch (pathType)
			{
			case RES_COIN_GET_PATH_TYPE.RES_COIN_GET_PATH_PVP_BATTLE:
				getCnt = this.m_coinGetInfoDaily[0].GetCntDaily;
				limitCnt = this.m_coinGetInfoDaily[0].LimitCntDaily;
				break;
			case RES_COIN_GET_PATH_TYPE.RES_COIN_GET_PATH_TASK_REWARD:
				getCnt = this.m_coinGetInfoDaily[1].GetCntDaily;
				limitCnt = this.m_coinGetInfoDaily[1].LimitCntDaily;
				break;
			case RES_COIN_GET_PATH_TYPE.RES_COIN_GET_PATH_FRIEND:
				getCnt = this.m_coinGetInfoDaily[2].GetCntDaily;
				limitCnt = this.m_coinGetInfoDaily[2].LimitCntDaily;
				break;
			}
		}

		public void CleanUpBattleRecord()
		{
			int num = 0;
			while ((long)num < (long)((ulong)this.pvpDetail.stKVDetail.dwNum))
			{
				COMDT_STATISTIC_KEY_VALUE_INFO cOMDT_STATISTIC_KEY_VALUE_INFO = this.pvpDetail.stKVDetail.astKVDetail[num];
				cOMDT_STATISTIC_KEY_VALUE_INFO.dwValue = 0u;
				num++;
			}
			this.pvpDetail.stFiveVsFiveInfo.dwTotalNum = 0u;
			this.pvpDetail.stFiveVsFiveInfo.dwWinNum = 0u;
			this.pvpDetail.stThreeVsThreeInfo.dwTotalNum = 0u;
			this.pvpDetail.stThreeVsThreeInfo.dwWinNum = 0u;
			this.pvpDetail.stTwoVsTwoInfo.dwTotalNum = 0u;
			this.pvpDetail.stTwoVsTwoInfo.dwWinNum = 0u;
			this.pvpDetail.stOneVsOneInfo.dwTotalNum = 0u;
			this.pvpDetail.stOneVsOneInfo.dwWinNum = 0u;
			this.pvpDetail.stVsMachineInfo.dwTotalNum = 0u;
			this.pvpDetail.stVsMachineInfo.dwWinNum = 0u;
			this.pvpDetail.stLadderInfo.dwTotalNum = 0u;
			this.pvpDetail.stLadderInfo.dwWinNum = 0u;
			this.pvpDetail.stEntertainmentInfo.dwTotalNum = 0u;
			this.pvpDetail.stEntertainmentInfo.dwWinNum = 0u;
			this.MostUsedHeroDetail.dwHeroNum = 0u;
			Singleton<CPlayerPvpHistoryController>.instance.ClearHostData();
		}

		public uint GetMaxPvpLevel()
		{
			ResAcntPvpExpInfo dataByIndex = GameDataMgr.acntPvpExpDatabin.GetDataByIndex(GameDataMgr.acntPvpExpDatabin.Count() - 1);
			if (dataByIndex != null)
			{
				return (uint)dataByIndex.bLevel;
			}
			return 30u;
		}
	}
}
