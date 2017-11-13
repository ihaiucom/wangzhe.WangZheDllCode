using System;
using UnityEngine;

namespace Pathfinding
{
	[AddComponentMenu("Pathfinding/GraphUpdateScene")]
	public class GraphUpdateScene : GraphModifier
	{
		public Vector3[] points;

		private Vector3[] convexPoints;

		[HideInInspector]
		public bool convex = true;

		[HideInInspector]
		public float minBoundsHeight = 1f;

		[HideInInspector]
		public int penaltyDelta;

		[HideInInspector]
		public bool modifyWalkability;

		[HideInInspector]
		public bool setWalkability;

		[HideInInspector]
		public bool applyOnStart = true;

		[HideInInspector]
		public bool applyOnScan = true;

		[HideInInspector]
		public bool useWorldSpace;

		[HideInInspector]
		public bool updatePhysics;

		[HideInInspector]
		public bool resetPenaltyOnPhysics = true;

		[HideInInspector]
		public bool updateErosion = true;

		[HideInInspector]
		public bool lockToY;

		[HideInInspector]
		public float lockToYValue;

		[HideInInspector]
		public bool modifyTag;

		[HideInInspector]
		public int setTag;

		private int setTagInvert;

		private bool firstApplied;

		public void Start()
		{
			if (!this.firstApplied && this.applyOnStart)
			{
				this.Apply();
			}
		}

		public override void OnPostScan()
		{
			if (this.applyOnScan)
			{
				this.Apply();
			}
		}

		public virtual void InvertSettings()
		{
			this.setWalkability = !this.setWalkability;
			this.penaltyDelta = -this.penaltyDelta;
			if (this.setTagInvert == 0)
			{
				this.setTagInvert = this.setTag;
				this.setTag = 0;
			}
			else
			{
				this.setTag = this.setTagInvert;
				this.setTagInvert = 0;
			}
		}

		public void RecalcConvex()
		{
			if (this.convex)
			{
				this.convexPoints = Polygon.ConvexHull(this.points);
			}
			else
			{
				this.convexPoints = null;
			}
		}

		public void ToggleUseWorldSpace()
		{
			this.useWorldSpace = !this.useWorldSpace;
			if (this.points == null)
			{
				return;
			}
			this.convexPoints = null;
			Matrix4x4 matrix4x = this.useWorldSpace ? base.transform.localToWorldMatrix : base.transform.worldToLocalMatrix;
			for (int i = 0; i < this.points.Length; i++)
			{
				this.points[i] = matrix4x.MultiplyPoint3x4(this.points[i]);
			}
		}

		public void LockToY()
		{
			if (this.points == null)
			{
				return;
			}
			for (int i = 0; i < this.points.Length; i++)
			{
				this.points[i].y = this.lockToYValue;
			}
		}

		public void Apply(AstarPath active)
		{
			if (this.applyOnScan)
			{
				this.Apply();
			}
		}

		public Bounds GetBounds()
		{
			Bounds bounds;
			if (this.points == null || this.points.Length == 0)
			{
				Collider component = base.GetComponent<Collider>();
				Renderer component2 = base.GetComponent<Renderer>();
				if (component != null)
				{
					bounds = component.bounds;
				}
				else
				{
					if (!(component2 != null))
					{
						return new Bounds(Vector3.zero, Vector3.zero);
					}
					bounds = component2.bounds;
				}
			}
			else
			{
				Matrix4x4 matrix4x = Matrix4x4.identity;
				if (!this.useWorldSpace)
				{
					matrix4x = base.transform.localToWorldMatrix;
				}
				Vector3 vector = matrix4x.MultiplyPoint3x4(this.points[0]);
				Vector3 vector2 = matrix4x.MultiplyPoint3x4(this.points[0]);
				for (int i = 0; i < this.points.Length; i++)
				{
					Vector3 rhs = matrix4x.MultiplyPoint3x4(this.points[i]);
					vector = Vector3.Min(vector, rhs);
					vector2 = Vector3.Max(vector2, rhs);
				}
				bounds = new Bounds((vector + vector2) * 0.5f, vector2 - vector);
			}
			if (bounds.size.y < this.minBoundsHeight)
			{
				bounds.size = new Vector3(bounds.size.x, this.minBoundsHeight, bounds.size.z);
			}
			return bounds;
		}

