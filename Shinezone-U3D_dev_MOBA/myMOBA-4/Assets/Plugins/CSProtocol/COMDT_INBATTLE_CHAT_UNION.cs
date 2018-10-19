using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_INBATTLE_CHAT_UNION : ProtocolObject
	{
		public ProtocolObject dataObject;

		public byte bReverse;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 288;

		public COMDT_INBATTLE_CHAT_ID stSignalID
		{
			get
			{
				return this.dataObject as COMDT_INBATTLE_CHAT_ID;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_INBATTLE_CHAT_ID stBubbleID
		{
			get
			{
				return this.dataObject as COMDT_INBATTLE_CHAT_ID;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_INBATTLE_CHAT_STR stContentStr
		{
			get
			{
				return this.dataObject as COMDT_INBATTLE_CHAT_STR;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_INBATTLE_CHAT_STR stSelfDefineStr
		{
			get
			{
				return this.dataObject as COMDT_INBATTLE_CHAT_STR;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 4L)
			{
				this.select_1_4(selector);
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
			this.bReverse = 0;
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
			if (cutVer == 0u || COMDT_INBATTLE_CHAT_UNION.CURRVERSION < cutVer)
			{
				cutVer = COMDT_INBATTLE_CHAT_UNION.CURRVERSION;
			}
			if (COMDT_INBATTLE_CHAT_UNION.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.pack(ref destBuf, cutVer);
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bReverse);
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
			if (cutVer == 0u || COMDT_INBATTLE_CHAT_UNION.CURRVERSION < cutVer)
			{
				cutVer = COMDT_INBATTLE_CHAT_UNION.CURRVERSION;
			}
			if (COMDT_INBATTLE_CHAT_UNION.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.unpack(ref srcBuf, cutVer);
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bReverse);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		private void select_1_4(long selector)
		{
			if (selector >= 1L && selector <= 4L)
			{
				switch ((int)(selector - 1L))
				{
				case 0:
					if (!(this.dataObject is COMDT_INBATTLE_CHAT_ID))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_INBATTLE_CHAT_ID)ProtocolObjectPool.Get(COMDT_INBATTLE_CHAT_ID.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is COMDT_INBATTLE_CHAT_ID))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_INBATTLE_CHAT_ID)ProtocolObjectPool.Get(COMDT_INBATTLE_CHAT_ID.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is COMDT_INBATTLE_CHAT_STR))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_INBATTLE_CHAT_STR)ProtocolObjectPool.Get(COMDT_INBATTLE_CHAT_STR.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is COMDT_INBATTLE_CHAT_STR))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_INBATTLE_CHAT_STR)ProtocolObjectPool.Get(COMDT_INBATTLE_CHAT_STR.CLASS_ID);
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
			return COMDT_INBATTLE_CHAT_UNION.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			this.bReverse = 0;
		}
	}
}
