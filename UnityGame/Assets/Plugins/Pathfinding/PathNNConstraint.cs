using System;

namespace Pathfinding
{
	public class PathNNConstraint : NNConstraint
	{
		public new static PathNNConstraint Default
		{
			get
			{
				return new PathNNConstraint
				{
					constrainArea = true
				};
			}
		}

		public virtual void SetStart(GraphNode node)
		{
			if (node != null)
			{
				this.area = (int)node.Area;
			}
			else
			{
				this.constrainArea = false;
			}
		}
	}
}
