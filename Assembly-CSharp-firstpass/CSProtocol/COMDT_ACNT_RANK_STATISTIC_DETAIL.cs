using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACNT_RANK_STATISTIC_DETAIL : ProtocolObject
	{
		public uint dwFirstPlayTime;

		public byte bNum;

		public COMDT_ACNT_RANK_STATISTIC_INFO[] astDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 615;

		public COMDT_ACNT_RANK_STATISTIC_DETAIL()
		{
			this.astDetail = new COMDT_ACNT_RANK_STATISTIC_INFO[15];
			for (int i = 0; i < 15; i++)
			{
				this.astDetail[i] = (COMDT_ACNT_RANK_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_ACNT_RANK_STATISTIC_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ACNT_RANK_STATISTIC_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_RANK_STATISTIC_DETAIL.CURRVERSION;
			}
			if (COMDT_ACNT_RANK_STATISTIC_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwFirstPlayTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (15 < this.bNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astDetail.Length < (int)this.bNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bNum; i++)
			{
				errorType = this.astDetail[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ACNT_RANK_STATISTIC_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_RANK_STATISTIC_DETAIL.CURRVERSION;
			}
			if (COMDT_ACNT_RANK_STATISTIC_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwFirstPlayTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (15 < this.bNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bNum; i++)
			{
				errorType = this.astDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACNT_RANK_STATISTIC_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwFirstPlayTime = 0u;
			this.bNum = 0;
			if (this.astDetail != null)
			{
				for (int i = 0; i < this.astDetail.Length; i++)
				{
					if (this.astDetail[i] != null)
					{
						this.astDetail[i].Release();
						this.astDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astDetail != null)
			{
				for (int i = 0; i < this.astDetail.Length; i++)
				{
					this.astDetail[i] = (COMDT_ACNT_RANK_STATISTIC_INFO)ProtocolObjectPool.Get(COMDT_ACNT_RANK_STATISTIC_INFO.CLASS_ID);
				}
			}
		}
	}
}
