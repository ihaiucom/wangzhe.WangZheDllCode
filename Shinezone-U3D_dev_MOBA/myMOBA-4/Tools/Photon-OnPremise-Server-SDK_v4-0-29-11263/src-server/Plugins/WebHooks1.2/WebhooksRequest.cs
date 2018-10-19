using System;
using System.Collections.Generic;

namespace Photon.Hive.Plugin.WebHooks
{
    // Note: renaming Fields will break scripts and tests
    internal class WebhooksRequest
    {
        #region Public Properties

        public int? ActorCount { get; set; }

        public int? ActorNr { get; set; }

        public string AppVersion { get; set; }

        public string AppId { get; set; }

        public bool? CreateIfNotExists { get; set; }

        public object CreateOptions { get; set; }

        public object Data { get; set; }

        public object Properties { get; set; }

        public string GameId { get; set; }

        public bool? IsInactive { get; set; }

        public string Reason { get; set; }

        public string Region { get; set; }

        public SerializableGameState State { get; set; }

        public string Type { get; set; }

        public string UserId { get; set; }

        public string Nickname { get; set; }

        [Obsolete("Use AuthCookie instead")]
        public object Secure { get; set; }

        public Dictionary<string, object> AuthCookie { get; set; }

        public byte? EvCode { get; set; }

        public int? TargetActor { get; set; }
        #endregion
    }
}