using Pathfinding.ClipperLib;
using Pathfinding.Util;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Pathfinding
{
	[AddComponentMenu("Pathfinding/Navmesh/Navmesh Cut")]
	public class NavmeshCut : MonoBehaviour
	{
		public enum MeshType
		{
			Rectangle,
			Circle,
			CustomMesh
		}

		public class DrawGizmos
		{
			public NavmeshCut.MeshType type;

			public Vector3 center;

			public Vector2 rectangleSize;

			public int circleResolution;

			public float circleRadius;

			public float height;

			public bool useRotation;

			public void OnDrawGismos(Transform tr)
			{
				List<IntPoint> list = ListPool<IntPoint>.Claim();
				Bounds bounds;
				if (this.type == NavmeshCut.MeshType.Circle)
				{
					NavmeshCut.GetContour_Circle(list, tr, this.circleResolution, this.circleRadius, this.center, this.useRotation);
					bounds = NavmeshCut.GetBounds_Circle(tr, this.center, this.circleRadius, this.height, this.useRotation);
				}
				else
				{
					NavmeshCut.GetContour_Rectangle(list, tr, this.rectangleSize, this.center, this.useRotation);
					bounds = NavmeshCut.GetBounds_Rectangle(tr, this.center, this.rectangleSize, this.height, this.useRotation);
				}
				float y = bounds.min.y;
				Vector3 b = Vector3.up * (bounds.max.y - y);
				for (int i = 0; i < list.get_Count(); i++)
				{
					Vector3 vector = NavmeshCut.IntPointToV3(list.get_Item(i));
					vector.y = y;
					Vector3 vector2 = NavmeshCut.IntPointToV3(list.get_Item((i + 1) % list.get_Count()));
					vector2.y = y;
					Gizmos.DrawLine(vector, vector2);
					Gizmos.DrawLine(vector + b, vector2 + b);
					Gizmos.DrawLine(vector, vector + b);
					Gizmos.DrawLine(vector2, vector2 + b);
				}
				ListPool<IntPoint>.Release(list);
			}

			public void OnDrawGizmosSelected(Transform tr)
			{
				Gizmos.color = Color.Lerp(NavmeshCut.GizmoColor, new Color(1f, 1f, 1f, 0.2f), 0.9f);
				Bounds bounds;
				if (this.type == NavmeshCut.MeshType.Circle)
				{
					bounds = NavmeshCut.GetBounds_Circle(tr, this.center, this.circleRadius, this.height, this.useRotation);
				}
				else
				{
					bounds = NavmeshCut.GetBounds_Rectangle(tr, this.center, this.rectangleSize, this.height, this.useRotation);
				}
				Gizmos.DrawCube(bounds.center, bounds.size);
				Gizmos.DrawWireCube(bounds.center, bounds.size);
			}
		}

		private static ListView<NavmeshCut> allCuts = new ListView<NavmeshCut>();

		public NavmeshCut.MeshType type = NavmeshCut.MeshType.Circle;

		public Mesh mesh;

		public Vector2 rectangleSize = new Vector2(1f, 1f);

		public float circleRadius = 1f;

		public int circleResolution = 6;

		public float height = 1f;

		public float meshScale = 1f;

		public Vector3 center;

		public float updateDistance = 0.4f;

		public bool isDual;

		public bool cutsAddedGeom = true;

		public float updateRotationDistance = 10f;

		public bool useRotation;

		private Vector3[][] contours;

		protected Transform tr;

		private Mesh lastMesh;

		private Vector3 lastPosition;

		private Quaternion lastRotation;

		private bool wasEnabled;

		private Bounds bounds;

		private Bounds lastBounds;

		public int campIndex = -1;

		[HideInInspector]
		[NonSerialized]
		public int cutIndex = -1;

		private static readonly Dictionary<VInt2, int> edges = new Dictionary<VInt2, int>();

		private static readonly Dictionary<int, int> pointers = new Dictionary<int, int>();

		public static readonly Color GizmoColor = new Color(0.145098045f, 0.721568644f, 0.9372549f);

		public static event Action<NavmeshCut> OnDestroyCallback
		{
			[MethodImpl(32)]
			add
			{
				NavmeshCut.OnDestroyCallback = (Action<NavmeshCut>)Delegate.Combine(NavmeshCut.OnDestroyCallback, value);
			}
			[MethodImpl(32)]
			remove
			{
				NavmeshCut.OnDestroyCallback = (Action<NavmeshCut>)Delegate.Remove(NavmeshCut.OnDestroyCallback, value);
			}
		}

		public Bounds LastBounds
		{
			get
			{
				return this.lastBounds;
			}
		}

		private static void AddCut(NavmeshCut obj)
		{
			NavmeshCut.allCuts.Add(obj);
		}

		private static void RemoveCut(NavmeshCut obj)
		{
			NavmeshCut.allCuts.Remove(obj);
		}

		public static ListView<NavmeshCut> GetAllInRange(Bounds b)
		{
			ListView<NavmeshCut> listView = new ListView<NavmeshCut>();
			for (int i = 0; i < NavmeshCut.allCuts.Count; i++)
			{
				if (NavmeshCut.allCuts[i].enabled && NavmeshCut.Intersects(b, NavmeshCut.allCuts[i].GetBounds()))
				{
					listView.Add(NavmeshCut.allCuts[i]);
				}
			}
			return listView;
		}

		private static bool Intersects(Bounds b1, Bounds b2)
		{
			Vector3 min = b1.min;
			Vector3 max = b1.max;
			Vector3 min2 = b2.min;
			Vector3 max2 = b2.max;
			return min.x <= max2.x && max.x >= min2.x && min.y <= max2.y && max.y >= min2.y && min.z <= max2.z && max.z >= min2.z;
		}

		public static ListView<NavmeshCut> GetAll()
		{
			return NavmeshCut.allCuts;
		}

		public void Awake()
		{
			NavmeshCut.AddCut(this);
		}

		public void OnEnable()
		{
			this.tr = base.transform;
			this.lastPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
			this.lastRotation = this.tr.rotation;
		}

		public void Check()
		{
			if (this.tr == null)
			{
				this.tr = base.transform;
				this.lastPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
				this.lastRotation = this.tr.rotation;
			}
		}

		public void OnDestroy()
		{
			if (NavmeshCut.OnDestroyCallback != null)
			{
				NavmeshCut.OnDestroyCallback.Invoke(this);
			}
			NavmeshCut.RemoveCut(this);
		}

		public void ForceUpdate()
		{
			this.lastPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		}

		public bool RequiresUpdate()
		{
			return this.wasEnabled != base.enabled || (this.wasEnabled && ((this.tr.position - this.lastPosition).sqrMagnitude > this.updateDistance * this.updateDistance || (this.useRotation && Quaternion.Angle(this.lastRotation, this.tr.rotation) > this.updateRotationDistance)));
		}

		public virtual void UsedForCut()
		{
		}

		public void NotifyUpdated()
		{
			this.wasEnabled = base.enabled;
			if (this.wasEnabled)
			{
				this.lastPosition = this.tr.position;
				this.lastBounds = this.GetBounds();
				if (this.useRotation)
				{
					this.lastRotation = this.tr.rotation;
				}
			}
		}

		private void CalculateMeshContour()
		{
			if (this.mesh == null)
			{
				return;
			}
			NavmeshCut.edges.Clear();
			NavmeshCut.pointers.Clear();
			Vector3[] vertices = this.mesh.vertices;
			int[] triangles = this.mesh.triangles;
			for (int i = 0; i < triangles.Length; i += 3)
			{
				if (Polygon.IsClockwise(vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]]))
				{
					int num = triangles[i];
					triangles[i] = triangles[i + 2];
					triangles[i + 2] = num;
				}
				NavmeshCut.edges.set_Item(new VInt2(triangles[i], triangles[i + 1]), i);
				NavmeshCut.edges.set_Item(new VInt2(triangles[i + 1], triangles[i + 2]), i);
				NavmeshCut.edges.set_Item(new VInt2(triangles[i + 2], triangles[i]), i);
			}
			for (int j = 0; j < triangles.Length; j += 3)
			{
				for (int k = 0; k < 3; k++)
				{
					if (!NavmeshCut.edges.ContainsKey(new VInt2(triangles[j + (k + 1) % 3], triangles[j + k % 3])))
					{
						NavmeshCut.pointers.set_Item(triangles[j + k % 3], triangles[j + (k + 1) % 3]);
					}
				}
			}
			ListLinqView<Vector3[]> listLinqView = new ListLinqView<Vector3[]>();
			List<Vector3> list = ListPool<Vector3>.Claim();
			for (int l = 0; l < vertices.Length; l++)
			{
				if (NavmeshCut.pointers.ContainsKey(l))
				{
					list.Clear();
					int num2 = l;
					do
					{
						int num3 = NavmeshCut.pointers.get_Item(num2);
						if (num3 == -1)
						{
							break;
						}
						NavmeshCut.pointers.set_Item(num2, -1);
						list.Add(vertices[num2]);
						num2 = num3;
						if (num2 == -1)
						{
							goto Block_9;
						}
					}
					while (num2 != l);
					IL_1E2:
					if (list.get_Count() > 0)
					{
						listLinqView.Add(list.ToArray());
						goto IL_236;
					}
					goto IL_236;
					Block_9:
					Debug.LogError("Invalid Mesh '" + this.mesh.name + " in " + base.gameObject.name);
					goto IL_1E2;
				}
				IL_236:;
			}
			ListPool<Vector3>.Release(list);
			this.contours = listLinqView.ToArray();
		}

		public static Bounds GetBounds_Rectangle(Transform tr, Vector3 center, Vector2 rectangleSize, float height, bool useRotation)
		{
			Bounds result;
			if (useRotation)
			{
				Matrix4x4 localToWorldMatrix = tr.localToWorldMatrix;
				result = new Bounds(localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(-rectangleSize.x, -height, -rectangleSize.y) * 0.5f), Vector3.zero);
				result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(rectangleSize.x, -height, -rectangleSize.y) * 0.5f));
				result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(rectangleSize.x, -height, rectangleSize.y) * 0.5f));
				result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(-rectangleSize.x, -height, rectangleSize.y) * 0.5f));
				result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(-rectangleSize.x, height, -rectangleSize.y) * 0.5f));
				result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(rectangleSize.x, height, -rectangleSize.y) * 0.5f));
				result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(rectangleSize.x, height, rectangleSize.y) * 0.5f));
				result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(-rectangleSize.x, height, rectangleSize.y) * 0.5f));
			}
			else
			{
				result = new Bounds(tr.position + tr.lossyScale.Mul(center), tr.lossyScale.Mul(new Vector3(rectangleSize.x, height, rectangleSize.y)));
			}
			return result;
		}

		public static Bounds GetBounds_Circle(Transform tr, Vector3 center, float circleRadius, float height, bool useRotation)
		{
			Bounds result;
			if (useRotation)
			{
				Matrix4x4 localToWorldMatrix = tr.localToWorldMatrix;
				Vector3 size = tr.lossyScale.Mul(new Vector3(circleRadius * 2f, height, circleRadius * 2f));
				result = new Bounds(localToWorldMatrix.MultiplyPoint3x4(center), size);
			}
			else
			{
				Vector3 size2 = tr.lossyScale.Mul(new Vector3(circleRadius * 2f, height, circleRadius * 2f));
				result = new Bounds(tr.position + tr.lossyScale.Mul(center), size2);
			}
			return result;
		}

		public Bounds GetBounds()
		{
			switch (this.type)
			{
			case NavmeshCut.MeshType.Rectangle:
				this.bounds = NavmeshCut.GetBounds_Rectangle(this.tr, this.center, this.rectangleSize, this.height, this.useRotation);
				break;
			case NavmeshCut.MeshType.Circle:
				this.bounds = NavmeshCut.GetBounds_Circle(this.tr, this.center, this.circleRadius, this.height, this.useRotation);
				break;
			case NavmeshCut.MeshType.CustomMesh:
				if (!(this.mesh == null))
				{
					Bounds bounds = this.mesh.bounds;
					if (this.useRotation)
					{
						Matrix4x4 localToWorldMatrix = this.tr.localToWorldMatrix;
						bounds.center *= this.meshScale;
						bounds.size *= this.meshScale;
						this.bounds = new Bounds(localToWorldMatrix.MultiplyPoint3x4(this.center + bounds.center), Vector3.zero);
						Vector3 max = bounds.max;
						Vector3 min = bounds.min;
						this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(max.x, max.y, max.z)));
						this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(min.x, max.y, max.z)));
						this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(min.x, max.y, min.z)));
						this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(max.x, max.y, min.z)));
						this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(max.x, min.y, max.z)));
						this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(min.x, min.y, max.z)));
						this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(min.x, min.y, min.z)));
						this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(max.x, min.y, min.z)));
						Vector3 size = this.bounds.size;
						size.y = Mathf.Max(size.y, this.height * this.tr.lossyScale.y);
						this.bounds.size = size;
					}
					else
					{
						Vector3 size2 = bounds.size * this.meshScale;
						size2.y = Mathf.Max(size2.y, this.height);
						this.bounds = new Bounds(base.transform.position + this.center + bounds.center * this.meshScale, size2);
					}
				}
				break;
			}
			return this.bounds;
		}

		public static void GetContour_Circle(List<IntPoint> buffer, Transform tr, int circleResolution, float circleRadius, Vector3 center, bool useRotation)
		{
			Vector3 a = tr.position;
			if (useRotation)
			{
				Matrix4x4 localToWorldMatrix = tr.localToWorldMatrix;
				for (int i = 0; i < circleResolution; i++)
				{
					buffer.Add(NavmeshCut.V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(Mathf.Cos((float)(i * 2) * 3.14159274f / (float)circleResolution), 0f, Mathf.Sin((float)(i * 2) * 3.14159274f / (float)circleResolution)) * circleRadius)));
				}
			}
			else
			{
				Vector3 zero = Vector3.zero;
				a += center.Mul(tr.lossyScale);
				for (int j = 0; j < circleResolution; j++)
				{
					zero.x = Mathf.Cos((float)(j * 2) * 3.14159274f / (float)circleResolution) * circleRadius * tr.lossyScale.x;
					zero.z = Mathf.Sin((float)(j * 2) * 3.14159274f / (float)circleResolution) * circleRadius * tr.lossyScale.z;
					buffer.Add(NavmeshCut.V3ToIntPoint(a + zero));
				}
			}
		}

		public static void GetContour_Rectangle(List<IntPoint> buffer, Transform tr, Vector2 rectangleSize, Vector3 center, bool useRotation)
		{
			Vector3 a = tr.position;
			if (useRotation)
			{
				Matrix4x4 localToWorldMatrix = tr.localToWorldMatrix;
				buffer.Add(NavmeshCut.V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(-rectangleSize.x, 0f, -rectangleSize.y) * 0.5f)));
				buffer.Add(NavmeshCut.V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(rectangleSize.x, 0f, -rectangleSize.y) * 0.5f)));
				buffer.Add(NavmeshCut.V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(rectangleSize.x, 0f, rectangleSize.y) * 0.5f)));
				buffer.Add(NavmeshCut.V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(center + new Vector3(-rectangleSize.x, 0f, rectangleSize.y) * 0.5f)));
			}
			else
			{
				float num = rectangleSize.x * tr.lossyScale.x;
				float num2 = rectangleSize.y * tr.lossyScale.y;
				a += center.Mul(tr.lossyScale);
				buffer.Add(NavmeshCut.V3ToIntPoint(a + new Vector3(-num, 0f, -num2) * 0.5f));
				buffer.Add(NavmeshCut.V3ToIntPoint(a + new Vector3(num, 0f, -num2) * 0.5f));
				buffer.Add(NavmeshCut.V3ToIntPoint(a + new Vector3(num, 0f, num2) * 0.5f));
				buffer.Add(NavmeshCut.V3ToIntPoint(a + new Vector3(-num, 0f, num2) * 0.5f));
			}
		}

		public void GetContour(List<List<IntPoint>> buffer)
		{
			if (this.circleResolution < 3)
			{
				this.circleResolution = 3;
			}
			Vector3 a = this.tr.position;
			switch (this.type)
			{
			case NavmeshCut.MeshType.Rectangle:
			{
				List<IntPoint> list = ListPool<IntPoint>.Claim();
				NavmeshCut.GetContour_Rectangle(list, this.tr, this.rectangleSize, this.center, this.useRotation);
				buffer.Add(list);
				break;
			}
			case NavmeshCut.MeshType.Circle:
			{
				List<IntPoint> list2 = ListPool<IntPoint>.Claim(this.circleResolution);
				NavmeshCut.GetContour_Circle(list2, this.tr, this.circleResolution, this.circleRadius, this.center, this.useRotation);
				buffer.Add(list2);
				break;
			}
			case NavmeshCut.MeshType.CustomMesh:
				if (this.mesh != this.lastMesh || this.contours == null)
				{
					this.CalculateMeshContour();
					this.lastMesh = this.mesh;
				}
				if (this.contours != null)
				{
					a += this.center;
					bool flag = Vector3.Dot(this.tr.up, Vector3.up) < 0f;
					for (int i = 0; i < this.contours.Length; i++)
					{
						Vector3[] array = this.contours[i];
						List<IntPoint> list3 = ListPool<IntPoint>.Claim(array.Length);
						if (this.useRotation)
						{
							Matrix4x4 localToWorldMatrix = this.tr.localToWorldMatrix;
							for (int j = 0; j < array.Length; j++)
							{
								list3.Add(NavmeshCut.V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(this.center + array[j] * this.meshScale)));
							}
						}
						else
						{
							for (int k = 0; k < array.Length; k++)
							{
								list3.Add(NavmeshCut.V3ToIntPoint(a + array[k] * this.meshScale));
							}
						}
						if (flag)
						{
							list3.Reverse();
						}
						buffer.Add(list3);
					}
				}
				break;
			}
		}

		public static IntPoint V3ToIntPoint(Vector3 p)
		{
			VInt3 vInt = (VInt3)p;
			return new IntPoint((long)vInt.x, (long)vInt.z);
		}

		public static Vector3 IntPointToV3(IntPoint p)
		{
			VInt3 ob = new VInt3((int)p.X, 0, (int)p.Y);
			return (Vector3)ob;
		}

		public void OnDrawGizmos()
		{
			if (this.tr == null)
			{
				this.tr = base.transform;
			}
			List<List<IntPoint>> list = ListPool<List<IntPoint>>.Claim();
			this.GetContour(list);
			Gizmos.color = NavmeshCut.GizmoColor;
			Bounds bounds = this.GetBounds();
			float y = bounds.min.y;
			Vector3 b = Vector3.up * (bounds.max.y - y);
			for (int i = 0; i < list.get_Count(); i++)
			{
				List<IntPoint> list2 = list.get_Item(i);
				for (int j = 0; j < list2.get_Count(); j++)
				{
					Vector3 vector = NavmeshCut.IntPointToV3(list2.get_Item(j));
					vector.y = y;
					Vector3 vector2 = NavmeshCut.IntPointToV3(list2.get_Item((j + 1) % list2.get_Count()));
					vector2.y = y;
					Gizmos.DrawLine(vector, vector2);
					Gizmos.DrawLine(vector + b, vector2 + b);
					Gizmos.DrawLine(vector, vector + b);
					Gizmos.DrawLine(vector2, vector2 + b);
				}
			}
			ListPool<List<IntPoint>>.Release(list);
		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.Lerp(NavmeshCut.GizmoColor, new Color(1f, 1f, 1f, 0.2f), 0.9f);
			Bounds bounds = this.GetBounds();
			Gizmos.DrawCube(bounds.center, bounds.size);
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}
	}
}
