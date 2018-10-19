using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_PROFIT_LIMIT : ProtocolObject
	{
		public uint dwProfitNum;

		public COMDT_PROFIT_LIMIT_DETAIL[] astProfitUnion;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 635;

		public COMDT_PROFIT_LIMIT()
		{
			this.astProfitUnion = new COMDT_PROFIT_LIMIT_DETAIL[2];
			for (int i = 0; i < 2; i++)
			{
				this.astProfitUnion[i] = (COMDT_PROFIT_LIMIT_DETAIL)ProtocolObjectPool.Get(COMDT_PROFIT_LIMIT_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_PROFIT_LIMIT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PROFIT_LIMIT.CURRVERSION;
			}
			if (COMDT_PROFIT_LIMIT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwProfitNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (2u < this.dwProfitNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astProfitUnion.Length < (long)((ulong)this.dwProfitNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwProfitNum))
			{
				errorType = this.astProfitUnion[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_PROFIT_LIMIT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PROFIT_LIMIT.CURRVERSION;
			}
			if (COMDT_PROFIT_LIMIT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwProfitNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (2u < this.dwProfitNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwProfitNum))
			{
				errorType = this.astProfitUnion[num].unpack(ref srcBuf, cutVer);
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
			return COMDT_PROFIT_LIMIT.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwProfitNum = 0u;
			if (this.astProfitUnion != null)
			{
				for (int i = 0; i < this.astProfitUnion.Length; i++)
				{
					if (this.astProfitUnion[i] != null)
					{
						this.astProfitUnion[i].Release();
						this.astProfitUnion[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astProfitUnion != null)
			{
				for (int i = 0; i < this.astProfitUnion.Length; i++)
				{
					this.astProfitUnion[i] = (COMDT_PROFIT_LIMIT_DETAIL)ProtocolObjectPool.Get(COMDT_PROFIT_LIMIT_DETAIL.CLASS_ID);
				}
			}
		}
	}
}
