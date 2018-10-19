using System;

namespace Apollo
{
	public interface IApolloHttpClient
	{
		IApolloHttpRequest CreateRequest(IApolloConnector connector);

		ApolloResult ReleaseRequest(IApolloHttpRequest request);

		void Update();
	}
}
