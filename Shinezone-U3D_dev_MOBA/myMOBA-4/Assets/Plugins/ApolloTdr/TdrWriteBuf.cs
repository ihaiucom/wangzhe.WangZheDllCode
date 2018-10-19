using System;
using System.Net;

namespace ApolloTdr
{
	public class TdrWriteBuf
	{
		private byte[] beginPtr;

		private int position;

		private int length;

		public TdrWriteBuf()
		{
			this.beginPtr = null;
			this.position = 0;
			this.length = 0;
		}

		public TdrWriteBuf(ref byte[] ptr, int len)
		{
			this._set(ref ptr, len);
		}

		public TdrWriteBuf(int len)
		{
			this.beginPtr = new byte[len];
			this.position = 0;
			this.length = 0;
			if (this.beginPtr != null)
			{
				this.length = len;
			}
		}

		private void _set(ref byte[] ptr, int len)
		{
			this.beginPtr = ptr;
			this.position = 0;
			this.length = 0;
			if (this.beginPtr != null)
			{
				this.length = len;
			}
		}

		private void _reset()
		{
			this.position = 0;
			this.length = 0;
			this.beginPtr = null;
		}

		public void reset()
		{
			this._reset();
		}

		public void set(ref byte[] ptr, int len)
		{
			this._set(ref ptr, len);
		}

		public int getUsedSize()
		{
			return this.position;
		}

		public int getTotalSize()
		{
			return this.length;
		}

		public int getLeftSize()
		{
			return this.length - this.position;
		}

		public byte[] getBeginPtr()
		{
			return this.beginPtr;
		}

		public TdrError.ErrorType reserve(int gap)
		{
			if (this.position > this.length)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			if (gap > this.length - this.position)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			this.position += gap;
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType writeInt8(sbyte src)
		{
			return this.writeUInt8((byte)src);
		}

		public TdrError.ErrorType writeUInt8(byte src)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (1 > this.length - this.position)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			this.beginPtr[this.position++] = src;
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType writeUInt16(ushort src)
		{
			return this.writeInt16((short)src);
		}

		public TdrError.ErrorType writeInt16(short src)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (2 > this.length - this.position)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			if (BitConverter.IsLittleEndian)
			{
				src = IPAddress.HostToNetworkOrder(src);
			}
			byte[] bytes = BitConverter.GetBytes(src);
			for (int i = 0; i < bytes.GetLength(0); i++)
			{
				this.beginPtr[this.position++] = bytes[i];
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType writeUInt32(uint src)
		{
			return this.writeInt32((int)src);
		}

		public TdrError.ErrorType writeInt32(int src)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (4 > this.length - this.position)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			if (BitConverter.IsLittleEndian)
			{
				src = IPAddress.HostToNetworkOrder(src);
			}
			byte[] bytes = BitConverter.GetBytes(src);
			for (int i = 0; i < bytes.GetLength(0); i++)
			{
				this.beginPtr[this.position++] = bytes[i];
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType writeUInt64(ulong src)
		{
			return this.writeInt64((long)src);
		}

		public TdrError.ErrorType writeInt64(long src)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (8 > this.length - this.position)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			if (BitConverter.IsLittleEndian)
			{
				src = IPAddress.HostToNetworkOrder(src);
			}
			byte[] bytes = BitConverter.GetBytes(src);
			for (int i = 0; i < bytes.GetLength(0); i++)
			{
				this.beginPtr[this.position++] = bytes[i];
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType writeFloat(float src)
		{
			int src2 = BitConverter.ToInt32(BitConverter.GetBytes(src), 0);
			return this.writeInt32(src2);
		}

		public TdrError.ErrorType writeDouble(double src)
		{
			long src2 = BitConverter.DoubleToInt64Bits(src);
			return this.writeInt64(src2);
		}

		public TdrError.ErrorType writeVarUInt16(ushort src)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (!BitConverter.IsLittleEndian)
			{
				src = (ushort)IPAddress.HostToNetworkOrder((short)src);
			}
			while (this.length - this.position > 0)
			{
				byte[] bytes = BitConverter.GetBytes(src);
				byte b = (byte)(bytes[0] & (byte)127);
				src = (ushort)(src >> 7);
				if (src != 0)
				{
					b |= 128;
				}
				this.beginPtr[this.position++] = b;
				if (src == 0)
				{
					return TdrError.ErrorType.TDR_NO_ERROR;
				}
			}
			return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
		}

		public TdrError.ErrorType writeVarInt16(short src)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (!BitConverter.IsLittleEndian)
			{
				src = IPAddress.HostToNetworkOrder(src);
			}
			src = (short)((int)src << 1 ^ src >> 15);
			ushort num = (ushort)src;
			while (this.length - this.position > 0)
			{
				byte[] bytes = BitConverter.GetBytes(num);
				byte b = (byte)(bytes[0] & (byte)127);
				num = (ushort)(num >> 7);
				if (num != 0)
				{
					b |= 128;
				}
				this.beginPtr[this.position++] = b;
				if (num == 0)
				{
					return TdrError.ErrorType.TDR_NO_ERROR;
				}
			}
			return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
		}

		public TdrError.ErrorType writeVarUInt32(uint src)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (!BitConverter.IsLittleEndian)
			{
				src = (uint)IPAddress.HostToNetworkOrder((int)src);
			}
			while (this.length - this.position > 0)
			{
				byte[] bytes = BitConverter.GetBytes(src);
				byte b = (byte)(bytes[0] & (byte)127);
				src >>= 7;
				if (src != 0u)
				{
					b |= 128;
				}
				this.beginPtr[this.position++] = b;
				if (src == 0u)
				{
					return TdrError.ErrorType.TDR_NO_ERROR;
				}
			}
			return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
		}

		public TdrError.ErrorType writeVarInt32(int src)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (!BitConverter.IsLittleEndian)
			{
				src = IPAddress.HostToNetworkOrder(src);
			}
			src = (src << 1 ^ src >> 31);
			uint num = (uint)src;
			while (this.length - this.position > 0)
			{
				byte[] bytes = BitConverter.GetBytes(num);
				byte b = (byte)(bytes[0] & (byte)127);
				num >>= 7;
				if (num != 0u)
				{
					b |= 128;
				}
				this.beginPtr[this.position++] = b;
				if (num == 0u)
				{
					return TdrError.ErrorType.TDR_NO_ERROR;
				}
			}
			return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
		}

