using System;

public class TssSdtFloatSlot
{
	private uint[] m_value;

	private byte m_xor_key;

	private int m_index;

	public TssSdtFloatSlot()
	{
		this.m_value = new uint[TssSdtDataTypeFactory.GetValueArraySize()];
		this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
	}

	public static TssSdtFloatSlot NewSlot(TssSdtFloatSlot slot)
	{
		TssSdtFloatSlot.CollectSlot(slot);
		return new TssSdtFloatSlot();
	}

	private static void CollectSlot(TssSdtFloatSlot slot)
	{
	}

	public void SetValue(float v)
	{
		this.m_xor_key = TssSdtDataTypeFactory.GetByteXORKey();
		int num = this.m_index + 1;
		if (num == this.m_value.Length)
		{
			num = 0;
		}
		this.m_value[num] = TssSdtDataTypeFactory.GetFloatEncValue(v, this.m_xor_key);
		this.m_index = num;
	}

	public float GetValue()
	{
		uint v = this.m_value[this.m_index];
		return TssSdtDataTypeFactory.GetFloatDecValue(v, this.m_xor_key);
	}
}
