using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResSkillCfgInfo : tsf4g_csharp_interface, IUnpackable
	{
		public int iCfgID;

		public byte[] szSkillName_ByteArray;

		public byte[] szSkillDesc_ByteArray;

		public byte[] szLobbySkillDesc_ByteArray;

		public byte bSkillType;

		public byte bWheelType;

		public byte[] szSkillValueDesc_ByteArray;

		public byte[] szSkillUpTip_ByteArray;

		public byte[] szIconPath_ByteArray;

		public byte bSelectEffectPrefab;

		public byte[] szGuidePrefab_ByteArray;

		public byte[] szEffectPrefab_ByteArray;

		public byte[] szGuideWarnPrefab_ByteArray;

		public byte[] szEffectWarnPrefab_ByteArray;

		public byte[] szFixedPrefab_ByteArray;

		public byte[] szFixedWarnPrefab_ByteArray;

		public byte[] szMapIndicatorNormalPrefab_ByteArray;

		public byte[] szMapIndicatorRedPrefab_ByteArray;

		public int iSmallMapIndicatorHeight;

		public int iBigMapIndicatorHeight;

		public CrypticInteger iCoolDown;

		public byte bImmediateUse;

		public byte bIsInterruptImmediateUseSkill;

		public byte bBIngnoreDisable;

		public byte[] szPrefab_ByteArray;

		public int iSelfSkillCombine;

		public int iTargetSkillCombine;

		public byte bRangeAppointType;

		public byte bIndicatorType;

		public CrypticInteger iRangeRadius;

		public byte bTgtIncludeSelf;

		public byte bTgtIncludeEnemy;

		public CrypticInteger iBaseDamage;

		public CrypticInteger iFixedDistance;

		public CrypticInteger iGuideDistance;

		public CrypticInteger iMaxAttackDistance;

		public int iGreaterAttackDistance;

		public int iMaxSearchDistance;

		public int iMaxSearchDistanceGrowthValue;

		public int iMaxChaseDistance;

		public byte bSkillUseRule;

		public byte bSkillTargetRule;

		public uint dwSkillTargetFilter;

		public byte bIsCheckAntiCheat;

		public ushort[] SkillEffectType;

		public byte bAutoEnergyCost;

		public byte bEnergyCostType;

		public byte bEnergyCostCalcType;

		public CrypticInteger iEnergyCost;

		public CrypticInteger iEnergyCostGrowth;

		public CrypticInteger iCoolDownGrowth;

		public byte bIsStunSkill;

		public byte bNoInfluenceAnim;

		public byte bAgeImmeExcute;

		public ResDT_SkillDescription[] astSkillPropertyDescInfo;

		public byte bSkillNonExposing;

		public byte bSkillIndicatorFocusOnCallMonster;

		public string szSkillName;

		public string szSkillDesc;

		public string szLobbySkillDesc;

		public string szSkillValueDesc;

		public string szSkillUpTip;

		public string szIconPath;

		public string szGuidePrefab;

		public string szEffectPrefab;

		public string szGuideWarnPrefab;

		public string szEffectWarnPrefab;

		public string szFixedPrefab;

		public string szFixedWarnPrefab;

		public string szMapIndicatorNormalPrefab;

		public string szMapIndicatorRedPrefab;

		public string szPrefab;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szSkillName = 128u;

		public static readonly uint LENGTH_szSkillDesc = 1024u;

		public static readonly uint LENGTH_szLobbySkillDesc = 1024u;

		public static readonly uint LENGTH_szSkillValueDesc = 1024u;

		public static readonly uint LENGTH_szSkillUpTip = 256u;

		public static readonly uint LENGTH_szIconPath = 128u;

		public static readonly uint LENGTH_szGuidePrefab = 128u;

		public static readonly uint LENGTH_szEffectPrefab = 128u;

		public static readonly uint LENGTH_szGuideWarnPrefab = 128u;

		public static readonly uint LENGTH_szEffectWarnPrefab = 128u;

		public static readonly uint LENGTH_szFixedPrefab = 128u;

		public static readonly uint LENGTH_szFixedWarnPrefab = 128u;

		public static readonly uint LENGTH_szMapIndicatorNormalPrefab = 128u;

		public static readonly uint LENGTH_szMapIndicatorRedPrefab = 128u;

		public static readonly uint LENGTH_szPrefab = 128u;

		public ResSkillCfgInfo()
		{
			this.szSkillName_ByteArray = new byte[1];
			this.szSkillDesc_ByteArray = new byte[1];
			this.szLobbySkillDesc_ByteArray = new byte[1];
			this.szSkillValueDesc_ByteArray = new byte[1];
			this.szSkillUpTip_ByteArray = new byte[1];
			this.szIconPath_ByteArray = new byte[1];
			this.szGuidePrefab_ByteArray = new byte[1];
			this.szEffectPrefab_ByteArray = new byte[1];
			this.szGuideWarnPrefab_ByteArray = new byte[1];
			this.szEffectWarnPrefab_ByteArray = new byte[1];
			this.szFixedPrefab_ByteArray = new byte[1];
			this.szFixedWarnPrefab_ByteArray = new byte[1];
			this.szMapIndicatorNormalPrefab_ByteArray = new byte[1];
			this.szMapIndicatorRedPrefab_ByteArray = new byte[1];
			this.szPrefab_ByteArray = new byte[1];
			this.SkillEffectType = new ushort[2];
			this.astSkillPropertyDescInfo = new ResDT_SkillDescription[5];
			for (int i = 0; i < 5; i++)
			{
				this.astSkillPropertyDescInfo[i] = new ResDT_SkillDescription();
			}
			this.szSkillName = string.Empty;
			this.szSkillDesc = string.Empty;
			this.szLobbySkillDesc = string.Empty;
			this.szSkillValueDesc = string.Empty;
			this.szSkillUpTip = string.Empty;
			this.szIconPath = string.Empty;
			this.szGuidePrefab = string.Empty;
			this.szEffectPrefab = string.Empty;
			this.szGuideWarnPrefab = string.Empty;
			this.szEffectWarnPrefab = string.Empty;
			this.szFixedPrefab = string.Empty;
			this.szFixedWarnPrefab = string.Empty;
			this.szMapIndicatorNormalPrefab = string.Empty;
			this.szMapIndicatorRedPrefab = string.Empty;
			this.szPrefab = string.Empty;
		}

		private void TransferData()
		{
			this.szSkillName = StringHelper.UTF8BytesToString(ref this.szSkillName_ByteArray);
			this.szSkillName_ByteArray = null;
			this.szSkillDesc = StringHelper.UTF8BytesToString(ref this.szSkillDesc_ByteArray);
			this.szSkillDesc_ByteArray = null;
			this.szLobbySkillDesc = StringHelper.UTF8BytesToString(ref this.szLobbySkillDesc_ByteArray);
			this.szLobbySkillDesc_ByteArray = null;
			this.szSkillValueDesc = StringHelper.UTF8BytesToString(ref this.szSkillValueDesc_ByteArray);
			this.szSkillValueDesc_ByteArray = null;
			this.szSkillUpTip = StringHelper.UTF8BytesToString(ref this.szSkillUpTip_ByteArray);
			this.szSkillUpTip_ByteArray = null;
			this.szIconPath = StringHelper.UTF8BytesToString(ref this.szIconPath_ByteArray);
			this.szIconPath_ByteArray = null;
			this.szGuidePrefab = StringHelper.UTF8BytesToString(ref this.szGuidePrefab_ByteArray);
			this.szGuidePrefab_ByteArray = null;
			this.szEffectPrefab = StringHelper.UTF8BytesToString(ref this.szEffectPrefab_ByteArray);
			this.szEffectPrefab_ByteArray = null;
			this.szGuideWarnPrefab = StringHelper.UTF8BytesToString(ref this.szGuideWarnPrefab_ByteArray);
			this.szGuideWarnPrefab_ByteArray = null;
			this.szEffectWarnPrefab = StringHelper.UTF8BytesToString(ref this.szEffectWarnPrefab_ByteArray);
			this.szEffectWarnPrefab_ByteArray = null;
			this.szFixedPrefab = StringHelper.UTF8BytesToString(ref this.szFixedPrefab_ByteArray);
			this.szFixedPrefab_ByteArray = null;
			this.szFixedWarnPrefab = StringHelper.UTF8BytesToString(ref this.szFixedWarnPrefab_ByteArray);
			this.szFixedWarnPrefab_ByteArray = null;
			this.szMapIndicatorNormalPrefab = StringHelper.UTF8BytesToString(ref this.szMapIndicatorNormalPrefab_ByteArray);
			this.szMapIndicatorNormalPrefab_ByteArray = null;
			this.szMapIndicatorRedPrefab = StringHelper.UTF8BytesToString(ref this.szMapIndicatorRedPrefab_ByteArray);
			this.szMapIndicatorRedPrefab_ByteArray = null;
			this.szPrefab = StringHelper.UTF8BytesToString(ref this.szPrefab_ByteArray);
			this.szPrefab_ByteArray = null;
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
			if (cutVer == 0u || ResSkillCfgInfo.CURRVERSION < cutVer)
			{
				cutVer = ResSkillCfgInfo.CURRVERSION;
			}
			if (ResSkillCfgInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iCfgID);
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
			if (num > (uint)this.szSkillName_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResSkillCfgInfo.LENGTH_szSkillName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSkillName_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSkillName_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSkillName_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szSkillName_ByteArray) + 1;
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
			if (num3 > (uint)this.szSkillDesc_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResSkillCfgInfo.LENGTH_szSkillDesc)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSkillDesc_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSkillDesc_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSkillDesc_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szSkillDesc_ByteArray) + 1;
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
			if (num5 > (uint)this.szLobbySkillDesc_ByteArray.GetLength(0))
			{
				if ((ulong)num5 > (ulong)ResSkillCfgInfo.LENGTH_szLobbySkillDesc)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szLobbySkillDesc_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szLobbySkillDesc_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szLobbySkillDesc_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szLobbySkillDesc_ByteArray) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bSkillType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bWheelType);
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
			if (num7 > (uint)this.szSkillValueDesc_ByteArray.GetLength(0))
			{
				if ((ulong)num7 > (ulong)ResSkillCfgInfo.LENGTH_szSkillValueDesc)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSkillValueDesc_ByteArray = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSkillValueDesc_ByteArray, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSkillValueDesc_ByteArray[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szSkillValueDesc_ByteArray) + 1;
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
			if (num9 > (uint)this.szSkillUpTip_ByteArray.GetLength(0))
			{
				if ((ulong)num9 > (ulong)ResSkillCfgInfo.LENGTH_szSkillUpTip)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSkillUpTip_ByteArray = new byte[num9];
			}
			if (1u > num9)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSkillUpTip_ByteArray, (int)num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSkillUpTip_ByteArray[(int)(num9 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num10 = TdrTypeUtil.cstrlen(this.szSkillUpTip_ByteArray) + 1;
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
			if (num11 > (uint)this.szIconPath_ByteArray.GetLength(0))
			{
				if ((ulong)num11 > (ulong)ResSkillCfgInfo.LENGTH_szIconPath)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szIconPath_ByteArray = new byte[num11];
			}
			if (1u > num11)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szIconPath_ByteArray, (int)num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szIconPath_ByteArray[(int)(num11 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num12 = TdrTypeUtil.cstrlen(this.szIconPath_ByteArray) + 1;
			if ((ulong)num11 != (ulong)((long)num12))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bSelectEffectPrefab);
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
			if (num13 > (uint)this.szGuidePrefab_ByteArray.GetLength(0))
			{
				if ((ulong)num13 > (ulong)ResSkillCfgInfo.LENGTH_szGuidePrefab)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGuidePrefab_ByteArray = new byte[num13];
			}
			if (1u > num13)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGuidePrefab_ByteArray, (int)num13);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGuidePrefab_ByteArray[(int)(num13 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num14 = TdrTypeUtil.cstrlen(this.szGuidePrefab_ByteArray) + 1;
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
			if (num15 > (uint)this.szEffectPrefab_ByteArray.GetLength(0))
			{
				if ((ulong)num15 > (ulong)ResSkillCfgInfo.LENGTH_szEffectPrefab)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szEffectPrefab_ByteArray = new byte[num15];
			}
			if (1u > num15)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szEffectPrefab_ByteArray, (int)num15);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szEffectPrefab_ByteArray[(int)(num15 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num16 = TdrTypeUtil.cstrlen(this.szEffectPrefab_ByteArray) + 1;
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
			if (num17 > (uint)this.szGuideWarnPrefab_ByteArray.GetLength(0))
			{
				if ((ulong)num17 > (ulong)ResSkillCfgInfo.LENGTH_szGuideWarnPrefab)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGuideWarnPrefab_ByteArray = new byte[num17];
			}
			if (1u > num17)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGuideWarnPrefab_ByteArray, (int)num17);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGuideWarnPrefab_ByteArray[(int)(num17 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num18 = TdrTypeUtil.cstrlen(this.szGuideWarnPrefab_ByteArray) + 1;
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
			if (num19 > (uint)this.szEffectWarnPrefab_ByteArray.GetLength(0))
			{
				if ((ulong)num19 > (ulong)ResSkillCfgInfo.LENGTH_szEffectWarnPrefab)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szEffectWarnPrefab_ByteArray = new byte[num19];
			}
			if (1u > num19)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szEffectWarnPrefab_ByteArray, (int)num19);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szEffectWarnPrefab_ByteArray[(int)(num19 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num20 = TdrTypeUtil.cstrlen(this.szEffectWarnPrefab_ByteArray) + 1;
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
			if (num21 > (uint)this.szFixedPrefab_ByteArray.GetLength(0))
			{
				if ((ulong)num21 > (ulong)ResSkillCfgInfo.LENGTH_szFixedPrefab)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szFixedPrefab_ByteArray = new byte[num21];
			}
			if (1u > num21)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szFixedPrefab_ByteArray, (int)num21);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szFixedPrefab_ByteArray[(int)(num21 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num22 = TdrTypeUtil.cstrlen(this.szFixedPrefab_ByteArray) + 1;
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
			if (num23 > (uint)this.szFixedWarnPrefab_ByteArray.GetLength(0))
			{
				if ((ulong)num23 > (ulong)ResSkillCfgInfo.LENGTH_szFixedWarnPrefab)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szFixedWarnPrefab_ByteArray = new byte[num23];
			}
			if (1u > num23)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szFixedWarnPrefab_ByteArray, (int)num23);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szFixedWarnPrefab_ByteArray[(int)(num23 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num24 = TdrTypeUtil.cstrlen(this.szFixedWarnPrefab_ByteArray) + 1;
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
			if (num25 > (uint)this.szMapIndicatorNormalPrefab_ByteArray.GetLength(0))
			{
				if ((ulong)num25 > (ulong)ResSkillCfgInfo.LENGTH_szMapIndicatorNormalPrefab)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szMapIndicatorNormalPrefab_ByteArray = new byte[num25];
			}
			if (1u > num25)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szMapIndicatorNormalPrefab_ByteArray, (int)num25);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szMapIndicatorNormalPrefab_ByteArray[(int)(num25 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num26 = TdrTypeUtil.cstrlen(this.szMapIndicatorNormalPrefab_ByteArray) + 1;
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
			if (num27 > (uint)this.szMapIndicatorRedPrefab_ByteArray.GetLength(0))
			{
				if ((ulong)num27 > (ulong)ResSkillCfgInfo.LENGTH_szMapIndicatorRedPrefab)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szMapIndicatorRedPrefab_ByteArray = new byte[num27];
			}
			if (1u > num27)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szMapIndicatorRedPrefab_ByteArray, (int)num27);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szMapIndicatorRedPrefab_ByteArray[(int)(num27 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num28 = TdrTypeUtil.cstrlen(this.szMapIndicatorRedPrefab_ByteArray) + 1;
			if ((ulong)num27 != (ulong)((long)num28))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readInt32(ref this.iSmallMapIndicatorHeight);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBigMapIndicatorHeight);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue);
			this.iCoolDown = nValue;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bImmediateUse);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsInterruptImmediateUseSkill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bBIngnoreDisable);
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
			if (num29 > (uint)this.szPrefab_ByteArray.GetLength(0))
			{
				if ((ulong)num29 > (ulong)ResSkillCfgInfo.LENGTH_szPrefab)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szPrefab_ByteArray = new byte[num29];
			}
			if (1u > num29)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szPrefab_ByteArray, (int)num29);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szPrefab_ByteArray[(int)(num29 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num30 = TdrTypeUtil.cstrlen(this.szPrefab_ByteArray) + 1;
			if ((ulong)num29 != (ulong)((long)num30))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readInt32(ref this.iSelfSkillCombine);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iTargetSkillCombine);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bRangeAppointType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIndicatorType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue2);
			this.iRangeRadius = nValue2;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bTgtIncludeSelf);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bTgtIncludeEnemy);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue3);
			this.iBaseDamage = nValue3;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue4);
			this.iFixedDistance = nValue4;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue5);
			this.iGuideDistance = nValue5;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue6);
			this.iMaxAttackDistance = nValue6;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iGreaterAttackDistance);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMaxSearchDistance);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMaxSearchDistanceGrowthValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMaxChaseDistance);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSkillUseRule);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSkillTargetRule);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSkillTargetFilter);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsCheckAntiCheat);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 2; i++)
			{
				errorType = srcBuf.readUInt16(ref this.SkillEffectType[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bAutoEnergyCost);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bEnergyCostType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bEnergyCostCalcType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue7);
			this.iEnergyCost = nValue7;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue8);
			this.iEnergyCostGrowth = nValue8;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue9);
			this.iCoolDownGrowth = nValue9;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsStunSkill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bNoInfluenceAnim);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bAgeImmeExcute);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int j = 0; j < 5; j++)
			{
				errorType = this.astSkillPropertyDescInfo[j].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bSkillNonExposing);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSkillIndicatorFocusOnCallMonster);
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
			srcBuf.disableEndian();
			if (cutVer == 0u || ResSkillCfgInfo.CURRVERSION < cutVer)
			{
				cutVer = ResSkillCfgInfo.CURRVERSION;
			}
			if (ResSkillCfgInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iCfgID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 128;
			if (this.szSkillName_ByteArray.GetLength(0) < num)
			{
				this.szSkillName_ByteArray = new byte[ResSkillCfgInfo.LENGTH_szSkillName];
			}
			errorType = srcBuf.readCString(ref this.szSkillName_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 1024;
			if (this.szSkillDesc_ByteArray.GetLength(0) < num2)
			{
				this.szSkillDesc_ByteArray = new byte[ResSkillCfgInfo.LENGTH_szSkillDesc];
			}
			errorType = srcBuf.readCString(ref this.szSkillDesc_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 1024;
			if (this.szLobbySkillDesc_ByteArray.GetLength(0) < num3)
			{
				this.szLobbySkillDesc_ByteArray = new byte[ResSkillCfgInfo.LENGTH_szLobbySkillDesc];
			}
			errorType = srcBuf.readCString(ref this.szLobbySkillDesc_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSkillType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bWheelType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num4 = 1024;
			if (this.szSkillValueDesc_ByteArray.GetLength(0) < num4)
			{
				this.szSkillValueDesc_ByteArray = new byte[ResSkillCfgInfo.LENGTH_szSkillValueDesc];
			}
			errorType = srcBuf.readCString(ref this.szSkillValueDesc_ByteArray, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num5 = 256;
			if (this.szSkillUpTip_ByteArray.GetLength(0) < num5)
			{
				this.szSkillUpTip_ByteArray = new byte[ResSkillCfgInfo.LENGTH_szSkillUpTip];
			}
			errorType = srcBuf.readCString(ref this.szSkillUpTip_ByteArray, num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num6 = 128;
			if (this.szIconPath_ByteArray.GetLength(0) < num6)
			{
				this.szIconPath_ByteArray = new byte[ResSkillCfgInfo.LENGTH_szIconPath];
			}
			errorType = srcBuf.readCString(ref this.szIconPath_ByteArray, num6);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSelectEffectPrefab);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num7 = 128;
			if (this.szGuidePrefab_ByteArray.GetLength(0) < num7)
			{
				this.szGuidePrefab_ByteArray = new byte[ResSkillCfgInfo.LENGTH_szGuidePrefab];
			}
			errorType = srcBuf.readCString(ref this.szGuidePrefab_ByteArray, num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num8 = 128;
			if (this.szEffectPrefab_ByteArray.GetLength(0) < num8)
			{
				this.szEffectPrefab_ByteArray = new byte[ResSkillCfgInfo.LENGTH_szEffectPrefab];
			}
			errorType = srcBuf.readCString(ref this.szEffectPrefab_ByteArray, num8);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num9 = 128;
			if (this.szGuideWarnPrefab_ByteArray.GetLength(0) < num9)
			{
				this.szGuideWarnPrefab_ByteArray = new byte[ResSkillCfgInfo.LENGTH_szGuideWarnPrefab];
			}
			errorType = srcBuf.readCString(ref this.szGuideWarnPrefab_ByteArray, num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num10 = 128;
			if (this.szEffectWarnPrefab_ByteArray.GetLength(0) < num10)
			{
				this.szEffectWarnPrefab_ByteArray = new byte[ResSkillCfgInfo.LENGTH_szEffectWarnPrefab];
			}
			errorType = srcBuf.readCString(ref this.szEffectWarnPrefab_ByteArray, num10);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num11 = 128;
			if (this.szFixedPrefab_ByteArray.GetLength(0) < num11)
			{
				this.szFixedPrefab_ByteArray = new byte[ResSkillCfgInfo.LENGTH_szFixedPrefab];
			}
			errorType = srcBuf.readCString(ref this.szFixedPrefab_ByteArray, num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num12 = 128;
			if (this.szFixedWarnPrefab_ByteArray.GetLength(0) < num12)
			{
				this.szFixedWarnPrefab_ByteArray = new byte[ResSkillCfgInfo.LENGTH_szFixedWarnPrefab];
			}
			errorType = srcBuf.readCString(ref this.szFixedWarnPrefab_ByteArray, num12);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num13 = 128;
			if (this.szMapIndicatorNormalPrefab_ByteArray.GetLength(0) < num13)
			{
				this.szMapIndicatorNormalPrefab_ByteArray = new byte[ResSkillCfgInfo.LENGTH_szMapIndicatorNormalPrefab];
			}
			errorType = srcBuf.readCString(ref this.szMapIndicatorNormalPrefab_ByteArray, num13);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num14 = 128;
			if (this.szMapIndicatorRedPrefab_ByteArray.GetLength(0) < num14)
			{
				this.szMapIndicatorRedPrefab_ByteArray = new byte[ResSkillCfgInfo.LENGTH_szMapIndicatorRedPrefab];
			}
			errorType = srcBuf.readCString(ref this.szMapIndicatorRedPrefab_ByteArray, num14);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSmallMapIndicatorHeight);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBigMapIndicatorHeight);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue);
			this.iCoolDown = nValue;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bImmediateUse);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsInterruptImmediateUseSkill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bBIngnoreDisable);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num15 = 128;
			if (this.szPrefab_ByteArray.GetLength(0) < num15)
			{
				this.szPrefab_ByteArray = new byte[ResSkillCfgInfo.LENGTH_szPrefab];
			}
			errorType = srcBuf.readCString(ref this.szPrefab_ByteArray, num15);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSelfSkillCombine);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iTargetSkillCombine);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bRangeAppointType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIndicatorType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue2);
			this.iRangeRadius = nValue2;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bTgtIncludeSelf);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bTgtIncludeEnemy);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue3);
			this.iBaseDamage = nValue3;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue4);
			this.iFixedDistance = nValue4;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue5);
			this.iGuideDistance = nValue5;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue6);
			this.iMaxAttackDistance = nValue6;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iGreaterAttackDistance);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMaxSearchDistance);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMaxSearchDistanceGrowthValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMaxChaseDistance);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSkillUseRule);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSkillTargetRule);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSkillTargetFilter);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsCheckAntiCheat);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 2; i++)
			{
				errorType = srcBuf.readUInt16(ref this.SkillEffectType[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bAutoEnergyCost);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bEnergyCostType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bEnergyCostCalcType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue7);
			this.iEnergyCost = nValue7;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue8);
			this.iEnergyCostGrowth = nValue8;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref nValue9);
			this.iCoolDownGrowth = nValue9;
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsStunSkill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bNoInfluenceAnim);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bAgeImmeExcute);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int j = 0; j < 5; j++)
			{
				errorType = this.astSkillPropertyDescInfo[j].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bSkillNonExposing);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSkillIndicatorFocusOnCallMonster);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}
