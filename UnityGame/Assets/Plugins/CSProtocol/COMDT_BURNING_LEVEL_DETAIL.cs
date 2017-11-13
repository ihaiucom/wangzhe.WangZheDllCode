using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_BURNING_LEVEL_DETAIL : ProtocolObject
	{
		public byte bDifficultType;

		public uint dwLastPlayTime;

		public byte bResetNum;

		public COMDT_BURNING_HERO_DETAIL stHeroDetail;

		public byte bLevelNum;

		public COMDT_BURNING_LEVEL_INFO[] astLevelDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 352;

		public COMDT_BURNING_LEVEL_DETAIL()
		{
			this.stHeroDetail = (COMDT_BURNING_HERO_DETAIL)ProtocolObjectPool.Get(COMDT_BURNING_HERO_DETAIL.CLASS_ID);
			this.astLevelDetail = new COMDT_BURNING_LEVEL_INFO[10];
			for (int i = 0; i < 10; i++)
			{
				this.astLevelDetail[i] = (COMDT_BURNING_LEVEL_INFO)ProtocolObjectPool.Get(COMDT_BURNING_LEVEL_INFO.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			this.bDifficultType = 0;
			this.dwLastPlayTime = 0u;
			this.bResetNum = 0;
			TdrError.ErrorType errorType = this.stHeroDetail.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.bLevelNum = 0;
			for (int i = 0; i < 10; i++)
			{
				errorType = this.astLevelDetail[i].construct();
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
			if (cutVer == 0u || COMDT_BURNING_LEVEL_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_BURNING_LEVEL_DETAIL.CURRVERSION;
			}
			if (COMDT_BURNING_LEVEL_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bDifficultType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwLastPlayTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bResetNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroDetail.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bLevelNum);
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
			if (cutVer == 0u || COMDT_BURNING_LEVEL_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_BURNING_LEVEL_DETAIL.CURRVERSION;
			}
			if (COMDT_BURNING_LEVEL_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bDifficultType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwLastPlayTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bResetNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroDetail.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bLevelNum);
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
			return COMDT_BURNING_LEVEL_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bDifficultType = 0;
			this.dwLastPlayTime = 0u;
			this.bResetNum = 0;
			if (this.stHeroDetail != null)
			{
				this.stHeroDetail.Release();
				this.stHeroDetail = null;
			}
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
			this.stHeroDetail = (COMDT_BURNING_HERO_DETAIL)ProtocolObjectPool.Get(COMDT_BURNING_HERO_DETAIL.CLASS_ID);
			if (this.astLevelDetail != null)
			{
				for (int i = 0; i < this.astLevelDetail.Length; i++)
				{
					this.astLevelDetail[i] = (COMDT_BURNING_LEVEL_INFO)ProtocolObjectPool.Get(COMDT_BURNING_LEVEL_INFO.CLASS_ID);
				}
			}
		}
	}
}
