using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_SETTLE_RESULT_DETAIL : ProtocolObject
	{
		public COMDT_GAME_INFO stGameInfo;

		public COMDT_ACNT_INFO stAcntInfo;

		public COMDT_RANK_SETTLE_INFO stRankInfo;

		public COMDT_SETTLE_HERO_RESULT_DETAIL stHeroList;

		public COMDT_REWARD_DETAIL stReward;

		public COMDT_REWARD_MULTIPLE_DETAIL stMultipleDetail;

		public COMDT_PVPSPECITEM_OUTPUT stSpecReward;

		public uint dwTipsMask;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 175u;

		public static readonly int CLASS_ID = 242;

		public COMDT_SETTLE_RESULT_DETAIL()
		{
			this.stGameInfo = (COMDT_GAME_INFO)ProtocolObjectPool.Get(COMDT_GAME_INFO.CLASS_ID);
			this.stAcntInfo = (COMDT_ACNT_INFO)ProtocolObjectPool.Get(COMDT_ACNT_INFO.CLASS_ID);
			this.stRankInfo = (COMDT_RANK_SETTLE_INFO)ProtocolObjectPool.Get(COMDT_RANK_SETTLE_INFO.CLASS_ID);
			this.stHeroList = (COMDT_SETTLE_HERO_RESULT_DETAIL)ProtocolObjectPool.Get(COMDT_SETTLE_HERO_RESULT_DETAIL.CLASS_ID);
			this.stReward = (COMDT_REWARD_DETAIL)ProtocolObjectPool.Get(COMDT_REWARD_DETAIL.CLASS_ID);
			this.stMultipleDetail = (COMDT_REWARD_MULTIPLE_DETAIL)ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_DETAIL.CLASS_ID);
			this.stSpecReward = (COMDT_PVPSPECITEM_OUTPUT)ProtocolObjectPool.Get(COMDT_PVPSPECITEM_OUTPUT.CLASS_ID);
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
			if (cutVer == 0u || COMDT_SETTLE_RESULT_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SETTLE_RESULT_DETAIL.CURRVERSION;
			}
			if (COMDT_SETTLE_RESULT_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stGameInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stAcntInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRankInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroList.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stReward.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMultipleDetail.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSpecReward.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTipsMask);
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
			if (cutVer == 0u || COMDT_SETTLE_RESULT_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SETTLE_RESULT_DETAIL.CURRVERSION;
			}
			if (COMDT_SETTLE_RESULT_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stGameInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stAcntInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRankInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHeroList.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stReward.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMultipleDetail.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stSpecReward.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTipsMask);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_SETTLE_RESULT_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stGameInfo != null)
			{
				this.stGameInfo.Release();
				this.stGameInfo = null;
			}
			if (this.stAcntInfo != null)
			{
				this.stAcntInfo.Release();
				this.stAcntInfo = null;
			}
			if (this.stRankInfo != null)
			{
				this.stRankInfo.Release();
				this.stRankInfo = null;
			}
			if (this.stHeroList != null)
			{
				this.stHeroList.Release();
				this.stHeroList = null;
			}
			if (this.stReward != null)
			{
				this.stReward.Release();
				this.stReward = null;
			}
			if (this.stMultipleDetail != null)
			{
				this.stMultipleDetail.Release();
				this.stMultipleDetail = null;
			}
			if (this.stSpecReward != null)
			{
				this.stSpecReward.Release();
				this.stSpecReward = null;
			}
			this.dwTipsMask = 0u;
		}

		public override void OnUse()
		{
			this.stGameInfo = (COMDT_GAME_INFO)ProtocolObjectPool.Get(COMDT_GAME_INFO.CLASS_ID);
			this.stAcntInfo = (COMDT_ACNT_INFO)ProtocolObjectPool.Get(COMDT_ACNT_INFO.CLASS_ID);
			this.stRankInfo = (COMDT_RANK_SETTLE_INFO)ProtocolObjectPool.Get(COMDT_RANK_SETTLE_INFO.CLASS_ID);
			this.stHeroList = (COMDT_SETTLE_HERO_RESULT_DETAIL)ProtocolObjectPool.Get(COMDT_SETTLE_HERO_RESULT_DETAIL.CLASS_ID);
			this.stReward = (COMDT_REWARD_DETAIL)ProtocolObjectPool.Get(COMDT_REWARD_DETAIL.CLASS_ID);
			this.stMultipleDetail = (COMDT_REWARD_MULTIPLE_DETAIL)ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_DETAIL.CLASS_ID);
			this.stSpecReward = (COMDT_PVPSPECITEM_OUTPUT)ProtocolObjectPool.Get(COMDT_PVPSPECITEM_OUTPUT.CLASS_ID);
		}
	}
}
