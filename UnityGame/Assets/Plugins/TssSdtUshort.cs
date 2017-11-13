using System;

public class TssSdtUshort
{
	private TssSdtUshortSlot m_slot;

	public static TssSdtUshort NewTssSdtUshort()
	{
		return new TssSdtUshort
		{
			m_slot = TssSdtUshortSlot.NewSlot(null)
		};
	}

	private ushort GetValue()
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtUshortSlot.NewSlot(null);
		}
		return this.m_slot.GetValue();
	}

	private void SetValue(ushort v)
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtUshortSlot.NewSlot(null);
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

	public static implicit operator ushort(TssSdtUshort v)
	{
		if (v == null)
		{
			return 0;
		}
		return v.GetValue();
	}

	public static implicit operator TssSdtUshort(ushort v)
	{
		TssSdtUshort tssSdtUshort = new TssSdtUshort();
		tssSdtUshort.SetValue(v);
		return tssSdtUshort;
	}

	public static bool operator ==(TssSdtUshort a, TssSdtUshort b)
	{
		return (object.Equals(a, null) && object.Equals(b, null)) || (!object.Equals(a, null) && !object.Equals(b, null) && a.GetValue() == b.GetValue());
	}

	public static bool operator !=(TssSdtUshort a, TssSdtUshort b)
	{
		return (!object.Equals(a, null) || !object.Equals(b, null)) && (object.Equals(a, null) || object.Equals(b, null) || a.GetValue() != b.GetValue());
	}

	public static TssSdtUshort operator ++(TssSdtUshort v)
	{
		TssSdtUshort tssSdtUshort = new TssSdtUshort();
		if (v == null)
		{
			tssSdtUshort.SetValue(1);
		}
		else
		{
			ushort num = v.GetValue();
			num += 1;
			tssSdtUshort.SetValue(num);
		}
		return tssSdtUshort;
	}

	public static TssSdtUshort operator --(TssSdtUshort v)
	{
		TssSdtUshort tssSdtUshort = new TssSdtUshort();
		if (v == null)
		{
			ushort num = 0;
			num -= 1;
			tssSdtUshort.SetValue(num);
		}
		else
		{
			ushort num2 = v.GetValue();
			num2 -= 1;
			tssSdtUshort.SetValue(num2);
		}
		return tssSdtUshort;
	}
}
