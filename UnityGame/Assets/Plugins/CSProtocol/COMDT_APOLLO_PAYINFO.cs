using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_APOLLO_PAYINFO : ProtocolObject
	{
		public ProtocolObject dataObject;

		public byte bReverse;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly int CLASS_ID = 478;

		public COMDT_APOLLO_PAY_SHOPBUY stShopBuy
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_SHOPBUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_ITEMBUY stItemBuy
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_ITEMBUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_COINBUY stCoinBuy
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_COINBUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_MANUALREF stManualRef
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_MANUALREF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_HEROBUY stHeroBuy
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_HEROBUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_GAMESWEEP stGameSweep
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_GAMESWEEP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_BUILDCRT stGuildCrt
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_BUILDCRT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_BUYSKIN stBuySkin
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_BUYSKIN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_CLRCD stClrCd
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_CLRCD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_PAYGUILDDONATE stGuildDonate
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_PAYGUILDDONATE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_SPECSALE stSpecSale
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_SPECSALE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_MONTHFILLIN stMonthFillIn
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_MONTHFILLIN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_DIRECTBUYITEM stDirectBuyItem
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_DIRECTBUYITEM;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_PVEREVIVE stPveRevive
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_PVEREVIVE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_PAYGUILDUPD stGuildUpgrade
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_PAYGUILDUPD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_TALENTBUY stTalentBuy
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_TALENTBUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_LUCKYDRAW stLuckyDraw
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_LUCKYDRAW;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_CHAT stChatBuy
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_CHAT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_SALERECMD_BUY stSaleRecmd
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_SALERECMD_BUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_RANDDRAW stRandDraw
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_RANDDRAW;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_PRESENTHERO stPresentHero
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_PRESENTHERO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_PRESENTSKIN stPresentSkin
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_PRESENTSKIN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_TICKETBUY stTicketBuy
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_TICKETBUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_IDIPDEL stIdipDel
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_IDIPDEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_APOLLO_PAY_ASKFORREQ stAskforPay
		{
			get
			{
				return this.dataObject as COMDT_APOLLO_PAY_ASKFORREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 26L)
			{
				this.select_1_26(selector);
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
			if (cutVer == 0u || COMDT_APOLLO_PAYINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_APOLLO_PAYINFO.CURRVERSION;
			}
			if (COMDT_APOLLO_PAYINFO.BASEVERSION > cutVer)
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
			if (cutVer == 0u || COMDT_APOLLO_PAYINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_APOLLO_PAYINFO.CURRVERSION;
			}
			if (COMDT_APOLLO_PAYINFO.BASEVERSION > cutVer)
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

		private void select_1_26(long selector)
		{
			if (selector >= 1L && selector <= 26L)
			{
				switch ((int)(selector - 1L))
				{
				case 0:
					if (!(this.dataObject is COMDT_APOLLO_PAY_SHOPBUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_SHOPBUY)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_SHOPBUY.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is COMDT_APOLLO_PAY_ITEMBUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_ITEMBUY)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_ITEMBUY.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is COMDT_APOLLO_PAY_COINBUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_COINBUY)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_COINBUY.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is COMDT_APOLLO_PAY_MANUALREF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_MANUALREF)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_MANUALREF.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is COMDT_APOLLO_PAY_HEROBUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_HEROBUY)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_HEROBUY.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is COMDT_APOLLO_PAY_GAMESWEEP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_GAMESWEEP)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_GAMESWEEP.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is COMDT_APOLLO_PAY_BUILDCRT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_BUILDCRT)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_BUILDCRT.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is COMDT_APOLLO_PAY_BUYSKIN))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_BUYSKIN)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_BUYSKIN.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is COMDT_APOLLO_PAY_CLRCD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_CLRCD)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_CLRCD.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is COMDT_APOLLO_PAY_PAYGUILDDONATE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_PAYGUILDDONATE)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_PAYGUILDDONATE.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is COMDT_APOLLO_PAY_SPECSALE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_SPECSALE)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_SPECSALE.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is COMDT_APOLLO_PAY_MONTHFILLIN))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_MONTHFILLIN)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_MONTHFILLIN.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is COMDT_APOLLO_PAY_DIRECTBUYITEM))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_DIRECTBUYITEM)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_DIRECTBUYITEM.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is COMDT_APOLLO_PAY_PVEREVIVE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_PVEREVIVE)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_PVEREVIVE.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is COMDT_APOLLO_PAY_PAYGUILDUPD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_PAYGUILDUPD)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_PAYGUILDUPD.CLASS_ID);
					}
					return;
				case 16:
					if (!(this.dataObject is COMDT_APOLLO_PAY_TALENTBUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_TALENTBUY)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_TALENTBUY.CLASS_ID);
					}
					return;
				case 17:
					if (!(this.dataObject is COMDT_APOLLO_PAY_LUCKYDRAW))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_LUCKYDRAW)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_LUCKYDRAW.CLASS_ID);
					}
					return;
				case 18:
					if (!(this.dataObject is COMDT_APOLLO_PAY_CHAT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_CHAT)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_CHAT.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is COMDT_APOLLO_PAY_SALERECMD_BUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_SALERECMD_BUY)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_SALERECMD_BUY.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is COMDT_APOLLO_PAY_RANDDRAW))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_RANDDRAW)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_RANDDRAW.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is COMDT_APOLLO_PAY_PRESENTHERO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_PRESENTHERO)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_PRESENTHERO.CLASS_ID);
					}
					return;
				case 22:
					if (!(this.dataObject is COMDT_APOLLO_PAY_PRESENTSKIN))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_PRESENTSKIN)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_PRESENTSKIN.CLASS_ID);
					}
					return;
				case 23:
					if (!(this.dataObject is COMDT_APOLLO_PAY_TICKETBUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_TICKETBUY)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_TICKETBUY.CLASS_ID);
					}
					return;
				case 24:
					if (!(this.dataObject is COMDT_APOLLO_PAY_IDIPDEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_IDIPDEL)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_IDIPDEL.CLASS_ID);
					}
					return;
				case 25:
					if (!(this.dataObject is COMDT_APOLLO_PAY_ASKFORREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_APOLLO_PAY_ASKFORREQ)ProtocolObjectPool.Get(COMDT_APOLLO_PAY_ASKFORREQ.CLASS_ID);
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
			return COMDT_APOLLO_PAYINFO.CLASS_ID;
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
