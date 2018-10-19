using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_FRIEND_INFO : ProtocolObject
	{
		public COMDT_FRIEND_INFO stFriendInfo;

		public ulong ullDonateAPSec;

		public byte bGameState;

		public uint dwGameStartTime;

		public COMDT_INTIMACY_DATA stIntimacyData;

		public byte bVideoState;

		public CSDT_GAMEINFO stGameInfo;

		public COMDT_RECRUITMENT_DATA stRecruitmentInfo;

		public byte bNoAskforFlag;

		public uint dwOtherStateBits;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly int CLASS_ID = 1002;

		public CSDT_FRIEND_INFO()
		{
			this.stFriendInfo = (COMDT_FRIEND_INFO)ProtocolObjectPool.Get(COMDT_FRIEND_INFO.CLASS_ID);
			this.stIntimacyData = (COMDT_INTIMACY_DATA)ProtocolObjectPool.Get(COMDT_INTIMACY_DATA.CLASS_ID);
			this.stGameInfo = (CSDT_GAMEINFO)ProtocolObjectPool.Get(CSDT_GAMEINFO.CLASS_ID);
			this.stRecruitmentInfo = (COMDT_RECRUITMENT_DATA)ProtocolObjectPool.Get(COMDT_RECRUITMENT_DATA.CLASS_ID);
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
			if (cutVer == 0u || CSDT_FRIEND_INFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_FRIEND_INFO.CURRVERSION;
			}
			if (CSDT_FRIEND_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stFriendInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullDonateAPSec);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bGameState);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwGameStartTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stIntimacyData.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bVideoState);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bVideoState;
			errorType = this.stGameInfo.pack(selector, ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRecruitmentInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bNoAskforFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwOtherStateBits);
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
			if (cutVer == 0u || CSDT_FRIEND_INFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_FRIEND_INFO.CURRVERSION;
			}
			if (CSDT_FRIEND_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stFriendInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullDonateAPSec);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bGameState);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGameStartTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stIntimacyData.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bVideoState);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bVideoState;
			errorType = this.stGameInfo.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRecruitmentInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bNoAskforFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwOtherStateBits);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_FRIEND_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stFriendInfo != null)
			{
				this.stFriendInfo.Release();
				this.stFriendInfo = null;
			}
			this.ullDonateAPSec = 0uL;
			this.bGameState = 0;
			this.dwGameStartTime = 0u;
			if (this.stIntimacyData != null)
			{
				this.stIntimacyData.Release();
				this.stIntimacyData = null;
			}
			this.bVideoState = 0;
			if (this.stGameInfo != null)
			{
				this.stGameInfo.Release();
				this.stGameInfo = null;
			}
			if (this.stRecruitmentInfo != null)
			{
				this.stRecruitmentInfo.Release();
				this.stRecruitmentInfo = null;
			}
			this.bNoAskforFlag = 0;
			this.dwOtherStateBits = 0u;
		}

		public override void OnUse()
		{
			this.stFriendInfo = (COMDT_FRIEND_INFO)ProtocolObjectPool.Get(COMDT_FRIEND_INFO.CLASS_ID);
			this.stIntimacyData = (COMDT_INTIMACY_DATA)ProtocolObjectPool.Get(COMDT_INTIMACY_DATA.CLASS_ID);
			this.stGameInfo = (CSDT_GAMEINFO)ProtocolObjectPool.Get(CSDT_GAMEINFO.CLASS_ID);
			this.stRecruitmentInfo = (COMDT_RECRUITMENT_DATA)ProtocolObjectPool.Get(COMDT_RECRUITMENT_DATA.CLASS_ID);
		}
	}
}
