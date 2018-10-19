using System;

public struct OutOfControl
{
	public bool m_isOutOfControl;

	public OutOfControlType m_outOfControlType;

	public OutOfControl(bool isOutOfControl = false, OutOfControlType outOfControlType = OutOfControlType.Null)
	{
		this.m_isOutOfControl = isOutOfControl;
		this.m_outOfControlType = outOfControlType;
	}

	public void ResetOnUse()
	{
		this.m_isOutOfControl = false;
		this.m_outOfControlType = OutOfControlType.Null;
	}

	public bool Equals(OutOfControl other)
	{
		return this.m_isOutOfControl == other.m_isOutOfControl && this.m_outOfControlType == other.m_outOfControlType;
	}

	public override bool Equals(object obj)
	{
		return obj != null && base.GetType() == obj.GetType() && this.Equals((OutOfControl)obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
