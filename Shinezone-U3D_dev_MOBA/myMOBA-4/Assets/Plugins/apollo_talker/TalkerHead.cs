using ApolloTdr;
using System;

namespace apollo_talker
{
	public class TalkerHead : tsf4g_csharp_interface, IPackable, IUnpackable
	{
		public uint dwAsync;

		public byte bFlag;

		public byte bDomain;

		public byte bCmdFmt;

		public CmdValue stCommand;

		private uint[] has_bits_ = new uint[1];

		public TalkerHead()
		{
			this.stCommand = new CmdValue();
		}

		public void set_has_Async()
		{
			this.has_bits_[0] |= 1u;
		}

		private void clear_has_Async()
		{
			this.has_bits_[0] = (uint)((ulong)this.has_bits_[0] & 18446744073709551614uL);
		}

		public bool has_Async()
		{
			return (this.has_bits_[0] & 1u) != 0u;
		}

		public void set_has_Flag()
		{
			this.has_bits_[0] |= 2u;
		}

		private void clear_has_Flag()
		{
			this.has_bits_[0] = (uint)((ulong)this.has_bits_[0] & 18446744073709551613uL);
		}

		public bool has_Flag()
		{
			return (this.has_bits_[0] & 2u) != 0u;
		}

		public void set_has_Domain()
		{
			this.has_bits_[0] |= 4u;
		}

		private void clear_has_Domain()
		{
			this.has_bits_[0] = (uint)((ulong)this.has_bits_[0] & 18446744073709551611uL);
		}

		public bool has_Domain()
		{
			return (this.has_bits_[0] & 4u) != 0u;
		}

		public void set_has_CmdFmt()
		{
			this.has_bits_[0] |= 8u;
		}

		private void clear_has_CmdFmt()
		{
			this.has_bits_[0] = (uint)((ulong)this.has_bits_[0] & 18446744073709551607uL);
		}

		public bool has_CmdFmt()
		{
			return (this.has_bits_[0] & 8u) != 0u;
		}

		public void set_has_Command()
		{
			this.has_bits_[0] |= 16u;
		}

		private void clear_has_Command()
		{
			this.has_bits_[0] = (uint)((ulong)this.has_bits_[0] & 18446744073709551599uL);
		}

		public bool has_Command()
		{
			return (this.has_bits_[0] & 16u) != 0u;
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
				errorType = destBuf.writeVarUInt32(this.dwAsync);
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
				errorType = destBuf.writeUInt32(this.dwAsync);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			uint src3 = TdrTLV.makeTag(2, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_1_BYTE);
			errorType = destBuf.writeVarUInt32(src3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint src4 = TdrTLV.makeTag(3, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_1_BYTE);
			errorType = destBuf.writeVarUInt32(src4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bDomain);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint src5 = TdrTLV.makeTag(4, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_1_BYTE);
			errorType = destBuf.writeVarUInt32(src5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bCmdFmt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint src6 = TdrTLV.makeTag(5, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED);
			errorType = destBuf.writeVarUInt32(src6);
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
			errorType = this.stCommand.packTLV((long)this.bCmdFmt, ref destBuf, useVarInt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src7 = destBuf.getUsedSize() - usedSize - 4;
			errorType = destBuf.writeInt32(src7, usedSize);
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
					if (!this.has_Async())
					{
						this.set_has_Async();
					}
					if (useVarInt)
					{
						errorType = srcBuf.readVarUInt32(ref this.dwAsync);
						if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
						{
							return errorType;
						}
					}
					else
					{
						errorType = srcBuf.readUInt32(ref this.dwAsync);
						if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
						{
							return errorType;
						}
					}
					break;
				case 2u:
					if (!this.has_Flag())
					{
						this.set_has_Flag();
					}
					errorType = srcBuf.readUInt8(ref this.bFlag);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					break;
				case 3u:
					if (!this.has_Domain())
					{
						this.set_has_Domain();
					}
					errorType = srcBuf.readUInt8(ref this.bDomain);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					break;
				case 4u:
					if (!this.has_CmdFmt())
					{
						this.set_has_CmdFmt();
					}
					errorType = srcBuf.readUInt8(ref this.bCmdFmt);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					break;
				case 5u:
				{
					int length2 = 0;
					errorType = srcBuf.readInt32(ref length2);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					long num2 = 0L;
					errorType = this.stCommand.unpackTLV(ref num2, ref srcBuf, length2, useVarInt);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
					this.bCmdFmt = (byte)num2;
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
	}
}
