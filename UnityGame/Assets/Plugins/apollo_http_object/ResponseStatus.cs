using ApolloTdr;
using System;
using System.Text;

namespace apollo_http_object
{
	public class ResponseStatus : IPackable, IUnpackable, tsf4g_csharp_interface
	{
		public byte[] szHttpVersion;

		public byte[] szStatusCode;

		public byte[] szReasonPhrase;

		private uint[] has_bits_ = new uint[1];

		public static readonly uint LENGTH_szHttpVersion = 32u;

		public static readonly uint LENGTH_szStatusCode = 16u;

		public static readonly uint LENGTH_szReasonPhrase = 64u;

		public ResponseStatus()
		{
			this.szHttpVersion = new byte[32];
			this.szStatusCode = new byte[16];
			this.szReasonPhrase = new byte[64];
		}

		public void set_has_HttpVersion()
		{
			this.has_bits_[0] |= 1u;
		}

		private void clear_has_HttpVersion()
		{
			this.has_bits_[0] = (uint)((ulong)this.has_bits_[0] & 18446744073709551614uL);
		}

		public bool has_HttpVersion()
		{
			return (this.has_bits_[0] & 1u) != 0u;
		}

		public void set_has_StatusCode()
		{
			this.has_bits_[0] |= 2u;
		}

		private void clear_has_StatusCode()
		{
			this.has_bits_[0] = (uint)((ulong)this.has_bits_[0] & 18446744073709551613uL);
		}

		public bool has_StatusCode()
		{
			return (this.has_bits_[0] & 2u) != 0u;
		}

		public void set_has_ReasonPhrase()
		{
			this.has_bits_[0] |= 4u;
		}

		private void clear_has_ReasonPhrase()
		{
			this.has_bits_[0] = (uint)((ulong)this.has_bits_[0] & 18446744073709551611uL);
		}

		public bool has_ReasonPhrase()
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
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			string text = "HTTP/1.1";
			byte[] bytes = Encoding.get_ASCII().GetBytes(text);
			if (bytes.GetLength(0) + 1 > this.szHttpVersion.GetLength(0))
			{
				if ((long)bytes.GetLength(0) >= (long)((ulong)ResponseStatus.LENGTH_szHttpVersion))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szHttpVersion = new byte[bytes.GetLength(0) + 1];
			}
			for (int i = 0; i < bytes.GetLength(0); i++)
			{
				this.szHttpVersion[i] = bytes[i];
			}
			this.szHttpVersion[bytes.GetLength(0)] = 0;
			string text2 = "200";
			byte[] bytes2 = Encoding.get_ASCII().GetBytes(text2);
			if (bytes2.GetLength(0) + 1 > this.szStatusCode.GetLength(0))
			{
				if ((long)bytes2.GetLength(0) >= (long)((ulong)ResponseStatus.LENGTH_szStatusCode))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szStatusCode = new byte[bytes2.GetLength(0) + 1];
			}
			for (int j = 0; j < bytes2.GetLength(0); j++)
			{
				this.szStatusCode[j] = bytes2[j];
			}
			this.szStatusCode[bytes2.GetLength(0)] = 0;
			string text3 = "OK";
			byte[] bytes3 = Encoding.get_ASCII().GetBytes(text3);
			if (bytes3.GetLength(0) + 1 > this.szReasonPhrase.GetLength(0))
			{
				if ((long)bytes3.GetLength(0) >= (long)((ulong)ResponseStatus.LENGTH_szReasonPhrase))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szReasonPhrase = new byte[bytes3.GetLength(0) + 1];
			}
			for (int k = 0; k < bytes3.GetLength(0); k++)
			{
				this.szReasonPhrase[k] = bytes3[k];
			}
			this.szReasonPhrase[bytes3.GetLength(0)] = 0;
			return result;
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
			int num = TdrTypeUtil.cstrlen(this.szHttpVersion);
			if (num >= 32)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szHttpVersion, num);
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
			int num2 = TdrTypeUtil.cstrlen(this.szStatusCode);
			if (num2 >= 16)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szStatusCode, num2);
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
			int num3 = TdrTypeUtil.cstrlen(this.szReasonPhrase);
			if (num3 >= 64)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szReasonPhrase, num3);
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
					if (!this.has_HttpVersion())
					{
						this.set_has_HttpVersion();
					}
					int num2 = 0;
					errorType = srcBuf.readInt32(ref num2);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					if (num2 >= 32)
					{
						return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
					}
					errorType = srcBuf.readCString(ref this.szHttpVersion, num2);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					break;
				}
				case 2u:
				{
					if (!this.has_StatusCode())
					{
						this.set_has_StatusCode();
					}
					int num3 = 0;
					errorType = srcBuf.readInt32(ref num3);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					if (num3 >= 16)
					{
						return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
					}
					errorType = srcBuf.readCString(ref this.szStatusCode, num3);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					break;
				}
				case 3u:
				{
					if (!this.has_ReasonPhrase())
					{
						this.set_has_ReasonPhrase();
					}
					int num4 = 0;
					errorType = srcBuf.readInt32(ref num4);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					if (num4 >= 64)
					{
						return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
					}
					errorType = srcBuf.readCString(ref this.szReasonPhrase, num4);
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
			TdrError.ErrorType errorType = TdrBufUtil.printString(ref destBuf, indent, separator, "[szHttpVersion]", this.szHttpVersion);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = TdrBufUtil.printString(ref destBuf, indent, separator, "[szStatusCode]", this.szStatusCode);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = TdrBufUtil.printString(ref destBuf, indent, separator, "[szReasonPhrase]", this.szReasonPhrase);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}
	}
}
