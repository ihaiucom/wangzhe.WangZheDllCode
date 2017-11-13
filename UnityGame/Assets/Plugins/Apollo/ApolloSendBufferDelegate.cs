using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal delegate void ApolloSendBufferDelegate(ulong objectId, [MarshalAs(20)] string function, IntPtr buffer, int size);
}
