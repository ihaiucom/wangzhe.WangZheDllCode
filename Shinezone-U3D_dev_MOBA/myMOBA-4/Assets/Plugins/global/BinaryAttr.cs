using System;
using System.Text;

public struct BinaryAttr
{
	public static readonly int INT_LEN = 4;

	private byte[] m_data;

	private int m_offset;

	private int m_dataLen;

	private int m_nameOffset;

	private int m_nameLen;

	private int m_valueOffset;

	private int m_valueLen;

	internal BinaryAttr(byte[] data, int offset = 0)
	{
		this.m_data = data;
		this.m_offset = offset;
		this.m_dataLen = BitConverter.ToInt32(data, offset);
		this.m_nameOffset = offset + BinaryAttr.INT_LEN;
		this.m_nameLen = BitConverter.ToInt32(data, this.m_nameOffset) - BinaryAttr.INT_LEN;
		this.m_valueOffset = this.m_nameOffset + this.m_nameLen + BinaryAttr.INT_LEN;
		this.m_valueLen = this.m_offset + this.m_dataLen - this.m_valueOffset;
	}

	public byte[] GetValue()
	{
		if (this.m_valueLen == 0)
		{
			return null;
		}
		byte[] array = new byte[this.m_valueLen];
		Array.Copy(this.m_data, this.m_valueOffset, array, 0, array.Length);
		return array;
	}

	public string GetValueString()
	{
		return this.GetValueString(Encoding.UTF8);
	}

	public string GetValueString(Encoding encoding)
	{
		if (this.m_valueLen == 0)
		{
			return null;
		}
		return Encoding.UTF8.GetString(this.m_data, this.m_valueOffset, this.m_valueLen);
	}

	public string GetName()
	{
		return Encoding.UTF8.GetString(this.m_data, this.m_nameOffset + BinaryAttr.INT_LEN, this.m_nameLen);
	}
}
