using System;

public class TssSdtUlong
{
	private TssSdtUlongSlot m_slot;

	public static TssSdtUlong NewTssSdtUlong()
	{
		return new TssSdtUlong
		{
			m_slot = TssSdtUlongSlot.NewSlot(null)
		};
	}

	private ulong GetValue()
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtUlongSlot.NewSlot(null);
		}
		return this.m_slot.GetValue();
	}

	private void SetValue(ulong v)
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtUlongSlot.NewSlot(null);
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

	public static implicit operator ulong(TssSdtUlong v)
	{
		if (v == null)
		{
			return 0uL;
		}
		return v.GetValue();
	}

	public static implicit operator TssSdtUlong(ulong v)
	{
		TssSdtUlong tssSdtUlong = new TssSdtUlong();
		tssSdtUlong.SetValue(v);
		return tssSdtUlong;
	}

	public static bool operator ==(TssSdtUlong a, TssSdtUlong b)
	{
		return (object.Equals(a, null) && object.Equals(b, null)) || (!object.Equals(a, null) && !object.Equals(b, null) && a.GetValue() == b.GetValue());
	}

	public static bool operator !=(TssSdtUlong a, TssSdtUlong b)
	{
		return (!object.Equals(a, null) || !object.Equals(b, null)) && (object.Equals(a, null) || object.Equals(b, null) || a.GetValue() != b.GetValue());
	}

	public static TssSdtUlong operator ++(TssSdtUlong v)
	{
		TssSdtUlong tssSdtUlong = new TssSdtUlong();
		if (v == null)
		{
			tssSdtUlong.SetValue(1uL);
		}
		else
		{
			ulong num = v.GetValue();
			num += 1uL;
			tssSdtUlong.SetValue(num);
		}
		return tssSdtUlong;
	}

	public static TssSdtUlong operator --(TssSdtUlong v)
	{
		TssSdtUlong tssSdtUlong = new TssSdtUlong();
		if (v == null)
		{
			ulong num = 0uL;
			num -= 1uL;
			tssSdtUlong.SetValue(num);
		}
		else
		{
			ulong num2 = v.GetValue();
			num2 -= 1uL;
			tssSdtUlong.SetValue(num2);
		}
		return tssSdtUlong;
	}
}
