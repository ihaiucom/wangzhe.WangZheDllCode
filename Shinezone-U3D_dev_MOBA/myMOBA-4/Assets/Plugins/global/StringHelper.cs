using System;
using System.Text;
using UnityEngine;

public class StringHelper
{
	public static StringBuilder Formater = new StringBuilder(1024);

	public static void ClearFormater()
	{
		StringHelper.Formater.Remove(0, StringHelper.Formater.Length);
	}

	public static string BytesToString(byte[] bytes)
	{
		return Encoding.UTF8.GetString(bytes).TrimEnd(new char[1]);
	}

	public static string BytesToString_FindFristZero(byte[] bytes)
	{
		if (bytes == null)
		{
			return string.Empty;
		}
		int i;
		for (i = 0; i < bytes.Length; i++)
		{
			if (bytes[i] == 0)
			{
				break;
			}
		}
		return Encoding.UTF8.GetString(bytes, 0, i);
	}

	public static string BytesToString(string str)
	{
		return str;
	}

	public static string UTF8BytesToString(ref byte[] str)
	{
		string result;
		try
		{
			result = ((str == null) ? null : Encoding.UTF8.GetString(str).TrimEnd(new char[1]));
		}
		catch (Exception var_0_2F)
		{
			result = null;
		}
		return result;
	}

	public static string ASCIIBytesToString(byte[] data)
	{
		if (data == null)
		{
			return null;
		}
		string result;
		try
		{
			result = Encoding.ASCII.GetString(data).TrimEnd(new char[1]);
		}
		catch (Exception)
		{
			result = null;
		}
		return result;
	}

	public static string UTF8BytesToString(ref string str)
	{
		return str;
	}

	public static void StringToUTF8Bytes(string str, ref byte[] buffer)
	{
		if (str == null || buffer == null)
		{
			return;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(str);
		if (bytes.Length >= buffer.Length)
		{
			StringHelper.FillErrorCodeToBuf(ref buffer);
		}
		else
		{
			Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);
			buffer[bytes.Length] = 0;
		}
	}

	private static void FillErrorCodeToBuf(ref byte[] buffer)
	{
		try
		{
			buffer[0] = 79;
			buffer[1] = 86;
			buffer[2] = 69;
			buffer[3] = 82;
			buffer[4] = 70;
			buffer[5] = 76;
			buffer[6] = 79;
			buffer[7] = 87;
			buffer[8] = 48;
			buffer[9] = 88;
			buffer[10] = 67;
			buffer[11] = 67;
			buffer[12] = 67;
			buffer[13] = 67;
			buffer[14] = 67;
			buffer[15] = 67;
			buffer[16] = 0;
		}
		catch (Exception)
		{
		}
	}

	private static void FillErrorCodeToSBuf(ref sbyte[] buffer)
	{
		try
		{
			buffer[0] = 79;
			buffer[1] = 86;
			buffer[2] = 69;
			buffer[3] = 82;
			buffer[4] = 70;
			buffer[5] = 76;
			buffer[6] = 79;
			buffer[7] = 87;
			buffer[8] = 48;
			buffer[9] = 88;
			buffer[10] = 67;
			buffer[11] = 67;
			buffer[12] = 67;
			buffer[13] = 67;
			buffer[14] = 67;
			buffer[15] = 67;
			buffer[16] = 0;
		}
		catch (Exception)
		{
		}
	}

	public static bool IsAvailableString(string str)
	{
		int num = 0;
		int i = 0;
		bool flag = false;
		int length = str.Length;
		while (i < length)
		{
			char c = str[i];
			if (flag)
			{
				if (c < '\udc00' || c > '\udfff')
				{
					Debug.Log(string.Format("invalid utf-16 sequence at {0} (missing surrogate tail)", i));
					return false;
				}
				num += 4;
				flag = false;
			}
			else
			{
				if (c < '\u0080')
				{
					while (i < length)
					{
						if (str[i] >= '\u0080')
						{
							break;
						}
						num++;
						i++;
					}
					continue;
				}
				if (c < 'ࠀ')
				{
					num += 2;
				}
				else if (c >= '\ud800' && c <= '\udbff')
				{
					flag = true;
				}
				else
				{
					if (c >= '\udc00' && c <= '\udfff')
					{
						Debug.Log(string.Format("invalid utf-16 sequence at {0} (missing surrogate head)", i));
						return false;
					}
					num += 3;
				}
			}
			i++;
		}
		return true;
	}

	public static void StringToUTF8Bytes(string str, ref sbyte[] buffer)
	{
		if (str == null || buffer == null)
		{
			return;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(str);
		if (bytes.Length >= buffer.Length)
		{
			StringHelper.FillErrorCodeToSBuf(ref buffer);
		}
		else
		{
			Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);
			buffer[bytes.Length] = 0;
		}
	}
}
