using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACNT_ACTIVITY_INFO : ProtocolObject
	{
		public ushort wActivityCnt;

		public COMDT_ACTIVITY_DETAIL[] astActivityDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 148;

		public COMDT_ACNT_ACTIVITY_INFO()
		{
			this.astActivityDetail = new COMDT_ACTIVITY_DETAIL[30];
			for (int i = 0; i < 30; i++)
			{
				this.astActivityDetail[i] = (COMDT_ACTIVITY_DETAIL)ProtocolObjectPool.Get(COMDT_ACTIVITY_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ACNT_ACTIVITY_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_ACTIVITY_INFO.CURRVERSION;
			}
			if (COMDT_ACNT_ACTIVITY_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt16(this.wActivityCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (30 < this.wActivityCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astActivityDetail.Length < (int)this.wActivityCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wActivityCnt; i++)
			{
				errorType = this.astActivityDetail[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ACNT_ACTIVITY_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_ACTIVITY_INFO.CURRVERSION;
			}
			if (COMDT_ACNT_ACTIVITY_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wActivityCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (30 < this.wActivityCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wActivityCnt; i++)
			{
				errorType = this.astActivityDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACNT_ACTIVITY_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wActivityCnt = 0;
			if (this.astActivityDetail != null)
			{
				for (int i = 0; i < this.astActivityDetail.Length; i++)
				{
					if (this.astActivityDetail[i] != null)
					{
						this.astActivityDetail[i].Release();
						this.astActivityDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astActivityDetail != null)
			{
				for (int i = 0; i < this.astActivityDetail.Length; i++)
				{
					this.astActivityDetail[i] = (COMDT_ACTIVITY_DETAIL)ProtocolObjectPool.Get(COMDT_ACTIVITY_DETAIL.CLASS_ID);
				}
			}
		}
	}
}
