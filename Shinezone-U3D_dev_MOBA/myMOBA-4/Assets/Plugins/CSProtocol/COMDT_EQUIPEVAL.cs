using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_EQUIPEVAL : ProtocolObject
	{
		public uint dwEvalNum;

		public byte bEvalInfoCnt;

		public COMDT_EQUIP_EVALINFO[] astEvalInfo;

		public uint dwTotalScore;

		public byte bScoreCnt;

		public uint[] ScoreStatistic;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 170u;

		public static readonly uint VERSION_bScoreCnt = 170u;

		public static readonly uint VERSION_ScoreStatistic = 170u;

		public static readonly int CLASS_ID = 590;

		public COMDT_EQUIPEVAL()
		{
			this.astEvalInfo = new COMDT_EQUIP_EVALINFO[20];
			for (int i = 0; i < 20; i++)
			{
				this.astEvalInfo[i] = (COMDT_EQUIP_EVALINFO)ProtocolObjectPool.Get(COMDT_EQUIP_EVALINFO.CLASS_ID);
			}
			this.ScoreStatistic = new uint[5];
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
			if (cutVer == 0u || COMDT_EQUIPEVAL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_EQUIPEVAL.CURRVERSION;
			}
			if (COMDT_EQUIPEVAL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwEvalNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bEvalInfoCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bEvalInfoCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astEvalInfo.Length < (int)this.bEvalInfoCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bEvalInfoCnt; i++)
			{
				errorType = this.astEvalInfo[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt32(this.dwTotalScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_EQUIPEVAL.VERSION_bScoreCnt <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bScoreCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_EQUIPEVAL.VERSION_ScoreStatistic <= cutVer)
			{
				if (5 < this.bScoreCnt)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				if (this.ScoreStatistic.Length < (int)this.bScoreCnt)
				{
					return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
				}
				for (int j = 0; j < (int)this.bScoreCnt; j++)
				{
					errorType = destBuf.writeUInt32(this.ScoreStatistic[j]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
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
			if (cutVer == 0u || COMDT_EQUIPEVAL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_EQUIPEVAL.CURRVERSION;
			}
			if (COMDT_EQUIPEVAL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwEvalNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bEvalInfoCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bEvalInfoCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bEvalInfoCnt; i++)
			{
				errorType = this.astEvalInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt32(ref this.dwTotalScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_EQUIPEVAL.VERSION_bScoreCnt <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bScoreCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bScoreCnt = 0;
			}
			if (COMDT_EQUIPEVAL.VERSION_ScoreStatistic <= cutVer)
			{
				if (5 < this.bScoreCnt)
				{
					return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
				}
				this.ScoreStatistic = new uint[(int)this.bScoreCnt];
				for (int j = 0; j < (int)this.bScoreCnt; j++)
				{
					errorType = srcBuf.readUInt32(ref this.ScoreStatistic[j]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			else if (5 < this.bScoreCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_EQUIPEVAL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwEvalNum = 0u;
			this.bEvalInfoCnt = 0;
			if (this.astEvalInfo != null)
			{
				for (int i = 0; i < this.astEvalInfo.Length; i++)
				{
					if (this.astEvalInfo[i] != null)
					{
						this.astEvalInfo[i].Release();
						this.astEvalInfo[i] = null;
					}
				}
			}
			this.dwTotalScore = 0u;
			this.bScoreCnt = 0;
		}

		public override void OnUse()
		{
			if (this.astEvalInfo != null)
			{
				for (int i = 0; i < this.astEvalInfo.Length; i++)
				{
					this.astEvalInfo[i] = (COMDT_EQUIP_EVALINFO)ProtocolObjectPool.Get(COMDT_EQUIP_EVALINFO.CLASS_ID);
				}
			}
		}
	}
}
