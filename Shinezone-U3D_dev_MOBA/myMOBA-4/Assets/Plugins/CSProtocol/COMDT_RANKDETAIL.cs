using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_RANKDETAIL : ProtocolObject
	{
		public int iMMR;

		public uint dwTotalFightCnt;

		public uint dwTotalWinCnt;

		public uint dwScore;

		public byte bState;

		public uint dwContinuousWin;

		public uint dwContinuousLose;

		public COMDT_RANK_GRADEUP stGradeUp;

		public COMDT_RANK_GRADEDOWN stGradeDown;

		public uint dwSeasonIdx;

		public uint dwSeasonStartTime;

		public uint dwSeasonEndTime;

		public uint dwMaxContinuousWinCnt;

		public byte bGetReward;

		public byte bSubState;

		public byte bMaxSeasonGrade;

		public uint dwLastActiveTime;

		public uint dwAddScoreOfConWinCnt;

		public uint dwMaxSeasonClass;

		public uint dwTopClassOfRank;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 121u;

		public static readonly uint VERSION_dwSeasonIdx = 33u;

		public static readonly uint VERSION_dwSeasonStartTime = 33u;

		public static readonly uint VERSION_dwSeasonEndTime = 33u;

		public static readonly uint VERSION_dwMaxContinuousWinCnt = 33u;

		public static readonly uint VERSION_bGetReward = 33u;

		public static readonly uint VERSION_bSubState = 33u;

		public static readonly uint VERSION_bMaxSeasonGrade = 72u;

		public static readonly uint VERSION_dwLastActiveTime = 103u;

		public static readonly uint VERSION_dwAddScoreOfConWinCnt = 103u;

		public static readonly uint VERSION_dwMaxSeasonClass = 121u;

		public static readonly uint VERSION_dwTopClassOfRank = 121u;

		public static readonly int CLASS_ID = 187;

		public COMDT_RANKDETAIL()
		{
			this.stGradeUp = (COMDT_RANK_GRADEUP)ProtocolObjectPool.Get(COMDT_RANK_GRADEUP.CLASS_ID);
			this.stGradeDown = (COMDT_RANK_GRADEDOWN)ProtocolObjectPool.Get(COMDT_RANK_GRADEDOWN.CLASS_ID);
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
			if (cutVer == 0u || COMDT_RANKDETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKDETAIL.CURRVERSION;
			}
			if (COMDT_RANKDETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iMMR);
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
			errorType = destBuf.writeUInt32(this.dwScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bState);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwContinuousWin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwContinuousLose);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGradeUp.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGradeDown.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_RANKDETAIL.VERSION_dwSeasonIdx <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwSeasonIdx);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKDETAIL.VERSION_dwSeasonStartTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwSeasonStartTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKDETAIL.VERSION_dwSeasonEndTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwSeasonEndTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKDETAIL.VERSION_dwMaxContinuousWinCnt <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwMaxContinuousWinCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKDETAIL.VERSION_bGetReward <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bGetReward);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKDETAIL.VERSION_bSubState <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bSubState);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKDETAIL.VERSION_bMaxSeasonGrade <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bMaxSeasonGrade);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKDETAIL.VERSION_dwLastActiveTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLastActiveTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKDETAIL.VERSION_dwAddScoreOfConWinCnt <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwAddScoreOfConWinCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKDETAIL.VERSION_dwMaxSeasonClass <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwMaxSeasonClass);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_RANKDETAIL.VERSION_dwTopClassOfRank <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwTopClassOfRank);
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
			if (cutVer == 0u || COMDT_RANKDETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKDETAIL.CURRVERSION;
			}
			if (COMDT_RANKDETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iMMR);
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
			errorType = srcBuf.readUInt32(ref this.dwScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bState);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwContinuousWin);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwContinuousLose);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGradeUp.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGradeDown.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_RANKDETAIL.VERSION_dwSeasonIdx <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwSeasonIdx);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwSeasonIdx = 0u;
			}
			if (COMDT_RANKDETAIL.VERSION_dwSeasonStartTime <= cutVer)
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
			if (COMDT_RANKDETAIL.VERSION_dwSeasonEndTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwSeasonEndTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwSeasonEndTime = 0u;
			}
			if (COMDT_RANKDETAIL.VERSION_dwMaxContinuousWinCnt <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwMaxContinuousWinCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwMaxContinuousWinCnt = 0u;
			}
			if (COMDT_RANKDETAIL.VERSION_bGetReward <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bGetReward);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bGetReward = 0;
			}
			if (COMDT_RANKDETAIL.VERSION_bSubState <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bSubState);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bSubState = 0;
			}
			if (COMDT_RANKDETAIL.VERSION_bMaxSeasonGrade <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bMaxSeasonGrade);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bMaxSeasonGrade = 0;
			}
			if (COMDT_RANKDETAIL.VERSION_dwLastActiveTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLastActiveTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLastActiveTime = 0u;
			}
			if (COMDT_RANKDETAIL.VERSION_dwAddScoreOfConWinCnt <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwAddScoreOfConWinCnt);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwAddScoreOfConWinCnt = 0u;
			}
			if (COMDT_RANKDETAIL.VERSION_dwMaxSeasonClass <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwMaxSeasonClass);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwMaxSeasonClass = 0u;
			}
			if (COMDT_RANKDETAIL.VERSION_dwTopClassOfRank <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwTopClassOfRank);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwTopClassOfRank = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_RANKDETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iMMR = 0;
			this.dwTotalFightCnt = 0u;
			this.dwTotalWinCnt = 0u;
			this.dwScore = 0u;
			this.bState = 0;
			this.dwContinuousWin = 0u;
			this.dwContinuousLose = 0u;
			if (this.stGradeUp != null)
			{
				this.stGradeUp.Release();
				this.stGradeUp = null;
			}
			if (this.stGradeDown != null)
			{
				this.stGradeDown.Release();
				this.stGradeDown = null;
			}
			this.dwSeasonIdx = 0u;
			this.dwSeasonStartTime = 0u;
			this.dwSeasonEndTime = 0u;
			this.dwMaxContinuousWinCnt = 0u;
			this.bGetReward = 0;
			this.bSubState = 0;
			this.bMaxSeasonGrade = 0;
			this.dwLastActiveTime = 0u;
			this.dwAddScoreOfConWinCnt = 0u;
			this.dwMaxSeasonClass = 0u;
			this.dwTopClassOfRank = 0u;
		}

		public override void OnUse()
		{
			this.stGradeUp = (COMDT_RANK_GRADEUP)ProtocolObjectPool.Get(COMDT_RANK_GRADEUP.CLASS_ID);
			this.stGradeDown = (COMDT_RANK_GRADEDOWN)ProtocolObjectPool.Get(COMDT_RANK_GRADEDOWN.CLASS_ID);
		}
	}
}
