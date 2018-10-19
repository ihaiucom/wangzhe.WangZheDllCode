using System;

namespace ApolloTdr
{
	public class TdrTLV
	{
		public enum TLV_MAGIC
		{
			TLV_MAGIC_VARINT = 170,
			TLV_MAGIC_NOVARINT = 153
		}

		public enum TdrTLVTypeId
		{
			TDR_TYPE_ID_VARINT,
			TDR_TYPE_ID_1_BYTE,
			TDR_TYPE_ID_2_BYTE,
			TDR_TYPE_ID_4_BYTE,
			TDR_TYPE_ID_8_BYTE,
			TDR_TYPE_ID_LENGTH_DELIMITED
		}

		public static readonly int TLV_MSG_MAGIC_SIZE = 1;

		public static readonly int TLV_MSG_MIN_SIZE = 5;

		public static uint makeTag(int id, TdrTLV.TdrTLVTypeId type)
		{
			return (uint)(id << 4 | (int)type);
		}

		public static uint getFieldId(uint tagid)
		{
			return tagid >> 4;
		}

		public static uint getTypeId(uint tagid)
		{
			return tagid & 15u;
		}

		public static int getMsgSize(ref byte[] buffer, int size)
		{
			if (buffer == null || size < TdrTLV.TLV_MSG_MIN_SIZE)
			{
				return -1;
			}
			int result = 0;
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref buffer, size);
			tdrReadBuf.readInt32(ref result, TdrTLV.TLV_MSG_MAGIC_SIZE);
			return result;
		}

		public static TdrError.ErrorType skipUnknownFields(ref TdrReadBuf srcBuf, TdrTLV.TdrTLVTypeId type_id)
		{
			TdrError.ErrorType errorType;
			switch (type_id)
			{
			case TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_VARINT:
			{
				long num = 0L;
				errorType = srcBuf.readVarInt64(ref num);
				break;
			}
			case TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_1_BYTE:
				errorType = srcBuf.skipForward(1);
				break;
			case TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_2_BYTE:
				errorType = srcBuf.skipForward(2);
				break;
			case TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_4_BYTE:
				errorType = srcBuf.skipForward(4);
				break;
			case TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_8_BYTE:
				errorType = srcBuf.skipForward(8);
				break;
			case TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED:
			{
				int step = 0;
				errorType = srcBuf.readInt32(ref step);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				errorType = srcBuf.skipForward(step);
				break;
			}
			default:
				errorType = TdrError.ErrorType.TDR_ERR_UNKNOWN_TYPE_ID;
				break;
			}
			return errorType;
		}
	}
}
