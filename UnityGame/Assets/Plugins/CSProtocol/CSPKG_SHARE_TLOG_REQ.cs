using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSPKG_SHARE_TLOG_REQ : ProtocolObject
	{
		public byte bNum;

		public CSDT_SHARE_TLOG_INFO[] astShareDetail;

		public uint dwSecretaryNum;

		public uint[] SecretaryDetail;

		public uint dwTrankNum;

		public CSDT_TRANK_TLOG_INFO[] astTrankDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1401;

		public CSPKG_SHARE_TLOG_REQ()
		{
			this.astShareDetail = new CSDT_SHARE_TLOG_INFO[64];
			for (int i = 0; i < 64; i++)
			{
				this.astShareDetail[i] = (CSDT_SHARE_TLOG_INFO)ProtocolObjectPool.Get(CSDT_SHARE_TLOG_INFO.CLASS_ID);
			}
			this.SecretaryDetail = new uint[64];
			this.astTrankDetail = new CSDT_TRANK_TLOG_INFO[68];
			for (int j = 0; j < 68; j++)
			{
				this.astTrankDetail[j] = (CSDT_TRANK_TLOG_INFO)ProtocolObjectPool.Get(CSDT_TRANK_TLOG_INFO.CLASS_ID);
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
			if (cutVer == 0u || CSPKG_SHARE_TLOG_REQ.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_SHARE_TLOG_REQ.CURRVERSION;
			}
			if (CSPKG_SHARE_TLOG_REQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (64 < this.bNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astShareDetail.Length < (int)this.bNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bNum; i++)
			{
				errorType = this.astShareDetail[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt32(this.dwSecretaryNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (64u < this.dwSecretaryNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.SecretaryDetail.Length < (long)((ulong)this.dwSecretaryNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwSecretaryNum))
			{
				errorType = destBuf.writeUInt32(this.SecretaryDetail[num]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			errorType = destBuf.writeUInt32(this.dwTrankNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (68u < this.dwTrankNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astTrankDetail.Length < (long)((ulong)this.dwTrankNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num2 = 0;
			while ((long)num2 < (long)((ulong)this.dwTrankNum))
			{
				errorType = this.astTrankDetail[num2].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num2++;
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
			if (cutVer == 0u || CSPKG_SHARE_TLOG_REQ.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_SHARE_TLOG_REQ.CURRVERSION;
			}
			if (CSPKG_SHARE_TLOG_REQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (64 < this.bNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bNum; i++)
			{
				errorType = this.astShareDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwSecretaryNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (64u < this.dwSecretaryNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.SecretaryDetail = new uint[this.dwSecretaryNum];
			int num = 0;
			while ((long)num < (long)((ulong)this.dwSecretaryNum))
			{
				errorType = srcBuf.readUInt32(ref this.SecretaryDetail[num]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			errorType = srcBuf.readUInt32(ref this.dwTrankNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (68u < this.dwTrankNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num2 = 0;
			while ((long)num2 < (long)((ulong)this.dwTrankNum))
			{
				errorType = this.astTrankDetail[num2].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num2++;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSPKG_SHARE_TLOG_REQ.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bNum = 0;
			if (this.astShareDetail != null)
			{
				for (int i = 0; i < this.astShareDetail.Length; i++)
				{
					if (this.astShareDetail[i] != null)
					{
						this.astShareDetail[i].Release();
						this.astShareDetail[i] = null;
					}
				}
			}
			this.dwSecretaryNum = 0u;
			this.dwTrankNum = 0u;
			if (this.astTrankDetail != null)
			{
				for (int j = 0; j < this.astTrankDetail.Length; j++)
				{
					if (this.astTrankDetail[j] != null)
					{
						this.astTrankDetail[j].Release();
						this.astTrankDetail[j] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astShareDetail != null)
			{
				for (int i = 0; i < this.astShareDetail.Length; i++)
				{
					this.astShareDetail[i] = (CSDT_SHARE_TLOG_INFO)ProtocolObjectPool.Get(CSDT_SHARE_TLOG_INFO.CLASS_ID);
				}
			}
			if (this.astTrankDetail != null)
			{
				for (int j = 0; j < this.astTrankDetail.Length; j++)
				{
					this.astTrankDetail[j] = (CSDT_TRANK_TLOG_INFO)ProtocolObjectPool.Get(CSDT_TRANK_TLOG_INFO.CLASS_ID);
				}
			}
		}
	}
}
