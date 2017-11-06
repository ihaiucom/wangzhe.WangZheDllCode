using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	public sealed class NoneAccountService
	{
		public static ApolloResult Initialize(NoneAccountInitInfo initInfo)
		{
			if (initInfo == null)
			{
				ADebug.LogError("NoneAccountService initInfo == null");
				return ApolloResult.InvalidArgument;
			}
			byte[] array = null;
			if (initInfo.Encode(out array) && array != null)
			{
				NoneAccountService.apollo_none_account_initialize(array, array.Length);
				return ApolloResult.Success;
			}
			ADebug.LogError("NoneAccountService Encode error!");
			return ApolloResult.InnerError;
		}

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool apollo_none_account_initialize([MarshalAs(42)] byte[] data, int len);
	}
}
