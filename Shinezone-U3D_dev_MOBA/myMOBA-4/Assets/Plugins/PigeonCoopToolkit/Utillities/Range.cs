using System;

namespace PigeonCoopToolkit.Utillities
{
	[Serializable]
	public class Range
	{
		public float Min;

		public float Max;

		public bool WithinRange(float value)
		{
			return this.Min <= value && this.Max >= value;
		}
	}
}
