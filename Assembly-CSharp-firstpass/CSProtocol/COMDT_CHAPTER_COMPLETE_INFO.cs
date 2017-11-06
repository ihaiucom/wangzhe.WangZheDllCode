using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_CHAPTER_COMPLETE_INFO : ProtocolObject
	{
		public byte bLevelNum;

		public COMDT_LEVEL_COMPLETE_INFO[] astLevelDetail;

		public byte bDiffNum;

		public COMDT_CHAPTER_DIFFICULT_INFO[] astDiffDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 141;

		public COMDT_CHAPTER_COMPLETE_INFO()
		{
			this.astLevelDetail = new COMDT_LEVEL_COMPLETE_INFO[4];
			for (int i = 0; i < 4; i++)
			{
				this.astLevelDetail[i] = (COMDT_LEVEL_COMPLETE_INFO)ProtocolObjectPool.Get(COMDT_LEVEL_COMPLETE_INFO.CLASS_ID);
			}
			this.astDiffDetail = new COMDT_CHAPTER_DIFFICULT_INFO[4];
			for (int j = 0; j < 4; j++)
			{
				this.astDiffDetail[j] = (COMDT_CHAPTER_DIFFICULT_INFO)ProtocolObjectPool.Get(COMDT_CHAPTER_DIFFICULT_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_CHAPTER_COMPLETE_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_CHAPTER_COMPLETE_INFO.CURRVERSION;
			}
			if (COMDT_CHAPTER_COMPLETE_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bLevelNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bLevelNum)
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
			errorType = destBuf.writeUInt8(this.bDiffNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bDiffNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astDiffDetail.Length < (int)this.bDiffNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int j = 0; j < (int)this.bDiffNum; j++)
			{
				errorType = this.astDiffDetail[j].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_CHAPTER_COMPLETE_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_CHAPTER_COMPLETE_INFO.CURRVERSION;
			}
			if (COMDT_CHAPTER_COMPLETE_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bLevelNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bLevelNum)
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
			errorType = srcBuf.readUInt8(ref this.bDiffNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bDiffNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int j = 0; j < (int)this.bDiffNum; j++)
			{
				errorType = this.astDiffDetail[j].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_CHAPTER_COMPLETE_INFO.CLASS_ID;
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
			this.bDiffNum = 0;
			if (this.astDiffDetail != null)
			{
				for (int j = 0; j < this.astDiffDetail.Length; j++)
				{
					if (this.astDiffDetail[j] != null)
					{
						this.astDiffDetail[j].Release();
						this.astDiffDetail[j] = null;
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
					this.astLevelDetail[i] = (COMDT_LEVEL_COMPLETE_INFO)ProtocolObjectPool.Get(COMDT_LEVEL_COMPLETE_INFO.CLASS_ID);
				}
			}
			if (this.astDiffDetail != null)
			{
				for (int j = 0; j < this.astDiffDetail.Length; j++)
				{
					this.astDiffDetail[j] = (COMDT_CHAPTER_DIFFICULT_INFO)ProtocolObjectPool.Get(COMDT_CHAPTER_DIFFICULT_INFO.CLASS_ID);
				}
			}
		}
	}
}
