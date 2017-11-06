using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Apollo
{
	public class Tx : ApolloObject
	{
		public static Tx Instance = new Tx();

		public event NetworkStateChangedNotify TXNetworkChangedEvent
		{
			[MethodImpl(32)]
			add
			{
				this.TXNetworkChangedEvent = (NetworkStateChangedNotify)Delegate.Combine(this.TXNetworkChangedEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.TXNetworkChangedEvent = (NetworkStateChangedNotify)Delegate.Remove(this.TXNetworkChangedEvent, value);
			}
		}

		private Tx() : base(false, true)
		{
		}

		public void Initialize()
		{
			ADebug.Log("TX Initialize");
			Tx.tx_object_unity_enable_ui_update();
			this.setNetworkChangedCallback();
		}

		protected override void OnUpdate(float deltaTime)
		{
			Tx.tx_object_unity_update();
		}

		public NetworkState GetNetworkState()
		{
			return Tx.tx_network_GetNetworkState();
		}

		private void setNetworkChangedCallback()
		{
			Tx.tx_network_SetNetworkChangedCallback(new NetworkStateChangedNotify(Tx.onNetworkChanged));
		}

		[MonoPInvokeCallback(typeof(NetworkStateChangedNotify))]
		private static void onNetworkChanged(NetworkState state)
		{
			if (Tx.Instance.TXNetworkChangedEvent != null)
			{
				Tx.Instance.TXNetworkChangedEvent(state);
			}
		}

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void tx_object_unity_enable_ui_update();

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void tx_object_unity_update();

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern NetworkState tx_network_GetNetworkState();

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void tx_network_SetNetworkChangedCallback([MarshalAs(38)] NetworkStateChangedNotify callback);
	}
}
