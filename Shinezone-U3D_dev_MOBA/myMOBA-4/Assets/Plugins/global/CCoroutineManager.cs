using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class CCoroutineManager : Singleton<CCoroutineManager>
{
	private DateTime m_lastFrameTime;

	private DateTime m_waitTillTime;

	private long m_lastTimeUpdateBeginTime;

	private LinkedList<CCoroutine> m_execlusiveCoroutineList = new LinkedList<CCoroutine>();

	private int m_frameRate;

	private double m_deltaMillSecondsPerFrame;

	private int sleepTime;

	private int WaitTime;

	private DateTime m_lastEvaluateTime = DateTime.Now;

	private bool m_evaluateStart;

	private double standardDeltaTime
	{
		get
		{
			return 33.333332061767578;
		}
	}

	public override void Init()
	{
		MonoSingleton<____CCorourtineManagerMonobehaviour>.GetInstance().s_smartCoroutine = this;
		this.m_lastFrameTime = DateTime.Now;
		this.m_waitTillTime = DateTime.Now;
		this.m_frameRate = 30;
		this.m_deltaMillSecondsPerFrame = this.standardDeltaTime * 1.0;
	}

	public void Update()
	{
		if (DateTime.Now <= this.m_waitTillTime)
		{
			return;
		}
		this.m_lastFrameTime = DateTime.Now;
		while ((DateTime.Now - this.m_lastFrameTime).TotalMilliseconds < this.m_deltaMillSecondsPerFrame && this.m_execlusiveCoroutineList.Count > 0)
		{
			LinkedListNode<CCoroutine> last = this.m_execlusiveCoroutineList.Last;
			IEnumerator iter = last.Value.iter;
			if (!iter.MoveNext())
			{
				this.m_execlusiveCoroutineList.Remove(last);
			}
			else
			{
				object current = iter.Current;
				if (current == null)
				{
					this.m_execlusiveCoroutineList.Remove(last);
				}
				else if (!(current is CCoroutine))
				{
					if (current is CHoldForSecond)
					{
						this.ProcessHoldForSeconds(current as CHoldForSecond);
					}
					else
					{
						if (current is CWaitForSecond)
						{
							this.ProcessWaitForSeconds(current as CWaitForSecond);
							break;
						}
						if (current is CWaitForNextFrame)
						{
							break;
						}
					}
				}
			}
		}
		this.m_lastFrameTime = DateTime.Now;
	}

	private void ReEvaluateTimeCost()
	{
		if (!this.m_evaluateStart)
		{
			this.m_lastEvaluateTime = DateTime.Now;
			this.m_evaluateStart = true;
			return;
		}
		DateTime now = DateTime.Now;
		double totalMilliseconds = (now - this.m_lastEvaluateTime).TotalMilliseconds;
		this.m_lastEvaluateTime = now;
		if (totalMilliseconds > this.standardDeltaTime)
		{
			this.m_deltaMillSecondsPerFrame = Math.Max(this.standardDeltaTime * 0.30000001192092896, this.m_deltaMillSecondsPerFrame - (totalMilliseconds - this.standardDeltaTime));
		}
		else if (this.m_deltaMillSecondsPerFrame < this.standardDeltaTime * 0.75)
		{
			this.m_deltaMillSecondsPerFrame += 1.0;
		}
	}

	private void ProcessHoldForSeconds(CHoldForSecond handle)
	{
		if (handle.m_interval > 0f)
		{
			this.sleepTime += (int)(handle.m_interval * 1000f);
			Thread.Sleep((int)(handle.m_interval * 1000f));
		}
	}

	private void ProcessWaitForSeconds(CWaitForSecond handle)
	{
		if (handle.m_interval > 0f)
		{
			this.WaitTime += (int)(handle.m_interval * 1000f);
			this.m_waitTillTime = DateTime.Now.AddSeconds((double)handle.m_interval);
		}
	}

	public CCoroutine StartCoroutine(IEnumerator coroutine)
	{
		CCoroutine cCoroutine = new CCoroutine(coroutine);
		this.m_execlusiveCoroutineList.AddLast(cCoroutine);
		return cCoroutine;
	}

	public void StopCoroutine(CCoroutine c, bool stopChild = true)
	{
		if (c != null)
		{
			bool flag = false;
			LinkedList<CCoroutine>.Enumerator enumerator = this.m_execlusiveCoroutineList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == c)
				{
					flag = true;
					break;
				}
			}
			if (!stopChild)
			{
				if (flag)
				{
					this.m_execlusiveCoroutineList.Remove(enumerator.Current);
				}
				return;
			}
			if (flag)
			{
				for (CCoroutine value = this.m_execlusiveCoroutineList.Last.Value; value != c; value = this.m_execlusiveCoroutineList.Last.Value)
				{
					this.m_execlusiveCoroutineList.RemoveLast();
				}
				this.m_execlusiveCoroutineList.RemoveLast();
			}
		}
	}
}
