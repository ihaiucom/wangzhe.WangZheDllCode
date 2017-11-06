using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class stStateHeatmapStorage
	{
		public float[][] displayRates;

		public List<Vector2> myVertex;

		public Vector2 defaultCtrlRect;

		public Vector2 mapOrgRect;

		public float maxRate;

		public int crossLen;

		public float radius = 0.0025f;

		public float intensity = 0.1f;

		public float workRadius = 0.1f;

		public uint objID;

		public stStateHeatmapStorage()
		{
			this.myVertex = new List<Vector2>();
			this.defaultCtrlRect = default(Vector2);
			this.mapOrgRect = default(Vector2);
		}

		public void Reset()
		{
			this.displayRates = null;
			this.myVertex.Clear();
			this.maxRate = 0f;
		}
	}
}
