using Apollo;
using System;

namespace Assets.Scripts.Framework
{
	public class BaseConnector
	{
		protected IApolloConnector connector;

		public ConnectorParam initParam;

		public bool connected;

		public static uint connectTimeout = 10u;

		public ConnectorParam GetConnectionParam()
		{
			return this.initParam;
		}

		public void DestroyConnector()
		{
			if (this.connector != null)
			{
				this.connector.ConnectEvent -= new ConnectEventHandler(this.onConnectEvent);
				this.connector.DisconnectEvent -= new DisconnectEventHandler(this.onDisconnectEvent);
				this.connector.ReconnectEvent -= new ReconnectEventHandler(this.onReconnectEvent);
				this.connector.ErrorEvent -= new ConnectorErrorEventHandler(this.onConnectError);
				this.connector.Disconnect();
				IApollo.Instance.DestroyApolloConnector(this.connector);
				this.connector = null;
				this.connected = false;
			}
		}

		public bool CreateConnector(ConnectorParam param)
		{
			this.DestroyConnector();
			if (param == null)
			{
				return false;
			}
			this.initParam = param;
			this.connected = false;
			this.initParam.DealWithHttpDNS();
			this.connector = IApollo.Instance.CreateApolloConnection(ApolloConfig.platform, 16777215u, this.initParam.url);
			if (this.connector == null)
			{
				return false;
			}
			Console.WriteLine("Create Connect Entered!{0}  {1}", ApolloConfig.platform, this.initParam.url);
			this.connector.ConnectEvent += new ConnectEventHandler(this.onConnectEvent);
			this.connector.DisconnectEvent += new DisconnectEventHandler(this.onDisconnectEvent);
			this.connector.ReconnectEvent += new ReconnectEventHandler(this.onReconnectEvent);
			this.connector.ErrorEvent += new ConnectorErrorEventHandler(this.onConnectError);
			this.connector.SetSecurityInfo(this.initParam.enc, this.initParam.keyMaking, ConnectorParam.DH);
			return this.connector.Connect(BaseConnector.connectTimeout) == ApolloResult.Success;
		}

		public void RestartConnector()
		{
			this.DestroyConnector();
			this.CreateConnector(this.initParam);
		}

		protected virtual void DealConnectSucc()
		{
		}

		protected virtual void DealConnectFail(ApolloResult result, ApolloLoginInfo loginInfo)
		{
		}

		protected virtual void DealConnectError(ApolloResult result)
		{
		}

		protected virtual void DealConnectClose(ApolloResult result)
		{
		}

		private void onConnectError(ApolloResult result)
		{
			this.connected = false;
			this.DealConnectError(result);
		}

		private void onConnectEvent(ApolloResult result, ApolloLoginInfo loginInfo)
		{
			if (this.connector == null)
			{
				return;
			}
			if (result == ApolloResult.Success)
			{
				this.connected = true;
				this.DealConnectSucc();
			}
			else
			{
				this.DealConnectFail(result, loginInfo);
			}
		}

		private void onDisconnectEvent(ApolloResult result)
		{
			if (result == ApolloResult.Success)
			{
				this.connected = false;
				this.DealConnectClose(result);
			}
		}

		private void onReconnectEvent(ApolloResult result)
		{
			if (this.connector == null)
			{
				return;
			}
			if (result == ApolloResult.Success)
			{
				this.connected = true;
				this.DealConnectSucc();
			}
			else
			{
				this.DealConnectFail(result, null);
			}
		}
	}
}
