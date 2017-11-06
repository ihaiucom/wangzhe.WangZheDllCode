using System;

namespace Pathfinding
{
	public class QuadtreeNode : GraphNode
	{
		public GraphNode[] connections;

		public uint[] connectionCosts;

		public QuadtreeNode(AstarPath astar) : base(astar)
		{
		}

		public void SetPosition(VInt3 value)
		{
			this.position = value;
		}

		public override void GetConnections(GraphNodeDelegate del)
		{
			if (this.connections == null)
			{
				return;
			}
			for (int i = 0; i < this.connections.Length; i++)
			{
				del(this.connections[i]);
			}
		}

		public override void AddConnection(GraphNode node, uint cost)
		{
			throw new NotImplementedException("QuadTree Nodes do not have support for adding manual connections");
		}

		public override void RemoveConnection(GraphNode node)
		{
			throw new NotImplementedException("QuadTree Nodes do not have support for adding manual connections");
		}

		public override void ClearConnections(bool alsoReverse)
		{
			if (alsoReverse)
			{
				for (int i = 0; i < this.connections.Length; i++)
				{
					this.connections[i].RemoveConnection(this);
				}
			}
			this.connections = null;
			this.connectionCosts = null;
		}

		public override void Open(Path path, PathNode pathNode, PathHandler handler)
		{
			if (this.connections == null)
			{
				return;
			}
			for (int i = 0; i < this.connections.Length; i++)
			{
				GraphNode graphNode = this.connections[i];
				if (path.CanTraverse(graphNode))
				{
					PathNode pathNode2 = handler.GetPathNode(graphNode);
					if (pathNode2.pathID != handler.PathID)
					{
						pathNode2.node = graphNode;
						pathNode2.parent = pathNode;
						pathNode2.pathID = handler.PathID;
						pathNode2.cost = this.connectionCosts[i];
						pathNode2.H = path.CalculateHScore(graphNode);
						graphNode.UpdateG(path, pathNode2);
						handler.PushNode(pathNode2);
					}
					else
					{
						uint num = this.connectionCosts[i];
						if (pathNode.G + num + path.GetTraversalCost(graphNode) < pathNode2.G)
						{
							pathNode2.cost = num;
							pathNode2.parent = pathNode;
							graphNode.UpdateRecursiveG(path, pathNode2, handler);
						}
						else if (pathNode2.G + num + path.GetTraversalCost(this) < pathNode.G && graphNode.ContainsConnection(this))
						{
							pathNode.parent = pathNode2;
							pathNode.cost = num;
							this.UpdateRecursiveG(path, pathNode, handler);
						}
					}
				}
			}
		}
	}
}
