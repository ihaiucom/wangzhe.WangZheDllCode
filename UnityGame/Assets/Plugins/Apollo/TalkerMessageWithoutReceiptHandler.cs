using ApolloTdr;
using System;

namespace Apollo
{
	internal delegate void TalkerMessageWithoutReceiptHandler(IUnpackable resp);
	public delegate void TalkerMessageWithoutReceiptHandler<TResp>(TResp resp) where TResp : IUnpackable;
}
