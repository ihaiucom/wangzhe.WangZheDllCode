using System;
using UnityEngine;

namespace Pathfinding
{
	public class FloodPath : Path
	{
		public Vector3 originalStartPoint;

		public Vector3 startPoint;

		public GraphNode startNode;

		public bool saveParents = true;

		protected DictionaryObjectView<GraphNode, GraphNode> parents;

		public override bool FloodingPath
		{
			get
			{
				return true;
			}
		}

		public static FloodPath Construct(GraphNode start, OnPathDelegate callback = null)
		{
			if (start == null)
			{
				throw new ArgumentNullException("start");
			}
			FloodPath path = PathPool<FloodPath>.GetPath();
			path.Setup(start, callback);
			return path;
		}

		protected void Setup(GraphNode start, OnPathDelegate callback)
		{
			this.callback = callback;
			this.originalStartPoint = (Vector3)start.position;
			this.startNode = start;
			this.startPoint = (Vector3)start.position;
			this.heuristic = Heuristic.None;
		}

		public override void Reset()
		{
			base.Reset();
			this.originalStartPoint = Vector3.zero;
			this.startPoint = Vector3.zero;
			this.startNode = null;
			this.parents = new DictionaryObjectView<GraphNode, GraphNode>();
			this.saveParents = true;
		}

		protected override void Recycle()
		{
			PathPool<FloodPath>.Recycle(this);
		}

		public override void Prepare()
		{
			if (this.startNode == null)
			{
				this.nnConstraint.tags = this.enabledTags;
				NNInfo nearest = AstarPath.active.GetNearest(this.originalStartPoint, this.nnConstraint);
				this.startPoint = nearest.clampedPosition;
				this.startNode = nearest.node;
			}
			else
			{
				this.startPoint = (Vector3)this.startNode.position;
			}
			if (this.startNode == null)
			{
				base.Error();
				base.LogError("Couldn't find a close node to the start point");
				return;
			}
			if (!this.startNode.Walkable)
			{
				base.Error();
				base.LogError("The node closest to the start point is not walkable");
				return;
			}
		}

		public override void Initialize()
		{
			PathNode pathNode = this.pathHandler.GetPathNode(this.startNode);
			pathNode.node = this.startNode;
			pathNode.pathID = this.pathHandler.PathID;
			pathNode.parent = null;
			pathNode.cost = 0u;
			pathNode.G = base.GetTraversalCost(this.startNode);
			pathNode.H = base.CalculateHScore(this.startNode);
			this.parents[this.startNode] = null;
			this.startNode.Open(this, pathNode, this.pathHandler);
			this.searchedNodes++;
			if (this.pathHandler.HeapEmpty())
			{
				base.CompleteState = PathCompleteState.Complete;
			}
			this.currentR = this.pathHandler.PopNode();
		}

		public override void CalculateStep(long targetTick)
		{
			int num = 0;
			while (base.CompleteState == PathCompleteState.NotCalculated)
			{
				this.searchedNodes++;
				this.currentR.node.Open(this, this.currentR, this.pathHandler);
				if (this.saveParents)
				{
					this.parents[this.currentR.node] = this.currentR.parent.node;
				}
				if (this.pathHandler.HeapEmpty())
				{
					base.CompleteState = PathCompleteState.Complete;
					break;
				}
				this.currentR = this.pathHandler.PopNode();
				if (num > 500)
				{
					if (DateTime.get_UtcNow().get_Ticks() >= targetTick)
					{
						return;
					}
					num = 0;
					if (this.searchedNodes > 1000000)
					{
						throw new Exception("Probable infinite loop. Over 1,000,000 nodes searched");
					}
				}
				num++;
			}
		}
	}
}
