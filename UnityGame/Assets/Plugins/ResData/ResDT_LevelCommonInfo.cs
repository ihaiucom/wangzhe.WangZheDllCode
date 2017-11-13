using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResDT_LevelCommonInfo : tsf4g_csharp_interface, IUnpackable
	{
		public byte[] szName_ByteArray;

		public byte[] szDesignFileName_ByteArray;

		public byte[] szArtistFileName_ByteArray;

		public byte bMaxAcntNum;

		public byte bValidRoomType;

		public byte bHeroNum;

		public byte bIsAllowHeroDup;

		public uint dwHeroFormId;

		public int iHeroAIType;

		public byte[] szThumbnailPath_ByteArray;

		public byte[] szMapPath_ByteArray;

		public byte[] szBigMapPath_ByteArray;

		public int iMapWidth;

		public int iMapHeight;

		public int iBigMapWidth;

		public int iBigMapHeight;

		public float fMapFowScale;

		public float fBigMapFowScale;

		public uint dwSoulID;

		public byte bIsOpenExpCompensate;

		public ResDT_ExpCompensateInfo[] astExpCompensateDetail;

		public int iExtraSkillId;

		public int iExtraSkill2Id;

		public int iExtraPassiveSkillId;

		public byte bFinResultType;

		public byte bRandPickHero;

		public uint dwAddWinCondStarId;

		public uint dwAddLoseCondStarId;

		public uint dwTimeDuration;

		public uint dwSoulAllocId;

		public byte bCameraFlip;

		public int iSoldierActivateCountDelay1;

		public int iSoldierActivateCountDelay2;

		public int iSoldierActivateDelay;

		public byte bBattleEquipLimit;

		public byte bBirthLevelConfig;

		public byte bChaosPickRule;

		public byte bHeadPtsUpperLimit;

		public byte bSrvLeastDestoryTowerNum;

		public byte bSrvLeastDestoryBaseNum;

		public byte bSrvLeastKillCntNum;

		public byte bDealHangUp;

		public uint dwUnLockCondID;

		public byte bShowHonor;

		public ResDT_PickRuleInfo stPickRuleInfo;

		public uint dwCooldownReduceUpperLimit;

		public int iPvpDifficulty;

		public uint dwChatID;

		public ResDT_UnUseSkill stUnUseSkillInfo;

		public uint dwAttackOrderID;

		public uint dwDynamicPropertyCfg;

		public ushort wOriginalGoldCoinInBattle;

		public uint dwBattleTaskOfCamp1;

		public uint dwBattleTaskOfCamp2;

		public byte[] szMusicStartEvent_ByteArray;

		public byte[] szMusicEndEvent_ByteArray;

		public byte[] szAmbientSoundEvent_ByteArray;

		public byte[] szBankResourceName_ByteArray;

		public byte bSupportCameraDrag;

		public byte[] szGameMatchName_ByteArray;

		public byte bIsEnableFow;

		public byte bIsEnableShopHorizonTab;

		public byte bIsEnableOrnamentSlot;

		public int iOrnamentSkillId;

		public int iOrnamentSwitchCD;

		public int iOrnamentFirstSwitchCD;

		public int iOrnamentFirstSwitchCDEftTime;

		public uint dwJudgeNum;

		public byte bPauseNum;

		public uint dwWealGameSubType;

		public byte bSupportHighTowerSoldier;

		public byte bShowTeamHint;

		public byte bShowWinRatio;

		public byte bForceTimeoutPick;

		public int iFakeSightRange;

		public string szName;

		public string szDesignFileName;

		public string szArtistFileName;

		public string szThumbnailPath;

		public string szMapPath;

		public string szBigMapPath;

		public string szMusicStartEvent;

		public string szMusicEndEvent;

		public string szAmbientSoundEvent;

		public string szBankResourceName;

		public string szGameMatchName;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szName = 32u;

		public static readonly uint LENGTH_szDesignFileName = 32u;

		public static readonly uint LENGTH_szArtistFileName = 32u;

		public static readonly uint LENGTH_szThumbnailPath = 128u;

		public static readonly uint LENGTH_szMapPath = 128u;

		public static readonly uint LENGTH_szBigMapPath = 128u;

		public static readonly uint LENGTH_szMusicStartEvent = 32u;

		public static readonly uint LENGTH_szMusicEndEvent = 32u;

		public static readonly uint LENGTH_szAmbientSoundEvent = 32u;

		public static readonly uint LENGTH_szBankResourceName = 32u;

		public static readonly uint LENGTH_szGameMatchName = 256u;

		public ResDT_LevelCommonInfo()
		{
			this.szName_ByteArray = new byte[1];
			this.szDesignFileName_ByteArray = new byte[1];
			this.szArtistFileName_ByteArray = new byte[1];
			this.szThumbnailPath_ByteArray = new byte[1];
			this.szMapPath_ByteArray = new byte[1];
			this.szBigMapPath_ByteArray = new byte[1];
			this.astExpCompensateDetail = new ResDT_ExpCompensateInfo[5];
			for (int i = 0; i < 5; i++)
			{
				this.astExpCompensateDetail[i] = new ResDT_ExpCompensateInfo();
			}
			this.stPickRuleInfo = new ResDT_PickRuleInfo();
			this.stUnUseSkillInfo = new ResDT_UnUseSkill();
			this.szMusicStartEvent_ByteArray = new byte[1];
			this.szMusicEndEvent_ByteArray = new byte[1];
			this.szAmbientSoundEvent_ByteArray = new byte[1];
			this.szBankResourceName_ByteArray = new byte[1];
			this.szGameMatchName_ByteArray = new byte[1];
			this.szName = string.Empty;
			this.szDesignFileName = string.Empty;
			this.szArtistFileName = string.Empty;
			this.szThumbnailPath = string.Empty;
			this.szMapPath = string.Empty;
			this.szBigMapPath = string.Empty;
			this.szMusicStartEvent = string.Empty;
			this.szMusicEndEvent = string.Empty;
			this.szAmbientSoundEvent = string.Empty;
			this.szBankResourceName = string.Empty;
			this.szGameMatchName = string.Empty;
		}

		private void TransferData()
		{
			this.szName = StringHelper.UTF8BytesToString(ref this.szName_ByteArray);
			this.szName_ByteArray = null;
			this.szDesignFileName = StringHelper.UTF8BytesToString(ref this.szDesignFileName_ByteArray);
			this.szDesignFileName_ByteArray = null;
			this.szArtistFileName = StringHelper.UTF8BytesToString(ref this.szArtistFileName_ByteArray);
			this.szArtistFileName_ByteArray = null;
			this.szThumbnailPath = StringHelper.UTF8BytesToString(ref this.szThumbnailPath_ByteArray);
			this.szThumbnailPath_ByteArray = null;
			this.szMapPath = StringHelper.UTF8BytesToString(ref this.szMapPath_ByteArray);
			this.szMapPath_ByteArray = null;
			this.szBigMapPath = StringHelper.UTF8BytesToString(ref this.szBigMapPath_ByteArray);
			this.szBigMapPath_ByteArray = null;
			this.szMusicStartEvent = StringHelper.UTF8BytesToString(ref this.szMusicStartEvent_ByteArray);
			this.szMusicStartEvent_ByteArray = null;
			this.szMusicEndEvent = StringHelper.UTF8BytesToString(ref this.szMusicEndEvent_ByteArray);
			this.szMusicEndEvent_ByteArray = null;
			this.szAmbientSoundEvent = StringHelper.UTF8BytesToString(ref this.szAmbientSoundEvent_ByteArray);
			this.szAmbientSoundEvent_ByteArray = null;
			this.szBankResourceName = StringHelper.UTF8BytesToString(ref this.szBankResourceName_ByteArray);
			this.szBankResourceName_ByteArray = null;
			this.szGameMatchName = StringHelper.UTF8BytesToString(ref this.szGameMatchName_ByteArray);
			this.szGameMatchName_ByteArray = null;
		}

		public TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref buffer, size);
			TdrError.ErrorType result = this.unpack(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			return result;
		}

		public TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || ResDT_LevelCommonInfo.CURRVERSION < cutVer)
			{
				cutVer = ResDT_LevelCommonInfo.CURRVERSION;
			}
			if (ResDT_LevelCommonInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			uint num = 0u;
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num > (uint)this.szName_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResDT_LevelCommonInfo.LENGTH_szName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szName_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szName_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szName_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szName_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
			if (num3 > (uint)this.szDesignFileName_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResDT_LevelCommonInfo.LENGTH_szDesignFileName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szDesignFileName_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szDesignFileName_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szDesignFileName_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szDesignFileName_ByteArray) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
			if (num5 > (uint)this.szArtistFileName_ByteArray.GetLength(0))
			{
				if ((ulong)num5 > (ulong)ResDT_LevelCommonInfo.LENGTH_szArtistFileName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szArtistFileName_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szArtistFileName_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szArtistFileName_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szArtistFileName_ByteArray) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bMaxAcntNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bValidRoomType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsAllowHeroDup);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHeroFormId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iHeroAIType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num7 = 0u;
			errorType = srcBuf.readUInt32(ref num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num7 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num7 > (uint)this.szThumbnailPath_ByteArray.GetLength(0))
			{
				if ((ulong)num7 > (ulong)ResDT_LevelCommonInfo.LENGTH_szThumbnailPath)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szThumbnailPath_ByteArray = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szThumbnailPath_ByteArray, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szThumbnailPath_ByteArray[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szThumbnailPath_ByteArray) + 1;
			if ((ulong)num7 != (ulong)((long)num8))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num9 = 0u;
			errorType = srcBuf.readUInt32(ref num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num9 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num9 > (uint)this.szMapPath_ByteArray.GetLength(0))
			{
				if ((ulong)num9 > (ulong)ResDT_LevelCommonInfo.LENGTH_szMapPath)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szMapPath_ByteArray = new byte[num9];
			}
			if (1u > num9)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szMapPath_ByteArray, (int)num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szMapPath_ByteArray[(int)(num9 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num10 = TdrTypeUtil.cstrlen(this.szMapPath_ByteArray) + 1;
			if ((ulong)num9 != (ulong)((long)num10))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num11 = 0u;
			errorType = srcBuf.readUInt32(ref num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num11 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num11 > (uint)this.szBigMapPath_ByteArray.GetLength(0))
			{
				if ((ulong)num11 > (ulong)ResDT_LevelCommonInfo.LENGTH_szBigMapPath)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szBigMapPath_ByteArray = new byte[num11];
			}
			if (1u > num11)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szBigMapPath_ByteArray, (int)num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szBigMapPath_ByteArray[(int)(num11 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num12 = TdrTypeUtil.cstrlen(this.szBigMapPath_ByteArray) + 1;
			if ((ulong)num11 != (ulong)((long)num12))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readInt32(ref this.iMapWidth);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMapHeight);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBigMapWidth);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBigMapHeight);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readFloat(ref this.fMapFowScale);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readFloat(ref this.fBigMapFowScale);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSoulID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsOpenExpCompensate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astExpCompensateDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readInt32(ref this.iExtraSkillId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iExtraSkill2Id);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iExtraPassiveSkillId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bFinResultType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bRandPickHero);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAddWinCondStarId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAddLoseCondStarId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTimeDuration);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSoulAllocId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCameraFlip);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSoldierActivateCountDelay1);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSoldierActivateCountDelay2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSoldierActivateDelay);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bBattleEquipLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bBirthLevelConfig);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bChaosPickRule);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bHeadPtsUpperLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSrvLeastDestoryTowerNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSrvLeastDestoryBaseNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSrvLeastKillCntNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bDealHangUp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwUnLockCondID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowHonor);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPickRuleInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCooldownReduceUpperLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iPvpDifficulty);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwChatID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stUnUseSkillInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAttackOrderID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDynamicPropertyCfg);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wOriginalGoldCoinInBattle);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBattleTaskOfCamp1);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBattleTaskOfCamp2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num13 = 0u;
			errorType = srcBuf.readUInt32(ref num13);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num13 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num13 > (uint)this.szMusicStartEvent_ByteArray.GetLength(0))
			{
				if ((ulong)num13 > (ulong)ResDT_LevelCommonInfo.LENGTH_szMusicStartEvent)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szMusicStartEvent_ByteArray = new byte[num13];
			}
			if (1u > num13)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szMusicStartEvent_ByteArray, (int)num13);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szMusicStartEvent_ByteArray[(int)(num13 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num14 = TdrTypeUtil.cstrlen(this.szMusicStartEvent_ByteArray) + 1;
			if ((ulong)num13 != (ulong)((long)num14))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num15 = 0u;
			errorType = srcBuf.readUInt32(ref num15);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num15 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num15 > (uint)this.szMusicEndEvent_ByteArray.GetLength(0))
			{
				if ((ulong)num15 > (ulong)ResDT_LevelCommonInfo.LENGTH_szMusicEndEvent)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szMusicEndEvent_ByteArray = new byte[num15];
			}
			if (1u > num15)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szMusicEndEvent_ByteArray, (int)num15);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szMusicEndEvent_ByteArray[(int)(num15 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num16 = TdrTypeUtil.cstrlen(this.szMusicEndEvent_ByteArray) + 1;
			if ((ulong)num15 != (ulong)((long)num16))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num17 = 0u;
			errorType = srcBuf.readUInt32(ref num17);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num17 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num17 > (uint)this.szAmbientSoundEvent_ByteArray.GetLength(0))
			{
				if ((ulong)num17 > (ulong)ResDT_LevelCommonInfo.LENGTH_szAmbientSoundEvent)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szAmbientSoundEvent_ByteArray = new byte[num17];
			}
			if (1u > num17)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szAmbientSoundEvent_ByteArray, (int)num17);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szAmbientSoundEvent_ByteArray[(int)(num17 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num18 = TdrTypeUtil.cstrlen(this.szAmbientSoundEvent_ByteArray) + 1;
			if ((ulong)num17 != (ulong)((long)num18))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num19 = 0u;
			errorType = srcBuf.readUInt32(ref num19);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num19 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num19 > (uint)this.szBankResourceName_ByteArray.GetLength(0))
			{
				if ((ulong)num19 > (ulong)ResDT_LevelCommonInfo.LENGTH_szBankResourceName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szBankResourceName_ByteArray = new byte[num19];
			}
			if (1u > num19)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szBankResourceName_ByteArray, (int)num19);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szBankResourceName_ByteArray[(int)(num19 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num20 = TdrTypeUtil.cstrlen(this.szBankResourceName_ByteArray) + 1;
			if ((ulong)num19 != (ulong)((long)num20))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bSupportCameraDrag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num21 = 0u;
			errorType = srcBuf.readUInt32(ref num21);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num21 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num21 > (uint)this.szGameMatchName_ByteArray.GetLength(0))
			{
				if ((ulong)num21 > (ulong)ResDT_LevelCommonInfo.LENGTH_szGameMatchName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGameMatchName_ByteArray = new byte[num21];
			}
			if (1u > num21)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGameMatchName_ByteArray, (int)num21);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGameMatchName_ByteArray[(int)(num21 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num22 = TdrTypeUtil.cstrlen(this.szGameMatchName_ByteArray) + 1;
			if ((ulong)num21 != (ulong)((long)num22))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bIsEnableFow);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsEnableShopHorizonTab);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsEnableOrnamentSlot);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iOrnamentSkillId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iOrnamentSwitchCD);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iOrnamentFirstSwitchCD);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iOrnamentFirstSwitchCDEftTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwJudgeNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bPauseNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwWealGameSubType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSupportHighTowerSoldier);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowTeamHint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowWinRatio);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bForceTimeoutPick);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iFakeSightRange);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}

		public TdrError.ErrorType load(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref buffer, size);
			TdrError.ErrorType result = this.load(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			return result;
		}

		public TdrError.ErrorType load(ref TdrReadBuf srcBuf, uint cutVer)
		{
			srcBuf.disableEndian();
			if (cutVer == 0u || ResDT_LevelCommonInfo.CURRVERSION < cutVer)
			{
				cutVer = ResDT_LevelCommonInfo.CURRVERSION;
			}
			if (ResDT_LevelCommonInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			int num = 32;
			if (this.szName_ByteArray.GetLength(0) < num)
			{
				this.szName_ByteArray = new byte[ResDT_LevelCommonInfo.LENGTH_szName];
			}
			TdrError.ErrorType errorType = srcBuf.readCString(ref this.szName_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 32;
			if (this.szDesignFileName_ByteArray.GetLength(0) < num2)
			{
				this.szDesignFileName_ByteArray = new byte[ResDT_LevelCommonInfo.LENGTH_szDesignFileName];
			}
			errorType = srcBuf.readCString(ref this.szDesignFileName_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 32;
			if (this.szArtistFileName_ByteArray.GetLength(0) < num3)
			{
				this.szArtistFileName_ByteArray = new byte[ResDT_LevelCommonInfo.LENGTH_szArtistFileName];
			}
			errorType = srcBuf.readCString(ref this.szArtistFileName_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bMaxAcntNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bValidRoomType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsAllowHeroDup);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHeroFormId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iHeroAIType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num4 = 128;
			if (this.szThumbnailPath_ByteArray.GetLength(0) < num4)
			{
				this.szThumbnailPath_ByteArray = new byte[ResDT_LevelCommonInfo.LENGTH_szThumbnailPath];
			}
			errorType = srcBuf.readCString(ref this.szThumbnailPath_ByteArray, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num5 = 128;
			if (this.szMapPath_ByteArray.GetLength(0) < num5)
			{
				this.szMapPath_ByteArray = new byte[ResDT_LevelCommonInfo.LENGTH_szMapPath];
			}
			errorType = srcBuf.readCString(ref this.szMapPath_ByteArray, num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num6 = 128;
			if (this.szBigMapPath_ByteArray.GetLength(0) < num6)
			{
				this.szBigMapPath_ByteArray = new byte[ResDT_LevelCommonInfo.LENGTH_szBigMapPath];
			}
			errorType = srcBuf.readCString(ref this.szBigMapPath_ByteArray, num6);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMapWidth);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMapHeight);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBigMapWidth);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBigMapHeight);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readFloat(ref this.fMapFowScale);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readFloat(ref this.fBigMapFowScale);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSoulID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsOpenExpCompensate);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astExpCompensateDetail[i].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readInt32(ref this.iExtraSkillId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iExtraSkill2Id);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iExtraPassiveSkillId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bFinResultType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bRandPickHero);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAddWinCondStarId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAddLoseCondStarId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTimeDuration);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSoulAllocId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCameraFlip);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSoldierActivateCountDelay1);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSoldierActivateCountDelay2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSoldierActivateDelay);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bBattleEquipLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bBirthLevelConfig);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bChaosPickRule);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bHeadPtsUpperLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSrvLeastDestoryTowerNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSrvLeastDestoryBaseNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSrvLeastKillCntNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bDealHangUp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwUnLockCondID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowHonor);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPickRuleInfo.load(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCooldownReduceUpperLimit);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iPvpDifficulty);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwChatID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stUnUseSkillInfo.load(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAttackOrderID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDynamicPropertyCfg);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wOriginalGoldCoinInBattle);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBattleTaskOfCamp1);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBattleTaskOfCamp2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num7 = 32;
			if (this.szMusicStartEvent_ByteArray.GetLength(0) < num7)
			{
				this.szMusicStartEvent_ByteArray = new byte[ResDT_LevelCommonInfo.LENGTH_szMusicStartEvent];
			}
			errorType = srcBuf.readCString(ref this.szMusicStartEvent_ByteArray, num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num8 = 32;
			if (this.szMusicEndEvent_ByteArray.GetLength(0) < num8)
			{
				this.szMusicEndEvent_ByteArray = new byte[ResDT_LevelCommonInfo.LENGTH_szMusicEndEvent];
			}
			errorType = srcBuf.readCString(ref this.szMusicEndEvent_ByteArray, num8);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num9 = 32;
			if (this.szAmbientSoundEvent_ByteArray.GetLength(0) < num9)
			{
				this.szAmbientSoundEvent_ByteArray = new byte[ResDT_LevelCommonInfo.LENGTH_szAmbientSoundEvent];
			}
			errorType = srcBuf.readCString(ref this.szAmbientSoundEvent_ByteArray, num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num10 = 32;
			if (this.szBankResourceName_ByteArray.GetLength(0) < num10)
			{
				this.szBankResourceName_ByteArray = new byte[ResDT_LevelCommonInfo.LENGTH_szBankResourceName];
			}
			errorType = srcBuf.readCString(ref this.szBankResourceName_ByteArray, num10);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSupportCameraDrag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num11 = 256;
			if (this.szGameMatchName_ByteArray.GetLength(0) < num11)
			{
				this.szGameMatchName_ByteArray = new byte[ResDT_LevelCommonInfo.LENGTH_szGameMatchName];
			}
			errorType = srcBuf.readCString(ref this.szGameMatchName_ByteArray, num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsEnableFow);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsEnableShopHorizonTab);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsEnableOrnamentSlot);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iOrnamentSkillId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iOrnamentSwitchCD);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iOrnamentFirstSwitchCD);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iOrnamentFirstSwitchCDEftTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwJudgeNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bPauseNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwWealGameSubType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSupportHighTowerSoldier);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowTeamHint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowWinRatio);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bForceTimeoutPick);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iFakeSightRange);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}
