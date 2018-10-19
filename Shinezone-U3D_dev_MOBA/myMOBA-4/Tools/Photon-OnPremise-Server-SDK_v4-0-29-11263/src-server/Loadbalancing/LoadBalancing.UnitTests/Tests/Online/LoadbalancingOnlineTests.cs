using ExitGames.Client.Photon;
using NUnit.Framework;
using Photon.LoadBalancing.UnitTests.UnifiedServer.Policy;
using Photon.LoadBalancing.UnitTests.UnifiedTests;

namespace Photon.LoadBalancing.UnitTests.Online
{
    [TestFixture("TokenAuth")]
    [TestFixture("TokenAuth", ConnectionProtocol.WebSocket)]
    [TestFixture("TokenAuthNoUserIds")]
    [TestFixture("TokenLessAuthForOldClients")]
    public class LoadbalancingOnlineTests : LBApiTestsImpl
    {
        public LoadbalancingOnlineTests(string schemeName )
            : this(schemeName, ConnectionProtocol.Tcp)
        { }

        public LoadbalancingOnlineTests(string schemeName, ConnectionProtocol protocol)
            : base(new OnlineConnectPolicy(GetAuthScheme(schemeName), protocol))
        {
            if (schemeName == "TokenLessAuthForOldClients" || schemeName == "TokenAuthNoUserIds")
            {
                this.Player1 = null;
                this.Player2 = null;
                this.Player3 = null;
            }
        }
    }
}