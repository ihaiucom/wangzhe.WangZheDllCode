using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_FRAPBOOTINFO : ProtocolObject
	{
		public byte bSession;

		public byte bSpareNum;

		public SCPKG_FRAPBOOT_SINGLE[] astSpareFrap;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1269;

		public SCPKG_FRAPBOOTINFO()
		{
			this.astSpareFrap = new SCPKG_FRAPBOOT_SINGLE[4];
			for (int i = 0; i < 4; i++)
			{
				this.astSpareFrap[i] = (SCPKG_FRAPBOOT_SINGLE)ProtocolObjectPool.Get(SCPKG_FRAPBOOT_SINGLE.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_FRAPBOOTINFO.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_FRAPBOOTINFO.CURRVERSION;
			}
			if (SCPKG_FRAPBOOTINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bSession);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bSpareNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bSpareNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astSpareFrap.Length < (int)this.bSpareNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bSpareNum; i++)
			{
				errorType = this.astSpareFrap[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_FRAPBOOTINFO.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_FRAPBOOTINFO.CURRVERSION;
			}
			if (SCPKG_FRAPBOOTINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bSession);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bSpareNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bSpareNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bSpareNum; i++)
			{
				errorType = this.astSpareFrap[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_FRAPBOOTINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bSession = 0;
			this.bSpareNum = 0;
			if (this.astSpareFrap != null)
			{
				for (int i = 0; i < this.astSpareFrap.Length; i++)
				{
					if (this.astSpareFrap[i] != null)
					{
						this.astSpareFrap[i].Release();
						this.astSpareFrap[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astSpareFrap != null)
			{
				for (int i = 0; i < this.astSpareFrap.Length; i++)
				{
					this.astSpareFrap[i] = (SCPKG_FRAPBOOT_SINGLE)ProtocolObjectPool.Get(SCPKG_FRAPBOOT_SINGLE.CLASS_ID);
				}
			}
		}
	}
}
