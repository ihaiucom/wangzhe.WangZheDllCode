using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_DAILY_CHECK_DATA : ProtocolObject
	{
		public uint dwLastAddPvpCoinTime;

		public int iTotalAddPvpCoinOneDay;

		public uint dwLastUpdateTime;

		public uint dwDailyAddExp;

		public uint dwDailyAddCoin;

		public uint dwDailyPvpCnt;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 28u;

		public static readonly uint VERSION_dwLastUpdateTime = 14u;

		public static readonly uint VERSION_dwDailyAddExp = 14u;

		public static readonly uint VERSION_dwDailyAddCoin = 14u;

		public static readonly uint VERSION_dwDailyPvpCnt = 28u;

		public static readonly int CLASS_ID = 551;

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
			if (cutVer == 0u || COMDT_DAILY_CHECK_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_DAILY_CHECK_DATA.CURRVERSION;
			}
			if (COMDT_DAILY_CHECK_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwLastAddPvpCoinTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iTotalAddPvpCoinOneDay);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_DAILY_CHECK_DATA.VERSION_dwLastUpdateTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastUpdateTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_DAILY_CHECK_DATA.VERSION_dwDailyAddExp <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwDailyAddExp);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_DAILY_CHECK_DATA.VERSION_dwDailyAddCoin <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwDailyAddCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_DAILY_CHECK_DATA.VERSION_dwDailyPvpCnt <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwDailyPvpCnt);
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
			if (cutVer == 0u || COMDT_DAILY_CHECK_DATA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_DAILY_CHECK_DATA.CURRVERSION;
			}
			if (COMDT_DAILY_CHECK_DATA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwLastAddPvpCoinTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iTotalAddPvpCoinOneDay);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_DAILY_CHECK_DATA.VERSION_dwLastUpdateTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastUpdateTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastUpdateTime = 0u;
			}
			if (COMDT_DAILY_CHECK_DATA.VERSION_dwDailyAddExp <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwDailyAddExp);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwDailyAddExp = 0u;
			}
			if (COMDT_DAILY_CHECK_DATA.VERSION_dwDailyAddCoin <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwDailyAddCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwDailyAddCoin = 0u;
			}
			if (COMDT_DAILY_CHECK_DATA.VERSION_dwDailyPvpCnt <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwDailyPvpCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwDailyPvpCnt = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_DAILY_CHECK_DATA.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwLastAddPvpCoinTime = 0u;
			this.iTotalAddPvpCoinOneDay = 0;
			this.dwLastUpdateTime = 0u;
			this.dwDailyAddExp = 0u;
			this.dwDailyAddCoin = 0u;
			this.dwDailyPvpCnt = 0u;
		}
	}
}
