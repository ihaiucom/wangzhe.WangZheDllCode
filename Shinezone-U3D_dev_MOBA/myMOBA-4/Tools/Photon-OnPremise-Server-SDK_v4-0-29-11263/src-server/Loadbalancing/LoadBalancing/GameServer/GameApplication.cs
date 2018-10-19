// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameApplication.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GameApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Photon.Common.Authentication;
using Photon.Common.LoadBalancer;
using Photon.Common.LoadBalancer.Common;
using Photon.Common.LoadBalancer.LoadShedding;
using Photon.Common.LoadBalancer.LoadShedding.Diagnostics;
using Photon.Common.Misk;
using Photon.Hive.Common;
using Photon.Hive.Plugin;
using Photon.Hive.WebRpc;
using Photon.Hive.WebRpc.Configuration;
using Photon.SocketServer.Rpc.Protocols;

namespace Photon.LoadBalancing.GameServer
{
    #region using directives

    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using ExitGames.Concurrency.Fibers;
    using ExitGames.Logging;
    using ExitGames.Logging.Log4Net;
    using Photon.Hive;
    using Photon.Hive.Messages;
    using log4net;
    using log4net.Config;
    using Photon.LoadBalancing.Common;
    using Photon.SocketServer;
    using Photon.SocketServer.Diagnostics;

    using ConfigurationException = ExitGames.Configuration.ConfigurationException;
    using LogManager = ExitGames.Logging.LogManager;

    #endregion

    public class GameApplication : ApplicationBase
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly NodesReader reader;

        private ServerStateManager serverStateManager;

        private PoolFiber executionFiber;

        #endregion

        #region Constructors and Destructors

        public GameApplication()
        {
            AppDomain.CurrentDomain.AssemblyResolve += PluginManager.OnAssemblyResolve;

            this.UpdateMasterEndPoint();

            this.ServerId = Guid.NewGuid();
            this.GamingTcpPort = GameServerSettings.Default.GamingTcpPort;
            this.GamingUdpPort = GameServerSettings.Default.GamingUdpPort;
            this.GamingWebSocketPort = GameServerSettings.Default.GamingWebSocketPort;
            this.GamingSecureWebSocketPort = GameServerSettings.Default.GamingSecureWebSocketPort;
            this.GamingHttpPort = GameServerSettings.Default.GamingHttpPort;
            this.GamingHttpsPort = GameServerSettings.Default.GamingHttpsPort;
            this.GamingHttpPath = string.IsNullOrEmpty(GameServerSettings.Default.GamingHttpPath) ? string.Empty : "/" + GameServerSettings.Default.GamingHttpPath;

            this.ConnectRetryIntervalSeconds = GameServerSettings.Default.ConnectReytryInterval;

            this.reader = new NodesReader(this.ApplicationRootPath, CommonSettings.Default.NodesFileName);

            this.S2SCacheMan = new S2SCustomTypeCacheMan();
        }

        #endregion

        #region Public Properties

        public Guid ServerId { get; private set; }

        public int? GamingTcpPort { get; protected set; }

        public int? GamingUdpPort { get; protected set; }

        public int? GamingWebSocketPort { get; protected set; }

        public int? GamingSecureWebSocketPort { get; set; }

        public int? GamingHttpPort { get; protected set; }

        public int? GamingHttpsPort { get; protected set; }

        public string GamingHttpPath { get; protected set; }

        public IPEndPoint MasterEndPoint { get; protected set; }

        public ApplicationStatsPublisher AppStatsPublisher { get; protected set; }

        public MasterServerConnection MasterServerConnection { get; private set; }

        public IPAddress PublicIpAddress { get; protected set; }

        public IPAddress PublicIpAddressIPv6 { get; protected set; }

        public WorkloadController WorkloadController { get; protected set; }

        public virtual GameCache GameCache { get; protected set; }

        public AuthTokenFactory TokenCreator { get; protected set; }

        public S2SCustomTypeCacheMan S2SCacheMan { get; protected set; }

        public int ConnectRetryIntervalSeconds { get; set; }
        #endregion

        #region Properties

        protected bool IsMaster { get; set; }

        #endregion

        #region Public Methods

        public byte GetCurrentNodeId()
        {
            return this.reader.ReadCurrentNodeId();
        }

        public void OnMasterConnectionEstablished(MasterServerConnectionBase masterServerConnectionBase)
        {
            this.serverStateManager.CheckAppOffline();
        }

        public CustomTypeCache GetS2SCustomTypeCache()
        {
            return this.S2SCacheMan.GetCustomTypeCache();
        }

        #endregion

        #region Methods

