using System;
using UnityEngine;

public class NcTimerTool
{
	protected bool m_bEnable;

	private float m_fLastEngineTime;

	private float m_fCurrentTime;

	private float m_fLastTime;

	private float m_fTimeScale = 1f;

	private int m_nSmoothCount = 10;

	private int m_nSmoothIndex;

	private float m_fSmoothRate = 1.3f;

	private float[] m_fSmoothTimes;

	private float m_fLastSmoothDeltaTime;

	public static float GetEngineTime()
	{
		if (Time.time == 0f)
		{
			return 1E-06f;
		}
		return Time.time;
	}

	public static float GetEngineDeltaTime()
	{
		return Time.deltaTime;
	}

	private void InitSmoothTime()
	{
		if (this.m_fSmoothTimes == null)
		{
			this.m_fSmoothTimes = new float[this.m_nSmoothCount];
			for (int i = 0; i < this.m_nSmoothCount; i++)
			{
				this.m_fSmoothTimes[i] = Time.deltaTime;
			}
			this.m_fLastSmoothDeltaTime = Time.deltaTime;
		}
	}

	private float UpdateSmoothTime(float fDeltaTime)
	{
		this.m_fSmoothTimes[this.m_nSmoothIndex++] = Mathf.Min(fDeltaTime, this.m_fLastSmoothDeltaTime * this.m_fSmoothRate);
		if (this.m_nSmoothCount <= this.m_nSmoothIndex)
		{
			this.m_nSmoothIndex = 0;
		}
		this.m_fLastSmoothDeltaTime = 0f;
		for (int i = 0; i < this.m_nSmoothCount; i++)
		{
			this.m_fLastSmoothDeltaTime += this.m_fSmoothTimes[i];
		}
		this.m_fLastSmoothDeltaTime /= (float)this.m_nSmoothCount;
		return this.m_fLastSmoothDeltaTime;
	}

	public bool IsUpdateTimer()
	{
		return this.m_fLastEngineTime != NcTimerTool.GetEngineTime();
	}

	private float UpdateTimer()
	{
		if (this.m_bEnable)
		{
			if (this.m_fLastEngineTime != NcTimerTool.GetEngineTime())
			{
				this.m_fLastTime = this.m_fCurrentTime;
				this.m_fCurrentTime += (NcTimerTool.GetEngineTime() - this.m_fLastEngineTime) * this.GetTimeScale();
				this.m_fLastEngineTime = NcTimerTool.GetEngineTime();
				if (this.m_fSmoothTimes != null)
				{
					this.UpdateSmoothTime(this.m_fCurrentTime - this.m_fLastTime);
				}
			}
		}
		else
		{
			this.m_fLastEngineTime = NcTimerTool.GetEngineTime();
		}
		return this.m_fCurrentTime;
	}

	public float GetTime()
	{
		return this.UpdateTimer();
	}

	public float GetDeltaTime()
	{
		if (!this.m_bEnable)
		{
			return 0f;
		}
		if (Time.timeScale == 0f)
		{
			return 0f;
		}
		this.UpdateTimer();
		return this.m_fCurrentTime - this.m_fLastTime;
	}

	public float GetSmoothDeltaTime()
	{
		if (!this.m_bEnable)
		{
			return 0f;
		}
		if (Time.timeScale == 0f)
		{
			return 0f;
		}
		if (this.m_fSmoothTimes == null)
		{
			this.InitSmoothTime();
		}
		this.UpdateTimer();
		return this.m_fLastSmoothDeltaTime;
	}

	public bool IsEnable()
	{
		return this.m_bEnable;
	}

	public void Start()
	{
		this.m_bEnable = true;
		this.m_fCurrentTime = 0f;
		this.m_fLastEngineTime = NcTimerTool.GetEngineTime() - 1E-06f;
		this.UpdateTimer();
	}

	public void Reset(float fElapsedTime)
	{
		this.m_fCurrentTime = fElapsedTime;
		this.m_fLastEngineTime = NcTimerTool.GetEngineTime() - 1E-06f;
		this.UpdateTimer();
	}

	public void Pause()
	{
		this.UpdateTimer();
		this.m_bEnable = false;
		this.UpdateTimer();
	}

	public void Resume()
	{
		this.UpdateTimer();
		this.m_bEnable = true;
		this.UpdateTimer();
	}

	public void SetTimeScale(float fTimeScale)
	{
		this.m_fTimeScale = fTimeScale;
	}

	protected virtual float GetTimeScale()
	{
		return this.m_fTimeScale;
	}
}
