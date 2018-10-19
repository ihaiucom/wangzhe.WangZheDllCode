// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameList.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GameList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Common;

namespace Photon.LoadBalancing.MasterServer.ChannelLobby
{
    #region using directives

    using System.Collections;
    using System.Collections.Generic;
    using ExitGames.Logging;

    using Photon.LoadBalancing.MasterServer.GameServer;
    using Photon.LoadBalancing.MasterServer.Lobby;
    using Photon.LoadBalancing.Operations;
    using Photon.LoadBalancing.ServerToServer.Events;
    using Photon.SocketServer;

    #endregion

    public class GameChannelList : GameListBase
    {
        #region Constants and Fields

        internal readonly Dictionary<GameChannelKey, GameChannel> GameChannels = new Dictionary<GameChannelKey, GameChannel>();

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors and Destructors

        public GameChannelList(AppLobby lobby)
            : base(lobby)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Creating new GameChannelList");
            }
        }

        #endregion

        #region Properties

        internal LinkedListDictionary<string, GameState> GameDict { get { return this.gameDict; } }

        #endregion

        #region Public Methods


        public override IGameListSubscription AddSubscription(PeerBase peer, Hashtable gamePropertyFilter, int maxGameCount)
        {
            if (gamePropertyFilter == null)
            {
                gamePropertyFilter = new Hashtable(0);
            }

            GameChannel gameChannel;
            var key = new GameChannelKey(gamePropertyFilter);

            if (!this.GameChannels.TryGetValue(key, out gameChannel))
            {
                gameChannel = new GameChannel(this, key);
                this.GameChannels.Add(key, gameChannel);
            }

            return gameChannel.AddSubscription(peer, maxGameCount);
        }

        protected override bool RemoveGameState(GameState gameState)
        {
            if (log.IsDebugEnabled)
            {
                LogGameState("RemoveGameState:", gameState);
            }

            foreach (GameChannel channel in this.GameChannels.Values)
            {
                channel.OnGameRemoved(gameState);
            }

            this.gameDict.Remove(gameState.Id);
            this.PlayerCount -= gameState.PlayerCount;

            return true;
        }

        public override ErrorCode TryGetRandomGame(JoinRandomGameRequest joinRequest, ILobbyPeer peer, out GameState gameState, out string message)
        {
            message = null;

            foreach (GameState game in this.gameDict)
            {
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


                gameState = game;
                return ErrorCode.Ok;
            }

            gameState = null;
            return ErrorCode.NoMatchFound;
        }

        public override bool UpdateGameState(UpdateGameEvent updateOperation, IncomingGameServerPeer gameServerPeer, out GameState gameState)
        {
            // try to get the game state 
            if (this.gameDict.TryGetValue(updateOperation.GameId, out gameState) == false)
            {
                if (updateOperation.Reinitialize)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Reinitialize: Add Game State {0}", updateOperation.GameId);
                    }

                    gameState = new GameState(this.Lobby, updateOperation.GameId, 0, gameServerPeer);

                    this.AddGameState(gameState);
                }
                else
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("Game not found: {0}", updateOperation.GameId);
                    }

                    return false;
                }
            }

            bool oldVisible = gameState.IsVisbleInLobby;
            bool changed = gameState.Update(updateOperation);

            if (!changed)
            {
                return false;
            }

            if (log.IsDebugEnabled)
            {
                LogGameState("UpdateGameState: ", gameState);
            }

            if (gameState.IsVisbleInLobby)
            {
                foreach (GameChannel channel in this.GameChannels.Values)
                {
                    channel.OnGameUpdated(gameState);
                }

                return true;
            }

            if (oldVisible)
            {
                foreach (GameChannel channel in this.GameChannels.Values)
                {
                    channel.OnGameRemoved(gameState);
                }
            }

            return true;
        }

        public override void PublishGameChanges()
        {
            foreach (var channel in this.GameChannels.Values)
            {
                channel.PublishGameChanges();
            }
        }

        #endregion

    }
}