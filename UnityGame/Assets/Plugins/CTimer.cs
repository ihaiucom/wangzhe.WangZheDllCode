using System;

public class CTimer
{
	public delegate void OnTimeUpHandler(int timerSequence);

	public delegate void OnTimeUpWithParamsHandler(int timerSequence, stTimerParams timerParams);

	private CTimer.OnTimeUpHandler m_timeUpHandler;

	private CTimer.OnTimeUpWithParamsHandler m_timeUpWithParamsHandler;

	private int m_loop = 1;

	private int m_totalTime;

	private int m_currentTime;

	private bool m_isFinished;

	private bool m_isRunning;

	private int m_sequence;

	private stTimerParams m_timerParams;

	public int CurrentTime
	{
		get
		{
			return this.m_currentTime;
		}
	}

	public int TimerSeq
	{
		get
		{
			return this.m_sequence;
		}
	}

	public CTimer(int time, int loop, CTimer.OnTimeUpHandler timeUpHandler, int sequence)
	{
		if (loop == 0)
		{
			loop = -1;
		}
		this.m_totalTime = time;
		this.m_loop = loop;
		this.m_timeUpHandler = timeUpHandler;
		this.m_sequence = sequence;
		this.m_currentTime = 0;
		this.m_isRunning = true;
		this.m_isFinished = false;
	}

	public CTimer(int time, int loop, CTimer.OnTimeUpWithParamsHandler timeUpWithParamsHandler, int sequence, stTimerParams timerParams)
	{
		if (loop == 0)
		{
			loop = -1;
		}
		this.m_totalTime = time;
		this.m_loop = loop;
		this.m_timeUpWithParamsHandler = timeUpWithParamsHandler;
		this.m_sequence = sequence;
		this.m_timerParams = timerParams;
		this.m_currentTime = 0;
		this.m_isRunning = true;
		this.m_isFinished = false;
	}

	public void Update(int deltaTime)
	{
		if (this.m_isFinished || !this.m_isRunning)
		{
			return;
		}
		if (this.m_loop == 0)
		{
			this.m_isFinished = true;
		}
		else
		{
			this.m_currentTime += deltaTime;
			if (this.m_currentTime >= this.m_totalTime)
			{
				if (this.m_timeUpHandler != null)
				{
					this.m_timeUpHandler(this.m_sequence);
				}
				if (this.m_timeUpWithParamsHandler != null)
				{
					this.m_timeUpWithParamsHandler(this.m_sequence, this.m_timerParams);
				}
				this.m_currentTime = 0;
				this.m_loop--;
			}
		}
	}

	public int GetLeftTime()
	{
		return this.m_totalTime - this.m_currentTime;
	}

	public void Finish()
	{
		this.m_isFinished = true;
	}

	public bool IsFinished()
	{
		return this.m_isFinished;
	}

	public void Pause()
	{
		this.m_isRunning = false;
	}

	public void Resume()
	{
		this.m_isRunning = true;
	}

	public void Reset()
	{
		this.m_currentTime = 0;
	}

	public void ResetTotalTime(int totalTime)
	{
		if (this.m_totalTime == totalTime)
		{
			return;
		}
		this.m_currentTime = 0;
		this.m_totalTime = totalTime;
	}

	public bool IsSequenceMatched(int sequence)
	{
		return this.m_sequence == sequence;
	}

	public bool IsDelegateMatched(CTimer.OnTimeUpHandler timeUpHandler)
	{
		return this.m_timeUpHandler == timeUpHandler;
	}

	public bool IsDelegateWithParamsMatched(CTimer.OnTimeUpWithParamsHandler timeUpWithParamsHandler)
	{
		return this.m_timeUpWithParamsHandler == timeUpWithParamsHandler;
	}

	public stTimerParams GetTimerParams()
	{
		return this.m_timerParams;
	}
}
