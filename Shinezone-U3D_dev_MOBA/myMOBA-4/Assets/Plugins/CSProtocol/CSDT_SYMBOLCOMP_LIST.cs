using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_SYMBOLCOMP_LIST : ProtocolObject
	{
		public byte bPartCnt;

		public CSDT_ITEM_DELINFO[] astPartInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 936;

		public CSDT_SYMBOLCOMP_LIST()
		{
			this.astPartInfo = new CSDT_ITEM_DELINFO[4];
			for (int i = 0; i < 4; i++)
			{
				this.astPartInfo[i] = (CSDT_ITEM_DELINFO)ProtocolObjectPool.Get(CSDT_ITEM_DELINFO.CLASS_ID);
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
			if (cutVer == 0u || CSDT_SYMBOLCOMP_LIST.CURRVERSION < cutVer)
			{
				cutVer = CSDT_SYMBOLCOMP_LIST.CURRVERSION;
			}
			if (CSDT_SYMBOLCOMP_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bPartCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bPartCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astPartInfo.Length < (int)this.bPartCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bPartCnt; i++)
			{
				errorType = this.astPartInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_SYMBOLCOMP_LIST.CURRVERSION < cutVer)
			{
				cutVer = CSDT_SYMBOLCOMP_LIST.CURRVERSION;
			}
			if (CSDT_SYMBOLCOMP_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bPartCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (4 < this.bPartCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bPartCnt; i++)
			{
				errorType = this.astPartInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_SYMBOLCOMP_LIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bPartCnt = 0;
			if (this.astPartInfo != null)
			{
				for (int i = 0; i < this.astPartInfo.Length; i++)
				{
					if (this.astPartInfo[i] != null)
					{
						this.astPartInfo[i].Release();
						this.astPartInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astPartInfo != null)
			{
				for (int i = 0; i < this.astPartInfo.Length; i++)
				{
					this.astPartInfo[i] = (CSDT_ITEM_DELINFO)ProtocolObjectPool.Get(CSDT_ITEM_DELINFO.CLASS_ID);
				}
			}
		}
	}
}
