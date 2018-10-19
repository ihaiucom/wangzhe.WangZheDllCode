using Photon.LoadBalancing.UnifiedClient.AuthenticationSchemes;
using Photon.LoadBalancing.UnitTests.UnifiedServer.Policy;
using Photon.UnitTest.Utils.Basic;

namespace Photon.LoadBalancing.UnitTests.Online
{
    using ExitGames.Client.Photon;

    using NUnit.Framework;

    [TestFixture("LOCAL")]
    public class PluginOnlineTests : PluginTestsImpl
    {
        public PluginOnlineTests(string policyName, ConnectionProtocol protocol)
            : base(GetPolicy(policyName, protocol))
        {
            ExitGames.Client.Photon.PhotonPeer.RegisterType(typeof(CustomPluginType), 1, 
                CustomPluginType.Serialize, CustomPluginType.Deserialize);
        }

        public PluginOnlineTests(string policyName)
            : this(policyName, ConnectionProtocol.Tcp)
        {
        }

        public static ConnectPolicy GetPolicy(string policyName, ConnectionProtocol protocol)
        {
            switch (policyName)
            {
                case "DEV":
                    //    return new OnlineConnectPolicy(new TokenAuthenticationScheme(), protocol)
                case "LIVE":
                    //    return new OnlineConnectPolicy(new TokenAuthenticationScheme(), protocol);
                case "LOCAL":
                default:
                    return new OnlineConnectPolicy(new TokenAuthenticationScheme(), protocol);
            }
        }
    }
}
