using NUnit.Framework;
using Photon.LoadBalancing.UnifiedClient.AuthenticationSchemes;
using Photon.LoadBalancing.UnitTests.TestsImpl;
using Photon.LoadBalancing.UnitTests.UnifiedServer.Policy;

namespace Photon.LoadBalancing.UnitTests.Offline
{
    [TestFixture]
    public class OfflineSecureTests : SecureTestsImpl
    {
        public OfflineSecureTests() : base(new OfflineConnectPolicy(new TokenAuthenticationScheme()))
        {
        }
    }
}
