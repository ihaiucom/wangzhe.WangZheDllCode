using ApolloTdr;
using System;

namespace apollo_http_object
{
	public interface tsf4g_csharp_interface : IPackable, IUnpackable
	{
		TdrError.ErrorType construct();

		TdrError.ErrorType packTLV(ref TdrWriteBuf destBuf, bool useVarInt);

		TdrError.ErrorType unpackTLV(ref TdrReadBuf srcBuf, int length, bool useVarInt);

		TdrError.ErrorType visualize(ref string buffer, int indent, char separator);

		TdrError.ErrorType visualize(ref TdrVisualBuf destBuf, int indent, char separator);
	}
}
