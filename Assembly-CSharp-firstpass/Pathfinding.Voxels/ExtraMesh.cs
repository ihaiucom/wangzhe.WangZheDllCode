using System;
using UnityEngine;

namespace Pathfinding.Voxels
{
	public struct ExtraMesh
	{
		public MeshFilter original;

		public int area;

		public Vector3[] vertices;

		public int[] triangles;

		public Bounds bounds;

		public Matrix4x4 matrix;

		public string name;

		public ExtraMesh(Vector3[] v, int[] t, Bounds b)
		{
			this.matrix = Matrix4x4.identity;
			this.vertices = v;
			this.triangles = t;
			this.bounds = b;
			this.original = null;
			this.name = null;
			this.area = 0;
		}

		public ExtraMesh(Vector3[] v, int[] t, Bounds b, Matrix4x4 matrix)
		{
			this.matrix = matrix;
			this.vertices = v;
			this.triangles = t;
			this.bounds = b;
			this.original = null;
			this.name = null;
			this.area = 0;
		}

		public void RecalculateBounds()
		{
			Bounds bounds = new Bounds(this.matrix.MultiplyPoint3x4(this.vertices[0]), Vector3.zero);
			for (int i = 1; i < this.vertices.Length; i++)
			{
				bounds.Encapsulate(this.matrix.MultiplyPoint3x4(this.vertices[i]));
			}
			this.bounds = bounds;
		}
	}
}
