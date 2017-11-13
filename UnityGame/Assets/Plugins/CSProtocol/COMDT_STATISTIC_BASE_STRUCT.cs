using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_STATISTIC_BASE_STRUCT : ProtocolObject
	{
		public COMDT_STATISTIC_BASE_STRUCT_KEY_INFO stKeyInfo;

		public COMDT_STATISTIC_BASE_STRUCT_VALUE_INFO stValueInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 153;

		public COMDT_STATISTIC_BASE_STRUCT()
		{
			this.stKeyInfo = (COMDT_STATISTIC_BASE_STRUCT_KEY_INFO)ProtocolObjectPool.Get(COMDT_STATISTIC_BASE_STRUCT_KEY_INFO.CLASS_ID);
			this.stValueInfo = (COMDT_STATISTIC_BASE_STRUCT_VALUE_INFO)ProtocolObjectPool.Get(COMDT_STATISTIC_BASE_STRUCT_VALUE_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_STATISTIC_BASE_STRUCT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_STATISTIC_BASE_STRUCT.CURRVERSION;
			}
			if (COMDT_STATISTIC_BASE_STRUCT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stKeyInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stValueInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_STATISTIC_BASE_STRUCT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_STATISTIC_BASE_STRUCT.CURRVERSION;
			}
			if (COMDT_STATISTIC_BASE_STRUCT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stKeyInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stValueInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_STATISTIC_BASE_STRUCT.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stKeyInfo != null)
			{
				this.stKeyInfo.Release();
				this.stKeyInfo = null;
			}
			if (this.stValueInfo != null)
			{
				this.stValueInfo.Release();
				this.stValueInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stKeyInfo = (COMDT_STATISTIC_BASE_STRUCT_KEY_INFO)ProtocolObjectPool.Get(COMDT_STATISTIC_BASE_STRUCT_KEY_INFO.CLASS_ID);
			this.stValueInfo = (COMDT_STATISTIC_BASE_STRUCT_VALUE_INFO)ProtocolObjectPool.Get(COMDT_STATISTIC_BASE_STRUCT_VALUE_INFO.CLASS_ID);
		}
	}
}
