using System;

namespace Pathfinding
{
	public interface INavmesh
	{
		void GetNodes(GraphNodeDelegateCancelable del);
	}
}
