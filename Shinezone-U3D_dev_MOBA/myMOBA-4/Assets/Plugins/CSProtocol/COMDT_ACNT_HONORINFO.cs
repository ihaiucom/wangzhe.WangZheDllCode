using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACNT_HONORINFO : ProtocolObject
	{
		public int iCurUseHonorID;

		public int iCurUseHonorLevel;

		public byte bHonorCnt;

		public COMDT_HONORINFO[] astHonorInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 69;

		public COMDT_ACNT_HONORINFO()
		{
			this.astHonorInfo = new COMDT_HONORINFO[6];
			for (int i = 0; i < 6; i++)
			{
				this.astHonorInfo[i] = (COMDT_HONORINFO)ProtocolObjectPool.Get(COMDT_HONORINFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ACNT_HONORINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_HONORINFO.CURRVERSION;
			}
			if (COMDT_ACNT_HONORINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iCurUseHonorID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iCurUseHonorLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bHonorCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (6 < this.bHonorCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astHonorInfo.Length < (int)this.bHonorCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bHonorCnt; i++)
			{
				errorType = this.astHonorInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_ACNT_HONORINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_HONORINFO.CURRVERSION;
			}
			if (COMDT_ACNT_HONORINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iCurUseHonorID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iCurUseHonorLevel);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bHonorCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (6 < this.bHonorCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bHonorCnt; i++)
			{
				errorType = this.astHonorInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACNT_HONORINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iCurUseHonorID = 0;
			this.iCurUseHonorLevel = 0;
			this.bHonorCnt = 0;
			if (this.astHonorInfo != null)
			{
				for (int i = 0; i < this.astHonorInfo.Length; i++)
				{
					if (this.astHonorInfo[i] != null)
					{
						this.astHonorInfo[i].Release();
						this.astHonorInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astHonorInfo != null)
			{
				for (int i = 0; i < this.astHonorInfo.Length; i++)
				{
					this.astHonorInfo[i] = (COMDT_HONORINFO)ProtocolObjectPool.Get(COMDT_HONORINFO.CLASS_ID);
				}
			}
		}
	}
}
