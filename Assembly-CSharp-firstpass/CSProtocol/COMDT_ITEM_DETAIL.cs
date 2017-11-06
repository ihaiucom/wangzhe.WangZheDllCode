using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ITEM_DETAIL : ProtocolObject
	{
		public ProtocolObject dataObject;

		public byte bReverse;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 80;

		public COMDT_PROP_DETAIL stPropInfo
		{
			get
			{
				return this.dataObject as COMDT_PROP_DETAIL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_EQUIP_DETAIL stEquipInfo
		{
			get
			{
				return this.dataObject as COMDT_EQUIP_DETAIL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_SYMBOL_DETAIL stSymbolInfo
		{
			get
			{
				return this.dataObject as COMDT_SYMBOL_DETAIL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_GEAR_DETAIL stGearInfo
		{
			get
			{
				return this.dataObject as COMDT_GEAR_DETAIL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 6L)
			{
				this.select_2_6(selector);
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
			if (cutVer == 0u || COMDT_ITEM_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ITEM_DETAIL.CURRVERSION;
			}
			if (COMDT_ITEM_DETAIL.BASEVERSION > cutVer)
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
			if (cutVer == 0u || COMDT_ITEM_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ITEM_DETAIL.CURRVERSION;
			}
			if (COMDT_ITEM_DETAIL.BASEVERSION > cutVer)
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

		private void select_2_6(long selector)
		{
			if (selector >= 2L && selector <= 6L)
			{
				switch ((int)(selector - 2L))
				{
				case 0:
					if (!(this.dataObject is COMDT_PROP_DETAIL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_PROP_DETAIL)ProtocolObjectPool.Get(COMDT_PROP_DETAIL.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is COMDT_EQUIP_DETAIL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_EQUIP_DETAIL)ProtocolObjectPool.Get(COMDT_EQUIP_DETAIL.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is COMDT_SYMBOL_DETAIL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_SYMBOL_DETAIL)ProtocolObjectPool.Get(COMDT_SYMBOL_DETAIL.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is COMDT_GEAR_DETAIL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_GEAR_DETAIL)ProtocolObjectPool.Get(COMDT_GEAR_DETAIL.CLASS_ID);
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
			return COMDT_ITEM_DETAIL.CLASS_ID;
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
