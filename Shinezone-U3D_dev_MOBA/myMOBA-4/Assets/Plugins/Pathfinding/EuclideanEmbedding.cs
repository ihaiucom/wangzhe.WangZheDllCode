using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	[Serializable]
	public class EuclideanEmbedding
	{
		public HeuristicOptimizationMode mode;

		public int seed;

		public Transform pivotPointRoot;

		public int spreadOutCount = 1;

		private uint[] costs = new uint[8];

		private int maxNodeIndex;

		private int pivotCount;

		[NonSerialized]
		public bool dirty;

		private GraphNode[] pivots;

		private uint ra = 12820163u;

		private uint rc = 1140671485u;

		private uint rval;

		private object lockObj = new object();

		public uint GetRandom()
		{
			this.rval = this.ra * this.rval + this.rc;
			return this.rval;
		}

		private void EnsureCapacity(int index)
		{
			if (index > this.maxNodeIndex && index > this.maxNodeIndex)
			{
				if (index >= this.costs.Length)
				{
					uint[] array = new uint[Math.Max(index * 2, this.pivots.Length * 2)];
					for (int i = 0; i < this.costs.Length; i++)
					{
						array[i] = this.costs[i];
					}
					this.costs = array;
				}
				this.maxNodeIndex = index;
			}
		}

		public uint GetHeuristic(int nodeIndex1, int nodeIndex2)
		{
			nodeIndex1 *= this.pivotCount;
			nodeIndex2 *= this.pivotCount;
			if (nodeIndex1 >= this.costs.Length || nodeIndex2 >= this.costs.Length)
			{
				this.EnsureCapacity((nodeIndex1 <= nodeIndex2) ? nodeIndex2 : nodeIndex1);
			}
			uint num = 0u;
			for (int i = 0; i < this.pivotCount; i++)
			{
				uint num2 = (uint)Math.Abs((int)(this.costs[nodeIndex1 + i] - this.costs[nodeIndex2 + i]));
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		private void GetClosestWalkableNodesToChildrenRecursively(Transform tr, List<GraphNode> nodes)
		{
			foreach (Transform transform in tr)
			{
				NNInfo nearest = AstarPath.active.GetNearest(transform.position, NNConstraint.Default);
				if (nearest.node != null && nearest.node.Walkable)
				{
					nodes.Add(nearest.node);
				}
				this.GetClosestWalkableNodesToChildrenRecursively(tr, nodes);
			}
		}

		public void RecalculatePivots()
		{
			if (this.mode == HeuristicOptimizationMode.None)
			{
				this.pivotCount = 0;
				this.pivots = null;
				return;
			}
			this.rval = (uint)this.seed;
			NavGraph[] graphs = AstarPath.active.graphs;
			List<GraphNode> pivotList = ListPool<GraphNode>.Claim();
			if (this.mode == HeuristicOptimizationMode.Custom)
			{
				if (this.pivotPointRoot == null)
				{
					throw new Exception("Grid Graph -> heuristicOptimizationMode is HeuristicOptimizationMode.Custom, but no 'customHeuristicOptimizationPivotsRoot' is set");
				}
				this.GetClosestWalkableNodesToChildrenRecursively(this.pivotPointRoot, pivotList);
			}
			else if (this.mode == HeuristicOptimizationMode.Random)
			{
				int n = 0;
				for (int i = 0; i < graphs.Length; i++)
				{
					graphs[i].GetNodes(delegate(GraphNode node)
					{
						if (!node.Destroyed && node.Walkable)
						{
							n++;
							if ((ulong)this.GetRandom() % (ulong)((long)n) < (ulong)((long)this.spreadOutCount))
							{
								if (pivotList.Count < this.spreadOutCount)
								{
									pivotList.Add(node);
								}
								else
								{
									pivotList[(int)((ulong)this.GetRandom() % (ulong)((long)pivotList.Count))] = node;
								}
							}
						}
						return true;
					});
				}
			}
			else
			{
				if (this.mode != HeuristicOptimizationMode.RandomSpreadOut)
				{
					throw new Exception("Invalid HeuristicOptimizationMode: " + this.mode);
				}
				GraphNode first = null;
				if (this.pivotPointRoot != null)
				{
					this.GetClosestWalkableNodesToChildrenRecursively(this.pivotPointRoot, pivotList);
				}
				else
				{
					for (int j = 0; j < graphs.Length; j++)
					{
						graphs[j].GetNodes(delegate(GraphNode node)
						{
							if (node != null && node.Walkable)
							{
								first = node;
								return false;
							}
							return true;
						});
					}
					if (first == null)
					{
						Debug.LogError("Could not find any walkable node in any of the graphs.");
						ListPool<GraphNode>.Release(pivotList);
						return;
					}
					pivotList.Add(first);
				}
				for (int k = 0; k < this.spreadOutCount; k++)
				{
					pivotList.Add(null);
				}
			}
			this.pivots = pivotList.ToArray();
			ListPool<GraphNode>.Release(pivotList);
		}

		public void RecalculateCosts()
		{
			if (this.pivots == null)
			{
				this.RecalculatePivots();
			}
			if (this.mode == HeuristicOptimizationMode.None)
			{
				return;
			}
			this.pivotCount = 0;
			DebugHelper.Assert(this.pivots != null);
			for (int i = 0; i < this.pivots.Length; i++)
			{
				if (this.pivots[i] != null && (this.pivots[i].Destroyed || !this.pivots[i].Walkable))
				{
					throw new Exception("Invalid pivot nodes (destroyed or unwalkable)");
				}
			}
			if (this.mode != HeuristicOptimizationMode.RandomSpreadOut)
			{
				for (int j = 0; j < this.pivots.Length; j++)
				{
					if (this.pivots[j] == null)
					{
						throw new Exception("Invalid pivot nodes (null)");
					}
				}
			}
			Debug.Log("Recalculating costs...");
			this.pivotCount = this.pivots.Length;
			Action<int> startCostCalculation = null;
			startCostCalculation = delegate(int k)
			{
				GraphNode pivot = this.pivots[k];
				FloodPath fp = null;
				fp = FloodPath.Construct(pivot, null);
				fp.immediateCallback = delegate(Path _p)
				{
					_p.Claim(this);
					MeshNode meshNode = pivot as MeshNode;
					uint costOffset = 0u;
					if (meshNode != null && meshNode.connectionCosts != null)
					{
						for (int i = 0; i < meshNode.connectionCosts.Length; i++)
						{
							costOffset = Math.Max(costOffset, meshNode.connectionCosts[i]);
						}
					}
					NavGraph[] graphs = AstarPath.active.graphs;
					for (int m = graphs.Length - 1; m >= 0; m--)
					{
						graphs[m].GetNodes(delegate(GraphNode node)
						{
							int num6 = node.NodeIndex * this.pivotCount + k;
							this.EnsureCapacity(num6);
							PathNode pathNode = fp.pathHandler.GetPathNode(node);
							if (costOffset > 0u)
							{
								this.costs[num6] = ((pathNode.pathID != fp.pathID || pathNode.parent == null) ? 0u : Math.Max(pathNode.parent.G - costOffset, 0u));
							}
							else
							{
								this.costs[num6] = ((pathNode.pathID != fp.pathID) ? 0u : pathNode.G);
							}
							return true;
						});
					}
					if (this.mode == HeuristicOptimizationMode.RandomSpreadOut && k < this.pivots.Length - 1)
					{
						int num = -1;
						uint num2 = 0u;
						int num3 = this.maxNodeIndex / this.pivotCount;
						for (int n = 1; n < num3; n++)
						{
							uint num4 = 1073741824u;
							for (int num5 = 0; num5 <= k; num5++)
							{
								num4 = Math.Min(num4, this.costs[n * this.pivotCount + num5]);
							}
							GraphNode node2 = fp.pathHandler.GetPathNode(n).node;
							if ((num4 > num2 || num == -1) && node2 != null && !node2.Destroyed && node2.Walkable)
							{
								num = n;
								num2 = num4;
							}
						}
						if (num == -1)
						{
							Debug.LogError("Failed generating random pivot points for heuristic optimizations");
							return;
						}
						this.pivots[k + 1] = fp.pathHandler.GetPathNode(num).node;
						Debug.Log(string.Concat(new object[]
						{
							"Found node at ",
							this.pivots[k + 1].position,
							" with score ",
							num2
						}));
						startCostCalculation(k + 1);
					}
					_p.Release(this);
				};
				AstarPath.StartPath(fp, true);
			};
			if (this.mode != HeuristicOptimizationMode.RandomSpreadOut)
			{
				for (int l = 0; l < this.pivots.Length; l++)
				{
					startCostCalculation(l);
				}
			}
			else
			{
				startCostCalculation(0);
			}
			this.dirty = false;
		}

		public void OnDrawGizmos()
		{
			if (this.pivots != null)
			{
				for (int i = 0; i < this.pivots.Length; i++)
				{
					Gizmos.color = new Color(0.623529434f, 0.368627459f, 0.7607843f, 0.8f);
					if (this.pivots[i] != null && !this.pivots[i].Destroyed)
					{
						Gizmos.DrawCube((Vector3)this.pivots[i].position, Vector3.one);
					}
				}
			}
		}
	}
}
