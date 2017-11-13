using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_MULTIGAMEOVER_INFO : ProtocolObject
	{
		public uint dwAcntObjID;

		public COMDT_MULTI_GAME_PARAM stBattleParam;

		public COMDT_SETTLE_COMMON_DATA stCommonData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 784;

		public CSDT_MULTIGAMEOVER_INFO()
		{
			this.stBattleParam = (COMDT_MULTI_GAME_PARAM)ProtocolObjectPool.Get(COMDT_MULTI_GAME_PARAM.CLASS_ID);
			this.stCommonData = (COMDT_SETTLE_COMMON_DATA)ProtocolObjectPool.Get(COMDT_SETTLE_COMMON_DATA.CLASS_ID);
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
			if (cutVer == 0u || CSDT_MULTIGAMEOVER_INFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_MULTIGAMEOVER_INFO.CURRVERSION;
			}
			if (CSDT_MULTIGAMEOVER_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwAcntObjID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBattleParam.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stCommonData.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_MULTIGAMEOVER_INFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_MULTIGAMEOVER_INFO.CURRVERSION;
			}
			if (CSDT_MULTIGAMEOVER_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwAcntObjID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBattleParam.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stCommonData.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_MULTIGAMEOVER_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwAcntObjID = 0u;
			if (this.stBattleParam != null)
			{
				this.stBattleParam.Release();
				this.stBattleParam = null;
			}
			if (this.stCommonData != null)
			{
				this.stCommonData.Release();
				this.stCommonData = null;
			}
		}

		public override void OnUse()
		{
			this.stBattleParam = (COMDT_MULTI_GAME_PARAM)ProtocolObjectPool.Get(COMDT_MULTI_GAME_PARAM.CLASS_ID);
			this.stCommonData = (COMDT_SETTLE_COMMON_DATA)ProtocolObjectPool.Get(COMDT_SETTLE_COMMON_DATA.CLASS_ID);
		}
	}
}
