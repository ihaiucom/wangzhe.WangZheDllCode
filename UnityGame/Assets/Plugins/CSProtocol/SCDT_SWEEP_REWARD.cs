using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCDT_SWEEP_REWARD : ProtocolObject
	{
		public uint dwAcntExp;

		public uint dwGold;

		public uint dwSweepCnt;

		public COMDT_REWARD_DETAIL[] astRewardDetail;

		public ushort wMultipleApplyCnt;

		public COMDT_REWARD_MULTIPLE_DETAIL stMultipleDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 175u;

		public static readonly int CLASS_ID = 797;

		public SCDT_SWEEP_REWARD()
		{
			this.astRewardDetail = new COMDT_REWARD_DETAIL[10];
			for (int i = 0; i < 10; i++)
			{
				this.astRewardDetail[i] = (COMDT_REWARD_DETAIL)ProtocolObjectPool.Get(COMDT_REWARD_DETAIL.CLASS_ID);
			}
			this.stMultipleDetail = (COMDT_REWARD_MULTIPLE_DETAIL)ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || SCDT_SWEEP_REWARD.CURRVERSION < cutVer)
			{
				cutVer = SCDT_SWEEP_REWARD.CURRVERSION;
			}
			if (SCDT_SWEEP_REWARD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwAcntExp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwGold);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwSweepCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10u < this.dwSweepCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astRewardDetail.Length < (long)((ulong)this.dwSweepCnt))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwSweepCnt))
			{
				errorType = this.astRewardDetail[num].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			errorType = destBuf.writeUInt16(this.wMultipleApplyCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMultipleDetail.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCDT_SWEEP_REWARD.CURRVERSION < cutVer)
			{
				cutVer = SCDT_SWEEP_REWARD.CURRVERSION;
			}
			if (SCDT_SWEEP_REWARD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwAcntExp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGold);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwSweepCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10u < this.dwSweepCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwSweepCnt))
			{
				errorType = this.astRewardDetail[num].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			errorType = srcBuf.readUInt16(ref this.wMultipleApplyCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMultipleDetail.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCDT_SWEEP_REWARD.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwAcntExp = 0u;
			this.dwGold = 0u;
			this.dwSweepCnt = 0u;
			if (this.astRewardDetail != null)
			{
				for (int i = 0; i < this.astRewardDetail.Length; i++)
				{
					if (this.astRewardDetail[i] != null)
					{
						this.astRewardDetail[i].Release();
						this.astRewardDetail[i] = null;
					}
				}
			}
			this.wMultipleApplyCnt = 0;
			if (this.stMultipleDetail != null)
			{
				this.stMultipleDetail.Release();
				this.stMultipleDetail = null;
			}
		}

		public override void OnUse()
		{
			if (this.astRewardDetail != null)
			{
				for (int i = 0; i < this.astRewardDetail.Length; i++)
				{
					this.astRewardDetail[i] = (COMDT_REWARD_DETAIL)ProtocolObjectPool.Get(COMDT_REWARD_DETAIL.CLASS_ID);
				}
			}
			this.stMultipleDetail = (COMDT_REWARD_MULTIPLE_DETAIL)ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_DETAIL.CLASS_ID);
		}
	}
}
