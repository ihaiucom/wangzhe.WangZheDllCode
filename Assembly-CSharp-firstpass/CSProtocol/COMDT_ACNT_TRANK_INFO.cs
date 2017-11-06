using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACNT_TRANK_INFO : ProtocolObject
	{
		public uint dwLastAllPower;

		public uint dwLastPowerRankNo;

		public uint dwLastPvpExpRankNo;

		public uint dwLastHeroNumNo;

		public uint dwLastSkinNumNo;

		public uint dwLastLadderPointNo;

		public uint dwLastAchievementNo;

		public uint dwLastWinGameNumNo;

		public uint dwLastContinousWinNo;

		public uint dwWeekTopConWinTime;

		public uint dwWeekTopConWinNum;

		public uint dwCurContinousWinNum;

		public uint dwLastUseCouponsNo;

		public uint dwLastUseCouponsTime;

		public uint dwCurWeekUseCouponsNum;

		public uint dwLastVipScoreNo;

		public uint dwLastMasterPointNo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 178u;

		public static readonly uint VERSION_dwLastHeroNumNo = 20u;

		public static readonly uint VERSION_dwLastSkinNumNo = 20u;

		public static readonly uint VERSION_dwLastLadderPointNo = 27u;

		public static readonly uint VERSION_dwLastAchievementNo = 27u;

		public static readonly uint VERSION_dwLastWinGameNumNo = 27u;

		public static readonly uint VERSION_dwLastContinousWinNo = 27u;

		public static readonly uint VERSION_dwWeekTopConWinTime = 27u;

		public static readonly uint VERSION_dwWeekTopConWinNum = 27u;

		public static readonly uint VERSION_dwCurContinousWinNum = 27u;

		public static readonly uint VERSION_dwLastUseCouponsNo = 30u;

		public static readonly uint VERSION_dwLastUseCouponsTime = 30u;

		public static readonly uint VERSION_dwCurWeekUseCouponsNum = 30u;

		public static readonly uint VERSION_dwLastVipScoreNo = 34u;

		public static readonly uint VERSION_dwLastMasterPointNo = 178u;

		public static readonly int CLASS_ID = 524;

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
			if (cutVer == 0u || COMDT_ACNT_TRANK_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_TRANK_INFO.CURRVERSION;
			}
			if (COMDT_ACNT_TRANK_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwLastAllPower);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwLastPowerRankNo);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwLastPvpExpRankNo);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastHeroNumNo <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastHeroNumNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastSkinNumNo <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastSkinNumNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastLadderPointNo <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastLadderPointNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastAchievementNo <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastAchievementNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastWinGameNumNo <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastWinGameNumNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastContinousWinNo <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastContinousWinNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwWeekTopConWinTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwWeekTopConWinTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwWeekTopConWinNum <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwWeekTopConWinNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwCurContinousWinNum <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwCurContinousWinNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastUseCouponsNo <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastUseCouponsNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastUseCouponsTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastUseCouponsTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwCurWeekUseCouponsNum <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwCurWeekUseCouponsNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastVipScoreNo <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastVipScoreNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastMasterPointNo <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastMasterPointNo);
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
			if (cutVer == 0u || COMDT_ACNT_TRANK_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACNT_TRANK_INFO.CURRVERSION;
			}
			if (COMDT_ACNT_TRANK_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwLastAllPower);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwLastPowerRankNo);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwLastPvpExpRankNo);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastHeroNumNo <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastHeroNumNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastHeroNumNo = 0u;
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastSkinNumNo <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastSkinNumNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastSkinNumNo = 0u;
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastLadderPointNo <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastLadderPointNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastLadderPointNo = 0u;
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastAchievementNo <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastAchievementNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastAchievementNo = 0u;
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastWinGameNumNo <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastWinGameNumNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastWinGameNumNo = 0u;
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastContinousWinNo <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastContinousWinNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastContinousWinNo = 0u;
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwWeekTopConWinTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwWeekTopConWinTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwWeekTopConWinTime = 0u;
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwWeekTopConWinNum <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwWeekTopConWinNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwWeekTopConWinNum = 0u;
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwCurContinousWinNum <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwCurContinousWinNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwCurContinousWinNum = 0u;
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastUseCouponsNo <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastUseCouponsNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastUseCouponsNo = 0u;
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastUseCouponsTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastUseCouponsTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastUseCouponsTime = 0u;
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwCurWeekUseCouponsNum <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwCurWeekUseCouponsNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwCurWeekUseCouponsNum = 0u;
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastVipScoreNo <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastVipScoreNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastVipScoreNo = 0u;
			}
			if (COMDT_ACNT_TRANK_INFO.VERSION_dwLastMasterPointNo <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastMasterPointNo);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastMasterPointNo = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACNT_TRANK_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwLastAllPower = 0u;
			this.dwLastPowerRankNo = 0u;
			this.dwLastPvpExpRankNo = 0u;
			this.dwLastHeroNumNo = 0u;
			this.dwLastSkinNumNo = 0u;
			this.dwLastLadderPointNo = 0u;
			this.dwLastAchievementNo = 0u;
			this.dwLastWinGameNumNo = 0u;
			this.dwLastContinousWinNo = 0u;
			this.dwWeekTopConWinTime = 0u;
			this.dwWeekTopConWinNum = 0u;
			this.dwCurContinousWinNum = 0u;
			this.dwLastUseCouponsNo = 0u;
			this.dwLastUseCouponsTime = 0u;
			this.dwCurWeekUseCouponsNum = 0u;
			this.dwLastVipScoreNo = 0u;
			this.dwLastMasterPointNo = 0u;
		}
	}
}
