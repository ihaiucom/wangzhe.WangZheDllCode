using System;

public class TssSdtInt
{
	private TssSdtIntSlot m_slot;

	public static TssSdtInt NewTssSdtInt()
	{
		return new TssSdtInt
		{
			m_slot = TssSdtIntSlot.NewSlot(null)
		};
	}

	private int GetValue()
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtIntSlot.NewSlot(null);
		}
		return this.m_slot.GetValue();
	}

	private void SetValue(int v)
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtIntSlot.NewSlot(null);
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

	public static implicit operator int(TssSdtInt v)
	{
		if (v == null)
		{
			return 0;
		}
		return v.GetValue();
	}

	public static implicit operator TssSdtInt(int v)
	{
		TssSdtInt tssSdtInt = new TssSdtInt();
		tssSdtInt.SetValue(v);
		return tssSdtInt;
	}

	public static bool operator ==(TssSdtInt a, TssSdtInt b)
	{
		return (object.Equals(a, null) && object.Equals(b, null)) || (!object.Equals(a, null) && !object.Equals(b, null) && a.GetValue() == b.GetValue());
	}

	public static bool operator !=(TssSdtInt a, TssSdtInt b)
	{
		return (!object.Equals(a, null) || !object.Equals(b, null)) && (object.Equals(a, null) || object.Equals(b, null) || a.GetValue() != b.GetValue());
	}

	public static TssSdtInt operator ++(TssSdtInt v)
	{
		TssSdtInt tssSdtInt = new TssSdtInt();
		if (v == null)
		{
			tssSdtInt.SetValue(1);
		}
		else
		{
			int num = v.GetValue();
			num++;
			tssSdtInt.SetValue(num);
		}
		return tssSdtInt;
	}

	public static TssSdtInt operator --(TssSdtInt v)
	{
		TssSdtInt tssSdtInt = new TssSdtInt();
		if (v == null)
		{
			tssSdtInt.SetValue(-1);
		}
		else
		{
			int num = v.GetValue();
			num--;
			tssSdtInt.SetValue(num);
		}
		return tssSdtInt;
	}
}
