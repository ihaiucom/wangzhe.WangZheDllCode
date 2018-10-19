using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_MULTI_GAME_SETTLE_NORMAL : ProtocolObject
	{
		public COMDT_ACNT_SETTLE_RANK_INFO stAcntInfo;

		public COMDT_SETTLE_COMMON_DATA stCommonData;

		public COMDT_CLIENT_RECORD_DATA stClientRecordData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 194;

		public COMDT_MULTI_GAME_SETTLE_NORMAL()
		{
			this.stAcntInfo = (COMDT_ACNT_SETTLE_RANK_INFO)ProtocolObjectPool.Get(COMDT_ACNT_SETTLE_RANK_INFO.CLASS_ID);
			this.stCommonData = (COMDT_SETTLE_COMMON_DATA)ProtocolObjectPool.Get(COMDT_SETTLE_COMMON_DATA.CLASS_ID);
			this.stClientRecordData = (COMDT_CLIENT_RECORD_DATA)ProtocolObjectPool.Get(COMDT_CLIENT_RECORD_DATA.CLASS_ID);
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
			if (cutVer == 0u || COMDT_MULTI_GAME_SETTLE_NORMAL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MULTI_GAME_SETTLE_NORMAL.CURRVERSION;
			}
			if (COMDT_MULTI_GAME_SETTLE_NORMAL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stAcntInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stCommonData.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stClientRecordData.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_MULTI_GAME_SETTLE_NORMAL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MULTI_GAME_SETTLE_NORMAL.CURRVERSION;
			}
			if (COMDT_MULTI_GAME_SETTLE_NORMAL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stAcntInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stCommonData.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stClientRecordData.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_MULTI_GAME_SETTLE_NORMAL.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stAcntInfo != null)
			{
				this.stAcntInfo.Release();
				this.stAcntInfo = null;
			}
			if (this.stCommonData != null)
			{
				this.stCommonData.Release();
				this.stCommonData = null;
			}
			if (this.stClientRecordData != null)
			{
				this.stClientRecordData.Release();
				this.stClientRecordData = null;
			}
		}

		public override void OnUse()
		{
			this.stAcntInfo = (COMDT_ACNT_SETTLE_RANK_INFO)ProtocolObjectPool.Get(COMDT_ACNT_SETTLE_RANK_INFO.CLASS_ID);
			this.stCommonData = (COMDT_SETTLE_COMMON_DATA)ProtocolObjectPool.Get(COMDT_SETTLE_COMMON_DATA.CLASS_ID);
			this.stClientRecordData = (COMDT_CLIENT_RECORD_DATA)ProtocolObjectPool.Get(COMDT_CLIENT_RECORD_DATA.CLASS_ID);
		}
	}
}
