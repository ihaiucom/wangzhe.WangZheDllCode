using System;
using tsf4g_tdr_csharp;

namespace Assets.Scripts.Common
{
	public abstract class ProtocolObject
	{
		public abstract int GetClassID();

		public virtual TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public virtual TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public virtual TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public void Release()
		{
			this.OnRelease();
			ProtocolObjectPool.Release(this);
		}

		public virtual void OnUse()
		{
		}

		public virtual void OnRelease()
		{
		}
	}
}
