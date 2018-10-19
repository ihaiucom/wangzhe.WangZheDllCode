using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class NavmeshAdd : MonoBehaviour
	{
		public enum MeshType
		{
			Rectangle,
			CustomMesh
		}

		private static List<NavmeshAdd> allCuts = new List<NavmeshAdd>();

		public NavmeshAdd.MeshType type;

		public Mesh mesh;

		private Vector3[] verts;

		private int[] tris;

		public Vector2 rectangleSize = new Vector2(1f, 1f);

		public float meshScale = 1f;

		public Vector3 center;

		private Bounds bounds;

		public bool useRotation;

		protected Transform tr;

		public static readonly Color GizmoColor = new Color(0.368627459f, 0.9372549f, 0.145098045f);

		public Vector3 Center
		{
			get
			{
				return this.tr.position + ((!this.useRotation) ? this.center : this.tr.TransformPoint(this.center));
			}
		}

		private static void Add(NavmeshAdd obj)
		{
			NavmeshAdd.allCuts.Add(obj);
		}

		private static void Remove(NavmeshAdd obj)
		{
			NavmeshAdd.allCuts.Remove(obj);
		}

		public static List<NavmeshAdd> GetAllInRange(Bounds b)
		{
			List<NavmeshAdd> list = ListPool<NavmeshAdd>.Claim();
			for (int i = 0; i < NavmeshAdd.allCuts.Count; i++)
			{
				if (NavmeshAdd.allCuts[i].enabled && NavmeshAdd.Intersects(b, NavmeshAdd.allCuts[i].GetBounds()))
				{
					list.Add(NavmeshAdd.allCuts[i]);
				}
			}
			return list;
		}

		private static bool Intersects(Bounds b1, Bounds b2)
		{
			Vector3 min = b1.min;
			Vector3 max = b1.max;
			Vector3 min2 = b2.min;
			Vector3 max2 = b2.max;
			return min.x <= max2.x && max.x >= min2.x && min.z <= max2.z && max.z >= min2.z;
		}

		public static List<NavmeshAdd> GetAll()
		{
			return NavmeshAdd.allCuts;
		}

		public void Awake()
		{
			NavmeshAdd.Add(this);
		}

		public void OnEnable()
		{
			this.tr = base.transform;
		}

		public void OnDestroy()
		{
			NavmeshAdd.Remove(this);
		}

		[ContextMenu("Rebuild Mesh")]
		public void RebuildMesh()
		{
			if (this.type == NavmeshAdd.MeshType.CustomMesh)
			{
				if (this.mesh == null)
				{
					this.verts = null;
					this.tris = null;
				}
				else
				{
					this.verts = this.mesh.vertices;
					this.tris = this.mesh.triangles;
				}
			}
			else
			{
				if (this.verts == null || this.verts.Length != 4 || this.tris == null || this.tris.Length != 6)
				{
					this.verts = new Vector3[4];
					this.tris = new int[6];
				}
				this.tris[0] = 0;
				this.tris[1] = 1;
				this.tris[2] = 2;
				this.tris[3] = 0;
				this.tris[4] = 2;
				this.tris[5] = 3;
				this.verts[0] = new Vector3(-this.rectangleSize.x * 0.5f, 0f, -this.rectangleSize.y * 0.5f);
				this.verts[1] = new Vector3(this.rectangleSize.x * 0.5f, 0f, -this.rectangleSize.y * 0.5f);
				this.verts[2] = new Vector3(this.rectangleSize.x * 0.5f, 0f, this.rectangleSize.y * 0.5f);
				this.verts[3] = new Vector3(-this.rectangleSize.x * 0.5f, 0f, this.rectangleSize.y * 0.5f);
			}
		}

		public Bounds GetBounds()
		{
			NavmeshAdd.MeshType meshType = this.type;
			if (meshType != NavmeshAdd.MeshType.Rectangle)
			{
				if (meshType == NavmeshAdd.MeshType.CustomMesh)
				{
					if (!(this.mesh == null))
					{
						Bounds bounds = this.mesh.bounds;
						if (this.useRotation)
						{
							Matrix4x4 matrix4x = Matrix4x4.TRS(this.tr.position, this.tr.rotation, Vector3.one * this.meshScale);
							this.bounds = new Bounds(matrix4x.MultiplyPoint3x4(this.center + bounds.center), Vector3.zero);
							Vector3 max = bounds.max;
							Vector3 min = bounds.min;
							this.bounds.Encapsulate(matrix4x.MultiplyPoint3x4(this.center + new Vector3(max.x, min.y, max.z)));
							this.bounds.Encapsulate(matrix4x.MultiplyPoint3x4(this.center + new Vector3(min.x, min.y, max.z)));
							this.bounds.Encapsulate(matrix4x.MultiplyPoint3x4(this.center + new Vector3(min.x, max.y, min.z)));
							this.bounds.Encapsulate(matrix4x.MultiplyPoint3x4(this.center + new Vector3(max.x, max.y, min.z)));
						}
						else
						{
							Vector3 size = bounds.size * this.meshScale;
							this.bounds = new Bounds(base.transform.position + this.center + bounds.center * this.meshScale, size);
						}
					}
				}
			}
			else if (this.useRotation)
			{
				Matrix4x4 matrix4x2 = Matrix4x4.TRS(this.tr.position, this.tr.rotation, Vector3.one);
				this.bounds = new Bounds(matrix4x2.MultiplyPoint3x4(this.center + new Vector3(-this.rectangleSize.x, 0f, -this.rectangleSize.y) * 0.5f), Vector3.zero);
				this.bounds.Encapsulate(matrix4x2.MultiplyPoint3x4(this.center + new Vector3(this.rectangleSize.x, 0f, -this.rectangleSize.y) * 0.5f));
				this.bounds.Encapsulate(matrix4x2.MultiplyPoint3x4(this.center + new Vector3(this.rectangleSize.x, 0f, this.rectangleSize.y) * 0.5f));
				this.bounds.Encapsulate(matrix4x2.MultiplyPoint3x4(this.center + new Vector3(-this.rectangleSize.x, 0f, this.rectangleSize.y) * 0.5f));
			}
			else
			{
				this.bounds = new Bounds(this.tr.position + this.center, new Vector3(this.rectangleSize.x, 0f, this.rectangleSize.y));
			}
			return this.bounds;
		}

		public void GetMesh(VInt3 offset, ref VInt3[] vbuffer, out int[] tbuffer)
		{
			if (this.verts == null)
			{
				this.RebuildMesh();
			}
			if (this.verts == null)
			{
				tbuffer = new int[0];
				return;
			}
			if (vbuffer == null || vbuffer.Length < this.verts.Length)
			{
				vbuffer = new VInt3[this.verts.Length];
			}
			tbuffer = this.tris;
			if (this.useRotation)
			{
				Matrix4x4 matrix4x = Matrix4x4.TRS(this.tr.position + this.center, this.tr.rotation, this.tr.localScale * this.meshScale);
				for (int i = 0; i < this.verts.Length; i++)
				{
					vbuffer[i] = offset + (VInt3)matrix4x.MultiplyPoint3x4(this.verts[i]);
				}
			}
			else
			{
				Vector3 a = this.tr.position + this.center;
				for (int j = 0; j < this.verts.Length; j++)
				{
					vbuffer[j] = offset + (VInt3)(a + this.verts[j] * this.meshScale);
				}
			}
		}
	}
}
