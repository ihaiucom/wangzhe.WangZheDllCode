using System;

public class TssSdtDoubleSlot
{
	private ulong[] m_value;

	private byte m_xor_key;

	private int m_index;

	public TssSdtDoubleSlot()
	{
		this.m_value = new ulong[TssSdtDataTypeFactory.GetValueArraySize()];
		this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
	}

	public static TssSdtDoubleSlot NewSlot(TssSdtDoubleSlot slot)
	{
		TssSdtDoubleSlot.CollectSlot(slot);
		return new TssSdtDoubleSlot();
	}

	private static void CollectSlot(TssSdtDoubleSlot slot)
	{
	}

	public void SetValue(double v)
	{
		this.m_xor_key = TssSdtDataTypeFactory.GetByteXORKey();
		int num = this.m_index + 1;
		if (num == this.m_value.Length)
		{
			num = 0;
		}
		this.m_value[num] = TssSdtDataTypeFactory.GetDoubleEncValue(v, this.m_xor_key);
		this.m_index = num;
	}

	public double GetValue()
	{
		ulong v = this.m_value[this.m_index];
		return TssSdtDataTypeFactory.GetDoubleDecValue(v, this.m_xor_key);
	}
}
