using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACNT_LEVEL_COMPLETE_DETAIL : ProtocolObject
	{
		public byte bLastOpenDiffType;

		public byte bChapterNum;

		public COMDT_CHAPTER_COMPLETE_INFO[] astChapterDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 142;

		public COMDT_ACNT_LEVEL_COMPLETE_DETAIL()
		{
			this.astChapterDetail = new COMDT_CHAPTER_COMPLETE_INFO[10];
			for (int i = 0; i < 10; i++)
			{
				this.astChapterDetail[i] = (COMDT_CHAPTER_COMPLETE_INFO)ProtocolObjectPool.Get(COMDT_CHAPTER_COMPLETE_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ACNT_LEVEL_COMPLETE_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_LEVEL_COMPLETE_DETAIL.CURRVERSION;
			}
			if (COMDT_ACNT_LEVEL_COMPLETE_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bLastOpenDiffType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bChapterNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bChapterNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astChapterDetail.Length < (int)this.bChapterNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bChapterNum; i++)
			{
				errorType = this.astChapterDetail[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ACNT_LEVEL_COMPLETE_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_LEVEL_COMPLETE_DETAIL.CURRVERSION;
			}
			if (COMDT_ACNT_LEVEL_COMPLETE_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bLastOpenDiffType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bChapterNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bChapterNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bChapterNum; i++)
			{
				errorType = this.astChapterDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACNT_LEVEL_COMPLETE_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bLastOpenDiffType = 0;
			this.bChapterNum = 0;
			if (this.astChapterDetail != null)
			{
				for (int i = 0; i < this.astChapterDetail.Length; i++)
				{
					if (this.astChapterDetail[i] != null)
					{
						this.astChapterDetail[i].Release();
						this.astChapterDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astChapterDetail != null)
			{
				for (int i = 0; i < this.astChapterDetail.Length; i++)
				{
					this.astChapterDetail[i] = (COMDT_CHAPTER_COMPLETE_INFO)ProtocolObjectPool.Get(COMDT_CHAPTER_COMPLETE_INFO.CLASS_ID);
				}
			}
		}
	}
}
