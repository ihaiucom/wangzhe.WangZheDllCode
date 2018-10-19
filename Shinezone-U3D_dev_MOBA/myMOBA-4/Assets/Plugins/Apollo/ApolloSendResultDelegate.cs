using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal delegate void ApolloSendResultDelegate(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, int result);
}
