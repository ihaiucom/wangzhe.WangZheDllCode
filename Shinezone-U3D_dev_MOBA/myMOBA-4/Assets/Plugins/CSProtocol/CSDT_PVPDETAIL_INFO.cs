using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_PVPDETAIL_INFO : ProtocolObject
	{
		public COMDT_PVPBATTLE_INFO stOneVsOneInfo;

		public COMDT_PVPBATTLE_INFO stTwoVsTwoInfo;

		public COMDT_PVPBATTLE_INFO stThreeVsThreeInfo;

		public COMDT_PVPBATTLE_INFO stFourVsFourInfo;

		public COMDT_PVPBATTLE_INFO stFiveVsFiveInfo;

		public COMDT_PVPBATTLE_INFO stLadderInfo;

		public COMDT_PVPBATTLE_INFO stVsMachineInfo;

		public COMDT_PVPBATTLE_INFO stEntertainmentInfo;

		public COMDT_STATISTIC_KEY_VALUE_DETAIL stKVDetail;

		public COMDT_PVPBATTLE_INFO stGuildMatch;

		public COMDT_STATISTIC_DATA_EXTRA_DETAIL stMultiExtraDetail;

		public CSDT_STATISTIC_DATA_EXTRA_RADAR_DETAIL stRadarDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 208u;

		public static readonly uint VERSION_stFourVsFourInfo = 52u;

		public static readonly uint VERSION_stFiveVsFiveInfo = 52u;

		public static readonly int CLASS_ID = 1153;

		public CSDT_PVPDETAIL_INFO()
		{
			this.stOneVsOneInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stTwoVsTwoInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stThreeVsThreeInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stFourVsFourInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stFiveVsFiveInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stLadderInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stVsMachineInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stEntertainmentInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stKVDetail = (COMDT_STATISTIC_KEY_VALUE_DETAIL)ProtocolObjectPool.Get(COMDT_STATISTIC_KEY_VALUE_DETAIL.CLASS_ID);
			this.stGuildMatch = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stMultiExtraDetail = (COMDT_STATISTIC_DATA_EXTRA_DETAIL)ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_EXTRA_DETAIL.CLASS_ID);
			this.stRadarDetail = (CSDT_STATISTIC_DATA_EXTRA_RADAR_DETAIL)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || CSDT_PVPDETAIL_INFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_PVPDETAIL_INFO.CURRVERSION;
			}
			if (CSDT_PVPDETAIL_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stOneVsOneInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTwoVsTwoInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stThreeVsThreeInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (CSDT_PVPDETAIL_INFO.VERSION_stFourVsFourInfo <= cutVer)
			{
				errorType = this.stFourVsFourInfo.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (CSDT_PVPDETAIL_INFO.VERSION_stFiveVsFiveInfo <= cutVer)
			{
				errorType = this.stFiveVsFiveInfo.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stLadderInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stVsMachineInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stEntertainmentInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stKVDetail.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGuildMatch.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMultiExtraDetail.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRadarDetail.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_PVPDETAIL_INFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_PVPDETAIL_INFO.CURRVERSION;
			}
			if (CSDT_PVPDETAIL_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stOneVsOneInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTwoVsTwoInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stThreeVsThreeInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (CSDT_PVPDETAIL_INFO.VERSION_stFourVsFourInfo <= cutVer)
			{
				errorType = this.stFourVsFourInfo.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stFourVsFourInfo.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (CSDT_PVPDETAIL_INFO.VERSION_stFiveVsFiveInfo <= cutVer)
			{
				errorType = this.stFiveVsFiveInfo.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stFiveVsFiveInfo.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stLadderInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stVsMachineInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stEntertainmentInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stKVDetail.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGuildMatch.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMultiExtraDetail.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRadarDetail.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_PVPDETAIL_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stOneVsOneInfo != null)
			{
				this.stOneVsOneInfo.Release();
				this.stOneVsOneInfo = null;
			}
			if (this.stTwoVsTwoInfo != null)
			{
				this.stTwoVsTwoInfo.Release();
				this.stTwoVsTwoInfo = null;
			}
			if (this.stThreeVsThreeInfo != null)
			{
				this.stThreeVsThreeInfo.Release();
				this.stThreeVsThreeInfo = null;
			}
			if (this.stFourVsFourInfo != null)
			{
				this.stFourVsFourInfo.Release();
				this.stFourVsFourInfo = null;
			}
			if (this.stFiveVsFiveInfo != null)
			{
				this.stFiveVsFiveInfo.Release();
				this.stFiveVsFiveInfo = null;
			}
			if (this.stLadderInfo != null)
			{
				this.stLadderInfo.Release();
				this.stLadderInfo = null;
			}
			if (this.stVsMachineInfo != null)
			{
				this.stVsMachineInfo.Release();
				this.stVsMachineInfo = null;
			}
			if (this.stEntertainmentInfo != null)
			{
				this.stEntertainmentInfo.Release();
				this.stEntertainmentInfo = null;
			}
			if (this.stKVDetail != null)
			{
				this.stKVDetail.Release();
				this.stKVDetail = null;
			}
			if (this.stGuildMatch != null)
			{
				this.stGuildMatch.Release();
				this.stGuildMatch = null;
			}
			if (this.stMultiExtraDetail != null)
			{
				this.stMultiExtraDetail.Release();
				this.stMultiExtraDetail = null;
			}
			if (this.stRadarDetail != null)
			{
				this.stRadarDetail.Release();
				this.stRadarDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stOneVsOneInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stTwoVsTwoInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stThreeVsThreeInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stFourVsFourInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stFiveVsFiveInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stLadderInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stVsMachineInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stEntertainmentInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stKVDetail = (COMDT_STATISTIC_KEY_VALUE_DETAIL)ProtocolObjectPool.Get(COMDT_STATISTIC_KEY_VALUE_DETAIL.CLASS_ID);
			this.stGuildMatch = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stMultiExtraDetail = (COMDT_STATISTIC_DATA_EXTRA_DETAIL)ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_EXTRA_DETAIL.CLASS_ID);
			this.stRadarDetail = (CSDT_STATISTIC_DATA_EXTRA_RADAR_DETAIL)ProtocolObjectPool.Get(CSDT_STATISTIC_DATA_EXTRA_RADAR_DETAIL.CLASS_ID);
		}
	}
}
