using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_PVP_PUNISH_INFO : ProtocolObject
	{
		public uint dwTodayCnt;

		public uint dwPunishEndTime;

		public uint dwLastRefreshTime;

		public uint dwTodayHangUpCnt;

		public byte bType;

		public uint dwCreditPunishCntWeek;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 174u;

		public static readonly uint VERSION_dwTodayHangUpCnt = 122u;

		public static readonly uint VERSION_bType = 122u;

		public static readonly uint VERSION_dwCreditPunishCntWeek = 174u;

		public static readonly int CLASS_ID = 597;

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
			if (cutVer == 0u || COMDT_PVP_PUNISH_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PVP_PUNISH_INFO.CURRVERSION;
			}
			if (COMDT_PVP_PUNISH_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwTodayCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwPunishEndTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwLastRefreshTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_PVP_PUNISH_INFO.VERSION_dwTodayHangUpCnt <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwTodayHangUpCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_PVP_PUNISH_INFO.VERSION_bType <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bType);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_PVP_PUNISH_INFO.VERSION_dwCreditPunishCntWeek <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwCreditPunishCntWeek);
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
			if (cutVer == 0u || COMDT_PVP_PUNISH_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PVP_PUNISH_INFO.CURRVERSION;
			}
			if (COMDT_PVP_PUNISH_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwTodayCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPunishEndTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwLastRefreshTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_PVP_PUNISH_INFO.VERSION_dwTodayHangUpCnt <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwTodayHangUpCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwTodayHangUpCnt = 0u;
			}
			if (COMDT_PVP_PUNISH_INFO.VERSION_bType <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bType);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bType = 0;
			}
			if (COMDT_PVP_PUNISH_INFO.VERSION_dwCreditPunishCntWeek <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwCreditPunishCntWeek);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwCreditPunishCntWeek = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_PVP_PUNISH_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwTodayCnt = 0u;
			this.dwPunishEndTime = 0u;
			this.dwLastRefreshTime = 0u;
			this.dwTodayHangUpCnt = 0u;
			this.bType = 0;
			this.dwCreditPunishCntWeek = 0u;
		}
	}
}
