using System;
using System.Collections.Generic;
using System.Threading;
using Photon.Hive.Collections;
using Photon.Hive.Plugin;
using Photon.Hive.Messages;
using Photon.Hive.Operations;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using SendParameters = Photon.SocketServer.SendParameters;

namespace Photon.Hive.Tests
{
 
    public class TestActor : Actor
    {
        public TestActor(int actorNr, string UserId)
            :base(actorNr, UserId, null, null)
        {
        }
    }

    public class TestGameCache : Caching.RoomCacheBase
    {
        public static readonly TestGameCache Instance = new TestGameCache();

        public static readonly IPluginManager PluginManager = new TestPluginManager();
        /// <summary>
        /// Creates a new <see cref="HiveGame"/>.
        /// </summary>
        /// <param name="roomId">
        /// The room id.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// A new <see cref="HiveGame"/>
        /// </returns>
        protected override Room CreateRoom(string roomId, params object[] args)
        {
            var pluginName = args.Length == 0 ? string.Empty : (string)args[0];
            return new TestGame(roomId, this, 1000 // maximum TTL for test game is 1 sec
                , PluginManager, pluginName
                );
        }
    }

    public class TestActorsManager : ActorsManager
    {
        public void AddInactive(Actor actor)
        {
            this.allActors.Add(actor);
        }
    }

    public class TestGameState : GameState
    {
        public TestGameState()
        {
            this.actorsManager = new TestActorsManager();
        }
    }
    public class TestGame : HiveHostGame
    {
        private readonly AutoResetEvent onRemovePlayerEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent onCleanupActorEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent onRaiseEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent onCachOpEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent onDispose = new AutoResetEvent(false);
        private readonly AutoResetEvent onAllActorsDisposed = new AutoResetEvent(false);

        class TestGameStateFactory : IGameStateFactory
        {
            public GameState Create()
            {
                return new TestGameState();
            }
        }
        public TestGame(string roomId, Caching.RoomCacheBase parent, int emptyRoomTTL, IPluginManager eManager, string pluginName)
            : base(roomId, parent, new TestGameStateFactory(), emptyRoomTTL, eManager, pluginName)
        {
            //this.IsOpen = true;
            //this.IsVisible = true;
            //this.EventCache.SetGameAppCounters(NullHiveGameAppCounters.Instance);
            //this.EventCache.AddSlice(0);
        }

        public int ActorCounter
        {
            get
            {
                return this.ActorsManager.ActorNumberCounter;
            }
            set
            {
                this.ActorsManager.ActorNumberCounter = value;
            }
        }

        public new TestActorsManager ActorsManager
        {
            get
            {
                return (TestActorsManager)base.ActorsManager;
            }
        }
        public void SetIsOpen(bool value)
        {
            this.IsOpen = value;
        }

        public void SetSuppressRoomEvents(bool value)
        {
            this.SuppressRoomEvents = value;
        }

        public void SetSlice(int value)
        {
            this.EventCache.Slice = value;
        }

        public IEnumerable<Actor> GetDisconnectedActors()
        {
            return this.InactiveActors;
        }

        public IEnumerable<Actor> GetActors()
        {
            return this.Actors;
        }

        public TestPlugin GetPlugin()
        {
            return (TestPlugin)this.Plugin;
        }

        protected override void ProcessMessage(Messages.IMessage message)
        {
            base.ProcessMessage(message);
            if (message.Action == (byte)GameMessageCodes.RemovePeerFromGame)
            {
                this.onRemovePlayerEvent.Set();
            }
        }

        protected override void CleanupActor(Actor actor)
        {
            this.onCleanupActorEvent.Set();
            base.CleanupActor(actor);
            if (this.ActorsManager.Count == 0)
            {
                this.onAllActorsDisposed.Set();
            }
        }

        public void WaitRemovePeerFromGame()
        {
            this.onRemovePlayerEvent.WaitOne();
        }

        public void WaitForRaisevent()
        {
            this.onRaiseEvent.WaitOne();
        }

        public void WaitForCacheOpEvent()
        {
            this.onCachOpEvent.WaitOne();
        }

        public bool WaitForDispose()
        {
            return this.onDispose.WaitOne(5000);
        }

        public void WaitForAllActorsDisposed()
        {
            this.onAllActorsDisposed.WaitOne();
        }

        protected override bool ProcessRaiseEvent(HivePeer peer, RaiseEventRequest raiseEventRequest, SendParameters sendParameters, Actor actor)
        {
            var result = base.ProcessRaiseEvent(peer, raiseEventRequest, sendParameters, actor);
            if (raiseEventRequest.Cache >= 10)
            {
                this.onCachOpEvent.Set();
            }
            else
                this.onRaiseEvent.Set();
            return result;
        }

