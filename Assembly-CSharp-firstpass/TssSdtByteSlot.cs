using System;

public class TssSdtByteSlot
{
	private byte[] m_value;

	private byte m_xor_key;

	private int m_index;

	public TssSdtByteSlot()
	{
		this.m_value = new byte[TssSdtDataTypeFactory.GetValueArraySize()];
		this.m_index = TssSdtDataTypeFactory.GetRandomValueIndex() % this.m_value.Length;
	}

	public static TssSdtByteSlot NewSlot(TssSdtByteSlot slot)
	{
		TssSdtByteSlot.CollectSlot(slot);
		return new TssSdtByteSlot();
	}

	private static void CollectSlot(TssSdtByteSlot slot)
	{
	}

	public void SetValue(byte v)
	{
		this.m_xor_key = TssSdtDataTypeFactory.GetByteXORKey();
		int num = this.m_index + 1;
		if (num == this.m_value.Length)
		{
			num = 0;
		}
		byte b = v ^ this.m_xor_key;
		this.m_value[num] = b;
		this.m_index = num;
	}

	public byte GetValue()
	{
		byte b = this.m_value[this.m_index];
		return b ^ this.m_xor_key;
	}
}
