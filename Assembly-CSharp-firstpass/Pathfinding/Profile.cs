using System;
using System.Diagnostics;
using UnityEngine;

namespace Pathfinding
{
	public class Profile
	{
		private const bool PROFILE_MEM = false;

		public string name;

		private Stopwatch w;

		private int counter;

		private long mem;

		private long smem;

		private int control = 1073741824;

		private bool dontCountFirst;

		public Profile(string name)
		{
			this.name = name;
			this.w = new Stopwatch();
		}

		public int ControlValue()
		{
			return this.control;
		}

		[Conditional("PROFILE")]
		public void Start()
		{
			if (this.dontCountFirst && this.counter == 1)
			{
				return;
			}
			this.w.Start();
		}

		[Conditional("PROFILE")]
		public void Stop()
		{
			this.counter++;
			if (this.dontCountFirst && this.counter == 1)
			{
				return;
			}
			this.w.Stop();
		}

		[Conditional("PROFILE")]
		public void Log()
		{
			Debug.Log(this.ToString());
		}

		[Conditional("PROFILE")]
		public void ConsoleLog()
		{
			Console.WriteLine(this.ToString());
		}

		[Conditional("PROFILE")]
		public void Stop(int control)
		{
			this.counter++;
			if (this.dontCountFirst && this.counter == 1)
			{
				return;
			}
			this.w.Stop();
			if (this.control == 1073741824)
			{
				this.control = control;
			}
			else if (this.control != control)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Control numbers do not match ",
					this.control,
					" != ",
					control
				}));
			}
		}

		[Conditional("PROFILE")]
		public void Control(Profile other)
		{
			if (this.ControlValue() != other.ControlValue())
			{
				throw new Exception(string.Concat(new object[]
				{
					"Control numbers do not match (",
					this.name,
					" ",
					other.name,
					") ",
					this.ControlValue(),
					" != ",
					other.ControlValue()
				}));
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.name,
				" #",
				this.counter,
				" ",
				this.w.get_Elapsed().get_TotalMilliseconds().ToString("0.0 ms"),
				" avg: ",
				(this.w.get_Elapsed().get_TotalMilliseconds() / (double)this.counter).ToString("0.00 ms")
			});
		}
	}
}
