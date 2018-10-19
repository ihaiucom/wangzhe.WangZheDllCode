using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;

public class SProfiler : MonoSingleton<SProfiler>
{
	private class STimer
	{
		public static double currentTime
		{
			get
			{
				return (double)Time.realtimeSinceStartup;
			}
		}
	}

	public enum SortType
	{
		Name,
		Time,
		Time2,
		Time_Diff,
		SelfTime,
		SelfTime2,
		SelfTime_Diff,
		AvgTime,
		AvgTime2,
		AvgTime_Diff,
		AvgSelfTime,
		AvgSelfTime2,
		AvgSelfTime_Diff,
		MaxTime,
		MaxTime2,
		MaxTime_Diff,
		MaxSelfTime,
		MaxSelfTime2,
		MaxSelfTime_Diff,
		Count,
		Count2,
		Count_Diff
	}

	private struct ColumnProp
	{
		public string name;

		public int width;

		public string data;

		public bool showInCompareMode;

		public int colorIndex;

		public bool hidden;
	}

	[Serializable]
	public class Sample
	{
		public double start;

		public double time;

		public double stackTime;

		public double stackTimeThisCall;

		public double profilerTime0;

		public double profilerTime1;

		public double profilerTimeThisCall0;

		public double profilerTimeThisCall1;

		public int count;

		public string name;

		public int identity;

		public double maxTime;

		public double maxSelfTime;

		public void begin()
		{
			this.stackTimeThisCall = 0.0;
			this.profilerTimeThisCall0 = 0.0;
			this.profilerTimeThisCall1 = 0.0;
			this.start = SProfiler.STimer.currentTime;
		}

		public double end()
		{
			double num = SProfiler.STimer.currentTime - this.start;
			this.time += num;
			this.maxTime = Math.Max(this.maxTime, num - this.profilerTimeThisCall0);
			this.maxSelfTime = Math.Max(this.maxSelfTime, num - this.stackTimeThisCall - this.profilerTimeThisCall1);
			this.count++;
			return num;
		}

		public void reset()
		{
			this.time = 0.0;
			this.start = 0.0;
			this.stackTime = 0.0;
			this.stackTimeThisCall = 0.0;
			this.profilerTime0 = 0.0;
			this.profilerTime1 = 0.0;
			this.profilerTimeThisCall0 = 0.0;
			this.profilerTimeThisCall1 = 0.0;
			this.count = 0;
			this.maxTime = 0.0;
			this.maxSelfTime = 0.0;
		}
	}

	public class Group
	{
		public string name;

		public int count;

		public double time;

		public double selfTime;

		public double timePercent;

		public double selfTimePercent;

		public double maxTime;

		public double maxSelfTime;

		public float timeToClean;

		public List<SProfiler.Sample> samples = new List<SProfiler.Sample>();

		public SProfiler.Group groupToCompare;

		public void flush(double totalTime)
		{
			double num = this.maxSelfTime;
			this.count = 0;
			this.time = 0.0;
			this.selfTime = 0.0;
			this.maxTime = 0.0;
			this.maxSelfTime = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < this.samples.Count; i++)
			{
				SProfiler.Sample sample = this.samples[i];
				num2 += sample.profilerTime0;
				num3 += sample.profilerTime1;
				this.time += sample.time;
				this.selfTime += sample.stackTime;
				this.count += sample.count;
				this.maxTime = Math.Max(this.maxTime, sample.maxTime);
				this.maxSelfTime = Math.Max(this.maxSelfTime, sample.maxSelfTime);
			}
			this.selfTime = this.time - this.selfTime;
			this.time -= num2;
			this.selfTime -= num3;
			this.timePercent = this.time * 100.0 / totalTime;
			this.selfTimePercent = this.selfTime * 100.0 / totalTime;
			this.time *= 1000.0;
			this.selfTime *= 1000.0;
			this.maxTime *= 1000.0;
			this.maxSelfTime *= 1000.0;
			if (this.maxSelfTime > SProfiler.MaxSelfTimeThreshold && this.maxSelfTime > num)
			{
				this.timeToClean = Time.realtimeSinceStartup + SProfiler.MaxSelfTimeDispearTime;
			}
		}

		public void checkClean()
		{
			if (Time.realtimeSinceStartup > this.timeToClean && this.timeToClean > 0f)
			{
				this.timeToClean = -1f;
				for (int i = 0; i < this.samples.Count; i++)
				{
					SProfiler.Sample sample = this.samples[i];
					sample.maxSelfTime = 0.0;
					sample.maxTime = 0.0;
				}
			}
		}

		public void AppendStr_Time(ref string str_time, bool showPercent)
		{
			str_time += this.time.ToString("F2");
			if (showPercent)
			{
				str_time += " (";
				str_time += this.timePercent.ToString("F2");
				str_time += "%)";
			}
			str_time += "\n";
		}

		public void AppendStr_SelfTime(ref string str_selfTime, bool showPercent)
		{
			str_selfTime += this.selfTime.ToString("F2");
			if (showPercent)
			{
				str_selfTime += " (";
				str_selfTime += this.selfTimePercent.ToString("F2");
				str_selfTime += "%)";
			}
			str_selfTime += "\n";
		}

		public void AppendStr_AvgTime(ref string str_avgTime)
		{
			double num = (this.count == 0) ? 0.0 : (this.time / (double)this.count);
			str_avgTime += num.ToString("F2");
			str_avgTime += "\n";
		}

		public void AppendStr_AvgSelfTime(ref string str_avgSelfTime)
		{
			double num = (this.count == 0) ? 0.0 : (this.selfTime / (double)this.count);
			str_avgSelfTime += num.ToString("F2");
			str_avgSelfTime += "\n";
		}

		public void AppendStr_MaxTime(ref string str_maxTime)
		{
			string str = this.maxTime.ToString("F2");
			if (this.maxTime > 5.0)
			{
				str_maxTime += "<color=red>";
				str_maxTime += str;
				str_maxTime += "</color>";
			}
			else
			{
				str_maxTime += str;
			}
			str_maxTime += "\n";
		}

