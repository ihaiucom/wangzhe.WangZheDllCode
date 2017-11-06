using System;
using System.Threading;

public abstract class WorkerThreadBase<T> : Singleton<T> where T : class, new()
{
	private object m_lock = new object();

	private bool m_isWorkDataPrepared = true;

	private bool m_isWorkFinished = true;

	public void PrepareWorkerData()
	{
		Monitor.Enter(this.m_lock);
		try
		{
			this._PrepareWorkerData();
		}
		catch (Exception var_0_16)
		{
		}
		this.m_isWorkDataPrepared = true;
		Monitor.PulseAll(this.m_lock);
		Monitor.Exit(this.m_lock);
	}

	public void WaitWorkerThread()
	{
		Monitor.Enter(this.m_lock);
		while (!this.m_isWorkFinished)
		{
			Monitor.PulseAll(this.m_lock);
			Monitor.Wait(this.m_lock);
		}
		this.m_isWorkFinished = false;
		Monitor.PulseAll(this.m_lock);
		Monitor.Exit(this.m_lock);
	}

	public void Start()
	{
		this.BeforeStart();
		Thread thread = new Thread(new ThreadStart(this._DoStart));
		thread.Start();
	}

	private void _DoStart()
	{
		while (true)
		{
			Monitor.Enter(this.m_lock);
			while (!this.m_isWorkDataPrepared)
			{
				Monitor.PulseAll(this.m_lock);
				Monitor.Wait(this.m_lock);
			}
			try
			{
				this._Run();
			}
			catch (Exception var_0_3D)
			{
			}
			this.m_isWorkFinished = true;
			this.m_isWorkDataPrepared = false;
			Monitor.PulseAll(this.m_lock);
			Monitor.Exit(this.m_lock);
		}
	}

	protected void GetLock()
	{
		Monitor.Enter(this.m_lock);
	}

	protected void ReleaseLock()
	{
		Monitor.PulseAll(this.m_lock);
		Monitor.Exit(this.m_lock);
	}

	protected abstract void _Run();

	protected abstract void _PrepareWorkerData();

	protected virtual void BeforeStart()
	{
	}
}
