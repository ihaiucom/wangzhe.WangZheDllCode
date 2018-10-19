using System;
using System.Collections.Generic;

namespace TMPro
{
	[Serializable]
	public class KerningTable
	{
		public List<KerningPair> kerningPairs;

		public KerningTable()
		{
			this.kerningPairs = new List<KerningPair>();
		}

		public int AddKerningPair(int left, int right, float offset)
		{
			int num = this.kerningPairs.FindIndex((KerningPair item) => item.AscII_Left == left && item.AscII_Right == right);
			if (num == -1)
			{
				this.kerningPairs.Add(new KerningPair(left, right, offset));
				return 0;
			}
			return -1;
		}

		public void RemoveKerningPair(int left, int right)
		{
			int num = this.kerningPairs.FindIndex((KerningPair item) => item.AscII_Left == left && item.AscII_Right == right);
			if (num != -1)
			{
				this.kerningPairs.RemoveAt(num);
			}
		}

		public void RemoveKerningPair(int index)
		{
			this.kerningPairs.RemoveAt(index);
		}

		public void SortKerningPairs()
		{
		}
	}
}
