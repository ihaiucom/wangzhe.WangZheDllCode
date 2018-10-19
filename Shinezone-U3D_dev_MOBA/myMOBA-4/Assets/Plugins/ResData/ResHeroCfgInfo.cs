using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResHeroCfgInfo : IUnpackable, tsf4g_csharp_interface
	{
		public uint dwCfgID;

		public byte[] szName_ByteArray;

		public byte[] szNamePinYin_ByteArray;

		public byte bIsTrainUse;

		public byte[] szHeroTitle_ByteArray;

		public byte[] szStoryUrl_ByteArray;

		public byte[] szImagePath_ByteArray;

		public byte[] szCharacterInfo_ByteArray;

		public int iRecommendPosition;

		public byte bAttackDistanceType;

		public int iSightR;

		public CrypticInteger iBaseHP;

		public CrypticInteger iBaseHPAdd;

		public CrypticInteger iHPAddLvlup;

		public CrypticInteger iBaseATT;

		public CrypticInteger iBaseINT;

		public CrypticInteger iBaseDEF;

		public CrypticInteger iBaseRES;

		public CrypticInteger iBaseSpeed;

		public CrypticInteger iAtkSpdAddLvlup;

		public CrypticInteger iBaseAtkSpd;

		public CrypticInteger iCritRate;

		public CrypticInteger iCritEft;

		public CrypticInteger iHpGrowth;

		public CrypticInteger iAtkGrowth;

		public CrypticInteger iSpellGrowth;

		public CrypticInteger iDefGrowth;

		public CrypticInteger iResistGrowth;

		public int iPassiveID1;

		public int iPassiveID2;

		public ResDT_SkillInfo[] astSkill;

		public int iInitialStar;

		public byte bType;

		public byte bExpandType;

		public ushort wPVPNeedLevel;

		public int iPVPNeedQuality;

		public int iPVPNeedSubQuality;

		public int iPVPNeedStar;

		public CrypticInteger iViability;

		public CrypticInteger iPhyDamage;

		public CrypticInteger iSpellDamage;

		public int iStartedDifficulty;

		public byte bMainJob;

		public byte bMinorJob;

		public int[] JobFeature;

		public byte bDamageType;

		public byte bAttackType;

		public byte[] szHeroDesc_ByteArray;

		public byte bIOSHide;

		public uint dwShowSortId;

		public byte[] szAI_Entry_ByteArray;

		public byte[] szAI_Simple_ByteArray;

		public byte[] szAI_Normal_ByteArray;

		public byte[] szAI_Hard_ByteArray;

		public byte[] szAI_WarmSimple_ByteArray;

		public byte[] szAI_Warm_ByteArray;

		public byte[] szWakeDesc_ByteArray;

		public uint dwWakeTalentID;

		public uint dwWakeSkinID;

		public byte[] szAttackRangeDesc_ByteArray;

		public uint dwEnergyType;

		public CrypticInteger iEnergy;

		public CrypticInteger iEnergyGrowth;

		public CrypticInteger iEnergyRec;

		public CrypticInteger iEnergyRecGrowth;

		public byte[] szHeroTips_ByteArray;

		public byte[] szHeroSound_ByteArray;

		public uint dwSymbolRcmdID;

		public byte[] szBorn_Age_ByteArray;

		public byte[] szRevive_Age_ByteArray;

		public uint dwDeadControl;

		public byte bTag;

		public string szName;

		public string szNamePinYin;

		public string szHeroTitle;

		public string szStoryUrl;

		public string szImagePath;

		public string szCharacterInfo;

		public string szHeroDesc;

		public string szAI_Entry;

		public string szAI_Simple;

		public string szAI_Normal;

		public string szAI_Hard;

		public string szAI_WarmSimple;

		public string szAI_Warm;

		public string szWakeDesc;

		public string szAttackRangeDesc;

		public string szHeroTips;

		public string szHeroSound;

		public string szBorn_Age;

		public string szRevive_Age;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szName = 32u;

		public static readonly uint LENGTH_szNamePinYin = 32u;

		public static readonly uint LENGTH_szHeroTitle = 128u;

		public static readonly uint LENGTH_szStoryUrl = 256u;

		public static readonly uint LENGTH_szImagePath = 128u;

		public static readonly uint LENGTH_szCharacterInfo = 128u;

		public static readonly uint LENGTH_szHeroDesc = 256u;

		public static readonly uint LENGTH_szAI_Entry = 128u;

		public static readonly uint LENGTH_szAI_Simple = 128u;

		public static readonly uint LENGTH_szAI_Normal = 128u;

		public static readonly uint LENGTH_szAI_Hard = 128u;

		public static readonly uint LENGTH_szAI_WarmSimple = 128u;

		public static readonly uint LENGTH_szAI_Warm = 128u;

		public static readonly uint LENGTH_szWakeDesc = 512u;

		public static readonly uint LENGTH_szAttackRangeDesc = 16u;

		public static readonly uint LENGTH_szHeroTips = 4096u;

		public static readonly uint LENGTH_szHeroSound = 64u;

		public static readonly uint LENGTH_szBorn_Age = 128u;

		public static readonly uint LENGTH_szRevive_Age = 128u;

		public ResHeroCfgInfo()
		{
			this.szName_ByteArray = new byte[1];
			this.szNamePinYin_ByteArray = new byte[1];
			this.szHeroTitle_ByteArray = new byte[1];
			this.szStoryUrl_ByteArray = new byte[1];
			this.szImagePath_ByteArray = new byte[1];
			this.szCharacterInfo_ByteArray = new byte[1];
			this.astSkill = new ResDT_SkillInfo[5];
			for (int i = 0; i < 5; i++)
			{
				this.astSkill[i] = new ResDT_SkillInfo();
			}
			this.JobFeature = new int[2];
			this.szHeroDesc_ByteArray = new byte[1];
			this.szAI_Entry_ByteArray = new byte[1];
			this.szAI_Simple_ByteArray = new byte[1];
			this.szAI_Normal_ByteArray = new byte[1];
			this.szAI_Hard_ByteArray = new byte[1];
			this.szAI_WarmSimple_ByteArray = new byte[1];
			this.szAI_Warm_ByteArray = new byte[1];
			this.szWakeDesc_ByteArray = new byte[1];
			this.szAttackRangeDesc_ByteArray = new byte[1];
			this.szHeroTips_ByteArray = new byte[1];
			this.szHeroSound_ByteArray = new byte[1];
			this.szBorn_Age_ByteArray = new byte[1];
			this.szRevive_Age_ByteArray = new byte[1];
			this.szName = string.Empty;
			this.szNamePinYin = string.Empty;
			this.szHeroTitle = string.Empty;
			this.szStoryUrl = string.Empty;
			this.szImagePath = string.Empty;
			this.szCharacterInfo = string.Empty;
			this.szHeroDesc = string.Empty;
			this.szAI_Entry = string.Empty;
			this.szAI_Simple = string.Empty;
			this.szAI_Normal = string.Empty;
			this.szAI_Hard = string.Empty;
			this.szAI_WarmSimple = string.Empty;
			this.szAI_Warm = string.Empty;
			this.szWakeDesc = string.Empty;
			this.szAttackRangeDesc = string.Empty;
			this.szHeroTips = string.Empty;
			this.szHeroSound = string.Empty;
			this.szBorn_Age = string.Empty;
			this.szRevive_Age = string.Empty;
		}

		private void TransferData()
		{
			this.szName = StringHelper.UTF8BytesToString(ref this.szName_ByteArray);
			this.szName_ByteArray = null;
			this.szNamePinYin = StringHelper.UTF8BytesToString(ref this.szNamePinYin_ByteArray);
			this.szNamePinYin_ByteArray = null;
			this.szHeroTitle = StringHelper.UTF8BytesToString(ref this.szHeroTitle_ByteArray);
			this.szHeroTitle_ByteArray = null;
			this.szStoryUrl = StringHelper.UTF8BytesToString(ref this.szStoryUrl_ByteArray);
			this.szStoryUrl_ByteArray = null;
			this.szImagePath = StringHelper.UTF8BytesToString(ref this.szImagePath_ByteArray);
			this.szImagePath_ByteArray = null;
			this.szCharacterInfo = StringHelper.UTF8BytesToString(ref this.szCharacterInfo_ByteArray);
			this.szCharacterInfo_ByteArray = null;
			this.szHeroDesc = StringHelper.UTF8BytesToString(ref this.szHeroDesc_ByteArray);
			this.szHeroDesc_ByteArray = null;
			this.szAI_Entry = StringHelper.UTF8BytesToString(ref this.szAI_Entry_ByteArray);
			this.szAI_Entry_ByteArray = null;
			this.szAI_Simple = StringHelper.UTF8BytesToString(ref this.szAI_Simple_ByteArray);
			this.szAI_Simple_ByteArray = null;
			this.szAI_Normal = StringHelper.UTF8BytesToString(ref this.szAI_Normal_ByteArray);
			this.szAI_Normal_ByteArray = null;
			this.szAI_Hard = StringHelper.UTF8BytesToString(ref this.szAI_Hard_ByteArray);
			this.szAI_Hard_ByteArray = null;
			this.szAI_WarmSimple = StringHelper.UTF8BytesToString(ref this.szAI_WarmSimple_ByteArray);
			this.szAI_WarmSimple_ByteArray = null;
			this.szAI_Warm = StringHelper.UTF8BytesToString(ref this.szAI_Warm_ByteArray);
			this.szAI_Warm_ByteArray = null;
			this.szWakeDesc = StringHelper.UTF8BytesToString(ref this.szWakeDesc_ByteArray);
			this.szWakeDesc_ByteArray = null;
			this.szAttackRangeDesc = StringHelper.UTF8BytesToString(ref this.szAttackRangeDesc_ByteArray);
			this.szAttackRangeDesc_ByteArray = null;
			this.szHeroTips = StringHelper.UTF8BytesToString(ref this.szHeroTips_ByteArray);
			this.szHeroTips_ByteArray = null;
			this.szHeroSound = StringHelper.UTF8BytesToString(ref this.szHeroSound_ByteArray);
			this.szHeroSound_ByteArray = null;
			this.szBorn_Age = StringHelper.UTF8BytesToString(ref this.szBorn_Age_ByteArray);
			this.szBorn_Age_ByteArray = null;
			this.szRevive_Age = StringHelper.UTF8BytesToString(ref this.szRevive_Age_ByteArray);
			this.szRevive_Age_ByteArray = null;
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
			int nValue = 0;
			int nValue2 = 0;
			int nValue3 = 0;
			int nValue4 = 0;
			int nValue5 = 0;
			int nValue6 = 0;
			int nValue7 = 0;
			int nValue8 = 0;
			int nValue9 = 0;
			int nValue10 = 0;
			int nValue11 = 0;
			int nValue12 = 0;
			int nValue13 = 0;
			int nValue14 = 0;
			int nValue15 = 0;
			int nValue16 = 0;
			int nValue17 = 0;
			int nValue18 = 0;
			int nValue19 = 0;
			int nValue20 = 0;
			int nValue21 = 0;
			int nValue22 = 0;
			int nValue23 = 0;
			int nValue24 = 0;
			if (cutVer == 0u || ResHeroCfgInfo.CURRVERSION < cutVer)
			{
				cutVer = ResHeroCfgInfo.CURRVERSION;
			}
			if (ResHeroCfgInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwCfgID);
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
				if ((long)num > (long)((ulong)ResHeroCfgInfo.LENGTH_szName))
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
			if (num3 > (uint)this.szNamePinYin_ByteArray.GetLength(0))
			{
				if ((long)num3 > (long)((ulong)ResHeroCfgInfo.LENGTH_szNamePinYin))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szNamePinYin_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szNamePinYin_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szNamePinYin_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szNamePinYin_ByteArray) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bIsTrainUse);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (num5 > (uint)this.szHeroTitle_ByteArray.GetLength(0))
			{
				if ((long)num5 > (long)((ulong)ResHeroCfgInfo.LENGTH_szHeroTitle))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szHeroTitle_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szHeroTitle_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szHeroTitle_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szHeroTitle_ByteArray) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
			if (num7 > (uint)this.szStoryUrl_ByteArray.GetLength(0))
			{
				if ((long)num7 > (long)((ulong)ResHeroCfgInfo.LENGTH_szStoryUrl))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szStoryUrl_ByteArray = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szStoryUrl_ByteArray, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szStoryUrl_ByteArray[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szStoryUrl_ByteArray) + 1;
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
			if (num9 > (uint)this.szImagePath_ByteArray.GetLength(0))
			{
				if ((long)num9 > (long)((ulong)ResHeroCfgInfo.LENGTH_szImagePath))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szImagePath_ByteArray = new byte[num9];
			}
			if (1u > num9)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szImagePath_ByteArray, (int)num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szImagePath_ByteArray[(int)(num9 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num10 = TdrTypeUtil.cstrlen(this.szImagePath_ByteArray) + 1;
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
			if (num11 > (uint)this.szCharacterInfo_ByteArray.GetLength(0))
			{
				if ((long)num11 > (long)((ulong)ResHeroCfgInfo.LENGTH_szCharacterInfo))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szCharacterInfo_ByteArray = new byte[num11];
			}
			if (1u > num11)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szCharacterInfo_ByteArray, (int)num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szCharacterInfo_ByteArray[(int)(num11 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num12 = TdrTypeUtil.cstrlen(this.szCharacterInfo_ByteArray) + 1;
			if ((ulong)num11 != (ulong)((long)num12))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readInt32(ref this.iRecommendPosition);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bAttackDistanceType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSightR);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue);
			this.iBaseHP = nValue;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue2);
			this.iBaseHPAdd = nValue2;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue3);
			this.iHPAddLvlup = nValue3;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue4);
			this.iBaseATT = nValue4;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue5);
			this.iBaseINT = nValue5;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue6);
			this.iBaseDEF = nValue6;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue7);
			this.iBaseRES = nValue7;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue8);
			this.iBaseSpeed = nValue8;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue9);
			this.iAtkSpdAddLvlup = nValue9;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue10);
			this.iBaseAtkSpd = nValue10;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue11);
			this.iCritRate = nValue11;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue12);
			this.iCritEft = nValue12;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue13);
			this.iHpGrowth = nValue13;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue14);
			this.iAtkGrowth = nValue14;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue15);
			this.iSpellGrowth = nValue15;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue16);
			this.iDefGrowth = nValue16;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue17);
			this.iResistGrowth = nValue17;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iPassiveID1);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iPassiveID2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astSkill[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readInt32(ref this.iInitialStar);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bExpandType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wPVPNeedLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iPVPNeedQuality);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iPVPNeedSubQuality);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iPVPNeedStar);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue18);
			this.iViability = nValue18;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue19);
			this.iPhyDamage = nValue19;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue20);
			this.iSpellDamage = nValue20;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iStartedDifficulty);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bMainJob);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bMinorJob);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int j = 0; j < 2; j++)
			{
				errorType = srcBuf.readInt32(ref this.JobFeature[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bDamageType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bAttackType);
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
			if (num13 > (uint)this.szHeroDesc_ByteArray.GetLength(0))
			{
				if ((long)num13 > (long)((ulong)ResHeroCfgInfo.LENGTH_szHeroDesc))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szHeroDesc_ByteArray = new byte[num13];
			}
			if (1u > num13)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szHeroDesc_ByteArray, (int)num13);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szHeroDesc_ByteArray[(int)(num13 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num14 = TdrTypeUtil.cstrlen(this.szHeroDesc_ByteArray) + 1;
			if ((ulong)num13 != (ulong)((long)num14))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bIOSHide);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwShowSortId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (num15 > (uint)this.szAI_Entry_ByteArray.GetLength(0))
			{
				if ((long)num15 > (long)((ulong)ResHeroCfgInfo.LENGTH_szAI_Entry))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szAI_Entry_ByteArray = new byte[num15];
			}
			if (1u > num15)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szAI_Entry_ByteArray, (int)num15);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szAI_Entry_ByteArray[(int)(num15 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num16 = TdrTypeUtil.cstrlen(this.szAI_Entry_ByteArray) + 1;
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
			if (num17 > (uint)this.szAI_Simple_ByteArray.GetLength(0))
			{
				if ((long)num17 > (long)((ulong)ResHeroCfgInfo.LENGTH_szAI_Simple))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szAI_Simple_ByteArray = new byte[num17];
			}
			if (1u > num17)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szAI_Simple_ByteArray, (int)num17);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szAI_Simple_ByteArray[(int)(num17 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num18 = TdrTypeUtil.cstrlen(this.szAI_Simple_ByteArray) + 1;
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
			if (num19 > (uint)this.szAI_Normal_ByteArray.GetLength(0))
			{
				if ((long)num19 > (long)((ulong)ResHeroCfgInfo.LENGTH_szAI_Normal))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szAI_Normal_ByteArray = new byte[num19];
			}
			if (1u > num19)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szAI_Normal_ByteArray, (int)num19);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szAI_Normal_ByteArray[(int)(num19 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num20 = TdrTypeUtil.cstrlen(this.szAI_Normal_ByteArray) + 1;
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
			if (num21 > (uint)this.szAI_Hard_ByteArray.GetLength(0))
			{
				if ((long)num21 > (long)((ulong)ResHeroCfgInfo.LENGTH_szAI_Hard))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szAI_Hard_ByteArray = new byte[num21];
			}
			if (1u > num21)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szAI_Hard_ByteArray, (int)num21);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szAI_Hard_ByteArray[(int)(num21 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num22 = TdrTypeUtil.cstrlen(this.szAI_Hard_ByteArray) + 1;
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
			if (num23 > (uint)this.szAI_WarmSimple_ByteArray.GetLength(0))
			{
				if ((long)num23 > (long)((ulong)ResHeroCfgInfo.LENGTH_szAI_WarmSimple))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szAI_WarmSimple_ByteArray = new byte[num23];
			}
			if (1u > num23)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szAI_WarmSimple_ByteArray, (int)num23);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szAI_WarmSimple_ByteArray[(int)(num23 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num24 = TdrTypeUtil.cstrlen(this.szAI_WarmSimple_ByteArray) + 1;
			if ((ulong)num23 != (ulong)((long)num24))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num25 = 0u;
			errorType = srcBuf.readUInt32(ref num25);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num25 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num25 > (uint)this.szAI_Warm_ByteArray.GetLength(0))
			{
				if ((long)num25 > (long)((ulong)ResHeroCfgInfo.LENGTH_szAI_Warm))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szAI_Warm_ByteArray = new byte[num25];
			}
			if (1u > num25)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szAI_Warm_ByteArray, (int)num25);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szAI_Warm_ByteArray[(int)(num25 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num26 = TdrTypeUtil.cstrlen(this.szAI_Warm_ByteArray) + 1;
			if ((ulong)num25 != (ulong)((long)num26))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num27 = 0u;
			errorType = srcBuf.readUInt32(ref num27);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num27 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num27 > (uint)this.szWakeDesc_ByteArray.GetLength(0))
			{
				if ((long)num27 > (long)((ulong)ResHeroCfgInfo.LENGTH_szWakeDesc))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szWakeDesc_ByteArray = new byte[num27];
			}
			if (1u > num27)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szWakeDesc_ByteArray, (int)num27);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szWakeDesc_ByteArray[(int)(num27 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num28 = TdrTypeUtil.cstrlen(this.szWakeDesc_ByteArray) + 1;
			if ((ulong)num27 != (ulong)((long)num28))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwWakeTalentID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwWakeSkinID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num29 = 0u;
			errorType = srcBuf.readUInt32(ref num29);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num29 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num29 > (uint)this.szAttackRangeDesc_ByteArray.GetLength(0))
			{
				if ((long)num29 > (long)((ulong)ResHeroCfgInfo.LENGTH_szAttackRangeDesc))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szAttackRangeDesc_ByteArray = new byte[num29];
			}
			if (1u > num29)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szAttackRangeDesc_ByteArray, (int)num29);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szAttackRangeDesc_ByteArray[(int)(num29 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num30 = TdrTypeUtil.cstrlen(this.szAttackRangeDesc_ByteArray) + 1;
			if ((ulong)num29 != (ulong)((long)num30))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwEnergyType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue21);
			this.iEnergy = nValue21;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue22);
			this.iEnergyGrowth = nValue22;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue23);
			this.iEnergyRec = nValue23;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue24);
			this.iEnergyRecGrowth = nValue24;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num31 = 0u;
			errorType = srcBuf.readUInt32(ref num31);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num31 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num31 > (uint)this.szHeroTips_ByteArray.GetLength(0))
			{
				if ((long)num31 > (long)((ulong)ResHeroCfgInfo.LENGTH_szHeroTips))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szHeroTips_ByteArray = new byte[num31];
			}
			if (1u > num31)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szHeroTips_ByteArray, (int)num31);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szHeroTips_ByteArray[(int)(num31 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num32 = TdrTypeUtil.cstrlen(this.szHeroTips_ByteArray) + 1;
			if ((ulong)num31 != (ulong)((long)num32))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num33 = 0u;
			errorType = srcBuf.readUInt32(ref num33);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num33 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num33 > (uint)this.szHeroSound_ByteArray.GetLength(0))
			{
				if ((long)num33 > (long)((ulong)ResHeroCfgInfo.LENGTH_szHeroSound))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szHeroSound_ByteArray = new byte[num33];
			}
			if (1u > num33)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szHeroSound_ByteArray, (int)num33);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szHeroSound_ByteArray[(int)(num33 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num34 = TdrTypeUtil.cstrlen(this.szHeroSound_ByteArray) + 1;
			if ((ulong)num33 != (ulong)((long)num34))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwSymbolRcmdID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num35 = 0u;
			errorType = srcBuf.readUInt32(ref num35);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num35 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num35 > (uint)this.szBorn_Age_ByteArray.GetLength(0))
			{
				if ((long)num35 > (long)((ulong)ResHeroCfgInfo.LENGTH_szBorn_Age))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szBorn_Age_ByteArray = new byte[num35];
			}
			if (1u > num35)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szBorn_Age_ByteArray, (int)num35);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szBorn_Age_ByteArray[(int)(num35 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num36 = TdrTypeUtil.cstrlen(this.szBorn_Age_ByteArray) + 1;
			if ((ulong)num35 != (ulong)((long)num36))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num37 = 0u;
			errorType = srcBuf.readUInt32(ref num37);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num37 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num37 > (uint)this.szRevive_Age_ByteArray.GetLength(0))
			{
				if ((long)num37 > (long)((ulong)ResHeroCfgInfo.LENGTH_szRevive_Age))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szRevive_Age_ByteArray = new byte[num37];
			}
			if (1u > num37)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szRevive_Age_ByteArray, (int)num37);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szRevive_Age_ByteArray[(int)(num37 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num38 = TdrTypeUtil.cstrlen(this.szRevive_Age_ByteArray) + 1;
			if ((ulong)num37 != (ulong)((long)num38))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwDeadControl);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bTag);
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
			int nValue = 0;
			int nValue2 = 0;
			int nValue3 = 0;
			int nValue4 = 0;
			int nValue5 = 0;
			int nValue6 = 0;
			int nValue7 = 0;
			int nValue8 = 0;
			int nValue9 = 0;
			int nValue10 = 0;
			int nValue11 = 0;
			int nValue12 = 0;
			int nValue13 = 0;
			int nValue14 = 0;
			int nValue15 = 0;
			int nValue16 = 0;
			int nValue17 = 0;
			int nValue18 = 0;
			int nValue19 = 0;
			int nValue20 = 0;
			int nValue21 = 0;
			int nValue22 = 0;
			int nValue23 = 0;
			int nValue24 = 0;
			srcBuf.disableEndian();
			if (cutVer == 0u || ResHeroCfgInfo.CURRVERSION < cutVer)
			{
				cutVer = ResHeroCfgInfo.CURRVERSION;
			}
			if (ResHeroCfgInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwCfgID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 32;
			if (this.szName_ByteArray.GetLength(0) < num)
			{
				this.szName_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szName];
			}
			errorType = srcBuf.readCString(ref this.szName_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 32;
			if (this.szNamePinYin_ByteArray.GetLength(0) < num2)
			{
				this.szNamePinYin_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szNamePinYin];
			}
			errorType = srcBuf.readCString(ref this.szNamePinYin_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsTrainUse);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 128;
			if (this.szHeroTitle_ByteArray.GetLength(0) < num3)
			{
				this.szHeroTitle_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szHeroTitle];
			}
			errorType = srcBuf.readCString(ref this.szHeroTitle_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num4 = 256;
			if (this.szStoryUrl_ByteArray.GetLength(0) < num4)
			{
				this.szStoryUrl_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szStoryUrl];
			}
			errorType = srcBuf.readCString(ref this.szStoryUrl_ByteArray, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num5 = 128;
			if (this.szImagePath_ByteArray.GetLength(0) < num5)
			{
				this.szImagePath_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szImagePath];
			}
			errorType = srcBuf.readCString(ref this.szImagePath_ByteArray, num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num6 = 128;
			if (this.szCharacterInfo_ByteArray.GetLength(0) < num6)
			{
				this.szCharacterInfo_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szCharacterInfo];
			}
			errorType = srcBuf.readCString(ref this.szCharacterInfo_ByteArray, num6);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iRecommendPosition);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bAttackDistanceType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSightR);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue);
			this.iBaseHP = nValue;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue2);
			this.iBaseHPAdd = nValue2;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue3);
			this.iHPAddLvlup = nValue3;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue4);
			this.iBaseATT = nValue4;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue5);
			this.iBaseINT = nValue5;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue6);
			this.iBaseDEF = nValue6;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue7);
			this.iBaseRES = nValue7;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue8);
			this.iBaseSpeed = nValue8;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue9);
			this.iAtkSpdAddLvlup = nValue9;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue10);
			this.iBaseAtkSpd = nValue10;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue11);
			this.iCritRate = nValue11;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue12);
			this.iCritEft = nValue12;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue13);
			this.iHpGrowth = nValue13;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue14);
			this.iAtkGrowth = nValue14;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue15);
			this.iSpellGrowth = nValue15;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue16);
			this.iDefGrowth = nValue16;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue17);
			this.iResistGrowth = nValue17;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iPassiveID1);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iPassiveID2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astSkill[i].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readInt32(ref this.iInitialStar);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bExpandType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wPVPNeedLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iPVPNeedQuality);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iPVPNeedSubQuality);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iPVPNeedStar);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue18);
			this.iViability = nValue18;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue19);
			this.iPhyDamage = nValue19;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue20);
			this.iSpellDamage = nValue20;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iStartedDifficulty);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bMainJob);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bMinorJob);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int j = 0; j < 2; j++)
			{
				errorType = srcBuf.readInt32(ref this.JobFeature[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bDamageType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bAttackType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num7 = 256;
			if (this.szHeroDesc_ByteArray.GetLength(0) < num7)
			{
				this.szHeroDesc_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szHeroDesc];
			}
			errorType = srcBuf.readCString(ref this.szHeroDesc_ByteArray, num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIOSHide);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwShowSortId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num8 = 128;
			if (this.szAI_Entry_ByteArray.GetLength(0) < num8)
			{
				this.szAI_Entry_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szAI_Entry];
			}
			errorType = srcBuf.readCString(ref this.szAI_Entry_ByteArray, num8);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num9 = 128;
			if (this.szAI_Simple_ByteArray.GetLength(0) < num9)
			{
				this.szAI_Simple_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szAI_Simple];
			}
			errorType = srcBuf.readCString(ref this.szAI_Simple_ByteArray, num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num10 = 128;
			if (this.szAI_Normal_ByteArray.GetLength(0) < num10)
			{
				this.szAI_Normal_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szAI_Normal];
			}
			errorType = srcBuf.readCString(ref this.szAI_Normal_ByteArray, num10);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num11 = 128;
			if (this.szAI_Hard_ByteArray.GetLength(0) < num11)
			{
				this.szAI_Hard_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szAI_Hard];
			}
			errorType = srcBuf.readCString(ref this.szAI_Hard_ByteArray, num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num12 = 128;
			if (this.szAI_WarmSimple_ByteArray.GetLength(0) < num12)
			{
				this.szAI_WarmSimple_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szAI_WarmSimple];
			}
			errorType = srcBuf.readCString(ref this.szAI_WarmSimple_ByteArray, num12);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num13 = 128;
			if (this.szAI_Warm_ByteArray.GetLength(0) < num13)
			{
				this.szAI_Warm_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szAI_Warm];
			}
			errorType = srcBuf.readCString(ref this.szAI_Warm_ByteArray, num13);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num14 = 512;
			if (this.szWakeDesc_ByteArray.GetLength(0) < num14)
			{
				this.szWakeDesc_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szWakeDesc];
			}
			errorType = srcBuf.readCString(ref this.szWakeDesc_ByteArray, num14);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwWakeTalentID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwWakeSkinID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num15 = 16;
			if (this.szAttackRangeDesc_ByteArray.GetLength(0) < num15)
			{
				this.szAttackRangeDesc_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szAttackRangeDesc];
			}
			errorType = srcBuf.readCString(ref this.szAttackRangeDesc_ByteArray, num15);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwEnergyType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue21);
			this.iEnergy = nValue21;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue22);
			this.iEnergyGrowth = nValue22;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue23);
			this.iEnergyRec = nValue23;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue24);
			this.iEnergyRecGrowth = nValue24;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num16 = 4096;
			if (this.szHeroTips_ByteArray.GetLength(0) < num16)
			{
				this.szHeroTips_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szHeroTips];
			}
			errorType = srcBuf.readCString(ref this.szHeroTips_ByteArray, num16);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num17 = 64;
			if (this.szHeroSound_ByteArray.GetLength(0) < num17)
			{
				this.szHeroSound_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szHeroSound];
			}
			errorType = srcBuf.readCString(ref this.szHeroSound_ByteArray, num17);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSymbolRcmdID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num18 = 128;
			if (this.szBorn_Age_ByteArray.GetLength(0) < num18)
			{
				this.szBorn_Age_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szBorn_Age];
			}
			errorType = srcBuf.readCString(ref this.szBorn_Age_ByteArray, num18);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num19 = 128;
			if (this.szRevive_Age_ByteArray.GetLength(0) < num19)
			{
				this.szRevive_Age_ByteArray = new byte[ResHeroCfgInfo.LENGTH_szRevive_Age];
			}
			errorType = srcBuf.readCString(ref this.szRevive_Age_ByteArray, num19);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDeadControl);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bTag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}
