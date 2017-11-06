using System;

public class CWaitForSecond : CCoroutineYieldBase
{
	public float m_interval = -1f;

	public CWaitForSecond(float interval = -1f)
	{
		this.m_interval = interval;
	}
}
