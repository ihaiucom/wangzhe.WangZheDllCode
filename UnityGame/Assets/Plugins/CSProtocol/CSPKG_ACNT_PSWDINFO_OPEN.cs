using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSPKG_ACNT_PSWDINFO_OPEN : ProtocolObject
	{
		public sbyte[] szPswdStr;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1560;

		public CSPKG_ACNT_PSWDINFO_OPEN()
		{
			this.szPswdStr = new sbyte[64];
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
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			if (cutVer == 0u || CSPKG_ACNT_PSWDINFO_OPEN.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_ACNT_PSWDINFO_OPEN.CURRVERSION;
			}
			if (CSPKG_ACNT_PSWDINFO_OPEN.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			for (int i = 0; i < 64; i++)
			{
				errorType = destBuf.writeInt8(this.szPswdStr[i]);
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
			if (cutVer == 0u || CSPKG_ACNT_PSWDINFO_OPEN.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_ACNT_PSWDINFO_OPEN.CURRVERSION;
			}
			if (CSPKG_ACNT_PSWDINFO_OPEN.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			for (int i = 0; i < 64; i++)
			{
				errorType = srcBuf.readInt8(ref this.szPswdStr[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSPKG_ACNT_PSWDINFO_OPEN.CLASS_ID;
		}

		public override void OnRelease()
		{
		}
	}
}
