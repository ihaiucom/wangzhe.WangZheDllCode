using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_MAILOPTRES_GETACCESS : ProtocolObject
	{
		public int iMailIndex;

		public byte bMailType;

		public byte bResult;

		public byte bAccessCnt;

		public COMDT_MAILACCESS[] astAccess;

		public CSDT_MAILACCESS_FROM[] astAccessFrom;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 173u;

		public static readonly int CLASS_ID = 1130;

		public CSDT_MAILOPTRES_GETACCESS()
		{
			this.astAccess = new COMDT_MAILACCESS[10];
			for (int i = 0; i < 10; i++)
			{
				this.astAccess[i] = (COMDT_MAILACCESS)ProtocolObjectPool.Get(COMDT_MAILACCESS.CLASS_ID);
			}
			this.astAccessFrom = new CSDT_MAILACCESS_FROM[10];
			for (int j = 0; j < 10; j++)
			{
				this.astAccessFrom[j] = (CSDT_MAILACCESS_FROM)ProtocolObjectPool.Get(CSDT_MAILACCESS_FROM.CLASS_ID);
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
			if (cutVer == 0u || CSDT_MAILOPTRES_GETACCESS.CURRVERSION < cutVer)
			{
				cutVer = CSDT_MAILOPTRES_GETACCESS.CURRVERSION;
			}
			if (CSDT_MAILOPTRES_GETACCESS.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iMailIndex);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bMailType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bResult);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bAccessCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bAccessCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astAccess.Length < (int)this.bAccessCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bAccessCnt; i++)
			{
				errorType = this.astAccess[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (10 < this.bAccessCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astAccessFrom.Length < (int)this.bAccessCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int j = 0; j < (int)this.bAccessCnt; j++)
			{
				errorType = this.astAccessFrom[j].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_MAILOPTRES_GETACCESS.CURRVERSION < cutVer)
			{
				cutVer = CSDT_MAILOPTRES_GETACCESS.CURRVERSION;
			}
			if (CSDT_MAILOPTRES_GETACCESS.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iMailIndex);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bMailType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bResult);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bAccessCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bAccessCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.astAccess = new COMDT_MAILACCESS[(int)this.bAccessCnt];
			for (int i = 0; i < (int)this.bAccessCnt; i++)
			{
				this.astAccess[i] = (COMDT_MAILACCESS)ProtocolObjectPool.Get(COMDT_MAILACCESS.CLASS_ID);
			}
			for (int j = 0; j < (int)this.bAccessCnt; j++)
			{
				errorType = this.astAccess[j].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (10 < this.bAccessCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.astAccessFrom = new CSDT_MAILACCESS_FROM[(int)this.bAccessCnt];
			for (int k = 0; k < (int)this.bAccessCnt; k++)
			{
				this.astAccessFrom[k] = (CSDT_MAILACCESS_FROM)ProtocolObjectPool.Get(CSDT_MAILACCESS_FROM.CLASS_ID);
			}
			for (int l = 0; l < (int)this.bAccessCnt; l++)
			{
				errorType = this.astAccessFrom[l].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_MAILOPTRES_GETACCESS.CLASS_ID;
		}
	}
}
