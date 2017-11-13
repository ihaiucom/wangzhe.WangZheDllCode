using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_REWARDS_UNION : ProtocolObject
	{
		public ProtocolObject dataObject;

		public byte bReverse;

		public uint dwCoin;

		public uint dwCoupons;

		public uint dwBurningCoin;

		public uint dwArenaCoin;

		public uint dwAP;

		public uint dwPvpCoin;

		public uint dwHeroPoolExp;

		public uint dwSkinCoin;

		public uint dwSymbolCoin;

		public uint dwDiamond;

		public uint dwHuoYueDu;

		public uint dwMatchPointPer;

		public uint dwMatchPointGuild;

		public uint dwAchieve;

		public uint dwMasterPoint;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 219;

		public COMDT_REWARD_ITEM stItem
		{
			get
			{
				return this.dataObject as COMDT_REWARD_ITEM;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_REWARD_EXP stExp
		{
			get
			{
				return this.dataObject as COMDT_REWARD_EXP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_REWARD_EQUIP stEquip
		{
			get
			{
				return this.dataObject as COMDT_REWARD_EQUIP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_REWARD_HERO stHero
		{
			get
			{
				return this.dataObject as COMDT_REWARD_HERO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_REWARD_SYMBOL stSymbol
		{
			get
			{
				return this.dataObject as COMDT_REWARD_SYMBOL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_REWARD_SKIN stSkin
		{
			get
			{
				return this.dataObject as COMDT_REWARD_SKIN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_REWARD_HEADIMG stHeadImage
		{
			get
			{
				return this.dataObject as COMDT_REWARD_HEADIMG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 20L)
			{
				this.select_1_20(selector);
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
			if (selector == 0L)
			{
				this.dwCoin = 0u;
			}
			else if (selector == 3L)
			{
				this.dwCoupons = 0u;
			}
			else if (selector == 7L)
			{
				this.dwBurningCoin = 0u;
			}
			else if (selector == 8L)
			{
				this.dwArenaCoin = 0u;
			}
			else if (selector == 9L)
			{
				this.dwAP = 0u;
			}
			else if (selector == 11L)
			{
				this.dwPvpCoin = 0u;
			}
			else if (selector == 12L)
			{
				this.dwHeroPoolExp = 0u;
			}
			else if (selector == 13L)
			{
				this.dwSkinCoin = 0u;
			}
			else if (selector == 14L)
			{
				this.dwSymbolCoin = 0u;
			}
			else if (selector == 16L)
			{
				this.dwDiamond = 0u;
			}
			else if (selector == 17L)
			{
				this.dwHuoYueDu = 0u;
			}
			else if (selector == 18L)
			{
				this.dwMatchPointPer = 0u;
			}
			else if (selector == 19L)
			{
				this.dwMatchPointGuild = 0u;
			}
			else if (selector == 21L)
			{
				this.dwAchieve = 0u;
			}
			else if (selector == 22L)
			{
				this.dwMasterPoint = 0u;
			}
			else
			{
				this.bReverse = 0;
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
			if (cutVer == 0u || COMDT_REWARDS_UNION.CURRVERSION < cutVer)
			{
				cutVer = COMDT_REWARDS_UNION.CURRVERSION;
			}
			if (COMDT_REWARDS_UNION.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.pack(ref destBuf, cutVer);
			}
			TdrError.ErrorType errorType;
			if (selector == 0L)
			{
				errorType = destBuf.writeUInt32(this.dwCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 3L)
			{
				errorType = destBuf.writeUInt32(this.dwCoupons);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 7L)
			{
				errorType = destBuf.writeUInt32(this.dwBurningCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 8L)
			{
				errorType = destBuf.writeUInt32(this.dwArenaCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 9L)
			{
				errorType = destBuf.writeUInt32(this.dwAP);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 11L)
			{
				errorType = destBuf.writeUInt32(this.dwPvpCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 12L)
			{
				errorType = destBuf.writeUInt32(this.dwHeroPoolExp);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 13L)
			{
				errorType = destBuf.writeUInt32(this.dwSkinCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 14L)
			{
				errorType = destBuf.writeUInt32(this.dwSymbolCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 16L)
			{
				errorType = destBuf.writeUInt32(this.dwDiamond);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 17L)
			{
				errorType = destBuf.writeUInt32(this.dwHuoYueDu);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 18L)
			{
				errorType = destBuf.writeUInt32(this.dwMatchPointPer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 19L)
			{
				errorType = destBuf.writeUInt32(this.dwMatchPointGuild);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 21L)
			{
				errorType = destBuf.writeUInt32(this.dwAchieve);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 22L)
			{
				errorType = destBuf.writeUInt32(this.dwMasterPoint);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = destBuf.writeUInt8(this.bReverse);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
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
			if (cutVer == 0u || COMDT_REWARDS_UNION.CURRVERSION < cutVer)
			{
				cutVer = COMDT_REWARDS_UNION.CURRVERSION;
			}
			if (COMDT_REWARDS_UNION.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.unpack(ref srcBuf, cutVer);
			}
			TdrError.ErrorType errorType;
			if (selector == 0L)
			{
				errorType = srcBuf.readUInt32(ref this.dwCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 3L)
			{
				errorType = srcBuf.readUInt32(ref this.dwCoupons);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 7L)
			{
				errorType = srcBuf.readUInt32(ref this.dwBurningCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 8L)
			{
				errorType = srcBuf.readUInt32(ref this.dwArenaCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 9L)
			{
				errorType = srcBuf.readUInt32(ref this.dwAP);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 11L)
			{
				errorType = srcBuf.readUInt32(ref this.dwPvpCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 12L)
			{
				errorType = srcBuf.readUInt32(ref this.dwHeroPoolExp);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 13L)
			{
				errorType = srcBuf.readUInt32(ref this.dwSkinCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 14L)
			{
				errorType = srcBuf.readUInt32(ref this.dwSymbolCoin);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 16L)
			{
				errorType = srcBuf.readUInt32(ref this.dwDiamond);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 17L)
			{
				errorType = srcBuf.readUInt32(ref this.dwHuoYueDu);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 18L)
			{
				errorType = srcBuf.readUInt32(ref this.dwMatchPointPer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 19L)
			{
				errorType = srcBuf.readUInt32(ref this.dwMatchPointGuild);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 21L)
			{
				errorType = srcBuf.readUInt32(ref this.dwAchieve);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else if (selector == 22L)
			{
				errorType = srcBuf.readUInt32(ref this.dwMasterPoint);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = srcBuf.readUInt8(ref this.bReverse);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		private void select_1_20(long selector)
		{
			if (selector >= 1L && selector <= 10L)
			{
				switch ((int)(selector - 1L))
				{
				case 0:
					if (!(this.dataObject is COMDT_REWARD_ITEM))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_REWARD_ITEM)ProtocolObjectPool.Get(COMDT_REWARD_ITEM.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is COMDT_REWARD_EXP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_REWARD_EXP)ProtocolObjectPool.Get(COMDT_REWARD_EXP.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is COMDT_REWARD_EQUIP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_REWARD_EQUIP)ProtocolObjectPool.Get(COMDT_REWARD_EQUIP.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is COMDT_REWARD_HERO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_REWARD_HERO)ProtocolObjectPool.Get(COMDT_REWARD_HERO.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is COMDT_REWARD_SYMBOL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_REWARD_SYMBOL)ProtocolObjectPool.Get(COMDT_REWARD_SYMBOL.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is COMDT_REWARD_SKIN))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_REWARD_SKIN)ProtocolObjectPool.Get(COMDT_REWARD_SKIN.CLASS_ID);
					}
					return;
				}
			}
			if (selector != 20L)
			{
				if (this.dataObject != null)
				{
					this.dataObject.Release();
					this.dataObject = null;
				}
			}
			else if (!(this.dataObject is COMDT_REWARD_HEADIMG))
			{
				if (this.dataObject != null)
				{
					this.dataObject.Release();
				}
				this.dataObject = (COMDT_REWARD_HEADIMG)ProtocolObjectPool.Get(COMDT_REWARD_HEADIMG.CLASS_ID);
			}
		}

		public override int GetClassID()
		{
			return COMDT_REWARDS_UNION.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			this.bReverse = 0;
			this.dwCoin = 0u;
			this.dwCoupons = 0u;
			this.dwBurningCoin = 0u;
			this.dwArenaCoin = 0u;
			this.dwAP = 0u;
			this.dwPvpCoin = 0u;
			this.dwHeroPoolExp = 0u;
			this.dwSkinCoin = 0u;
			this.dwSymbolCoin = 0u;
			this.dwDiamond = 0u;
			this.dwHuoYueDu = 0u;
			this.dwMatchPointPer = 0u;
			this.dwMatchPointGuild = 0u;
			this.dwAchieve = 0u;
			this.dwMasterPoint = 0u;
		}
	}
}
