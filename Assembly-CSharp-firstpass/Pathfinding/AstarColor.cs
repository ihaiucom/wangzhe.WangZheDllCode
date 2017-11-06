using System;
using UnityEngine;

namespace Pathfinding
{
	[Serializable]
	public class AstarColor
	{
		public Color _NodeConnection;

		public Color _UnwalkableNode;

		public Color _BoundsHandles;

		public Color _ConnectionLowLerp;

		public Color _ConnectionHighLerp;

		public Color _MeshEdgeColor;

		public Color _MeshColor;

		public Color[] _AreaColors;

		public static Color NodeConnection = new Color(1f, 1f, 1f, 0.9f);

		public static Color UnwalkableNode = new Color(1f, 0f, 0f, 0.5f);

		public static Color BoundsHandles = new Color(0.29f, 0.454f, 0.741f, 0.9f);

		public static Color ConnectionLowLerp = new Color(0f, 1f, 0f, 0.5f);

		public static Color ConnectionHighLerp = new Color(1f, 0f, 0f, 0.5f);

		public static Color MeshEdgeColor = new Color(0f, 0f, 0f, 0.5f);

		public static Color MeshColor = new Color(0f, 0f, 0f, 0.5f);

		private static Color[] AreaColors;

		public AstarColor()
		{
			this._NodeConnection = new Color(1f, 1f, 1f, 0.9f);
			this._UnwalkableNode = new Color(1f, 0f, 0f, 0.5f);
			this._BoundsHandles = new Color(0.29f, 0.454f, 0.741f, 0.9f);
			this._ConnectionLowLerp = new Color(0f, 1f, 0f, 0.5f);
			this._ConnectionHighLerp = new Color(1f, 0f, 0f, 0.5f);
			this._MeshEdgeColor = new Color(0f, 0f, 0f, 0.5f);
			this._MeshColor = new Color(0.125f, 0.686f, 0f, 0.19f);
		}

		public static Color GetAreaColor(uint area)
		{
			if (AstarColor.AreaColors == null || (ulong)area >= (ulong)((long)AstarColor.AreaColors.Length))
			{
				return AstarMath.IntToColor((int)area, 1f);
			}
			return AstarColor.AreaColors[(int)area];
		}

		public void OnEnable()
		{
			AstarColor.NodeConnection = this._NodeConnection;
			AstarColor.UnwalkableNode = this._UnwalkableNode;
			AstarColor.BoundsHandles = this._BoundsHandles;
			AstarColor.ConnectionLowLerp = this._ConnectionLowLerp;
			AstarColor.ConnectionHighLerp = this._ConnectionHighLerp;
			AstarColor.MeshEdgeColor = this._MeshEdgeColor;
			AstarColor.MeshColor = this._MeshColor;
			AstarColor.AreaColors = this._AreaColors;
		}
	}
}
