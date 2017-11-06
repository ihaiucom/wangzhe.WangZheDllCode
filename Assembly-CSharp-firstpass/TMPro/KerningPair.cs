using System;

namespace TMPro
{
	[Serializable]
	public class KerningPair
	{
		public int AscII_Left;

		public int AscII_Right;

		public float XadvanceOffset;

		public KerningPair(int left, int right, float offset)
		{
			this.AscII_Left = left;
			this.AscII_Right = right;
			this.XadvanceOffset = offset;
		}
	}
}
