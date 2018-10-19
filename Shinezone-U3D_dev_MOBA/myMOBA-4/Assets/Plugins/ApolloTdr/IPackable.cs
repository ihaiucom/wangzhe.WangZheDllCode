using System;

namespace ApolloTdr
{
	public interface IPackable
	{
		TdrError.ErrorType packTLV(ref byte[] buffer, int size, ref int used, bool useVarInt);
	}
}
