using System;

public class TssSdtLongSlot
{
	private long[] m_value;

	private long m_xor_key;

	private int m_index;

	public TssSdtLongSlot()
	{
		this.m_value = new long[TssSdtDataTypeFactory.GetValueArraySize()];
		this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
	}

	public static TssSdtLongSlot NewSlot(TssSdtLongSlot slot)
	{
		TssSdtLongSlot.CollectSlot(slot);
		return new TssSdtLongSlot();
	}

	private static void CollectSlot(TssSdtLongSlot slot)
	{
	}

	public void SetValue(long v)
	{
		this.m_xor_key = TssSdtDataTypeFactory.GetLongXORKey();
		int num = this.m_index + 1;
		if (num == this.m_value.Length)
		{
			num = 0;
		}
		this.m_value[num] = (v ^ this.m_xor_key);
		this.m_index = num;
	}

	public long GetValue()
	{
		long num = this.m_value[this.m_index];
		return num ^ this.m_xor_key;
	}
}
