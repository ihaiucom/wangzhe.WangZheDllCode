using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_MULTGAMEREADYNTF : ProtocolObject
	{
		public byte bCamp;

		public COMDT_TGWINFO stRelayTGW;

		public COMDT_OB_DESK stDeskInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 749;

		public SCPKG_MULTGAMEREADYNTF()
		{
			this.stRelayTGW = (COMDT_TGWINFO)ProtocolObjectPool.Get(COMDT_TGWINFO.CLASS_ID);
			this.stDeskInfo = (COMDT_OB_DESK)ProtocolObjectPool.Get(COMDT_OB_DESK.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_MULTGAMEREADYNTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_MULTGAMEREADYNTF.CURRVERSION;
			}
			if (SCPKG_MULTGAMEREADYNTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bCamp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRelayTGW.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stDeskInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_MULTGAMEREADYNTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_MULTGAMEREADYNTF.CURRVERSION;
			}
			if (SCPKG_MULTGAMEREADYNTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bCamp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRelayTGW.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stDeskInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_MULTGAMEREADYNTF.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bCamp = 0;
			if (this.stRelayTGW != null)
			{
				this.stRelayTGW.Release();
				this.stRelayTGW = null;
			}
			if (this.stDeskInfo != null)
			{
				this.stDeskInfo.Release();
				this.stDeskInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stRelayTGW = (COMDT_TGWINFO)ProtocolObjectPool.Get(COMDT_TGWINFO.CLASS_ID);
			this.stDeskInfo = (COMDT_OB_DESK)ProtocolObjectPool.Get(COMDT_OB_DESK.CLASS_ID);
		}
	}
}
