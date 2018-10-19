using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSPkg : ProtocolObject
	{
		public CSPkgHead stPkgHead;

		public CSPkgBody stPkgData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 242u;

		public static readonly int CLASS_ID = 1647;

		public CSPkg()
		{
			this.stPkgHead = (CSPkgHead)ProtocolObjectPool.Get(CSPkgHead.CLASS_ID);
			this.stPkgData = (CSPkgBody)ProtocolObjectPool.Get(CSPkgBody.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = this.stPkgHead.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)((ulong)this.stPkgHead.dwMsgID);
			errorType = this.stPkgData.construct(selector);
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
			if (cutVer == 0u || CSPkg.CURRVERSION < cutVer)
			{
				cutVer = CSPkg.CURRVERSION;
			}
			if (CSPkg.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stPkgHead.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)((ulong)this.stPkgHead.dwMsgID);
			errorType = this.stPkgData.pack(selector, ref destBuf, cutVer);
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
			if (cutVer == 0u || CSPkg.CURRVERSION < cutVer)
			{
				cutVer = CSPkg.CURRVERSION;
			}
			if (CSPkg.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stPkgHead.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)((ulong)this.stPkgHead.dwMsgID);
			errorType = this.stPkgData.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSPkg.CLASS_ID;
		}

		public static CSPkg New()
		{
			return (CSPkg)ProtocolObjectPool.Get(CSPkg.CLASS_ID);
		}

		private void ClearVariables()
		{
			if (this.stPkgHead != null)
			{
				this.stPkgHead.Release();
				this.stPkgHead = null;
			}
			if (this.stPkgData != null)
			{
				this.stPkgData.Release();
				this.stPkgData = null;
			}
		}

		public override void OnRelease()
		{
			this.ClearVariables();
		}

		public override void OnUse()
		{
			this.ClearVariables();
			this.stPkgHead = (CSPkgHead)ProtocolObjectPool.Get(CSPkgHead.CLASS_ID);
			this.stPkgData = (CSPkgBody)ProtocolObjectPool.Get(CSPkgBody.CLASS_ID);
		}
	}
}
