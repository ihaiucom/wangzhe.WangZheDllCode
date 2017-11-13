using System;

namespace Pathfinding
{
	public struct GraphHitInfo
	{
		public VInt3 origin;

		public VInt3 point;

		public GraphNode node;

		public VInt3 tangentOrigin;

		public VInt3 tangent;

		public float distance
		{
			get
			{
				return (float)(this.point - this.origin).magnitude;
			}
		}

		public GraphHitInfo(VInt3 point)
		{
			this.tangentOrigin = VInt3.zero;
			this.origin = VInt3.zero;
			this.point = point;
			this.node = null;
			this.tangent = VInt3.zero;
		}
	}
}
