using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCDT_SETBATTLELISTOFARENA_SUCC : ProtocolObject
	{
		public COMDT_BATTLEHERO stBattleList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1369;

		public SCDT_SETBATTLELISTOFARENA_SUCC()
		{
			this.stBattleList = (COMDT_BATTLEHERO)ProtocolObjectPool.Get(COMDT_BATTLEHERO.CLASS_ID);
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
			if (cutVer == 0u || SCDT_SETBATTLELISTOFARENA_SUCC.CURRVERSION < cutVer)
			{
				cutVer = SCDT_SETBATTLELISTOFARENA_SUCC.CURRVERSION;
			}
			if (SCDT_SETBATTLELISTOFARENA_SUCC.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stBattleList.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (cutVer == 0u || SCDT_SETBATTLELISTOFARENA_SUCC.CURRVERSION < cutVer)
			{
				cutVer = SCDT_SETBATTLELISTOFARENA_SUCC.CURRVERSION;
			}
			if (SCDT_SETBATTLELISTOFARENA_SUCC.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stBattleList.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCDT_SETBATTLELISTOFARENA_SUCC.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stBattleList != null)
			{
				this.stBattleList.Release();
				this.stBattleList = null;
			}
		}

		public override void OnUse()
		{
			this.stBattleList = (COMDT_BATTLEHERO)ProtocolObjectPool.Get(COMDT_BATTLEHERO.CLASS_ID);
		}
	}
}
