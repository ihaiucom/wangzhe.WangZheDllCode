using System;
using System.Collections.Generic;
using UnityEngine;

public class CTimerManager : Singleton<CTimerManager>
{
	private enum enTimerType
	{
		Normal,
		FrameSync
	}

	private List<CTimer>[] m_timers;

	private int m_timerSequence;

	public override void Init()
	{
		this.m_timers = new List<CTimer>[Enum.GetValues(typeof(CTimerManager.enTimerType)).Length];
		for (int i = 0; i < this.m_timers.Length; i++)
		{
			this.m_timers[i] = new List<CTimer>();
		}
		this.m_timerSequence = 0;
	}

	public void Update()
	{
		this.UpdateTimer((int)(Time.deltaTime * 1000f), CTimerManager.enTimerType.Normal);
	}

	public void UpdateLogic(int delta)
	{
		this.UpdateTimer(delta, CTimerManager.enTimerType.FrameSync);
	}

	private void UpdateTimer(int delta, CTimerManager.enTimerType timerType)
	{
		List<CTimer> list = this.m_timers[(int)timerType];
		int i = 0;
		while (i < list.Count)
		{
			if (list[i].IsFinished())
			{
				list.RemoveAt(i);
			}
			else
			{
				list[i].Update(delta);
				i++;
			}
		}
	}

	public int AddTimer(int time, int loop, CTimer.OnTimeUpHandler onTimeUpHandler)
	{
		return this.AddTimer(time, loop, onTimeUpHandler, false);
	}

	public int AddTimer(int time, int loop, CTimer.OnTimeUpWithParamsHandler onTimeUpWithParamsHandler, stTimerParams timerParams)
	{
		return this.AddTimer(time, loop, onTimeUpWithParamsHandler, false, timerParams);
	}

	public int AddTimer(int time, int loop, CTimer.OnTimeUpHandler onTimeUpHandler, bool useFrameSync)
	{
		this.m_timerSequence++;
		this.m_timers[(!useFrameSync) ? 0 : 1].Add(new CTimer(time, loop, onTimeUpHandler, this.m_timerSequence));
		return this.m_timerSequence;
	}

	public int AddTimer(int time, int loop, CTimer.OnTimeUpWithParamsHandler onTimeUpWithParamsHandler, bool useFrameSync, stTimerParams timerParams)
	{
		this.m_timerSequence++;
		this.m_timers[(!useFrameSync) ? 0 : 1].Add(new CTimer(time, loop, onTimeUpWithParamsHandler, this.m_timerSequence, timerParams));
		return this.m_timerSequence;
	}

	public void RemoveTimer(int sequence)
	{
		for (int i = 0; i < this.m_timers.Length; i++)
		{
			List<CTimer> list = this.m_timers[i];
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].IsSequenceMatched(sequence))
				{
					list[j].Finish();
					return;
				}
			}
		}
	}

	public void RemoveTimerSafely(ref int sequence)
	{
		if (sequence != 0)
		{
			this.RemoveTimer(sequence);
			sequence = 0;
		}
	}

	public void PauseTimer(int sequence)
	{
		CTimer timer = this.GetTimer(sequence);
		if (timer != null)
		{
			timer.Pause();
		}
	}

	public void ResumeTimer(int sequence)
	{
		CTimer timer = this.GetTimer(sequence);
		if (timer != null)
		{
			timer.Resume();
		}
	}

	public void ResetTimer(int sequence)
	{
		CTimer timer = this.GetTimer(sequence);
		if (timer != null)
		{
			timer.Reset();
		}
	}

	public void ResetTimerTotalTime(int sequence, int totalTime)
	{
		CTimer timer = this.GetTimer(sequence);
		if (timer != null)
		{
			timer.ResetTotalTime(totalTime);
		}
	}

	public int GetTimerCurrent(int sequence)
	{
		CTimer timer = this.GetTimer(sequence);
		if (timer != null)
		{
			return timer.CurrentTime;
		}
		return -1;
	}

	public int GetLeftTime(int sequence)
	{
		CTimer timer = this.GetTimer(sequence);
		if (timer != null)
		{
			return timer.GetLeftTime() / 1000;
		}
		return -1;
	}

	public CTimer GetTimer(int sequence)
	{
		for (int i = 0; i < this.m_timers.Length; i++)
		{
			List<CTimer> list = this.m_timers[i];
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].IsSequenceMatched(sequence))
				{
					return list[j];
				}
			}
		}
		return null;
	}

	public void RemoveTimer(CTimer.OnTimeUpHandler onTimeUpHandler)
	{
		this.RemoveTimer(onTimeUpHandler, false);
	}

	public void RemoveTimer(CTimer.OnTimeUpWithParamsHandler onTimeUpWithParamsHandler)
	{
		this.RemoveTimer(onTimeUpWithParamsHandler, false);
	}

	public void RemoveTimer(CTimer.OnTimeUpHandler onTimeUpHandler, bool useFrameSync)
	{
		List<CTimer> list = this.m_timers[(!useFrameSync) ? 0 : 1];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].IsDelegateMatched(onTimeUpHandler))
			{
				list[i].Finish();
			}
		}
	}

	public void RemoveTimer(CTimer.OnTimeUpWithParamsHandler onTimeUpWithParamsHandler, bool useFrameSync)
	{
		List<CTimer> list = this.m_timers[(!useFrameSync) ? 0 : 1];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].IsDelegateWithParamsMatched(onTimeUpWithParamsHandler))
			{
				list[i].Finish();
			}
		}
	}

	public void RemoveAllTimer(bool useFrameSync)
	{
		this.m_timers[(!useFrameSync) ? 0 : 1].Clear();
	}

	public void RemoveAllTimer()
	{
		for (int i = 0; i < this.m_timers.Length; i++)
		{
			this.m_timers[i].Clear();
		}
	}
}
