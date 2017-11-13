using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ROOMCHG_DT : ProtocolObject
	{
		public ProtocolObject dataObject;

		public sbyte chReserve;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 64;

		public COMDT_ROOMCHG_PLAYERADD stPlayerAdd
		{
			get
			{
				return this.dataObject as COMDT_ROOMCHG_PLAYERADD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_ROOMCHG_PLAYERLEAVE stPlayerLeave
		{
			get
			{
				return this.dataObject as COMDT_ROOMCHG_PLAYERLEAVE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_ROOMCHG_STATECHG stStateChg
		{
			get
			{
				return this.dataObject as COMDT_ROOMCHG_STATECHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_ROOMCHG_MASTERCHG stMasterChg
		{
			get
			{
				return this.dataObject as COMDT_ROOMCHG_MASTERCHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_ROOMCHG_CHGMEMBERPOS stChgMemberPos
		{
			get
			{
				return this.dataObject as COMDT_ROOMCHG_CHGMEMBERPOS;
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
			this.chReserve = 0;
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
			if (cutVer == 0u || COMDT_ROOMCHG_DT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ROOMCHG_DT.CURRVERSION;
			}
			if (COMDT_ROOMCHG_DT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.pack(ref destBuf, cutVer);
			}
			TdrError.ErrorType errorType = destBuf.writeInt8(this.chReserve);
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
			if (cutVer == 0u || COMDT_ROOMCHG_DT.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ROOMCHG_DT.CURRVERSION;
			}
			if (COMDT_ROOMCHG_DT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.unpack(ref srcBuf, cutVer);
			}
			TdrError.ErrorType errorType = srcBuf.readInt8(ref this.chReserve);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		private void select_0_5(long selector)
		{
			if (selector >= 0L && selector <= 5L)
			{
				switch ((int)selector)
				{
				case 0:
					if (!(this.dataObject is COMDT_ROOMCHG_PLAYERADD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_ROOMCHG_PLAYERADD)ProtocolObjectPool.Get(COMDT_ROOMCHG_PLAYERADD.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is COMDT_ROOMCHG_PLAYERLEAVE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_ROOMCHG_PLAYERLEAVE)ProtocolObjectPool.Get(COMDT_ROOMCHG_PLAYERLEAVE.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is COMDT_ROOMCHG_STATECHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_ROOMCHG_STATECHG)ProtocolObjectPool.Get(COMDT_ROOMCHG_STATECHG.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is COMDT_ROOMCHG_MASTERCHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_ROOMCHG_MASTERCHG)ProtocolObjectPool.Get(COMDT_ROOMCHG_MASTERCHG.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is COMDT_ROOMCHG_CHGMEMBERPOS))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_ROOMCHG_CHGMEMBERPOS)ProtocolObjectPool.Get(COMDT_ROOMCHG_CHGMEMBERPOS.CLASS_ID);
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
			return COMDT_ROOMCHG_DT.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			this.chReserve = 0;
		}
	}
}
