using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal delegate void ApolloSendResultDelegate(ulong objectId, [MarshalAs(20)] string function, int result);
}
