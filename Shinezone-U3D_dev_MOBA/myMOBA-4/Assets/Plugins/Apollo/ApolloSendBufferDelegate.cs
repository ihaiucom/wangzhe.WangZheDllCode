using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal delegate void ApolloSendBufferDelegate(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, IntPtr buffer, int size);
}
