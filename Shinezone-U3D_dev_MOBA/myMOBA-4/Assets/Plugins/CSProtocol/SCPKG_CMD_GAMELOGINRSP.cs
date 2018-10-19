using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_CMD_GAMELOGINRSP : ProtocolObject
	{
		public byte bIsSucc;

		public uint dwGameAcntObjID;

		public ulong ullGameAcntUid;

		public uint dwLevel;

		public uint dwExp;

		public uint dwPvpLevel;

		public uint dwPvpExp;

		public uint dwMaxActionPoint;

		public uint dwCurActionPoint;

		public uint dwTitleId;

		public uint dwHeadId;

		public uint dwSkillPoint;

		public byte[] szName;

		public COMDT_ACNT_TASKINFO stLoginTaskInfo;

		public COMDT_ACNT_LEVEL_COMPLETE_DETAIL stPveProgress;

		public COMDT_NEWBIE_STATUS_BITS stNewbieBits;

		public COMDT_CLIENT_BITS stClientBits;

		public COMDT_NEWCLIENT_BITS stNewCltBits;

		public CSDT_ACNT_SHOPBUY_INFO stShopBuyRcd;

		public uint dwServerCurTimeSec;

		public uint dwServerCurTimeMs;

		public uint dwSPUpdTimeSec;

		public byte bGradeOfRank;

		public uint dwClassOfRank;

		public COMDT_ACNT_ACTIVITY_INFO stActivityInfo;

		public ulong ullFuncUnlockFlag;

		public COMDT_ACNT_GUILD_INFO stGuildBaseInfo;

		public COMDT_ACNT_GUILD_EXT_INFO stGuildExtInfo;

		public byte bAcntGM;

		public CSDT_ACNT_ARENADATA stArenaData;

		public uint dwHeroPoolExp;

		public COMDT_COIN_LIST stCoinList;

		public byte[] szHeadUrl;

		public byte bIsVisitorSvr;

		public byte bAcntNewbieType;

		public uint[] BanTime;

		public uint dwFirstLoginTime;

		public COMDT_PROP_MULTIPLE stPropMultiple;

		public uint dwApolloEnvFlag;

		public COMDT_MOST_USED_HERO_DETAIL stMostUsedHero;

		public uint dwQQVipInfo;

		public uint dwLastLoginTime;

		public uint dwChgNameCnt;

		public byte bPrivilege;

		public COMDT_INBATTLE_NEWBIE_BITS_DETAIL stInBattleNewbieBits;

		public uint dwNextFirstWinSec;

		public uint dwFirstWinPvpLvl;

		public byte bGender;

		public COMDT_ACNT_LICENSE stLicense;

		public COMDT_MONTH_WEEK_CARD_INFO stMonthWeekCardInfo;

		public COMDT_ACNT_HEADIMG_LIST stHeadImage;

		public ulong ullLevelRewardFlag;

		public uint dwRefuseFriendBits;

		public uint dwCreditValue;

		public int iSumDelCreditValue;

		public uint dwMostDelCreditType;

		public COMDT_HERO_PRESENT_LIMIT stPresentLimit;

		public byte bIsInBatInputAllowed;

		public byte bPlatChannelOpen;

		public COMDT_SELFDEFINE_CHATINFO stSelfDefineChatInfo;

		public byte bGetCoinNums;

		public COMDT_LIKE_NUMS stLikeNum;

		public byte[] szSignatureInfo;

		public CSDT_ACNTPSWD_LOGININFO stPasswdInfo;

		public byte bAcntOldType;

		public COMDT_RECENT_USED_HERO stRecentUsedHero;

		public COMDT_MOBA_INFO stMobaInfo;

		public COMDT_CHGNAME_CD stChgNameCD;

		public COMDT_ACNT_MASTER_INFO stAcntMasterInfo;

		public ulong ullUserPrivacyBits;

		public uint dwXunyou;

		public uint dwYunying;

		public COMDT_FRIEND_CARD stFriendCard;

		public byte bNoAskforFlag;

		public uint dwSendAskforReqTime;

		public uint dwSendAskforReqCnt;

		public byte bIsReserveMsgAllowed;

		public uint dwOtherStateBits;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 240u;

		public static readonly uint VERSION_dwLastLoginTime = 49u;

		public static readonly uint VERSION_dwChgNameCnt = 53u;

		public static readonly uint VERSION_bPrivilege = 62u;

		public static readonly uint VERSION_stInBattleNewbieBits = 65u;

		public static readonly uint VERSION_dwNextFirstWinSec = 69u;

		public static readonly uint VERSION_dwFirstWinPvpLvl = 69u;

		public static readonly uint VERSION_bGender = 70u;

		public static readonly uint VERSION_bGetCoinNums = 125u;

		public static readonly uint VERSION_stLikeNum = 126u;

		public static readonly uint LENGTH_szName = 64u;

		public static readonly uint LENGTH_szHeadUrl = 256u;

		public static readonly uint LENGTH_szSignatureInfo = 128u;

		public static readonly int CLASS_ID = 726;

		public SCPKG_CMD_GAMELOGINRSP()
		{
			this.szName = new byte[64];
			this.stLoginTaskInfo = (COMDT_ACNT_TASKINFO)ProtocolObjectPool.Get(COMDT_ACNT_TASKINFO.CLASS_ID);
			this.stPveProgress = (COMDT_ACNT_LEVEL_COMPLETE_DETAIL)ProtocolObjectPool.Get(COMDT_ACNT_LEVEL_COMPLETE_DETAIL.CLASS_ID);
			this.stNewbieBits = (COMDT_NEWBIE_STATUS_BITS)ProtocolObjectPool.Get(COMDT_NEWBIE_STATUS_BITS.CLASS_ID);
			this.stClientBits = (COMDT_CLIENT_BITS)ProtocolObjectPool.Get(COMDT_CLIENT_BITS.CLASS_ID);
			this.stNewCltBits = (COMDT_NEWCLIENT_BITS)ProtocolObjectPool.Get(COMDT_NEWCLIENT_BITS.CLASS_ID);
			this.stShopBuyRcd = (CSDT_ACNT_SHOPBUY_INFO)ProtocolObjectPool.Get(CSDT_ACNT_SHOPBUY_INFO.CLASS_ID);
			this.stActivityInfo = (COMDT_ACNT_ACTIVITY_INFO)ProtocolObjectPool.Get(COMDT_ACNT_ACTIVITY_INFO.CLASS_ID);
			this.stGuildBaseInfo = (COMDT_ACNT_GUILD_INFO)ProtocolObjectPool.Get(COMDT_ACNT_GUILD_INFO.CLASS_ID);
			this.stGuildExtInfo = (COMDT_ACNT_GUILD_EXT_INFO)ProtocolObjectPool.Get(COMDT_ACNT_GUILD_EXT_INFO.CLASS_ID);
			this.stArenaData = (CSDT_ACNT_ARENADATA)ProtocolObjectPool.Get(CSDT_ACNT_ARENADATA.CLASS_ID);
			this.stCoinList = (COMDT_COIN_LIST)ProtocolObjectPool.Get(COMDT_COIN_LIST.CLASS_ID);
			this.szHeadUrl = new byte[256];
			this.BanTime = new uint[100];
			this.stPropMultiple = (COMDT_PROP_MULTIPLE)ProtocolObjectPool.Get(COMDT_PROP_MULTIPLE.CLASS_ID);
			this.stMostUsedHero = (COMDT_MOST_USED_HERO_DETAIL)ProtocolObjectPool.Get(COMDT_MOST_USED_HERO_DETAIL.CLASS_ID);
			this.stInBattleNewbieBits = (COMDT_INBATTLE_NEWBIE_BITS_DETAIL)ProtocolObjectPool.Get(COMDT_INBATTLE_NEWBIE_BITS_DETAIL.CLASS_ID);
			this.stLicense = (COMDT_ACNT_LICENSE)ProtocolObjectPool.Get(COMDT_ACNT_LICENSE.CLASS_ID);
			this.stMonthWeekCardInfo = (COMDT_MONTH_WEEK_CARD_INFO)ProtocolObjectPool.Get(COMDT_MONTH_WEEK_CARD_INFO.CLASS_ID);
			this.stHeadImage = (COMDT_ACNT_HEADIMG_LIST)ProtocolObjectPool.Get(COMDT_ACNT_HEADIMG_LIST.CLASS_ID);
			this.stPresentLimit = (COMDT_HERO_PRESENT_LIMIT)ProtocolObjectPool.Get(COMDT_HERO_PRESENT_LIMIT.CLASS_ID);
			this.stSelfDefineChatInfo = (COMDT_SELFDEFINE_CHATINFO)ProtocolObjectPool.Get(COMDT_SELFDEFINE_CHATINFO.CLASS_ID);
			this.stLikeNum = (COMDT_LIKE_NUMS)ProtocolObjectPool.Get(COMDT_LIKE_NUMS.CLASS_ID);
			this.szSignatureInfo = new byte[128];
			this.stPasswdInfo = (CSDT_ACNTPSWD_LOGININFO)ProtocolObjectPool.Get(CSDT_ACNTPSWD_LOGININFO.CLASS_ID);
			this.stRecentUsedHero = (COMDT_RECENT_USED_HERO)ProtocolObjectPool.Get(COMDT_RECENT_USED_HERO.CLASS_ID);
			this.stMobaInfo = (COMDT_MOBA_INFO)ProtocolObjectPool.Get(COMDT_MOBA_INFO.CLASS_ID);
			this.stChgNameCD = (COMDT_CHGNAME_CD)ProtocolObjectPool.Get(COMDT_CHGNAME_CD.CLASS_ID);
			this.stAcntMasterInfo = (COMDT_ACNT_MASTER_INFO)ProtocolObjectPool.Get(COMDT_ACNT_MASTER_INFO.CLASS_ID);
			this.stFriendCard = (COMDT_FRIEND_CARD)ProtocolObjectPool.Get(COMDT_FRIEND_CARD.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = ClassObjPool<TdrWriteBuf>.Get();
			tdrWriteBuf.set(ref buffer, size);
			TdrError.ErrorType errorType = this.pack(ref tdrWriteBuf, cutVer);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				buffer = tdrWriteBuf.getBeginPtr();
				usedSize = tdrWriteBuf.getUsedSize();
			}
			tdrWriteBuf.Release();
			return errorType;
		}

		public override TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
		{
			if (cutVer == 0u || SCPKG_CMD_GAMELOGINRSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_GAMELOGINRSP.CURRVERSION;
			}
			if (SCPKG_CMD_GAMELOGINRSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bIsSucc);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwGameAcntObjID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullGameAcntUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwExp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwPvpLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwPvpExp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwMaxActionPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwCurActionPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTitleId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwHeadId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwSkillPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize2 = destBuf.getUsedSize();
			int num = TdrTypeUtil.cstrlen(this.szName);
			if (num >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szName, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src = destBuf.getUsedSize() - usedSize2;
			errorType = destBuf.writeUInt32((uint)src, usedSize);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stLoginTaskInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPveProgress.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stNewbieBits.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stClientBits.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stNewCltBits.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stShopBuyRcd.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwServerCurTimeSec);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwServerCurTimeMs);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwSPUpdTimeSec);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bGradeOfRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwClassOfRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stActivityInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullFuncUnlockFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGuildBaseInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGuildExtInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bAcntGM);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stArenaData.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwHeroPoolExp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stCoinList.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize3 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize4 = destBuf.getUsedSize();
			int num2 = TdrTypeUtil.cstrlen(this.szHeadUrl);
			if (num2 >= 256)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szHeadUrl, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src2 = destBuf.getUsedSize() - usedSize4;
			errorType = destBuf.writeUInt32((uint)src2, usedSize3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bIsVisitorSvr);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bAcntNewbieType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 100; i++)
			{
				errorType = destBuf.writeUInt32(this.BanTime[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt32(this.dwFirstLoginTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPropMultiple.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwApolloEnvFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMostUsedHero.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwQQVipInfo);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_dwLastLoginTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastLoginTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_dwChgNameCnt <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwChgNameCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_bPrivilege <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bPrivilege);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_stInBattleNewbieBits <= cutVer)
			{
				errorType = this.stInBattleNewbieBits.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_dwNextFirstWinSec <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwNextFirstWinSec);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_dwFirstWinPvpLvl <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwFirstWinPvpLvl);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_bGender <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bGender);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stLicense.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMonthWeekCardInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeadImage.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullLevelRewardFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRefuseFriendBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwCreditValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iSumDelCreditValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwMostDelCreditType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPresentLimit.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bIsInBatInputAllowed);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bPlatChannelOpen);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSelfDefineChatInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_bGetCoinNums <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bGetCoinNums);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_stLikeNum <= cutVer)
			{
				errorType = this.stLikeNum.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			int usedSize5 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize6 = destBuf.getUsedSize();
			int num3 = TdrTypeUtil.cstrlen(this.szSignatureInfo);
			if (num3 >= 128)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szSignatureInfo, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src3 = destBuf.getUsedSize() - usedSize6;
			errorType = destBuf.writeUInt32((uint)src3, usedSize5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPasswdInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bAcntOldType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRecentUsedHero.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMobaInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stChgNameCD.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stAcntMasterInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullUserPrivacyBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwXunyou);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwYunying);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFriendCard.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bNoAskforFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwSendAskforReqTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwSendAskforReqCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bIsReserveMsgAllowed);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwOtherStateBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = ClassObjPool<TdrReadBuf>.Get();
			tdrReadBuf.set(ref buffer, size);
			TdrError.ErrorType result = this.unpack(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			tdrReadBuf.Release();
			return result;
		}

		public override TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || SCPKG_CMD_GAMELOGINRSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_GAMELOGINRSP.CURRVERSION;
			}
			if (SCPKG_CMD_GAMELOGINRSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bIsSucc);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGameAcntObjID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullGameAcntUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwExp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPvpLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPvpExp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMaxActionPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCurActionPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTitleId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHeadId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSkillPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num = 0u;
			errorType = srcBuf.readUInt32(ref num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num > (uint)this.szName.GetLength(0))
			{
				if ((long)num > (long)((ulong)SCPKG_CMD_GAMELOGINRSP.LENGTH_szName))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szName = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szName, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szName[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szName) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = this.stLoginTaskInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPveProgress.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stNewbieBits.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stClientBits.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stNewCltBits.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stShopBuyRcd.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwServerCurTimeSec);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwServerCurTimeMs);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSPUpdTimeSec);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bGradeOfRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwClassOfRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stActivityInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullFuncUnlockFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGuildBaseInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGuildExtInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bAcntGM);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stArenaData.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHeroPoolExp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stCoinList.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num3 = 0u;
			errorType = srcBuf.readUInt32(ref num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num3 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num3 > (uint)this.szHeadUrl.GetLength(0))
			{
				if ((long)num3 > (long)((ulong)SCPKG_CMD_GAMELOGINRSP.LENGTH_szHeadUrl))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szHeadUrl = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szHeadUrl, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szHeadUrl[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szHeadUrl) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bIsVisitorSvr);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bAcntNewbieType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 100; i++)
			{
				errorType = srcBuf.readUInt32(ref this.BanTime[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwFirstLoginTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPropMultiple.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwApolloEnvFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMostUsedHero.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwQQVipInfo);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_dwLastLoginTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastLoginTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastLoginTime = 0u;
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_dwChgNameCnt <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwChgNameCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwChgNameCnt = 0u;
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_bPrivilege <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bPrivilege);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bPrivilege = 0;
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_stInBattleNewbieBits <= cutVer)
			{
				errorType = this.stInBattleNewbieBits.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stInBattleNewbieBits.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_dwNextFirstWinSec <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwNextFirstWinSec);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwNextFirstWinSec = 0u;
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_dwFirstWinPvpLvl <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwFirstWinPvpLvl);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwFirstWinPvpLvl = 0u;
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_bGender <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bGender);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bGender = 0;
			}
			errorType = this.stLicense.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMonthWeekCardInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeadImage.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullLevelRewardFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRefuseFriendBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCreditValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSumDelCreditValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMostDelCreditType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPresentLimit.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsInBatInputAllowed);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bPlatChannelOpen);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSelfDefineChatInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_bGetCoinNums <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bGetCoinNums);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bGetCoinNums = 0;
			}
			if (SCPKG_CMD_GAMELOGINRSP.VERSION_stLikeNum <= cutVer)
			{
				errorType = this.stLikeNum.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stLikeNum.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			uint num5 = 0u;
			errorType = srcBuf.readUInt32(ref num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num5 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num5 > (uint)this.szSignatureInfo.GetLength(0))
			{
				if ((long)num5 > (long)((ulong)SCPKG_CMD_GAMELOGINRSP.LENGTH_szSignatureInfo))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSignatureInfo = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSignatureInfo, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSignatureInfo[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szSignatureInfo) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = this.stPasswdInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bAcntOldType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRecentUsedHero.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMobaInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stChgNameCD.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stAcntMasterInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullUserPrivacyBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwXunyou);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwYunying);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFriendCard.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bNoAskforFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSendAskforReqTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSendAskforReqCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsReserveMsgAllowed);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwOtherStateBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_CMD_GAMELOGINRSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bIsSucc = 0;
			this.dwGameAcntObjID = 0u;
			this.ullGameAcntUid = 0uL;
			this.dwLevel = 0u;
			this.dwExp = 0u;
			this.dwPvpLevel = 0u;
			this.dwPvpExp = 0u;
			this.dwMaxActionPoint = 0u;
			this.dwCurActionPoint = 0u;
			this.dwTitleId = 0u;
			this.dwHeadId = 0u;
			this.dwSkillPoint = 0u;
			if (this.stLoginTaskInfo != null)
			{
				this.stLoginTaskInfo.Release();
				this.stLoginTaskInfo = null;
			}
			if (this.stPveProgress != null)
			{
				this.stPveProgress.Release();
				this.stPveProgress = null;
			}
			if (this.stNewbieBits != null)
			{
				this.stNewbieBits.Release();
				this.stNewbieBits = null;
			}
			if (this.stClientBits != null)
			{
				this.stClientBits.Release();
				this.stClientBits = null;
			}
			if (this.stNewCltBits != null)
			{
				this.stNewCltBits.Release();
				this.stNewCltBits = null;
			}
			if (this.stShopBuyRcd != null)
			{
				this.stShopBuyRcd.Release();
				this.stShopBuyRcd = null;
			}
			this.dwServerCurTimeSec = 0u;
			this.dwServerCurTimeMs = 0u;
			this.dwSPUpdTimeSec = 0u;
			this.bGradeOfRank = 0;
			this.dwClassOfRank = 0u;
			if (this.stActivityInfo != null)
			{
				this.stActivityInfo.Release();
				this.stActivityInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stLoginTaskInfo = (COMDT_ACNT_TASKINFO)ProtocolObjectPool.Get(COMDT_ACNT_TASKINFO.CLASS_ID);
			this.stPveProgress = (COMDT_ACNT_LEVEL_COMPLETE_DETAIL)ProtocolObjectPool.Get(COMDT_ACNT_LEVEL_COMPLETE_DETAIL.CLASS_ID);
			this.stNewbieBits = (COMDT_NEWBIE_STATUS_BITS)ProtocolObjectPool.Get(COMDT_NEWBIE_STATUS_BITS.CLASS_ID);
			this.stClientBits = (COMDT_CLIENT_BITS)ProtocolObjectPool.Get(COMDT_CLIENT_BITS.CLASS_ID);
			this.stNewCltBits = (COMDT_NEWCLIENT_BITS)ProtocolObjectPool.Get(COMDT_NEWCLIENT_BITS.CLASS_ID);
			this.stShopBuyRcd = (CSDT_ACNT_SHOPBUY_INFO)ProtocolObjectPool.Get(CSDT_ACNT_SHOPBUY_INFO.CLASS_ID);
			this.stActivityInfo = (COMDT_ACNT_ACTIVITY_INFO)ProtocolObjectPool.Get(COMDT_ACNT_ACTIVITY_INFO.CLASS_ID);
		}
	}
}
