using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.RVO
{
	[AddComponentMenu("Pathfinding/Local Avoidance/RVO Navmesh")]
	public class RVONavmesh : GraphModifier
	{
		public VInt wallHeight = 5000;

		private List<ObstacleVertex> obstacles = new List<ObstacleVertex>();

		private Simulator lastSim;

		public override void OnPostCacheLoad()
		{
			this.OnLatePostScan();
		}

		public override void OnLatePostScan()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			this.RemoveObstacles();
			NavGraph[] graphs = AstarPath.active.graphs;
			RVOSimulator rVOSimulator = UnityEngine.Object.FindObjectOfType(typeof(RVOSimulator)) as RVOSimulator;
			if (rVOSimulator == null)
			{
				throw new NullReferenceException("No RVOSimulator could be found in the scene. Please add one to any GameObject");
			}
			Simulator simulator = rVOSimulator.GetSimulator();
			for (int i = 0; i < graphs.Length; i++)
			{
				this.AddGraphObstacles(simulator, graphs[i]);
			}
			simulator.UpdateObstacles();
		}

		public void RemoveObstacles()
		{
			if (this.lastSim == null)
			{
				return;
			}
			Simulator simulator = this.lastSim;
			this.lastSim = null;
			for (int i = 0; i < this.obstacles.Count; i++)
			{
				simulator.RemoveObstacle(this.obstacles[i]);
			}
			this.obstacles.Clear();
		}

		public void AddGraphObstacles(Simulator sim, NavGraph graph)
		{
			if (this.obstacles.Count > 0 && this.lastSim != null && this.lastSim != sim)
			{
				this.RemoveObstacles();
			}
			this.lastSim = sim;
			INavmesh navmesh = graph as INavmesh;
			if (navmesh == null)
			{
				return;
			}
			int[] uses = new int[20];
			navmesh.GetNodes(delegate(GraphNode _node)
			{
				TriangleMeshNode triangleMeshNode = _node as TriangleMeshNode;
				uses[0] = (uses[1] = (uses[2] = 0));
				if (triangleMeshNode != null)
				{
					for (int i = 0; i < triangleMeshNode.connections.Length; i++)
					{
						TriangleMeshNode triangleMeshNode2 = triangleMeshNode.connections[i] as TriangleMeshNode;
						if (triangleMeshNode2 != null)
						{
							int num = triangleMeshNode.SharedEdge(triangleMeshNode2);
							if (num != -1)
							{
								uses[num] = 1;
							}
						}
					}
					for (int j = 0; j < 3; j++)
					{
						if (uses[j] == 0)
						{
							VInt3 vertex = triangleMeshNode.GetVertex(j);
							VInt3 vertex2 = triangleMeshNode.GetVertex((j + 1) % triangleMeshNode.GetVertexCount());
							this.obstacles.Add(sim.AddObstacle(vertex, vertex2, this.wallHeight));
						}
					}
				}
				return true;
			});
		}
	}
}
