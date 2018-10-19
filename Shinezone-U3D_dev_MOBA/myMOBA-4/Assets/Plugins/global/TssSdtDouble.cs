using System;

public class TssSdtDouble
{
	private TssSdtDoubleSlot m_slot;

	public static TssSdtDouble NewTssSdtDouble()
	{
		return new TssSdtDouble
		{
			m_slot = TssSdtDoubleSlot.NewSlot(null)
		};
	}

	private double GetValue()
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtDoubleSlot.NewSlot(null);
		}
		return this.m_slot.GetValue();
	}

	private void SetValue(double v)
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtDoubleSlot.NewSlot(null);
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

	public static implicit operator double(TssSdtDouble v)
	{
		if (v == null)
		{
			return 0.0;
		}
		return v.GetValue();
	}

	public static implicit operator TssSdtDouble(double v)
	{
		TssSdtDouble tssSdtDouble = new TssSdtDouble();
		tssSdtDouble.SetValue(v);
		return tssSdtDouble;
	}

	public static bool operator ==(TssSdtDouble a, TssSdtDouble b)
	{
		return (object.Equals(a, null) && object.Equals(b, null)) || (!object.Equals(a, null) && !object.Equals(b, null) && a.GetValue() == b.GetValue());
	}

	public static bool operator !=(TssSdtDouble a, TssSdtDouble b)
	{
		return (!object.Equals(a, null) || !object.Equals(b, null)) && (object.Equals(a, null) || object.Equals(b, null) || a.GetValue() != b.GetValue());
	}

	public static TssSdtDouble operator ++(TssSdtDouble v)
	{
		TssSdtDouble tssSdtDouble = new TssSdtDouble();
		if (v == null)
		{
			tssSdtDouble.SetValue(1.0);
		}
		else
		{
			double num = v.GetValue();
			num += 1.0;
			tssSdtDouble.SetValue(num);
		}
		return tssSdtDouble;
	}

	public static TssSdtDouble operator --(TssSdtDouble v)
	{
		TssSdtDouble tssSdtDouble = new TssSdtDouble();
		if (v == null)
		{
			tssSdtDouble.SetValue(-1.0);
		}
		else
		{
			double num = v.GetValue();
			num -= 1.0;
			tssSdtDouble.SetValue(num);
		}
		return tssSdtDouble;
	}
}
