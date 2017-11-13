using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	public class ApolloAndroidResTools
	{
		public static int GetResID(string resName, string typeName)
		{
			if (!ApolloAndroidResType.Valied(typeName))
			{
				return 0;
			}
			return ApolloAndroidResTools.apollo_utils_get_res_id(resName, typeName);
		}

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern int apollo_utils_get_res_id([MarshalAs(20)] string resName, [MarshalAs(20)] string typeName);
	}
}
