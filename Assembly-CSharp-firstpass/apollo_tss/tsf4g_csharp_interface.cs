using ApolloTdr;
using System;

namespace apollo_tss
{
	public interface tsf4g_csharp_interface : IPackable, IUnpackable
	{
		TdrError.ErrorType construct();

		TdrError.ErrorType packTLV(ref TdrWriteBuf destBuf, bool useVarInt);

		TdrError.ErrorType unpackTLV(ref TdrReadBuf srcBuf, int length, bool useVarInt);
	}
}
