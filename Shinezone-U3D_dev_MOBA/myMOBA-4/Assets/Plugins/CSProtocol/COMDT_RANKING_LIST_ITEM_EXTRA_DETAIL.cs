using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL : ProtocolObject
	{
		public ProtocolObject dataObject;

		public sbyte chReserved;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 240u;

		public static readonly uint VERSION_stMasterPoint = 178u;

		public static readonly uint VERSION_stGuildMatch = 145u;

		public static readonly uint VERSION_stGuildMatchWeek = 145u;

		public static readonly int CLASS_ID = 519;

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stPower
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stPvpExp
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER stGuildPower
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stGuildRankPoint
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHeroNum
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stSkinNum
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLadderPoint
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stAchievement
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stWinGameNum
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stContinousWin
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stUseCoupons
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stVipScore
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stMasterPoint
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stGuildSeason
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP stCustomEquip
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowCoinDay
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stMidCoinDay
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighCoinDay
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowDiamDay
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stMidDiamDay
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighDiamDay
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowCoupDay
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stMidCoupDay
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighCoupDay
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowCoinSeason
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stMidCoinSeason
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighCoinSeason
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowDiamSeason
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stMidDiamSeason
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighDiamSeason
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowCoupSeason
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stMidCoupSeason
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighCoupSeason
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stLowCoinGuild
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stMidCoinGuild
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stHighCoinGuild
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stLowDiamGuild
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stMidDiamGuild
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stHighDiamGuild
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stLowCoupGuild
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stMidCoupGuild
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stHighCoupGuild
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowCoinWin
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighCoinWin
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowDiamondWin
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighDiamondWin
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stDailyRankMatch
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO stMasterHero
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stGuildMatch
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stGuildMatchWeek
		{
			get
			{
				return this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 67L)
			{
				this.select_1_67(selector);
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
			this.chReserved = 0;
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
			if (cutVer == 0u || COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL.CURRVERSION;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.pack(ref destBuf, cutVer);
			}
			TdrError.ErrorType errorType = destBuf.writeInt8(this.chReserved);
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
			if (cutVer == 0u || COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL.CURRVERSION;
			}
			if (COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.unpack(ref srcBuf, cutVer);
			}
			TdrError.ErrorType errorType = srcBuf.readInt8(ref this.chReserved);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		private void select_1_67(long selector)
		{
			if (selector >= 1L && selector <= 67L)
			{
				switch ((int)(selector - 1L))
				{
				case 0:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.CLASS_ID);
					}
					return;
				case 32:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 33:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 34:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 35:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 36:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 37:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 38:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 39:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 40:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 41:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 42:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 43:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 44:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 45:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 46:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 47:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 48:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 49:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 50:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
					}
					return;
				case 51:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
					}
					return;
				case 52:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
					}
					return;
				case 53:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
					}
					return;
				case 54:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
					}
					return;
				case 55:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
					}
					return;
				case 56:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
					}
					return;
				case 57:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
					}
					return;
				case 58:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
					}
					return;
				case 59:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 60:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 61:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 62:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 63:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
					}
					return;
				case 64:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO.CLASS_ID);
					}
					return;
				case 65:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
					}
					return;
				case 66:
					if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT)ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
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
			return COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			this.chReserved = 0;
		}
	}
}
