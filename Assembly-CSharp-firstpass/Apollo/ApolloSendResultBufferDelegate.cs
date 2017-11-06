using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal delegate void ApolloSendResultBufferDelegate(ulong objectId, [MarshalAs(20)] string function, int result, IntPtr buffer, int size);
}
