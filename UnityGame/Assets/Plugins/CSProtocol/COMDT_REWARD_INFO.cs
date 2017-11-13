using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_REWARD_INFO : ProtocolObject
	{
		public byte bType;

		public COMDT_REWARDS_UNION stRewardInfo;

		public byte bFromType;

		public COMDT_REWARDS_FROM stFromInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 223;

		public COMDT_REWARD_INFO()
		{
			this.stRewardInfo = (COMDT_REWARDS_UNION)ProtocolObjectPool.Get(COMDT_REWARDS_UNION.CLASS_ID);
			this.stFromInfo = (COMDT_REWARDS_FROM)ProtocolObjectPool.Get(COMDT_REWARDS_FROM.CLASS_ID);
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
			if (cutVer == 0u || COMDT_REWARD_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_REWARD_INFO.CURRVERSION;
			}
			if (COMDT_REWARD_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bType;
			errorType = this.stRewardInfo.pack(selector, ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bFromType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector2 = (long)this.bFromType;
			errorType = this.stFromInfo.pack(selector2, ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_REWARD_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_REWARD_INFO.CURRVERSION;
			}
			if (COMDT_REWARD_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bType;
			errorType = this.stRewardInfo.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bFromType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector2 = (long)this.bFromType;
			errorType = this.stFromInfo.unpack(selector2, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_REWARD_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bType = 0;
			if (this.stRewardInfo != null)
			{
				this.stRewardInfo.Release();
				this.stRewardInfo = null;
			}
			this.bFromType = 0;
			if (this.stFromInfo != null)
			{
				this.stFromInfo.Release();
				this.stFromInfo = null;
			}
		}

		public override void OnUse()
		{
			this.stRewardInfo = (COMDT_REWARDS_UNION)ProtocolObjectPool.Get(COMDT_REWARDS_UNION.CLASS_ID);
			this.stFromInfo = (COMDT_REWARDS_FROM)ProtocolObjectPool.Get(COMDT_REWARDS_FROM.CLASS_ID);
		}
	}
}
