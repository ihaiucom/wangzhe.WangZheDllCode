using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_MAILACCESS_INFO : ProtocolObject
	{
		public ProtocolObject dataObject;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 173u;

		public static readonly uint VERSION_stExpHero = 57u;

		public static readonly uint VERSION_stExpSkin = 57u;

		public static readonly uint VERSION_stHeadImg = 83u;

		public static readonly uint VERSION_stMasterPoint = 173u;

		public static readonly int CLASS_ID = 273;

		public COMDT_MAILACCESS_PROP stProp
		{
			get
			{
				return this.dataObject as COMDT_MAILACCESS_PROP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_MAILACCESS_MONEY stMoney
		{
			get
			{
				return this.dataObject as COMDT_MAILACCESS_MONEY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_MAILACCESS_HEART stHeart
		{
			get
			{
				return this.dataObject as COMDT_MAILACCESS_HEART;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_MAILACCESS_RONGYU stRongYu
		{
			get
			{
				return this.dataObject as COMDT_MAILACCESS_RONGYU;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_MAILACCESS_EXP stExp
		{
			get
			{
				return this.dataObject as COMDT_MAILACCESS_EXP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_MAILACCESS_HERO stHero
		{
			get
			{
				return this.dataObject as COMDT_MAILACCESS_HERO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_MAILACCESS_PIFU stPiFu
		{
			get
			{
				return this.dataObject as COMDT_MAILACCESS_PIFU;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_MAILACCESS_EXPHERO stExpHero
		{
			get
			{
				return this.dataObject as COMDT_MAILACCESS_EXPHERO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_MAILACCESS_EXPSKIN stExpSkin
		{
			get
			{
				return this.dataObject as COMDT_MAILACCESS_EXPSKIN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_MAILACCESS_HEADIMG stHeadImg
		{
			get
			{
				return this.dataObject as COMDT_MAILACCESS_HEADIMG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_MAILACCESS_MASTERPOINT stMasterPoint
		{
			get
			{
				return this.dataObject as COMDT_MAILACCESS_MASTERPOINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 11L)
			{
				this.select_1_11(selector);
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
			if (cutVer == 0u || COMDT_MAILACCESS_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MAILACCESS_INFO.CURRVERSION;
			}
			if (COMDT_MAILACCESS_INFO.BASEVERSION > cutVer)
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
			if (cutVer == 0u || COMDT_MAILACCESS_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MAILACCESS_INFO.CURRVERSION;
			}
			if (COMDT_MAILACCESS_INFO.BASEVERSION > cutVer)
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

		private void select_1_11(long selector)
		{
			if (selector >= 1L && selector <= 11L)
			{
				switch ((int)(selector - 1L))
				{
				case 0:
					if (!(this.dataObject is COMDT_MAILACCESS_PROP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_MAILACCESS_PROP)ProtocolObjectPool.Get(COMDT_MAILACCESS_PROP.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is COMDT_MAILACCESS_MONEY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_MAILACCESS_MONEY)ProtocolObjectPool.Get(COMDT_MAILACCESS_MONEY.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is COMDT_MAILACCESS_HEART))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_MAILACCESS_HEART)ProtocolObjectPool.Get(COMDT_MAILACCESS_HEART.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is COMDT_MAILACCESS_RONGYU))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_MAILACCESS_RONGYU)ProtocolObjectPool.Get(COMDT_MAILACCESS_RONGYU.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is COMDT_MAILACCESS_EXP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_MAILACCESS_EXP)ProtocolObjectPool.Get(COMDT_MAILACCESS_EXP.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is COMDT_MAILACCESS_HERO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_MAILACCESS_HERO)ProtocolObjectPool.Get(COMDT_MAILACCESS_HERO.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is COMDT_MAILACCESS_PIFU))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_MAILACCESS_PIFU)ProtocolObjectPool.Get(COMDT_MAILACCESS_PIFU.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is COMDT_MAILACCESS_EXPHERO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_MAILACCESS_EXPHERO)ProtocolObjectPool.Get(COMDT_MAILACCESS_EXPHERO.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is COMDT_MAILACCESS_EXPSKIN))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_MAILACCESS_EXPSKIN)ProtocolObjectPool.Get(COMDT_MAILACCESS_EXPSKIN.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is COMDT_MAILACCESS_HEADIMG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_MAILACCESS_HEADIMG)ProtocolObjectPool.Get(COMDT_MAILACCESS_HEADIMG.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is COMDT_MAILACCESS_MASTERPOINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_MAILACCESS_MASTERPOINT)ProtocolObjectPool.Get(COMDT_MAILACCESS_MASTERPOINT.CLASS_ID);
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
			return COMDT_MAILACCESS_INFO.CLASS_ID;
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
