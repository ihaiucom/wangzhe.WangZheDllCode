// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameList.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GameList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Common;

namespace Photon.LoadBalancing.MasterServer.Lobby
{
    #region using directives

    using System;
    using System.Collections.Generic;

    using ExitGames.Logging;
    using Photon.LoadBalancing.Operations;

    #endregion

    public class GameList : GameListBase
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Random rnd = new Random();

        #endregion

        #region Constructors and Destructors

        public GameList(AppLobby lobby)
            : base(lobby)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Creating new GameList");
            }
        }

        #endregion

        #region Public Methods

        public override ErrorCode TryGetRandomGame(JoinRandomGameRequest joinRequest, ILobbyPeer peer, out GameState gameState, out string message)
        {
            message = null;

            if (this.gameDict.Count == 0)
            {
                gameState = null;
                return ErrorCode.NoMatchFound;
            }

            LinkedListNode<GameState> startNode;
            switch ((JoinRandomType)joinRequest.JoinRandomType)
            {
                default:
                case JoinRandomType.FillRoom:
                    startNode = this.gameDict.First;
                    break;

                case JoinRandomType.SerialMatching:
                    startNode = this.nextJoinRandomStartNode ?? this.gameDict.First;
                    break;

                case JoinRandomType.RandomMatching:
                    var startNodeIndex = this.rnd.Next(this.gameDict.Count);
                    startNode = this.gameDict.GetAtIndex(startNodeIndex);
                    break;
            }

            if (!TryGetRandomGame(startNode, joinRequest, peer, out gameState))
            {
                return ErrorCode.NoMatchFound;
            }

            return ErrorCode.Ok;
        }

        #endregion

        #region Methods

        private bool TryGetRandomGame(LinkedListNode<GameState> startNode, JoinRandomGameRequest joinRequest, ILobbyPeer peer, out GameState gameState)
        {
            var node = startNode;

            do
            {
                var game = node.Value;
                node = node.Next ?? this.gameDict.First;

                if (!game.IsOpen || !game.IsVisible || !game.HasBeenCreatedOnGameServer || (game.MaxPlayer > 0 && game.PlayerCount >= game.MaxPlayer))
                {
                    continue;
                }

                if (joinRequest.GameProperties != null && game.MatchGameProperties(joinRequest.GameProperties) == false)
                {
                    continue;
                }

                if (game.CheckUserIdOnJoin 
                    && (game.ContainsUser(peer.UserId)
                    || game.IsUserInExcludeList(peer.UserId)
                    || !game.CheckSlots(peer.UserId, joinRequest.AddUsers)))
                {
                    continue;
                }

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Found match. Next start node gameid={0}", node.Value.Id);
                }

                this.nextJoinRandomStartNode = node;
                gameState = game;
                return true;
            }
            while (node != startNode);

            gameState = null;
            return false;
        }

        #endregion
    }
}