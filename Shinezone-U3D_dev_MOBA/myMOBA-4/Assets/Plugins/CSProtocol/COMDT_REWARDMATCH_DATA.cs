using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_REWARDMATCH_DATA : ProtocolObject
	{
		public byte bRecordCnt;

		public COMDT_REWARDMATCH_RECORD[] astRecord;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 610;

		public COMDT_REWARDMATCH_DATA()
		{
			this.astRecord = new COMDT_REWARDMATCH_RECORD[4];
			for (int i = 0; i < 4; i++)
			{
				this.astRecord[i] = (COMDT_REWARDMATCH_RECORD)ProtocolObjectPool.Get(COMDT_REWARDMATCH_RECORD.CLASS_ID);
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
			if (cutVer == 0u || COMDT_REWARDMATCH_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_REWARDMATCH_DATA.CURRVERSION;
			}
			if (COMDT_REWARDMATCH_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bRecordCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bRecordCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astRecord.Length < (int)this.bRecordCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bRecordCnt; i++)
			{
				errorType = this.astRecord[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_REWARDMATCH_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_REWARDMATCH_DATA.CURRVERSION;
			}
			if (COMDT_REWARDMATCH_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bRecordCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bRecordCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bRecordCnt; i++)
			{
				errorType = this.astRecord[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_REWARDMATCH_DATA.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bRecordCnt = 0;
			if (this.astRecord != null)
			{
				for (int i = 0; i < this.astRecord.Length; i++)
				{
					if (this.astRecord[i] != null)
					{
						this.astRecord[i].Release();
						this.astRecord[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astRecord != null)
			{
				for (int i = 0; i < this.astRecord.Length; i++)
				{
					this.astRecord[i] = (COMDT_REWARDMATCH_RECORD)ProtocolObjectPool.Get(COMDT_REWARDMATCH_RECORD.CLASS_ID);
				}
			}
		}
	}
}
