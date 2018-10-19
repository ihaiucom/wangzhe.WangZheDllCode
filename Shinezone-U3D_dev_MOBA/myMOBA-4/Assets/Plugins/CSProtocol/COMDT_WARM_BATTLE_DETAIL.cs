using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_WARM_BATTLE_DETAIL : ProtocolObject
	{
		public byte bTodayCnt;

		public uint dwLastRefreshTime;

		public uint dwContinuousLoseCnt;

		public uint[] BattleCnt;

		public uint dwKillNum;

		public uint dwDeadNum;

		public COMDT_WARM_BATTLE_OF_RANK stWarmBattleOfRank;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 134u;

		public static readonly uint VERSION_stWarmBattleOfRank = 134u;

		public static readonly int CLASS_ID = 576;

		public COMDT_WARM_BATTLE_DETAIL()
		{
			this.BattleCnt = new uint[5];
			this.stWarmBattleOfRank = (COMDT_WARM_BATTLE_OF_RANK)ProtocolObjectPool.Get(COMDT_WARM_BATTLE_OF_RANK.CLASS_ID);
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
			if (cutVer == 0u || COMDT_WARM_BATTLE_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WARM_BATTLE_DETAIL.CURRVERSION;
			}
			if (COMDT_WARM_BATTLE_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bTodayCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwLastRefreshTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwContinuousLoseCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = destBuf.writeUInt32(this.BattleCnt[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt32(this.dwKillNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwDeadNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_WARM_BATTLE_DETAIL.VERSION_stWarmBattleOfRank <= cutVer)
			{
				errorType = this.stWarmBattleOfRank.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_WARM_BATTLE_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WARM_BATTLE_DETAIL.CURRVERSION;
			}
			if (COMDT_WARM_BATTLE_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bTodayCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwLastRefreshTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwContinuousLoseCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = srcBuf.readUInt32(ref this.BattleCnt[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwKillNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDeadNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_WARM_BATTLE_DETAIL.VERSION_stWarmBattleOfRank <= cutVer)
			{
				errorType = this.stWarmBattleOfRank.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stWarmBattleOfRank.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_WARM_BATTLE_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bTodayCnt = 0;
			this.dwLastRefreshTime = 0u;
			this.dwContinuousLoseCnt = 0u;
			this.dwKillNum = 0u;
			this.dwDeadNum = 0u;
			if (this.stWarmBattleOfRank != null)
			{
				this.stWarmBattleOfRank.Release();
				this.stWarmBattleOfRank = null;
			}
		}

		public override void OnUse()
		{
			this.stWarmBattleOfRank = (COMDT_WARM_BATTLE_OF_RANK)ProtocolObjectPool.Get(COMDT_WARM_BATTLE_OF_RANK.CLASS_ID);
		}
	}
}
