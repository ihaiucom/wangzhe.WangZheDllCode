using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_INTIMACY_DATA : ProtocolObject
	{
		public uint dwLastIntimacyTime;

		public ushort wDayIntimacyValue;

		public ushort wIntimacyValue;

		public byte bIntimacyState;

		public uint dwTerminateTime;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 171u;

		public static readonly uint VERSION_wDayIntimacyValue = 116u;

		public static readonly uint VERSION_bIntimacyState = 171u;

		public static readonly uint VERSION_dwTerminateTime = 171u;

		public static readonly int CLASS_ID = 300;

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
			if (cutVer == 0u || COMDT_INTIMACY_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_INTIMACY_DATA.CURRVERSION;
			}
			if (COMDT_INTIMACY_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwLastIntimacyTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_INTIMACY_DATA.VERSION_wDayIntimacyValue <= cutVer)
			{
				errorType = destBuf.writeUInt16(this.wDayIntimacyValue);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt16(this.wIntimacyValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_INTIMACY_DATA.VERSION_bIntimacyState <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bIntimacyState);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_INTIMACY_DATA.VERSION_dwTerminateTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwTerminateTime);
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
			if (cutVer == 0u || COMDT_INTIMACY_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_INTIMACY_DATA.CURRVERSION;
			}
			if (COMDT_INTIMACY_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwLastIntimacyTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_INTIMACY_DATA.VERSION_wDayIntimacyValue <= cutVer)
			{
				errorType = srcBuf.readUInt16(ref this.wDayIntimacyValue);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.wDayIntimacyValue = 0;
			}
			errorType = srcBuf.readUInt16(ref this.wIntimacyValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_INTIMACY_DATA.VERSION_bIntimacyState <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bIntimacyState);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bIntimacyState = 0;
			}
			if (COMDT_INTIMACY_DATA.VERSION_dwTerminateTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwTerminateTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwTerminateTime = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_INTIMACY_DATA.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwLastIntimacyTime = 0u;
			this.wDayIntimacyValue = 0;
			this.wIntimacyValue = 0;
			this.bIntimacyState = 0;
			this.dwTerminateTime = 0u;
		}
	}
}
