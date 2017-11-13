using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResSkillMarkCfgInfo : tsf4g_csharp_interface, IUnpackable
	{
		public int iCfgID;

		public int iDependCfgID;

		public byte[] szMarkName_ByteArray;

		public byte[] szMarkDesc_ByteArray;

		public byte[] szActionName_ByteArray;

		public byte bLayerEffect;

		public int iMaxLayer;

		public int iCostLayer;

		public int iTriggerLayer;

		public int iImmuneTime;

		public int iLastMaxTime;

		public int iCDTime;

		public byte bAutoTrigger;

		public uint dwEffectMask;

		public byte[] szLayerEffectName1_ByteArray;

		public byte[] szLayerEffectName2_ByteArray;

		public byte[] szLayerEffectName3_ByteArray;

		public byte[] szLayerEffectName4_ByteArray;

		public byte[] szLayerEffectName5_ByteArray;

		public byte bAgeImmeExcute;

		public string szMarkName;

		public string szMarkDesc;

		public string szActionName;

		public string szLayerEffectName1;

		public string szLayerEffectName2;

		public string szLayerEffectName3;

		public string szLayerEffectName4;

		public string szLayerEffectName5;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szMarkName = 128u;

		public static readonly uint LENGTH_szMarkDesc = 128u;

		public static readonly uint LENGTH_szActionName = 128u;

		public static readonly uint LENGTH_szLayerEffectName1 = 128u;

		public static readonly uint LENGTH_szLayerEffectName2 = 128u;

		public static readonly uint LENGTH_szLayerEffectName3 = 128u;

		public static readonly uint LENGTH_szLayerEffectName4 = 128u;

		public static readonly uint LENGTH_szLayerEffectName5 = 128u;

		public ResSkillMarkCfgInfo()
		{
			this.szMarkName_ByteArray = new byte[1];
			this.szMarkDesc_ByteArray = new byte[1];
			this.szActionName_ByteArray = new byte[1];
			this.szLayerEffectName1_ByteArray = new byte[1];
			this.szLayerEffectName2_ByteArray = new byte[1];
			this.szLayerEffectName3_ByteArray = new byte[1];
			this.szLayerEffectName4_ByteArray = new byte[1];
			this.szLayerEffectName5_ByteArray = new byte[1];
			this.szMarkName = string.Empty;
			this.szMarkDesc = string.Empty;
			this.szActionName = string.Empty;
			this.szLayerEffectName1 = string.Empty;
			this.szLayerEffectName2 = string.Empty;
			this.szLayerEffectName3 = string.Empty;
			this.szLayerEffectName4 = string.Empty;
			this.szLayerEffectName5 = string.Empty;
		}

		private void TransferData()
		{
			this.szMarkName = StringHelper.UTF8BytesToString(ref this.szMarkName_ByteArray);
			this.szMarkName_ByteArray = null;
			this.szMarkDesc = StringHelper.UTF8BytesToString(ref this.szMarkDesc_ByteArray);
			this.szMarkDesc_ByteArray = null;
			this.szActionName = StringHelper.UTF8BytesToString(ref this.szActionName_ByteArray);
			this.szActionName_ByteArray = null;
			this.szLayerEffectName1 = StringHelper.UTF8BytesToString(ref this.szLayerEffectName1_ByteArray);
			this.szLayerEffectName1_ByteArray = null;
			this.szLayerEffectName2 = StringHelper.UTF8BytesToString(ref this.szLayerEffectName2_ByteArray);
			this.szLayerEffectName2_ByteArray = null;
			this.szLayerEffectName3 = StringHelper.UTF8BytesToString(ref this.szLayerEffectName3_ByteArray);
			this.szLayerEffectName3_ByteArray = null;
			this.szLayerEffectName4 = StringHelper.UTF8BytesToString(ref this.szLayerEffectName4_ByteArray);
			this.szLayerEffectName4_ByteArray = null;
			this.szLayerEffectName5 = StringHelper.UTF8BytesToString(ref this.szLayerEffectName5_ByteArray);
			this.szLayerEffectName5_ByteArray = null;
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
			if (cutVer == 0u || ResSkillMarkCfgInfo.CURRVERSION < cutVer)
			{
				cutVer = ResSkillMarkCfgInfo.CURRVERSION;
			}
			if (ResSkillMarkCfgInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iCfgID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iDependCfgID);
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
			if (num > (uint)this.szMarkName_ByteArray.GetLength(0))
			{
				if ((ulong)num > (ulong)ResSkillMarkCfgInfo.LENGTH_szMarkName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szMarkName_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szMarkName_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szMarkName_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szMarkName_ByteArray) + 1;
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
			if (num3 > (uint)this.szMarkDesc_ByteArray.GetLength(0))
			{
				if ((ulong)num3 > (ulong)ResSkillMarkCfgInfo.LENGTH_szMarkDesc)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szMarkDesc_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szMarkDesc_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szMarkDesc_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szMarkDesc_ByteArray) + 1;
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
			if (num5 > (uint)this.szActionName_ByteArray.GetLength(0))
			{
				if ((ulong)num5 > (ulong)ResSkillMarkCfgInfo.LENGTH_szActionName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szActionName_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szActionName_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szActionName_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szActionName_ByteArray) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bLayerEffect);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMaxLayer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iCostLayer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iTriggerLayer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iImmuneTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iLastMaxTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iCDTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bAutoTrigger);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwEffectMask);
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
			if (num7 > (uint)this.szLayerEffectName1_ByteArray.GetLength(0))
			{
				if ((ulong)num7 > (ulong)ResSkillMarkCfgInfo.LENGTH_szLayerEffectName1)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szLayerEffectName1_ByteArray = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szLayerEffectName1_ByteArray, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szLayerEffectName1_ByteArray[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szLayerEffectName1_ByteArray) + 1;
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
			if (num9 > (uint)this.szLayerEffectName2_ByteArray.GetLength(0))
			{
				if ((ulong)num9 > (ulong)ResSkillMarkCfgInfo.LENGTH_szLayerEffectName2)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szLayerEffectName2_ByteArray = new byte[num9];
			}
			if (1u > num9)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szLayerEffectName2_ByteArray, (int)num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szLayerEffectName2_ByteArray[(int)(num9 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num10 = TdrTypeUtil.cstrlen(this.szLayerEffectName2_ByteArray) + 1;
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
			if (num11 > (uint)this.szLayerEffectName3_ByteArray.GetLength(0))
			{
				if ((ulong)num11 > (ulong)ResSkillMarkCfgInfo.LENGTH_szLayerEffectName3)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szLayerEffectName3_ByteArray = new byte[num11];
			}
			if (1u > num11)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szLayerEffectName3_ByteArray, (int)num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szLayerEffectName3_ByteArray[(int)(num11 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num12 = TdrTypeUtil.cstrlen(this.szLayerEffectName3_ByteArray) + 1;
			if ((ulong)num11 != (ulong)((long)num12))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
			if (num13 > (uint)this.szLayerEffectName4_ByteArray.GetLength(0))
			{
				if ((ulong)num13 > (ulong)ResSkillMarkCfgInfo.LENGTH_szLayerEffectName4)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szLayerEffectName4_ByteArray = new byte[num13];
			}
			if (1u > num13)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szLayerEffectName4_ByteArray, (int)num13);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szLayerEffectName4_ByteArray[(int)(num13 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num14 = TdrTypeUtil.cstrlen(this.szLayerEffectName4_ByteArray) + 1;
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
			if (num15 > (uint)this.szLayerEffectName5_ByteArray.GetLength(0))
			{
				if ((ulong)num15 > (ulong)ResSkillMarkCfgInfo.LENGTH_szLayerEffectName5)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szLayerEffectName5_ByteArray = new byte[num15];
			}
			if (1u > num15)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szLayerEffectName5_ByteArray, (int)num15);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szLayerEffectName5_ByteArray[(int)(num15 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num16 = TdrTypeUtil.cstrlen(this.szLayerEffectName5_ByteArray) + 1;
			if ((ulong)num15 != (ulong)((long)num16))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bAgeImmeExcute);
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
			if (cutVer == 0u || ResSkillMarkCfgInfo.CURRVERSION < cutVer)
			{
				cutVer = ResSkillMarkCfgInfo.CURRVERSION;
			}
			if (ResSkillMarkCfgInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iCfgID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iDependCfgID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 128;
			if (this.szMarkName_ByteArray.GetLength(0) < num)
			{
				this.szMarkName_ByteArray = new byte[ResSkillMarkCfgInfo.LENGTH_szMarkName];
			}
			errorType = srcBuf.readCString(ref this.szMarkName_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 128;
			if (this.szMarkDesc_ByteArray.GetLength(0) < num2)
			{
				this.szMarkDesc_ByteArray = new byte[ResSkillMarkCfgInfo.LENGTH_szMarkDesc];
			}
			errorType = srcBuf.readCString(ref this.szMarkDesc_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 128;
			if (this.szActionName_ByteArray.GetLength(0) < num3)
			{
				this.szActionName_ByteArray = new byte[ResSkillMarkCfgInfo.LENGTH_szActionName];
			}
			errorType = srcBuf.readCString(ref this.szActionName_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bLayerEffect);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMaxLayer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iCostLayer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iTriggerLayer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iImmuneTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iLastMaxTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iCDTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bAutoTrigger);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwEffectMask);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num4 = 128;
			if (this.szLayerEffectName1_ByteArray.GetLength(0) < num4)
			{
				this.szLayerEffectName1_ByteArray = new byte[ResSkillMarkCfgInfo.LENGTH_szLayerEffectName1];
			}
			errorType = srcBuf.readCString(ref this.szLayerEffectName1_ByteArray, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num5 = 128;
			if (this.szLayerEffectName2_ByteArray.GetLength(0) < num5)
			{
				this.szLayerEffectName2_ByteArray = new byte[ResSkillMarkCfgInfo.LENGTH_szLayerEffectName2];
			}
			errorType = srcBuf.readCString(ref this.szLayerEffectName2_ByteArray, num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num6 = 128;
			if (this.szLayerEffectName3_ByteArray.GetLength(0) < num6)
			{
				this.szLayerEffectName3_ByteArray = new byte[ResSkillMarkCfgInfo.LENGTH_szLayerEffectName3];
			}
			errorType = srcBuf.readCString(ref this.szLayerEffectName3_ByteArray, num6);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num7 = 128;
			if (this.szLayerEffectName4_ByteArray.GetLength(0) < num7)
			{
				this.szLayerEffectName4_ByteArray = new byte[ResSkillMarkCfgInfo.LENGTH_szLayerEffectName4];
			}
			errorType = srcBuf.readCString(ref this.szLayerEffectName4_ByteArray, num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num8 = 128;
			if (this.szLayerEffectName5_ByteArray.GetLength(0) < num8)
			{
				this.szLayerEffectName5_ByteArray = new byte[ResSkillMarkCfgInfo.LENGTH_szLayerEffectName5];
			}
			errorType = srcBuf.readCString(ref this.szLayerEffectName5_ByteArray, num8);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bAgeImmeExcute);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}
