using System;

public class TssSdtUintSlot
{
	private uint[] m_value;

	private uint m_xor_key;

	private int m_index;

	public TssSdtUintSlot()
	{
		this.m_value = new uint[TssSdtDataTypeFactory.GetValueArraySize()];
		this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
	}

	public static TssSdtUintSlot NewSlot(TssSdtUintSlot slot)
	{
		TssSdtUintSlot.CollectSlot(slot);
		return new TssSdtUintSlot();
	}

	private static void CollectSlot(TssSdtUintSlot slot)
	{
	}

	public void SetValue(uint v)
	{
		this.m_xor_key = TssSdtDataTypeFactory.GetUintXORKey();
		int num = this.m_index + 1;
		if (num == this.m_value.Length)
		{
			num = 0;
		}
		this.m_value[num] = (v ^ this.m_xor_key);
		this.m_index = num;
	}

	public uint GetValue()
	{
		uint num = this.m_value[this.m_index];
		return num ^ this.m_xor_key;
	}
}