		public void AppendStr_MaxSelfTime(ref string str_maxSelfTime)
		{
			string str = this.maxSelfTime.ToString("F2");
			if (this.maxSelfTime > 5.0)
			{
				str_maxSelfTime += "<color=red>";
				str_maxSelfTime += str;
				str_maxSelfTime += "</color>";
			}
			else
			{
				str_maxSelfTime += str;
			}
			str_maxSelfTime += "\n";
		}

		public void AppendStr_Count(ref string str_count)
		{
			str_count += this.count;
			str_count += "\n";
		}

		public void AppendStr_Time_Diff(ref string str_time, bool showPercent)
		{
			double num = this.groupToCompare.time - this.time;
			double num2 = this.groupToCompare.timePercent - this.timePercent;
			str_time += num.ToString("F2");
			str_time += "\n";
		}

		public void AppendStr_SelfTime_Diff(ref string str_selfTime, bool showPercent)
		{
			double num = this.groupToCompare.selfTime - this.selfTime;
			double num2 = this.groupToCompare.selfTimePercent - this.selfTimePercent;
			str_selfTime += num.ToString("F2");
			str_selfTime += "\n";
		}

		public void AppendStr_AvgTime_Diff(ref string str_avgTime)
		{
			double num = (this.count == 0) ? 0.0 : (this.time / (double)this.count);
			double num2 = (this.groupToCompare.count == 0) ? 0.0 : (this.groupToCompare.time / (double)this.groupToCompare.count);
			double num3 = num2 - num;
			str_avgTime += num3.ToString("F2");
			str_avgTime += "\n";
		}

		public void AppendStr_AvgSelfTime_Diff(ref string str_avgSelfTime)
		{
			double num = (this.count == 0) ? 0.0 : (this.selfTime / (double)this.count);
			double num2 = (this.groupToCompare.count == 0) ? 0.0 : (this.groupToCompare.selfTime / (double)this.groupToCompare.count);
			double num3 = num2 - num;
			str_avgSelfTime += num3.ToString("F2");
			str_avgSelfTime += "\n";
		}

		public void AppendStr_MaxTime_Diff(ref string str_maxTime)
		{
			double num = this.groupToCompare.maxTime - this.maxTime;
			str_maxTime += num.ToString("F2");
			str_maxTime += "\n";
		}

		public void AppendStr_MaxSelfTime_Diff(ref string str_maxSelfTime)
		{
			double num = this.groupToCompare.maxTime - this.maxSelfTime;
			str_maxSelfTime += this.maxSelfTime.ToString("F2");
			str_maxSelfTime += "\n";
		}

