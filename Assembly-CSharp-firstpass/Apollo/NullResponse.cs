using ApolloTdr;
using System;

namespace Apollo
{
	public class NullResponse : IUnpackable
	{
		public TdrError.ErrorType unpackTLV(ref byte[] buffer, int size, ref int used)
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}
	}
}
