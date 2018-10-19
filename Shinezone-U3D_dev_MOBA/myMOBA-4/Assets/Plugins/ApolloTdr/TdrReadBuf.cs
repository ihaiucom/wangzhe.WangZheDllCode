using System;
using System.Net;

namespace ApolloTdr
{
	public class TdrReadBuf
	{
		private byte[] beginPtr;

		private int position;

		private int length;

		private bool IsNetEndian;

		public TdrReadBuf()
		{
			this.length = 0;
			this.position = 0;
			this.beginPtr = null;
			this.IsNetEndian = true;
		}

		public TdrReadBuf(ref TdrWriteBuf writeBuf)
		{
			byte[] array = writeBuf.getBeginPtr();
			this.set(ref array, writeBuf.getUsedSize());
		}

		public TdrReadBuf(ref byte[] ptr, int len)
		{
			this.set(ref ptr, len);
		}

		public void reset()
		{
			this.length = 0;
			this.position = 0;
			this.beginPtr = null;
			this.IsNetEndian = true;
		}

		public void set(ref byte[] ptr, int len)
		{
			this.beginPtr = ptr;
			this.position = 0;
			this.length = 0;
			this.IsNetEndian = true;
			if (this.beginPtr != null)
			{
				this.length = len;
			}
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

		public void disableEndian()
		{
			this.IsNetEndian = false;
		}

		public TdrError.ErrorType readInt8(ref sbyte dest)
		{
			byte b = 0;
			TdrError.ErrorType result = this.readUInt8(ref b);
			dest = (sbyte)b;
			return result;
		}

		public TdrError.ErrorType readUInt8(ref byte dest)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (1 > this.length - this.position)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			dest = this.beginPtr[this.position++];
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType readInt16(ref short dest)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (2 > this.length - this.position)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			dest = BitConverter.ToInt16(this.beginPtr, this.position);
			this.position += 2;
			if (this.IsNetEndian && BitConverter.IsLittleEndian)
			{
				dest = IPAddress.NetworkToHostOrder(dest);
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType readUInt16(ref ushort dest)
		{
			short num = 0;
			TdrError.ErrorType result = this.readInt16(ref num);
			dest = (ushort)num;
			return result;
		}

		public TdrError.ErrorType readInt32(ref int dest)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (4 > this.length - this.position)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			dest = BitConverter.ToInt32(this.beginPtr, this.position);
			this.position += 4;
			if (this.IsNetEndian && BitConverter.IsLittleEndian)
			{
				dest = IPAddress.NetworkToHostOrder(dest);
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType readUInt32(ref uint dest)
		{
			int num = 0;
			TdrError.ErrorType result = this.readInt32(ref num);
			dest = (uint)num;
			return result;
		}

		public TdrError.ErrorType readInt64(ref long dest)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (8 > this.length - this.position)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			dest = BitConverter.ToInt64(this.beginPtr, this.position);
			this.position += 8;
			if (this.IsNetEndian && BitConverter.IsLittleEndian)
			{
				dest = IPAddress.NetworkToHostOrder(dest);
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType readUInt64(ref ulong dest)
		{
			long num = 0L;
			TdrError.ErrorType result = this.readInt64(ref num);
			dest = (ulong)num;
			return result;
		}

		public TdrError.ErrorType readFloat(ref float dest)
		{
			int value = 0;
			TdrError.ErrorType result = this.readInt32(ref value);
			dest = BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
			return result;
		}

		public TdrError.ErrorType readDouble(ref double dest)
		{
			long value = 0L;
			TdrError.ErrorType result = this.readInt64(ref value);
			dest = BitConverter.Int64BitsToDouble(value);
			return result;
		}

		public TdrError.ErrorType readVarUInt16(ref ushort dest)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			dest = 0;
			int num = 0;
			while (this.length - this.position > 0)
			{
				byte b = this.beginPtr[this.position++];
				dest |= (ushort)(((long)b & 127L) << 7 * num);
				num++;
				if ((b & 128) == 0)
				{
					if (!BitConverter.IsLittleEndian)
					{
						dest = (ushort)IPAddress.NetworkToHostOrder((short)dest);
					}
					return TdrError.ErrorType.TDR_NO_ERROR;
				}
			}
			return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
		}

		public TdrError.ErrorType readVarInt16(ref short dest)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			dest = 0;
			int num = 0;
			while (this.length - this.position > 0)
			{
				byte b = this.beginPtr[this.position++];
				dest |= (short)(((long)b & 127L) << 7 * num);
				num++;
				if ((b & 128) == 0)
				{
					if ((dest & 1) != 0)
					{
						dest = (short)((((int)dest ^ 65535) >> 1 & -32769) | (int)(dest & 1) << 15);
					}
					else
					{
						dest = (short)((dest >> 1 & -32769) | (int)(dest & 1) << 15);
					}
					if (!BitConverter.IsLittleEndian)
					{
						dest = IPAddress.NetworkToHostOrder(dest);
					}
					return TdrError.ErrorType.TDR_NO_ERROR;
				}
			}
			return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
		}

		public TdrError.ErrorType readVarUInt32(ref uint dest)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			dest = 0u;
			int num = 0;
			while (this.length - this.position > 0)
			{
				byte b = this.beginPtr[this.position++];
				dest |= (uint)((uint)((long)b & 127L) << 7 * num);
				num++;
				if ((b & 128) == 0)
				{
					if (!BitConverter.IsLittleEndian)
					{
						dest = (uint)IPAddress.NetworkToHostOrder((int)dest);
					}
					return TdrError.ErrorType.TDR_NO_ERROR;
				}
			}
			return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
		}

		public TdrError.ErrorType readVarInt32(ref int dest)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			dest = 0;
			int num = 0;
			while (this.length - this.position > 0)
			{
				byte b = this.beginPtr[this.position++];
				dest |= (int)((int)((long)b & 127L) << 7 * num);
				num++;
				if ((b & 128) == 0)
				{
					if ((dest & 1) != 0)
					{
						uint num2 = (uint)(dest ^ -1) >> 1;
						num2 &= 2147483647u;
						dest = (int)(num2 | (uint)((uint)(dest & 1) << 31));
					}
					else
					{
						uint num3 = (uint)(dest >> 1 & 2147483647);
						dest = (int)(num3 | (uint)((uint)(dest & 1) << 31));
					}
					if (!BitConverter.IsLittleEndian)
					{
						dest = IPAddress.NetworkToHostOrder(dest);
					}
					return TdrError.ErrorType.TDR_NO_ERROR;
				}
			}
			return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
		}