        protected override bool ProcessBeforeCloseGame(CloseRequest request)
        {
            var property = this.Properties.GetProperty("key");
            if (property != null
                && (((string)property.Value) == "BeforeCloseContinueException" || ((string)property.Value) == "CloseFatal"))
            {
                throw new Exception(string.Format("Expected Test Exception: {0}", property.Value));
            }

            return base.ProcessBeforeCloseGame(request);
        }

        protected override bool ProcessCloseGame(object state)
        {
            var property = this.Properties.GetProperty("key");
            if (property != null
                && (((string)property.Value) == "OnCloseContinueException" || ((string)property.Value) == "CloseFatal"))
            {
                throw new Exception(string.Format("Expected Test Exception: {0}", property.Value));
            }

            return base.ProcessCloseGame(state);
        }


        public bool WaitOnCleanupActor(int timeout)
        {
            return this.onCleanupActorEvent.WaitOne(timeout);
        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);
            this.onDispose.Set();
        }

        public void AddActorToGroup(byte groupId, TestActor actor)
        {
            this.GroupManager.AddActorToGroup(groupId, actor);
        }
    }

    public class TestHivePeer : HivePeer
    {
        private readonly AutoResetEvent onDisconnectEvent = new AutoResetEvent(false);

        public TestHivePeer(InitRequest initRequest)
            : base(initRequest)
        {
            this.Initialize(initRequest);
        }

        public TestHivePeer(IRpcProtocol rpcProtocol, IPhotonPeer nativePeer)
            : this(new InitRequest(rpcProtocol, nativePeer))
        {
        }

        public TestHivePeer(IRpcProtocol rpcProtocol, IPhotonPeer nativePeer, string userId)
            : this(rpcProtocol, nativePeer)
        {
            this.UserId = userId;
        }

        public void SetUserId(string value)
        {
            this.UserId = value;
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            base.OnDisconnect(reasonCode, reasonDetail);
            this.onDisconnectEvent.Set();
        }

        public void WaitOnDisconnect()
        {
            this.onDisconnectEvent.WaitOne();
        }

        protected override Caching.RoomReference GetRoomReference(JoinGameRequest joinRequest, params object[] args)
        {
            return TestGameCache.Instance.GetRoomReference(joinRequest.GameId, this, args);
        }

        protected override bool TryCreateRoom(string gameId, out Caching.RoomReference roomReference, params object[] args)
        {
            return TestGameCache.Instance.TryCreateRoom(gameId, this, out roomReference, args);
        }

        protected override Caching.RoomReference GetOrCreateRoom(string gameId, params object[] args)
        {
            return TestGameCache.Instance.GetRoomReference(gameId, this);
        }

        protected override bool TryGetRoomReference(string gameId, out Caching.RoomReference roomReference)
        {
            return TestGameCache.Instance.TryGetRoomReference(gameId, this, out roomReference);
        }

        protected override bool TryGetRoomWithoutReference(string gameId, out Room room)
        {
            return TestGameCache.Instance.TryGetRoomWithoutReference(gameId, out room);
        }
    }

    public class TestPlugin : PluginBase
    {
        private readonly ManualResetEvent onCreateEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent onBeforeCloseEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent onCloseEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent onBeforeJoinEvent = new ManualResetEvent(false);
        private readonly AutoResetEvent onJoinEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent onLeaveEvent = new AutoResetEvent(false);
        private readonly ManualResetEvent onDisconnectEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent onSetPropertiesEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent onBeforeSetPropertiesEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent onRaiseEvent = new ManualResetEvent(false);
        private readonly AutoResetEvent onReportError = new AutoResetEvent(false);
        private readonly AutoResetEvent allowContinueEvent = new AutoResetEvent(false);

        public override string Name
        {
            get { return "TestPlugin"; }
        }

        public TestPlugin()
        {
            this.UseStrictMode = true;
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            this.onCreateEvent.Set();

            if (this.PluginHost.GameId.EndsWith("OnCreateForgotCall"))
            {
                return;
            }

            info.Continue();
        }

        public bool WaitForOnCreateEvent(int timeout)
        {
            return this.onCreateEvent.WaitOne(timeout);
        }

        public override void BeforeCloseGame(IBeforeCloseGameCallInfo info)
        {
            this.onBeforeCloseEvent.Set();
            if (this.PluginHost.GameId.EndsWith("OnBeforeCloseForgotCall"))
            {
                return;
            }

            if (this.PluginHost.GameId.EndsWith("StopCloseGameIfThereIsActive"))
            {
                this.allowContinueEvent.WaitOne();
            }

            var property = (string)this.PluginHost.GameProperties["key"];
            if (property != null 
                && (property == "BeforeCloseGameException" || property == "CloseFatalPlugin"))
            {
                throw new Exception("BeforeCloseGameException for test");
            }

            info.Continue();
        }

        public bool WaitForOnBeforeCloseEvent(int timeout)
        {
            return this.onBeforeCloseEvent.WaitOne(timeout);
        }

        public override void OnCloseGame(ICloseGameCallInfo info)
        {
            this.onCloseEvent.Set();
            if (this.PluginHost.GameId.EndsWith("OnCloseForgotCall"))
            {
                return;
            }

            if (this.PluginHost.GameId.EndsWith("ReinitGame"))
            {
                this.allowContinueEvent.WaitOne();
            }

            var property = (string)this.PluginHost.GameProperties["key"];
            if (property != null && (property == "OnCloseGameException" || property == "CloseFatalPlugin"))
            {
                throw new Exception("OnCloseGameException for test");
            }

            info.Continue();
        }

        public void AllowContinue()
        {
            this.allowContinueEvent.Set();
        }

        public bool WaitForOnCloseEvent(int timeout)
        {
            return this.onCloseEvent.WaitOne(timeout);
        }

        public override void BeforeJoin(IBeforeJoinGameCallInfo info)
        {
            this.onBeforeJoinEvent.Set();
            info.Continue();
        }

        public bool WaitForOnBeforeJoinEvent(int timeout)
        {
            return this.onBeforeJoinEvent.WaitOne(timeout);
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            this.onJoinEvent.Set();
            info.Continue();
        }

        public bool WaitForOnJoinEvent(int timeout)
        {
            return this.onJoinEvent.WaitOne(timeout);
        }

        public override void OnLeave(ILeaveGameCallInfo info)
        {
            if (info.IsInactive)
            {
                this.onDisconnectEvent.Set();
            }
            else
            {
                this.onLeaveEvent.Set();
            }
            info.Continue();

            if (info.UserId.Contains("Ban"))
            {
                this.PluginHost.RemoveActor(info.ActorNr, RemoveActorReason.Banned, "TestBan");
            }
        }

        public bool WaitForOnLeaveEvent(int timeout)
        {
            return this.onLeaveEvent.WaitOne(timeout);
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            this.onRaiseEvent.Set();
            info.Continue();
        }

        public bool WaitForOnRaiseEvent(int timeout)
        {
            return this.onRaiseEvent.WaitOne(timeout);
        }

        public override void BeforeSetProperties(IBeforeSetPropertiesCallInfo info)
        {
            this.onBeforeSetPropertiesEvent.Set();
            info.Continue();
        }

        public bool WaitForOnBeforeSetPropertiesEvent(int timeout)
        {
            return this.onBeforeSetPropertiesEvent.WaitOne(timeout);
        }

        public override void OnSetProperties(ISetPropertiesCallInfo info)
        {
            this.onSetPropertiesEvent.Set();
            info.Continue();
        }

        public bool WaitForOnSetPropertiesEvent(int timeout)
        {
            return this.onSetPropertiesEvent.WaitOne(timeout);
        }

#if PLUGINS_0_9
        public override void OnDisconnect(IDisconnectCallInfo info)
        {
            onDisconnectEvent.Set();
            info.Continue();
        }
#endif

        public bool WaitForOnDisconnectEvent(int timeout)
        {
            return this.onDisconnectEvent.WaitOne(timeout);
        }

        protected override void ReportError(short errorCode, Exception exception, object state)
        {
            this.onReportError.Set();
        }

        public bool WaitForReportError(int timeout)
        {
            return this.onReportError.WaitOne(timeout);
        }
    }

    public class TestPluginManager : IPluginManager
    {
        public IPluginInstance GetGamePlugin(IPluginHost sink, string pluginName)
        {
            IGamePlugin plugin;
            if (pluginName == "ErrorPlugin")
            {
                plugin = new ErrorPlugin("Error plugin is used");
            }
            else
            {
                plugin = new TestPlugin();
            }

            string errorMsg;
            plugin.SetupInstance(sink, null, out errorMsg);
            return new PluginInstance { Plugin = plugin, Version = GetEnvironmentVersion() };
        }

        static private EnvironmentVersion GetEnvironmentVersion()
        {
            var currentPluginsVersion = typeof(PluginBase).Assembly.GetName().Version;
            return new EnvironmentVersion {BuiltWithVersion = currentPluginsVersion, HostVersion = currentPluginsVersion};
        }
    }
}
