using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSPKG_CMD_GAMELOGINREQ : ProtocolObject
	{
		public int iCltAppVersion;

		public int iCltResVersion;

		public COMDT_CLIENT_INFO_DATA stClientInfo;

		public ulong ullVisitorUid;

		public int iLogicWorldID;

		public uint dwTaccZoneID;

		public byte[] szCltIMEI;

		public byte bPrivilege;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 62u;

		public static readonly uint VERSION_bPrivilege = 62u;

		public static readonly uint LENGTH_szCltIMEI = 64u;

		public static readonly int CLASS_ID = 718;

		public CSPKG_CMD_GAMELOGINREQ()
		{
			this.stClientInfo = (COMDT_CLIENT_INFO_DATA)ProtocolObjectPool.Get(COMDT_CLIENT_INFO_DATA.CLASS_ID);
			this.szCltIMEI = new byte[64];
		}

		public override TdrError.ErrorType construct()
		{
			this.iCltAppVersion = 0;
			this.iCltResVersion = 0;
			TdrError.ErrorType errorType = this.stClientInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.ullVisitorUid = 0uL;
			this.iLogicWorldID = 0;
			this.dwTaccZoneID = 0u;
			this.bPrivilege = 0;
			return errorType;
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
			if (cutVer == 0u || CSPKG_CMD_GAMELOGINREQ.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_CMD_GAMELOGINREQ.CURRVERSION;
			}
			if (CSPKG_CMD_GAMELOGINREQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iCltAppVersion);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iCltResVersion);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stClientInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullVisitorUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iLogicWorldID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTaccZoneID);
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
			int num = TdrTypeUtil.cstrlen(this.szCltIMEI);
			if (num >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szCltIMEI, num);
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
			if (CSPKG_CMD_GAMELOGINREQ.VERSION_bPrivilege <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bPrivilege);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
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
			if (cutVer == 0u || CSPKG_CMD_GAMELOGINREQ.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_CMD_GAMELOGINREQ.CURRVERSION;
			}
			if (CSPKG_CMD_GAMELOGINREQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iCltAppVersion);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iCltResVersion);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stClientInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullVisitorUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iLogicWorldID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTaccZoneID);
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
			if (num > (uint)this.szCltIMEI.GetLength(0))
			{
				if ((ulong)num > (ulong)CSPKG_CMD_GAMELOGINREQ.LENGTH_szCltIMEI)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szCltIMEI = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szCltIMEI, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szCltIMEI[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szCltIMEI) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			if (CSPKG_CMD_GAMELOGINREQ.VERSION_bPrivilege <= cutVer)
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
			return errorType;
		}

		public override int GetClassID()
		{
			return CSPKG_CMD_GAMELOGINREQ.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iCltAppVersion = 0;
			this.iCltResVersion = 0;
			if (this.stClientInfo != null)
			{
				this.stClientInfo.Release();
				this.stClientInfo = null;
			}
			this.ullVisitorUid = 0uL;
			this.iLogicWorldID = 0;
			this.dwTaccZoneID = 0u;
			this.bPrivilege = 0;
		}

		public override void OnUse()
		{
			this.stClientInfo = (COMDT_CLIENT_INFO_DATA)ProtocolObjectPool.Get(COMDT_CLIENT_INFO_DATA.CLASS_ID);
		}
	}
}