		public void AppendStr_Count_Diff(ref string str_count)
		{
			int num = this.groupToCompare.count - this.count;
			str_count += num;
			str_count += "\n";
		}
	}

	[Serializable]
	public class ProfileObj
	{
		private const float drawGroup_startX = 2f;

		private const float drawGroup_startY = 30f;

		private const float drawGroup_marginX = 2f;

		private const float drawGroup_buttonWidth = 88f;

		private const int MaxStackCount = 32;

		public Dictionary<int, SProfiler.Sample> samples = new Dictionary<int, SProfiler.Sample>();

		public Dictionary<long, int> idmap = new Dictionary<long, int>();

		public int idAllocator;

		[NonSerialized]
		public Dictionary<string, SProfiler.Group> groups = new Dictionary<string, SProfiler.Group>();

		[NonSerialized]
		public List<SProfiler.Group> groupList = new List<SProfiler.Group>();

		[NonSerialized]
		public SProfiler.Sample[] stacks = new SProfiler.Sample[32];

		[NonSerialized]
		public int stackCount;

		public double totalTime;

		public bool showPercent = true;

		[NonSerialized]
		private List<SProfiler.Group> groupListForSort = new List<SProfiler.Group>();

		private static SProfiler.ColumnProp[] column_props = new SProfiler.ColumnProp[]
		{
			new SProfiler.ColumnProp
			{
				name = "NAME",
				width = 260
			},
			new SProfiler.ColumnProp
			{
				name = "TIME",
				width = 140
			},
			new SProfiler.ColumnProp
			{
				name = "TIME2",
				width = 140,
				showInCompareMode = true,
				colorIndex = 1
			},
			new SProfiler.ColumnProp
			{
				name = "DIFF",
				width = 100,
				showInCompareMode = true,
				colorIndex = 2
			},
			new SProfiler.ColumnProp
			{
				name = "SELF TIME",
				width = 140
			},
			new SProfiler.ColumnProp
			{
				name = "SELF TIME 2",
				width = 140,
				showInCompareMode = true,
				colorIndex = 1
			},
			new SProfiler.ColumnProp
			{
				name = "DIFF",
				width = 100,
				showInCompareMode = true,
				colorIndex = 2
			},
			new SProfiler.ColumnProp
			{
				name = "AVG TIME",
				width = 80
			},
			new SProfiler.ColumnProp
			{
				name = "AVG TIME2",
				width = 80,
				showInCompareMode = true,
				colorIndex = 1
			},
			new SProfiler.ColumnProp
			{
				name = "AVG DIFF",
				width = 80,
				showInCompareMode = true,
				colorIndex = 2
			},
			new SProfiler.ColumnProp
			{
				name = "AVG SELF",
				width = 80
			},
			new SProfiler.ColumnProp
			{
				name = "AVG SELF 2",
				width = 80,
				showInCompareMode = true,
				colorIndex = 1
			},
			new SProfiler.ColumnProp
			{
				name = "DIFF",
				width = 80,
				showInCompareMode = true,
				colorIndex = 2
			},
			new SProfiler.ColumnProp
			{
				name = "MAX",
				width = 80
			},
			new SProfiler.ColumnProp
			{
				name = "MAX2",
				width = 80,
				showInCompareMode = true,
				colorIndex = 1
			},
			new SProfiler.ColumnProp
			{
				name = "DIFF",
				width = 80,
				showInCompareMode = true,
				colorIndex = 2
			},
			new SProfiler.ColumnProp
			{
				name = "MAX SELF",
				width = 80
			},
			new SProfiler.ColumnProp
			{
				name = "MAX SELF 2",
				width = 80,
				showInCompareMode = true,
				colorIndex = 1
			},
			new SProfiler.ColumnProp
			{
				name = "DIFF",
				width = 80,
				showInCompareMode = true,
				colorIndex = 2
			},
			new SProfiler.ColumnProp
			{
				name = "COUNT",
				width = 80
			},
			new SProfiler.ColumnProp
			{
				name = "COUNT2",
				width = 80,
				showInCompareMode = true,
				colorIndex = 1
			},
			new SProfiler.ColumnProp
			{
				name = "DIFF",
				width = 80,
				showInCompareMode = true,
				colorIndex = 2
			}
		};

		private static Color[] column_colors = new Color[]
		{
			Color.green,
			Color.cyan,
			Color.yellow
		};

		public SProfiler.SortType curSortType = SProfiler.SortType.Time;

		public bool highToLow = true;

		[NonSerialized]
		public bool showProfileFiles;

		[NonSerialized]
		public bool compareMode;

		[NonSerialized]
		private SProfiler.ProfileObj compareObj;

		[NonSerialized]
		public string[] profileFiles;

		public void PostLoad()
		{
			if (this.groups == null)
			{
				this.groups = new Dictionary<string, SProfiler.Group>();
			}
			if (this.groupList == null)
			{
				this.groupList = new List<SProfiler.Group>();
			}
			if (this.stacks == null)
			{
				this.stacks = new SProfiler.Sample[32];
			}
			foreach (SProfiler.Sample current in this.samples.Values)
			{
				SProfiler.Group group = null;
				if (!this.groups.TryGetValue(current.name, out group))
				{
					group = new SProfiler.Group();
					group.name = current.name;
					this.groups.Add(current.name, group);
					this.groupList.Add(group);
				}
				group.samples.Add(current);
			}
		}

		public void Begin(string name)
		{
			if (string.IsNullOrEmpty(name) || SProfiler.paused || !SProfiler.showGUI)
			{
				return;
			}
			double num = SProfiler.STimer.currentTime;
			int identifity = this.getIdentifity(name);
			SProfiler.Sample sample = null;
			if (!this.samples.TryGetValue(identifity, out sample))
			{
				sample = new SProfiler.Sample();
				sample.name = name;
				sample.identity = identifity;
				this.samples.Add(sample.identity, sample);
				SProfiler.Group group = null;
				if (!this.groups.TryGetValue(name, out group))
				{
					group = new SProfiler.Group();
					group.name = name;
					this.groups.Add(name, group);
					this.groupList.Add(group);
				}
				group.samples.Add(sample);
			}
			if (this.stackCount >= this.stacks.Length)
			{
				SProfiler.Sample[] destinationArray = new SProfiler.Sample[this.stacks.Length * 2];
				Array.Copy(this.stacks, destinationArray, this.stacks.Length);
				this.stacks = destinationArray;
			}
			this.stacks[this.stackCount++] = sample;
			sample.begin();
			if (this.stackCount > 1)
			{
				num = sample.start - num;
				SProfiler.Sample sample2 = this.stacks[this.stackCount - 2];
				sample2.profilerTime1 += num;
				sample2.profilerTimeThisCall1 += num;
				for (int i = 0; i < this.stackCount - 1; i++)
				{
					sample2 = this.stacks[i];
					sample2.profilerTime0 += num;
					sample2.profilerTimeThisCall0 += num;
				}
				this.totalTime -= num;
			}
		}

		public void End()
		{
			if (this.stackCount == 0 || SProfiler.paused || !SProfiler.showGUI)
			{
				return;
			}
			SProfiler.Sample sample = this.stacks[this.stackCount - 1];
			this.stacks[this.stackCount--] = null;
			double num = sample.end();
			if (this.stackCount > 0)
			{
				SProfiler.Sample sample2 = this.stacks[this.stackCount - 1];
				sample2.stackTime += num;
				sample2.stackTimeThisCall += num;
			}
			else
			{
				this.totalTime += num;
			}
		}

		public int getIdentifity(string name)
		{
			long num = (long)name.GetHashCode();
			if (this.stackCount > 0)
			{
				long num2 = (long)this.stacks[this.stackCount - 1].identity;
				num |= num2 << 32;
			}
			int num3 = 0;
			if (!this.idmap.TryGetValue(num, out num3))
			{
				num3 = ++this.idAllocator;
				this.idmap.Add(num, num3);
			}
			return num3;
		}

		public void Cleanup()
		{
			for (int i = 0; i < this.stacks.Length; i++)
			{
				this.stacks[i] = null;
			}
			this.stackCount = 0;
			this.samples.Clear();
			this.groups.Clear();
			this.groupList.Clear();
			this.totalTime = 0.0;
			this.compareMode = false;
			this.compareObj = null;
			SProfiler.profileFrameCount = 0;
			this.curSortType = SProfiler.SortType.Time;
			this.highToLow = true;
		}

		public void Reset()
		{
			Dictionary<int, SProfiler.Sample>.Enumerator enumerator = this.samples.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, SProfiler.Sample> current = enumerator.Current;
				SProfiler.Sample value = current.Value;
				value.reset();
			}
			this.totalTime = 0.0;
			this.compareMode = false;
			this.compareObj = null;
			SProfiler.profileFrameCount = 0;
		}

		private static void LinkGroupList(SProfiler.ProfileObj obj1, SProfiler.ProfileObj obj2)
		{
			int count = obj1.groupList.Count;
			for (int i = 0; i < count; i++)
			{
				SProfiler.Group group = obj1.groupList[i];
				SProfiler.Group group2 = null;
				if (group.groupToCompare == null)
				{
					if (!obj2.groups.TryGetValue(group.name, out group2))
					{
						group2 = new SProfiler.Group();
						group2.name = group.name;
						obj2.groups.Add(group2.name, group2);
						obj2.groupList.Add(group2);
					}
					group2.groupToCompare = group;
					group.groupToCompare = group2;
				}
			}
		}

		public void SortGroups()
		{
			for (int i = 0; i < this.groupList.Count; i++)
			{
				SProfiler.Group group = this.groupList[i];
				group.flush(this.totalTime);
			}
			if (this.compareMode)
			{
				for (int j = 0; j < this.compareObj.groupList.Count; j++)
				{
					SProfiler.Group group2 = this.compareObj.groupList[j];
					group2.flush(this.compareObj.totalTime);
				}
				SProfiler.ProfileObj.LinkGroupList(this, this.compareObj);
				SProfiler.ProfileObj.LinkGroupList(this.compareObj, this);
			}
			int le = (!this.highToLow) ? -1 : 1;
			int ge = (!this.highToLow) ? 1 : -1;
			switch (this.curSortType)
			{
			case SProfiler.SortType.Name:
				if (this.highToLow)
				{
					this.groupList.Sort((SProfiler.Group x, SProfiler.Group y) => -string.Compare(x.name, y.name));
				}
				else
				{
					this.groupList.Sort((SProfiler.Group x, SProfiler.Group y) => string.Compare(x.name, y.name));
				}
				break;
			case SProfiler.SortType.Time:
				this.groupList.Sort(delegate(SProfiler.Group x, SProfiler.Group y)
				{
					if (x.time < y.time)
					{
						return le;
					}
					if (x.time == y.time)
					{
						return 0;
					}
					return ge;
				});
				break;
			case SProfiler.SortType.Time_Diff:
				this.groupList.Sort(delegate(SProfiler.Group x, SProfiler.Group y)
				{
					double num = x.time - x.groupToCompare.time;
					double num2 = y.time - y.groupToCompare.time;
					if (num < num2)
					{
						return le;
					}
					if (num == num2)
					{
						return 0;
					}
					return ge;
				});
				break;
			case SProfiler.SortType.SelfTime:
				this.groupList.Sort(delegate(SProfiler.Group x, SProfiler.Group y)
				{
					if (x.selfTime < y.selfTime)
					{
						return le;
					}
					if (x.selfTime == y.selfTime)
					{
						return 0;
					}
					return ge;
				});
				break;
			case SProfiler.SortType.SelfTime_Diff:
				this.groupList.Sort(delegate(SProfiler.Group x, SProfiler.Group y)
				{
					double num = x.selfTime - x.groupToCompare.selfTime;
					double num2 = y.selfTime - y.groupToCompare.selfTime;
					if (num < num2)
					{
						return le;
					}
					if (num == num2)
					{
						return 0;
					}
					return ge;
				});
				break;
			case SProfiler.SortType.AvgTime:
				this.groupList.Sort(delegate(SProfiler.Group x, SProfiler.Group y)
				{
					double num = (x.count == 0) ? 0.0 : (x.time / (double)x.count);
					double num2 = (y.count == 0) ? 0.0 : (y.time / (double)y.count);
					if (num < num2)
					{
						return le;
					}
					if (num == num2)
					{
						return 0;
					}
					return ge;
				});
				break;
			case SProfiler.SortType.AvgTime_Diff:
				this.groupList.Sort(delegate(SProfiler.Group x, SProfiler.Group y)
				{
					double num = (x.count == 0) ? 0.0 : (x.time / (double)x.count);
					double num2 = (y.count == 0) ? 0.0 : (y.time / (double)y.count);
					double num3 = (x.groupToCompare.count == 0) ? 0.0 : (x.groupToCompare.time / (double)x.groupToCompare.count);
					double num4 = (y.groupToCompare.count == 0) ? 0.0 : (y.groupToCompare.time / (double)y.groupToCompare.count);
					double num5 = num - num3;
					double num6 = num2 - num4;
					if (num5 < num6)
					{
						return le;
					}
					if (num5 == num6)
					{
						return 0;
					}
					return ge;
				});
				break;
			case SProfiler.SortType.AvgSelfTime:
				this.groupList.Sort(delegate(SProfiler.Group x, SProfiler.Group y)
				{
					double num = (x.count == 0) ? 0.0 : (x.selfTime / (double)x.count);
					double num2 = (y.count == 0) ? 0.0 : (y.selfTime / (double)y.count);
					if (num < num2)
					{
						return le;
					}
					if (num == num2)
					{
						return 0;
					}
					return ge;
				});
				break;
			case SProfiler.SortType.AvgSelfTime_Diff:
				this.groupList.Sort(delegate(SProfiler.Group x, SProfiler.Group y)
				{
					double num = (x.count == 0) ? 0.0 : (x.selfTime / (double)x.count);
					double num2 = (y.count == 0) ? 0.0 : (y.selfTime / (double)y.count);
					double num3 = (x.groupToCompare.count == 0) ? 0.0 : (x.groupToCompare.selfTime / (double)x.groupToCompare.count);
					double num4 = (y.groupToCompare.count == 0) ? 0.0 : (y.groupToCompare.selfTime / (double)y.groupToCompare.count);
					double num5 = num - num3;
					double num6 = num2 - num4;
					if (num5 < num6)
					{
						return le;
					}
					if (num5 == num6)
					{
						return 0;
					}
					return ge;
				});
				break;
			case SProfiler.SortType.MaxTime:
				this.groupList.Sort(delegate(SProfiler.Group x, SProfiler.Group y)
				{
					if (x.maxTime < y.maxTime)
					{
						return le;
					}
					if (x.maxTime == y.maxTime)
					{
						return 0;
					}
					return ge;
				});
				break;
			case SProfiler.SortType.MaxTime_Diff:
				this.groupList.Sort(delegate(SProfiler.Group x, SProfiler.Group y)
				{
					double num = x.maxTime - x.groupToCompare.maxTime;
					double num2 = y.maxTime - y.groupToCompare.maxTime;
					if (num < num2)
					{
						return le;
					}
					if (num == num2)
					{
						return 0;
					}
					return ge;
				});
				break;
			case SProfiler.SortType.MaxSelfTime:
				this.groupList.Sort(delegate(SProfiler.Group x, SProfiler.Group y)
				{
					if (x.maxSelfTime < y.maxSelfTime)
					{
						return le;
					}
					if (x.maxSelfTime == y.maxSelfTime)
					{
						return 0;
					}
					return ge;
				});
				break;
			case SProfiler.SortType.MaxSelfTime_Diff:
				this.groupList.Sort(delegate(SProfiler.Group x, SProfiler.Group y)
				{
					double num = x.maxSelfTime - x.groupToCompare.maxSelfTime;
					double num2 = y.maxSelfTime - y.groupToCompare.maxSelfTime;
					if (num < num2)
					{
						return le;
					}
					if (num == num2)
					{
						return 0;
					}
					return ge;
				});
				break;
			case SProfiler.SortType.Count:
				this.groupList.Sort(delegate(SProfiler.Group x, SProfiler.Group y)
				{
					if (x.count < y.count)
					{
						return le;
					}
					if (x.count == y.count)
					{
						return 0;
					}
					return ge;
				});
				break;
			case SProfiler.SortType.Count_Diff:
				this.groupList.Sort(delegate(SProfiler.Group x, SProfiler.Group y)
				{
					int num = x.count - x.groupToCompare.count;
					int num2 = y.count - y.groupToCompare.count;
					if (num < num2)
					{
						return le;
					}
					if (num == num2)
					{
						return 0;
					}
					return ge;
				});
				break;
			}
		}

		public void DrawGroups()
		{
			if (!this.showProfileFiles)
			{
				if (SProfiler.showData)
				{
					this.SortGroups();
					this.BuildGroupColumns();
					this.DrawGroupColumns();
				}
				this.DrawButtons();
			}
			else
			{
				this.DrawProfileFiles();
			}
		}

		private void BuildGroupColumns()
		{
			string text = string.Empty;
			string empty = string.Empty;
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			string empty4 = string.Empty;
			string empty5 = string.Empty;
			string empty6 = string.Empty;
			string empty7 = string.Empty;
			string empty8 = string.Empty;
			string empty9 = string.Empty;
			string empty10 = string.Empty;
			string empty11 = string.Empty;
			string empty12 = string.Empty;
			string empty13 = string.Empty;
			string empty14 = string.Empty;
			string empty15 = string.Empty;
			string empty16 = string.Empty;
			string empty17 = string.Empty;
			string empty18 = string.Empty;
			string empty19 = string.Empty;
			string empty20 = string.Empty;
			string empty21 = string.Empty;
			for (int i = 0; i < this.groupList.Count; i++)
			{
				SProfiler.Group group = this.groupList[i];
				SProfiler.Group groupToCompare = group.groupToCompare;
				text += group.name;
				text += "\n";
				if (SProfiler.showColomn_Time)
				{
					group.AppendStr_Time(ref empty, this.showPercent);
					if (this.compareMode)
					{
						groupToCompare.AppendStr_Time(ref empty8, this.showPercent);
						group.AppendStr_Time_Diff(ref empty15, this.showPercent);
					}
				}
				if (SProfiler.showColomn_SelfTime)
				{
					group.AppendStr_SelfTime(ref empty2, this.showPercent);
					if (this.compareMode)
					{
						groupToCompare.AppendStr_SelfTime(ref empty9, this.showPercent);
						group.AppendStr_SelfTime_Diff(ref empty16, this.showPercent);
					}
				}
				if (SProfiler.showColomn_AvgTime)
				{
					group.AppendStr_AvgTime(ref empty3);
					if (this.compareMode)
					{
						groupToCompare.AppendStr_AvgTime(ref empty10);
						group.AppendStr_AvgTime_Diff(ref empty17);
					}
				}
				if (SProfiler.showColomn_AvgSelfTime)
				{
					group.AppendStr_AvgSelfTime(ref empty4);
					if (this.compareMode)
					{
						groupToCompare.AppendStr_AvgSelfTime(ref empty11);
						group.AppendStr_AvgSelfTime_Diff(ref empty18);
					}
				}
				if (SProfiler.showColomn_MaxTime)
				{
					group.AppendStr_MaxTime(ref empty5);
					if (this.compareMode)
					{
						groupToCompare.AppendStr_MaxTime(ref empty12);
						group.AppendStr_MaxTime_Diff(ref empty19);
					}
				}
				if (SProfiler.showColomn_MaxSelfTime)
				{
					group.AppendStr_MaxSelfTime(ref empty6);
					if (this.compareMode)
					{
						groupToCompare.AppendStr_MaxSelfTime(ref empty13);
						group.AppendStr_MaxSelfTime_Diff(ref empty20);
					}
				}
				if (SProfiler.showColomn_Count)
				{
					group.AppendStr_Count(ref empty7);
					if (this.compareMode)
					{
						groupToCompare.AppendStr_Count(ref empty14);
						group.AppendStr_Count_Diff(ref empty21);
					}
				}
			}
			SProfiler.ProfileObj.column_props[0].data = text;
			SProfiler.ProfileObj.column_props[1].data = empty;
			SProfiler.ProfileObj.column_props[4].data = empty2;
			SProfiler.ProfileObj.column_props[7].data = empty3;
			SProfiler.ProfileObj.column_props[10].data = empty4;
			SProfiler.ProfileObj.column_props[13].data = empty5;
			SProfiler.ProfileObj.column_props[16].data = empty6;
			SProfiler.ProfileObj.column_props[19].data = empty7;
			SProfiler.ProfileObj.column_props[2].data = empty8;
			SProfiler.ProfileObj.column_props[5].data = empty9;
			SProfiler.ProfileObj.column_props[8].data = empty10;
			SProfiler.ProfileObj.column_props[11].data = empty11;
			SProfiler.ProfileObj.column_props[14].data = empty12;
			SProfiler.ProfileObj.column_props[17].data = empty13;
			SProfiler.ProfileObj.column_props[20].data = empty14;
			SProfiler.ProfileObj.column_props[3].data = empty15;
			SProfiler.ProfileObj.column_props[6].data = empty16;
			SProfiler.ProfileObj.column_props[9].data = empty17;
			SProfiler.ProfileObj.column_props[12].data = empty18;
			SProfiler.ProfileObj.column_props[15].data = empty19;
			SProfiler.ProfileObj.column_props[18].data = empty20;
			SProfiler.ProfileObj.column_props[21].data = empty21;
		}

		private void DrawGroupColumns()
		{
			GUI.color = Color.green;
			float num = 92f;
			float num2 = 30f;
			float height = (float)Screen.height - num2;
			for (int i = 0; i < SProfiler.ProfileObj.column_props.Length; i++)
			{
				SProfiler.ColumnProp columnProp = SProfiler.ProfileObj.column_props[i];
				if (!columnProp.showInCompareMode || this.compareMode)
				{
					if (!this.compareMode || !columnProp.hidden)
					{
						if (this.compareMode && columnProp.colorIndex != -1)
						{
							GUI.color = SProfiler.ProfileObj.column_colors[columnProp.colorIndex];
						}
						else
						{
							GUI.color = Color.green;
						}
						GUI.Box(new Rect(num, num2, (float)columnProp.width, height), string.Empty);
						GUI.Label(new Rect(num + 4f, num2, (float)(columnProp.width - 8), height), columnProp.data);
						if (GUI.Button(new Rect(num, 0f, (float)columnProp.width, num2), columnProp.name))
						{
							if (this.curSortType == (SProfiler.SortType)i)
							{
								this.highToLow = !this.highToLow;
							}
							else
							{
								this.curSortType = (SProfiler.SortType)i;
							}
						}
						num += (float)columnProp.width + 2f;
					}
				}
			}
		}

		private void DrawButtons()
		{
			float num = 26f;
			float num2 = 4f;
			float num3 = 0f;
			float left = 2f;
			GUI.color = Color.green;
			if (GUI.Button(new Rect(left, num3, 88f, num), "MODE"))
			{
				SProfiler.showMaxOnly = !SProfiler.showMaxOnly;
				this.Reset();
			}
			num3 += num + num2;
			if (GUI.Button(new Rect(left, num3, 88f, num), "RESET"))
			{
				this.Cleanup();
			}
			num3 += num + num2;
			Color color = GUI.color;
			GUI.color = ((!SProfiler.paused) ? color : Color.red);
			if (GUI.Button(new Rect(left, num3, 88f, num), "PAUSE"))
			{
				SProfiler.paused = !SProfiler.paused;
			}
			num3 += num + num2;
			GUI.color = color;
			GUI.color = ((!SProfiler.showData) ? Color.red : color);
			if (GUI.Button(new Rect(left, num3, 88f, num), "SHOW"))
			{
				SProfiler.showData = !SProfiler.showData;
			}
			num3 += num + num2;
			GUI.color = color;
			GUI.color = ((!SProfiler.disableGameInput) ? color : Color.red);
			if (GUI.Button(new Rect(left, num3, 88f, num), "EVENT SYS"))
			{
				SProfiler.disableGameInput = !SProfiler.disableGameInput;
				SProfiler.EnableEventSystem(!SProfiler.disableGameInput);
			}
			num3 += num + num2;
			GUI.color = color;
			if (GUI.Button(new Rect(left, num3, 88f, num), "SHOW %"))
			{
				this.showPercent = !this.showPercent;
			}
			num3 += num + num2;
			if (GUI.Button(new Rect(left, num3, 88f, num), "SAVE"))
			{
				this.SaveProfileData();
			}
			num3 += num + num2;
			if (GUI.Button(new Rect(left, num3, 88f, num), "LOAD"))
			{
				this.RefreshProfileFiles();
				this.showProfileFiles = true;
			}
			num3 += num + num2;
			if (GUI.Button(new Rect(left, num3, 88f, num), "COMPARE"))
			{
				this.RefreshProfileFiles();
				this.showProfileFiles = true;
				this.compareMode = true;
			}
			num3 += num + num2;
			GUI.color = ((!SProfiler.showFPSStatistic) ? Color.red : color);
			if (GUI.Button(new Rect(left, num3, 88f, num), "FPS_SHOW"))
			{
				SProfiler.showFPSStatistic = !SProfiler.showFPSStatistic;
			}
			num3 += num + num2;
			GUI.color = ((!FPSStatistic.sampleFps) ? Color.red : color);
			if (GUI.Button(new Rect(left, num3, 88f, num), "FPS_PAUSE"))
			{
				FPSStatistic.sampleFps = !FPSStatistic.sampleFps;
				ActorsStatistic.sampleActors = !ActorsStatistic.sampleActors;
			}
			num3 += num + num2;
			GUI.color = color;
			if (GUI.Button(new Rect(left, num3, 88f, num), "FPS_RESET"))
			{
				FPSStatistic.Reset();
				ActorsStatistic.Reset();
			}
			num3 += num + num2;
			if (this.compareMode)
			{
				num3 += num + num2;
				GUI.color = ((!SProfiler.showColomn_Time) ? Color.red : color);
				if (GUI.Button(new Rect(left, num3, 88f, num), "[TIME]"))
				{
					SProfiler.showColomn_Time = !SProfiler.showColomn_Time;
					SProfiler.ProfileObj.column_props[1].hidden = (SProfiler.ProfileObj.column_props[2].hidden = (SProfiler.ProfileObj.column_props[3].hidden = !SProfiler.showColomn_Time));
				}
				num3 += num + num2;
				GUI.color = ((!SProfiler.showColomn_SelfTime) ? Color.red : color);
				if (GUI.Button(new Rect(left, num3, 88f, num), "[SELF TIME]"))
				{
					SProfiler.showColomn_SelfTime = !SProfiler.showColomn_SelfTime;
					SProfiler.ProfileObj.column_props[4].hidden = (SProfiler.ProfileObj.column_props[5].hidden = (SProfiler.ProfileObj.column_props[6].hidden = !SProfiler.showColomn_SelfTime));
				}
				num3 += num + num2;
				GUI.color = ((!SProfiler.showColomn_AvgTime) ? Color.red : color);
				if (GUI.Button(new Rect(left, num3, 88f, num), "[AVG TIME]"))
				{
					SProfiler.showColomn_AvgTime = !SProfiler.showColomn_AvgTime;
					SProfiler.ProfileObj.column_props[7].hidden = (SProfiler.ProfileObj.column_props[8].hidden = (SProfiler.ProfileObj.column_props[9].hidden = !SProfiler.showColomn_AvgTime));
				}
				num3 += num + num2;
				GUI.color = ((!SProfiler.showColomn_AvgSelfTime) ? Color.red : color);
				if (GUI.Button(new Rect(left, num3, 88f, num), "[AVG SELF]"))
				{
					SProfiler.showColomn_AvgSelfTime = !SProfiler.showColomn_AvgSelfTime;
					SProfiler.ProfileObj.column_props[10].hidden = (SProfiler.ProfileObj.column_props[11].hidden = (SProfiler.ProfileObj.column_props[12].hidden = !SProfiler.showColomn_AvgSelfTime));
				}
				num3 += num + num2;
				GUI.color = ((!SProfiler.showColomn_MaxTime) ? Color.red : color);
				if (GUI.Button(new Rect(left, num3, 88f, num), "[MAX TIME]"))
				{
					SProfiler.showColomn_MaxTime = !SProfiler.showColomn_MaxTime;
					SProfiler.ProfileObj.column_props[13].hidden = (SProfiler.ProfileObj.column_props[14].hidden = (SProfiler.ProfileObj.column_props[15].hidden = !SProfiler.showColomn_MaxTime));
				}
				num3 += num + num2;
				GUI.color = ((!SProfiler.showColomn_MaxSelfTime) ? Color.red : color);
				if (GUI.Button(new Rect(left, num3, 88f, num), "[MAX SELF]"))
				{
					SProfiler.showColomn_MaxSelfTime = !SProfiler.showColomn_MaxSelfTime;
					SProfiler.ProfileObj.column_props[16].hidden = (SProfiler.ProfileObj.column_props[17].hidden = (SProfiler.ProfileObj.column_props[18].hidden = !SProfiler.showColomn_MaxSelfTime));
				}
				num3 += num + num2;
				GUI.color = ((!SProfiler.showColomn_Count) ? Color.red : color);
				if (GUI.Button(new Rect(left, num3, 88f, num), "[COUNT]"))
				{
					SProfiler.showColomn_Count = !SProfiler.showColomn_Count;
					SProfiler.ProfileObj.column_props[19].hidden = (SProfiler.ProfileObj.column_props[20].hidden = (SProfiler.ProfileObj.column_props[21].hidden = !SProfiler.showColomn_Count));
				}
				num3 += num + num2;
			}
		}

		public void SaveProfileData()
		{
			string arg = DateTime.Now.ToString("yyyyMMdd_HHmmss");
			string path = string.Format("{0}/{1}.spf", DebugHelper.logRootPath, arg);
			this.SaveToFile(path);
		}

		private void DrawProfileFiles()
		{
			GUI.color = Color.white;
			float num = 0f;
			float num2 = 2f;
			float num3 = 30f;
			float width = 500f;
			float num4 = 4f;
			if (GUI.Button(new Rect(num2, num, 88f, num3), "返回"))
			{
				this.showProfileFiles = false;
				if (this.compareObj == null)
				{
					this.compareMode = false;
				}
			}
			num2 += 90f;
			if (GUI.Button(new Rect(num2, num, 88f, num3), "刷新"))
			{
				this.RefreshProfileFiles();
			}
			num2 = 2f;
			num += num3 + num4;
			if (this.profileFiles != null && this.profileFiles.Length > 0)
			{
				int num5 = -1;
				for (int i = 0; i < this.profileFiles.Length; i++)
				{
					string text = this.profileFiles[i];
					int num6 = Mathf.Max(text.LastIndexOf('/'), text.LastIndexOf('\\'));
					text = text.Substring(num6 + 1);
					if (GUI.Button(new Rect(num2, num, width, num3), text))
					{
						num5 = i;
					}
					num += num3 + num4;
				}
				if (num5 != -1)
				{
					this.showProfileFiles = false;
					SProfiler.ProfileObj profileObj = SProfiler.ProfileObj.LoadFromFile(this.profileFiles[num5]);
					if (this.compareMode)
					{
						if (profileObj == null)
						{
							this.compareMode = false;
						}
						this.compareObj = profileObj;
					}
					else
					{
						this.CopyFrom(profileObj);
					}
					SProfiler.Pause();
				}
			}
		}

		private void CopyFrom(SProfiler.ProfileObj src)
		{
			this.Cleanup();
			if (src == null)
			{
				return;
			}
			this.samples = src.samples;
			this.groups = src.groups;
			this.groupList = src.groupList;
			this.totalTime = src.totalTime;
			this.idmap = src.idmap;
			this.idAllocator = src.idAllocator;
		}

		private void RefreshProfileFiles()
		{
			this.profileFiles = null;
			string logRootPath = DebugHelper.logRootPath;
			if (!Directory.Exists(logRootPath))
			{
				return;
			}
			this.profileFiles = Directory.GetFiles(logRootPath, "*.spf", SearchOption.TopDirectoryOnly);
		}

		public void DrawGroups_MaxSelfTimeOnly()
		{
			this.groupListForSort.Clear();
			for (int i = 0; i < this.groupList.Count; i++)
			{
				SProfiler.Group group = this.groupList[i];
				group.flush(this.totalTime);
				group.checkClean();
				if (group.maxSelfTime > SProfiler.MaxSelfTimeThreshold)
				{
					this.groupListForSort.Add(group);
				}
			}
			string text = string.Empty;
			if (this.groupListForSort.Count > 0)
			{
				this.groupListForSort.Sort((SProfiler.Group g0, SProfiler.Group g1) => g0.timeToClean.CompareTo(g1.timeToClean));
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("<size=24>");
				for (int j = 0; j < this.groupListForSort.Count; j++)
				{
					SProfiler.Group group2 = this.groupListForSort[j];
					stringBuilder.Append(group2.name);
					stringBuilder.AppendFormat(":\t\t{0:F2}\n", group2.maxSelfTime);
				}
				stringBuilder.Append("</size>");
				text = stringBuilder.ToString();
			}
			float num = 20f;
			float num2 = 30f;
			float height = (float)Screen.height - num2;
			GUI.color = Color.green;
			float num3 = 80f;
			if (GUI.Button(new Rect(num, 0f, num3, num2), "RESET"))
			{
				this.Cleanup();
			}
			num += num3 + 10f;
			if (GUI.Button(new Rect(num, 0f, num3, num2), "MODE"))
			{
				SProfiler.showMaxOnly = !SProfiler.showMaxOnly;
				this.Cleanup();
			}
			num += num3 + 10f;
			GUI.color = ((!SProfiler.paused) ? GUI.color : Color.red);
			if (GUI.Button(new Rect(num, 0f, num3, num2), "PAUSE"))
			{
				SProfiler.paused = !SProfiler.paused;
			}
			GUI.color = Color.yellow;
			GUI.Label(new Rect(0f, num2, (float)Screen.width, height), text);
			GUI.color = Color.white;
		}

		public void SaveToFile(string path)
		{
			FileStream fileStream = null;
			try
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
				binaryFormatter.Serialize(fileStream, this);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream.Dispose();
					fileStream = null;
				}
			}
		}

		public static SProfiler.ProfileObj LoadFromFile(string path)
		{
			SProfiler.ProfileObj profileObj = null;
			FileStream fileStream = null;
			try
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
				profileObj = (binaryFormatter.Deserialize(fileStream) as SProfiler.ProfileObj);
				if (profileObj != null)
				{
					profileObj.PostLoad();
				}
			}
			catch (Exception exception)
			{
				profileObj = null;
				Debug.LogException(exception);
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream.Dispose();
					fileStream = null;
				}
			}
			return profileObj;
		}
	}

	public static SProfiler.ProfileObj Obj = new SProfiler.ProfileObj();

	private static bool showMaxOnly = false;

	private static bool requestReset = false;

	private static int requestPause = 0;

	private static bool paused = false;

	private static bool deferStartup = false;

	private static double MaxSelfTimeThreshold = 15.0;

	private static float MaxSelfTimeDispearTime = 6f;

	private static bool showData = false;

	private static bool showFPSStatistic = false;

	private static bool disableGameInput = false;

	private static bool showColomn_Time = true;

	private static bool showColomn_SelfTime = true;

	private static bool showColomn_AvgTime = true;

	private static bool showColomn_AvgSelfTime = true;

	private static bool showColomn_MaxTime = true;

	private static bool showColomn_MaxSelfTime = true;

	private static bool showColomn_Count = true;

	public static int profileFrameCount = 0;

	public static long startFrameIndex = 0L;

	private static EventSystem gui_eventSystem;

	private static uint _curFrameIndex = 0u;

	public static uint StartFrameIndex = 0u;

	public static bool showGUI
	{
		get;
		private set;
	}

	public static uint CurFrameIndex
	{
		get
		{
			return SProfiler._curFrameIndex;
		}
		set
		{
			SProfiler._curFrameIndex = value;
			if (SProfiler.deferStartup && SProfiler.StartFrameIndex > 0u && SProfiler._curFrameIndex == SProfiler.StartFrameIndex)
			{
				SProfiler.deferStartup = false;
				SProfiler.StartProfile();
			}
		}
	}

	[Conditional("SGAME_PROFILE")]
	public static void BeginSample(string name)
	{
		SProfiler.Obj.Begin(name);
	}

	[Conditional("SGAME_PROFILE")]
	public static void EndSample()
	{
		SProfiler.Obj.End();
	}

	public static void Cleanup()
	{
		SProfiler.Obj.Cleanup();
	}

	public static void Pause()
	{
		SProfiler.paused = true;
	}

	public static void Reset()
	{
		SProfiler.Obj.Reset();
	}

	private void OnGUI()
	{
		if (!SProfiler.showGUI)
		{
			return;
		}
		if (SProfiler.showMaxOnly)
		{
			SProfiler.Obj.DrawGroups_MaxSelfTimeOnly();
		}
		else
		{
			SProfiler.Obj.DrawGroups();
		}
		if (SProfiler.showFPSStatistic)
		{
			FPSStatistic.Draw();
			ActorsStatistic.Draw();
		}
	}

	private static void EnableEventSystem(bool enable)
	{
		if (SProfiler.gui_eventSystem == null)
		{
			SProfiler.gui_eventSystem = EventSystem.current;
		}
		if (SProfiler.gui_eventSystem != null)
		{
			SProfiler.gui_eventSystem.enabled = enable;
		}
	}

	public void ToggleVisible()
	{
		this.ShowGUI(!SProfiler.showGUI);
	}

	private static void StartProfile()
	{
		SProfiler.paused = false;
		SProfiler.showGUI = true;
		SProfiler.showFPSStatistic = false;
		SProfiler.showData = false;
		SProfiler.StartFrameIndex = SProfiler.CurFrameIndex;
		FPSStatistic.Start();
		ActorsStatistic.Start();
	}

	public void StartProfileNFrames(int frameCount, uint startIndex = 0u)
	{
		SProfiler.Cleanup();
		SProfiler.profileFrameCount = frameCount;
		if (startIndex > 0u)
		{
			if (SProfiler.CurFrameIndex > startIndex)
			{
				startIndex *= SProfiler.CurFrameIndex / startIndex + 1u;
			}
			SProfiler.StartFrameIndex = startIndex;
			SProfiler.deferStartup = true;
			SProfiler.paused = true;
		}
		else
		{
			SProfiler.StartFrameIndex = SProfiler.CurFrameIndex;
			SProfiler.deferStartup = false;
			SProfiler.StartProfile();
		}
	}

	public void ShowGUI(bool show)
	{
		if (SProfiler.showGUI == show)
		{
			return;
		}
		SProfiler.showGUI = show;
		if (SProfiler.showGUI)
		{
			SProfiler.Cleanup();
			SProfiler.paused = false;
		}
		SProfiler.deferStartup = false;
	}

	public void RequestReset()
	{
		SProfiler.requestReset = true;
	}

	public void RequestPause(bool pause)
	{
		SProfiler.requestPause = ((!pause) ? -1 : 1);
	}

	private void LateUpdate()
	{
		if (SProfiler.requestReset)
		{
			SProfiler.requestReset = false;
			SProfiler.Reset();
		}
		if (SProfiler.requestPause != 0)
		{
			SProfiler.paused = (SProfiler.requestPause == 1);
			SProfiler.requestPause = 0;
		}
		if (SProfiler.profileFrameCount > 0 && !SProfiler.paused && (ulong)(SProfiler.CurFrameIndex - SProfiler.StartFrameIndex) >= (ulong)((long)SProfiler.profileFrameCount))
		{
			SProfiler.profileFrameCount = 0;
			FPSStatistic.sampleFps = false;
			ActorsStatistic.sampleActors = false;
			SProfiler.showFPSStatistic = true;
			SProfiler.showData = true;
			SProfiler.paused = true;
			SProfiler.Obj.SaveProfileData();
		}
	}
}