        private void SetupTokenCreator()
        {
            var sharedKey = Photon.Common.Authentication.Settings.Default.AuthTokenKey;
            if (string.IsNullOrEmpty(sharedKey))
            {
                log.WarnFormat("AuthTokenKey not specified in config. Authentication tokens are not supported");
                return;
            }

            var expirationTimeSeconds = Photon.Common.Authentication.Settings.Default.AuthTokenExpiration;
            //if (expirationTimeSeconds <= 0)
            //{
            //    log.ErrorFormat("Authentication token expiration to low: expiration={0} seconds", expirationTimeSeconds);
            //}

            var expiration = TimeSpan.FromSeconds(expirationTimeSeconds);
            this.TokenCreator = new AuthTokenFactory();
            this.TokenCreator.Initialize(sharedKey, expiration);

            log.InfoFormat("TokenCreator intialized with an expiration of {0}", expiration);
        }

        public void UpdateMasterEndPoint()
        {
            IPAddress masterAddress;
            if (!IPAddress.TryParse(GameServerSettings.Default.MasterIPAddress, out masterAddress))
            {
                var hostEntry = Dns.GetHostEntry(GameServerSettings.Default.MasterIPAddress);
                if (hostEntry.AddressList == null || hostEntry.AddressList.Length == 0)
                {
                    throw new ConfigurationException(
                        "MasterIPAddress setting is neither an IP nor an DNS entry: "
                        + GameServerSettings.Default.MasterIPAddress);
                }

                masterAddress =
                    hostEntry.AddressList.First(address => address.AddressFamily == AddressFamily.InterNetwork); 

                if (masterAddress == null)
                {
                    throw new ConfigurationException(
                        "MasterIPAddress does not resolve to an IPv4 address! Found: "
                        + string.Join(", ", hostEntry.AddressList.Select(a => a.ToString()).ToArray()));
                }
            }

            int masterPort = GameServerSettings.Default.OutgoingMasterServerPeerPort;
            this.MasterEndPoint = new IPEndPoint(masterAddress, masterPort);
        }

        /// <summary>
        ///   Sanity check to verify that game states are cleaned up correctly
        /// </summary>
        protected virtual void CheckGames()
        {
            var roomNames = this.GameCache.GetRoomNames();

            foreach (var roomName in roomNames)
            {
                Room room;
                if (this.GameCache.TryGetRoomWithoutReference(roomName, out room))
                {
                    room.EnqueueMessage(new RoomMessage((byte)GameMessageCodes.CheckGame));
                }
            }
        }

