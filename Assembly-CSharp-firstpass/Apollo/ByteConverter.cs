using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Apollo
{
	internal class ByteConverter
	{
		public static byte[] ReverseBytes(byte[] inArray)
		{
			int num = inArray.Length - 1;
			for (int i = 0; i < inArray.Length / 2; i++)
			{
				byte b = inArray[i];
				inArray[i] = inArray[num];
				inArray[num] = b;
				num--;
			}
			return inArray;
		}

		public static short ReverseEndian(short value)
		{
			if (BitConverter.IsLittleEndian)
			{
				return IPAddress.NetworkToHostOrder(value);
			}
			return value;
		}

		public static ushort ReverseEndian(ushort value)
		{
			if (BitConverter.IsLittleEndian)
			{
				return (ushort)IPAddress.NetworkToHostOrder((short)value);
			}
			return value;
		}

		public static int ReverseEndian(int value)
		{
			if (BitConverter.IsLittleEndian)
			{
				return IPAddress.NetworkToHostOrder(value);
			}
			return value;
		}

		public static uint ReverseEndian(uint value)
		{
			if (BitConverter.IsLittleEndian)
			{
				return (uint)IPAddress.NetworkToHostOrder((int)value);
			}
			return value;
		}

		public static long ReverseEndian(long value)
		{
			if (BitConverter.IsLittleEndian)
			{
				return IPAddress.NetworkToHostOrder(value);
			}
			return value;
		}

		public static ulong ReverseEndian(ulong value)
		{
			if (BitConverter.IsLittleEndian)
			{
				return (ulong)IPAddress.NetworkToHostOrder((long)value);
			}
			return value;
		}

		public static string Bytes2String(byte[] bytes)
		{
			string text = string.Empty;
			int i;
			for (i = 0; i < bytes.Length; i++)
			{
				if (bytes[i] == 0)
				{
					break;
				}
			}
			byte[] array = new byte[i];
			Array.Copy(bytes, array, array.Length);
			List<int> list = new List<int>();
			for (int j = 0; j < array.Length - 1; j++)
			{
				if (array[j] == 0)
				{
					break;
				}
				if (array[j] == 20)
				{
					list.Add(j);
					j++;
				}
			}
			if (list.get_Count() > 0)
			{
				if (list.get_Item(0) > 0)
				{
					text += Encoding.get_UTF8().GetString(array, 0, list.get_Item(0));
				}
				text += (char)array[list.get_Item(0)];
				text += (char)array[list.get_Item(0) + 1];
			}
			for (int k = 1; k < list.get_Count(); k++)
			{
				int num = list.get_Item(k) - list.get_Item(k - 1) - 2;
				if (num > 0)
				{
					text += Encoding.get_UTF8().GetString(array, list.get_Item(k - 1) + 2, num);
				}
				text += (char)array[list.get_Item(k)];
				text += (char)array[list.get_Item(k) + 1];
			}
			int num2 = 0;
			if (list.get_Count() > 0)
			{
				num2 = list.get_Item(list.get_Count() - 1) + 2;
			}
			if (num2 < array.Length)
			{
				text += Encoding.get_UTF8().GetString(array, num2, array.Length - num2);
			}
			return text;
		}

		public static bool IsCharValidate(char ch)
		{
			byte b = (byte)(ch >> 8 & 'ÿ');
			byte b2 = (byte)(ch & 'ÿ');
			return b != 0 || (b2 & 128) == 0;
		}

		public static byte[] String2Bytes(string strInput)
		{
			if (strInput == null)
			{
				return null;
			}
			return Encoding.get_UTF8().GetBytes(strInput);
		}
	}
}
