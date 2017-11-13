using System;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal delegate void ApolloSendMessageDelegate(ulong objectId, [MarshalAs(20)] string function, [MarshalAs(20)] string param);
}
