using System.Threading;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    class LongOnClosePlugin : TestPluginBase
    {
        public override void OnCloseGame(ICloseGameCallInfo info)
        {
            Thread.Sleep(2000);
            base.OnCloseGame(info);
        }
    }

    class LongOnClosePluginWithPersistence : LongOnClosePlugin
    {
        public override bool IsPersistent
        {
            get { return true; }
        }
    }
}
