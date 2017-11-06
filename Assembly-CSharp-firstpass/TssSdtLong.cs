using System;

public class TssSdtLong
{
	private TssSdtLongSlot m_slot;

	public static TssSdtLong NewTssSdtLong()
	{
		return new TssSdtLong
		{
			m_slot = TssSdtLongSlot.NewSlot(null)
		};
	}

	private long GetValue()
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtLongSlot.NewSlot(null);
		}
		return this.m_slot.GetValue();
	}

	private void SetValue(long v)
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtLongSlot.NewSlot(null);
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

	public static implicit operator long(TssSdtLong v)
	{
		if (v == null)
		{
			return 0L;
		}
		return v.GetValue();
	}

	public static implicit operator TssSdtLong(long v)
	{
		TssSdtLong tssSdtLong = new TssSdtLong();
		tssSdtLong.SetValue(v);
		return tssSdtLong;
	}

	public static bool operator ==(TssSdtLong a, TssSdtLong b)
	{
		return (object.Equals(a, null) && object.Equals(b, null)) || (!object.Equals(a, null) && !object.Equals(b, null) && a.GetValue() == b.GetValue());
	}

	public static bool operator !=(TssSdtLong a, TssSdtLong b)
	{
		return (!object.Equals(a, null) || !object.Equals(b, null)) && (object.Equals(a, null) || object.Equals(b, null) || a.GetValue() != b.GetValue());
	}

	public static TssSdtLong operator ++(TssSdtLong v)
	{
		TssSdtLong tssSdtLong = new TssSdtLong();
		if (v == null)
		{
			tssSdtLong.SetValue(1L);
		}
		else
		{
			long num = v.GetValue();
			num += 1L;
			tssSdtLong.SetValue(num);
		}
		return tssSdtLong;
	}

	public static TssSdtLong operator --(TssSdtLong v)
	{
		TssSdtLong tssSdtLong = new TssSdtLong();
		if (v == null)
		{
			tssSdtLong.SetValue(-1L);
		}
		else
		{
			long num = v.GetValue();
			num -= 1L;
			tssSdtLong.SetValue(num);
		}
		return tssSdtLong;
	}
}
