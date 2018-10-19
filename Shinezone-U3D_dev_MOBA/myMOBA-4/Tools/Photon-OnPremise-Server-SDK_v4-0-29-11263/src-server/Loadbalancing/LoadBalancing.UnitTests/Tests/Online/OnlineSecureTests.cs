using NUnit.Framework;
using Photon.LoadBalancing.UnifiedClient.AuthenticationSchemes;
using Photon.LoadBalancing.UnitTests.TestsImpl;
using Photon.LoadBalancing.UnitTests.UnifiedServer.Policy;

namespace Photon.LoadBalancing.UnitTests.Online
{
    [TestFixture]
    public class OnlineSecureTests : SecureTestsImpl
    {
        public OnlineSecureTests() : base(new OnlineConnectPolicy(new TokenAuthenticationScheme()))
        {
        }
    }
}
