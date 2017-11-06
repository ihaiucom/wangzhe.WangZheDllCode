using System;

namespace Assets.Scripts.GameSystem
{
	public class RollingInfo
	{
		public int priority;

		public int resetPriorityTime;

		public byte repeatCount;

		public bool isShowing;

		public string content;

		public string url;

		public RollingInfo()
		{
			this.priority = 0;
			this.resetPriorityTime = 0;
			this.repeatCount = 0;
			this.isShowing = false;
			this.content = string.Empty;
			this.url = string.Empty;
		}
	}
}
