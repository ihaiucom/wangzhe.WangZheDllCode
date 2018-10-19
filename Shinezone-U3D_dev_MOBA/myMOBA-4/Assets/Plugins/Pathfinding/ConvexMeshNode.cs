using System;
using UnityEngine;

namespace Pathfinding
{
	public class ConvexMeshNode : MeshNode
	{
		private int[] indices;

		protected static INavmeshHolder[] navmeshHolders;

		public ConvexMeshNode(AstarPath astar) : base(astar)
		{
			this.indices = new int[0];
		}

		static ConvexMeshNode()
		{
			ConvexMeshNode.navmeshHolders = new INavmeshHolder[0];
		}

		protected static INavmeshHolder GetNavmeshHolder(uint graphIndex)
		{
			return ConvexMeshNode.navmeshHolders[(int)graphIndex];
		}

		public void SetPosition(VInt3 p)
		{
			this.position = p;
		}

		public int GetVertexIndex(int i)
		{
			return this.indices[i];
		}

		public override VInt3 GetVertex(int i)
		{
			return ConvexMeshNode.GetNavmeshHolder(base.GraphIndex).GetVertex(this.GetVertexIndex(i));
		}

		public override int GetVertexCount()
		{
			return this.indices.Length;
		}

		public override Vector3 ClosestPointOnNode(Vector3 p)
		{
			throw new NotImplementedException();
		}

		public override Vector3 ClosestPointOnNodeXZ(Vector3 p)
		{
			throw new NotImplementedException();
		}

		public override VInt3 ClosestPointOnNodeXZ(VInt3 p)
		{
			throw new NotImplementedException();
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
