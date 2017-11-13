using System;

public class TssSdtDataTypeFactory
{
	private static byte m_byte_xor_key;

	private static short m_short_xor_key;

	private static ushort m_ushort_xor_key;

	private static int m_int_xor_key;

	private static uint m_uint_xor_key;

	private static long m_long_xor_key;

	private static ulong m_ulong_xor_key;

	public static byte GetByteXORKey()
	{
		if (TssSdtDataTypeFactory.m_byte_xor_key == 0)
		{
			Random random = new Random();
			TssSdtDataTypeFactory.m_byte_xor_key = (byte)random.Next(0, 255);
		}
		return TssSdtDataTypeFactory.m_byte_xor_key;
	}

	public static void SetByteXORKey(byte v)
	{
		TssSdtDataTypeFactory.m_byte_xor_key = v;
	}

	public static short GetShortXORKey()
	{
		if (TssSdtDataTypeFactory.m_short_xor_key == 0)
		{
			Random random = new Random();
			TssSdtDataTypeFactory.m_short_xor_key = (short)random.Next(0, 65535);
		}
		return TssSdtDataTypeFactory.m_short_xor_key;
	}

	public static ushort GetUshortXORKey()
	{
		if (TssSdtDataTypeFactory.m_ushort_xor_key == 0)
		{
			Random random = new Random();
			TssSdtDataTypeFactory.m_ushort_xor_key = (ushort)random.Next(0, 65535);
		}
		return TssSdtDataTypeFactory.m_ushort_xor_key;
	}

	public static int GetIntXORKey()
	{
		if (TssSdtDataTypeFactory.m_int_xor_key == 0)
		{
			Random random = new Random();
			TssSdtDataTypeFactory.m_int_xor_key = random.Next(0, 65535);
		}
		return TssSdtDataTypeFactory.m_int_xor_key;
	}

	public static uint GetUintXORKey()
	{
		if (TssSdtDataTypeFactory.m_uint_xor_key == 0u)
		{
			Random random = new Random();
			TssSdtDataTypeFactory.m_uint_xor_key = (uint)random.Next(0, 65535);
		}
		return TssSdtDataTypeFactory.m_uint_xor_key;
	}

	public static long GetLongXORKey()
	{
		if (TssSdtDataTypeFactory.m_long_xor_key == 0L)
		{
			Random random = new Random();
			TssSdtDataTypeFactory.m_long_xor_key = (long)random.Next(0, 65535);
		}
		return TssSdtDataTypeFactory.m_long_xor_key;
	}

	public static ulong GetUlongXORKey()
	{
		if (TssSdtDataTypeFactory.m_ulong_xor_key == 0uL)
		{
			Random random = new Random();
			TssSdtDataTypeFactory.m_ulong_xor_key = (ulong)((long)random.Next(0, 65535));
		}
		return TssSdtDataTypeFactory.m_ulong_xor_key;
	}

	public static int GetRandomValueIndex()
	{
		return TssSdtDataTypeFactory.m_int_xor_key;
	}

	public static int GetValueArraySize()
	{
		return 3;
	}

	public static uint GetFloatEncValue(float v, byte key)
	{
		byte[] bytes = BitConverter.GetBytes(v);
		for (int i = 0; i < bytes.Length; i++)
		{
			byte[] array = bytes;
			int num = i;
			byte[] expr_19_cp_0 = array;
			int expr_19_cp_1 = num;
			expr_19_cp_0[expr_19_cp_1] ^= key;
		}
		return BitConverter.ToUInt32(bytes, 0);
	}

	public static float GetFloatDecValue(uint v, byte key)
	{
		byte[] bytes = BitConverter.GetBytes(v);
		for (int i = 0; i < bytes.Length; i++)
		{
			byte[] array = bytes;
			int num = i;
			byte[] expr_19_cp_0 = array;
			int expr_19_cp_1 = num;
			expr_19_cp_0[expr_19_cp_1] ^= key;
		}
		return BitConverter.ToSingle(bytes, 0);
	}

	public static ulong GetDoubleEncValue(double v, byte key)
	{
		byte[] bytes = BitConverter.GetBytes(v);
		for (int i = 0; i < bytes.Length; i++)
		{
			byte[] array = bytes;
			int num = i;
			byte[] expr_19_cp_0 = array;
			int expr_19_cp_1 = num;
			expr_19_cp_0[expr_19_cp_1] ^= key;
		}
		return BitConverter.ToUInt64(bytes, 0);
	}

	public static double GetDoubleDecValue(ulong v, byte key)
	{
		byte[] bytes = BitConverter.GetBytes(v);
		for (int i = 0; i < bytes.Length; i++)
		{
			byte[] array = bytes;
			int num = i;
			byte[] expr_19_cp_0 = array;
			int expr_19_cp_1 = num;
			expr_19_cp_0[expr_19_cp_1] ^= key;
		}
		return BitConverter.ToDouble(bytes, 0);
	}
}
