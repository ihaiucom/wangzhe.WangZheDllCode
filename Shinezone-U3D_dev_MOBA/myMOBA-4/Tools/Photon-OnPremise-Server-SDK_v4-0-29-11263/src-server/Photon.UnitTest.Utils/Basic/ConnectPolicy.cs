using System;
using ExitGames.Client.Photon;

namespace Photon.UnitTest.Utils.Basic
{
    public abstract class ConnectPolicy
    {
        public const int WaitTime = 5000;

        public string ApplicationId = string.Empty;
        public string ApplicationVersion = string.Empty;
        public string Region = string.Empty;
        public string ApplicationName = string.Empty;

        public Version ClientVersion = new Version(4, 0, 5, 0); // new Version(4, 2, 255, 255);

        public ushort sdkId = 0x0000;

        protected const bool LogClientMessages = false;

        public ConnectionProtocol Protocol = ConnectionProtocol.Tcp;

        public IAuthenticationScheme AuthenticatonScheme;

        public virtual string ServerIp
        {
            get
            {
                return "127.0.0.1";
            }
        }

        public virtual string ServerPort
        {
            get
            {
                switch (this.Protocol)
                {
                    case ConnectionProtocol.Tcp:
                        return "4530";
                    case ConnectionProtocol.Udp:
                        return "5055";
                    case ConnectionProtocol.RHttp:
                        return "80/photon/m";
                    case ConnectionProtocol.WebSocket:
                        return "9090";
                    case ConnectionProtocol.WebSocketSecure:
                        return "19090";

                    default:
                        throw new NotSupportedException("Protocol: " + this.Protocol);
                }
            }
        }

        public string ServerAddress
        {
            get
            {
                return string.Format("{0}:{1}", this.ServerIp, this.ServerPort);
            }
        }

        virtual public bool IsOffline { get { return false; } }
        public bool IsOnline { get { return !this.IsOffline; } }

        virtual public bool IsInited { get { return true; } }

        public abstract bool Setup();
        public abstract void TearDown();

        public abstract UnifiedClientBase CreateTestClient();

        public abstract void ConnectToServer(INUnitClient client, string address);
    }
}
