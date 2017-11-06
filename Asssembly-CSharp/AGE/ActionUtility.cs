using System;
using UnityEngine;

namespace AGE
{
	public static class ActionUtility
	{
		public static int SecToMs(float s)
		{
			return Mathf.RoundToInt(s * 1000f);
		}

		public static float MsToSec(int ms)
		{
			return (float)ms * 0.001f;
		}
	}
}
