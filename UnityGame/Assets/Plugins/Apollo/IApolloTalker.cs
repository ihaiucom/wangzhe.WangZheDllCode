using ApolloTdr;
using System;

namespace Apollo
{
	public interface IApolloTalker
	{
		bool AutoUpdate
		{
			get;
			set;
		}

		void Update(int num);

		ApolloResult SendMessage(IPackable request);

		ApolloResult RegisterMessage<TResp>(TalkerMessageWithoutReceiptHandler<TResp> handler) where TResp : IUnpackable;

		ApolloResult Send<TResp>(IPackable request, TalkerMessageHandler<TResp> handler, object context, float timeout) where TResp : IUnpackable;

		ApolloResult Send(IPackable request);

		ApolloResult Send(byte[] data, int usedSize);

		ApolloResult Register<TResp>(TalkerMessageWithoutReceiptHandler<TResp> handler) where TResp : IUnpackable;

		ApolloResult Register<TResp, TReceipt>(TalkerMessageWithReceiptHandler<TResp, TReceipt> handler) where TResp : IUnpackable where TReceipt : IPackable;

		ApolloResult Register(RawMessageHandler handler);

		void Unregister(string cmd);

		void Unregister<TResp>();

		void UnregisterRawMessageHandler();
	}
}
