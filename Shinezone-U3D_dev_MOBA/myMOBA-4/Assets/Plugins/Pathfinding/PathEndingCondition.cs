using System;

namespace Pathfinding
{
	public class PathEndingCondition
	{
		protected Path p;

		protected PathEndingCondition()
		{
		}

		public PathEndingCondition(Path p)
		{
			if (p == null)
			{
				throw new ArgumentNullException("Please supply a non-null path");
			}
			this.p = p;
		}

		public virtual bool TargetFound(PathNode node)
		{
			return true;
		}
	}
}
