using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSPKG_CMD_ITEMSALE : ProtocolObject
	{
		public CSDT_ITEM_DELLIST stSaleList;

		public sbyte[] szPswdInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 923;

		public CSPKG_CMD_ITEMSALE()
		{
			this.stSaleList = (CSDT_ITEM_DELLIST)ProtocolObjectPool.Get(CSDT_ITEM_DELLIST.CLASS_ID);
			this.szPswdInfo = new sbyte[64];
		}

		public override TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = ClassObjPool<TdrWriteBuf>.Get();
			tdrWriteBuf.set(ref buffer, size);
			TdrError.ErrorType errorType = this.pack(ref tdrWriteBuf, cutVer);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				buffer = tdrWriteBuf.getBeginPtr();
				usedSize = tdrWriteBuf.getUsedSize();
			}
			tdrWriteBuf.Release();
			return errorType;
		}

		public override TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
		{
			if (cutVer == 0u || CSPKG_CMD_ITEMSALE.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_CMD_ITEMSALE.CURRVERSION;
			}
			if (CSPKG_CMD_ITEMSALE.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stSaleList.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 64; i++)
			{
				errorType = destBuf.writeInt8(this.szPswdInfo[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = ClassObjPool<TdrReadBuf>.Get();
			tdrReadBuf.set(ref buffer, size);
			TdrError.ErrorType result = this.unpack(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			tdrReadBuf.Release();
			return result;
		}

		public override TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || CSPKG_CMD_ITEMSALE.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_CMD_ITEMSALE.CURRVERSION;
			}
			if (CSPKG_CMD_ITEMSALE.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stSaleList.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 64; i++)
			{
				errorType = srcBuf.readInt8(ref this.szPswdInfo[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSPKG_CMD_ITEMSALE.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stSaleList != null)
			{
				this.stSaleList.Release();
				this.stSaleList = null;
			}
		}

		public override void OnUse()
		{
			this.stSaleList = (CSDT_ITEM_DELLIST)ProtocolObjectPool.Get(CSDT_ITEM_DELLIST.CLASS_ID);
		}
	}
}
