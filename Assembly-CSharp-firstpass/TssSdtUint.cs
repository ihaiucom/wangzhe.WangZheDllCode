using System;

public class TssSdtUint
{
	private TssSdtUintSlot m_slot;

	public static TssSdtUint NewTssSdtUint()
	{
		return new TssSdtUint
		{
			m_slot = TssSdtUintSlot.NewSlot(null)
		};
	}

	private uint GetValue()
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtUintSlot.NewSlot(null);
		}
		return this.m_slot.GetValue();
	}

	private void SetValue(uint v)
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtUintSlot.NewSlot(null);
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

	public static implicit operator uint(TssSdtUint v)
	{
		if (v == null)
		{
			return 0u;
		}
		return v.GetValue();
	}

	public static implicit operator TssSdtUint(uint v)
	{
		TssSdtUint tssSdtUint = new TssSdtUint();
		tssSdtUint.SetValue(v);
		return tssSdtUint;
	}

	public static bool operator ==(TssSdtUint a, TssSdtUint b)
	{
		return (object.Equals(a, null) && object.Equals(b, null)) || (!object.Equals(a, null) && !object.Equals(b, null) && a.GetValue() == b.GetValue());
	}

	public static bool operator !=(TssSdtUint a, TssSdtUint b)
	{
		return (!object.Equals(a, null) || !object.Equals(b, null)) && (object.Equals(a, null) || object.Equals(b, null) || a.GetValue() != b.GetValue());
	}

	public static TssSdtUint operator ++(TssSdtUint v)
	{
		TssSdtUint tssSdtUint = new TssSdtUint();
		if (v == null)
		{
			tssSdtUint.SetValue(1u);
		}
		else
		{
			uint num = v.GetValue();
			num += 1u;
			tssSdtUint.SetValue(num);
		}
		return tssSdtUint;
	}

	public static TssSdtUint operator --(TssSdtUint v)
	{
		TssSdtUint tssSdtUint = new TssSdtUint();
		if (v == null)
		{
			uint num = 0u;
			num -= 1u;
			tssSdtUint.SetValue(num);
		}
		else
		{
			uint num2 = v.GetValue();
			num2 -= 1u;
			tssSdtUint.SetValue(num2);
		}
		return tssSdtUint;
	}
}