		public void Apply()
		{
			if (AstarPath.active == null)
			{
				Debug.LogError("There is no AstarPath object in the scene");
				return;
			}
			GraphUpdateObject graphUpdateObject;
			if (this.points == null || this.points.Length == 0)
			{
				Collider component = base.GetComponent<Collider>();
				Renderer component2 = base.GetComponent<Renderer>();
				Bounds bounds;
				if (component != null)
				{
					bounds = component.bounds;
				}
				else
				{
					if (!(component2 != null))
					{
						Debug.LogWarning("Cannot apply GraphUpdateScene, no points defined and no renderer or collider attached");
						return;
					}
					bounds = component2.bounds;
				}
				if (bounds.size.y < this.minBoundsHeight)
				{
					bounds.size = new Vector3(bounds.size.x, this.minBoundsHeight, bounds.size.z);
				}
				graphUpdateObject = new GraphUpdateObject(bounds);
			}
			else
			{
				GraphUpdateShape graphUpdateShape = new GraphUpdateShape();
				graphUpdateShape.convex = this.convex;
				Vector3[] array = this.points;
				if (!this.useWorldSpace)
				{
					array = new Vector3[this.points.Length];
					Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = localToWorldMatrix.MultiplyPoint3x4(this.points[i]);
					}
				}
				graphUpdateShape.points = array;
				Bounds bounds2 = graphUpdateShape.GetBounds();
				if (bounds2.size.y < this.minBoundsHeight)
				{
					bounds2.size = new Vector3(bounds2.size.x, this.minBoundsHeight, bounds2.size.z);
				}
				graphUpdateObject = new GraphUpdateObject(bounds2);
				graphUpdateObject.shape = graphUpdateShape;
			}
			this.firstApplied = true;
			graphUpdateObject.modifyWalkability = this.modifyWalkability;
			graphUpdateObject.setWalkability = this.setWalkability;
			graphUpdateObject.addPenalty = this.penaltyDelta;
			graphUpdateObject.updatePhysics = this.updatePhysics;
			graphUpdateObject.updateErosion = this.updateErosion;
			graphUpdateObject.resetPenaltyOnPhysics = this.resetPenaltyOnPhysics;
			graphUpdateObject.modifyTag = this.modifyTag;
			graphUpdateObject.setTag = this.setTag;
			AstarPath.active.UpdateGraphs(graphUpdateObject);
		}

		public void OnDrawGizmos()
		{
			this.OnDrawGizmos(false);
		}

		public void OnDrawGizmosSelected()
		{
			this.OnDrawGizmos(true);
		}

		public void OnDrawGizmos(bool selected)
		{
			Color color = selected ? new Color(0.8901961f, 0.239215687f, 0.08627451f, 1f) : new Color(0.8901961f, 0.239215687f, 0.08627451f, 0.9f);
			if (selected)
			{
				Gizmos.color = Color.Lerp(color, new Color(1f, 1f, 1f, 0.2f), 0.9f);
				Bounds bounds = this.GetBounds();
				Gizmos.DrawCube(bounds.center, bounds.size);
				Gizmos.DrawWireCube(bounds.center, bounds.size);
			}
			if (this.points == null)
			{
				return;
			}
			if (this.convex)
			{
				color.a *= 0.5f;
			}
			Gizmos.color = color;
			Matrix4x4 matrix4x = this.useWorldSpace ? Matrix4x4.identity : base.transform.localToWorldMatrix;
			if (this.convex)
			{
				color.r -= 0.1f;
				color.g -= 0.2f;
				color.b -= 0.1f;
				Gizmos.color = color;
			}
			if (selected || !this.convex)
			{
				for (int i = 0; i < this.points.Length; i++)
				{
					Gizmos.DrawLine(matrix4x.MultiplyPoint3x4(this.points[i]), matrix4x.MultiplyPoint3x4(this.points[(i + 1) % this.points.Length]));
				}
			}
			if (this.convex)
			{
				if (this.convexPoints == null)
				{
					this.RecalcConvex();
				}
				Gizmos.color = (selected ? new Color(0.8901961f, 0.239215687f, 0.08627451f, 1f) : new Color(0.8901961f, 0.239215687f, 0.08627451f, 0.9f));
				if (this.convexPoints != null)
				{
					for (int j = 0; j < this.convexPoints.Length; j++)
					{
						Gizmos.DrawLine(matrix4x.MultiplyPoint3x4(this.convexPoints[j]), matrix4x.MultiplyPoint3x4(this.convexPoints[(j + 1) % this.convexPoints.Length]));
					}
				}
			}
		}
	}
}
