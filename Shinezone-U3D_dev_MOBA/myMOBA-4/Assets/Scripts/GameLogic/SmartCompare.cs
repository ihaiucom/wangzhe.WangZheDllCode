using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public abstract class SmartCompare
	{
		public static bool Compare<T>(T InFirst, T InSecond, int CompareOperation)
		{
			int num = Comparer<T>.Default.Compare(InFirst, InSecond);
			switch (CompareOperation)
			{
			case 1:
				return num < 0;
			case 2:
				return num <= 0;
			case 3:
				return num == 0;
			case 4:
				return num > 0;
			case 5:
				return num >= 0;
			default:
				DebugHelper.Assert(false, "what the fuck?");
				return false;
			}
		}
	}
}
