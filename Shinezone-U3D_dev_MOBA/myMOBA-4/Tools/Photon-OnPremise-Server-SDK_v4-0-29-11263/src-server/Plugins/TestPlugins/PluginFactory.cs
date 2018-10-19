
using System;
using System.Collections.Generic;
using Photon.Hive.Plugin;
using Photon.Hive.Plugin.WebHooks;

namespace TestPlugins
{
    public class PluginFactory : Photon.Hive.Plugin.IPluginFactory
    {
        private static PluginBase pluginInstase = null;

        public IGamePlugin Create(IPluginHost sink, string pluginName, Dictionary<string, string> config, out string errorMsg)
        {
            var prefix = sink.GameId.Contains("_") ? sink.GameId.Substring(0, sink.GameId.IndexOf('_')) : string.Empty; 

            IGamePlugin plugin;
            switch (pluginName)
            {
                case "SetPropertiesCheckPlugin":
                    plugin = new SetPropertiesCheckPlugin();
                    break;
                case "JoinFailuresCheckPlugin":
                    plugin = new JoinFailuresCheckPlugin();
                    break;
                case "RaiseEventChecksPlugin":
                    plugin = new RaiseEventChecksPlugin();
                    break;
                case "BasicTestsPlugin":
                    plugin = new BasicTestsPlugin();
                    break;
                case "SameInstancePlugin":
                    if (pluginInstase == null)
                    {
                        pluginInstase = new SameInstancePlugin();
                    }
                    plugin = pluginInstase;
                    break;
                case "SaveLoadStateTestPlugin":
                    plugin = new SaveLoadStateTestPlugin();
                    break;
                case "ScheduleBroadcastTestPlugin":
                    plugin = new ScheduleBroadcastTestPlugin();
                    break;
                case "ScheduleSetPropertiesTestPlugin":
                    plugin = new ScheduleSetPropertiesTestPlugin();
                    break;
                case "Webhooks":
                    plugin = new WebHooksPlugin();
                    plugin.SetupInstance(sink, config, out errorMsg);
                    return plugin;
                case "MasterClientIdPlugin":
                    plugin = new MasterClientIdPlugin();
                    break;
                case "CustomTypeCheckPlugin":
                    plugin = new CustomTypeCheckPlugin();
                    break;
                case "SyncAsyncHttpTestPlugin":
                    plugin = new SyncAsyncHttpTestPlugin();
                    break;
                case "CustomTypeMapperPlugin":
                    plugin = new CustomTypeMapperPlugin();
                    break;
                case "JoinExceptionsPlugin":
                    plugin = new JoinExceptionsPlugin();
                    break;
                case "CheckSecurePlugin":
                {
                    plugin = new SecureCheckPlugin("CheckSecurePlugin");
                    config = new Dictionary<string, string>
                    {
                        {"BaseUrl", "http://photon-forward.webscript.io"},
                        {"PathJoin", "JoinGameSecure"},
                        {"PathEvent", "RaiseEventSecure"},
                        {"PathCreate", "CreateGameSecure"},
                        {"PathGameProperties", "SetPropertiesSecure"},
                    };
                    break;
                }
                case "StrictModeFailurePlugin":
                    plugin = new StrictModeFailurePlugin();
                    break;
                case "SetStateAfterContinueTestPlugin":
                    plugin = new SetStateAfterContinueTestPlugin();
                    break;
                case "ErrorPlugin":
                    plugin = new ErrorPlugin("Error plugin is used");
                    break;
                case "StripedGameStatePlugin":
                    plugin = new StripedGameStatePlugin();
                    break;
                case "NullRefPlugin":
                    errorMsg = "NullRefPlugin is called";
                    return null;

                case "ExceptionPlugin":
                    errorMsg = "Exception plugin is called";
                    throw new Exception("From exception:" + errorMsg);
                case "BanTestPlugin":
                    plugin = new BanTestPlugin();
                    break;
                case "ChangeGamePropertiesOnJoinPlugin":
                    plugin = new ChangeGamePropertiesOnJoinPlugin();
                    break;
                case "RemovingActorPlugin":
                    plugin = new RemovingActorPlugin();
                    break;
                case "BroadcastEventPlugin":
                    plugin = new BroadcastEventPlugin();
                    break;
                case "LongOnClosePlugin":
                    plugin = new LongOnClosePlugin();
                    break;
                case "LongOnClosePluginWithPersistence":
                    plugin = new LongOnClosePluginWithPersistence();
                    break;
                case "WrongUrlTestPlugin":
                    plugin = new WrongUrlTestPlugin();
                    break;
                default:
                    switch (prefix)
                    {
                        case "ForwardPlugin1":
                            plugin = new WebHooksPlugin();
                            config = new Dictionary<string, string> {{"BaseUrl", "X"}};
                            break;
                        case "ForwardPlugin2":
                            if (string.IsNullOrEmpty(pluginName))
                            {
                                plugin = new PluginBase();
                            }
                            else
                            {
                                plugin = new WebHooksPlugin();
                                sink = new PluginHostWrapper(sink);
                                config = new Dictionary<string, string>
                                {
                                    {"BaseUrl", "http://photon-photon-pluginsdk-v1.webscript.io"},
                                    {"PathClose", "GameClose"},
                                    {"PathCreate", "GameCreate"},
                                };
                            }
                            break;
                        default:
                            plugin = new PluginBase();
                            break;
                    }
                    break;
            }

            if (plugin.SetupInstance(sink, config, out errorMsg))
            {
                return  plugin;
            }

            return null;
        }
    }

    class SameInstancePlugin : TestPluginBase
    {
    }
}
