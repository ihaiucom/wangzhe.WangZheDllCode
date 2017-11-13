using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_MASTERREQ_LIST : ProtocolObject
	{
		public uint dwNum;

		public COMDT_MASTERREQ_INFO[] astMasterReqList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly int CLASS_ID = 1626;

		public SCPKG_MASTERREQ_LIST()
		{
			this.astMasterReqList = new COMDT_MASTERREQ_INFO[10];
			for (int i = 0; i < 10; i++)
			{
				this.astMasterReqList[i] = (COMDT_MASTERREQ_INFO)ProtocolObjectPool.Get(COMDT_MASTERREQ_INFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_MASTERREQ_LIST.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_MASTERREQ_LIST.CURRVERSION;
			}
			if (SCPKG_MASTERREQ_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10u < this.dwNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astMasterReqList.Length < (long)((ulong)this.dwNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwNum))
			{
				errorType = this.astMasterReqList[num].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
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
			if (cutVer == 0u || SCPKG_MASTERREQ_LIST.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_MASTERREQ_LIST.CURRVERSION;
			}
			if (SCPKG_MASTERREQ_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10u < this.dwNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwNum))
			{
				errorType = this.astMasterReqList[num].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_MASTERREQ_LIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwNum = 0u;
			if (this.astMasterReqList != null)
			{
				for (int i = 0; i < this.astMasterReqList.Length; i++)
				{
					if (this.astMasterReqList[i] != null)
					{
						this.astMasterReqList[i].Release();
						this.astMasterReqList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astMasterReqList != null)
			{
				for (int i = 0; i < this.astMasterReqList.Length; i++)
				{
					this.astMasterReqList[i] = (COMDT_MASTERREQ_INFO)ProtocolObjectPool.Get(COMDT_MASTERREQ_INFO.CLASS_ID);
				}
			}
		}
	}
}
