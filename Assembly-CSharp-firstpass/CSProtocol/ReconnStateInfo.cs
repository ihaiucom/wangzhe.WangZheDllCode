using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class ReconnStateInfo : ProtocolObject
	{
		public ProtocolObject dataObject;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 1288;

		public CSDT_RECONN_BANINFO stBanInfo
		{
			get
			{
				return this.dataObject as CSDT_RECONN_BANINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_RECONN_PICKINFO stPickInfo
		{
			get
			{
				return this.dataObject as CSDT_RECONN_PICKINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_RECONN_ADJUSTINFO stAdjustInfo
		{
			get
			{
				return this.dataObject as CSDT_RECONN_ADJUSTINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_RECONN_LOADINGINFO stLoadingInfo
		{
			get
			{
				return this.dataObject as CSDT_RECONN_LOADINGINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_RECONN_GAMEINGINFO stGamingInfo
		{
			get
			{
				return this.dataObject as CSDT_RECONN_GAMEINGINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 5L)
			{
				this.select_1_5(selector);
			}
			else if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			return this.dataObject;
		}

		public TdrError.ErrorType construct(long selector)
		{
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.construct();
			}
			return result;
		}

		public TdrError.ErrorType pack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = ClassObjPool<TdrWriteBuf>.Get();
			tdrWriteBuf.set(ref buffer, size);
			TdrError.ErrorType errorType = this.pack(selector, ref tdrWriteBuf, cutVer);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				buffer = tdrWriteBuf.getBeginPtr();
				usedSize = tdrWriteBuf.getUsedSize();
			}
			tdrWriteBuf.Release();
			return errorType;
		}

		public TdrError.ErrorType pack(long selector, ref TdrWriteBuf destBuf, uint cutVer)
		{
			if (cutVer == 0u || ReconnStateInfo.CURRVERSION < cutVer)
			{
				cutVer = ReconnStateInfo.CURRVERSION;
			}
			if (ReconnStateInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.pack(ref destBuf, cutVer);
			}
			return result;
		}

		public TdrError.ErrorType unpack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = ClassObjPool<TdrReadBuf>.Get();
			tdrReadBuf.set(ref buffer, size);
			TdrError.ErrorType result = this.unpack(selector, ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			tdrReadBuf.Release();
			return result;
		}

		public TdrError.ErrorType unpack(long selector, ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || ReconnStateInfo.CURRVERSION < cutVer)
			{
				cutVer = ReconnStateInfo.CURRVERSION;
			}
			if (ReconnStateInfo.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.unpack(ref srcBuf, cutVer);
			}
			return result;
		}

		private void select_1_5(long selector)
		{
			if (selector >= 1L && selector <= 5L)
			{
				switch ((int)(selector - 1L))
				{
				case 0:
					if (!(this.dataObject is CSDT_RECONN_BANINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_RECONN_BANINFO)ProtocolObjectPool.Get(CSDT_RECONN_BANINFO.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is CSDT_RECONN_PICKINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_RECONN_PICKINFO)ProtocolObjectPool.Get(CSDT_RECONN_PICKINFO.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSDT_RECONN_ADJUSTINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_RECONN_ADJUSTINFO)ProtocolObjectPool.Get(CSDT_RECONN_ADJUSTINFO.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSDT_RECONN_LOADINGINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_RECONN_LOADINGINFO)ProtocolObjectPool.Get(CSDT_RECONN_LOADINGINFO.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSDT_RECONN_GAMEINGINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_RECONN_GAMEINGINFO)ProtocolObjectPool.Get(CSDT_RECONN_GAMEINGINFO.CLASS_ID);
					}
					return;
				}
			}
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}

		public override int GetClassID()
		{
			return ReconnStateInfo.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}
	}
}
