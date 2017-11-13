using System;

namespace Assets.Scripts.GameLogic
{
	internal class CampData
	{
		public int Golds;

		public int[] PositiveGolds = new int[3];

		public int[] NegativeGolds = new int[3];

		public void Clear()
		{
			this.Golds = 0;
			for (int i = 0; i < this.NegativeGolds.Length; i++)
			{
				this.NegativeGolds[i] = 0;
				this.PositiveGolds[i] = 0;
			}
		}
	}
}
