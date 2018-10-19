using PigeonCoopToolkit.Utillities;
using System;
using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails
{
	public class PCTrail : IDisposable
	{
		public CircularBuffer<PCTrailPoint> Points;

		public Mesh Mesh;

		public Vector3[] verticies;

		public Vector3[] normals;

		public Vector2[] uvs;

		public Color[] colors;

		public int[] indicies;

		public int activePointCount;

		public bool IsActiveTrail;

		public int NumPoints;

		public PCTrail(int numPoints)
		{
			this.Mesh = new Mesh();
			this.Mesh.MarkDynamic();
			this.verticies = new Vector3[2 * numPoints];
			this.normals = new Vector3[2 * numPoints];
			this.uvs = new Vector2[2 * numPoints];
			this.colors = new Color[2 * numPoints];
			this.indicies = new int[2 * numPoints * 3];
			this.Points = new CircularBuffer<PCTrailPoint>(numPoints);
			this.NumPoints = numPoints;
		}

		public void Dispose()
		{
			if (this.Mesh != null)
			{
				if (Application.isEditor)
				{
					UnityEngine.Object.DestroyImmediate(this.Mesh, true);
				}
				else
				{
					UnityEngine.Object.Destroy(this.Mesh);
				}
			}
			this.Points.Clear();
			this.Points = null;
		}
	}
}
