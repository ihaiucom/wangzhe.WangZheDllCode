using Assets.Scripts.Common;
using System;
using System.Text;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_CLIENT_INFO_DATA : ProtocolObject
	{
		public byte[] szSystemSoftware;

		public byte[] szSystemHardware;

		public byte[] szTelecomOper;

		public byte[] szNetwork;

		public int iLoginChannel;

		public byte[] szClientVersion;

		public int iMemorySize;

		public byte[] szCltBuildNumber;

		public byte[] szCltSvnVersion;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szSystemSoftware = 64u;

		public static readonly uint LENGTH_szSystemHardware = 64u;

		public static readonly uint LENGTH_szTelecomOper = 64u;

		public static readonly uint LENGTH_szNetwork = 64u;

		public static readonly uint LENGTH_szClientVersion = 64u;

		public static readonly uint LENGTH_szCltBuildNumber = 64u;

		public static readonly uint LENGTH_szCltSvnVersion = 64u;

		public static readonly int CLASS_ID = 552;

		public COMDT_CLIENT_INFO_DATA()
		{
			this.szSystemSoftware = new byte[64];
			this.szSystemHardware = new byte[64];
			this.szTelecomOper = new byte[64];
			this.szNetwork = new byte[64];
			this.szClientVersion = new byte[64];
			this.szCltBuildNumber = new byte[64];
			this.szCltSvnVersion = new byte[64];
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			string text = "NULL";
			byte[] bytes = Encoding.get_ASCII().GetBytes(text);
			if (bytes.GetLength(0) + 1 > this.szSystemSoftware.GetLength(0))
			{
				if ((long)bytes.GetLength(0) >= (long)((ulong)COMDT_CLIENT_INFO_DATA.LENGTH_szSystemSoftware))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSystemSoftware = new byte[bytes.GetLength(0) + 1];
			}
			for (int i = 0; i < bytes.GetLength(0); i++)
			{
				this.szSystemSoftware[i] = bytes[i];
			}
			this.szSystemSoftware[bytes.GetLength(0)] = 0;
			string text2 = "NULL";
			byte[] bytes2 = Encoding.get_ASCII().GetBytes(text2);
			if (bytes2.GetLength(0) + 1 > this.szSystemHardware.GetLength(0))
			{
				if ((long)bytes2.GetLength(0) >= (long)((ulong)COMDT_CLIENT_INFO_DATA.LENGTH_szSystemHardware))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSystemHardware = new byte[bytes2.GetLength(0) + 1];
			}
			for (int j = 0; j < bytes2.GetLength(0); j++)
			{
				this.szSystemHardware[j] = bytes2[j];
			}
			this.szSystemHardware[bytes2.GetLength(0)] = 0;
			string text3 = "NULL";
			byte[] bytes3 = Encoding.get_ASCII().GetBytes(text3);
			if (bytes3.GetLength(0) + 1 > this.szTelecomOper.GetLength(0))
			{
				if ((long)bytes3.GetLength(0) >= (long)((ulong)COMDT_CLIENT_INFO_DATA.LENGTH_szTelecomOper))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szTelecomOper = new byte[bytes3.GetLength(0) + 1];
			}
			for (int k = 0; k < bytes3.GetLength(0); k++)
			{
				this.szTelecomOper[k] = bytes3[k];
			}
			this.szTelecomOper[bytes3.GetLength(0)] = 0;
			string text4 = "NULL";
			byte[] bytes4 = Encoding.get_ASCII().GetBytes(text4);
			if (bytes4.GetLength(0) + 1 > this.szNetwork.GetLength(0))
			{
				if ((long)bytes4.GetLength(0) >= (long)((ulong)COMDT_CLIENT_INFO_DATA.LENGTH_szNetwork))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szNetwork = new byte[bytes4.GetLength(0) + 1];
			}
			for (int l = 0; l < bytes4.GetLength(0); l++)
			{
				this.szNetwork[l] = bytes4[l];
			}
			this.szNetwork[bytes4.GetLength(0)] = 0;
			this.iLoginChannel = 0;
			string text5 = "NULL";
			byte[] bytes5 = Encoding.get_ASCII().GetBytes(text5);
			if (bytes5.GetLength(0) + 1 > this.szClientVersion.GetLength(0))
			{
				if ((long)bytes5.GetLength(0) >= (long)((ulong)COMDT_CLIENT_INFO_DATA.LENGTH_szClientVersion))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szClientVersion = new byte[bytes5.GetLength(0) + 1];
			}
			for (int m = 0; m < bytes5.GetLength(0); m++)
			{
				this.szClientVersion[m] = bytes5[m];
			}
			this.szClientVersion[bytes5.GetLength(0)] = 0;
			this.iMemorySize = 0;
			return result;
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
			if (cutVer == 0u || COMDT_CLIENT_INFO_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_CLIENT_INFO_DATA.CURRVERSION;
			}
			if (COMDT_CLIENT_INFO_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			int usedSize = destBuf.getUsedSize();
			TdrError.ErrorType errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize2 = destBuf.getUsedSize();
			int num = TdrTypeUtil.cstrlen(this.szSystemSoftware);
			if (num >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szSystemSoftware, num);
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
			int usedSize3 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize4 = destBuf.getUsedSize();
			int num2 = TdrTypeUtil.cstrlen(this.szSystemHardware);
			if (num2 >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szSystemHardware, num2);
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
			int usedSize5 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize6 = destBuf.getUsedSize();
			int num3 = TdrTypeUtil.cstrlen(this.szTelecomOper);
			if (num3 >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szTelecomOper, num3);
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
			int usedSize7 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize8 = destBuf.getUsedSize();
			int num4 = TdrTypeUtil.cstrlen(this.szNetwork);
			if (num4 >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szNetwork, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src4 = destBuf.getUsedSize() - usedSize8;
			errorType = destBuf.writeUInt32((uint)src4, usedSize7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iLoginChannel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize9 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize10 = destBuf.getUsedSize();
			int num5 = TdrTypeUtil.cstrlen(this.szClientVersion);
			if (num5 >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szClientVersion, num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src5 = destBuf.getUsedSize() - usedSize10;
			errorType = destBuf.writeUInt32((uint)src5, usedSize9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iMemorySize);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize11 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize12 = destBuf.getUsedSize();
			int num6 = TdrTypeUtil.cstrlen(this.szCltBuildNumber);
			if (num6 >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szCltBuildNumber, num6);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src6 = destBuf.getUsedSize() - usedSize12;
			errorType = destBuf.writeUInt32((uint)src6, usedSize11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize13 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize14 = destBuf.getUsedSize();
			int num7 = TdrTypeUtil.cstrlen(this.szCltSvnVersion);
			if (num7 >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szCltSvnVersion, num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src7 = destBuf.getUsedSize() - usedSize14;
			errorType = destBuf.writeUInt32((uint)src7, usedSize13);
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
			if (cutVer == 0u || COMDT_CLIENT_INFO_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_CLIENT_INFO_DATA.CURRVERSION;
			}
			if (COMDT_CLIENT_INFO_DATA.BASEVERSION > cutVer)
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
			if (num > (uint)this.szSystemSoftware.GetLength(0))
			{
				if ((ulong)num > (ulong)COMDT_CLIENT_INFO_DATA.LENGTH_szSystemSoftware)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSystemSoftware = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSystemSoftware, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSystemSoftware[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szSystemSoftware) + 1;
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
			if (num3 > (uint)this.szSystemHardware.GetLength(0))
			{
				if ((ulong)num3 > (ulong)COMDT_CLIENT_INFO_DATA.LENGTH_szSystemHardware)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szSystemHardware = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szSystemHardware, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szSystemHardware[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szSystemHardware) + 1;
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
			if (num5 > (uint)this.szTelecomOper.GetLength(0))
			{
				if ((ulong)num5 > (ulong)COMDT_CLIENT_INFO_DATA.LENGTH_szTelecomOper)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szTelecomOper = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szTelecomOper, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szTelecomOper[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szTelecomOper) + 1;
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
			if (num7 > (uint)this.szNetwork.GetLength(0))
			{
				if ((ulong)num7 > (ulong)COMDT_CLIENT_INFO_DATA.LENGTH_szNetwork)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szNetwork = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szNetwork, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szNetwork[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szNetwork) + 1;
			if ((ulong)num7 != (ulong)((long)num8))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readInt32(ref this.iLoginChannel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (num9 > (uint)this.szClientVersion.GetLength(0))
			{
				if ((ulong)num9 > (ulong)COMDT_CLIENT_INFO_DATA.LENGTH_szClientVersion)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szClientVersion = new byte[num9];
			}
			if (1u > num9)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szClientVersion, (int)num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szClientVersion[(int)(num9 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num10 = TdrTypeUtil.cstrlen(this.szClientVersion) + 1;
			if ((ulong)num9 != (ulong)((long)num10))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readInt32(ref this.iMemorySize);
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
			if (num11 > (uint)this.szCltBuildNumber.GetLength(0))
			{
				if ((ulong)num11 > (ulong)COMDT_CLIENT_INFO_DATA.LENGTH_szCltBuildNumber)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szCltBuildNumber = new byte[num11];
			}
			if (1u > num11)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szCltBuildNumber, (int)num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szCltBuildNumber[(int)(num11 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num12 = TdrTypeUtil.cstrlen(this.szCltBuildNumber) + 1;
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
			if (num13 > (uint)this.szCltSvnVersion.GetLength(0))
			{
				if ((ulong)num13 > (ulong)COMDT_CLIENT_INFO_DATA.LENGTH_szCltSvnVersion)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szCltSvnVersion = new byte[num13];
			}
			if (1u > num13)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szCltSvnVersion, (int)num13);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szCltSvnVersion[(int)(num13 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num14 = TdrTypeUtil.cstrlen(this.szCltSvnVersion) + 1;
			if ((ulong)num13 != (ulong)((long)num14))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_CLIENT_INFO_DATA.CLASS_ID;
		}
	}
}
