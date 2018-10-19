using System;

public class TssSdtByte
{
	private TssSdtByteSlot m_slot;

	public static TssSdtByte NewTssSdtByte()
	{
		return new TssSdtByte
		{
			m_slot = TssSdtByteSlot.NewSlot(null)
		};
	}

	private byte GetValue()
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtByteSlot.NewSlot(null);
		}
		return this.m_slot.GetValue();
	}

	private void SetValue(byte v)
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtByteSlot.NewSlot(null);
		}
		this.m_slot.SetValue(v);
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return this.GetValue().GetHashCode();
	}

	public override string ToString()
	{
		return string.Format("{0}", this.GetValue());
	}

	public static implicit operator byte(TssSdtByte v)
	{
		if (v == null)
		{
			return 0;
		}
		return v.GetValue();
	}

	public static implicit operator TssSdtByte(byte v)
	{
		TssSdtByte tssSdtByte = new TssSdtByte();
		tssSdtByte.SetValue(v);
		return tssSdtByte;
	}

	public static TssSdtByte operator ++(TssSdtByte v)
	{
		TssSdtByte tssSdtByte = new TssSdtByte();
		if (v == null)
		{
			tssSdtByte.SetValue(1);
		}
		else
		{
			byte b = v.GetValue();
			b += 1;
			tssSdtByte.SetValue(b);
		}
		return tssSdtByte;
	}

	public static bool operator ==(TssSdtByte a, TssSdtByte b)
	{
		return (object.Equals(a, null) && object.Equals(b, null)) || (!object.Equals(a, null) && !object.Equals(b, null) && a.GetValue() == b.GetValue());
	}

	public static bool operator !=(TssSdtByte a, TssSdtByte b)
	{
		return (!object.Equals(a, null) || !object.Equals(b, null)) && (object.Equals(a, null) || object.Equals(b, null) || a.GetValue() != b.GetValue());
	}

	public static TssSdtByte operator --(TssSdtByte v)
	{
		TssSdtByte tssSdtByte = new TssSdtByte();
		if (v == null)
		{
			byte b = 0;
			b -= 1;
			tssSdtByte.SetValue(b);
		}
		else
		{
			byte b2 = v.GetValue();
			b2 -= 1;
			tssSdtByte.SetValue(b2);
		}
		return tssSdtByte;
	}
}
