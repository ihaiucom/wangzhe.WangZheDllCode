using System;
using System.Runtime.InteropServices;

namespace com.tencent.pandora
{
	public class MinizLib
	{
		public delegate void MinizHandler(int encodedDataLen, [MarshalAs(20)] string encodedData);

		private static string lastMinizData = string.Empty;

		public static string Compress(int rawDataLen, string rawData)
		{
			MinizLib.PandoraNet_RegisterMinizHandler(new MinizLib.MinizHandler(MinizLib.MinizCallback));
			if (MinizLib.PandoraNet_Compress(rawDataLen, rawData) == 0)
			{
				return MinizLib.lastMinizData;
			}
			return string.Empty;
		}

		public static string UnCompress(int encodedCompressedDataLen, string encodedCompressedData)
		{
			MinizLib.PandoraNet_RegisterMinizHandler(new MinizLib.MinizHandler(MinizLib.MinizCallback));
			if (MinizLib.PandoraNet_UnCompress(encodedCompressedDataLen, encodedCompressedData) == 0)
			{
				return MinizLib.lastMinizData;
			}
			return string.Empty;
		}

		[MonoPInvokeCallback(typeof(MinizLib.MinizHandler))]
		public static void MinizCallback(int encodedDataLen, [MarshalAs(20)] string encodedData)
		{
			MinizLib.lastMinizData = encodedData;
		}

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern void PandoraNet_RegisterMinizHandler([MarshalAs(38)] MinizLib.MinizHandler handler);

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern int PandoraNet_Compress(int rawDataLen, [MarshalAs(20)] string rawData);

		[DllImport("PandoraNet", CallingConvention = CallingConvention.Cdecl)]
		public static extern int PandoraNet_UnCompress(int encodedCompressedDataLen, [MarshalAs(20)] string encodedCompressedData);
	}
}