        protected virtual PeerBase CreateGamePeer(InitRequest initRequest)
        {
            var peer = new GameClientPeer(initRequest, this);
            {
                var settings = WebRpcSettings.Default;
                if (settings != null && settings.Enabled)
                {
                    peer.WebRpcHandler = new WebRpcHandler(
                        settings.BaseUrl.Value,
                        new Dictionary<string, object>
                        {
                            {"AppId", this.HwId},
                            {"AppVersion", ""},
                            {"Region", ""},
                            {"Cloud", ""},
                        }, 
                        null, 
                        new HttpRequestQueueOptions(httpQueueReconnectInterval: settings.ReconnectInterval * 1000));
                }
                initRequest.ResponseObject = "ResponseObject";
            }
            return peer;
        }

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("CreatePeer for {0}", initRequest.ApplicationId);
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat(
                    "incoming game peer at {0}:{1} from {2}:{3}", 
                    initRequest.LocalIP, 
                    initRequest.LocalPort, 
                    initRequest.RemoteIP, 
                    initRequest.RemotePort);
            }

            return this.CreateGamePeer(initRequest);
        }

        protected virtual void InitLogging()
        {
            LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");
            GlobalContext.Properties["LogFileName"] = "GS" + this.ApplicationName;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(this.BinaryPath, "log4net.config")));
        }

        protected override void OnServerConnectionFailed(int errorCode, string errorMessage, object state)
        {
            log.DebugFormat("OnServerConnectionFailed state={0}", state);

            var masterConnection = state as MasterServerConnectionBase;
            if (masterConnection != null)
            {
                masterConnection.OnConnectionFailed(errorCode, errorMessage);
                return;
            }

            var ipEndPoint = state as IPEndPoint;
            if (ipEndPoint == null)
            {
                log.ErrorFormat("Unknown connection failed with err {0}: {1}", errorCode, errorMessage);
            }
        }

        protected override void OnStopRequested()
        {
            log.InfoFormat("OnStopRequested: serverid={0}", ServerId);

            if (this.MasterServerConnection != null)
            {
                this.MasterServerConnection.Dispose();
            }

            if (this.WorkloadController != null)
            {
                this.WorkloadController.Stop();
            }

            if (this.MasterServerConnection != null)
            {
                this.MasterServerConnection.Dispose();
                this.MasterServerConnection = null;
            }

            base.OnStopRequested();
        }

        protected override void Setup()
        {
            this.InitLogging();

            log.InfoFormat("Setup: serverId={0}", ServerId);

            Protocol.AllowRawCustomValues = true;
            Protocol.RegisterTypeMapper(new UnknownTypeMapper());

            this.PublicIpAddress = PublicIPAddressReader.ParsePublicIpAddress(GameServerSettings.Default.PublicIPAddress);
            this.PublicIpAddressIPv6 = string.IsNullOrEmpty(GameServerSettings.Default.PublicIPAddressIPv6) ? null : IPAddress.Parse(GameServerSettings.Default.PublicIPAddressIPv6);

            this.IsMaster = PublicIPAddressReader.IsLocalIpAddress(this.MasterEndPoint.Address) || this.MasterEndPoint.Address.Equals(this.PublicIpAddress);

            Counter.IsMasterServer.RawValue = this.IsMaster ? 1 : 0;

            this.GameCache = new GameCache(this);

            if (CommonSettings.Default.EnablePerformanceCounters)
            {
                HttpQueuePerformanceCounters.Initialize();
            }
            else
            {
                log.Info("Performance counters are disabled");
            }

            this.SetupTokenCreator();
            this.SetupFeedbackControlSystem();
            this.SetupServerStateMonitor();
            this.SetupMasterConnection();

            if (GameServerSettings.Default.AppStatsPublishInterval > 0)
            {
                this.AppStatsPublisher = new ApplicationStatsPublisher(this, GameServerSettings.Default.AppStatsPublishInterval);
            }

            CounterPublisher.DefaultInstance.AddStaticCounterClass(typeof(Hive.Diagnostics.Counter), this.ApplicationName);
            CounterPublisher.DefaultInstance.AddStaticCounterClass(typeof(Counter), this.ApplicationName);

            this.executionFiber = new PoolFiber();
            this.executionFiber.Start();
            this.executionFiber.ScheduleOnInterval(this.CheckGames, 60000, 60000);
        }

        protected void SetupServerStateMonitor()
        {
            var serverStateFilePath = GameServerSettings.Default.ServerStateFile;

            this.serverStateManager = new ServerStateManager(this.WorkloadController);
            this.serverStateManager.OnNewServerState += OnNewServerState;

            if (string.IsNullOrEmpty(serverStateFilePath) == false)
            {
                this.serverStateManager.Start(Path.Combine(this.ApplicationRootPath, serverStateFilePath));
            }

            if (GameServerSettings.Default.EnableNamedPipe)
            {
                serverStateManager.StartListenPipe();
            }
        }

        protected virtual void SetupMasterConnection()
        {
            if (log.IsInfoEnabled)
            {
                log.Info("Initializing master server connection ...");
            }

            var masterAddress = GameServerSettings.Default.MasterIPAddress;
            var masterPost = GameServerSettings.Default.OutgoingMasterServerPeerPort;
            this.MasterServerConnection = new MasterServerConnection(this, masterAddress, masterPost, this.ConnectRetryIntervalSeconds);
            this.MasterServerConnection.Initialize();
        }

        protected void SetupFeedbackControlSystem()
        {
            var workLoadConfigFile = GameServerSettings.Default.WorkloadConfigFile;

            this.WorkloadController = new WorkloadController(
                this, this.PhotonInstanceName, 1000, this.ServerId.ToString(), workLoadConfigFile);

            if (!this.WorkloadController.IsInitialized)
            {
                const string message = "WorkloadController failed to be constructed";

                if (CommonSettings.Default.EnablePerformanceCounters)
                {
                    throw new Exception(message);
                }

                log.Warn(message);
            }

            this.WorkloadController.Start();
        }

        protected override void TearDown()
        {
            log.InfoFormat("TearDown: serverId={0}", ServerId);

            if (this.WorkloadController != null)
            {
                this.WorkloadController.Stop();
            }

            if (this.MasterServerConnection != null)
            {
                this.MasterServerConnection.Dispose();
            }

            if (this.serverStateManager != null)
            {
                this.serverStateManager.StopListenPipe();
            }
        }

        protected virtual void OnNewServerState(ServerState oldState, ServerState requestedState, TimeSpan offlineTime)
        {
            switch (requestedState)
            {
                case ServerState.Normal:
                case ServerState.OutOfRotation:
                    if (oldState == ServerState.Offline)
                    {
                        if (this.MasterServerConnection != null)
                        {
                            this.MasterServerConnection.UpdateAllGameStates();
                        }
                        else
                        {
                            log.WarnFormat("Server state is updated but there is not connection to master server");
                        }
                    }
                    break;

                case ServerState.Offline:
                    this.RaiseOfflineEvent(offlineTime);
                    break;
            }
        }


        protected virtual void RaiseOfflineEvent(TimeSpan time)
        {
           
        }

        #endregion
    }
}