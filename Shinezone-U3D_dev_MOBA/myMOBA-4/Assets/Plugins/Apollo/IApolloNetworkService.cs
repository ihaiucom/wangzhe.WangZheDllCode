using System;

namespace Apollo
{
	public interface IApolloNetworkService : IApolloServiceBase
	{
		event NetworkStateChanged NetworkChangedEvent;

		NetworkState GetNetworkState();
	}
}
