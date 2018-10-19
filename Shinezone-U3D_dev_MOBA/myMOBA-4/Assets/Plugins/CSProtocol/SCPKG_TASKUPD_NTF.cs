using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_TASKUPD_NTF : ProtocolObject
	{
		public uint dwUpdTaskCnt;

		public SCDT_UPDTASKONE[] astUpdTaskDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1108;

		public SCPKG_TASKUPD_NTF()
		{
			this.astUpdTaskDetail = new SCDT_UPDTASKONE[85];
			for (int i = 0; i < 85; i++)
			{
				this.astUpdTaskDetail[i] = (SCDT_UPDTASKONE)ProtocolObjectPool.Get(SCDT_UPDTASKONE.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_TASKUPD_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_TASKUPD_NTF.CURRVERSION;
			}
			if (SCPKG_TASKUPD_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwUpdTaskCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (85u < this.dwUpdTaskCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astUpdTaskDetail.Length < (long)((ulong)this.dwUpdTaskCnt))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwUpdTaskCnt))
			{
				errorType = this.astUpdTaskDetail[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_TASKUPD_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_TASKUPD_NTF.CURRVERSION;
			}
			if (SCPKG_TASKUPD_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwUpdTaskCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (85u < this.dwUpdTaskCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwUpdTaskCnt))
			{
				errorType = this.astUpdTaskDetail[num].unpack(ref srcBuf, cutVer);
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
			return SCPKG_TASKUPD_NTF.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwUpdTaskCnt = 0u;
			if (this.astUpdTaskDetail != null)
			{
				for (int i = 0; i < this.astUpdTaskDetail.Length; i++)
				{
					if (this.astUpdTaskDetail[i] != null)
					{
						this.astUpdTaskDetail[i].Release();
						this.astUpdTaskDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astUpdTaskDetail != null)
			{
				for (int i = 0; i < this.astUpdTaskDetail.Length; i++)
				{
					this.astUpdTaskDetail[i] = (SCDT_UPDTASKONE)ProtocolObjectPool.Get(SCDT_UPDTASKONE.CLASS_ID);
				}
			}
		}
	}
}
