using System;
using System.Runtime.CompilerServices;

namespace Apollo
{
	public class ApolloNetworkService : IApolloServiceBase, IApolloNetworkService
	{
		public static readonly ApolloNetworkService Intance = new ApolloNetworkService();

		public event NetworkStateChanged NetworkChangedEvent;

		private ApolloNetworkService()
		{
			Tx.Instance.TXNetworkChangedEvent += new NetworkStateChangedNotify(this.onNetworkStateChanged);
		}

		public NetworkState GetNetworkState()
		{
			return Tx.Instance.GetNetworkState();
		}

		private void onNetworkStateChanged(NetworkState state)
		{
			ADebug.Log("C# ApolloNetworkService onNetworkStateChanged state:" + state);
			if (this.NetworkChangedEvent != null)
			{
				this.NetworkChangedEvent(state);
			}
		}
	}
}
