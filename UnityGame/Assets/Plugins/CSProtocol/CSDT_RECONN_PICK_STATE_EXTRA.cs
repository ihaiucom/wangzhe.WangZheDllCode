using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_RECONN_PICK_STATE_EXTRA : ProtocolObject
	{
		public byte bPickType;

		public CSDT_RECONN_PICK_STATE_EXTRA_DETAIL stPickDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1278;

		public CSDT_RECONN_PICK_STATE_EXTRA()
		{
			this.stPickDetail = (CSDT_RECONN_PICK_STATE_EXTRA_DETAIL)ProtocolObjectPool.Get(CSDT_RECONN_PICK_STATE_EXTRA_DETAIL.CLASS_ID);
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
			if (cutVer == 0u || CSDT_RECONN_PICK_STATE_EXTRA.CURRVERSION < cutVer)
			{
				cutVer = CSDT_RECONN_PICK_STATE_EXTRA.CURRVERSION;
			}
			if (CSDT_RECONN_PICK_STATE_EXTRA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bPickType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bPickType;
			errorType = this.stPickDetail.pack(selector, ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_RECONN_PICK_STATE_EXTRA.CURRVERSION < cutVer)
			{
				cutVer = CSDT_RECONN_PICK_STATE_EXTRA.CURRVERSION;
			}
			if (CSDT_RECONN_PICK_STATE_EXTRA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bPickType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bPickType;
			errorType = this.stPickDetail.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_RECONN_PICK_STATE_EXTRA.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bPickType = 0;
			if (this.stPickDetail != null)
			{
				this.stPickDetail.Release();
				this.stPickDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stPickDetail = (CSDT_RECONN_PICK_STATE_EXTRA_DETAIL)ProtocolObjectPool.Get(CSDT_RECONN_PICK_STATE_EXTRA_DETAIL.CLASS_ID);
		}
	}
}
