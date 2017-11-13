using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACNT_LICENSE : ProtocolObject
	{
		public byte bLicenseCnt;

		public COMDT_LICENSE_INFO[] astLicenseList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 586;

		public COMDT_ACNT_LICENSE()
		{
			this.astLicenseList = new COMDT_LICENSE_INFO[64];
			for (int i = 0; i < 64; i++)
			{
				this.astLicenseList[i] = (COMDT_LICENSE_INFO)ProtocolObjectPool.Get(COMDT_LICENSE_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ACNT_LICENSE.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_LICENSE.CURRVERSION;
			}
			if (COMDT_ACNT_LICENSE.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bLicenseCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (64 < this.bLicenseCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astLicenseList.Length < (int)this.bLicenseCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bLicenseCnt; i++)
			{
				errorType = this.astLicenseList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ACNT_LICENSE.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_LICENSE.CURRVERSION;
			}
			if (COMDT_ACNT_LICENSE.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bLicenseCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (64 < this.bLicenseCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bLicenseCnt; i++)
			{
				errorType = this.astLicenseList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACNT_LICENSE.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bLicenseCnt = 0;
			if (this.astLicenseList != null)
			{
				for (int i = 0; i < this.astLicenseList.Length; i++)
				{
					if (this.astLicenseList[i] != null)
					{
						this.astLicenseList[i].Release();
						this.astLicenseList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astLicenseList != null)
			{
				for (int i = 0; i < this.astLicenseList.Length; i++)
				{
					this.astLicenseList[i] = (COMDT_LICENSE_INFO)ProtocolObjectPool.Get(COMDT_LICENSE_INFO.CLASS_ID);
				}
			}
		}
	}
}
