using System;

namespace TMPro
{
	public struct KerningPairKey
	{
		public int ascii_Left;

		public int ascii_Right;

		public int key;

		public KerningPairKey(int ascii_left, int ascii_right)
		{
			this.ascii_Left = ascii_left;
			this.ascii_Right = ascii_right;
			this.key = (ascii_right << 16) + ascii_left;
		}
	}
}
