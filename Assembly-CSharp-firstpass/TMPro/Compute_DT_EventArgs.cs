using System;
using UnityEngine;

namespace TMPro
{
	public class Compute_DT_EventArgs
	{
		public Compute_DistanceTransform_EventTypes EventType;

		public float ProgressPercentage;

		public Color[] Colors;

		public Compute_DT_EventArgs(Compute_DistanceTransform_EventTypes type, float progress)
		{
			this.EventType = type;
			this.ProgressPercentage = progress;
		}

		public Compute_DT_EventArgs(Compute_DistanceTransform_EventTypes type, Color[] colors)
		{
			this.EventType = type;
			this.Colors = colors;
		}
	}
}
