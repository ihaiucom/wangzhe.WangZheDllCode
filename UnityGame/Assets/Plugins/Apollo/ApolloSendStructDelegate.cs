using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal delegate void ApolloSendStructDelegate(ulong objectId, [MarshalAs(20)] string function, IntPtr param);
}
