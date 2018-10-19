using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_WEAL_UNION : ProtocolObject
	{
		public ProtocolObject dataObject;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 127u;

		public static readonly uint VERSION_stCondition = 12u;

		public static readonly uint VERSION_stExchange = 76u;

		public static readonly uint VERSION_stPtExchange = 127u;

		public static readonly int CLASS_ID = 539;

		public COMDT_WEAL_CHECKIN_DETAIL stCheckIn
		{
			get
			{
				return this.dataObject as COMDT_WEAL_CHECKIN_DETAIL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_WEAL_FIXEDTIME_DETAIL stFixedTime
		{
			get
			{
				return this.dataObject as COMDT_WEAL_FIXEDTIME_DETAIL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_WEAL_MUTIPLE_DETAIL stMultiple
		{
			get
			{
				return this.dataObject as COMDT_WEAL_MUTIPLE_DETAIL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_WEAL_CONDITION_DETAIL stCondition
		{
			get
			{
				return this.dataObject as COMDT_WEAL_CONDITION_DETAIL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_WEAL_EXCHANGE_DETAIL stExchange
		{
			get
			{
				return this.dataObject as COMDT_WEAL_EXCHANGE_DETAIL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_WEAL_EXCHANGE_DETAIL stPtExchange
		{
			get
			{
				return this.dataObject as COMDT_WEAL_EXCHANGE_DETAIL;
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
				this.select_0_5(selector);
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
			if (cutVer == 0u || COMDT_WEAL_UNION.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WEAL_UNION.CURRVERSION;
			}
			if (COMDT_WEAL_UNION.BASEVERSION > cutVer)
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
			if (cutVer == 0u || COMDT_WEAL_UNION.CURRVERSION < cutVer)
			{
				cutVer = COMDT_WEAL_UNION.CURRVERSION;
			}
			if (COMDT_WEAL_UNION.BASEVERSION > cutVer)
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

		private void select_0_5(long selector)
		{
			if (selector >= 0L && selector <= 5L)
			{
				switch ((int)selector)
				{
				case 0:
					if (!(this.dataObject is COMDT_WEAL_CHECKIN_DETAIL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_WEAL_CHECKIN_DETAIL)ProtocolObjectPool.Get(COMDT_WEAL_CHECKIN_DETAIL.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is COMDT_WEAL_FIXEDTIME_DETAIL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_WEAL_FIXEDTIME_DETAIL)ProtocolObjectPool.Get(COMDT_WEAL_FIXEDTIME_DETAIL.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is COMDT_WEAL_MUTIPLE_DETAIL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_WEAL_MUTIPLE_DETAIL)ProtocolObjectPool.Get(COMDT_WEAL_MUTIPLE_DETAIL.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is COMDT_WEAL_CONDITION_DETAIL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_WEAL_CONDITION_DETAIL)ProtocolObjectPool.Get(COMDT_WEAL_CONDITION_DETAIL.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is COMDT_WEAL_EXCHANGE_DETAIL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_WEAL_EXCHANGE_DETAIL)ProtocolObjectPool.Get(COMDT_WEAL_EXCHANGE_DETAIL.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is COMDT_WEAL_EXCHANGE_DETAIL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_WEAL_EXCHANGE_DETAIL)ProtocolObjectPool.Get(COMDT_WEAL_EXCHANGE_DETAIL.CLASS_ID);
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
			return COMDT_WEAL_UNION.CLASS_ID;
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
