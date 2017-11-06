using System;

public class CHoldForSecond : CCoroutineYieldBase
{
	public float m_interval = -1f;

	public CHoldForSecond(float interval = 0f)
	{
		this.m_interval = interval;
	}
}
