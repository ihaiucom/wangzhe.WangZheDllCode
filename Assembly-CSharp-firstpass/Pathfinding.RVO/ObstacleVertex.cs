using System;

namespace Pathfinding.RVO
{
	public class ObstacleVertex
	{
		public bool ignore;

		public VInt3 position;

		public VInt2 dir;

		public VInt height;

		public RVOLayer layer;

		public bool convex;

		public bool split;

		public ObstacleVertex next;

		public ObstacleVertex prev;
	}
}
