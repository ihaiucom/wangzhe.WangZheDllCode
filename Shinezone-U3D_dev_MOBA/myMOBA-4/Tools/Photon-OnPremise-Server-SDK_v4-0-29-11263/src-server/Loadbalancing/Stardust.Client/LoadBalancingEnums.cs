// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadBalancingEnums.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.StarDust.Client
{
    // TODO: use the Client LoadBalancing API instead!
    public class LoadBalancingEnums
    {
        public enum OperationCode : byte
        {
            Authenticate = 230,
            JoinLobby = 229,
            LeaveLobby = 228,
            CreateGame = 227,
            JoinGame = 226,
            JoinRandomGame = 225,
            CancelJoinRandomGame = 224,
            DebugGame = 223
        }

        public enum ParameterCode : byte
        {
            Address = 230,
            PeerCount = 229,
            GameCount = 228,
            MasterPeerCount = 227,
            GameId = ExitGames.Client.Photon.LoadBalancing.ParameterCode.RoomName,
            UserId = 225,
            ApplicationId = 224,
            Position = 223,
            GameList = 222,
            Secret = 221,
            AppVersion = 220,
            NodeId = 219,
            Info = 218
        }

        public enum EventCode: byte
        {
            GameList = 230,
            GameListUpdate = 229,
            QueueState = 228,
            AppStats = 226,
            GameServerOffline = 225
        }
    }
}
