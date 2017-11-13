using ApolloTdr;
using System;

namespace apollo_http_object
{
	public class RequestContent : IPackable, IUnpackable, tsf4g_csharp_interface
	{
		public uint dwDataLen;

		public byte[] szData;

		private uint[] has_bits_ = new uint[1];

		public RequestContent()
		{
			this.szData = new byte[8096];
		}

		public void set_has_DataLen()
		{
			this.has_bits_[0] |= 1u;
		}

		private void clear_has_DataLen()
		{
			this.has_bits_[0] = (uint)((ulong)this.has_bits_[0] & 18446744073709551614uL);
		}

		public bool has_DataLen()
		{
			return (this.has_bits_[0] & 1u) != 0u;
		}

		public void set_has_Data()
		{
			this.has_bits_[0] |= 2u;
		}

		private void clear_has_Data()
		{
			this.has_bits_[0] = (uint)((ulong)this.has_bits_[0] & 18446744073709551613uL);
		}

		public bool has_Data()
		{
			return (this.has_bits_[0] & 2u) != 0u;
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
			return TdrError.ErrorType.TDR_NO_ERROR;
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
			TdrError.ErrorType errorType;
			if (useVarInt)
			{
				uint src = TdrTLV.makeTag(1, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_VARINT);
				errorType = destBuf.writeVarUInt32(src);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				errorType = destBuf.writeVarUInt32(this.dwDataLen);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				uint src2 = TdrTLV.makeTag(1, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_4_BYTE);
				errorType = destBuf.writeVarUInt32(src2);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				errorType = destBuf.writeUInt32(this.dwDataLen);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (this.dwDataLen > 8096u)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.dwDataLen > 0u)
			{
				uint src3 = TdrTLV.makeTag(2, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED);
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
				int num = 0;
				while ((long)num < (long)((ulong)this.dwDataLen))
				{
					errorType = destBuf.writeUInt8(this.szData[num]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					num++;
				}
				int src4 = destBuf.getUsedSize() - usedSize - 4;
				errorType = destBuf.writeInt32(src4, usedSize);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
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
				uint fieldId = TdrTLV.getFieldId(tagid);
				uint num2 = fieldId;
				if (num2 != 1u)
				{
					if (num2 != 2u)
					{
						uint typeId = TdrTLV.getTypeId(tagid);
						errorType = TdrTLV.skipUnknownFields(ref srcBuf, (TdrTLV.TdrTLVTypeId)typeId);
						if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
						{
							return errorType;
						}
					}
					else
					{
						if (!this.has_Data())
						{
							this.set_has_Data();
						}
						int num3 = 0;
						errorType = srcBuf.readInt32(ref num3);
						if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
						{
							return errorType;
						}
						if (num3 == 0)
						{
							return TdrError.ErrorType.TDR_ERR_NULL_ARRAY;
						}
						int usedSize2 = srcBuf.getUsedSize();
						for (int i = 0; i < 8096; i++)
						{
							errorType = srcBuf.readUInt8(ref this.szData[i]);
							if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
							{
								return errorType;
							}
							if (srcBuf.getUsedSize() > usedSize2 + num3)
							{
								return TdrError.ErrorType.TDR_ERR_UNMATCHED_LENGTH;
							}
							if (srcBuf.getUsedSize() == usedSize2 + num3)
							{
								this.dwDataLen = (uint)(i + 1);
								break;
							}
						}
					}
				}
				else
				{
					if (!this.has_DataLen())
					{
						this.set_has_DataLen();
					}
					if (useVarInt)
					{
						errorType = srcBuf.readVarUInt32(ref this.dwDataLen);
						if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
						{
							return errorType;
						}
					}
					else
					{
						errorType = srcBuf.readUInt32(ref this.dwDataLen);
						if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
						{
							return errorType;
						}
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
			TdrError.ErrorType errorType = TdrBufUtil.printVariable(ref destBuf, indent, separator, "[dwDataLen]", "{0:d}", new object[]
			{
				this.dwDataLen
			});
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (8096u < this.dwDataLen)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			errorType = TdrBufUtil.printArray(ref destBuf, indent, separator, "[szData]", (long)((ulong)this.dwDataLen));
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwDataLen))
			{
				errorType = destBuf.sprintf("0x{0:x2}", new object[]
				{
					this.szData[num]
				});
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			errorType = destBuf.sprintf("{0}", new object[]
			{
				separator
			});
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}
	}
}
