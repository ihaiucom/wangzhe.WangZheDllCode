using System;

public struct TimeStamp
{
	public long startTime;

	public long endTime;

	public TimeStamp(long startTime, long endTime)
	{
		this.startTime = startTime;
		this.endTime = endTime;
	}
}
