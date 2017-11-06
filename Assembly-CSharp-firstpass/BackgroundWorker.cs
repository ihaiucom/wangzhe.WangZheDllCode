using System;
using System.Threading;

public class BackgroundWorker : Singleton<BackgroundWorker>
{
	public delegate void BackgroudDelegate();

	private Thread WorkingThread;

	private bool bRequestExit;

	private ListView<BackgroundWorker.BackgroudDelegate> PendingWork = new ListView<BackgroundWorker.BackgroudDelegate>();

	private ListView<BackgroundWorker.BackgroudDelegate> WorkingList = new ListView<BackgroundWorker.BackgroudDelegate>();

	public int ThreadID;

	public override void Init()
	{
		this.WorkingThread = new Thread(new ThreadStart(BackgroundWorker.StaticEntry));
		this.ThreadID = this.WorkingThread.get_ManagedThreadId();
		this.WorkingThread.Start();
	}

	public override void UnInit()
	{
		this.bRequestExit = true;
		this.WorkingThread.Join();
		this.WorkingThread = null;
	}

	protected static void StaticEntry()
	{
		Singleton<BackgroundWorker>.instance.Entry();
	}

	private static void Swap<T>(ref T a, ref T b)
	{
		T t = a;
		a = b;
		b = t;
	}

	protected void Entry()
	{
		while (!this.bRequestExit)
		{
			ListView<BackgroundWorker.BackgroudDelegate> pendingWork = this.PendingWork;
			ListView<BackgroundWorker.BackgroudDelegate> listView = pendingWork;
			lock (listView)
			{
				BackgroundWorker.Swap<ListView<BackgroundWorker.BackgroudDelegate>>(ref this.PendingWork, ref this.WorkingList);
			}
			int count = this.WorkingList.Count;
			for (int i = 0; i < count; i++)
			{
				try
				{
					this.WorkingList[i]();
				}
				catch (Exception var_4_5A)
				{
				}
			}
			this.WorkingList.Clear();
			Thread.Sleep(60);
		}
	}

	public void AddBackgroudOperation(BackgroundWorker.BackgroudDelegate InDelegate)
	{
		ListView<BackgroundWorker.BackgroudDelegate> pendingWork = this.PendingWork;
		ListView<BackgroundWorker.BackgroudDelegate> listView = pendingWork;
		lock (listView)
		{
			this.PendingWork.Add(InDelegate);
		}
	}
}
