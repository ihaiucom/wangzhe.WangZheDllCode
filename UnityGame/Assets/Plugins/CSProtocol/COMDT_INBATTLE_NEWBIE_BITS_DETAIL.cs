using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_INBATTLE_NEWBIE_BITS_DETAIL : ProtocolObject
	{
		public byte bLevelNum;

		public COMDT_INBATTLE_NEWBIE_BITS_INFO[] astLevelDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 157u;

		public static readonly int CLASS_ID = 574;

		public COMDT_INBATTLE_NEWBIE_BITS_DETAIL()
		{
			this.astLevelDetail = new COMDT_INBATTLE_NEWBIE_BITS_INFO[10];
			for (int i = 0; i < 10; i++)
			{
				this.astLevelDetail[i] = (COMDT_INBATTLE_NEWBIE_BITS_INFO)ProtocolObjectPool.Get(COMDT_INBATTLE_NEWBIE_BITS_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_INBATTLE_NEWBIE_BITS_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_INBATTLE_NEWBIE_BITS_DETAIL.CURRVERSION;
			}
			if (COMDT_INBATTLE_NEWBIE_BITS_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bLevelNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bLevelNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astLevelDetail.Length < (int)this.bLevelNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bLevelNum; i++)
			{
				errorType = this.astLevelDetail[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_INBATTLE_NEWBIE_BITS_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_INBATTLE_NEWBIE_BITS_DETAIL.CURRVERSION;
			}
			if (COMDT_INBATTLE_NEWBIE_BITS_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bLevelNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bLevelNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bLevelNum; i++)
			{
				errorType = this.astLevelDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_INBATTLE_NEWBIE_BITS_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bLevelNum = 0;
			if (this.astLevelDetail != null)
			{
				for (int i = 0; i < this.astLevelDetail.Length; i++)
				{
					if (this.astLevelDetail[i] != null)
					{
						this.astLevelDetail[i].Release();
						this.astLevelDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astLevelDetail != null)
			{
				for (int i = 0; i < this.astLevelDetail.Length; i++)
				{
					this.astLevelDetail[i] = (COMDT_INBATTLE_NEWBIE_BITS_INFO)ProtocolObjectPool.Get(COMDT_INBATTLE_NEWBIE_BITS_INFO.CLASS_ID);
				}
			}
		}
	}
}
