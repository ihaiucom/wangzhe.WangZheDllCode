using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_SINGLE_GAME_DETAIL : ProtocolObject
	{
		public ProtocolObject dataObject;

		public byte bReserved;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 743;

		public CSDT_SINGLE_GAME_OF_ADVENTURE stGameOfAdventure
		{
			get
			{
				return this.dataObject as CSDT_SINGLE_GAME_OF_ADVENTURE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_SINGLE_GAME_OF_COMBAT stGameOfCombat
		{
			get
			{
				return this.dataObject as CSDT_SINGLE_GAME_OF_COMBAT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_SINGLE_GAME_OF_GUIDE stGameOfGuide
		{
			get
			{
				return this.dataObject as CSDT_SINGLE_GAME_OF_GUIDE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_SINGLE_GAME_OF_ACTIVITY stGameOfActivity
		{
			get
			{
				return this.dataObject as CSDT_SINGLE_GAME_OF_ACTIVITY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_SINGLE_GAME_OF_BURNING stGameOfBurning
		{
			get
			{
				return this.dataObject as CSDT_SINGLE_GAME_OF_BURNING;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_SINGLE_GAME_OF_ARENA stGameOfArena
		{
			get
			{
				return this.dataObject as CSDT_SINGLE_GAME_OF_ARENA;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 8L)
			{
				this.select_0_8(selector);
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
			if (cutVer == 0u || CSDT_SINGLE_GAME_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = CSDT_SINGLE_GAME_DETAIL.CURRVERSION;
			}
			if (CSDT_SINGLE_GAME_DETAIL.BASEVERSION > cutVer)
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
			if (cutVer == 0u || CSDT_SINGLE_GAME_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = CSDT_SINGLE_GAME_DETAIL.CURRVERSION;
			}
			if (CSDT_SINGLE_GAME_DETAIL.BASEVERSION > cutVer)
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

		private void select_0_8(long selector)
		{
			if (selector >= 0L && selector <= 8L)
			{
				switch ((int)selector)
				{
				case 0:
					if (!(this.dataObject is CSDT_SINGLE_GAME_OF_ADVENTURE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_SINGLE_GAME_OF_ADVENTURE)ProtocolObjectPool.Get(CSDT_SINGLE_GAME_OF_ADVENTURE.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is CSDT_SINGLE_GAME_OF_COMBAT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_SINGLE_GAME_OF_COMBAT)ProtocolObjectPool.Get(CSDT_SINGLE_GAME_OF_COMBAT.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSDT_SINGLE_GAME_OF_GUIDE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_SINGLE_GAME_OF_GUIDE)ProtocolObjectPool.Get(CSDT_SINGLE_GAME_OF_GUIDE.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSDT_SINGLE_GAME_OF_ACTIVITY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_SINGLE_GAME_OF_ACTIVITY)ProtocolObjectPool.Get(CSDT_SINGLE_GAME_OF_ACTIVITY.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is CSDT_SINGLE_GAME_OF_BURNING))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_SINGLE_GAME_OF_BURNING)ProtocolObjectPool.Get(CSDT_SINGLE_GAME_OF_BURNING.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is CSDT_SINGLE_GAME_OF_ARENA))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_SINGLE_GAME_OF_ARENA)ProtocolObjectPool.Get(CSDT_SINGLE_GAME_OF_ARENA.CLASS_ID);
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
			return CSDT_SINGLE_GAME_DETAIL.CLASS_ID;
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
