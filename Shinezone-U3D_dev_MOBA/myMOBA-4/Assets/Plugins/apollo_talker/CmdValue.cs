using ApolloTdr;
using System;

namespace apollo_talker
{
	public class CmdValue
	{
		public int iNilCmd;

		public byte[] szStrCmd;

		public int iIntCmd;

		public static readonly uint LENGTH_szStrCmd = 64u;

		public tsf4g_csharp_interface select(long selector)
		{
			return null;
		}

		public TdrError.ErrorType construct(long selector)
		{
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			tsf4g_csharp_interface tsf4g_csharp_interface = this.select(selector);
			if (tsf4g_csharp_interface != null)
			{
				return tsf4g_csharp_interface.construct();
			}
			if (selector == 0L)
			{
				this.iNilCmd = 0;
			}
			else if (selector == 1L)
			{
				if (this.szStrCmd == null)
				{
					this.szStrCmd = new byte[64];
				}
			}
			else if (selector == 2L)
			{
				this.iIntCmd = 0;
			}
			return result;
		}

		public TdrError.ErrorType packTLV(long selector, ref byte[] buffer, int size, ref int used, bool useVarInt)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = new TdrWriteBuf(ref buffer, size);
			TdrError.ErrorType errorType;
			if (useVarInt)
			{
				errorType = tdrWriteBuf.writeUInt8(170);
			}
			else
			{
				errorType = tdrWriteBuf.writeUInt8(153);
			}
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize = tdrWriteBuf.getUsedSize();
			errorType = tdrWriteBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.packTLV(selector, ref tdrWriteBuf, useVarInt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			used = tdrWriteBuf.getUsedSize();
			tdrWriteBuf.writeInt32(used, usedSize);
			return errorType;
		}

		public TdrError.ErrorType packTLV(long selector, ref TdrWriteBuf destBuf, bool useVarInt)
		{
			if (destBuf == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			TdrError.ErrorType errorType;
			if (selector >= 0L && selector <= 2L)
			{
				switch ((int)selector)
				{
				case 0:
					if (useVarInt)
					{
						uint src = TdrTLV.makeTag(0, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_VARINT);
						errorType = destBuf.writeVarUInt32(src);
						if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
						{
							return errorType;
						}
						errorType = destBuf.writeVarInt32(this.iNilCmd);
						if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
						{
							return errorType;
						}
					}
					else
					{
						uint src2 = TdrTLV.makeTag(0, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_4_BYTE);
						errorType = destBuf.writeVarUInt32(src2);
						if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
						{
							return errorType;
						}
						errorType = destBuf.writeInt32(this.iNilCmd);
						if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
						{
							return errorType;
						}
					}
					return errorType;
				case 1:
				{
					if (this.szStrCmd == null)
					{
						return TdrError.ErrorType.TDR_ERR_UNION_SELECTE_FIELD_IS_NULL;
					}
					uint src3 = TdrTLV.makeTag(1, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED);
					errorType = destBuf.writeVarUInt32(src3);
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
					int num = TdrTypeUtil.cstrlen(this.szStrCmd);
					if (num >= 64)
					{
						return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
					}
					errorType = destBuf.writeCString(this.szStrCmd, num);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					int src4 = destBuf.getUsedSize() - usedSize - 4;
					errorType = destBuf.writeInt32(src4, usedSize);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					return errorType;
				}
				case 2:
					if (useVarInt)
					{
						uint src5 = TdrTLV.makeTag(2, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_VARINT);
						errorType = destBuf.writeVarUInt32(src5);
						if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
						{
							return errorType;
						}
						errorType = destBuf.writeVarInt32(this.iIntCmd);
						if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
						{
							return errorType;
						}
					}
					else
					{
						uint src6 = TdrTLV.makeTag(2, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_4_BYTE);
						errorType = destBuf.writeVarUInt32(src6);
						if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
						{
							return errorType;
						}
						errorType = destBuf.writeInt32(this.iIntCmd);
						if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
						{
							return errorType;
						}
					}
					return errorType;
				}
			}
			errorType = TdrError.ErrorType.TDR_ERR_SUSPICIOUS_SELECTOR;
			return errorType;
		}

		public TdrError.ErrorType unpackTLV(ref long selector, ref byte[] buffer, int size, ref int used)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			if (size < TdrTLV.TLV_MSG_MIN_SIZE)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref buffer, size);
			byte b = 0;
			TdrError.ErrorType errorType = tdrReadBuf.readUInt8(ref b);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			bool useVarInt;
			if (b == 170)
			{
				useVarInt = true;
			}
			else
			{
				if (b != 153)
				{
					return TdrError.ErrorType.TDR_ERR_BAD_TLV_MAGIC;
				}
				useVarInt = false;
			}
			int num = 0;
			tdrReadBuf.readInt32(ref num);
			if (size < num)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			errorType = this.unpackTLV(ref selector, ref tdrReadBuf, num - TdrTLV.TLV_MSG_MIN_SIZE, useVarInt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			used = tdrReadBuf.getUsedSize();
			return errorType;
		}

		public TdrError.ErrorType unpackTLV(ref long selector, ref TdrReadBuf srcBuf, int length, bool useVarInt)
		{
			if (srcBuf == null || length == 0)
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			int usedSize = srcBuf.getUsedSize();
			uint tagid = 0u;
			TdrError.ErrorType errorType = srcBuf.readVarUInt32(ref tagid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint fieldId = TdrTLV.getFieldId(tagid);
			switch (fieldId)
			{
			case 0u:
				if (useVarInt)
				{
					errorType = srcBuf.readVarInt32(ref this.iNilCmd);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
				else
				{
					errorType = srcBuf.readInt32(ref this.iNilCmd);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
				break;
			case 1u:
			{
				if (this.szStrCmd == null)
				{
					this.szStrCmd = new byte[64];
				}
				int num = 0;
				errorType = srcBuf.readInt32(ref num);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				if (num >= 64)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				errorType = srcBuf.readCString(ref this.szStrCmd, num);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				break;
			}
			case 2u:
				if (useVarInt)
				{
					errorType = srcBuf.readVarInt32(ref this.iIntCmd);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
				else
				{
					errorType = srcBuf.readInt32(ref this.iIntCmd);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
				break;
			default:
				errorType = TdrError.ErrorType.TDR_ERR_SUSPICIOUS_SELECTOR;
				break;
			}
			if (srcBuf.getUsedSize() > usedSize + length)
			{
				return TdrError.ErrorType.TDR_ERR_UNMATCHED_LENGTH;
			}
			selector = (long)((ulong)fieldId);
			return errorType;
		}
	}
}
