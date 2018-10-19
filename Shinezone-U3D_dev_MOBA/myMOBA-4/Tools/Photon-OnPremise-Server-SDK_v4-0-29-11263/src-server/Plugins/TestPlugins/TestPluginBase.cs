using Photon.Hive.Plugin;

namespace TestPlugins
{
    abstract class TestPluginBase : PluginBase
    {
        public override string Name
        {
            get { return this.GetType().Name; }
        }
    }
}