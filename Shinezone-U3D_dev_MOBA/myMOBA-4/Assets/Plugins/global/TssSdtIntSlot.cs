using System;

public class TssSdtIntSlot
{
	private int[] m_value;

	private int m_xor_key;

	private int m_index;

	public TssSdtIntSlot()
	{
		this.m_value = new int[TssSdtDataTypeFactory.GetValueArraySize()];
		this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
	}

	public static TssSdtIntSlot NewSlot(TssSdtIntSlot slot)
	{
		TssSdtIntSlot.CollectSlot(slot);
		return new TssSdtIntSlot();
	}

	private static void CollectSlot(TssSdtIntSlot slot)
	{
	}

	public void SetValue(int v)
	{
		this.m_xor_key = TssSdtDataTypeFactory.GetIntXORKey();
		int num = this.m_index + 1;
		if (num == this.m_value.Length)
		{
			num = 0;
		}
		this.m_value[num] = (v ^ this.m_xor_key);
		this.m_index = num;
	}

	public int GetValue()
	{
		int num = this.m_value[this.m_index];
		return num ^ this.m_xor_key;
	}
}
