using System;
using System.Text;
using UnityEngine;

namespace Pathfinding
{
	public class ABPath : Path
	{
		public bool recalcStartEndCosts = true;

		public GraphNode startNode;

		public GraphNode endNode;

		public GraphNode startHint;

		public GraphNode endHint;

		public VInt3 originalStartPoint;

		public VInt3 originalEndPoint;

		public VInt3 startPoint;

		public VInt3 endPoint;

		protected bool hasEndPoint = true;

		public bool calculatePartial;

		protected PathNode partialBestTarget;

		protected int[] endNodeCosts;

		public AstarData targetAstarData
		{
			get
			{
				if (AstarPath.active.astarDataArray != null && this.astarDataIndex >= 0 && this.astarDataIndex < AstarPath.active.astarDataArray.Length)
				{
					return AstarPath.active.astarDataArray[this.astarDataIndex];
				}
				return AstarPath.active.astarData;
			}
		}

		[Obsolete("Use PathPool<T>.GetPath instead")]
		public ABPath(VInt3 start, VInt3 end, OnPathDelegate callbackDelegate)
		{
			this.Reset();
			this.Setup(ref start, ref end, callbackDelegate);
		}

		public ABPath()
		{
		}

		public static ABPath Construct(ref VInt3 start, ref VInt3 end, OnPathDelegate callback = null)
		{
			ABPath path = PathPool<ABPath>.GetPath();
			path.Setup(ref start, ref end, callback);
			return path;
		}

		protected void Setup(ref VInt3 start, ref VInt3 end, OnPathDelegate callbackDelegate)
		{
			this.callback = callbackDelegate;
			this.UpdateStartEnd(ref start, ref end);
		}

		protected void UpdateStartEnd(ref VInt3 start, ref VInt3 end)
		{
			this.originalStartPoint = start;
			this.originalEndPoint = end;
			this.startPoint = start;
			this.endPoint = end;
			this.hTarget = end;
		}

		public override uint GetConnectionSpecialCost(GraphNode a, GraphNode b, uint currentCost)
		{
			if (this.startNode != null && this.endNode != null)
			{
				if (a == this.startNode)
				{
					return (uint)((double)(this.startPoint - ((b != this.endNode) ? b.position : this.hTarget)).costMagnitude * (currentCost * 1.0 / (double)(a.position - b.position).costMagnitude));
				}
				if (b == this.startNode)
				{
					return (uint)((double)(this.startPoint - ((a != this.endNode) ? a.position : this.hTarget)).costMagnitude * (currentCost * 1.0 / (double)(a.position - b.position).costMagnitude));
				}
				if (a == this.endNode)
				{
					return (uint)((double)(this.hTarget - b.position).costMagnitude * (currentCost * 1.0 / (double)(a.position - b.position).costMagnitude));
				}
				if (b == this.endNode)
				{
					return (uint)((double)(this.hTarget - a.position).costMagnitude * (currentCost * 1.0 / (double)(a.position - b.position).costMagnitude));
				}
			}
			else
			{
				if (a == this.startNode)
				{
					return (uint)((double)(this.startPoint - b.position).costMagnitude * (currentCost * 1.0 / (double)(a.position - b.position).costMagnitude));
				}
				if (b == this.startNode)
				{
					return (uint)((double)(this.startPoint - a.position).costMagnitude * (currentCost * 1.0 / (double)(a.position - b.position).costMagnitude));
				}
			}
			return currentCost;
		}

		public override void Reset()
		{
			base.Reset();
			this.startNode = null;
			this.endNode = null;
			this.startHint = null;
			this.endHint = null;
			this.originalStartPoint = VInt3.zero;
			this.originalEndPoint = VInt3.zero;
			this.startPoint = VInt3.zero;
			this.endPoint = VInt3.zero;
			this.calculatePartial = false;
			this.partialBestTarget = null;
			this.hasEndPoint = true;
			this.hTarget = default(VInt3);
			this.endNodeCosts = null;
		}

		public override void Prepare()
		{
			AstarData targetAstarData = this.targetAstarData;
			VInt3 vInt;
			this.startNode = targetAstarData.GetNearestByRasterizer(this.startPoint, out vInt);
			if (this.startNode != null)
			{
				this.startPoint = vInt;
			}
			if (this.hasEndPoint)
			{
				this.endNode = targetAstarData.GetNearestByRasterizer(this.endPoint, out vInt);
				if (this.endNode != null)
				{
					this.endPoint = vInt;
				}
				this.hTarget = this.endPoint;
				this.hTargetNode = this.endNode;
			}
			if (this.startNode == null && this.hasEndPoint && this.endNode == null)
			{
				base.Error();
				base.LogError("Couldn't find close nodes to the start point or the end point");
				return;
			}
			if (this.startNode == null)
			{
				base.Error();
				base.LogError("Couldn't find a close node to the start point");
				return;
			}
			if (this.endNode == null && this.hasEndPoint)
			{
				base.Error();
				base.LogError("Couldn't find a close node to the end point");
				return;
			}
			if (!this.startNode.Walkable)
			{
				base.Error();
				base.LogError("The node closest to the start point is not walkable");
				return;
			}
			if (this.hasEndPoint && !this.endNode.Walkable)
			{
				base.Error();
				base.LogError("The node closest to the end point is not walkable");
				return;
			}
			if (this.hasEndPoint && this.startNode.Area != this.endNode.Area)
			{
				base.Error();
				base.LogError(string.Concat(new object[]
				{
					"There is no valid path to the target (start area: ",
					this.startNode.Area,
					", target area: ",
					this.endNode.Area,
					")"
				}));
				return;
			}
		}

