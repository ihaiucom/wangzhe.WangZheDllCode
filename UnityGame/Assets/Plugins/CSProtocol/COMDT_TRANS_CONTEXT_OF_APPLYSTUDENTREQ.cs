using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TRANS_CONTEXT_OF_APPLYSTUDENTREQ : ProtocolObject
	{
		public COMDT_ACNT_UNIQ stMasterUniq;

		public COMDT_ACNT_UNIQ stStudentUniq;

		public byte[] szVerificationInfo;

		public COMDT_FRIEND_INFO stSrcUserInfo;

		public byte bDestAcntOnline;

		public uint dwDestAcntGameSvrEntity;

		public byte bMasterMaxGrade;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly uint LENGTH_szVerificationInfo = 128u;

		public static readonly int CLASS_ID = 427;

		public COMDT_TRANS_CONTEXT_OF_APPLYSTUDENTREQ()
		{
			this.stMasterUniq = (COMDT_ACNT_UNIQ)ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID);
			this.stStudentUniq = (COMDT_ACNT_UNIQ)ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID);
			this.szVerificationInfo = new byte[128];
			this.stSrcUserInfo = (COMDT_FRIEND_INFO)ProtocolObjectPool.Get(COMDT_FRIEND_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_TRANS_CONTEXT_OF_APPLYSTUDENTREQ.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANS_CONTEXT_OF_APPLYSTUDENTREQ.CURRVERSION;
			}
			if (COMDT_TRANS_CONTEXT_OF_APPLYSTUDENTREQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stMasterUniq.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stStudentUniq.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize2 = destBuf.getUsedSize();
			int num = TdrTypeUtil.cstrlen(this.szVerificationInfo);
			if (num >= 128)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szVerificationInfo, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src = destBuf.getUsedSize() - usedSize2;
			errorType = destBuf.writeUInt32((uint)src, usedSize);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSrcUserInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bDestAcntOnline);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwDestAcntGameSvrEntity);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bMasterMaxGrade);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (cutVer == 0u || COMDT_TRANS_CONTEXT_OF_APPLYSTUDENTREQ.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANS_CONTEXT_OF_APPLYSTUDENTREQ.CURRVERSION;
			}
			if (COMDT_TRANS_CONTEXT_OF_APPLYSTUDENTREQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stMasterUniq.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stStudentUniq.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num = 0u;
			errorType = srcBuf.readUInt32(ref num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num > (uint)this.szVerificationInfo.GetLength(0))
			{
				if ((ulong)num > (ulong)COMDT_TRANS_CONTEXT_OF_APPLYSTUDENTREQ.LENGTH_szVerificationInfo)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szVerificationInfo = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szVerificationInfo, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szVerificationInfo[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szVerificationInfo) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = this.stSrcUserInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bDestAcntOnline);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDestAcntGameSvrEntity);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bMasterMaxGrade);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_TRANS_CONTEXT_OF_APPLYSTUDENTREQ.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stMasterUniq != null)
			{
				this.stMasterUniq.Release();
				this.stMasterUniq = null;
			}
			if (this.stStudentUniq != null)
			{
				this.stStudentUniq.Release();
				this.stStudentUniq = null;
			}
			if (this.stSrcUserInfo != null)
			{
				this.stSrcUserInfo.Release();
				this.stSrcUserInfo = null;
			}
			this.bDestAcntOnline = 0;
			this.dwDestAcntGameSvrEntity = 0u;
			this.bMasterMaxGrade = 0;
		}

		public override void OnUse()
		{
			this.stMasterUniq = (COMDT_ACNT_UNIQ)ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID);
			this.stStudentUniq = (COMDT_ACNT_UNIQ)ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID);
			this.stSrcUserInfo = (COMDT_FRIEND_INFO)ProtocolObjectPool.Get(COMDT_FRIEND_INFO.CLASS_ID);
		}
	}
}
