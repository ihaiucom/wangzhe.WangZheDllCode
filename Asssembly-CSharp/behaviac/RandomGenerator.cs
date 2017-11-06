using System;

namespace behaviac
{
	internal class RandomGenerator
	{
		private static RandomGenerator Instance;

		private uint m_seed;

		protected RandomGenerator(uint seed)
		{
			this.m_seed = seed;
		}

		public static RandomGenerator GetInstance()
		{
			if (RandomGenerator.Instance == null)
			{
				RandomGenerator.Instance = new RandomGenerator(0u);
			}
			return RandomGenerator.Instance;
		}

		public float GetRandom()
		{
			this.m_seed = 214013u * this.m_seed + 2531011u;
			return this.m_seed * 2.32830644E-10f;
		}

		public float InRange(float low, float high)
		{
			float random = this.GetRandom();
			return random * (high - low) + low;
		}

		public void SetSeed(uint seed)
		{
			this.m_seed = seed;
		}

		~RandomGenerator()
		{
		}
	}
}
