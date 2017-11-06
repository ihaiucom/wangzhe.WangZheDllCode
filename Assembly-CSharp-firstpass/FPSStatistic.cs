using System;
using System.Text;
using UnityEngine;

public class FPSStatistic
{
	public class Distribute
	{
		public float MinValue;

		public string name;

		public int count;
	}

	public static int count;

	public static float MinFps;

	public static float MaxFps;

	private static double total_;

	public static FPSStatistic.Distribute[] distributes;

	public static bool sampleFps;

	public static double profiler_lastTime;

	public static float profiler_fps;

	public static float avgFps
	{
		get
		{
			return (float)(FPSStatistic.total_ / (double)FPSStatistic.count);
		}
	}

	static FPSStatistic()
	{
		FPSStatistic.count = 0;
		FPSStatistic.MinFps = 9999f;
		FPSStatistic.MaxFps = -9999f;
		FPSStatistic.total_ = 0.0;
		FPSStatistic.distributes = new FPSStatistic.Distribute[]
		{
			new FPSStatistic.Distribute
			{
				MinValue = 55f
			},
			new FPSStatistic.Distribute
			{
				MinValue = 50f
			},
			new FPSStatistic.Distribute
			{
				MinValue = 45f
			},
			new FPSStatistic.Distribute
			{
				MinValue = 40f
			},
			new FPSStatistic.Distribute
			{
				MinValue = 35f
			},
			new FPSStatistic.Distribute
			{
				MinValue = 30f
			},
			new FPSStatistic.Distribute
			{
				MinValue = 25f
			},
			new FPSStatistic.Distribute
			{
				MinValue = 20f
			},
			new FPSStatistic.Distribute
			{
				MinValue = 15f
			},
			new FPSStatistic.Distribute
			{
				MinValue = 10f
			},
			new FPSStatistic.Distribute
			{
				MinValue = 5f
			},
			new FPSStatistic.Distribute()
		};
		FPSStatistic.sampleFps = true;
		FPSStatistic.profiler_lastTime = -10.0;
		FPSStatistic.profiler_fps = 0f;
		for (int i = 0; i < FPSStatistic.distributes.Length; i++)
		{
			if (i == 0)
			{
				FPSStatistic.distributes[i].name = ">= " + FPSStatistic.distributes[i].MinValue;
			}
			else
			{
				FPSStatistic.distributes[i].name = FPSStatistic.distributes[i].MinValue + "-" + FPSStatistic.distributes[i - 1].MinValue;
			}
		}
	}

	public static void Reset()
	{
		FPSStatistic.count = 0;
		FPSStatistic.MinFps = 9999f;
		FPSStatistic.MaxFps = -9999f;
		FPSStatistic.total_ = 0.0;
		for (int i = 0; i < FPSStatistic.distributes.Length; i++)
		{
			FPSStatistic.distributes[i].count = 0;
		}
		FPSStatistic.profiler_lastTime = -10.0;
		FPSStatistic.profiler_fps = 0f;
	}

	public static void Start()
	{
		FPSStatistic.Reset();
		FPSStatistic.sampleFps = true;
	}

	public static void Stop()
	{
		FPSStatistic.sampleFps = false;
	}

	private static void AddSample(float fps)
	{
		FPSStatistic.total_ += (double)fps;
		FPSStatistic.count++;
		if (FPSStatistic.MinFps > fps)
		{
			FPSStatistic.MinFps = fps;
		}
		if (FPSStatistic.MaxFps < fps)
		{
			FPSStatistic.MaxFps = fps;
		}
		for (int i = 0; i < FPSStatistic.distributes.Length; i++)
		{
			FPSStatistic.Distribute distribute = FPSStatistic.distributes[i];
			if (fps >= distribute.MinValue)
			{
				distribute.count++;
				break;
			}
		}
	}

	public static void Update()
	{
		if (!FPSStatistic.sampleFps)
		{
			return;
		}
		double num = (double)Time.realtimeSinceStartup;
		if (FPSStatistic.profiler_lastTime < 0.0)
		{
			FPSStatistic.profiler_lastTime = num;
		}
		else
		{
			FPSStatistic.profiler_fps = 1f / (float)(num - FPSStatistic.profiler_lastTime);
			FPSStatistic.profiler_lastTime = num;
		}
		FPSStatistic.AddSample(FPSStatistic.profiler_fps);
	}

	public static void Draw()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<size=24>");
		stringBuilder.AppendLine("FPS Statistics:");
		stringBuilder.AppendLine(string.Format("Frame: {0}", FPSStatistic.count));
		stringBuilder.AppendLine(string.Format("Avg:   {0:F2}", FPSStatistic.avgFps));
		stringBuilder.AppendLine(string.Format("Min:   {0:F2}", FPSStatistic.MinFps));
		stringBuilder.AppendLine(string.Format("Max:   {0:F2}", FPSStatistic.MaxFps));
		bool flag = false;
		for (int i = 0; i < FPSStatistic.distributes.Length; i++)
		{
			FPSStatistic.Distribute distribute = FPSStatistic.distributes[i];
			if (distribute.count != 0 || flag)
			{
				flag = true;
				stringBuilder.AppendLine(string.Format("{0}: {1} ({2:F2}%)", distribute.name, distribute.count, (double)distribute.count * 100.0 / (double)FPSStatistic.count));
			}
		}
		stringBuilder.Append("</size>");
		Color color = GUI.color;
		GUI.color = Color.red;
		GUI.Label(new Rect(20f, 20f, (float)Screen.width, (float)Screen.height), stringBuilder.ToString());
		GUI.color = color;
	}
}
