using System;
using UnityEngine;

namespace Pathfinding
{
	[AddComponentMenu("Pathfinding/Link")]
	public class NodeLink : GraphModifier
	{
		public Transform end;

		public float costFactor = 1f;

		public bool oneWay;

		public bool deleteConnection;

		public Transform Start
		{
			get
			{
				return base.transform;
			}
		}

		public Transform End
		{
			get
			{
				return this.end;
			}
		}

		public override void OnPostScan()
		{
			if (AstarPath.active.isScanning)
			{
				this.InternalOnPostScan();
			}
			else
			{
				AstarPath.active.AddWorkItem(new AstarPath.AstarWorkItem(delegate(bool force)
				{
					this.InternalOnPostScan();
					return true;
				}));
			}
		}

		public void InternalOnPostScan()
		{
			this.Apply();
		}

		public override void OnGraphsPostUpdate()
		{
			if (!AstarPath.active.isScanning)
			{
				AstarPath.active.AddWorkItem(new AstarPath.AstarWorkItem(delegate(bool force)
				{
					this.InternalOnPostScan();
					return true;
				}));
			}
		}

		public virtual void Apply()
		{
			if (this.Start == null || this.End == null || AstarPath.active == null)
			{
				return;
			}
			GraphNode node = AstarPath.active.GetNearest(this.Start.position).node;
			GraphNode node2 = AstarPath.active.GetNearest(this.End.position).node;
			if (node == null || node2 == null)
			{
				return;
			}
			if (this.deleteConnection)
			{
				node.RemoveConnection(node2);
				if (!this.oneWay)
				{
					node2.RemoveConnection(node);
				}
			}
			else
			{
				uint cost = (uint)MMGame_Math.RoundToInt((double)((float)(node.position - node2.position).costMagnitude * this.costFactor));
				node.AddConnection(node2, cost);
				if (!this.oneWay)
				{
					node2.AddConnection(node, cost);
				}
			}
		}

		public void OnDrawGizmos()
		{
			if (this.Start == null || this.End == null)
			{
				return;
			}
			Vector3 position = this.Start.position;
			Vector3 position2 = this.End.position;
			Gizmos.color = (this.deleteConnection ? Color.red : Color.green);
			this.DrawGizmoBezier(position, position2);
		}

		private void DrawGizmoBezier(Vector3 p1, Vector3 p2)
		{
			Vector3 vector = p2 - p1;
			if (vector == Vector3.zero)
			{
				return;
			}
			Vector3 rhs = Vector3.Cross(Vector3.up, vector);
			Vector3 vector2 = Vector3.Cross(vector, rhs).normalized;
			vector2 *= vector.magnitude * 0.1f;
			Vector3 p3 = p1 + vector2;
			Vector3 p4 = p2 + vector2;
			Vector3 from = p1;
			for (int i = 1; i <= 20; i++)
			{
				float t = (float)i / 20f;
				Vector3 vector3 = AstarMath.CubicBezier(p1, p3, p4, p2, t);
				Gizmos.DrawLine(from, vector3);
				from = vector3;
			}
		}
	}
}
