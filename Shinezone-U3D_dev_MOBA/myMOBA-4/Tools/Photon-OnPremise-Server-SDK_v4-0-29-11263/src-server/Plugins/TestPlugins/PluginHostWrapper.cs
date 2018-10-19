using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    public class PluginHostWrapper : IPluginHost
    {
        private readonly IPluginHost host;

        public Dictionary<string, object> Environment { get { return host.Environment; } }
        public IList<IActor> GameActors { get { return host.GameActors; } }
        public IList<IActor> GameActorsActive { get; private set; }
        public IList<IActor> GameActorsInactive { get; private set; }
        public string GameId { get { return host.GameId; } }
        public Hashtable GameProperties { get { return host.GameProperties; } }
        public Dictionary<string, object> CustomGameProperties { get { return host.CustomGameProperties; } }
        public int MasterClientId { get { return host.MasterClientId; } }


        public PluginHostWrapper(IPluginHost host)
        {
            this.host = host;
        }

        public void BroadcastEvent(IList<int> recieverActors, int senderActor, byte evCode, Dictionary<byte, object> data, byte cacheOp, SendParameters sendParameters)
        {
            this.host.BroadcastEvent(recieverActors, senderActor, evCode, data, cacheOp, sendParameters);
        }

        public void BroadcastEvent(byte target, int senderActor, byte targetGroup, byte evCode, Dictionary<byte, object> data, byte cacheOp, SendParameters sendParameters)
        {
            this.host.BroadcastEvent(target, senderActor, targetGroup, evCode, data, cacheOp);
        }

        public void BroadcastErrorInfoEvent(string message, SendParameters sendParameters)
        {
            this.host.BroadcastErrorInfoEvent(message, sendParameters);
        }

        public void BroadcastErrorInfoEvent(string message, ICallInfo info, SendParameters sendParameters)
        {
            this.host.BroadcastErrorInfoEvent(message, info, sendParameters);
        }

        public object CreateOneTimeTimer(Action callback, int dueTimeMs)
        {
            return this.host.CreateOneTimeTimer(callback, dueTimeMs);
        }

        public object CreateTimer(Action callback, int dueTimeMs, int intervalMs)
        {
            return this.host.CreateTimer(callback, dueTimeMs, intervalMs);
        }

#if PLUGINS_0_9
        public Dictionary<byte, byte[]> GetGameStateAsByteArray()
        {
            return this.host.GetGameStateAsByteArray();
        }

        public Dictionary<string, object> GetGameState()
        {
            return this.host.GetGameState();
        }
#endif

        public SerializableGameState GetSerializableGameState()
        {
            return this.host.GetSerializableGameState();
        }

        public void HttpRequest(HttpRequest request)
        {
            this.host.HttpRequest(request);
        }

        public void LogDebug(object message)
        {
            this.host.LogDebug(message);
        }

        public void LogError(object message)
        {
            this.host.LogError(message);
        }

        public void LogFatal(object message)
        {
            this.host.LogFatal(message);
        }

        public void LogInfo(object message)
        {
            this.host.LogInfo(message);
        }

        public void LogWarning(object message)
        {
            this.host.LogWarning(message);
        }


#if PLUGINS_0_9
        public bool SetGameState(Dictionary<byte, byte[]> state)
        {
            return this.host.SetGameState(state);
        }

        public bool SetGameState(Dictionary<string, object> state)
        {
            return this.host.SetGameState(state);
        }
#endif

        public bool SetGameState(SerializableGameState state)
        {
            return this.host.SetGameState(state);
        }

        public bool SetProperties(int actorNr, Hashtable properties, Hashtable expected, bool broadcast)
        {
            return this.host.SetProperties(actorNr, properties, expected, broadcast);
        }

        public void StopTimer(object timer)
        {
            this.host.StopTimer(timer);
        }

        public bool RemoveActor(int actorNr, string reasonDetail)
        {
            return this.host.RemoveActor(actorNr, reasonDetail);
        }

        public bool RemoveActor(int actorNr, byte reason, string reasonDetail)
        {
            return this.host.RemoveActor(actorNr, reason, reasonDetail);
        }

        public bool TryRegisterType(Type type, byte typeCode, Func<object, byte[]> serializeFunction, Func<byte[], object> deserializeFunction)
        {
            return this.host.TryRegisterType(type, typeCode, serializeFunction, deserializeFunction);
        }

        public EnvironmentVersion GetEnvironmentVersion()
        {
            return this.host.GetEnvironmentVersion();
        }
    }
}
