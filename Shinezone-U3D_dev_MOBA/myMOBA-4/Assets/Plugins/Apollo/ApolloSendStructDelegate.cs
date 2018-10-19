using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal delegate void ApolloSendStructDelegate(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, IntPtr param);
}
