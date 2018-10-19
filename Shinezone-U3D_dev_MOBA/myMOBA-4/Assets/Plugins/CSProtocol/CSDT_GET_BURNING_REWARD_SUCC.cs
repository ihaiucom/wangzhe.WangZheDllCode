using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_GET_BURNING_REWARD_SUCC : ProtocolObject
	{
		public COMDT_REWARD_DETAIL stReward;

		public byte bNextLevelNo;

		public COMDT_BURNING_ENEMY_TEAM_INFO stNextEnemyInfo;

		public COMDT_REWARD_MULTIPLE_DETAIL stMultipleDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 1297;

		public CSDT_GET_BURNING_REWARD_SUCC()
		{
			this.stReward = (COMDT_REWARD_DETAIL)ProtocolObjectPool.Get(COMDT_REWARD_DETAIL.CLASS_ID);
			this.stNextEnemyInfo = (COMDT_BURNING_ENEMY_TEAM_INFO)ProtocolObjectPool.Get(COMDT_BURNING_ENEMY_TEAM_INFO.CLASS_ID);
			this.stMultipleDetail = (COMDT_REWARD_MULTIPLE_DETAIL)ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_DETAIL.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = this.stReward.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.bNextLevelNo = 0;
			errorType = this.stNextEnemyInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMultipleDetail.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
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
			if (cutVer == 0u || CSDT_GET_BURNING_REWARD_SUCC.CURRVERSION < cutVer)
			{
				cutVer = CSDT_GET_BURNING_REWARD_SUCC.CURRVERSION;
			}
			if (CSDT_GET_BURNING_REWARD_SUCC.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stReward.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bNextLevelNo);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stNextEnemyInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMultipleDetail.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_GET_BURNING_REWARD_SUCC.CURRVERSION < cutVer)
			{
				cutVer = CSDT_GET_BURNING_REWARD_SUCC.CURRVERSION;
			}
			if (CSDT_GET_BURNING_REWARD_SUCC.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stReward.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bNextLevelNo);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stNextEnemyInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMultipleDetail.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_GET_BURNING_REWARD_SUCC.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stReward != null)
			{
				this.stReward.Release();
				this.stReward = null;
			}
			this.bNextLevelNo = 0;
			if (this.stNextEnemyInfo != null)
			{
				this.stNextEnemyInfo.Release();
				this.stNextEnemyInfo = null;
			}
			if (this.stMultipleDetail != null)
			{
				this.stMultipleDetail.Release();
				this.stMultipleDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stReward = (COMDT_REWARD_DETAIL)ProtocolObjectPool.Get(COMDT_REWARD_DETAIL.CLASS_ID);
			this.stNextEnemyInfo = (COMDT_BURNING_ENEMY_TEAM_INFO)ProtocolObjectPool.Get(COMDT_BURNING_ENEMY_TEAM_INFO.CLASS_ID);
			this.stMultipleDetail = (COMDT_REWARD_MULTIPLE_DETAIL)ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_DETAIL.CLASS_ID);
		}
	}
}
