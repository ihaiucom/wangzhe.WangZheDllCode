using ApolloTdr;
using System;

namespace apollo_http_object
{
	public class HttpReq : IPackable, IUnpackable, tsf4g_csharp_interface
	{
		public RequestLine stRequestLine;

		public HttpHeaders stHttpHeaders;

		public RequestContent stRequestContent;

		private uint[] has_bits_ = new uint[1];

		public HttpReq()
		{
			this.stRequestLine = new RequestLine();
			this.stHttpHeaders = new HttpHeaders();
			this.stRequestContent = new RequestContent();
		}

		public void set_has_RequestLine()
		{
			this.has_bits_[0] |= 1u;
		}

		private void clear_has_RequestLine()
		{
			this.has_bits_[0] = (uint)((ulong)this.has_bits_[0] & 18446744073709551614uL);
		}

		public bool has_RequestLine()
		{
			return (this.has_bits_[0] & 1u) != 0u;
		}

		public void set_has_HttpHeaders()
		{
			this.has_bits_[0] |= 2u;
		}

		private void clear_has_HttpHeaders()
		{
			this.has_bits_[0] = (uint)((ulong)this.has_bits_[0] & 18446744073709551613uL);
		}

		public bool has_HttpHeaders()
		{
			return (this.has_bits_[0] & 2u) != 0u;
		}

		public void set_has_RequestContent()
		{
			this.has_bits_[0] |= 4u;
		}

		private void clear_has_RequestContent()
		{
			this.has_bits_[0] = (uint)((ulong)this.has_bits_[0] & 18446744073709551611uL);
		}

		public bool has_RequestContent()
		{
			return (this.has_bits_[0] & 4u) != 0u;
		}

		private int requiredFieldNum()
		{
			return 0;
		}

		public string getLastLostRequiredFields()
		{
			return string.Empty;
		}

		public TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = this.stRequestLine.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHttpHeaders.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stRequestContent.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public TdrError.ErrorType packTLV(ref byte[] buffer, int size, ref int used, bool useVarInt)
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
			errorType = this.packTLV(ref tdrWriteBuf, useVarInt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			used = tdrWriteBuf.getUsedSize();
			tdrWriteBuf.writeInt32(used, usedSize);
			return errorType;
		}

		public TdrError.ErrorType packTLV(ref TdrWriteBuf destBuf, bool useVarInt)
		{
			uint src = TdrTLV.makeTag(1, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED);
			TdrError.ErrorType errorType = destBuf.writeVarUInt32(src);
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
			errorType = this.stRequestLine.packTLV(ref destBuf, useVarInt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src2 = destBuf.getUsedSize() - usedSize - 4;
			errorType = destBuf.writeInt32(src2, usedSize);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint src3 = TdrTLV.makeTag(2, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED);
			errorType = destBuf.writeVarUInt32(src3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize2 = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stHttpHeaders.packTLV(ref destBuf, useVarInt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src4 = destBuf.getUsedSize() - usedSize2 - 4;
			errorType = destBuf.writeInt32(src4, usedSize2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint src5 = TdrTLV.makeTag(3, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED);
			errorType = destBuf.writeVarUInt32(src5);
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
			errorType = this.stRequestContent.packTLV(ref destBuf, useVarInt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src6 = destBuf.getUsedSize() - usedSize3 - 4;
			errorType = destBuf.writeInt32(src6, usedSize3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public TdrError.ErrorType unpackTLV(ref byte[] buffer, int size, ref int used)
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
			errorType = this.unpackTLV(ref tdrReadBuf, num - TdrTLV.TLV_MSG_MIN_SIZE, useVarInt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			used = tdrReadBuf.getUsedSize();
			return errorType;
		}

		public TdrError.ErrorType unpackTLV(ref TdrReadBuf srcBuf, int length, bool useVarInt)
		{
			if (srcBuf == null || length <= 0)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			uint tagid = 0u;
			int num = 0;
			int usedSize = srcBuf.getUsedSize();
			while (srcBuf.getUsedSize() < usedSize + length)
			{
				errorType = srcBuf.readVarUInt32(ref tagid);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				switch (TdrTLV.getFieldId(tagid))
				{
				case 1u:
				{
					if (!this.has_RequestLine())
					{
						this.set_has_RequestLine();
					}
					int length2 = 0;
					errorType = srcBuf.readInt32(ref length2);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					errorType = this.stRequestLine.unpackTLV(ref srcBuf, length2, useVarInt);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					break;
				}
				case 2u:
				{
					if (!this.has_HttpHeaders())
					{
						this.set_has_HttpHeaders();
					}
					int length3 = 0;
					errorType = srcBuf.readInt32(ref length3);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					errorType = this.stHttpHeaders.unpackTLV(ref srcBuf, length3, useVarInt);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					break;
				}
				case 3u:
				{
					if (!this.has_RequestContent())
					{
						this.set_has_RequestContent();
					}
					int length4 = 0;
					errorType = srcBuf.readInt32(ref length4);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					errorType = this.stRequestContent.unpackTLV(ref srcBuf, length4, useVarInt);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					break;
				}
				default:
				{
					uint typeId = TdrTLV.getTypeId(tagid);
					errorType = TdrTLV.skipUnknownFields(ref srcBuf, (TdrTLV.TdrTLVTypeId)typeId);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					break;
				}
				}
			}
			if (srcBuf.getUsedSize() > usedSize + length)
			{
				return TdrError.ErrorType.TDR_ERR_UNMATCHED_LENGTH;
			}
			if (num < this.requiredFieldNum())
			{
				return TdrError.ErrorType.TDR_ERR_LOST_REQUIRED_FIELD;
			}
			return errorType;
		}

		public TdrError.ErrorType visualize(ref string buffer, int indent, char separator)
		{
			TdrVisualBuf tdrVisualBuf = new TdrVisualBuf();
			TdrError.ErrorType result = this.visualize(ref tdrVisualBuf, indent, separator);
			buffer = tdrVisualBuf.getVisualBuf();
			return result;
		}

		public TdrError.ErrorType visualize(ref TdrVisualBuf destBuf, int indent, char separator)
		{
			TdrError.ErrorType errorType = TdrBufUtil.printVariable(ref destBuf, indent, separator, "[stRequestLine]", true);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (0 > indent)
			{
				errorType = this.stRequestLine.visualize(ref destBuf, indent, separator);
			}
			else
			{
				errorType = this.stRequestLine.visualize(ref destBuf, indent + 1, separator);
			}
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = TdrBufUtil.printVariable(ref destBuf, indent, separator, "[stHttpHeaders]", true);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (0 > indent)
			{
				errorType = this.stHttpHeaders.visualize(ref destBuf, indent, separator);
			}
			else
			{
				errorType = this.stHttpHeaders.visualize(ref destBuf, indent + 1, separator);
			}
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = TdrBufUtil.printVariable(ref destBuf, indent, separator, "[stRequestContent]", true);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (0 > indent)
			{
				errorType = this.stRequestContent.visualize(ref destBuf, indent, separator);
			}
			else
			{
				errorType = this.stRequestContent.visualize(ref destBuf, indent + 1, separator);
			}
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}
	}
}
