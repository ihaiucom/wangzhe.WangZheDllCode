using System;

public class TssSdtUlongSlot
{
	private ulong[] m_value;

	private ulong m_xor_key;

	private int m_index;

	public TssSdtUlongSlot()
	{
		this.m_value = new ulong[TssSdtDataTypeFactory.GetValueArraySize()];
		this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
	}

	public static TssSdtUlongSlot NewSlot(TssSdtUlongSlot slot)
	{
		TssSdtUlongSlot.CollectSlot(slot);
		return new TssSdtUlongSlot();
	}

	private static void CollectSlot(TssSdtUlongSlot slot)
	{
	}

	public void SetValue(ulong v)
	{
		this.m_xor_key = TssSdtDataTypeFactory.GetUlongXORKey();
		int num = this.m_index + 1;
		if (num == this.m_value.Length)
		{
			num = 0;
		}
		this.m_value[num] = (v ^ this.m_xor_key);
		this.m_index = num;
	}

	public ulong GetValue()
	{
		ulong num = this.m_value[this.m_index];
		return num ^ this.m_xor_key;
	}
}
