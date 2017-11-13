using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_GUILD_EVENT_INFO : ProtocolObject
	{
		public ProtocolObject dataObject;

		public byte bReserved;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 369;

		public COMDT_GUILD_EVENT_JOIN stJoinEvent
		{
			get
			{
				return this.dataObject as COMDT_GUILD_EVENT_JOIN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_GUILD_EVENT_QUIT stQuitEvent
		{
			get
			{
				return this.dataObject as COMDT_GUILD_EVENT_QUIT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_GUILD_EVENT_BEKICK stKickEvent
		{
			get
			{
				return this.dataObject as COMDT_GUILD_EVENT_BEKICK;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_GUILD_EVENT_PINGJIUP stPingjiEvent
		{
			get
			{
				return this.dataObject as COMDT_GUILD_EVENT_PINGJIUP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_GUILD_EVENT_MEMBERNUM_UP stMemberNumUpEvent
		{
			get
			{
				return this.dataObject as COMDT_GUILD_EVENT_MEMBERNUM_UP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_GUILD_EVENT_MEMBERUPTO_WANGZHE stUpToWangZheEvent
		{
			get
			{
				return this.dataObject as COMDT_GUILD_EVENT_MEMBERUPTO_WANGZHE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 7L)
			{
				this.select_1_7(selector);
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
			if (cutVer == 0u || COMDT_GUILD_EVENT_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GUILD_EVENT_INFO.CURRVERSION;
			}
			if (COMDT_GUILD_EVENT_INFO.BASEVERSION > cutVer)
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
			if (cutVer == 0u || COMDT_GUILD_EVENT_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GUILD_EVENT_INFO.CURRVERSION;
			}
			if (COMDT_GUILD_EVENT_INFO.BASEVERSION > cutVer)
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

		private void select_1_7(long selector)
		{
			if (selector >= 1L && selector <= 7L)
			{
				switch ((int)(selector - 1L))
				{
				case 0:
					if (!(this.dataObject is COMDT_GUILD_EVENT_JOIN))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_GUILD_EVENT_JOIN)ProtocolObjectPool.Get(COMDT_GUILD_EVENT_JOIN.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is COMDT_GUILD_EVENT_QUIT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_GUILD_EVENT_QUIT)ProtocolObjectPool.Get(COMDT_GUILD_EVENT_QUIT.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is COMDT_GUILD_EVENT_BEKICK))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_GUILD_EVENT_BEKICK)ProtocolObjectPool.Get(COMDT_GUILD_EVENT_BEKICK.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is COMDT_GUILD_EVENT_PINGJIUP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_GUILD_EVENT_PINGJIUP)ProtocolObjectPool.Get(COMDT_GUILD_EVENT_PINGJIUP.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is COMDT_GUILD_EVENT_MEMBERNUM_UP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_GUILD_EVENT_MEMBERNUM_UP)ProtocolObjectPool.Get(COMDT_GUILD_EVENT_MEMBERNUM_UP.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is COMDT_GUILD_EVENT_MEMBERUPTO_WANGZHE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_GUILD_EVENT_MEMBERUPTO_WANGZHE)ProtocolObjectPool.Get(COMDT_GUILD_EVENT_MEMBERUPTO_WANGZHE.CLASS_ID);
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
			return COMDT_GUILD_EVENT_INFO.CLASS_ID;
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
