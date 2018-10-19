using System;
using System.Diagnostics;

namespace ApolloTdr
{
	public class TdrDebug
	{
		public static void tdrTrace()
		{
			StackTrace stackTrace = new StackTrace(true);
			for (int i = 1; i < stackTrace.FrameCount; i++)
			{
				if (stackTrace.GetFrame(i).GetFileName() != null)
				{
					Console.WriteLine("TSF4G_TRACE:  " + stackTrace.GetFrame(i).ToString());
				}
			}
		}
	}
}
