using System;
using System.Text;
using UnityEngine;

public class ActorsStatistic
{
	public static int frameCount;

	public static bool sampleActors;

	public static long _heroCount;

	public static long _organCount;

	public static long _towerCount;

	public static long _soldierCount;

	public static void Reset()
	{
		ActorsStatistic.frameCount = 0;
		ActorsStatistic._heroCount = 0L;
		ActorsStatistic._organCount = 0L;
		ActorsStatistic._towerCount = 0L;
		ActorsStatistic._soldierCount = 0L;
	}

	public static void Start()
	{
		ActorsStatistic.Reset();
		ActorsStatistic.sampleActors = true;
	}

	public static void Stop()
	{
		ActorsStatistic.sampleActors = false;
	}

	public static void AddSample(int heroCount, int organCount, int towerCount, int soldierCount)
	{
		if (!ActorsStatistic.sampleActors)
		{
			return;
		}
		ActorsStatistic.frameCount++;
		ActorsStatistic._heroCount += (long)heroCount;
		ActorsStatistic._organCount += (long)organCount;
		ActorsStatistic._towerCount += (long)towerCount;
		ActorsStatistic._soldierCount += (long)soldierCount;
	}

	public static void Draw()
	{
		double num = (ActorsStatistic.frameCount == 0) ? 0.0 : (1.0 / (double)ActorsStatistic.frameCount);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<size=24><color=red>");
		stringBuilder.AppendLine("Actors Statistics:");
		stringBuilder.AppendLine(string.Format("Hero:    {0:F2}", num * (double)ActorsStatistic._heroCount));
		stringBuilder.AppendLine(string.Format("Organ:   {0:F2}", num * (double)ActorsStatistic._organCount));
		stringBuilder.AppendLine(string.Format("Tower:   {0:F2}", num * (double)ActorsStatistic._towerCount));
		stringBuilder.AppendLine(string.Format("Soldier: {0:F2}", num * (double)ActorsStatistic._soldierCount));
		stringBuilder.Append("</color></size>");
		GUI.Label(new Rect((float)(Screen.width / 2), 20f, (float)Screen.width, (float)Screen.height), stringBuilder.ToString());
	}
}
