using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKRSP : ProtocolObject
	{
		public byte bErrCode;

		public COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANK_DETAIL stDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 240u;

		public static readonly int CLASS_ID = 694;

		public COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKRSP()
		{
			this.stDetail = (COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANK_DETAIL)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANK_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKRSP.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKRSP.CURRVERSION;
			}
			if (COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKRSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bErrCode);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bErrCode;
			errorType = this.stDetail.pack(selector, ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKRSP.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKRSP.CURRVERSION;
			}
			if (COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKRSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bErrCode);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bErrCode;
			errorType = this.stDetail.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKRSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bErrCode = 0;
			if (this.stDetail != null)
			{
				this.stDetail.Release();
				this.stDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stDetail = (COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANK_DETAIL)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANK_DETAIL.CLASS_ID);
		}
	}
}
