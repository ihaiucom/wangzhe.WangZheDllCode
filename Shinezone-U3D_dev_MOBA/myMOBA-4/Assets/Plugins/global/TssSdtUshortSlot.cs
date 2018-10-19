using System;

public class TssSdtUshortSlot
{
	private ushort[] m_value;

	private ushort m_xor_key;

	private int m_index;

	public TssSdtUshortSlot()
	{
		this.m_value = new ushort[TssSdtDataTypeFactory.GetValueArraySize()];
		this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
	}

	public static TssSdtUshortSlot NewSlot(TssSdtUshortSlot slot)
	{
		TssSdtUshortSlot.CollectSlot(slot);
		return new TssSdtUshortSlot();
	}

	private static void CollectSlot(TssSdtUshortSlot slot)
	{
	}

	public void SetValue(ushort v)
	{
		this.m_xor_key = TssSdtDataTypeFactory.GetUshortXORKey();
		int num = this.m_index + 1;
		if (num == this.m_value.Length)
		{
			num = 0;
		}
		ushort num2 = (ushort)(v ^ this.m_xor_key);
		this.m_value[num] = num2;
		this.m_index = num;
	}

	public ushort GetValue()
	{
		ushort num = this.m_value[this.m_index];
		return (ushort)(num ^ this.m_xor_key);
	}
}
