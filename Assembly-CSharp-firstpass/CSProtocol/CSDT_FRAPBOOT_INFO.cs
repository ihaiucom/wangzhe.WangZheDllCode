using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_FRAPBOOT_INFO : ProtocolObject
	{
		public uint dwKFrapsNo;

		public byte bNum;

		public CSDT_FRAPBOOT_DETAIL[] astBootInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1267;

		public CSDT_FRAPBOOT_INFO()
		{
			this.astBootInfo = new CSDT_FRAPBOOT_DETAIL[40];
			for (int i = 0; i < 40; i++)
			{
				this.astBootInfo[i] = (CSDT_FRAPBOOT_DETAIL)ProtocolObjectPool.Get(CSDT_FRAPBOOT_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || CSDT_FRAPBOOT_INFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_FRAPBOOT_INFO.CURRVERSION;
			}
			if (CSDT_FRAPBOOT_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwKFrapsNo);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (40 < this.bNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astBootInfo.Length < (int)this.bNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bNum; i++)
			{
				errorType = this.astBootInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_FRAPBOOT_INFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_FRAPBOOT_INFO.CURRVERSION;
			}
			if (CSDT_FRAPBOOT_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwKFrapsNo);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (40 < this.bNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bNum; i++)
			{
				errorType = this.astBootInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_FRAPBOOT_INFO.CLASS_ID;
		}

		public static CSDT_FRAPBOOT_INFO New()
		{
			return (CSDT_FRAPBOOT_INFO)ProtocolObjectPool.Get(CSDT_FRAPBOOT_INFO.CLASS_ID);
		}

		public override void OnRelease()
		{
			this.dwKFrapsNo = 0u;
			this.bNum = 0;
			if (this.astBootInfo != null)
			{
				for (int i = 0; i < this.astBootInfo.Length; i++)
				{
					if (this.astBootInfo[i] != null)
					{
						this.astBootInfo[i].Release();
						this.astBootInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astBootInfo != null)
			{
				for (int i = 0; i < this.astBootInfo.Length; i++)
				{
					this.astBootInfo[i] = (CSDT_FRAPBOOT_DETAIL)ProtocolObjectPool.Get(CSDT_FRAPBOOT_DETAIL.CLASS_ID);
				}
			}
		}
	}
}