		public override void Initialize()
		{
			if (this.startNode != null)
			{
				this.pathHandler.GetPathNode(this.startNode).flag2 = true;
			}
			if (this.endNode != null)
			{
				this.pathHandler.GetPathNode(this.endNode).flag2 = true;
			}
			if (this.hasEndPoint && this.startNode == this.endNode)
			{
				PathNode pathNode = this.pathHandler.GetPathNode(this.endNode);
				pathNode.node = this.endNode;
				pathNode.parent = null;
				pathNode.H = 0u;
				pathNode.G = 0u;
				this.Trace(pathNode);
				base.CompleteState = PathCompleteState.Complete;
				return;
			}
			PathNode pathNode2 = this.pathHandler.GetPathNode(this.startNode);
			pathNode2.node = this.startNode;
			pathNode2.pathID = this.pathHandler.PathID;
			pathNode2.parent = null;
			pathNode2.cost = 0u;
			pathNode2.G = ((this.startNode == null) ? 0u : base.GetTraversalCost(this.startNode));
			pathNode2.H = ((this.startNode == null) ? 0u : base.CalculateHScore(this.startNode));
			DebugHelper.Assert(this.startNode != null, "startNode != null");
			if (this.startNode != null)
			{
				this.startNode.Open(this, pathNode2, this.pathHandler);
			}
			this.searchedNodes++;
			this.partialBestTarget = pathNode2;
			if (this.pathHandler.HeapEmpty())
			{
				if (!this.calculatePartial)
				{
					base.Error();
					base.LogError("No open points, the start node didn't open any nodes");
					return;
				}
				base.CompleteState = PathCompleteState.Partial;
				this.Trace(this.partialBestTarget);
			}
			this.currentR = this.pathHandler.PopNode();
		}

		public override void Cleanup()
		{
			if (this.startNode != null)
			{
				this.pathHandler.GetPathNode(this.startNode).flag2 = false;
			}
			if (this.endNode != null)
			{
				this.pathHandler.GetPathNode(this.endNode).flag2 = false;
			}
		}

		public override void CalculateStep(long targetTick)
		{
			int num = 0;
			while (base.CompleteState == PathCompleteState.NotCalculated)
			{
				this.searchedNodes++;
				if (this.currentR.node == this.endNode)
				{
					base.CompleteState = PathCompleteState.Complete;
					break;
				}
				if (this.currentR.H < this.partialBestTarget.H)
				{
					this.partialBestTarget = this.currentR;
				}
				this.currentR.node.Open(this, this.currentR, this.pathHandler);
				if (this.pathHandler.HeapEmpty())
				{
					base.Error();
					base.LogError("No open points, whole area searched");
					return;
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
			if (base.CompleteState == PathCompleteState.Complete)
			{
				this.Trace(this.currentR);
			}
			else if (this.calculatePartial && this.partialBestTarget != null)
			{
				base.CompleteState = PathCompleteState.Partial;
				this.Trace(this.partialBestTarget);
			}
		}

		public void ResetCosts(Path p)
		{
		}

		public override string DebugString(PathLog logMode)
		{
			if (logMode == PathLog.None || (!base.error && logMode == PathLog.OnlyErrors))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append((!base.error) ? "Path Completed : " : "Path Failed : ");
			stringBuilder.Append("Computation Time ");
			stringBuilder.Append(this.duration.ToString((logMode != PathLog.Heavy) ? "0.00" : "0.000"));
			stringBuilder.Append(" ms Searched Nodes ");
			stringBuilder.Append(this.searchedNodes);
			if (!base.error)
			{
				stringBuilder.Append(" Path Length ");
				stringBuilder.Append((this.path != null) ? this.path.Count.ToString() : "Null");
				if (logMode == PathLog.Heavy)
				{
					stringBuilder.Append("\nSearch Iterations " + this.searchIterations);
					if (this.hasEndPoint && this.endNode != null)
					{
						PathNode pathNode = this.pathHandler.GetPathNode(this.endNode);
						stringBuilder.Append("\nEnd Node\n\tG: ");
						stringBuilder.Append(pathNode.G);
						stringBuilder.Append("\n\tH: ");
						stringBuilder.Append(pathNode.H);
						stringBuilder.Append("\n\tF: ");
						stringBuilder.Append(pathNode.F);
						stringBuilder.Append("\n\tPoint: ");
						stringBuilder.Append(((Vector3)this.endPoint).ToString());
						stringBuilder.Append("\n\tGraph: ");
						stringBuilder.Append(this.endNode.GraphIndex);
					}
					stringBuilder.Append("\nStart Node");
					stringBuilder.Append("\n\tPoint: ");
					stringBuilder.Append(((Vector3)this.startPoint).ToString());
					stringBuilder.Append("\n\tGraph: ");
					if (this.startNode != null)
					{
						stringBuilder.Append(this.startNode.GraphIndex);
					}
					else
					{
						stringBuilder.Append("< null startNode >");
					}
				}
			}
			if (base.error)
			{
				stringBuilder.Append("\nError: ");
				stringBuilder.Append(base.errorLog);
			}
			if (logMode == PathLog.Heavy && !AstarPath.IsUsingMultithreading)
			{
				stringBuilder.Append("\nCallback references ");
				if (this.callback != null)
				{
					stringBuilder.Append(this.callback.Target.GetType().FullName).AppendLine();
				}
				else
				{
					stringBuilder.AppendLine("NULL");
				}
			}
			stringBuilder.Append("\nPath Number ");
			stringBuilder.Append(this.pathID);
			return stringBuilder.ToString();
		}

		protected override void Recycle()
		{
			PathPool<ABPath>.Recycle(this);
		}
	}
}
