using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ARENA_HEROINFO : ProtocolObject
	{
		public COMDT_ARENA_HERODETAIL[] astHero;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 413;

		public COMDT_ARENA_HEROINFO()
		{
			this.astHero = new COMDT_ARENA_HERODETAIL[3];
			for (int i = 0; i < 3; i++)
			{
				this.astHero[i] = (COMDT_ARENA_HERODETAIL)ProtocolObjectPool.Get(COMDT_ARENA_HERODETAIL.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			for (int i = 0; i < 3; i++)
			{
				errorType = this.astHero[i].construct();
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
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			if (cutVer == 0u || COMDT_ARENA_HEROINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ARENA_HEROINFO.CURRVERSION;
			}
			if (COMDT_ARENA_HEROINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = this.astHero[i].pack(ref destBuf, cutVer);
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
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			if (cutVer == 0u || COMDT_ARENA_HEROINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ARENA_HEROINFO.CURRVERSION;
			}
			if (COMDT_ARENA_HEROINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = this.astHero[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ARENA_HEROINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.astHero != null)
			{
				for (int i = 0; i < this.astHero.Length; i++)
				{
					if (this.astHero[i] != null)
					{
						this.astHero[i].Release();
						this.astHero[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astHero != null)
			{
				for (int i = 0; i < this.astHero.Length; i++)
				{
					this.astHero[i] = (COMDT_ARENA_HERODETAIL)ProtocolObjectPool.Get(COMDT_ARENA_HERODETAIL.CLASS_ID);
				}
			}
		}
	}
}
