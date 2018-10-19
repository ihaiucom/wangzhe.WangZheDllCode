using System;
using ExitGames.Client.Photon;
using Photon.LoadBalancing.UnifiedClient.AuthenticationSchemes;
using Photon.UnitTest.Utils.Basic;

namespace Photon.LoadBalancing.UnitTests.UnifiedServer.Policy
{
    public abstract class LBConnectPolicyBase : ConnectPolicy
    {
        public const string MasterServerAppName = "Master";

        public const string GameServerAppName = "Game";

        public virtual string MasterServerAddress 
        {
            get
            {
                switch (this.Protocol)
                {
                    case ConnectionProtocol.RHttp:
                        return string.Format("http://{0}:{1}", this.ServerIp, this.ServerPort);
                    case ConnectionProtocol.WebSocket:
                        return string.Format("ws://{0}:{1}", this.ServerIp, this.ServerPort);
                    case ConnectionProtocol.WebSocketSecure:
                        return string.Format("wss://{0}:{1}", this.ServerIp, this.ServerPort);
                    default:
                        return this.ServerAddress;
                }
            }
        }

        public string GameServerAddress
        {
            get
            {
                switch (this.Protocol)
                {
                    case ConnectionProtocol.Tcp:
                        return String.Format("{0}:{1}", this.ServerIp, 4531);
                    case ConnectionProtocol.Udp:
                        return String.Format("{0}:{1}", this.ServerIp, 5056);
                    case ConnectionProtocol.WebSocket:
                        return String.Format("ws://{0}:{1}", this.ServerIp, 9091);
                    case ConnectionProtocol.WebSocketSecure:
                        return String.Format("wss://{0}:{1}", this.ServerIp, 19091);
                    case ConnectionProtocol.RHttp:
                        return String.Format("http://{0}:80/photon/g", this.ServerIp);
                    default:
                        throw new NotSupportedException("Protocol: " + this.Protocol);
                }
            }
        }

        protected LBConnectPolicyBase()
        {
            this.AuthenticatonScheme = new TokenLessAuthenticationScheme();
            this.ApplicationId = MasterServerAppName;
        }
    }
}