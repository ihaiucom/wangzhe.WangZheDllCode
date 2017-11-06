using System;

namespace Apollo
{
	public class ApolloHttpClient : ApolloObject, IApolloHttpClient
	{
		private ListView<IApolloHttpRequest> requestPool = new ListView<IApolloHttpRequest>();

		public IApolloHttpRequest CreateRequest(IApolloConnector connector)
		{
			if (connector == null)
			{
				return null;
			}
			IApolloHttpRequest apolloHttpRequest = new ApolloHttpRequest(connector);
			this.requestPool.Add(apolloHttpRequest);
			return apolloHttpRequest;
		}

		public ApolloResult ReleaseRequest(IApolloHttpRequest request)
		{
			if (!this.requestPool.Contains(request))
			{
				return ApolloResult.Success;
			}
			if (this.requestPool.Remove(request))
			{
				return ApolloResult.Success;
			}
			return ApolloResult.Error;
		}

		public override void Update()
		{
			foreach (IApolloHttpRequest current in this.requestPool)
			{
				current.Update();
			}
		}
	}
}
