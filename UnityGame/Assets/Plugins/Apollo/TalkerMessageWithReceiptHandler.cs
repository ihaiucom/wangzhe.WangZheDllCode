using ApolloTdr;
using System;

namespace Apollo
{
	internal delegate void TalkerMessageWithReceiptHandler(IUnpackable resp, ref IPackable receipt);
	public delegate void TalkerMessageWithReceiptHandler<TResp, TReceipt>(TResp resp, ref TReceipt receipt) where TResp : IUnpackable where TReceipt : IPackable;
}
