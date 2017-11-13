using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_STATISTIC_KEY_VALUE_DETAIL : ProtocolObject
	{
		public uint dwNum;

		public COMDT_STATISTIC_KEY_VALUE_INFO[] astKVDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 498;

		public COMDT_STATISTIC_KEY_VALUE_DETAIL()
		{
			this.astKVDetail = new COMDT_STATISTIC_KEY_VALUE_INFO[50];
			for (int i = 0; i < 50; i++)
			{
				this.astKVDetail[i] = (COMDT_STATISTIC_KEY_VALUE_INFO)ProtocolObjectPool.Get(COMDT_STATISTIC_KEY_VALUE_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_STATISTIC_KEY_VALUE_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_STATISTIC_KEY_VALUE_DETAIL.CURRVERSION;
			}
			if (COMDT_STATISTIC_KEY_VALUE_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (50u < this.dwNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astKVDetail.Length < (long)((ulong)this.dwNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwNum))
			{
				errorType = this.astKVDetail[num].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
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
			if (cutVer == 0u || COMDT_STATISTIC_KEY_VALUE_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_STATISTIC_KEY_VALUE_DETAIL.CURRVERSION;
			}
			if (COMDT_STATISTIC_KEY_VALUE_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (50u < this.dwNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwNum))
			{
				errorType = this.astKVDetail[num].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_STATISTIC_KEY_VALUE_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwNum = 0u;
			if (this.astKVDetail != null)
			{
				for (int i = 0; i < this.astKVDetail.Length; i++)
				{
					if (this.astKVDetail[i] != null)
					{
						this.astKVDetail[i].Release();
						this.astKVDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astKVDetail != null)
			{
				for (int i = 0; i < this.astKVDetail.Length; i++)
				{
					this.astKVDetail[i] = (COMDT_STATISTIC_KEY_VALUE_INFO)ProtocolObjectPool.Get(COMDT_STATISTIC_KEY_VALUE_INFO.CLASS_ID);
				}
			}
		}
	}
}
