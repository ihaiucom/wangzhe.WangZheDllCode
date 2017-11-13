using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_BURNING_HERO_DETAIL : ProtocolObject
	{
		public ushort wHeroNum;

		public COMDT_BURNING_HERO_INFO[] astHeroList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 351;

		public COMDT_BURNING_HERO_DETAIL()
		{
			this.astHeroList = new COMDT_BURNING_HERO_INFO[200];
			for (int i = 0; i < 200; i++)
			{
				this.astHeroList[i] = (COMDT_BURNING_HERO_INFO)ProtocolObjectPool.Get(COMDT_BURNING_HERO_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_BURNING_HERO_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_BURNING_HERO_DETAIL.CURRVERSION;
			}
			if (COMDT_BURNING_HERO_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt16(this.wHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (200 < this.wHeroNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astHeroList.Length < (int)this.wHeroNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wHeroNum; i++)
			{
				errorType = this.astHeroList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_BURNING_HERO_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_BURNING_HERO_DETAIL.CURRVERSION;
			}
			if (COMDT_BURNING_HERO_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (200 < this.wHeroNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wHeroNum; i++)
			{
				errorType = this.astHeroList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_BURNING_HERO_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wHeroNum = 0;
			if (this.astHeroList != null)
			{
				for (int i = 0; i < this.astHeroList.Length; i++)
				{
					if (this.astHeroList[i] != null)
					{
						this.astHeroList[i].Release();
						this.astHeroList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astHeroList != null)
			{
				for (int i = 0; i < this.astHeroList.Length; i++)
				{
					this.astHeroList[i] = (COMDT_BURNING_HERO_INFO)ProtocolObjectPool.Get(COMDT_BURNING_HERO_INFO.CLASS_ID);
				}
			}
		}
	}
}
