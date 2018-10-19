using Photon.LoadBalancing.UnifiedClient.AuthenticationSchemes;
using Photon.LoadBalancing.UnitTests.UnifiedServer.Policy;

namespace Photon.LoadBalancing.UnitTests.Offline
{
    using NUnit.Framework;

    [TestFixture]
    public class PluginOfflineTests : PluginTestsImpl
    {

        public PluginOfflineTests()
            : base(new OfflineConnectPolicy(new TokenAuthenticationScheme()))
        {
            Photon.SocketServer.Protocol.TryRegisterCustomType(typeof(CustomPluginType), 1, 
                CustomPluginType.Serialize, CustomPluginType.Deserialize);
        }
    }
}
