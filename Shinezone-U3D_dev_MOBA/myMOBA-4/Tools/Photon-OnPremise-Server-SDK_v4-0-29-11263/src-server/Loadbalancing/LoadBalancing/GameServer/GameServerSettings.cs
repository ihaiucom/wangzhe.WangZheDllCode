namespace Photon.LoadBalancing.GameServer
{
    using System;
    using System.Configuration;
    using System.Diagnostics;

    public sealed class GameServerSettings : ApplicationSettingsBase
    {
        #region Static Fields

        private static readonly GameServerSettings defaultInstance =
            ((GameServerSettings)(Synchronized(new GameServerSettings())));

        #endregion

        #region Public Properties

        public static GameServerSettings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("1000")]
        public int AppStatsPublishInterval
        {
            get
            {
                return ((int)(this["AppStatsPublishInterval"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("15")]
        public int ConnectReytryInterval
        {
            get
            {
                return ((int)(this["ConnectReytryInterval"]));
            }
        }


        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool EnableNamedPipe
        {
            get
            {
                return ((bool)(this["EnableNamedPipe"]));
            }
        }

         
        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("photon/g")]
        public string GamingHttpPath
        {
            get
            {
                return ((string)(this["GamingHttpPath"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("80")]
        public int GamingHttpPort
        {
            get
            {
                return ((int)(this["GamingHttpPort"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("443")]
        public int GamingHttpsPort
        {
            get
            {
                return ((int)(this["GamingHttpsPort"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("19091")]
        public int GamingSecureWebSocketPort
        {
            get
            {
                return ((int)(this["GamingSecureWebSocketPort"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("4531")]
        public int GamingTcpPort
        {
            get
            {
                return ((int)(this["GamingTcpPort"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("5056")]
        public int GamingUdpPort
        {
            get
            {
                return ((int)(this["GamingUdpPort"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("9091")]
        public int GamingWebSocketPort
        {
            get
            {
                return ((int)(this["GamingWebSocketPort"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("30")]
        public int HttpQueueMaxErrors
        {
            get
            {
                return ((int)(this["HttpQueueMaxErrors"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("30")]
        public int HttpQueueMaxTimeouts
        {
            get
            {
                return ((int)(this["HttpQueueMaxTimeouts"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("30000")]
        public int HttpQueueRequestTimeout
        {
            get
            {
                return ((int)(this["HttpQueueRequestTimeout"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("10000")]
        public int HttpQueueMaxBackoffTime
        {
            get
            {
                return ((int)(this["HttpQueueMaxBackoffTime"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("5000")]
        public int HttpQueueMaxQueuedRequests
        {
            get
            {
                return ((int)(this["HttpQueueMaxQueuedRequests"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("90000")]
        public int HttpQueueQueueTimeout
        {
            get
            {
                return ((int)(this["HttpQueueQueueTimeout"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("60000")]
        public int HttpQueueReconnectInterval
        {
            get
            {
                return ((int)(this["HttpQueueReconnectInterval"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("1")]
        public int HttpQueueMaxConcurrentRequests
        {
            get
            {
                return ((int)(this["HttpQueueMaxConcurrentRequests"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("60")]
        public int LastTouchCheckIntervalSeconds
        {
            get
            {
                return ((int)(this["LastTouchCheckIntervalSeconds"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        public int LastTouchSecondsDisconnect
        {
            get
            {
                return ((int)(this["LastTouchSecondsDisconnect"]));
            }
        }
        

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("127.0.0.1")]
        public string MasterIPAddress
        {
            get
            {
                return ((string)(this["MasterIPAddress"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("60000")]
        public int MaxEmptyRoomTTL
        {
            get
            {
                return ((int)(this["MaxEmptyRoomTTL"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("4520")]
        public int OutgoingMasterServerPeerPort
        {
            get
            {
                return ((int)(this["OutgoingMasterServerPeerPort"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("127.0.0.1")]
        public string PublicIPAddress
        {
            get
            {
                return ((string)(this["PublicIPAddress"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue(null)]
        public string PublicIPAddressIPv6
        {
            get
            {
                return ((string)(this["PublicIPAddressIPv6"]));
            }
        }


        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue(null)]
        public string PublicHostName
        {
            get
            {
                return this["PublicHostName"] == null ? null : Environment.ExpandEnvironmentVariables((string)(this["PublicHostName"]));
            }
        }
        

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        public int RelayPortHttp
        {
            get
            {
                return ((int)(this["RelayPortHttp"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        public int RelayPortSecureWebSocket
        {
            get
            {
                return ((int)(this["RelayPortSecureWebSocket"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        public int RelayPortTcp
        {
            get
            {
                return ((int)(this["RelayPortTcp"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        public int RelayPortUdp
        {
            get
            {
                return ((int)(this["RelayPortUdp"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        public int RelayPortWebSocket
        {
            get
            {
                return ((int)(this["RelayPortWebSocket"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("ServerState.txt")]
        public string ServerStateFile
        {
            get
            {
                return ((string)(this["ServerStateFile"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("Workload.config")]
        public string WorkloadConfigFile
        {
            get
            {
                return ((string)(this["WorkloadConfigFile"]));
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        public int HttpForwardLimit
        {
            get
            {
                return ((int)(this["HttpForwardLimit"]));
            }
        }

        #endregion
    }
}