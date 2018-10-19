using System;
using UnityEngine;

namespace Pathfinding
{
	[Serializable]
	public class StartEndModifier : PathModifier
	{
		public enum Exactness
		{
			SnapToNode,
			Original,
			Interpolate,
			ClosestOnNode
		}

		public bool addPoints;

		public StartEndModifier.Exactness exactStartPoint = StartEndModifier.Exactness.ClosestOnNode;

		public StartEndModifier.Exactness exactEndPoint = StartEndModifier.Exactness.ClosestOnNode;

		public LayerMask mask = -1;

		public bool useGraphRaycasting;

		public override ModifierData input
		{
			get
			{
				return ModifierData.Vector;
			}
		}

		public override ModifierData output
		{
			get
			{
				return ((!this.addPoints) ? ModifierData.StrictVectorPath : ModifierData.None) | ModifierData.VectorPath;
			}
		}

		public override void Apply(Path _p, ModifierData source)
		{
			ABPath aBPath = _p as ABPath;
			if (aBPath == null)
			{
				return;
			}
			if (aBPath.vectorPath.Count == 0)
			{
				return;
			}
			if (aBPath.vectorPath.Count < 2 && !this.addPoints)
			{
				aBPath.vectorPath.Add(aBPath.vectorPath[0]);
			}
			VInt3 vInt = VInt3.zero;
			VInt3 vInt2 = VInt3.zero;
			if (this.exactStartPoint == StartEndModifier.Exactness.Original)
			{
				vInt = this.GetClampedPoint(aBPath.path[0].position, aBPath.originalStartPoint, aBPath.path[0]);
			}
			else if (this.exactStartPoint == StartEndModifier.Exactness.ClosestOnNode)
			{
				vInt = this.GetClampedPoint(aBPath.path[0].position, aBPath.startPoint, aBPath.path[0]);
			}
			else if (this.exactStartPoint == StartEndModifier.Exactness.Interpolate)
			{
				vInt = this.GetClampedPoint(aBPath.path[0].position, aBPath.originalStartPoint, aBPath.path[0]);
				VInt3 position = aBPath.path[0].position;
				VInt3 position2 = aBPath.path[(1 < aBPath.path.Count) ? 1 : 0].position;
				vInt = AstarMath.NearestPointStrict(ref position, ref position2, ref vInt);
			}
			else
			{
				vInt = aBPath.path[0].position;
			}
			if (this.exactEndPoint == StartEndModifier.Exactness.Original)
			{
				vInt2 = this.GetClampedPoint(aBPath.path[aBPath.path.Count - 1].position, aBPath.originalEndPoint, aBPath.path[aBPath.path.Count - 1]);
			}
			else if (this.exactEndPoint == StartEndModifier.Exactness.ClosestOnNode)
			{
				vInt2 = this.GetClampedPoint(aBPath.path[aBPath.path.Count - 1].position, aBPath.endPoint, aBPath.path[aBPath.path.Count - 1]);
			}
			else if (this.exactEndPoint == StartEndModifier.Exactness.Interpolate)
			{
				vInt2 = this.GetClampedPoint(aBPath.path[aBPath.path.Count - 1].position, aBPath.originalEndPoint, aBPath.path[aBPath.path.Count - 1]);
				VInt3 position3 = aBPath.path[aBPath.path.Count - 1].position;
				VInt3 position4 = aBPath.path[(aBPath.path.Count - 2 >= 0) ? (aBPath.path.Count - 2) : 0].position;
				vInt2 = AstarMath.NearestPointStrict(ref position3, ref position4, ref vInt2);
			}
			else
			{
				vInt2 = aBPath.path[aBPath.path.Count - 1].position;
			}
			if (!this.addPoints)
			{
				aBPath.vectorPath[0] = vInt;
				aBPath.vectorPath[aBPath.vectorPath.Count - 1] = vInt2;
			}
			else
			{
				if (this.exactStartPoint != StartEndModifier.Exactness.SnapToNode)
				{
					aBPath.vectorPath.Insert(0, vInt);
				}
				if (this.exactEndPoint != StartEndModifier.Exactness.SnapToNode)
				{
					aBPath.vectorPath.Add(vInt2);
				}
			}
		}

		public VInt3 GetClampedPoint(VInt3 from, VInt3 to, GraphNode hint)
		{
			VInt3 vInt = to;
			if (this.useGraphRaycasting && hint != null)
			{
				NavGraph graph = AstarData.GetGraph(hint);
				if (graph != null)
				{
					IRaycastableGraph raycastableGraph = graph as IRaycastableGraph;
					GraphHitInfo graphHitInfo;
					if (raycastableGraph != null && raycastableGraph.Linecast(from, vInt, hint, out graphHitInfo))
					{
						vInt = graphHitInfo.point;
					}
				}
			}
			return vInt;
		}
	}
}
