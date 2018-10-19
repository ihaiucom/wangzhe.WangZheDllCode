using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal delegate void ApolloSendMessageDelegate(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, [MarshalAs(UnmanagedType.LPStr)] string param);
}
