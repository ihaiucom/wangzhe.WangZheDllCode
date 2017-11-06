using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_JOIN_TEAM_RSP : ProtocolObject
	{
		public ProtocolObject dataObject;

		public sbyte chReserve;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 92u;

		public static readonly int CLASS_ID = 329;

		public COMDT_TEAM_INFO stOfSucc
		{
			get
			{
				return this.dataObject as COMDT_TEAM_INFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TEAM_ERR_BEPUNISH stOfBePunished
		{
			get
			{
				return this.dataObject as COMDT_TEAM_ERR_BEPUNISH;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TEAM_ERR_FORBIDLADDER stOfForbidLadder
		{
			get
			{
				return this.dataObject as COMDT_TEAM_ERR_FORBIDLADDER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 28L)
			{
				this.select_0_28(selector);
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
			if (cutVer == 0u || COMDT_JOIN_TEAM_RSP.CURRVERSION < cutVer)
			{
				cutVer = COMDT_JOIN_TEAM_RSP.CURRVERSION;
			}
			if (COMDT_JOIN_TEAM_RSP.BASEVERSION > cutVer)
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
			if (cutVer == 0u || COMDT_JOIN_TEAM_RSP.CURRVERSION < cutVer)
			{
				cutVer = COMDT_JOIN_TEAM_RSP.CURRVERSION;
			}
			if (COMDT_JOIN_TEAM_RSP.BASEVERSION > cutVer)
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

		private void select_0_28(long selector)
		{
			if (selector != 0L)
			{
				if (selector != 17L)
				{
					if (selector != 28L)
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
							this.dataObject = null;
						}
					}
					else if (!(this.dataObject is COMDT_TEAM_ERR_FORBIDLADDER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TEAM_ERR_FORBIDLADDER)ProtocolObjectPool.Get(COMDT_TEAM_ERR_FORBIDLADDER.CLASS_ID);
					}
				}
				else if (!(this.dataObject is COMDT_TEAM_ERR_BEPUNISH))
				{
					if (this.dataObject != null)
					{
						this.dataObject.Release();
					}
					this.dataObject = (COMDT_TEAM_ERR_BEPUNISH)ProtocolObjectPool.Get(COMDT_TEAM_ERR_BEPUNISH.CLASS_ID);
				}
			}
			else if (!(this.dataObject is COMDT_TEAM_INFO))
			{
				if (this.dataObject != null)
				{
					this.dataObject.Release();
				}
				this.dataObject = (COMDT_TEAM_INFO)ProtocolObjectPool.Get(COMDT_TEAM_INFO.CLASS_ID);
			}
		}

		public override int GetClassID()
		{
			return COMDT_JOIN_TEAM_RSP.CLASS_ID;
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
