using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_NOTICELIST_RSP : ProtocolObject
	{
		public uint dwDataVersion;

		public ushort wNoticeCnt;

		public CSDT_NOTICE_INFO[] astNoticeList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1162;

		public SCPKG_NOTICELIST_RSP()
		{
			this.astNoticeList = new CSDT_NOTICE_INFO[30];
			for (int i = 0; i < 30; i++)
			{
				this.astNoticeList[i] = (CSDT_NOTICE_INFO)ProtocolObjectPool.Get(CSDT_NOTICE_INFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_NOTICELIST_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_NOTICELIST_RSP.CURRVERSION;
			}
			if (SCPKG_NOTICELIST_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwDataVersion);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt16(this.wNoticeCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (30 < this.wNoticeCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astNoticeList.Length < (int)this.wNoticeCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wNoticeCnt; i++)
			{
				errorType = this.astNoticeList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_NOTICELIST_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_NOTICELIST_RSP.CURRVERSION;
			}
			if (SCPKG_NOTICELIST_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwDataVersion);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wNoticeCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (30 < this.wNoticeCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wNoticeCnt; i++)
			{
				errorType = this.astNoticeList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_NOTICELIST_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwDataVersion = 0u;
			this.wNoticeCnt = 0;
			if (this.astNoticeList != null)
			{
				for (int i = 0; i < this.astNoticeList.Length; i++)
				{
					if (this.astNoticeList[i] != null)
					{
						this.astNoticeList[i].Release();
						this.astNoticeList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astNoticeList != null)
			{
				for (int i = 0; i < this.astNoticeList.Length; i++)
				{
					this.astNoticeList[i] = (CSDT_NOTICE_INFO)ProtocolObjectPool.Get(CSDT_NOTICE_INFO.CLASS_ID);
				}
			}
		}
	}
}
