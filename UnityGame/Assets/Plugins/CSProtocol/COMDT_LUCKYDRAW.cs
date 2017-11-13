using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_LUCKYDRAW : ProtocolObject
	{
		public uint dwNextRefreshTime;

		public COMDT_LUCKYDRAW_INFO stDiamond;

		public COMDT_LUCKYDRAW_INFO stCoupons;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 47u;

		public static readonly int CLASS_ID = 393;

		public COMDT_LUCKYDRAW()
		{
			this.stDiamond = (COMDT_LUCKYDRAW_INFO)ProtocolObjectPool.Get(COMDT_LUCKYDRAW_INFO.CLASS_ID);
			this.stCoupons = (COMDT_LUCKYDRAW_INFO)ProtocolObjectPool.Get(COMDT_LUCKYDRAW_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_LUCKYDRAW.CURRVERSION < cutVer)
			{
				cutVer = COMDT_LUCKYDRAW.CURRVERSION;
			}
			if (COMDT_LUCKYDRAW.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwNextRefreshTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stDiamond.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stCoupons.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_LUCKYDRAW.CURRVERSION < cutVer)
			{
				cutVer = COMDT_LUCKYDRAW.CURRVERSION;
			}
			if (COMDT_LUCKYDRAW.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwNextRefreshTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stDiamond.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stCoupons.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_LUCKYDRAW.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwNextRefreshTime = 0u;
			if (this.stDiamond != null)
			{
				this.stDiamond.Release();
				this.stDiamond = null;
			}
			if (this.stCoupons != null)
			{
				this.stCoupons.Release();
				this.stCoupons = null;
			}
		}

		public override void OnUse()
		{
			this.stDiamond = (COMDT_LUCKYDRAW_INFO)ProtocolObjectPool.Get(COMDT_LUCKYDRAW_INFO.CLASS_ID);
			this.stCoupons = (COMDT_LUCKYDRAW_INFO)ProtocolObjectPool.Get(COMDT_LUCKYDRAW_INFO.CLASS_ID);
		}
	}
}
