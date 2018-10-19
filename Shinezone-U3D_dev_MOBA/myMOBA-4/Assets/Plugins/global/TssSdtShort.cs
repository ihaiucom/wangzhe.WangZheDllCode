using System;

public class TssSdtShort
{
	private TssSdtShortSlot m_slot;

	public static TssSdtShort NewTssSdtShort()
	{
		return new TssSdtShort
		{
			m_slot = TssSdtShortSlot.NewSlot(null)
		};
	}

	private short GetValue()
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtShortSlot.NewSlot(null);
		}
		return this.m_slot.GetValue();
	}

	private void SetValue(short v)
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtShortSlot.NewSlot(null);
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

	public static implicit operator short(TssSdtShort v)
	{
		if (v == null)
		{
			return 0;
		}
		return v.GetValue();
	}

	public static implicit operator TssSdtShort(short v)
	{
		TssSdtShort tssSdtShort = new TssSdtShort();
		tssSdtShort.SetValue(v);
		return tssSdtShort;
	}

	public static bool operator ==(TssSdtShort a, TssSdtShort b)
	{
		return (object.Equals(a, null) && object.Equals(b, null)) || (!object.Equals(a, null) && !object.Equals(b, null) && a.GetValue() == b.GetValue());
	}

	public static bool operator !=(TssSdtShort a, TssSdtShort b)
	{
		return (!object.Equals(a, null) || !object.Equals(b, null)) && (object.Equals(a, null) || object.Equals(b, null) || a.GetValue() != b.GetValue());
	}

	public static TssSdtShort operator ++(TssSdtShort v)
	{
		TssSdtShort tssSdtShort = new TssSdtShort();
		if (v == null)
		{
			tssSdtShort.SetValue(1);
		}
		else
		{
			short num = v.GetValue();
			num += 1;
			tssSdtShort.SetValue(num);
		}
		return tssSdtShort;
	}

	public static TssSdtShort operator --(TssSdtShort v)
	{
		TssSdtShort tssSdtShort = new TssSdtShort();
		if (v == null)
		{
			tssSdtShort.SetValue(-1);
		}
		else
		{
			short num = v.GetValue();
			num -= 1;
			tssSdtShort.SetValue(num);
		}
		return tssSdtShort;
	}
}
