using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSPKG_CMD_ASKFORREQ_SEND : ProtocolObject
	{
		public COMDT_ACNT_UNIQ stAskforUniq;

		public COMDT_ITEM_SIMPINFO stItemInfo;

		public COMDT_ACNT_ASKFORMSG_INFO stMsgInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1140;

		public CSPKG_CMD_ASKFORREQ_SEND()
		{
			this.stAskforUniq = (COMDT_ACNT_UNIQ)ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID);
			this.stItemInfo = (COMDT_ITEM_SIMPINFO)ProtocolObjectPool.Get(COMDT_ITEM_SIMPINFO.CLASS_ID);
			this.stMsgInfo = (COMDT_ACNT_ASKFORMSG_INFO)ProtocolObjectPool.Get(COMDT_ACNT_ASKFORMSG_INFO.CLASS_ID);
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
			if (cutVer == 0u || CSPKG_CMD_ASKFORREQ_SEND.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_CMD_ASKFORREQ_SEND.CURRVERSION;
			}
			if (CSPKG_CMD_ASKFORREQ_SEND.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stAskforUniq.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stItemInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMsgInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSPKG_CMD_ASKFORREQ_SEND.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_CMD_ASKFORREQ_SEND.CURRVERSION;
			}
			if (CSPKG_CMD_ASKFORREQ_SEND.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stAskforUniq.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stItemInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMsgInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSPKG_CMD_ASKFORREQ_SEND.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stAskforUniq != null)
			{
				this.stAskforUniq.Release();
				this.stAskforUniq = null;
			}
			if (this.stItemInfo != null)
			{
				this.stItemInfo.Release();
				this.stItemInfo = null;
			}
			if (this.stMsgInfo != null)
			{
				this.stMsgInfo.Release();
				this.stMsgInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stAskforUniq = (COMDT_ACNT_UNIQ)ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID);
			this.stItemInfo = (COMDT_ITEM_SIMPINFO)ProtocolObjectPool.Get(COMDT_ITEM_SIMPINFO.CLASS_ID);
			this.stMsgInfo = (COMDT_ACNT_ASKFORMSG_INFO)ProtocolObjectPool.Get(COMDT_ACNT_ASKFORMSG_INFO.CLASS_ID);
		}
	}
}
