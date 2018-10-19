using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_FREEHERO_INACNT : ProtocolObject
	{
		public byte bHeroCnt;

		public COMDT_FREEHERO_INFO[] astHeroInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 29u;

		public static readonly int CLASS_ID = 116;

		public COMDT_FREEHERO_INACNT()
		{
			this.astHeroInfo = new COMDT_FREEHERO_INFO[50];
			for (int i = 0; i < 50; i++)
			{
				this.astHeroInfo[i] = (COMDT_FREEHERO_INFO)ProtocolObjectPool.Get(COMDT_FREEHERO_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_FREEHERO_INACNT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_FREEHERO_INACNT.CURRVERSION;
			}
			if (COMDT_FREEHERO_INACNT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bHeroCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (50 < this.bHeroCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astHeroInfo.Length < (int)this.bHeroCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bHeroCnt; i++)
			{
				errorType = this.astHeroInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_FREEHERO_INACNT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_FREEHERO_INACNT.CURRVERSION;
			}
			if (COMDT_FREEHERO_INACNT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bHeroCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (50 < this.bHeroCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bHeroCnt; i++)
			{
				errorType = this.astHeroInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_FREEHERO_INACNT.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bHeroCnt = 0;
			if (this.astHeroInfo != null)
			{
				for (int i = 0; i < this.astHeroInfo.Length; i++)
				{
					if (this.astHeroInfo[i] != null)
					{
						this.astHeroInfo[i].Release();
						this.astHeroInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astHeroInfo != null)
			{
				for (int i = 0; i < this.astHeroInfo.Length; i++)
				{
					this.astHeroInfo[i] = (COMDT_FREEHERO_INFO)ProtocolObjectPool.Get(COMDT_FREEHERO_INFO.CLASS_ID);
				}
			}
		}
	}
}
