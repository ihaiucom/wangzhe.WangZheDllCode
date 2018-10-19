using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_RECONN_CAMPVOICEINFO : ProtocolObject
	{
		public byte bPlayerNum;

		public CSDT_RECONN_PLAYERVOICEINFO[] astPlayerVoiceState;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1286;

		public CSDT_RECONN_CAMPVOICEINFO()
		{
			this.astPlayerVoiceState = new CSDT_RECONN_PLAYERVOICEINFO[5];
			for (int i = 0; i < 5; i++)
			{
				this.astPlayerVoiceState[i] = (CSDT_RECONN_PLAYERVOICEINFO)ProtocolObjectPool.Get(CSDT_RECONN_PLAYERVOICEINFO.CLASS_ID);
			}
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
			if (cutVer == 0u || CSDT_RECONN_CAMPVOICEINFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_RECONN_CAMPVOICEINFO.CURRVERSION;
			}
			if (CSDT_RECONN_CAMPVOICEINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bPlayerNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5 < this.bPlayerNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astPlayerVoiceState.Length < (int)this.bPlayerNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bPlayerNum; i++)
			{
				errorType = this.astPlayerVoiceState[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_RECONN_CAMPVOICEINFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_RECONN_CAMPVOICEINFO.CURRVERSION;
			}
			if (CSDT_RECONN_CAMPVOICEINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bPlayerNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5 < this.bPlayerNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bPlayerNum; i++)
			{
				errorType = this.astPlayerVoiceState[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_RECONN_CAMPVOICEINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bPlayerNum = 0;
			if (this.astPlayerVoiceState != null)
			{
				for (int i = 0; i < this.astPlayerVoiceState.Length; i++)
				{
					if (this.astPlayerVoiceState[i] != null)
					{
						this.astPlayerVoiceState[i].Release();
						this.astPlayerVoiceState[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astPlayerVoiceState != null)
			{
				for (int i = 0; i < this.astPlayerVoiceState.Length; i++)
				{
					this.astPlayerVoiceState[i] = (CSDT_RECONN_PLAYERVOICEINFO)ProtocolObjectPool.Get(CSDT_RECONN_PLAYERVOICEINFO.CLASS_ID);
				}
			}
		}
	}
}
