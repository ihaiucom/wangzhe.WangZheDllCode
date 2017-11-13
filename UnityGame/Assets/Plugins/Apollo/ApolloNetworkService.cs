using System;
using System.Runtime.CompilerServices;

namespace Apollo
{
	public class ApolloNetworkService : IApolloNetworkService, IApolloServiceBase
	{
		public static readonly ApolloNetworkService Intance = new ApolloNetworkService();

		public event NetworkStateChanged NetworkChangedEvent
		{
			[MethodImpl(32)]
			add
			{
				this.NetworkChangedEvent = (NetworkStateChanged)Delegate.Combine(this.NetworkChangedEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.NetworkChangedEvent = (NetworkStateChanged)Delegate.Remove(this.NetworkChangedEvent, value);
			}
		}

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
