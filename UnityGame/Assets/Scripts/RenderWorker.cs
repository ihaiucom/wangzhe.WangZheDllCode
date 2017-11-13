using System;

public class RenderWorker : WorkerThreadBase<RenderWorker>
{
	public void PreBeginLevel()
	{
		base.GetLock();
		try
		{
			FogOfWar.PreBeginLevel();
		}
		catch (Exception var_0_10)
		{
		}
		finally
		{
			base.ReleaseLock();
		}
	}

	public void BeginLevel()
	{
		base.GetLock();
		try
		{
			FogOfWar.BeginLevel();
		}
		catch (Exception var_0_10)
		{
		}
		finally
		{
			base.ReleaseLock();
		}
	}

	public void EndLevel()
	{
		base.GetLock();
		try
		{
			FogOfWar.EndLevel();
		}
		catch (Exception var_0_10)
		{
		}
		finally
		{
			base.ReleaseLock();
		}
	}

	protected override void BeforeStart()
	{
		base.BeforeStart();
	}

	protected override void _Run()
	{
		FogOfWar.Run();
	}

	protected override void _PrepareWorkerData()
	{
		FogOfWar.PrepareData();
	}
}
