using System;
using UnityEngine;

namespace Pathfinding
{
	public class GraphUpdateShape
	{
		private Vector3[] _points;

		private Vector3[] _convexPoints;

		private bool _convex;

		public Vector3[] points
		{
			get
			{
				return this._points;
			}
			set
			{
				this._points = value;
				if (this.convex)
				{
					this.CalculateConvexHull();
				}
			}
		}

		public bool convex
		{
			get
			{
				return this._convex;
			}
			set
			{
				if (this._convex != value && value)
				{
					this._convex = value;
					this.CalculateConvexHull();
				}
				else
				{
					this._convex = value;
				}
			}
		}

		private void CalculateConvexHull()
		{
			if (this.points == null)
			{
				this._convexPoints = null;
				return;
			}
			this._convexPoints = Polygon.ConvexHull(this.points);
			for (int i = 0; i < this._convexPoints.Length; i++)
			{
				Debug.DrawLine(this._convexPoints[i], this._convexPoints[(i + 1) % this._convexPoints.Length], Color.green);
			}
		}

		public Bounds GetBounds()
		{
			if (this.points == null || this.points.Length == 0)
			{
				return default(Bounds);
			}
			Vector3 vector = this.points[0];
			Vector3 vector2 = this.points[0];
			for (int i = 0; i < this.points.Length; i++)
			{
				vector = Vector3.Min(vector, this.points[i]);
				vector2 = Vector3.Max(vector2, this.points[i]);
			}
			return new Bounds((vector + vector2) * 0.5f, vector2 - vector);
		}

		public bool Contains(GraphNode node)
		{
			Vector3 p = (Vector3)node.position;
			if (!this.convex)
			{
				return this._points != null && Polygon.ContainsPoint(this._points, p);
			}
			if (this._convexPoints == null)
			{
				return false;
			}
			int i = 0;
			int num = this._convexPoints.Length - 1;
			while (i < this._convexPoints.Length)
			{
				if (Polygon.Left(this._convexPoints[i], this._convexPoints[num], p))
				{
					return false;
				}
				num = i;
				i++;
			}
			return true;
		}

		public bool Contains(Vector3 point)
		{
			if (!this.convex)
			{
				return this._points != null && Polygon.ContainsPoint(this._points, point);
			}
			if (this._convexPoints == null)
			{
				return false;
			}
			int i = 0;
			int num = this._convexPoints.Length - 1;
			while (i < this._convexPoints.Length)
			{
				if (Polygon.Left(this._convexPoints[i], this._convexPoints[num], point))
				{
					return false;
				}
				num = i;
				i++;
			}
			return true;
		}
	}
}
