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
            ApolloResult result2 = result;
            switch (result2)
            {
                case ApolloResult.Success:
                    this.shouldReconnect = false;
                    this.sessionStopped = false;
                    return;

                case ApolloResult.NetworkException:
                    this.shouldReconnect = true;
                    this.sessionStopped = false;
                    this.reconnectTime = (this.tryCount != 0) ? ((float)timeWait) : ((float)0);
                    return;

                case ApolloResult.Timeout:
                case ApolloResult.GcpError:
                case ApolloResult.PeerStopSession:
                    break;

                default:
                    if ((result2 != ApolloResult.ConnectFailed) && (result2 != ApolloResult.TokenSvrError))
                    {
                        this.shouldReconnect = true;
                        this.sessionStopped = true;
                        this.reconnectTime = (this.tryCount != 0) ? ((float)timeWait) : ((float)0);
                        return;
                    }
                    break;
            }
            this.shouldReconnect = true;
            this.sessionStopped = true;
            this.reconnectTime = (this.tryCount != 0) ? ((float)timeWait) : ((float)0);
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
