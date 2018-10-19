using ApolloTdr;
using System;

namespace Apollo
{
	internal delegate void TalkerMessageHandler(object request, TalkerEventArgs e);
	public delegate void TalkerMessageHandler<T>(object request, TalkerEventArgs<T> e) where T : IUnpackable;
}
