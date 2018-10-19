using NUnit.Framework;
using Photon.LoadBalancing.UnitTests.UnifiedServer.Policy;
using Photon.LoadBalancing.UnitTests.UnifiedTests;

namespace Photon.LoadBalancing.UnitTests.Offline
{
    [TestFixture("TokenAuth")]
    [TestFixture("TokenAuthNoUserIds")]
    [TestFixture("TokenLessAuthForOldClients")]
    public class LoadBalancingOfflineTests : LBApiTestsImpl
    {
        public LoadBalancingOfflineTests(string schemeName)
            : base(new OfflineConnectPolicy(GetAuthScheme(schemeName)))
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