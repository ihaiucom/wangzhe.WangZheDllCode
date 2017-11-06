using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_GUILD_RANK_INFO : ProtocolObject
	{
		public uint dwTotalRankPoint;

		public uint dwRankPointResetTime;

		public uint dwWeekRankPoint;

		public uint dwLastRankPoint;

		public uint dwSeasonStartTime;

		public uint dwYesterdayRP;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 82u;

		public static readonly uint VERSION_dwWeekRankPoint = 82u;

		public static readonly uint VERSION_dwLastRankPoint = 82u;

		public static readonly uint VERSION_dwSeasonStartTime = 82u;

		public static readonly uint VERSION_dwYesterdayRP = 82u;

		public static readonly int CLASS_ID = 358;

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
			if (cutVer == 0u || COMDT_GUILD_RANK_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GUILD_RANK_INFO.CURRVERSION;
			}
			if (COMDT_GUILD_RANK_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwTotalRankPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwRankPointResetTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_GUILD_RANK_INFO.VERSION_dwWeekRankPoint <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwWeekRankPoint);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_GUILD_RANK_INFO.VERSION_dwLastRankPoint <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastRankPoint);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_GUILD_RANK_INFO.VERSION_dwSeasonStartTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwSeasonStartTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_GUILD_RANK_INFO.VERSION_dwYesterdayRP <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwYesterdayRP);
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
			if (cutVer == 0u || COMDT_GUILD_RANK_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GUILD_RANK_INFO.CURRVERSION;
			}
			if (COMDT_GUILD_RANK_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwTotalRankPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwRankPointResetTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_GUILD_RANK_INFO.VERSION_dwWeekRankPoint <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwWeekRankPoint);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwWeekRankPoint = 0u;
			}
			if (COMDT_GUILD_RANK_INFO.VERSION_dwLastRankPoint <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastRankPoint);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastRankPoint = 0u;
			}
			if (COMDT_GUILD_RANK_INFO.VERSION_dwSeasonStartTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwSeasonStartTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwSeasonStartTime = 0u;
			}
			if (COMDT_GUILD_RANK_INFO.VERSION_dwYesterdayRP <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwYesterdayRP);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwYesterdayRP = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_GUILD_RANK_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwTotalRankPoint = 0u;
			this.dwRankPointResetTime = 0u;
			this.dwWeekRankPoint = 0u;
			this.dwLastRankPoint = 0u;
			this.dwSeasonStartTime = 0u;
			this.dwYesterdayRP = 0u;
		}
	}
}
