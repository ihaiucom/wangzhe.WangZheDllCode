// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameServerState.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GameServerState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Common.LoadBalancer.LoadShedding;

namespace Photon.LoadBalancing.MasterServer.GameServer
{
    #region using directives

    using System;

    #endregion

    public class GameServerState : IComparable<GameServerState>
    {
        public GameServerState(FeedbackLevel loadLevel, int peerCount)
        {
            this.LoadLevel = loadLevel;
            this.PeerCount = peerCount;
        }

        public FeedbackLevel LoadLevel { get; private set; }

        public int PeerCount { get; private set; }

        public static bool operator >(GameServerState a, GameServerState b)
        {
            if (a.LoadLevel > b.LoadLevel)
            {
                return true;
            }

            return a.PeerCount > b.PeerCount;
        }

        public static bool operator <(GameServerState a, GameServerState b)
        {
            if (a.LoadLevel < b.LoadLevel)
            {
                return true;
            }

            return a.PeerCount < b.PeerCount;
        }

        public int CompareTo(GameServerState other)
        {
            if (this < other)
            {
                return -1;
            }

            if (this > other)
            {
                return 1;
            }

            return 0;
        }
    }
}