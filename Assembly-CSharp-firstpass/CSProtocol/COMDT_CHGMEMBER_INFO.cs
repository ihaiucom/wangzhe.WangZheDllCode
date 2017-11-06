using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_CHGMEMBER_INFO : ProtocolObject
	{
		public COMDT_ROOMMEMBER_BRIF stSender;

		public COMDT_ROOMMEMBER_BRIF stReceiver;

		public uint dwChgPosSeq;

		public uint dwTimeOutSec;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 54;

		public COMDT_CHGMEMBER_INFO()
		{
			this.stSender = (COMDT_ROOMMEMBER_BRIF)ProtocolObjectPool.Get(COMDT_ROOMMEMBER_BRIF.CLASS_ID);
			this.stReceiver = (COMDT_ROOMMEMBER_BRIF)ProtocolObjectPool.Get(COMDT_ROOMMEMBER_BRIF.CLASS_ID);
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
			if (cutVer == 0u || COMDT_CHGMEMBER_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_CHGMEMBER_INFO.CURRVERSION;
			}
			if (COMDT_CHGMEMBER_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stSender.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stReceiver.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwChgPosSeq);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTimeOutSec);
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
			if (cutVer == 0u || COMDT_CHGMEMBER_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_CHGMEMBER_INFO.CURRVERSION;
			}
			if (COMDT_CHGMEMBER_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stSender.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stReceiver.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwChgPosSeq);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTimeOutSec);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_CHGMEMBER_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stSender != null)
			{
				this.stSender.Release();
				this.stSender = null;
			}
			if (this.stReceiver != null)
			{
				this.stReceiver.Release();
				this.stReceiver = null;
			}
			this.dwChgPosSeq = 0u;
			this.dwTimeOutSec = 0u;
		}

		public override void OnUse()
		{
			this.stSender = (COMDT_ROOMMEMBER_BRIF)ProtocolObjectPool.Get(COMDT_ROOMMEMBER_BRIF.CLASS_ID);
			this.stReceiver = (COMDT_ROOMMEMBER_BRIF)ProtocolObjectPool.Get(COMDT_ROOMMEMBER_BRIF.CLASS_ID);
		}
	}
}
