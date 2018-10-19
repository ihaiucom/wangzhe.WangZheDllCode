using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_PVPSPECITEM_OUTPUT : ProtocolObject
	{
		public byte bOutputCnt;

		public COMDT_PVPSPEC_OUTPUTINFO[] astOutputInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 241;

		public COMDT_PVPSPECITEM_OUTPUT()
		{
			this.astOutputInfo = new COMDT_PVPSPEC_OUTPUTINFO[3];
			for (int i = 0; i < 3; i++)
			{
				this.astOutputInfo[i] = (COMDT_PVPSPEC_OUTPUTINFO)ProtocolObjectPool.Get(COMDT_PVPSPEC_OUTPUTINFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_PVPSPECITEM_OUTPUT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PVPSPECITEM_OUTPUT.CURRVERSION;
			}
			if (COMDT_PVPSPECITEM_OUTPUT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bOutputCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3 < this.bOutputCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astOutputInfo.Length < (int)this.bOutputCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bOutputCnt; i++)
			{
				errorType = this.astOutputInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_PVPSPECITEM_OUTPUT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PVPSPECITEM_OUTPUT.CURRVERSION;
			}
			if (COMDT_PVPSPECITEM_OUTPUT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bOutputCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3 < this.bOutputCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bOutputCnt; i++)
			{
				errorType = this.astOutputInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_PVPSPECITEM_OUTPUT.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bOutputCnt = 0;
			if (this.astOutputInfo != null)
			{
				for (int i = 0; i < this.astOutputInfo.Length; i++)
				{
					if (this.astOutputInfo[i] != null)
					{
						this.astOutputInfo[i].Release();
						this.astOutputInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astOutputInfo != null)
			{
				for (int i = 0; i < this.astOutputInfo.Length; i++)
				{
					this.astOutputInfo[i] = (COMDT_PVPSPEC_OUTPUTINFO)ProtocolObjectPool.Get(COMDT_PVPSPEC_OUTPUTINFO.CLASS_ID);
				}
			}
		}
	}
}
