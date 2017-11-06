using System;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class FrameRandom
	{
		private const uint maxShort = 65536u;

		private const uint multiper = 1194211693u;

		private const uint addValue = 12345u;

		private static uint nSeed = (uint)UnityEngine.Random.Range(32767, 2147483647);

		public static uint callNum = 0u;

		public static int GetSeed()
		{
			return (int)FrameRandom.nSeed;
		}

		public static void ResetSeed(uint seed)
		{
			FrameRandom.nSeed = seed;
			FrameRandom.callNum = 0u;
		}

		public static ushort Random(uint nMax)
		{
			FrameRandom.callNum += 1u;
			DebugHelper.Assert(nMax > 0u);
			if (nMax == 0u)
			{
				return 0;
			}
			FrameRandom.nSeed = FrameRandom.nSeed * 1194211693u + 12345u;
			return (ushort)((FrameRandom.nSeed >> 16) % nMax);
		}

		public static float fRandom()
		{
			FrameRandom.callNum += 1u;
			return (float)FrameRandom.Random(65536u) / 65536f;
		}
	}
}
