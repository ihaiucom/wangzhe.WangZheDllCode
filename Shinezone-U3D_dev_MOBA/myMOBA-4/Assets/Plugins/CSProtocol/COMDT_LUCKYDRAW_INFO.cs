using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_LUCKYDRAW_INFO : ProtocolObject
	{
		public uint dwCnt;

		public uint dwReachMask;

		public uint dwDrawMask;

		public COMDT_LUCKYDRAW_MISS stNormalMiss;

		public COMDT_LUCKYDRAW_MISS stPeriodMiss;

		public uint dwLuckyPoint;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 47u;

		public static readonly uint VERSION_dwLuckyPoint = 47u;

		public static readonly int CLASS_ID = 392;

		public COMDT_LUCKYDRAW_INFO()
		{
			this.stNormalMiss = (COMDT_LUCKYDRAW_MISS)ProtocolObjectPool.Get(COMDT_LUCKYDRAW_MISS.CLASS_ID);
			this.stPeriodMiss = (COMDT_LUCKYDRAW_MISS)ProtocolObjectPool.Get(COMDT_LUCKYDRAW_MISS.CLASS_ID);
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
			if (cutVer == 0u || COMDT_LUCKYDRAW_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_LUCKYDRAW_INFO.CURRVERSION;
			}
			if (COMDT_LUCKYDRAW_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwReachMask);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwDrawMask);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stNormalMiss.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPeriodMiss.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_LUCKYDRAW_INFO.VERSION_dwLuckyPoint <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwLuckyPoint);
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
			if (cutVer == 0u || COMDT_LUCKYDRAW_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_LUCKYDRAW_INFO.CURRVERSION;
			}
			if (COMDT_LUCKYDRAW_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwReachMask);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwDrawMask);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stNormalMiss.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPeriodMiss.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_LUCKYDRAW_INFO.VERSION_dwLuckyPoint <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwLuckyPoint);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwLuckyPoint = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_LUCKYDRAW_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwCnt = 0u;
			this.dwReachMask = 0u;
			this.dwDrawMask = 0u;
			if (this.stNormalMiss != null)
			{
				this.stNormalMiss.Release();
				this.stNormalMiss = null;
			}
			if (this.stPeriodMiss != null)
			{
				this.stPeriodMiss.Release();
				this.stPeriodMiss = null;
			}
			this.dwLuckyPoint = 0u;
		}

		public override void OnUse()
		{
			this.stNormalMiss = (COMDT_LUCKYDRAW_MISS)ProtocolObjectPool.Get(COMDT_LUCKYDRAW_MISS.CLASS_ID);
			this.stPeriodMiss = (COMDT_LUCKYDRAW_MISS)ProtocolObjectPool.Get(COMDT_LUCKYDRAW_MISS.CLASS_ID);
		}
	}
}
