using System;

namespace Assets.Scripts.GameSystem
{
	public static class SimpleNumericString
	{
		private static string[] RawString = new string[240];

		private static bool bIsInitialized = false;

		private static void Init()
		{
			for (int i = 0; i < SimpleNumericString.RawString.Length; i++)
			{
				SimpleNumericString.RawString[i] = string.Format("{0}", i);
			}
		}

		public static string GetNumeric(int InValue)
		{
			if (!SimpleNumericString.bIsInitialized)
			{
				SimpleNumericString.bIsInitialized = true;
				SimpleNumericString.Init();
			}
			if (InValue >= 0 && InValue < SimpleNumericString.RawString.Length)
			{
				return SimpleNumericString.RawString[InValue];
			}
			return string.Format("{0}", InValue);
		}
	}
}
