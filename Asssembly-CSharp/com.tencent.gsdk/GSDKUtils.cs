using System;
using UnityEngine;

namespace com.tencent.gsdk
{
	internal class GSDKUtils
	{
		public static bool isDebug = true;

		public static void Logger(string s)
		{
			if (!GSDKUtils.isDebug)
			{
				return;
			}
			MonoBehaviour.print("GSDKTag " + s);
		}
	}
}
