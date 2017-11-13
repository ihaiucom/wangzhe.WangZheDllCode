using System;

namespace Assets.Scripts.GameLogic
{
	internal struct RangeConfig
	{
		public int MinValue;

		public int MaxValue;

		public int Attenuation;

		public static int Clamp(int value, int min, int max)
		{
			return (value < min) ? min : ((value > max) ? max : value);
		}

		public bool Intersect(int InBase, int InValue)
		{
			return InValue >= InBase + this.MinValue && InValue < InBase + this.MaxValue;
		}

		public bool Intersect(int InBase, int InMin, int InMax, out int OutMin, out int OutMax)
		{
			OutMin = (OutMax = 0);
			int num = this.MinValue + InBase;
			int num2 = this.MaxValue + InBase;
			if (InMax < num)
			{
				return false;
			}
			if (InMin > num2)
			{
				return false;
			}
			OutMin = RangeConfig.Clamp(InMin, num, num2);
			OutMax = RangeConfig.Clamp(InMax, num, num2);
			return true;
		}
	}
}
