using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal delegate void ApolloSendResultBufferDelegate(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, int result, IntPtr buffer, int size);
}
