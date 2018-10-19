using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class ConstantPath : Path
	{
		public GraphNode startNode;

		public Vector3 startPoint;

		public Vector3 originalStartPoint;

		public List<GraphNode> allNodes;

		public PathEndingCondition endingCondition;

		public override bool FloodingPath
		{
			get
			{
				return true;
			}
		}

		public ConstantPath()
		{
		}

		[Obsolete("Please use the Construct method instead")]
		public ConstantPath(Vector3 start, OnPathDelegate callbackDelegate)
		{
			throw new Exception("This constructor is obsolete, please use the Construct method instead");
		}

		[Obsolete("Please use the Construct method instead")]
		public ConstantPath(Vector3 start, int maxGScore, OnPathDelegate callbackDelegate)
		{
			throw new Exception("This constructor is obsolete, please use the Construct method instead");
		}

		public static ConstantPath Construct(Vector3 start, int maxGScore, OnPathDelegate callback = null)
		{
			ConstantPath path = PathPool<ConstantPath>.GetPath();
			path.Setup(start, maxGScore, callback);
			return path;
		}

		protected void Setup(Vector3 start, int maxGScore, OnPathDelegate callback)
		{
			this.callback = callback;
			this.startPoint = start;
			this.originalStartPoint = this.startPoint;
			this.endingCondition = new EndingConditionDistance(this, maxGScore);
		}

		public override void OnEnterPool()
		{
			base.OnEnterPool();
			if (this.allNodes != null)
			{
				ListPool<GraphNode>.Release(this.allNodes);
			}
		}

		protected override void Recycle()
		{
			PathPool<ConstantPath>.Recycle(this);
		}

		public override void Reset()
		{
			base.Reset();
			this.allNodes = ListPool<GraphNode>.Claim();
			this.endingCondition = null;
			this.originalStartPoint = Vector3.zero;
			this.startPoint = Vector3.zero;
			this.startNode = null;
			this.heuristic = Heuristic.None;
		}

		public override void Prepare()
		{
			this.nnConstraint.tags = this.enabledTags;
			this.startNode = AstarPath.active.GetNearest(this.startPoint, this.nnConstraint).node;
			if (this.startNode == null)
			{
				base.Error();
				base.LogError("Could not find close node to the start point");
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
			this.startNode.Open(this, pathNode, this.pathHandler);
			this.searchedNodes++;
			pathNode.flag1 = true;
			this.allNodes.Add(this.startNode);
			if (this.pathHandler.HeapEmpty())
			{
				base.CompleteState = PathCompleteState.Complete;
				return;
			}
			this.currentR = this.pathHandler.PopNode();
		}

		public override void Cleanup()
		{
			int count = this.allNodes.Count;
			for (int i = 0; i < count; i++)
			{
				this.pathHandler.GetPathNode(this.allNodes[i]).flag1 = false;
			}
		}

		public override void CalculateStep(long targetTick)
		{
			int num = 0;
			while (base.CompleteState == PathCompleteState.NotCalculated)
			{
				this.searchedNodes++;
				if (this.endingCondition.TargetFound(this.currentR))
				{
					base.CompleteState = PathCompleteState.Complete;
					break;
				}
				if (!this.currentR.flag1)
				{
					this.allNodes.Add(this.currentR.node);
					this.currentR.flag1 = true;
				}
				this.currentR.node.Open(this, this.currentR, this.pathHandler);
				if (this.pathHandler.HeapEmpty())
				{
					base.CompleteState = PathCompleteState.Complete;
					break;
				}
				this.currentR = this.pathHandler.PopNode();
				if (num > 500)
				{
					if (DateTime.UtcNow.Ticks >= targetTick)
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
