using Photon.Hive.Plugin;

namespace TestPlugins
{
    class StripedGameStatePlugin : PluginBase
    {
        public override string Name
        {
            get { return this.GetType().Name; }
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            const string stripedState = "{\"LobbyId\":null,\"LobbyType\":0,\"CustomProperties\":{},\"EmptyRoomTTL\":5000,\"PlayerTTL\":2147483647,\"CheckUserOnJoin\":false,\"DeleteCacheOnLeave\":true,\"SuppressRoomEvents\":false}";

            var serializableGameState = Newtonsoft.Json.JsonConvert.DeserializeObject<SerializableGameState>(stripedState);

            if (!this.PluginHost.SetGameState(serializableGameState))
            {
                info.Fail("Failed to load state");
            }
            else
            {
                info.Continue();
            }
        }
    }
}
