using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_ACNTSELFMSGINFO : ProtocolObject
	{
		public byte bMsgCnt;

		public CSDT_SELFMSG_INFO[] astMsgList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1156;

		public SCPKG_ACNTSELFMSGINFO()
		{
			this.astMsgList = new CSDT_SELFMSG_INFO[10];
			for (int i = 0; i < 10; i++)
			{
				this.astMsgList[i] = (CSDT_SELFMSG_INFO)ProtocolObjectPool.Get(CSDT_SELFMSG_INFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_ACNTSELFMSGINFO.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_ACNTSELFMSGINFO.CURRVERSION;
			}
			if (SCPKG_ACNTSELFMSGINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bMsgCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bMsgCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astMsgList.Length < (int)this.bMsgCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bMsgCnt; i++)
			{
				errorType = this.astMsgList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_ACNTSELFMSGINFO.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_ACNTSELFMSGINFO.CURRVERSION;
			}
			if (SCPKG_ACNTSELFMSGINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bMsgCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bMsgCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bMsgCnt; i++)
			{
				errorType = this.astMsgList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_ACNTSELFMSGINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bMsgCnt = 0;
			if (this.astMsgList != null)
			{
				for (int i = 0; i < this.astMsgList.Length; i++)
				{
					if (this.astMsgList[i] != null)
					{
						this.astMsgList[i].Release();
						this.astMsgList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astMsgList != null)
			{
				for (int i = 0; i < this.astMsgList.Length; i++)
				{
					this.astMsgList[i] = (CSDT_SELFMSG_INFO)ProtocolObjectPool.Get(CSDT_SELFMSG_INFO.CLASS_ID);
				}
			}
		}
	}
}
