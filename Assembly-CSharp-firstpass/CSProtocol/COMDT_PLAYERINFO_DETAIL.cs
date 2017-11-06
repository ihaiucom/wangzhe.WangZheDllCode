using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_PLAYERINFO_DETAIL : ProtocolObject
	{
		public ProtocolObject dataObject;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 132;

		public COMDT_PLAYERINFO_OF_ACNT stPlayerOfAcnt
		{
			get
			{
				return this.dataObject as COMDT_PLAYERINFO_OF_ACNT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_PLAYERINFO_OF_NPC stPlayerOfNpc
		{
			get
			{
				return this.dataObject as COMDT_PLAYERINFO_OF_NPC;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 2L)
			{
				this.select_1_2(selector);
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
			if (cutVer == 0u || COMDT_PLAYERINFO_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PLAYERINFO_DETAIL.CURRVERSION;
			}
			if (COMDT_PLAYERINFO_DETAIL.BASEVERSION > cutVer)
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
			if (cutVer == 0u || COMDT_PLAYERINFO_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PLAYERINFO_DETAIL.CURRVERSION;
			}
			if (COMDT_PLAYERINFO_DETAIL.BASEVERSION > cutVer)
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

		private void select_1_2(long selector)
		{
			if (selector != 1L)
			{
				if (selector != 2L)
				{
					if (this.dataObject != null)
					{
						this.dataObject.Release();
						this.dataObject = null;
					}
				}
				else if (!(this.dataObject is COMDT_PLAYERINFO_OF_NPC))
				{
					if (this.dataObject != null)
					{
						this.dataObject.Release();
					}
					this.dataObject = (COMDT_PLAYERINFO_OF_NPC)ProtocolObjectPool.Get(COMDT_PLAYERINFO_OF_NPC.CLASS_ID);
				}
			}
			else if (!(this.dataObject is COMDT_PLAYERINFO_OF_ACNT))
			{
				if (this.dataObject != null)
				{
					this.dataObject.Release();
				}
				this.dataObject = (COMDT_PLAYERINFO_OF_ACNT)ProtocolObjectPool.Get(COMDT_PLAYERINFO_OF_ACNT.CLASS_ID);
			}
		}

		public override int GetClassID()
		{
			return COMDT_PLAYERINFO_DETAIL.CLASS_ID;
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
