using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_CHEATCMD_DETAIL : ProtocolObject
	{
		public ProtocolObject dataObject;

		public sbyte chReserve;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 208u;

		public static readonly int CLASS_ID = 859;

		public CSDT_CHEAT_COMVAL stAddCoin
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stBuyCoupons
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stSetAcntLvl
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stAddAcntExp
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stAddAcntMaxAP
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stAddAcntCurAP
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_ITEMINFO stAddItem
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_ITEMINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_HERO stAddHero
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_HERO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_TASKDONE stTaskDone
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_TASKDONE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_HEROVAL stSetHeroLvl
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_HEROVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_HEROVAL stAddHeroExp
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_HEROVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_UNLOCK_LEVEL stUnlockLevel
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_UNLOCK_LEVEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_SENDMAIL stSendMail
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_SENDMAIL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_UPDACNTINFO stUpdAcntInfo
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_UPDACNTINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_RANDOMREWARD stRandomReward
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_RANDOMREWARD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stSetMMROfRank
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stSetGradeOfRank
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stSetScoreOfRank
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stSetConWinCntOfRank
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stSetConLoseCntOfRank
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stSetMMROfNormal
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_SHOPTYPE stClrShopRefresh
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_SHOPTYPE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_SETHEROSTAR stSetHeroStar
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_SETHEROSTAR;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_SETHEROQUALITY stSetHeroQuality
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_SETHEROQUALITY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_UNLOCK_ACTIVITY stUnlockActivity
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_UNLOCK_ACTIVITY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_CLR_ELITE_LEVEL stClrEliteLevel
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_CLR_ELITE_LEVEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_DYE_NEWBIE_BIT stDyeNewbieBit
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_DYE_NEWBIE_BIT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_PASS_SINGLE_GAME stPassSingleGame
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_PASS_SINGLE_GAME;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_PASS_MULTI_GAME stPassMultiGame
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_PASS_MULTI_GAME;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_SET_OFFSET_SEC stSetOffsetSec
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_SET_OFFSET_SEC;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_SET_FREE_HERO stSetFreeHero
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_SET_FREE_HERO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_UNLOCK_HEROPVPMASK stUnlockHeroPVPMask
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_UNLOCK_HEROPVPMASK;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_SHOPTYPE stOpenAPRefreshShop
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_SHOPTYPE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_CLR_BURNING_LIMIT stClrBurningLimit
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_CLR_BURNING_LIMIT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stAddPvpCoin
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stAddPvpExp
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stSetPvpLevel
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stRefreshPower
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_SET_GUILD_INFO stSetGuildInfo
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_SET_GUILD_INFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stAddBurningCoin
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stAddArenaCoin
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_SET_SKILLLVL_MAX stSetSkillLvlMax
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_SET_SKILLLVL_MAX;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stAddSkinCoin
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_GEARADV_ALL stGearAdvAll
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_GEARADV_ALL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stGiveCoupons
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stCutPackageCnt
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stAddSymbolCoin
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_HEROVAL stAddHeroProficiency
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_HEROVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stAddDiamond
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_HERO stHeroWake
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_HERO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_HERO stHeroSleep
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_HERO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stSetRankTotalFightCnt
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stSetRankTotalWinCnt
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stSetMaxFriendCnt
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stAddAllSkin
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_HUOYUEDU stHuoYueDuOpt
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_HUOYUEDU;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stTodayWarmCnt
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_WARMBATTLE_CNT stWarmBattleCnt
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_WARMBATTLE_CNT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stWarmBattleConLoseCnt
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stWarmBattleKillNum
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stWarmBattleDeadNum
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_SET_GUILD_MEMBER_INFO stSetGuildMemberInfo
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_SET_GUILD_MEMBER_INFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_SET_HERO_CUSTOM_EQUIP stSetHeroCustomEquip
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_SET_HERO_CUSTOM_EQUIP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_HEADIMAGE_ADD stAddHeadImage
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_HEADIMAGE_ADD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_HEADIMAGE_DEL stDelHeadImage
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_HEADIMAGE_DEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stChgAcntCreditValue
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_MATCHPOINT stAddRewardMatchPoint
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_MATCHPOINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_MATCHPOOL stAddRewardMatchPool
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_MATCHPOOL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_LEVELREWARD stLevelReward
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_LEVELREWARD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_CHG_NEW_NORMALMMR stChgNewNormalMMR
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_CHG_NEW_NORMALMMR;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_CHG_HONORINFO stChgHonorInfo
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_CHG_HONORINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_CHG_REWARDMATCH_INFO stChgRewardMatchInfo
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_CHG_REWARDMATCH_INFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CREATE_GUILD stCreateGuild
		{
			get
			{
				return this.dataObject as CSDT_CREATE_GUILD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_SET_MASTERHERO stSetMasterHero
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_SET_MASTERHERO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_ADD_FIGHTHISTORY stAddFightHistory
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_ADD_FIGHTHISTORY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_ADD_GAME_STATISTIC stAddGameStatistic
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_ADD_GAME_STATISTIC;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_SET_MOBA_INFO stSetMobaInfo
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_SET_MOBA_INFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_GET_MOBA_INFO stGetMobaInfo
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_GET_MOBA_INFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stClrChgNameCD
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stClrProfitLimit
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_CHG_INTIMACY stChgIntimacy
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_CHG_INTIMACY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_RESET_ACHIEVE stResetAchieve
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_RESET_ACHIEVE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_DONE_ACHIEVE stDoneAchieve
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_DONE_ACHIEVE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_DELCREDIT stDelCreditByType
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_DELCREDIT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stAddMasterPoint
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_INTIMACYRELATION stIntimacyRelation
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_INTIMACYRELATION;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_RECRUITMENTRELATION stRecruitmentRelation
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_RECRUITMENTRELATION;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_COMVAL stSetAddStarScore
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_COMVAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CHEAT_SET_OFFSET_SEC stSetPvpBanEndTime
		{
			get
			{
				return this.dataObject as CSDT_CHEAT_SET_OFFSET_SEC;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 61L)
			{
				this.select_1_61(selector);
			}
			else if (selector <= 91L)
			{
				this.select_62_91(selector);
			}
			else if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			return this.dataObject;
		}

		public TdrError.ErrorType construct(long selector)
		{
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.construct();
			}
			this.chReserve = 0;
			return result;
		}

		public TdrError.ErrorType pack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = ClassObjPool<TdrWriteBuf>.Get();
			tdrWriteBuf.set(ref buffer, size);
			TdrError.ErrorType errorType = this.pack(selector, ref tdrWriteBuf, cutVer);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				buffer = tdrWriteBuf.getBeginPtr();
				usedSize = tdrWriteBuf.getUsedSize();
			}
			tdrWriteBuf.Release();
			return errorType;
		}

		public TdrError.ErrorType pack(long selector, ref TdrWriteBuf destBuf, uint cutVer)
		{
			if (cutVer == 0u || CSDT_CHEATCMD_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = CSDT_CHEATCMD_DETAIL.CURRVERSION;
			}
			if (CSDT_CHEATCMD_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.pack(ref destBuf, cutVer);
			}
			TdrError.ErrorType errorType = destBuf.writeInt8(this.chReserve);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public TdrError.ErrorType unpack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = ClassObjPool<TdrReadBuf>.Get();
			tdrReadBuf.set(ref buffer, size);
			TdrError.ErrorType result = this.unpack(selector, ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			tdrReadBuf.Release();
			return result;
		}

		public TdrError.ErrorType unpack(long selector, ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || CSDT_CHEATCMD_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = CSDT_CHEATCMD_DETAIL.CURRVERSION;
			}
			if (CSDT_CHEATCMD_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.unpack(ref srcBuf, cutVer);
			}
			TdrError.ErrorType errorType = srcBuf.readInt8(ref this.chReserve);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		private void select_1_61(long selector)
		{
			if (selector >= 1L && selector <= 61L)
			{
				switch ((int)(selector - 1L))
				{
				case 0:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is CSDT_CHEAT_ITEMINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_ITEMINFO)ProtocolObjectPool.Get(CSDT_CHEAT_ITEMINFO.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is CSDT_CHEAT_HERO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_HERO)ProtocolObjectPool.Get(CSDT_CHEAT_HERO.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is CSDT_CHEAT_TASKDONE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_TASKDONE)ProtocolObjectPool.Get(CSDT_CHEAT_TASKDONE.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is CSDT_CHEAT_HEROVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_HEROVAL)ProtocolObjectPool.Get(CSDT_CHEAT_HEROVAL.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is CSDT_CHEAT_HEROVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_HEROVAL)ProtocolObjectPool.Get(CSDT_CHEAT_HEROVAL.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is CSDT_CHEAT_UNLOCK_LEVEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_UNLOCK_LEVEL)ProtocolObjectPool.Get(CSDT_CHEAT_UNLOCK_LEVEL.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is CSDT_CHEAT_SENDMAIL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_SENDMAIL)ProtocolObjectPool.Get(CSDT_CHEAT_SENDMAIL.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is CSDT_CHEAT_UPDACNTINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_UPDACNTINFO)ProtocolObjectPool.Get(CSDT_CHEAT_UPDACNTINFO.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is CSDT_CHEAT_RANDOMREWARD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_RANDOMREWARD)ProtocolObjectPool.Get(CSDT_CHEAT_RANDOMREWARD.CLASS_ID);
					}
					return;
				case 16:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 17:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 18:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 22:
					if (!(this.dataObject is CSDT_CHEAT_SHOPTYPE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_SHOPTYPE)ProtocolObjectPool.Get(CSDT_CHEAT_SHOPTYPE.CLASS_ID);
					}
					return;
				case 23:
					if (!(this.dataObject is CSDT_CHEAT_SETHEROSTAR))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_SETHEROSTAR)ProtocolObjectPool.Get(CSDT_CHEAT_SETHEROSTAR.CLASS_ID);
					}
					return;
				case 24:
					if (!(this.dataObject is CSDT_CHEAT_SETHEROQUALITY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_SETHEROQUALITY)ProtocolObjectPool.Get(CSDT_CHEAT_SETHEROQUALITY.CLASS_ID);
					}
					return;
				case 25:
					if (!(this.dataObject is CSDT_CHEAT_UNLOCK_ACTIVITY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_UNLOCK_ACTIVITY)ProtocolObjectPool.Get(CSDT_CHEAT_UNLOCK_ACTIVITY.CLASS_ID);
					}
					return;
				case 26:
					if (!(this.dataObject is CSDT_CHEAT_CLR_ELITE_LEVEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_CLR_ELITE_LEVEL)ProtocolObjectPool.Get(CSDT_CHEAT_CLR_ELITE_LEVEL.CLASS_ID);
					}
					return;
				case 27:
					if (!(this.dataObject is CSDT_CHEAT_DYE_NEWBIE_BIT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_DYE_NEWBIE_BIT)ProtocolObjectPool.Get(CSDT_CHEAT_DYE_NEWBIE_BIT.CLASS_ID);
					}
					return;
				case 28:
					if (!(this.dataObject is CSDT_CHEAT_PASS_SINGLE_GAME))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_PASS_SINGLE_GAME)ProtocolObjectPool.Get(CSDT_CHEAT_PASS_SINGLE_GAME.CLASS_ID);
					}
					return;
				case 29:
					if (!(this.dataObject is CSDT_CHEAT_PASS_MULTI_GAME))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_PASS_MULTI_GAME)ProtocolObjectPool.Get(CSDT_CHEAT_PASS_MULTI_GAME.CLASS_ID);
					}
					return;
				case 30:
					if (!(this.dataObject is CSDT_CHEAT_SET_OFFSET_SEC))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_SET_OFFSET_SEC)ProtocolObjectPool.Get(CSDT_CHEAT_SET_OFFSET_SEC.CLASS_ID);
					}
					return;
				case 31:
					if (!(this.dataObject is CSDT_CHEAT_SET_FREE_HERO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_SET_FREE_HERO)ProtocolObjectPool.Get(CSDT_CHEAT_SET_FREE_HERO.CLASS_ID);
					}
					return;
				case 32:
					if (!(this.dataObject is CSDT_CHEAT_UNLOCK_HEROPVPMASK))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_UNLOCK_HEROPVPMASK)ProtocolObjectPool.Get(CSDT_CHEAT_UNLOCK_HEROPVPMASK.CLASS_ID);
					}
					return;
				case 33:
					if (!(this.dataObject is CSDT_CHEAT_SHOPTYPE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_SHOPTYPE)ProtocolObjectPool.Get(CSDT_CHEAT_SHOPTYPE.CLASS_ID);
					}
					return;
				case 34:
					if (!(this.dataObject is CSDT_CHEAT_CLR_BURNING_LIMIT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_CLR_BURNING_LIMIT)ProtocolObjectPool.Get(CSDT_CHEAT_CLR_BURNING_LIMIT.CLASS_ID);
					}
					return;
				case 35:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 36:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 37:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 38:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 39:
					if (!(this.dataObject is CSDT_CHEAT_SET_GUILD_INFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_SET_GUILD_INFO)ProtocolObjectPool.Get(CSDT_CHEAT_SET_GUILD_INFO.CLASS_ID);
					}
					return;
				case 40:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 41:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 42:
					if (!(this.dataObject is CSDT_CHEAT_SET_SKILLLVL_MAX))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_SET_SKILLLVL_MAX)ProtocolObjectPool.Get(CSDT_CHEAT_SET_SKILLLVL_MAX.CLASS_ID);
					}
					return;
				case 43:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 44:
					if (!(this.dataObject is CSDT_CHEAT_GEARADV_ALL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_GEARADV_ALL)ProtocolObjectPool.Get(CSDT_CHEAT_GEARADV_ALL.CLASS_ID);
					}
					return;
				case 45:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 46:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 47:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 48:
					if (!(this.dataObject is CSDT_CHEAT_HEROVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_HEROVAL)ProtocolObjectPool.Get(CSDT_CHEAT_HEROVAL.CLASS_ID);
					}
					return;
				case 49:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 50:
					if (!(this.dataObject is CSDT_CHEAT_HERO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_HERO)ProtocolObjectPool.Get(CSDT_CHEAT_HERO.CLASS_ID);
					}
					return;
				case 51:
					if (!(this.dataObject is CSDT_CHEAT_HERO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_HERO)ProtocolObjectPool.Get(CSDT_CHEAT_HERO.CLASS_ID);
					}
					return;
				case 52:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 53:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 54:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 55:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 56:
					if (!(this.dataObject is CSDT_CHEAT_HUOYUEDU))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_HUOYUEDU)ProtocolObjectPool.Get(CSDT_CHEAT_HUOYUEDU.CLASS_ID);
					}
					return;
				case 57:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 58:
					if (!(this.dataObject is CSDT_CHEAT_WARMBATTLE_CNT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_WARMBATTLE_CNT)ProtocolObjectPool.Get(CSDT_CHEAT_WARMBATTLE_CNT.CLASS_ID);
					}
					return;
				case 59:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 60:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				}
			}
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}

		private void select_62_91(long selector)
		{
			if (selector >= 62L && selector <= 91L)
			{
				switch ((int)(selector - 62L))
				{
				case 0:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is CSDT_CHEAT_SET_GUILD_MEMBER_INFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_SET_GUILD_MEMBER_INFO)ProtocolObjectPool.Get(CSDT_CHEAT_SET_GUILD_MEMBER_INFO.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSDT_CHEAT_SET_HERO_CUSTOM_EQUIP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_SET_HERO_CUSTOM_EQUIP)ProtocolObjectPool.Get(CSDT_CHEAT_SET_HERO_CUSTOM_EQUIP.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSDT_CHEAT_HEADIMAGE_ADD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_HEADIMAGE_ADD)ProtocolObjectPool.Get(CSDT_CHEAT_HEADIMAGE_ADD.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSDT_CHEAT_HEADIMAGE_DEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_HEADIMAGE_DEL)ProtocolObjectPool.Get(CSDT_CHEAT_HEADIMAGE_DEL.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is CSDT_CHEAT_MATCHPOINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_MATCHPOINT)ProtocolObjectPool.Get(CSDT_CHEAT_MATCHPOINT.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is CSDT_CHEAT_MATCHPOOL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_MATCHPOOL)ProtocolObjectPool.Get(CSDT_CHEAT_MATCHPOOL.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is CSDT_CHEAT_LEVELREWARD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_LEVELREWARD)ProtocolObjectPool.Get(CSDT_CHEAT_LEVELREWARD.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is CSDT_CHEAT_CHG_NEW_NORMALMMR))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_CHG_NEW_NORMALMMR)ProtocolObjectPool.Get(CSDT_CHEAT_CHG_NEW_NORMALMMR.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is CSDT_CHEAT_CHG_HONORINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_CHG_HONORINFO)ProtocolObjectPool.Get(CSDT_CHEAT_CHG_HONORINFO.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is CSDT_CHEAT_CHG_REWARDMATCH_INFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_CHG_REWARDMATCH_INFO)ProtocolObjectPool.Get(CSDT_CHEAT_CHG_REWARDMATCH_INFO.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is CSDT_CREATE_GUILD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CREATE_GUILD)ProtocolObjectPool.Get(CSDT_CREATE_GUILD.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is CSDT_CHEAT_SET_MASTERHERO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_SET_MASTERHERO)ProtocolObjectPool.Get(CSDT_CHEAT_SET_MASTERHERO.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is CSDT_CHEAT_ADD_FIGHTHISTORY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_ADD_FIGHTHISTORY)ProtocolObjectPool.Get(CSDT_CHEAT_ADD_FIGHTHISTORY.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is CSDT_CHEAT_ADD_GAME_STATISTIC))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_ADD_GAME_STATISTIC)ProtocolObjectPool.Get(CSDT_CHEAT_ADD_GAME_STATISTIC.CLASS_ID);
					}
					return;
				case 17:
					if (!(this.dataObject is CSDT_CHEAT_SET_MOBA_INFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_SET_MOBA_INFO)ProtocolObjectPool.Get(CSDT_CHEAT_SET_MOBA_INFO.CLASS_ID);
					}
					return;
				case 18:
					if (!(this.dataObject is CSDT_CHEAT_GET_MOBA_INFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_GET_MOBA_INFO)ProtocolObjectPool.Get(CSDT_CHEAT_GET_MOBA_INFO.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is CSDT_CHEAT_CHG_INTIMACY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_CHG_INTIMACY)ProtocolObjectPool.Get(CSDT_CHEAT_CHG_INTIMACY.CLASS_ID);
					}
					return;
				case 22:
					if (!(this.dataObject is CSDT_CHEAT_RESET_ACHIEVE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_RESET_ACHIEVE)ProtocolObjectPool.Get(CSDT_CHEAT_RESET_ACHIEVE.CLASS_ID);
					}
					return;
				case 23:
					if (!(this.dataObject is CSDT_CHEAT_DONE_ACHIEVE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_DONE_ACHIEVE)ProtocolObjectPool.Get(CSDT_CHEAT_DONE_ACHIEVE.CLASS_ID);
					}
					return;
				case 24:
					if (!(this.dataObject is CSDT_CHEAT_DELCREDIT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_DELCREDIT)ProtocolObjectPool.Get(CSDT_CHEAT_DELCREDIT.CLASS_ID);
					}
					return;
				case 25:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 26:
					if (!(this.dataObject is CSDT_CHEAT_INTIMACYRELATION))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_INTIMACYRELATION)ProtocolObjectPool.Get(CSDT_CHEAT_INTIMACYRELATION.CLASS_ID);
					}
					return;
				case 27:
					if (!(this.dataObject is CSDT_CHEAT_RECRUITMENTRELATION))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_RECRUITMENTRELATION)ProtocolObjectPool.Get(CSDT_CHEAT_RECRUITMENTRELATION.CLASS_ID);
					}
					return;
				case 28:
					if (!(this.dataObject is CSDT_CHEAT_COMVAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_COMVAL)ProtocolObjectPool.Get(CSDT_CHEAT_COMVAL.CLASS_ID);
					}
					return;
				case 29:
					if (!(this.dataObject is CSDT_CHEAT_SET_OFFSET_SEC))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CHEAT_SET_OFFSET_SEC)ProtocolObjectPool.Get(CSDT_CHEAT_SET_OFFSET_SEC.CLASS_ID);
					}
					return;
				}
			}
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}

		public override int GetClassID()
		{
			return CSDT_CHEATCMD_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			this.chReserve = 0;
		}
	}
}
