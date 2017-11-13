using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ : ProtocolObject
	{
		public ulong ullUid;

		public uint dwLogicWordID;

		public COMDT_IDIP_ADDITEM_LIST stAddItemList;

		public byte[] szIdipMailContent;

		public byte[] szIdipMailTitle;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szIdipMailContent = 600u;

		public static readonly uint LENGTH_szIdipMailTitle = 128u;

		public static readonly int CLASS_ID = 688;

		public COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ()
		{
			this.stAddItemList = (COMDT_IDIP_ADDITEM_LIST)ProtocolObjectPool.Get(COMDT_IDIP_ADDITEM_LIST.CLASS_ID);
			this.szIdipMailContent = new byte[600];
			this.szIdipMailTitle = new byte[128];
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
			if (cutVer == 0u || COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ.CURRVERSION;
			}
			if (COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt64(this.ullUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwLogicWordID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stAddItemList.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize2 = destBuf.getUsedSize();
			int num = TdrTypeUtil.cstrlen(this.szIdipMailContent);
			if (num >= 600)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szIdipMailContent, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src = destBuf.getUsedSize() - usedSize2;
			errorType = destBuf.writeUInt32((uint)src, usedSize);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize3 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize4 = destBuf.getUsedSize();
			int num2 = TdrTypeUtil.cstrlen(this.szIdipMailTitle);
			if (num2 >= 128)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szIdipMailTitle, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src2 = destBuf.getUsedSize() - usedSize4;
			errorType = destBuf.writeUInt32((uint)src2, usedSize3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (cutVer == 0u || COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ.CURRVERSION;
			}
			if (COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt64(ref this.ullUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwLogicWordID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stAddItemList.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num = 0u;
			errorType = srcBuf.readUInt32(ref num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num > (uint)this.szIdipMailContent.GetLength(0))
			{
				if ((ulong)num > (ulong)COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ.LENGTH_szIdipMailContent)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szIdipMailContent = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szIdipMailContent, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szIdipMailContent[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szIdipMailContent) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num3 = 0u;
			errorType = srcBuf.readUInt32(ref num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num3 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num3 > (uint)this.szIdipMailTitle.GetLength(0))
			{
				if ((ulong)num3 > (ulong)COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ.LENGTH_szIdipMailTitle)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szIdipMailTitle = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szIdipMailTitle, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szIdipMailTitle[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szIdipMailTitle) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.ullUid = 0uL;
			this.dwLogicWordID = 0u;
			if (this.stAddItemList != null)
			{
				this.stAddItemList.Release();
				this.stAddItemList = null;
			}
		}

		public override void OnUse()
		{
			this.stAddItemList = (COMDT_IDIP_ADDITEM_LIST)ProtocolObjectPool.Get(COMDT_IDIP_ADDITEM_LIST.CLASS_ID);
		}
	}
}
