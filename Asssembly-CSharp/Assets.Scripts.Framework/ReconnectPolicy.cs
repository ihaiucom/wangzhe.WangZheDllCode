using Apollo;
using System;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class ReconnectPolicy
	{
		private BaseConnector connector;

		private tryReconnectDelegate callback;

		private bool sessionStopped;

		private float reconnectTime;

		private uint reconnectCount = 4u;

		private uint tryCount;

		private uint connectTimeout = 10u;

		public bool shouldReconnect;

		public void SetConnector(BaseConnector inConnector, tryReconnectDelegate inEvent, uint tryMax)
		{
			this.StopPolicy();
			this.connector = inConnector;
			this.callback = inEvent;
			this.reconnectCount = tryMax;
		}

		public void StopPolicy()
		{
			this.sessionStopped = false;
			this.shouldReconnect = false;
			this.reconnectTime = this.connectTimeout;
			this.tryCount = 0u;
		}

		public void StartPolicy(ApolloResult result, int timeWait)
		{
			switch (result)
			{
			case ApolloResult.Success:
				this.shouldReconnect = false;
				this.sessionStopped = false;
				return;
			case ApolloResult.Error:
				IL_1A:
				switch (result)
				{
				case ApolloResult.GcpError:
				case ApolloResult.PeerStopSession:
					goto IL_6F;
				}
				if (result != ApolloResult.ConnectFailed && result != ApolloResult.TokenSvrError)
				{
					this.shouldReconnect = true;
					this.sessionStopped = true;
					this.reconnectTime = ((this.tryCount == 0u) ? 0f : ((float)timeWait));
					return;
				}
				goto IL_B5;
			case ApolloResult.NetworkException:
				this.shouldReconnect = true;
				this.sessionStopped = false;
				this.reconnectTime = ((this.tryCount == 0u) ? 0f : ((float)timeWait));
				return;
			case ApolloResult.Timeout:
				goto IL_6F;
			}
			goto IL_1A;
			IL_6F:
			IL_B5:
			this.shouldReconnect = true;
			this.sessionStopped = true;
			this.reconnectTime = ((this.tryCount == 0u) ? 0f : ((float)timeWait));
		}

		public void UpdatePolicy(bool bForce)
		{
			if (this.connector != null && !this.connector.connected)
			{
				if (bForce)
				{
					this.reconnectTime = this.connectTimeout;
					this.tryCount = this.reconnectCount;
					if (this.sessionStopped)
					{
						this.connector.RestartConnector();
					}
					else
					{
						this.connector.RestartConnector();
					}
				}
				else
				{
					this.reconnectTime -= Time.unscaledDeltaTime;
					if (this.reconnectTime < 0f)
					{
						this.tryCount += 1u;
						this.reconnectTime = this.connectTimeout;
						uint num = this.tryCount;
						if (this.callback != null)
						{
							num = this.callback(num, this.reconnectCount);
						}
						if (num > this.reconnectCount)
						{
							return;
						}
						this.tryCount = num;
						if (this.sessionStopped)
						{
							this.connector.RestartConnector();
						}
						else
						{
							this.connector.RestartConnector();
						}
					}
				}
			}
		}
	}
}
