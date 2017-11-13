using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_SHOPBUY_EXTRA : ProtocolObject
	{
		public ProtocolObject dataObject;

		public byte bReverse;

		public uint dwLevelID;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 451;

		public COMDT_SHOPBUY_OF_ENTERTAINMENTRANDHERO stRandHero
		{
			get
			{
				return this.dataObject as COMDT_SHOPBUY_OF_ENTERTAINMENTRANDHERO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 13L)
			{
				this.select_13_13(selector);
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
			if (selector == 6L)
			{
				this.dwLevelID = 0u;
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
			if (cutVer == 0u || COMDT_SHOPBUY_EXTRA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SHOPBUY_EXTRA.CURRVERSION;
			}
			if (COMDT_SHOPBUY_EXTRA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.pack(ref destBuf, cutVer);
			}
			TdrError.ErrorType errorType;
			if (selector == 6L)
			{
				errorType = destBuf.writeUInt32(this.dwLevelID);
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
			if (cutVer == 0u || COMDT_SHOPBUY_EXTRA.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SHOPBUY_EXTRA.CURRVERSION;
			}
			if (COMDT_SHOPBUY_EXTRA.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.unpack(ref srcBuf, cutVer);
			}
			TdrError.ErrorType errorType;
			if (selector == 6L)
			{
				errorType = srcBuf.readUInt32(ref this.dwLevelID);
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

		private void select_13_13(long selector)
		{
			if (selector != 13L)
			{
				if (this.dataObject != null)
				{
					this.dataObject.Release();
					this.dataObject = null;
				}
			}
			else if (!(this.dataObject is COMDT_SHOPBUY_OF_ENTERTAINMENTRANDHERO))
			{
				if (this.dataObject != null)
				{
					this.dataObject.Release();
				}
				this.dataObject = (COMDT_SHOPBUY_OF_ENTERTAINMENTRANDHERO)ProtocolObjectPool.Get(COMDT_SHOPBUY_OF_ENTERTAINMENTRANDHERO.CLASS_ID);
			}
		}

		public override int GetClassID()
		{
			return COMDT_SHOPBUY_EXTRA.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			this.bReverse = 0;
			this.dwLevelID = 0u;
		}
	}
}
