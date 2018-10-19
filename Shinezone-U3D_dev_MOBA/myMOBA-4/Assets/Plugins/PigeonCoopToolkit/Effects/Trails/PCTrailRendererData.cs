using System;
using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails
{
	[Serializable]
	public class PCTrailRendererData
	{
		public Material TrailMaterial;

		public float Lifetime = 1f;

		public bool UsingSimpleSize;

		public float SimpleSizeOverLifeStart;

		public float SimpleSizeOverLifeEnd;

		public AnimationCurve SizeOverLife = new AnimationCurve();

		public bool UsingSimpleColor;

		public Color SimpleColorOverLifeStart;

		public Color SimpleColorOverLifeEnd;

		public Gradient ColorOverLife;

		public bool StretchSizeToFit;

		public bool StretchColorToFit;

		public float MaterialTileLength;

		public bool UseForwardOverride;

		public Vector3 ForwardOverride;

		public bool ForwardOverrideRelative;
	}
}
