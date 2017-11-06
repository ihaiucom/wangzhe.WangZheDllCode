using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_GET_BURNING_REWARD_RSP : ProtocolObject
	{
		public int iErrCode;

		public CSDT_BURNING_REWARD_DETAIL stRewardDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 1299;

		public SCPKG_GET_BURNING_REWARD_RSP()
		{
			this.stRewardDetail = (CSDT_BURNING_REWARD_DETAIL)ProtocolObjectPool.Get(CSDT_BURNING_REWARD_DETAIL.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			this.iErrCode = 0;
			long selector = (long)this.iErrCode;
			TdrError.ErrorType errorType = this.stRewardDetail.construct(selector);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
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
			if (cutVer == 0u || SCPKG_GET_BURNING_REWARD_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GET_BURNING_REWARD_RSP.CURRVERSION;
			}
			if (SCPKG_GET_BURNING_REWARD_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iErrCode);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.iErrCode;
			errorType = this.stRewardDetail.pack(selector, ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_GET_BURNING_REWARD_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GET_BURNING_REWARD_RSP.CURRVERSION;
			}
			if (SCPKG_GET_BURNING_REWARD_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iErrCode);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.iErrCode;
			errorType = this.stRewardDetail.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_GET_BURNING_REWARD_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iErrCode = 0;
			if (this.stRewardDetail != null)
			{
				this.stRewardDetail.Release();
				this.stRewardDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stRewardDetail = (CSDT_BURNING_REWARD_DETAIL)ProtocolObjectPool.Get(CSDT_BURNING_REWARD_DETAIL.CLASS_ID);
		}
	}
}
