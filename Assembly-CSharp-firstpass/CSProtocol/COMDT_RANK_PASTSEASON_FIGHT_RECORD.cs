using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_RANK_PASTSEASON_FIGHT_RECORD : ProtocolObject
	{
		public uint dwSeaStartTime;

		public uint dwSeaEndTime;

		public byte bGrade;

		public uint dwTotalFightCnt;

		public uint dwTotalWinCnt;

		public uint dwMaxContinuousWinCnt;

		public uint dwCommonUsedHeroNum;

		public COMDT_RANK_COMMON_USED_HERO[] astCommonUsedHeroInfo;

		public uint dwClassOfRank;

		public byte bMaxSesaonShowGrade;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly uint VERSION_dwClassOfRank = 121u;

		public static readonly uint VERSION_bMaxSesaonShowGrade = 241u;

		public static readonly int CLASS_ID = 446;

		public COMDT_RANK_PASTSEASON_FIGHT_RECORD()
		{
			this.astCommonUsedHeroInfo = new COMDT_RANK_COMMON_USED_HERO[5];
			for (int i = 0; i < 5; i++)
			{
				this.astCommonUsedHeroInfo[i] = (COMDT_RANK_COMMON_USED_HERO)ProtocolObjectPool.Get(COMDT_RANK_COMMON_USED_HERO.CLASS_ID);
			}
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
			if (cutVer == 0u || COMDT_RANK_PASTSEASON_FIGHT_RECORD.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANK_PASTSEASON_FIGHT_RECORD.CURRVERSION;
			}
			if (COMDT_RANK_PASTSEASON_FIGHT_RECORD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwSeaStartTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwSeaEndTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bGrade);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTotalFightCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTotalWinCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwMaxContinuousWinCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwCommonUsedHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5u < this.dwCommonUsedHeroNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astCommonUsedHeroInfo.Length < (long)((ulong)this.dwCommonUsedHeroNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwCommonUsedHeroNum))
			{
				errorType = this.astCommonUsedHeroInfo[num].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			if (COMDT_RANK_PASTSEASON_FIGHT_RECORD.VERSION_dwClassOfRank <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwClassOfRank);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANK_PASTSEASON_FIGHT_RECORD.VERSION_bMaxSesaonShowGrade <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bMaxSesaonShowGrade);
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
			if (cutVer == 0u || COMDT_RANK_PASTSEASON_FIGHT_RECORD.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANK_PASTSEASON_FIGHT_RECORD.CURRVERSION;
			}
			if (COMDT_RANK_PASTSEASON_FIGHT_RECORD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwSeaStartTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSeaEndTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bGrade);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTotalFightCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTotalWinCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMaxContinuousWinCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwCommonUsedHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5u < this.dwCommonUsedHeroNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwCommonUsedHeroNum))
			{
				errorType = this.astCommonUsedHeroInfo[num].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			if (COMDT_RANK_PASTSEASON_FIGHT_RECORD.VERSION_dwClassOfRank <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwClassOfRank);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwClassOfRank = 0u;
			}
			if (COMDT_RANK_PASTSEASON_FIGHT_RECORD.VERSION_bMaxSesaonShowGrade <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bMaxSesaonShowGrade);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bMaxSesaonShowGrade = 0;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_RANK_PASTSEASON_FIGHT_RECORD.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwSeaStartTime = 0u;
			this.dwSeaEndTime = 0u;
			this.bGrade = 0;
			this.dwTotalFightCnt = 0u;
			this.dwTotalWinCnt = 0u;
			this.dwMaxContinuousWinCnt = 0u;
			this.dwCommonUsedHeroNum = 0u;
			if (this.astCommonUsedHeroInfo != null)
			{
				for (int i = 0; i < this.astCommonUsedHeroInfo.Length; i++)
				{
					if (this.astCommonUsedHeroInfo[i] != null)
					{
						this.astCommonUsedHeroInfo[i].Release();
						this.astCommonUsedHeroInfo[i] = null;
					}
				}
			}
			this.dwClassOfRank = 0u;
			this.bMaxSesaonShowGrade = 0;
		}

		public override void OnUse()
		{
			if (this.astCommonUsedHeroInfo != null)
			{
				for (int i = 0; i < this.astCommonUsedHeroInfo.Length; i++)
				{
					this.astCommonUsedHeroInfo[i] = (COMDT_RANK_COMMON_USED_HERO)ProtocolObjectPool.Get(COMDT_RANK_COMMON_USED_HERO.CLASS_ID);
				}
			}
		}
	}
}
