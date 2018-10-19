using System.Collections.Generic;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    public class SaveLoadStateTestPlugin : PluginBase
    {
        protected static readonly Dictionary<string, SerializableGameState> gameStates = new Dictionary<string, SerializableGameState>();

        public override string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public override bool IsPersistent
        {
            get
            {
                return true;
            }
        }

        public override void OnCloseGame(ICloseGameCallInfo info)
        {
            base.OnCloseGame(info);
            var state = this.PluginHost.GetSerializableGameState();
            gameStates[this.PluginHost.GameId] = state;
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            SerializableGameState state;
            if (gameStates.TryGetValue(this.PluginHost.GameId, out state))
            {
                this.PluginHost.SetGameState(state);
            }
            base.OnCreateGame(info);
        }
    }

    public class SetStateAfterContinueTestPlugin : SaveLoadStateTestPlugin
    {
        public override string Name
        {
            get { return this.GetType().Name; }
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            base.OnCreateGame(info);

            SerializableGameState state;
            if (gameStates.TryGetValue(this.PluginHost.GameId, out state))
            {
                if (this.PluginHost.SetGameState(state))
                {
                    this.PluginHost.BroadcastErrorInfoEvent("SetGameState after call to info.Continue succeeded");
                }
                else
                {
                    this.BroadcastEvent(123, null);
                }
            }
        }
    }
}
