using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_CMD_ASKFORREQ_GET : ProtocolObject
	{
		public ushort wReqCnt;

		public CSDT_ASKFORREQ_DETAIL[] astReqList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1136;

		public SCPKG_CMD_ASKFORREQ_GET()
		{
			this.astReqList = new CSDT_ASKFORREQ_DETAIL[100];
			for (int i = 0; i < 100; i++)
			{
				this.astReqList[i] = (CSDT_ASKFORREQ_DETAIL)ProtocolObjectPool.Get(CSDT_ASKFORREQ_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_CMD_ASKFORREQ_GET.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_ASKFORREQ_GET.CURRVERSION;
			}
			if (SCPKG_CMD_ASKFORREQ_GET.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt16(this.wReqCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100 < this.wReqCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astReqList.Length < (int)this.wReqCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wReqCnt; i++)
			{
				errorType = this.astReqList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_CMD_ASKFORREQ_GET.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_ASKFORREQ_GET.CURRVERSION;
			}
			if (SCPKG_CMD_ASKFORREQ_GET.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wReqCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100 < this.wReqCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wReqCnt; i++)
			{
				errorType = this.astReqList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_CMD_ASKFORREQ_GET.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wReqCnt = 0;
			if (this.astReqList != null)
			{
				for (int i = 0; i < this.astReqList.Length; i++)
				{
					if (this.astReqList[i] != null)
					{
						this.astReqList[i].Release();
						this.astReqList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astReqList != null)
			{
				for (int i = 0; i < this.astReqList.Length; i++)
				{
					this.astReqList[i] = (CSDT_ASKFORREQ_DETAIL)ProtocolObjectPool.Get(CSDT_ASKFORREQ_DETAIL.CLASS_ID);
				}
			}
		}
	}
}
