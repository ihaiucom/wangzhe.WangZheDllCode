using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_MAILOPTRES_GETMAILLIST : ProtocolObject
	{
		public byte bMailType;

		public byte bIsDiff;

		public uint dwVersion;

		public byte bCnt;

		public CSDT_GETMAIL_RES[] astMailInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 173u;

		public static readonly int CLASS_ID = 1125;

		public CSDT_MAILOPTRES_GETMAILLIST()
		{
			this.astMailInfo = new CSDT_GETMAIL_RES[100];
			for (int i = 0; i < 100; i++)
			{
				this.astMailInfo[i] = (CSDT_GETMAIL_RES)ProtocolObjectPool.Get(CSDT_GETMAIL_RES.CLASS_ID);
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
			if (cutVer == 0u || CSDT_MAILOPTRES_GETMAILLIST.CURRVERSION < cutVer)
			{
				cutVer = CSDT_MAILOPTRES_GETMAILLIST.CURRVERSION;
			}
			if (CSDT_MAILOPTRES_GETMAILLIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bMailType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bIsDiff);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwVersion);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100 < this.bCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astMailInfo.Length < (int)this.bCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bCnt; i++)
			{
				errorType = this.astMailInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_MAILOPTRES_GETMAILLIST.CURRVERSION < cutVer)
			{
				cutVer = CSDT_MAILOPTRES_GETMAILLIST.CURRVERSION;
			}
			if (CSDT_MAILOPTRES_GETMAILLIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bMailType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsDiff);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwVersion);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100 < this.bCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bCnt; i++)
			{
				errorType = this.astMailInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_MAILOPTRES_GETMAILLIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bMailType = 0;
			this.bIsDiff = 0;
			this.dwVersion = 0u;
			this.bCnt = 0;
			if (this.astMailInfo != null)
			{
				for (int i = 0; i < this.astMailInfo.Length; i++)
				{
					if (this.astMailInfo[i] != null)
					{
						this.astMailInfo[i].Release();
						this.astMailInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astMailInfo != null)
			{
				for (int i = 0; i < this.astMailInfo.Length; i++)
				{
					this.astMailInfo[i] = (CSDT_GETMAIL_RES)ProtocolObjectPool.Get(CSDT_GETMAIL_RES.CLASS_ID);
				}
			}
		}
	}
}
