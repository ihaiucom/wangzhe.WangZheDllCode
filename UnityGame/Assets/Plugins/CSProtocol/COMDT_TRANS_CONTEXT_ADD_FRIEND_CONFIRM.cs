using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TRANS_CONTEXT_ADD_FRIEND_CONFIRM : ProtocolObject
	{
		public COMDT_ACNT_UNIQ stAcntUniq;

		public COMDT_ACNT_UNIQ stFriendUniq;

		public COMDT_FRIEND_INFO stSrcUserInfo;

		public COMDT_FRIEND_INFO stFriendUserInfo;

		public byte bDestAcntOnline;

		public uint dwDestAcntGameSvrEntity;

		public byte bAddFriendType;

		public byte bDelFriendReqType;

		public byte bIsBothInsertFriend;

		public ushort wIntimacyValue;

		public uint dwIntimacyID;

		public byte bNoAskforFlag;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly int CLASS_ID = 434;

		public COMDT_TRANS_CONTEXT_ADD_FRIEND_CONFIRM()
		{
			this.stAcntUniq = (COMDT_ACNT_UNIQ)ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID);
			this.stFriendUniq = (COMDT_ACNT_UNIQ)ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID);
			this.stSrcUserInfo = (COMDT_FRIEND_INFO)ProtocolObjectPool.Get(COMDT_FRIEND_INFO.CLASS_ID);
			this.stFriendUserInfo = (COMDT_FRIEND_INFO)ProtocolObjectPool.Get(COMDT_FRIEND_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_TRANS_CONTEXT_ADD_FRIEND_CONFIRM.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANS_CONTEXT_ADD_FRIEND_CONFIRM.CURRVERSION;
			}
			if (COMDT_TRANS_CONTEXT_ADD_FRIEND_CONFIRM.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stAcntUniq.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFriendUniq.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSrcUserInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFriendUserInfo.pack(ref destBuf, cutVer);
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
			errorType = destBuf.writeUInt8(this.bAddFriendType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bDelFriendReqType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bIsBothInsertFriend);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt16(this.wIntimacyValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwIntimacyID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bNoAskforFlag);
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
			if (cutVer == 0u || COMDT_TRANS_CONTEXT_ADD_FRIEND_CONFIRM.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANS_CONTEXT_ADD_FRIEND_CONFIRM.CURRVERSION;
			}
			if (COMDT_TRANS_CONTEXT_ADD_FRIEND_CONFIRM.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stAcntUniq.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFriendUniq.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSrcUserInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFriendUserInfo.unpack(ref srcBuf, cutVer);
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
			errorType = srcBuf.readUInt8(ref this.bAddFriendType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bDelFriendReqType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsBothInsertFriend);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wIntimacyValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwIntimacyID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bNoAskforFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_TRANS_CONTEXT_ADD_FRIEND_CONFIRM.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stAcntUniq != null)
			{
				this.stAcntUniq.Release();
				this.stAcntUniq = null;
			}
			if (this.stFriendUniq != null)
			{
				this.stFriendUniq.Release();
				this.stFriendUniq = null;
			}
			if (this.stSrcUserInfo != null)
			{
				this.stSrcUserInfo.Release();
				this.stSrcUserInfo = null;
			}
			if (this.stFriendUserInfo != null)
			{
				this.stFriendUserInfo.Release();
				this.stFriendUserInfo = null;
			}
			this.bDestAcntOnline = 0;
			this.dwDestAcntGameSvrEntity = 0u;
			this.bAddFriendType = 0;
			this.bDelFriendReqType = 0;
			this.bIsBothInsertFriend = 0;
			this.wIntimacyValue = 0;
			this.dwIntimacyID = 0u;
			this.bNoAskforFlag = 0;
		}

		public override void OnUse()
		{
			this.stAcntUniq = (COMDT_ACNT_UNIQ)ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID);
			this.stFriendUniq = (COMDT_ACNT_UNIQ)ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID);
			this.stSrcUserInfo = (COMDT_FRIEND_INFO)ProtocolObjectPool.Get(COMDT_FRIEND_INFO.CLASS_ID);
			this.stFriendUserInfo = (COMDT_FRIEND_INFO)ProtocolObjectPool.Get(COMDT_FRIEND_INFO.CLASS_ID);
		}
	}
}
