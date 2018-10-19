using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResLevelCfgInfo : IUnpackable, tsf4g_csharp_interface
	{
		public int iCfgID;

		public int iChapterId;

		public byte bLevelNo;

		public byte bLevelDifficulty;

		public uint dwChallengeNum;

		public byte bMaxAcntNum;

		public byte[] szName_ByteArray;

		public byte[] szDesignFileName_ByteArray;

		public byte[] szArtistFileName_ByteArray;

		public int iLevelType;

		public byte[] szLevelIcon_ByteArray;

		public uint dwSelfCampAIPlayerLevel;

		public uint[] SelfCampAIHeroID;

		public uint dwAIPlayerLevel;

		public uint[] AIHeroID;

		public int iHeroNum;

		public int iHeroAIType;

		public ResDT_IntParamArrayNode[] astStarDetail;

		public int iLoseCondition;

		public uint dwDefaultActive;

		public int iActivateLevelId;

		public ResDT_PveRewardShowInfo[] astRewardShowDetail;

		public int[] RecommendLevel;

		public int[] RecommendPower;

		public int[] ServerCheckPower;

		public byte bHideMinimap;

		public byte[] szThumbnailPath_ByteArray;

		public byte[] szMapPath_ByteArray;

		public byte[] szBigMapPath_ByteArray;

		public int iMapWidth;

		public int iMapHeight;

		public int iBigMapWidth;

		public int iBigMapHeight;

		public int iPassDialogId;

		public int iPreDialogId;

		public int iFailureDialogId;

		public uint dwEnterConsumeAP;

		public uint dwFinishConsumeAP;

		public uint dwBattleListID;

		public uint[] SettleIDDetail;

		public byte bSoulGrow;

		public uint dwSoulID;

		public uint dwAttackOrderID;

		public uint dwReviveTime;

		public uint dwDynamicPropertyCfg;

		public byte[] szMusicStartEvent_ByteArray;

		public byte[] szMusicEndEvent_ByteArray;

		public byte[] szAmbientSoundEvent_ByteArray;

		public byte[] szBankResourceName_ByteArray;

		public byte bEnableHorizon;

		public byte bIsOpenAutoAI;

		public ResDT_MapBuff[] astMapBuffs;

		public byte[] szLevelDesc_ByteArray;

		public ResDT_PveReviveInfo[] astReviveInfo;

		public byte bReviveTimeMax;

		public int iExtraSkillId;

		public int iExtraSkill2Id;

		public int iExtraPassiveSkillId;

		public byte bFinResultType;

		public byte bRandPickHero;

		public uint dwSoulAllocId;

		public byte bShowTrainingHelper;

		public byte bGuideLevelSubType;

		public byte bSupportCameraDrag;

		public string szName;

		public string szDesignFileName;

		public string szArtistFileName;

		public string szLevelIcon;

		public string szThumbnailPath;

		public string szMapPath;

		public string szBigMapPath;

		public string szMusicStartEvent;

		public string szMusicEndEvent;

		public string szAmbientSoundEvent;

		public string szBankResourceName;

		public string szLevelDesc;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szName = 32u;

		public static readonly uint LENGTH_szDesignFileName = 32u;

		public static readonly uint LENGTH_szArtistFileName = 32u;

		public static readonly uint LENGTH_szLevelIcon = 128u;

		public static readonly uint LENGTH_szThumbnailPath = 128u;

		public static readonly uint LENGTH_szMapPath = 128u;

		public static readonly uint LENGTH_szBigMapPath = 128u;

		public static readonly uint LENGTH_szMusicStartEvent = 32u;

		public static readonly uint LENGTH_szMusicEndEvent = 32u;

		public static readonly uint LENGTH_szAmbientSoundEvent = 32u;

		public static readonly uint LENGTH_szBankResourceName = 32u;

		public static readonly uint LENGTH_szLevelDesc = 150u;

		public ResLevelCfgInfo()
		{
			this.szName_ByteArray = new byte[1];
			this.szDesignFileName_ByteArray = new byte[1];
			this.szArtistFileName_ByteArray = new byte[1];
			this.szLevelIcon_ByteArray = new byte[1];
			this.SelfCampAIHeroID = new uint[5];
			this.AIHeroID = new uint[5];
			this.astStarDetail = new ResDT_IntParamArrayNode[3];
			for (int i = 0; i < 3; i++)
			{
				this.astStarDetail[i] = new ResDT_IntParamArrayNode();
			}
			this.astRewardShowDetail = new ResDT_PveRewardShowInfo[5];
			for (int j = 0; j < 5; j++)
			{
				this.astRewardShowDetail[j] = new ResDT_PveRewardShowInfo();
			}
			this.RecommendLevel = new int[4];
			this.RecommendPower = new int[4];
			this.ServerCheckPower = new int[4];
			this.szThumbnailPath_ByteArray = new byte[1];
			this.szMapPath_ByteArray = new byte[1];
			this.szBigMapPath_ByteArray = new byte[1];
			this.SettleIDDetail = new uint[4];
			this.szMusicStartEvent_ByteArray = new byte[1];
			this.szMusicEndEvent_ByteArray = new byte[1];
			this.szAmbientSoundEvent_ByteArray = new byte[1];
			this.szBankResourceName_ByteArray = new byte[1];
			this.astMapBuffs = new ResDT_MapBuff[4];
			for (int k = 0; k < 4; k++)
			{
				this.astMapBuffs[k] = new ResDT_MapBuff();
			}
			this.szLevelDesc_ByteArray = new byte[1];
			this.astReviveInfo = new ResDT_PveReviveInfo[4];
			for (int l = 0; l < 4; l++)
			{
				this.astReviveInfo[l] = new ResDT_PveReviveInfo();
			}
			this.szName = string.Empty;
			this.szDesignFileName = string.Empty;
			this.szArtistFileName = string.Empty;
			this.szLevelIcon = string.Empty;
			this.szThumbnailPath = string.Empty;
			this.szMapPath = string.Empty;
			this.szBigMapPath = string.Empty;
			this.szMusicStartEvent = string.Empty;
			this.szMusicEndEvent = string.Empty;
			this.szAmbientSoundEvent = string.Empty;
			this.szBankResourceName = string.Empty;
			this.szLevelDesc = string.Empty;
		}

		private void TransferData()
		{
			this.szName = StringHelper.UTF8BytesToString(ref this.szName_ByteArray);
			this.szName_ByteArray = null;
			this.szDesignFileName = StringHelper.UTF8BytesToString(ref this.szDesignFileName_ByteArray);
			this.szDesignFileName_ByteArray = null;
			this.szArtistFileName = StringHelper.UTF8BytesToString(ref this.szArtistFileName_ByteArray);
			this.szArtistFileName_ByteArray = null;
			this.szLevelIcon = StringHelper.UTF8BytesToString(ref this.szLevelIcon_ByteArray);
			this.szLevelIcon_ByteArray = null;
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
			this.szLevelDesc = StringHelper.UTF8BytesToString(ref this.szLevelDesc_ByteArray);
			this.szLevelDesc_ByteArray = null;
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
			if (cutVer == 0u || ResLevelCfgInfo.CURRVERSION < cutVer)
			{
				cutVer = ResLevelCfgInfo.CURRVERSION;
			}
			if (ResLevelCfgInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iCfgID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iChapterId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bLevelNo);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bLevelDifficulty);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwChallengeNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bMaxAcntNum);
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
			if (num > (uint)this.szName_ByteArray.GetLength(0))
			{
				if ((long)num > (long)((ulong)ResLevelCfgInfo.LENGTH_szName))
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
				if ((long)num3 > (long)((ulong)ResLevelCfgInfo.LENGTH_szDesignFileName))
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
				if ((long)num5 > (long)((ulong)ResLevelCfgInfo.LENGTH_szArtistFileName))
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
			errorType = srcBuf.readInt32(ref this.iLevelType);
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
			if (num7 > (uint)this.szLevelIcon_ByteArray.GetLength(0))
			{
				if ((long)num7 > (long)((ulong)ResLevelCfgInfo.LENGTH_szLevelIcon))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szLevelIcon_ByteArray = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szLevelIcon_ByteArray, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szLevelIcon_ByteArray[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szLevelIcon_ByteArray) + 1;
			if ((ulong)num7 != (ulong)((long)num8))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwSelfCampAIPlayerLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = srcBuf.readUInt32(ref this.SelfCampAIHeroID[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwAIPlayerLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int j = 0; j < 5; j++)
			{
				errorType = srcBuf.readUInt32(ref this.AIHeroID[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readInt32(ref this.iHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iHeroAIType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int k = 0; k < 3; k++)
			{
				errorType = this.astStarDetail[k].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readInt32(ref this.iLoseCondition);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDefaultActive);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iActivateLevelId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int l = 0; l < 5; l++)
			{
				errorType = this.astRewardShowDetail[l].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int m = 0; m < 4; m++)
			{
				errorType = srcBuf.readInt32(ref this.RecommendLevel[m]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int n = 0; n < 4; n++)
			{
				errorType = srcBuf.readInt32(ref this.RecommendPower[n]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int num9 = 0; num9 < 4; num9++)
			{
				errorType = srcBuf.readInt32(ref this.ServerCheckPower[num9]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bHideMinimap);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num10 = 0u;
			errorType = srcBuf.readUInt32(ref num10);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num10 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num10 > (uint)this.szThumbnailPath_ByteArray.GetLength(0))
			{
				if ((long)num10 > (long)((ulong)ResLevelCfgInfo.LENGTH_szThumbnailPath))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szThumbnailPath_ByteArray = new byte[num10];
			}
			if (1u > num10)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szThumbnailPath_ByteArray, (int)num10);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szThumbnailPath_ByteArray[(int)(num10 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num11 = TdrTypeUtil.cstrlen(this.szThumbnailPath_ByteArray) + 1;
			if ((ulong)num10 != (ulong)((long)num11))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num12 = 0u;
			errorType = srcBuf.readUInt32(ref num12);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num12 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num12 > (uint)this.szMapPath_ByteArray.GetLength(0))
			{
				if ((long)num12 > (long)((ulong)ResLevelCfgInfo.LENGTH_szMapPath))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szMapPath_ByteArray = new byte[num12];
			}
			if (1u > num12)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szMapPath_ByteArray, (int)num12);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szMapPath_ByteArray[(int)(num12 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num13 = TdrTypeUtil.cstrlen(this.szMapPath_ByteArray) + 1;
			if ((ulong)num12 != (ulong)((long)num13))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num14 = 0u;
			errorType = srcBuf.readUInt32(ref num14);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num14 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num14 > (uint)this.szBigMapPath_ByteArray.GetLength(0))
			{
				if ((long)num14 > (long)((ulong)ResLevelCfgInfo.LENGTH_szBigMapPath))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szBigMapPath_ByteArray = new byte[num14];
			}
			if (1u > num14)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szBigMapPath_ByteArray, (int)num14);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szBigMapPath_ByteArray[(int)(num14 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num15 = TdrTypeUtil.cstrlen(this.szBigMapPath_ByteArray) + 1;
			if ((ulong)num14 != (ulong)((long)num15))
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
			errorType = srcBuf.readInt32(ref this.iPassDialogId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iPreDialogId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iFailureDialogId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwEnterConsumeAP);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwFinishConsumeAP);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBattleListID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int num16 = 0; num16 < 4; num16++)
			{
				errorType = srcBuf.readUInt32(ref this.SettleIDDetail[num16]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bSoulGrow);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSoulID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAttackOrderID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwReviveTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDynamicPropertyCfg);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (num17 > (uint)this.szMusicStartEvent_ByteArray.GetLength(0))
			{
				if ((long)num17 > (long)((ulong)ResLevelCfgInfo.LENGTH_szMusicStartEvent))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szMusicStartEvent_ByteArray = new byte[num17];
			}
			if (1u > num17)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szMusicStartEvent_ByteArray, (int)num17);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szMusicStartEvent_ByteArray[(int)(num17 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num18 = TdrTypeUtil.cstrlen(this.szMusicStartEvent_ByteArray) + 1;
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
			if (num19 > (uint)this.szMusicEndEvent_ByteArray.GetLength(0))
			{
				if ((long)num19 > (long)((ulong)ResLevelCfgInfo.LENGTH_szMusicEndEvent))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szMusicEndEvent_ByteArray = new byte[num19];
			}
			if (1u > num19)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szMusicEndEvent_ByteArray, (int)num19);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szMusicEndEvent_ByteArray[(int)(num19 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num20 = TdrTypeUtil.cstrlen(this.szMusicEndEvent_ByteArray) + 1;
			if ((ulong)num19 != (ulong)((long)num20))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
			if (num21 > (uint)this.szAmbientSoundEvent_ByteArray.GetLength(0))
			{
				if ((long)num21 > (long)((ulong)ResLevelCfgInfo.LENGTH_szAmbientSoundEvent))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szAmbientSoundEvent_ByteArray = new byte[num21];
			}
			if (1u > num21)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szAmbientSoundEvent_ByteArray, (int)num21);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szAmbientSoundEvent_ByteArray[(int)(num21 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num22 = TdrTypeUtil.cstrlen(this.szAmbientSoundEvent_ByteArray) + 1;
			if ((ulong)num21 != (ulong)((long)num22))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num23 = 0u;
			errorType = srcBuf.readUInt32(ref num23);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num23 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num23 > (uint)this.szBankResourceName_ByteArray.GetLength(0))
			{
				if ((long)num23 > (long)((ulong)ResLevelCfgInfo.LENGTH_szBankResourceName))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szBankResourceName_ByteArray = new byte[num23];
			}
			if (1u > num23)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szBankResourceName_ByteArray, (int)num23);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szBankResourceName_ByteArray[(int)(num23 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num24 = TdrTypeUtil.cstrlen(this.szBankResourceName_ByteArray) + 1;
			if ((ulong)num23 != (ulong)((long)num24))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bEnableHorizon);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsOpenAutoAI);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int num25 = 0; num25 < 4; num25++)
			{
				errorType = this.astMapBuffs[num25].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			uint num26 = 0u;
			errorType = srcBuf.readUInt32(ref num26);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num26 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num26 > (uint)this.szLevelDesc_ByteArray.GetLength(0))
			{
				if ((long)num26 > (long)((ulong)ResLevelCfgInfo.LENGTH_szLevelDesc))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szLevelDesc_ByteArray = new byte[num26];
			}
			if (1u > num26)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szLevelDesc_ByteArray, (int)num26);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szLevelDesc_ByteArray[(int)(num26 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num27 = TdrTypeUtil.cstrlen(this.szLevelDesc_ByteArray) + 1;
			if ((ulong)num26 != (ulong)((long)num27))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			for (int num28 = 0; num28 < 4; num28++)
			{
				errorType = this.astReviveInfo[num28].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bReviveTimeMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			errorType = srcBuf.readUInt32(ref this.dwSoulAllocId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowTrainingHelper);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bGuideLevelSubType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSupportCameraDrag);
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
			if (cutVer == 0u || ResLevelCfgInfo.CURRVERSION < cutVer)
			{
				cutVer = ResLevelCfgInfo.CURRVERSION;
			}
			if (ResLevelCfgInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iCfgID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iChapterId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bLevelNo);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bLevelDifficulty);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwChallengeNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bMaxAcntNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 32;
			if (this.szName_ByteArray.GetLength(0) < num)
			{
				this.szName_ByteArray = new byte[ResLevelCfgInfo.LENGTH_szName];
			}
			errorType = srcBuf.readCString(ref this.szName_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 32;
			if (this.szDesignFileName_ByteArray.GetLength(0) < num2)
			{
				this.szDesignFileName_ByteArray = new byte[ResLevelCfgInfo.LENGTH_szDesignFileName];
			}
			errorType = srcBuf.readCString(ref this.szDesignFileName_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 32;
			if (this.szArtistFileName_ByteArray.GetLength(0) < num3)
			{
				this.szArtistFileName_ByteArray = new byte[ResLevelCfgInfo.LENGTH_szArtistFileName];
			}
			errorType = srcBuf.readCString(ref this.szArtistFileName_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iLevelType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num4 = 128;
			if (this.szLevelIcon_ByteArray.GetLength(0) < num4)
			{
				this.szLevelIcon_ByteArray = new byte[ResLevelCfgInfo.LENGTH_szLevelIcon];
			}
			errorType = srcBuf.readCString(ref this.szLevelIcon_ByteArray, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSelfCampAIPlayerLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = srcBuf.readUInt32(ref this.SelfCampAIHeroID[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwAIPlayerLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int j = 0; j < 5; j++)
			{
				errorType = srcBuf.readUInt32(ref this.AIHeroID[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readInt32(ref this.iHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iHeroAIType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int k = 0; k < 3; k++)
			{
				errorType = this.astStarDetail[k].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readInt32(ref this.iLoseCondition);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDefaultActive);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iActivateLevelId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int l = 0; l < 5; l++)
			{
				errorType = this.astRewardShowDetail[l].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int m = 0; m < 4; m++)
			{
				errorType = srcBuf.readInt32(ref this.RecommendLevel[m]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int n = 0; n < 4; n++)
			{
				errorType = srcBuf.readInt32(ref this.RecommendPower[n]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int num5 = 0; num5 < 4; num5++)
			{
				errorType = srcBuf.readInt32(ref this.ServerCheckPower[num5]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bHideMinimap);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num6 = 128;
			if (this.szThumbnailPath_ByteArray.GetLength(0) < num6)
			{
				this.szThumbnailPath_ByteArray = new byte[ResLevelCfgInfo.LENGTH_szThumbnailPath];
			}
			errorType = srcBuf.readCString(ref this.szThumbnailPath_ByteArray, num6);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num7 = 128;
			if (this.szMapPath_ByteArray.GetLength(0) < num7)
			{
				this.szMapPath_ByteArray = new byte[ResLevelCfgInfo.LENGTH_szMapPath];
			}
			errorType = srcBuf.readCString(ref this.szMapPath_ByteArray, num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num8 = 128;
			if (this.szBigMapPath_ByteArray.GetLength(0) < num8)
			{
				this.szBigMapPath_ByteArray = new byte[ResLevelCfgInfo.LENGTH_szBigMapPath];
			}
			errorType = srcBuf.readCString(ref this.szBigMapPath_ByteArray, num8);
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
			errorType = srcBuf.readInt32(ref this.iPassDialogId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iPreDialogId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iFailureDialogId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwEnterConsumeAP);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwFinishConsumeAP);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwBattleListID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int num9 = 0; num9 < 4; num9++)
			{
				errorType = srcBuf.readUInt32(ref this.SettleIDDetail[num9]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bSoulGrow);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSoulID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAttackOrderID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwReviveTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDynamicPropertyCfg);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num10 = 32;
			if (this.szMusicStartEvent_ByteArray.GetLength(0) < num10)
			{
				this.szMusicStartEvent_ByteArray = new byte[ResLevelCfgInfo.LENGTH_szMusicStartEvent];
			}
			errorType = srcBuf.readCString(ref this.szMusicStartEvent_ByteArray, num10);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num11 = 32;
			if (this.szMusicEndEvent_ByteArray.GetLength(0) < num11)
			{
				this.szMusicEndEvent_ByteArray = new byte[ResLevelCfgInfo.LENGTH_szMusicEndEvent];
			}
			errorType = srcBuf.readCString(ref this.szMusicEndEvent_ByteArray, num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num12 = 32;
			if (this.szAmbientSoundEvent_ByteArray.GetLength(0) < num12)
			{
				this.szAmbientSoundEvent_ByteArray = new byte[ResLevelCfgInfo.LENGTH_szAmbientSoundEvent];
			}
			errorType = srcBuf.readCString(ref this.szAmbientSoundEvent_ByteArray, num12);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num13 = 32;
			if (this.szBankResourceName_ByteArray.GetLength(0) < num13)
			{
				this.szBankResourceName_ByteArray = new byte[ResLevelCfgInfo.LENGTH_szBankResourceName];
			}
			errorType = srcBuf.readCString(ref this.szBankResourceName_ByteArray, num13);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bEnableHorizon);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsOpenAutoAI);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int num14 = 0; num14 < 4; num14++)
			{
				errorType = this.astMapBuffs[num14].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			int num15 = 150;
			if (this.szLevelDesc_ByteArray.GetLength(0) < num15)
			{
				this.szLevelDesc_ByteArray = new byte[ResLevelCfgInfo.LENGTH_szLevelDesc];
			}
			errorType = srcBuf.readCString(ref this.szLevelDesc_ByteArray, num15);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int num16 = 0; num16 < 4; num16++)
			{
				errorType = this.astReviveInfo[num16].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bReviveTimeMax);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			errorType = srcBuf.readUInt32(ref this.dwSoulAllocId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bShowTrainingHelper);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bGuideLevelSubType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSupportCameraDrag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}