		public TdrError.ErrorType writeVarUInt64(ulong src)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (!BitConverter.IsLittleEndian)
			{
				src = (ulong)IPAddress.HostToNetworkOrder((long)src);
			}
			while (this.length - this.position > 0)
			{
				byte[] bytes = BitConverter.GetBytes(src);
				byte b = (byte)(bytes[0] & (byte)127);
				src >>= 7;
				if (src != 0uL)
				{
					b |= 128;
				}
				this.beginPtr[this.position++] = b;
				if (src == 0uL)
				{
					return TdrError.ErrorType.TDR_NO_ERROR;
				}
			}
			return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
		}

		public TdrError.ErrorType writeVarInt64(long src)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (!BitConverter.IsLittleEndian)
			{
				src = IPAddress.HostToNetworkOrder(src);
			}
			src = (src << 1 ^ src >> 63);
			ulong num = (ulong)src;
			while (this.length - this.position > 0)
			{
				byte[] bytes = BitConverter.GetBytes(num);
				byte b = (byte)(bytes[0] & (byte)127);
				num >>= 7;
				if (num != 0uL)
				{
					b |= 128;
				}
				this.beginPtr[this.position++] = b;
				if (num == 0uL)
				{
					return TdrError.ErrorType.TDR_NO_ERROR;
				}
			}
			return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
		}

		public TdrError.ErrorType writeCString(byte[] src, int count)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (count > this.length - this.position)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			for (int i = 0; i < count; i++)
			{
				this.beginPtr[this.position++] = src[i];
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType writeWString(short[] src, int count)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (2 * count > this.length - this.position)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			for (int i = 0; i < count; i++)
			{
				byte[] bytes = BitConverter.GetBytes(src[i]);
				for (int j = 0; j < bytes.GetLength(0); j++)
				{
					this.beginPtr[this.position++] = bytes[j];
				}
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType writeInt8(sbyte src, int pos)
		{
			return this.writeUInt8((byte)src, pos);
		}

		public TdrError.ErrorType writeUInt8(byte src, int pos)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (1 > this.length - pos)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			this.beginPtr[pos] = src;
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType writeUInt16(ushort src, int pos)
		{
			return this.writeInt16((short)src, pos);
		}

		public TdrError.ErrorType writeInt16(short src, int pos)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (2 > this.length - pos)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			if (BitConverter.IsLittleEndian)
			{
				src = IPAddress.HostToNetworkOrder(src);
			}
			byte[] bytes = BitConverter.GetBytes(src);
			for (int i = 0; i < bytes.GetLength(0); i++)
			{
				this.beginPtr[pos + i] = bytes[i];
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType writeUInt32(uint src, int pos)
		{
			return this.writeInt32((int)src, pos);
		}

		public TdrError.ErrorType writeInt32(int src, int pos)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (4 > this.length - pos)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			if (BitConverter.IsLittleEndian)
			{
				src = IPAddress.HostToNetworkOrder(src);
			}
			byte[] bytes = BitConverter.GetBytes(src);
			for (int i = 0; i < bytes.GetLength(0); i++)
			{
				this.beginPtr[pos + i] = bytes[i];
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType writeUInt64(ulong src, int pos)
		{
			return this.writeInt64((long)src, pos);
		}

		public TdrError.ErrorType writeInt64(long src, int pos)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (8 > this.length - pos)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			if (BitConverter.IsLittleEndian)
			{
				src = IPAddress.HostToNetworkOrder(src);
			}
			byte[] bytes = BitConverter.GetBytes(src);
			for (int i = 0; i < bytes.GetLength(0); i++)
			{
				this.beginPtr[pos + i] = bytes[i];
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType writeFloat(float src, int pos)
		{
			int src2 = BitConverter.ToInt32(BitConverter.GetBytes(src), 0);
			return this.writeInt32(src2, pos);
		}

		public TdrError.ErrorType writeDouble(double src, int pos)
		{
			long src2 = BitConverter.DoubleToInt64Bits(src);
			return this.writeInt64(src2, pos);
		}

		public TdrError.ErrorType writeCString(byte[] src, int count, int pos)
		{
			if (this.beginPtr == null || count > src.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (count > this.length - pos)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			for (int i = 0; i < count; i++)
			{
				this.beginPtr[pos + i] = src[i];
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType writeWString(short[] src, int count, int pos)
		{
			if (this.beginPtr == null || count > src.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (2 * count > this.length - pos)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			for (int i = 0; i < count; i++)
			{
				byte[] bytes = BitConverter.GetBytes(src[i]);
				for (int j = 0; j < bytes.GetLength(0); j++)
				{
					this.beginPtr[pos + (2 * i + j)] = bytes[j];
				}
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}
	}
}
