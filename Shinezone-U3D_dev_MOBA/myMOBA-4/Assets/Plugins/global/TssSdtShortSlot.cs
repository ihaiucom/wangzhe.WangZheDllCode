using System;

public class TssSdtShortSlot
{
	private short[] m_value;

	private short m_xor_key;

	private int m_index;

	public TssSdtShortSlot()
	{
		this.m_value = new short[TssSdtDataTypeFactory.GetValueArraySize()];
		this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
	}

	public static TssSdtShortSlot NewSlot(TssSdtShortSlot slot)
	{
		TssSdtShortSlot.CollectSlot(slot);
		return new TssSdtShortSlot();
	}

	private static void CollectSlot(TssSdtShortSlot slot)
	{
	}

	public void SetValue(short v)
	{
		this.m_xor_key = TssSdtDataTypeFactory.GetShortXORKey();
		int num = this.m_index + 1;
		if (num == this.m_value.Length)
		{
			num = 0;
		}
		short num2 = (short)(v ^ this.m_xor_key);
		this.m_value[num] = num2;
		this.m_index = num;
	}

	public short GetValue()
	{
		short num = this.m_value[this.m_index];
		return (short)(num ^ this.m_xor_key);
	}
}
