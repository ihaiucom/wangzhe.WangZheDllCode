using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_STATISTIC_DATA_DETAIL : ProtocolObject
	{
		public byte bSingleNum;

		public COMDT_STATISTIC_DATA_INFO_SINGLE[] astSingleDetail;

		public byte bMultiNum;

		public COMDT_STATISTIC_DATA_INFO_MULTI[] astMultiDetail;

		public COMDT_STATISTIC_KEY_VALUE_DETAIL stKVDetail;

		public byte bWarmNum;

		public COMDT_WARM_BATTLE_INFO[] astWarmDetail;

		public uint dwNormalMMRContinuousWinNum;

		public uint dwNormalMMRContinuousLoseNum;

		public uint dwNormalMMRWinNum;

		public uint dwNormalMMRLoseNum;

		public COMDT_WARM_BATTLE_INFO stLadderWarm;

		public COMDT_STATISTIC_DATA_EXTRA_DETAIL stMultiExtra;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 208u;

		public static readonly uint VERSION_bWarmNum = 54u;

		public static readonly uint VERSION_astWarmDetail = 54u;

		public static readonly uint VERSION_dwNormalMMRContinuousWinNum = 94u;

		public static readonly uint VERSION_dwNormalMMRContinuousLoseNum = 94u;

		public static readonly uint VERSION_dwNormalMMRWinNum = 95u;

		public static readonly uint VERSION_dwNormalMMRLoseNum = 95u;

		public static readonly uint VERSION_stLadderWarm = 140u;

		public static readonly uint VERSION_stMultiExtra = 146u;

		public static readonly int CLASS_ID = 500;

		public COMDT_STATISTIC_DATA_DETAIL()
		{
			this.astSingleDetail = new COMDT_STATISTIC_DATA_INFO_SINGLE[20];
			for (int i = 0; i < 20; i++)
			{
				this.astSingleDetail[i] = (COMDT_STATISTIC_DATA_INFO_SINGLE)ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_INFO_SINGLE.CLASS_ID);
			}
			this.astMultiDetail = new COMDT_STATISTIC_DATA_INFO_MULTI[40];
			for (int j = 0; j < 40; j++)
			{
				this.astMultiDetail[j] = (COMDT_STATISTIC_DATA_INFO_MULTI)ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_INFO_MULTI.CLASS_ID);
			}
			this.stKVDetail = (COMDT_STATISTIC_KEY_VALUE_DETAIL)ProtocolObjectPool.Get(COMDT_STATISTIC_KEY_VALUE_DETAIL.CLASS_ID);
			this.astWarmDetail = new COMDT_WARM_BATTLE_INFO[10];
			for (int k = 0; k < 10; k++)
			{
				this.astWarmDetail[k] = (COMDT_WARM_BATTLE_INFO)ProtocolObjectPool.Get(COMDT_WARM_BATTLE_INFO.CLASS_ID);
			}
			this.stLadderWarm = (COMDT_WARM_BATTLE_INFO)ProtocolObjectPool.Get(COMDT_WARM_BATTLE_INFO.CLASS_ID);
			this.stMultiExtra = (COMDT_STATISTIC_DATA_EXTRA_DETAIL)ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_EXTRA_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_STATISTIC_DATA_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_STATISTIC_DATA_DETAIL.CURRVERSION;
			}
			if (COMDT_STATISTIC_DATA_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bSingleNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bSingleNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astSingleDetail.Length < (int)this.bSingleNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bSingleNum; i++)
			{
				errorType = this.astSingleDetail[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt8(this.bMultiNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (40 < this.bMultiNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astMultiDetail.Length < (int)this.bMultiNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int j = 0; j < (int)this.bMultiNum; j++)
			{
				errorType = this.astMultiDetail[j].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stKVDetail.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_bWarmNum <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bWarmNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_astWarmDetail <= cutVer)
			{
				if (10 < this.bWarmNum)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				if (this.astWarmDetail.Length < (int)this.bWarmNum)
				{
					return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
				}
				for (int k = 0; k < (int)this.bWarmNum; k++)
				{
					errorType = this.astWarmDetail[k].pack(ref destBuf, cutVer);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_dwNormalMMRContinuousWinNum <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwNormalMMRContinuousWinNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_dwNormalMMRContinuousLoseNum <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwNormalMMRContinuousLoseNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_dwNormalMMRWinNum <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwNormalMMRWinNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_dwNormalMMRLoseNum <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwNormalMMRLoseNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_stLadderWarm <= cutVer)
			{
				errorType = this.stLadderWarm.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_stMultiExtra <= cutVer)
			{
				errorType = this.stMultiExtra.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_STATISTIC_DATA_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_STATISTIC_DATA_DETAIL.CURRVERSION;
			}
			if (COMDT_STATISTIC_DATA_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bSingleNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bSingleNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bSingleNum; i++)
			{
				errorType = this.astSingleDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bMultiNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (40 < this.bMultiNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int j = 0; j < (int)this.bMultiNum; j++)
			{
				errorType = this.astMultiDetail[j].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stKVDetail.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_bWarmNum <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bWarmNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bWarmNum = 0;
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_astWarmDetail <= cutVer)
			{
				if (10 < this.bWarmNum)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				for (int k = 0; k < (int)this.bWarmNum; k++)
				{
					errorType = this.astWarmDetail[k].unpack(ref srcBuf, cutVer);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			else
			{
				if (10 < this.bWarmNum)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				for (int l = 0; l < (int)this.bWarmNum; l++)
				{
					errorType = this.astWarmDetail[l].construct();
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_dwNormalMMRContinuousWinNum <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwNormalMMRContinuousWinNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwNormalMMRContinuousWinNum = 0u;
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_dwNormalMMRContinuousLoseNum <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwNormalMMRContinuousLoseNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwNormalMMRContinuousLoseNum = 0u;
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_dwNormalMMRWinNum <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwNormalMMRWinNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwNormalMMRWinNum = 0u;
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_dwNormalMMRLoseNum <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwNormalMMRLoseNum);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwNormalMMRLoseNum = 0u;
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_stLadderWarm <= cutVer)
			{
				errorType = this.stLadderWarm.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stLadderWarm.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_STATISTIC_DATA_DETAIL.VERSION_stMultiExtra <= cutVer)
			{
				errorType = this.stMultiExtra.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stMultiExtra.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_STATISTIC_DATA_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bSingleNum = 0;
			if (this.astSingleDetail != null)
			{
				for (int i = 0; i < this.astSingleDetail.Length; i++)
				{
					if (this.astSingleDetail[i] != null)
					{
						this.astSingleDetail[i].Release();
						this.astSingleDetail[i] = null;
					}
				}
			}
			this.bMultiNum = 0;
			if (this.astMultiDetail != null)
			{
				for (int j = 0; j < this.astMultiDetail.Length; j++)
				{
					if (this.astMultiDetail[j] != null)
					{
						this.astMultiDetail[j].Release();
						this.astMultiDetail[j] = null;
					}
				}
			}
			if (this.stKVDetail != null)
			{
				this.stKVDetail.Release();
				this.stKVDetail = null;
			}
			this.bWarmNum = 0;
			if (this.astWarmDetail != null)
			{
				for (int k = 0; k < this.astWarmDetail.Length; k++)
				{
					if (this.astWarmDetail[k] != null)
					{
						this.astWarmDetail[k].Release();
						this.astWarmDetail[k] = null;
					}
				}
			}
			this.dwNormalMMRContinuousWinNum = 0u;
			this.dwNormalMMRContinuousLoseNum = 0u;
			this.dwNormalMMRWinNum = 0u;
			this.dwNormalMMRLoseNum = 0u;
			if (this.stLadderWarm != null)
			{
				this.stLadderWarm.Release();
				this.stLadderWarm = null;
			}
			if (this.stMultiExtra != null)
			{
				this.stMultiExtra.Release();
				this.stMultiExtra = null;
			}
		}

		public override void OnUse()
		{
			if (this.astSingleDetail != null)
			{
				for (int i = 0; i < this.astSingleDetail.Length; i++)
				{
					this.astSingleDetail[i] = (COMDT_STATISTIC_DATA_INFO_SINGLE)ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_INFO_SINGLE.CLASS_ID);
				}
			}
			if (this.astMultiDetail != null)
			{
				for (int j = 0; j < this.astMultiDetail.Length; j++)
				{
					this.astMultiDetail[j] = (COMDT_STATISTIC_DATA_INFO_MULTI)ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_INFO_MULTI.CLASS_ID);
				}
			}
			this.stKVDetail = (COMDT_STATISTIC_KEY_VALUE_DETAIL)ProtocolObjectPool.Get(COMDT_STATISTIC_KEY_VALUE_DETAIL.CLASS_ID);
			if (this.astWarmDetail != null)
			{
				for (int k = 0; k < this.astWarmDetail.Length; k++)
				{
					this.astWarmDetail[k] = (COMDT_WARM_BATTLE_INFO)ProtocolObjectPool.Get(COMDT_WARM_BATTLE_INFO.CLASS_ID);
				}
			}
			this.stLadderWarm = (COMDT_WARM_BATTLE_INFO)ProtocolObjectPool.Get(COMDT_WARM_BATTLE_INFO.CLASS_ID);
			this.stMultiExtra = (COMDT_STATISTIC_DATA_EXTRA_DETAIL)ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_EXTRA_DETAIL.CLASS_ID);
		}
	}
}
