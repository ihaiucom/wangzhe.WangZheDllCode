using Photon.Hive.Plugin.WebHooks;

namespace TestPlugins
{
    class SecureCheckPlugin : WebHooksPlugin
    {
        private readonly string name;

        public SecureCheckPlugin(string name)
        {
            this.name = name;
        }

        public override string Name
        {
            get { return this.name; }
        }
    }
}