		public TdrError.ErrorType readVarUInt64(ref ulong dest)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			dest = 0uL;
			int num = 0;
			while (this.length - this.position > 0)
			{
				byte b = this.beginPtr[this.position++];
				dest |= (ulong)((ulong)((long)b & 127L) << 7 * num);
				num++;
				if ((b & 128) == 0)
				{
					if (!BitConverter.IsLittleEndian)
					{
						dest = (ulong)IPAddress.NetworkToHostOrder((long)dest);
					}
					return TdrError.ErrorType.TDR_NO_ERROR;
				}
			}
			return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
		}

		public TdrError.ErrorType readVarInt64(ref long dest)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			dest = 0L;
			int num = 0;
			while (this.length - this.position > 0)
			{
				byte b = this.beginPtr[this.position++];
				dest |= ((long)b & 127L) << 7 * num;
				num++;
				if ((b & 128) == 0)
				{
					if ((dest & 1L) != 0L)
					{
						ulong num2 = (ulong)(dest ^ -1L) >> 1;
						num2 &= 9223372036854775807uL;
						dest = (long)(num2 | (ulong)((ulong)(dest & 1L) << 63));
					}
					else
					{
						ulong num3 = (ulong)(dest >> 1 & 9223372036854775807L);
						dest = (long)(num3 | (ulong)((ulong)(dest & 1L) << 63));
					}
					if (!BitConverter.IsLittleEndian)
					{
						dest = IPAddress.NetworkToHostOrder(dest);
					}
					return TdrError.ErrorType.TDR_NO_ERROR;
				}
			}
			return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
		}

		public TdrError.ErrorType skipForward(int step)
		{
			if (this.length - this.position < step)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			this.position += step;
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType readCString(ref byte[] dest, int count)
		{
			if (this.beginPtr == null || count > dest.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (dest == null || dest.GetLength(0) == 0)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (count > this.length - this.position)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			for (int i = 0; i < count; i++)
			{
				dest[i] = this.beginPtr[this.position++];
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType readWString(ref short[] dest, int count)
		{
			if (this.beginPtr == null || count > dest.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (dest == null || dest.GetLength(0) == 0)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (2 * count > this.length - this.position)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			for (int i = 0; i < count; i++)
			{
				dest[i] = BitConverter.ToInt16(this.beginPtr, this.position);
				this.position += 2;
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType readInt8(ref sbyte dest, int pos)
		{
			byte b = 0;
			TdrError.ErrorType result = this.readUInt8(ref b, pos);
			dest = (sbyte)b;
			return result;
		}

		public TdrError.ErrorType readUInt8(ref byte dest, int pos)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (1 > this.length - pos)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			dest = this.beginPtr[pos];
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType readInt16(ref short dest, int pos)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (2 > this.length - pos)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			dest = BitConverter.ToInt16(this.beginPtr, pos);
			if (this.IsNetEndian && BitConverter.IsLittleEndian)
			{
				dest = IPAddress.NetworkToHostOrder(dest);
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType readUInt16(ref ushort dest, int pos)
		{
			short num = 0;
			TdrError.ErrorType result = this.readInt16(ref num, pos);
			dest = (ushort)num;
			return result;
		}

		public TdrError.ErrorType readInt32(ref int dest, int pos)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (4 > this.length - pos)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			dest = BitConverter.ToInt32(this.beginPtr, pos);
			if (this.IsNetEndian && BitConverter.IsLittleEndian)
			{
				dest = IPAddress.NetworkToHostOrder(dest);
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType readUInt32(ref uint dest, int pos)
		{
			int num = 0;
			TdrError.ErrorType result = this.readInt32(ref num, pos);
			dest = (uint)num;
			return result;
		}

		public TdrError.ErrorType readInt64(ref long dest, int pos)
		{
			if (this.beginPtr == null)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (8 > this.length - pos)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			dest = BitConverter.ToInt64(this.beginPtr, pos);
			if (this.IsNetEndian && BitConverter.IsLittleEndian)
			{
				dest = IPAddress.NetworkToHostOrder(dest);
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType readUInt64(ref ulong dest, int pos)
		{
			long num = 0L;
			TdrError.ErrorType result = this.readInt64(ref num, pos);
			dest = (ulong)num;
			return result;
		}

		public TdrError.ErrorType readFloat(ref float dest, int pos)
		{
			int value = 0;
			TdrError.ErrorType result = this.readInt32(ref value, pos);
			dest = BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
			return result;
		}

		public TdrError.ErrorType readDouble(ref double dest, int pos)
		{
			long value = 0L;
			TdrError.ErrorType result = this.readInt64(ref value, pos);
			dest = BitConverter.Int64BitsToDouble(value);
			return result;
		}

		public TdrError.ErrorType readCString(ref byte[] dest, int count, int pos)
		{
			if (this.beginPtr == null || count > dest.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (dest == null || dest.GetLength(0) == 0)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (count > this.length - pos)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			for (int i = 0; i < count; i++)
			{
				dest[i] = this.beginPtr[pos + count];
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType readWString(ref short[] dest, int count, int pos)
		{
			if (this.beginPtr == null || count > dest.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (dest == null || dest.GetLength(0) == 0)
			{
				return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
			}
			if (2 * count > this.length - pos)
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			for (int i = 0; i < count; i++)
			{
				dest[i] = BitConverter.ToInt16(this.beginPtr, pos + 2 * count);
			}
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType toHexStr(ref char[] buffer, out int usedsize)
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			int num = this.length - this.position;
			int num2 = num * 2 + 1;
			if (buffer.GetLength(0) < num2)
			{
				usedsize = 0;
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
			}
			string text = string.Empty;
			byte[] array = new byte[this.length - this.position];
			for (int i = 0; i < this.length - this.position; i++)
			{
				errorType = this.readUInt8(ref array[i], this.position + i);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					usedsize = 0;
					return errorType;
				}
				text += string.Format("{0:x2}", array[i]);
			}
			text += string.Format("{0:x}", 0);
			buffer = text.ToCharArray();
			usedsize = num2;
			return errorType;
		}

		public TdrError.ErrorType toHexStr(ref string buffer)
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			byte[] array = new byte[this.length - this.position];
			for (int i = 0; i < this.length - this.position; i++)
			{
				errorType = this.readUInt8(ref array[i], this.position + i);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				buffer += string.Format("{0:x2}", array[i]);
			}
			buffer += string.Format("{0:x}", 0);
			return errorType;
		}
	}
}
