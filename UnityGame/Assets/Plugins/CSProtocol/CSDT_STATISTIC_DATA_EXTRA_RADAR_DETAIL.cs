using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_STATISTIC_DATA_EXTRA_RADAR_DETAIL : ProtocolObject
	{
		public CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO stTotal;

		public CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO stTotal5v5;

		public CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO stTotalLadder;

		public CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO stTotalGuild;

		public CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO stRecentTotal;

		public CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO stRecent5v5;

		public CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO stRecentLadder;

		public CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO stRecentGuild;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1152;

		public CSDT_STATISTIC_DATA_EXTRA_RADAR_DETAIL()
		{
			this.stTotal = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
			this.stTotal5v5 = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
			this.stTotalLadder = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
			this.stTotalGuild = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
			this.stRecentTotal = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
			this.stRecent5v5 = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
			this.stRecentLadder = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
			this.stRecentGuild = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
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
			if (cutVer == 0u || CSDT_STATISTIC_DATA_EXTRA_RADAR_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = CSDT_STATISTIC_DATA_EXTRA_RADAR_DETAIL.CURRVERSION;
			}
			if (CSDT_STATISTIC_DATA_EXTRA_RADAR_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stTotal.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTotal5v5.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTotalLadder.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTotalGuild.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRecentTotal.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRecent5v5.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRecentLadder.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRecentGuild.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_STATISTIC_DATA_EXTRA_RADAR_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = CSDT_STATISTIC_DATA_EXTRA_RADAR_DETAIL.CURRVERSION;
			}
			if (CSDT_STATISTIC_DATA_EXTRA_RADAR_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stTotal.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTotal5v5.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTotalLadder.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTotalGuild.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRecentTotal.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRecent5v5.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRecentLadder.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRecentGuild.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_STATISTIC_DATA_EXTRA_RADAR_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stTotal != null)
			{
				this.stTotal.Release();
				this.stTotal = null;
			}
			if (this.stTotal5v5 != null)
			{
				this.stTotal5v5.Release();
				this.stTotal5v5 = null;
			}
			if (this.stTotalLadder != null)
			{
				this.stTotalLadder.Release();
				this.stTotalLadder = null;
			}
			if (this.stTotalGuild != null)
			{
				this.stTotalGuild.Release();
				this.stTotalGuild = null;
			}
			if (this.stRecentTotal != null)
			{
				this.stRecentTotal.Release();
				this.stRecentTotal = null;
			}
			if (this.stRecent5v5 != null)
			{
				this.stRecent5v5.Release();
				this.stRecent5v5 = null;
			}
			if (this.stRecentLadder != null)
			{
				this.stRecentLadder.Release();
				this.stRecentLadder = null;
			}
			if (this.stRecentGuild != null)
			{
				this.stRecentGuild.Release();
				this.stRecentGuild = null;
			}
		}

		public override void OnUse()
		{
			this.stTotal = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
			this.stTotal5v5 = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
			this.stTotalLadder = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
			this.stTotalGuild = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
			this.stRecentTotal = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
			this.stRecent5v5 = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
			this.stRecentLadder = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
			this.stRecentGuild = (CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.CLASS_ID);
		}
	}
}
