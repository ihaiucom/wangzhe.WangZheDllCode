using Photon.LoadBalancing.GameServer;

namespace Photon.LoadBalancing.UnitTests.UnifiedServer.OfflineExtra
{
    public class TestApplication : GameApplication
    {
        public override GameCache GameCache { get; protected set; }

        public TestApplication()
        {
        }

        protected override void Setup()
        {
            base.Setup();
            this.GameCache = new TestGameCache(this);
        }
    }
}
