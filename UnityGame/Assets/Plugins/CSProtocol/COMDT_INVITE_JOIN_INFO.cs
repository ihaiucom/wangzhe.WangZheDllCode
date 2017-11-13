using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_INVITE_JOIN_INFO : ProtocolObject
	{
		public int iInviteType;

		public COMDT_INVITER_INFO stInviterInfo;

		public uint dwInviteTime;

		public COMDT_INVITE_DETAIL stInviteDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 339;

		public COMDT_INVITE_JOIN_INFO()
		{
			this.stInviterInfo = (COMDT_INVITER_INFO)ProtocolObjectPool.Get(COMDT_INVITER_INFO.CLASS_ID);
			this.stInviteDetail = (COMDT_INVITE_DETAIL)ProtocolObjectPool.Get(COMDT_INVITE_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_INVITE_JOIN_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_INVITE_JOIN_INFO.CURRVERSION;
			}
			if (COMDT_INVITE_JOIN_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iInviteType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stInviterInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwInviteTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.iInviteType;
			errorType = this.stInviteDetail.pack(selector, ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_INVITE_JOIN_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_INVITE_JOIN_INFO.CURRVERSION;
			}
			if (COMDT_INVITE_JOIN_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iInviteType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stInviterInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwInviteTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.iInviteType;
			errorType = this.stInviteDetail.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_INVITE_JOIN_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iInviteType = 0;
			if (this.stInviterInfo != null)
			{
				this.stInviterInfo.Release();
				this.stInviterInfo = null;
			}
			this.dwInviteTime = 0u;
			if (this.stInviteDetail != null)
			{
				this.stInviteDetail.Release();
				this.stInviteDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stInviterInfo = (COMDT_INVITER_INFO)ProtocolObjectPool.Get(COMDT_INVITER_INFO.CLASS_ID);
			this.stInviteDetail = (COMDT_INVITE_DETAIL)ProtocolObjectPool.Get(COMDT_INVITE_DETAIL.CLASS_ID);
		}
	}
}
