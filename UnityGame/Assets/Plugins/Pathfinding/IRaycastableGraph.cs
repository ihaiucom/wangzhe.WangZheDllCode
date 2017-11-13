using System;
using System.Collections.Generic;

namespace Pathfinding
{
	public interface IRaycastableGraph
	{
		bool Linecast(VInt3 start, VInt3 end);

		bool Linecast(VInt3 start, VInt3 end, GraphNode hint);

		bool Linecast(VInt3 start, VInt3 end, GraphNode hint, out GraphHitInfo hit);

		bool Linecast(VInt3 start, VInt3 end, GraphNode hint, out GraphHitInfo hit, List<GraphNode> trace);
	}
}
