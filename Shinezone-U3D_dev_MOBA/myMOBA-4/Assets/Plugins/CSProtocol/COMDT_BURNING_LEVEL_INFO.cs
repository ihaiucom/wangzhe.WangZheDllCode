using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_BURNING_LEVEL_INFO : ProtocolObject
	{
		public byte bLevelNo;

		public int iLevelID;

		public byte bLevelStatus;

		public byte bRewardStatus;

		public uint dwAccPassCnt;

		public COMDT_BURNING_ENEMY_TEAM_INFO stEnemyDetail;

		public COMDT_BURNING_LUCKY_BUFF_INFO stLuckyBuffDetail;

		public COMDT_REWARD_MULTIPLE_DETAIL stMultipleDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly uint VERSION_stMultipleDetail = 5u;

		public static readonly int CLASS_ID = 350;

		public COMDT_BURNING_LEVEL_INFO()
		{
			this.stEnemyDetail = (COMDT_BURNING_ENEMY_TEAM_INFO)ProtocolObjectPool.Get(COMDT_BURNING_ENEMY_TEAM_INFO.CLASS_ID);
			this.stLuckyBuffDetail = (COMDT_BURNING_LUCKY_BUFF_INFO)ProtocolObjectPool.Get(COMDT_BURNING_LUCKY_BUFF_INFO.CLASS_ID);
			this.stMultipleDetail = (COMDT_REWARD_MULTIPLE_DETAIL)ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_DETAIL.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			this.bLevelNo = 0;
			this.iLevelID = 0;
			this.bLevelStatus = 0;
			this.bRewardStatus = 0;
			this.dwAccPassCnt = 0u;
			TdrError.ErrorType errorType = this.stEnemyDetail.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stLuckyBuffDetail.construct();
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
			if (cutVer == 0u || COMDT_BURNING_LEVEL_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_BURNING_LEVEL_INFO.CURRVERSION;
			}
			if (COMDT_BURNING_LEVEL_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bLevelNo);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iLevelID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bLevelStatus);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bRewardStatus);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwAccPassCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stEnemyDetail.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stLuckyBuffDetail.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_BURNING_LEVEL_INFO.VERSION_stMultipleDetail <= cutVer)
			{
				errorType = this.stMultipleDetail.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_BURNING_LEVEL_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_BURNING_LEVEL_INFO.CURRVERSION;
			}
			if (COMDT_BURNING_LEVEL_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bLevelNo);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iLevelID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bLevelStatus);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bRewardStatus);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAccPassCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stEnemyDetail.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stLuckyBuffDetail.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_BURNING_LEVEL_INFO.VERSION_stMultipleDetail <= cutVer)
			{
				errorType = this.stMultipleDetail.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stMultipleDetail.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_BURNING_LEVEL_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bLevelNo = 0;
			this.iLevelID = 0;
			this.bLevelStatus = 0;
			this.bRewardStatus = 0;
			this.dwAccPassCnt = 0u;
			if (this.stEnemyDetail != null)
			{
				this.stEnemyDetail.Release();
				this.stEnemyDetail = null;
			}
			if (this.stLuckyBuffDetail != null)
			{
				this.stLuckyBuffDetail.Release();
				this.stLuckyBuffDetail = null;
			}
			if (this.stMultipleDetail != null)
			{
				this.stMultipleDetail.Release();
				this.stMultipleDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stEnemyDetail = (COMDT_BURNING_ENEMY_TEAM_INFO)ProtocolObjectPool.Get(COMDT_BURNING_ENEMY_TEAM_INFO.CLASS_ID);
			this.stLuckyBuffDetail = (COMDT_BURNING_LUCKY_BUFF_INFO)ProtocolObjectPool.Get(COMDT_BURNING_LUCKY_BUFF_INFO.CLASS_ID);
			this.stMultipleDetail = (COMDT_REWARD_MULTIPLE_DETAIL)ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_DETAIL.CLASS_ID);
		}
	}
}
