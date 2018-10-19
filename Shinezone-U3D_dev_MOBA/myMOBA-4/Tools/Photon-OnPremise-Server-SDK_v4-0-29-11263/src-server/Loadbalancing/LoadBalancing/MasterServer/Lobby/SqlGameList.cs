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

    using ExitGames.Logging;
    using Photon.LoadBalancing.MasterServer.GameServer;
    using Photon.LoadBalancing.Operations;
    using Photon.LoadBalancing.ServerToServer.Events;

    #endregion

    public class SqlGameList : GameListBase
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly GameTable gameDatabase = new GameTable(10, "C");

        #endregion

        #region Constructors and Destructors

        public SqlGameList(AppLobby lobby)
            : base(lobby)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Creating new SqlGameList");
            }
        }

        #endregion

        
        #region Public Methods

        public override void AddGameState(GameState gameState)
        {
            base.AddGameState(gameState);

            if (gameState.IsJoinable)
            {
                this.gameDatabase.InsertGameState(gameState.Id, gameState.Properties);
            }
        }

        protected override bool RemoveGameState(GameState gameState)
        {
            this.gameDatabase.Delete(gameState.Id);
            return base.RemoveGameState(gameState);
        }

        public override ErrorCode TryGetRandomGame(JoinRandomGameRequest joinRequest, ILobbyPeer peer, out GameState gameState, out string message)
        {
            message = null;

            if (this.gameDict.Count == 0)
            {
                gameState = null;
                return ErrorCode.NoMatchFound;
            }

            if (string.IsNullOrEmpty(joinRequest.QueryData))
            {
                var node = this.gameDict.First;
                while (node != null)
                {
                    gameState = node.Value;
                    if (gameState.IsJoinable)
                    {
                        if (!gameState.CheckUserIdOnJoin 
                            || (!gameState.ContainsUser(peer.UserId) 
                                && !gameState.IsUserInExcludeList(peer.UserId)
                                && gameState.CheckSlots(peer.UserId, joinRequest.AddUsers)))
                        {
                            return ErrorCode.Ok;
                        }
                    }

                    node = node.Next;
                }

                gameState = null;
                return ErrorCode.NoMatchFound;
            }

            string id;
            try
            {
                id = this.gameDatabase.FindMatch(joinRequest.QueryData);
            }
            catch (System.Data.Common.DbException sqlException)
            {
                gameState = null;
                message = sqlException.Message;
                return ErrorCode.OperationInvalid;
            }

            if (string.IsNullOrEmpty(id))
            {
                gameState = null;
                return ErrorCode.NoMatchFound;
            }

            if (!this.gameDict.TryGet(id, out gameState))
            {
                return ErrorCode.NoMatchFound;
            }

            return ErrorCode.Ok;
        }

        public override void OnGameJoinableChanged(GameState gameState)
        {
            var isJoinable = gameState.IsJoinable;

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("OnGameJoinableChanged: gameId={0}, joinable={1}", gameState.Id, isJoinable);
            }

            if (isJoinable)
            {
                this.gameDatabase.InsertGameState(gameState.Id, gameState.Properties);
            }
            else
            {
                this.gameDatabase.Delete(gameState.Id);
            }
        }

        public override bool UpdateGameState(UpdateGameEvent updateOperation, IncomingGameServerPeer incomingGameServerPeer,
            out GameState gameState)
        {
            if (base.UpdateGameState(updateOperation, incomingGameServerPeer, out gameState))
            {
                if (gameState.IsJoinable)
                {
                    this.gameDatabase.Update(gameState.Id, gameState.Properties);
                    return true;
                }
            }
            return false;
        }

        #endregion

    }
}