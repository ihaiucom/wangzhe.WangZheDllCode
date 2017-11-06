using System;

public class TssSdtFloat
{
	private TssSdtFloatSlot m_slot;

	public static TssSdtFloat NewTssSdtFloat()
	{
		return new TssSdtFloat
		{
			m_slot = TssSdtFloatSlot.NewSlot(null)
		};
	}

	private float GetValue()
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtFloatSlot.NewSlot(null);
		}
		return this.m_slot.GetValue();
	}

	private void SetValue(float v)
	{
		if (this.m_slot == null)
		{
			this.m_slot = TssSdtFloatSlot.NewSlot(null);
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

	public static implicit operator float(TssSdtFloat v)
	{
		if (v == null)
		{
			return 0f;
		}
		return v.GetValue();
	}

	public static implicit operator TssSdtFloat(float v)
	{
		TssSdtFloat tssSdtFloat = new TssSdtFloat();
		tssSdtFloat.SetValue(v);
		return tssSdtFloat;
	}

	public static bool operator ==(TssSdtFloat a, TssSdtFloat b)
	{
		return (object.Equals(a, null) && object.Equals(b, null)) || (!object.Equals(a, null) && !object.Equals(b, null) && a.GetValue() == b.GetValue());
	}

	public static bool operator !=(TssSdtFloat a, TssSdtFloat b)
	{
		return (!object.Equals(a, null) || !object.Equals(b, null)) && (object.Equals(a, null) || object.Equals(b, null) || a.GetValue() != b.GetValue());
	}

	public static TssSdtFloat operator ++(TssSdtFloat v)
	{
		TssSdtFloat tssSdtFloat = new TssSdtFloat();
		if (v == null)
		{
			tssSdtFloat.SetValue(1f);
		}
		else
		{
			float num = v.GetValue();
			num += 1f;
			tssSdtFloat.SetValue(num);
		}
		return tssSdtFloat;
	}

	public static TssSdtFloat operator --(TssSdtFloat v)
	{
		TssSdtFloat tssSdtFloat = new TssSdtFloat();
		if (v == null)
		{
			tssSdtFloat.SetValue(-1f);
		}
		else
		{
			float num = v.GetValue();
			num -= 1f;
			tssSdtFloat.SetValue(num);
		}
		return tssSdtFloat;
	}
}
