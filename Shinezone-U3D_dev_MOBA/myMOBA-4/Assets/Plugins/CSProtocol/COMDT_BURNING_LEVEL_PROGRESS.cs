using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_BURNING_LEVEL_PROGRESS : ProtocolObject
	{
		public byte bDiffNum;

		public COMDT_BURNING_LEVEL_DETAIL[] astDiffDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 353;

		public COMDT_BURNING_LEVEL_PROGRESS()
		{
			this.astDiffDetail = new COMDT_BURNING_LEVEL_DETAIL[2];
			for (int i = 0; i < 2; i++)
			{
				this.astDiffDetail[i] = (COMDT_BURNING_LEVEL_DETAIL)ProtocolObjectPool.Get(COMDT_BURNING_LEVEL_DETAIL.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			this.bDiffNum = 0;
			for (int i = 0; i < 2; i++)
			{
				errorType = this.astDiffDetail[i].construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
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
			if (cutVer == 0u || COMDT_BURNING_LEVEL_PROGRESS.CURRVERSION < cutVer)
			{
				cutVer = COMDT_BURNING_LEVEL_PROGRESS.CURRVERSION;
			}
			if (COMDT_BURNING_LEVEL_PROGRESS.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bDiffNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (2 < this.bDiffNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astDiffDetail.Length < (int)this.bDiffNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bDiffNum; i++)
			{
				errorType = this.astDiffDetail[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_BURNING_LEVEL_PROGRESS.CURRVERSION < cutVer)
			{
				cutVer = COMDT_BURNING_LEVEL_PROGRESS.CURRVERSION;
			}
			if (COMDT_BURNING_LEVEL_PROGRESS.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bDiffNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (2 < this.bDiffNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bDiffNum; i++)
			{
				errorType = this.astDiffDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_BURNING_LEVEL_PROGRESS.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bDiffNum = 0;
			if (this.astDiffDetail != null)
			{
				for (int i = 0; i < this.astDiffDetail.Length; i++)
				{
					if (this.astDiffDetail[i] != null)
					{
						this.astDiffDetail[i].Release();
						this.astDiffDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astDiffDetail != null)
			{
				for (int i = 0; i < this.astDiffDetail.Length; i++)
				{
					this.astDiffDetail[i] = (COMDT_BURNING_LEVEL_DETAIL)ProtocolObjectPool.Get(COMDT_BURNING_LEVEL_DETAIL.CLASS_ID);
				}
			}
		}
	}
}
