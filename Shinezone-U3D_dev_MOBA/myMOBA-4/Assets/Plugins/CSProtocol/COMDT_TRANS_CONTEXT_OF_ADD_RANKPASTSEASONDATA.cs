using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA : ProtocolObject
	{
		public COMDT_RANK_PASTSEASON_FIGHT_RECORD stPastSeasonRecord;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 645;

		public COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA()
		{
			this.stPastSeasonRecord = (COMDT_RANK_PASTSEASON_FIGHT_RECORD)ProtocolObjectPool.Get(COMDT_RANK_PASTSEASON_FIGHT_RECORD.CLASS_ID);
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
			if (cutVer == 0u || COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA.CURRVERSION;
			}
			if (COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stPastSeasonRecord.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA.CURRVERSION;
			}
			if (COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stPastSeasonRecord.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stPastSeasonRecord != null)
			{
				this.stPastSeasonRecord.Release();
				this.stPastSeasonRecord = null;
			}
		}

		public override void OnUse()
		{
			this.stPastSeasonRecord = (COMDT_RANK_PASTSEASON_FIGHT_RECORD)ProtocolObjectPool.Get(COMDT_RANK_PASTSEASON_FIGHT_RECORD.CLASS_ID);
		}
	}
}
