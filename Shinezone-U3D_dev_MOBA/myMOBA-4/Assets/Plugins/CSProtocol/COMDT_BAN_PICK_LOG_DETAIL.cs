using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_BAN_PICK_LOG_DETAIL : ProtocolObject
	{
		public byte bRoundNum;

		public COMDT_BAN_PICK_LOG_INFO[] astRoundDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 202;

		public COMDT_BAN_PICK_LOG_DETAIL()
		{
			this.astRoundDetail = new COMDT_BAN_PICK_LOG_INFO[20];
			for (int i = 0; i < 20; i++)
			{
				this.astRoundDetail[i] = (COMDT_BAN_PICK_LOG_INFO)ProtocolObjectPool.Get(COMDT_BAN_PICK_LOG_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_BAN_PICK_LOG_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_BAN_PICK_LOG_DETAIL.CURRVERSION;
			}
			if (COMDT_BAN_PICK_LOG_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bRoundNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bRoundNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astRoundDetail.Length < (int)this.bRoundNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bRoundNum; i++)
			{
				errorType = this.astRoundDetail[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_BAN_PICK_LOG_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_BAN_PICK_LOG_DETAIL.CURRVERSION;
			}
			if (COMDT_BAN_PICK_LOG_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bRoundNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bRoundNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bRoundNum; i++)
			{
				errorType = this.astRoundDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_BAN_PICK_LOG_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bRoundNum = 0;
			if (this.astRoundDetail != null)
			{
				for (int i = 0; i < this.astRoundDetail.Length; i++)
				{
					if (this.astRoundDetail[i] != null)
					{
						this.astRoundDetail[i].Release();
						this.astRoundDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astRoundDetail != null)
			{
				for (int i = 0; i < this.astRoundDetail.Length; i++)
				{
					this.astRoundDetail[i] = (COMDT_BAN_PICK_LOG_INFO)ProtocolObjectPool.Get(COMDT_BAN_PICK_LOG_INFO.CLASS_ID);
				}
			}
		}
	}
}
