using System;

namespace Pathfinding
{
	public struct PathThreadInfo
	{
		public int threadIndex;

		public AstarPath astar;

		public PathHandler runData;

		private object _lock;

		public object Lock
		{
			get
			{
				return this._lock;
			}
		}

		public PathThreadInfo(int index, AstarPath astar, PathHandler runData)
		{
			this.threadIndex = index;
			this.astar = astar;
			this.runData = runData;
			this._lock = new object();
		}
	}
}
