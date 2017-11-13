using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCDT_NTF_ERRCODE : ProtocolObject
	{
		public ProtocolObject dataObject;

		public byte bReserved;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 711;

		public SCDT_NTF_REGISTERERRCODE stRegisterNameErrNtf
		{
			get
			{
				return this.dataObject as SCDT_NTF_REGISTERERRCODE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCDT_NTF_SWITCHDETAIL stSwitchErrNtf
		{
			get
			{
				return this.dataObject as SCDT_NTF_SWITCHDETAIL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCDT_NTF_REDIRECT_LOGICWORLDID stRedirectLogicWorldId
		{
			get
			{
				return this.dataObject as SCDT_NTF_REDIRECT_LOGICWORLDID;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 183L)
			{
				this.select_30_183(selector);
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
			this.bReserved = 0;
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
			if (cutVer == 0u || SCDT_NTF_ERRCODE.CURRVERSION < cutVer)
			{
				cutVer = SCDT_NTF_ERRCODE.CURRVERSION;
			}
			if (SCDT_NTF_ERRCODE.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.pack(ref destBuf, cutVer);
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bReserved);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
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
			if (cutVer == 0u || SCDT_NTF_ERRCODE.CURRVERSION < cutVer)
			{
				cutVer = SCDT_NTF_ERRCODE.CURRVERSION;
			}
			if (SCDT_NTF_ERRCODE.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.unpack(ref srcBuf, cutVer);
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bReserved);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		private void select_30_183(long selector)
		{
			if (selector != 182L)
			{
				if (selector != 183L)
				{
					if (selector != 30L)
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
							this.dataObject = null;
						}
					}
					else if (!(this.dataObject is SCDT_NTF_REGISTERERRCODE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCDT_NTF_REGISTERERRCODE)ProtocolObjectPool.Get(SCDT_NTF_REGISTERERRCODE.CLASS_ID);
					}
				}
				else if (!(this.dataObject is SCDT_NTF_REDIRECT_LOGICWORLDID))
				{
					if (this.dataObject != null)
					{
						this.dataObject.Release();
					}
					this.dataObject = (SCDT_NTF_REDIRECT_LOGICWORLDID)ProtocolObjectPool.Get(SCDT_NTF_REDIRECT_LOGICWORLDID.CLASS_ID);
				}
			}
			else if (!(this.dataObject is SCDT_NTF_SWITCHDETAIL))
			{
				if (this.dataObject != null)
				{
					this.dataObject.Release();
				}
				this.dataObject = (SCDT_NTF_SWITCHDETAIL)ProtocolObjectPool.Get(SCDT_NTF_SWITCHDETAIL.CLASS_ID);
			}
		}

		public override int GetClassID()
		{
			return SCDT_NTF_ERRCODE.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			this.bReserved = 0;
		}
	}
}
