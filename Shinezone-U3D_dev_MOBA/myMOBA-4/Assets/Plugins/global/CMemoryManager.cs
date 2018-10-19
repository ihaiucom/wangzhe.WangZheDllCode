using System;
using System.Text;
using UnityEngine;

public class CMemoryManager
{
	private const int c_arraySize = 100;

	public static int[] s_intArray = new int[100];

	public static MonoBehaviour[] s_scriptArray = new MonoBehaviour[100];

	public static GameObject[] s_gameObjectArray = new GameObject[100];

	public static string[] s_stringArray = new string[100];

	public static void WriteByte(byte value, byte[] data, ref int offset)
	{
		data[offset] = value;
		offset++;
	}

	public static void WriteShort(short value, byte[] data, ref int offset)
	{
		data[offset] = (byte)value;
		data[offset + 1] = (byte)(value >> 8);
		offset += 2;
	}

	public static void WriteInt(int value, byte[] data, ref int offset)
	{
		data[offset] = (byte)value;
		data[offset + 1] = (byte)(value >> 8);
		data[offset + 2] = (byte)(value >> 16);
		data[offset + 3] = (byte)(value >> 24);
		offset += 4;
	}

	public static void WriteLong(long value, byte[] data, ref int offset)
	{
		int value2 = (int)value;
		int value3 = (int)(value >> 32);
		CMemoryManager.WriteInt(value2, data, ref offset);
		CMemoryManager.WriteInt(value3, data, ref offset);
	}

	public static int ReadByte(byte[] data, ref int offset)
	{
		int result = (int)data[offset];
		offset++;
		return result;
	}

	public static int ReadShort(byte[] data, ref int offset)
	{
		int result = (int)data[offset + 1] << 8 | (int)data[offset];
		offset += 2;
		return result;
	}

	public static int ReadInt(byte[] data, ref int offset)
	{
		int result = (int)data[offset + 3] << 24 | (int)data[offset + 2] << 16 | (int)data[offset + 1] << 8 | (int)data[offset];
		offset += 4;
		return result;
	}

	public static long ReadLong(byte[] data, ref int offset)
	{
		int num = CMemoryManager.ReadInt(data, ref offset);
		int num2 = CMemoryManager.ReadInt(data, ref offset);
		return (long)num2 << 32 | (long)num;
	}

	public static void WriteString(string str, byte[] data, ref int offset)
	{
		int bytes = Encoding.UTF8.GetBytes(str, 0, str.Length, data, offset + 2);
		CMemoryManager.WriteShort((short)bytes, data, ref offset);
		offset += bytes;
	}

	public static string ReadString(byte[] data, ref int offset)
	{
		int num = CMemoryManager.ReadShort(data, ref offset);
		string @string = Encoding.UTF8.GetString(data, offset, num);
		offset += num;
		return @string;
	}

	public static void WriteDateTime(ref DateTime dateTime, byte[] data, ref int offset)
	{
		byte[] bytes = BitConverter.GetBytes(dateTime.Ticks);
		for (int i = 0; i < bytes.Length; i++)
		{
			data[offset] = bytes[i];
			offset++;
		}
	}

	public static DateTime ReadDateTime(byte[] data, ref int offset)
	{
		long num = BitConverter.ToInt64(data, offset);
		offset += 8;
		if (num < DateTime.MaxValue.Ticks && num > DateTime.MinValue.Ticks)
		{
			DateTime result = new DateTime(num);
			return result;
		}
		return default(DateTime);
	}
}
