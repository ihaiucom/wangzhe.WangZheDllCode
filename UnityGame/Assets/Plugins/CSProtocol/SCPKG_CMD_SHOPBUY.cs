using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_CMD_SHOPBUY : ProtocolObject
	{
		public int iBuyType;

		public int iBuySubType;

		public COMDT_SHOPBUY_EXTRA stExtraInfo;

		public CSDT_SHOPBUY_REWARDLIST stBuyResult;

		public int iChgValue;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 944;

		public SCPKG_CMD_SHOPBUY()
		{
			this.stExtraInfo = (COMDT_SHOPBUY_EXTRA)ProtocolObjectPool.Get(COMDT_SHOPBUY_EXTRA.CLASS_ID);
			this.stBuyResult = (CSDT_SHOPBUY_REWARDLIST)ProtocolObjectPool.Get(CSDT_SHOPBUY_REWARDLIST.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_CMD_SHOPBUY.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_SHOPBUY.CURRVERSION;
			}
			if (SCPKG_CMD_SHOPBUY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iBuyType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iBuySubType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.iBuyType;
			errorType = this.stExtraInfo.pack(selector, ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBuyResult.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iChgValue);
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
			if (cutVer == 0u || SCPKG_CMD_SHOPBUY.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_SHOPBUY.CURRVERSION;
			}
			if (SCPKG_CMD_SHOPBUY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iBuyType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBuySubType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.iBuyType;
			errorType = this.stExtraInfo.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBuyResult.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iChgValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_CMD_SHOPBUY.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iBuyType = 0;
			this.iBuySubType = 0;
			if (this.stExtraInfo != null)
			{
				this.stExtraInfo.Release();
				this.stExtraInfo = null;
			}
			if (this.stBuyResult != null)
			{
				this.stBuyResult.Release();
				this.stBuyResult = null;
			}
			this.iChgValue = 0;
		}

		public override void OnUse()
		{
			this.stExtraInfo = (COMDT_SHOPBUY_EXTRA)ProtocolObjectPool.Get(COMDT_SHOPBUY_EXTRA.CLASS_ID);
			this.stBuyResult = (CSDT_SHOPBUY_REWARDLIST)ProtocolObjectPool.Get(CSDT_SHOPBUY_REWARDLIST.CLASS_ID);
		}
	}
}
