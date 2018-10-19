using Photon.Hive;
using Photon.LoadBalancing.GameServer;

namespace Photon.LoadBalancing.UnitTests.UnifiedServer.OfflineExtra
{
    public class TestGameCache : GameCache
    {
        private readonly TestPluginManager pluginManager;

        public TestGameCache(GameApplication application)
            : base(application)
        {
            this.pluginManager = new TestPluginManager();
        }

        protected override Room CreateRoom(string roomId, params object[] args)
        {
            return new TestGame(this.Application, roomId, this, this.pluginManager, (string)args[0]);
        }
    }
}
