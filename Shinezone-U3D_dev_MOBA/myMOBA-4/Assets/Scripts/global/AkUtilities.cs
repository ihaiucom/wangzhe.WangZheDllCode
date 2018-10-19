using System;
using System.Text;

public class AkUtilities
{
	public class ShortIDGenerator
	{
		private const uint s_prime32 = 16777619u;

		private const uint s_offsetBasis32 = 2166136261u;

		private static byte s_hashSize;

		private static uint s_mask;

		public static byte HashSize
		{
			get
			{
				return AkUtilities.ShortIDGenerator.s_hashSize;
			}
			set
			{
				AkUtilities.ShortIDGenerator.s_hashSize = value;
				AkUtilities.ShortIDGenerator.s_mask = (1u << (int)AkUtilities.ShortIDGenerator.s_hashSize) - 1u;
			}
		}

		static ShortIDGenerator()
		{
			AkUtilities.ShortIDGenerator.HashSize = 32;
		}

		public static uint Compute(string in_name)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(in_name.ToLower());
			uint num = 2166136261u;
			for (int i = 0; i < bytes.Length; i++)
			{
				num *= 16777619u;
				num ^= (uint)bytes[i];
			}
			if (AkUtilities.ShortIDGenerator.s_hashSize == 32)
			{
				return num;
			}
			return num >> (int)AkUtilities.ShortIDGenerator.s_hashSize ^ (num & AkUtilities.ShortIDGenerator.s_mask);
		}
	}
}
