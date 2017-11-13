using System;

namespace ExitGames.Client.DemoParticle
{
	public class TimeKeeper
	{
		private int lastExecutionTime = Environment.get_TickCount();

		private bool shouldExecute;

		public int Interval
		{
			get;
			set;
		}

		public bool IsEnabled
		{
			get;
			set;
		}

		public bool ShouldExecute
		{
			get
			{
				return this.IsEnabled && (this.shouldExecute || Environment.get_TickCount() - this.lastExecutionTime > this.Interval);
			}
			set
			{
				this.shouldExecute = value;
			}
		}

		public TimeKeeper(int interval)
		{
			this.IsEnabled = true;
			this.Interval = interval;
		}

		public void Reset()
		{
			this.shouldExecute = false;
			this.lastExecutionTime = Environment.get_TickCount();
		}
	}
}
