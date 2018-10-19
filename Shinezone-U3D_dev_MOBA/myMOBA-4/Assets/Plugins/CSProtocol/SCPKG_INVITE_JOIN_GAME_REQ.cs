using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_INVITE_JOIN_GAME_REQ : ProtocolObject
	{
		public byte bIndex;

		public byte bInviteType;

		public CSDT_INVITE_JOIN_GAME_DETAIL stInviteDetail;

		public COMDT_INVITER_INFO stInviterInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1193;

		public SCPKG_INVITE_JOIN_GAME_REQ()
		{
			this.stInviteDetail = (CSDT_INVITE_JOIN_GAME_DETAIL)ProtocolObjectPool.Get(CSDT_INVITE_JOIN_GAME_DETAIL.CLASS_ID);
			this.stInviterInfo = (COMDT_INVITER_INFO)ProtocolObjectPool.Get(COMDT_INVITER_INFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_INVITE_JOIN_GAME_REQ.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_INVITE_JOIN_GAME_REQ.CURRVERSION;
			}
			if (SCPKG_INVITE_JOIN_GAME_REQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bIndex);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bInviteType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bInviteType;
			errorType = this.stInviteDetail.pack(selector, ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stInviterInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_INVITE_JOIN_GAME_REQ.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_INVITE_JOIN_GAME_REQ.CURRVERSION;
			}
			if (SCPKG_INVITE_JOIN_GAME_REQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bIndex);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bInviteType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bInviteType;
			errorType = this.stInviteDetail.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stInviterInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_INVITE_JOIN_GAME_REQ.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bIndex = 0;
			this.bInviteType = 0;
			if (this.stInviteDetail != null)
			{
				this.stInviteDetail.Release();
				this.stInviteDetail = null;
			}
			if (this.stInviterInfo != null)
			{
				this.stInviterInfo.Release();
				this.stInviterInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stInviteDetail = (CSDT_INVITE_JOIN_GAME_DETAIL)ProtocolObjectPool.Get(CSDT_INVITE_JOIN_GAME_DETAIL.CLASS_ID);
			this.stInviterInfo = (COMDT_INVITER_INFO)ProtocolObjectPool.Get(COMDT_INVITER_INFO.CLASS_ID);
		}
	}
}
