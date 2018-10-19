using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResHeroSkin : IUnpackable, tsf4g_csharp_interface
	{
		public uint dwID;

		public uint dwHeroID;

		public byte[] szHeroName_ByteArray;

		public uint dwSkinID;

		public byte[] szSkinName_ByteArray;

		public byte[] szSkinPicID_ByteArray;

		public byte[] szSkinSoundResPack_ByteArray;

		public byte[] szSoundSwitchEvent_ByteArray;

		public uint dwCombatAbility;

		public ResDT_FuncEft_Obj[] astAttr;

		public uint dwPresentHeadImg;

		public byte[] szShareSkinUrl_ByteArray;

		public ResDT_SkinFeature[] astFeature;

		public byte[] szUrl_ByteArray;

		public string szHeroName;

		public string szSkinName;

		public string szSkinPicID;

		public string szSkinSoundResPack;

		public string szSoundSwitchEvent;

		public string szShareSkinUrl;

		public string szUrl;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szHeroName = 32u;

		public static readonly uint LENGTH_szSkinName = 32u;

		public static readonly uint LENGTH_szSkinPicID = 128u;

		public static readonly uint LENGTH_szSkinSoundResPack = 128u;

		public static readonly uint LENGTH_szSoundSwitchEvent = 128u;

		public static readonly uint LENGTH_szShareSkinUrl = 64u;

		public static readonly uint LENGTH_szUrl = 128u;

		public ResHeroSkin()
		{
			this.szHeroName_ByteArray = new byte[1];
			this.szSkinName_ByteArray = new byte[1];
			this.szSkinPicID_ByteArray = new byte[1];
			this.szSkinSoundResPack_ByteArray = new byte[1];
			this.szSoundSwitchEvent_ByteArray = new byte[1];
			this.astAttr = new ResDT_FuncEft_Obj[15];
			for (int i = 0; i < 15; i++)
			{
				this.astAttr[i] = new ResDT_FuncEft_Obj();
			}
			this.szShareSkinUrl_ByteArray = new byte[1];
			this.astFeature = new ResDT_SkinFeature[10];
			for (int j = 0; j < 10; j++)
			{
				this.astFeature[j] = new ResDT_SkinFeature();
			}
			this.szUrl_ByteArray = new byte[1];
			this.szHeroName = string.Empty;
			this.szSkinName = string.Empty;
			this.szSkinPicID = string.Empty;
			this.szSkinSoundResPack = string.Empty;
			this.szSoundSwitchEvent = string.Empty;
			this.szShareSkinUrl = string.Empty;
			this.szUrl = string.Empty;
		}

		private void TransferData()
		{
			this.szHeroName = StringHelper.UTF8BytesToString(ref this.szHeroName_ByteArray);
			this.szHeroName_ByteArray = null;
			this.szSkinName = StringHelper.UTF8BytesToString(ref this.szSkinName_ByteArray);
			this.szSkinName_ByteArray = null;
			this.szSkinPicID = StringHelper.UTF8BytesToString(ref this.szSkinPicID_ByteArray);
			this.szSkinPicID_ByteArray = null;
			this.szSkinSoundResPack = StringHelper.UTF8BytesToString(ref this.szSkinSoundResPack_ByteArray);
			this.szSkinSoundResPack_ByteArray = null;
			this.szSoundSwitchEvent = StringHelper.UTF8BytesToString(ref this.szSoundSwitchEvent_ByteArray);
			this.szSoundSwitchEvent_ByteArray = null;
			this.szShareSkinUrl = StringHelper.UTF8BytesToString(ref this.szShareSkinUrl_ByteArray);
			this.szShareSkinUrl_ByteArray = null;
			this.szUrl = StringHelper.UTF8BytesToString(ref this.szUrl_ByteArray);
			this.szUrl_ByteArray = null;
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
			if (cutVer == 0u || ResHeroSkin.CURRVERSION < cutVer)
			{
				cutVer = ResHeroSkin.CURRVERSION;
			}
			if (ResHeroSkin.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHeroID);
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
			if (num > (uint)this.szHeroName_ByteArray.GetLength(0))
			{
				if ((long)num > (long)((ulong)ResHeroSkin.LENGTH_szHeroName))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szHeroName_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szHeroName_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szHeroName_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szHeroName_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwSkinID);
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
			if (num3 > (uint)this.szSkinName_ByteArray.GetLength(0))
			{
				if ((long)num3 > (long)((ulong)ResHeroSkin.LENGTH_szSkinName))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSkinName_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSkinName_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSkinName_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szSkinName_ByteArray) + 1;
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
			if (num5 > (uint)this.szSkinPicID_ByteArray.GetLength(0))
			{
				if ((long)num5 > (long)((ulong)ResHeroSkin.LENGTH_szSkinPicID))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSkinPicID_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSkinPicID_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSkinPicID_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szSkinPicID_ByteArray) + 1;
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
			if (num7 > (uint)this.szSkinSoundResPack_ByteArray.GetLength(0))
			{
				if ((long)num7 > (long)((ulong)ResHeroSkin.LENGTH_szSkinSoundResPack))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSkinSoundResPack_ByteArray = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSkinSoundResPack_ByteArray, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSkinSoundResPack_ByteArray[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szSkinSoundResPack_ByteArray) + 1;
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
			if (num9 > (uint)this.szSoundSwitchEvent_ByteArray.GetLength(0))
			{
				if ((long)num9 > (long)((ulong)ResHeroSkin.LENGTH_szSoundSwitchEvent))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSoundSwitchEvent_ByteArray = new byte[num9];
			}
			if (1u > num9)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSoundSwitchEvent_ByteArray, (int)num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSoundSwitchEvent_ByteArray[(int)(num9 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num10 = TdrTypeUtil.cstrlen(this.szSoundSwitchEvent_ByteArray) + 1;
			if ((ulong)num9 != (ulong)((long)num10))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwCombatAbility);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 15; i++)
			{
				errorType = this.astAttr[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwPresentHeadImg);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (num11 > (uint)this.szShareSkinUrl_ByteArray.GetLength(0))
			{
				if ((long)num11 > (long)((ulong)ResHeroSkin.LENGTH_szShareSkinUrl))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szShareSkinUrl_ByteArray = new byte[num11];
			}
			if (1u > num11)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szShareSkinUrl_ByteArray, (int)num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szShareSkinUrl_ByteArray[(int)(num11 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num12 = TdrTypeUtil.cstrlen(this.szShareSkinUrl_ByteArray) + 1;
			if ((ulong)num11 != (ulong)((long)num12))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			for (int j = 0; j < 10; j++)
			{
				errorType = this.astFeature[j].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
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
			if (num13 > (uint)this.szUrl_ByteArray.GetLength(0))
			{
				if ((long)num13 > (long)((ulong)ResHeroSkin.LENGTH_szUrl))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szUrl_ByteArray = new byte[num13];
			}
			if (1u > num13)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szUrl_ByteArray, (int)num13);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szUrl_ByteArray[(int)(num13 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num14 = TdrTypeUtil.cstrlen(this.szUrl_ByteArray) + 1;
			if ((ulong)num13 != (ulong)((long)num14))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
			if (cutVer == 0u || ResHeroSkin.CURRVERSION < cutVer)
			{
				cutVer = ResHeroSkin.CURRVERSION;
			}
			if (ResHeroSkin.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHeroID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 32;
			if (this.szHeroName_ByteArray.GetLength(0) < num)
			{
				this.szHeroName_ByteArray = new byte[ResHeroSkin.LENGTH_szHeroName];
			}
			errorType = srcBuf.readCString(ref this.szHeroName_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSkinID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 32;
			if (this.szSkinName_ByteArray.GetLength(0) < num2)
			{
				this.szSkinName_ByteArray = new byte[ResHeroSkin.LENGTH_szSkinName];
			}
			errorType = srcBuf.readCString(ref this.szSkinName_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 128;
			if (this.szSkinPicID_ByteArray.GetLength(0) < num3)
			{
				this.szSkinPicID_ByteArray = new byte[ResHeroSkin.LENGTH_szSkinPicID];
			}
			errorType = srcBuf.readCString(ref this.szSkinPicID_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num4 = 128;
			if (this.szSkinSoundResPack_ByteArray.GetLength(0) < num4)
			{
				this.szSkinSoundResPack_ByteArray = new byte[ResHeroSkin.LENGTH_szSkinSoundResPack];
			}
			errorType = srcBuf.readCString(ref this.szSkinSoundResPack_ByteArray, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num5 = 128;
			if (this.szSoundSwitchEvent_ByteArray.GetLength(0) < num5)
			{
				this.szSoundSwitchEvent_ByteArray = new byte[ResHeroSkin.LENGTH_szSoundSwitchEvent];
			}
			errorType = srcBuf.readCString(ref this.szSoundSwitchEvent_ByteArray, num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCombatAbility);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 15; i++)
			{
				errorType = this.astAttr[i].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwPresentHeadImg);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num6 = 64;
			if (this.szShareSkinUrl_ByteArray.GetLength(0) < num6)
			{
				this.szShareSkinUrl_ByteArray = new byte[ResHeroSkin.LENGTH_szShareSkinUrl];
			}
			errorType = srcBuf.readCString(ref this.szShareSkinUrl_ByteArray, num6);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int j = 0; j < 10; j++)
			{
				errorType = this.astFeature[j].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			int num7 = 128;
			if (this.szUrl_ByteArray.GetLength(0) < num7)
			{
				this.szUrl_ByteArray = new byte[ResHeroSkin.LENGTH_szUrl];
			}
			errorType = srcBuf.readCString(ref this.szUrl_ByteArray, num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.TransferData();
			return errorType;
		}
	}
}
