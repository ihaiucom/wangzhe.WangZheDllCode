// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppLobby.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AppLobby type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ExitGames.Threading;
using System.Text;
using Photon.Common;
using Photon.LoadBalancing.Common;

namespace Photon.LoadBalancing.MasterServer.Lobby
{
    #region using directives

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using ExitGames.Concurrency.Fibers;
    using ExitGames.Logging;

    using Photon.LoadBalancing.Events;
    using Photon.LoadBalancing.MasterServer.ChannelLobby;
    using Photon.LoadBalancing.MasterServer.GameServer;
    using Photon.LoadBalancing.Operations;
    using Photon.LoadBalancing.ServerToServer.Events;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using Photon.Hive.Operations;

    using OperationCode = Photon.LoadBalancing.Operations.OperationCode;
    using EventCode = Photon.LoadBalancing.Events.EventCode;
    using Photon.Hive.Common.Lobby;
    #endregion

    public class AppLobby
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public readonly GameApplication Application;

        public readonly string LobbyName;

        public readonly AppLobbyType LobbyType;

        public readonly TimeSpan JoinTimeOut = TimeSpan.FromSeconds(5);

        public readonly int MaxPlayersDefault; 

        internal readonly IGameList GameList;

        private readonly HashSet<PeerBase> peers = new HashSet<PeerBase>();

        private readonly int gameChangesPublishInterval = 1000;

        private readonly int gameListLimit;

        private IDisposable schedule;

        private IDisposable checkJoinTimeoutSchedule;

        #endregion

        #region Constructors and Destructors

        public AppLobby(GameApplication application, string lobbyName, AppLobbyType lobbyType)
            : this(application, lobbyName, lobbyType, 0, TimeSpan.FromSeconds(15))
        {
        }

        public AppLobby(GameApplication application, string lobbyName, AppLobbyType lobbyType, int maxPlayersDefault, TimeSpan joinTimeOut)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Creating lobby: name={0}, type={1}", lobbyName, lobbyType);
            }

            this.Application = application;
            this.LobbyName = lobbyName;
            this.LobbyType = lobbyType;
            this.MaxPlayersDefault = maxPlayersDefault;
            this.JoinTimeOut = joinTimeOut;
            this.gameListLimit = MasterServerSettings.Default.GameListLimit;

            if (MasterServerSettings.Default.GameChangesPublishInterval > 0)
            {
                this.gameChangesPublishInterval = MasterServerSettings.Default.GameChangesPublishInterval;
            }

            switch (lobbyType)
            {
                default:
                    this.GameList = new GameList(this);
                    break;

                case AppLobbyType.ChannelLobby:
                    this.GameList = new GameChannelList(this);
                    break;

                case AppLobbyType.SqlLobby:
                    this.GameList = new SqlGameList(this);
                    break;
                case AppLobbyType.AsyncRandomLobby:
                    this.GameList = new AsyncRandomGameList(this);
                    break;
            }

            this.ExecutionFiber = new PoolFiber();
            this.ExecutionFiber.Start();
        }

        #endregion

        #region Properties

        public PoolFiber ExecutionFiber { get; protected set; }

        /// <summary>
        /// Gets the number of peers in the lobby.
        /// </summary>
        public int PeerCount
        {
            get
            {
                return this.peers.Count;
            }
        }

        /// <summary>
        /// Gets the total number of players in all games in this lobby.
        /// </summary>
        public int PlayerCount
        {
            get
            {
                return this.GameList.PlayerCount;
            }
        }

        public int GameCount
        {
            get
            {
                return this.GameList.Count;
            }
        }

        #endregion

        #region Public Methods

        public void EnqueueOperation(MasterClientPeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            this.ExecutionFiber.Enqueue(() => this.ExecuteOperation(peer, operationRequest, sendParameters));
        }

        public void RemoveGame(string gameId)
        {
            this.ExecutionFiber.Enqueue(() => this.HandleRemoveGameState(gameId));
        }

        public void RemoveGameServer(IncomingGameServerPeer gameServer)
        {
            this.ExecutionFiber.Enqueue(() => this.HandleRemoveGameServer(gameServer));
        }

        public void RemovePeer(MasterClientPeer serverPeer)
        {
            this.ExecutionFiber.Enqueue(() => this.HandleRemovePeer(serverPeer));
        }

        public void UpdateGameState(UpdateGameEvent operation, IncomingGameServerPeer incomingGameServerPeer)
        {
            this.ExecutionFiber.Enqueue(() => this.HandleUpdateGameState(operation, incomingGameServerPeer));
        }

        public void JoinLobby(MasterClientPeer peer, JoinLobbyRequest joinLobbyrequest, SendParameters sendParameters)
        {
            this.ExecutionFiber.Enqueue(() => this.HandleJoinLobby(peer, joinLobbyrequest, sendParameters));
        }

        #endregion

        #region Methods

        protected virtual void ExecuteOperation(MasterClientPeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            OperationResponse response;

            try
            {
                switch ((OperationCode)operationRequest.OperationCode)
                {
                    default:
                        response = new OperationResponse(operationRequest.OperationCode)
                        {
                            ReturnCode = (short)ErrorCode.OperationInvalid,
                            DebugMessage = LBErrorMessages.UnknownOperationCode
                        };
                        break;

                    case OperationCode.JoinLobby:
                        var joinLobbyRequest = new JoinLobbyRequest(peer.Protocol, operationRequest);
                        if (OperationHelper.ValidateOperation(joinLobbyRequest, log, out response))
                        {
                            response = this.HandleJoinLobby(peer, joinLobbyRequest, sendParameters);
                        }
                        break;

                    case OperationCode.LeaveLobby:
                        response = this.HandleLeaveLobby(peer, operationRequest);
                        break;

                    case OperationCode.CreateGame:
                        response = this.HandleCreateGame(peer, operationRequest);
                        break;

                    case OperationCode.JoinGame:
                        response = this.HandleJoinGame(peer, operationRequest);
                        break;

                    case OperationCode.JoinRandomGame:
                        response = this.HandleJoinRandomGame(peer, operationRequest);
                        break;

                    case OperationCode.DebugGame:
                        response = this.HandleDebugGame(peer, operationRequest);
                        break;
                }

            }
            catch (Exception ex)
            {
                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.InternalServerError,
                    DebugMessage = ex.Message
                };
                log.Error(ex);
            }
            try
            {
                if (response != null)
                {
                    peer.SendOperationResponse(response, sendParameters);
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        protected virtual object GetCreateGameResponse(MasterClientPeer peer, GameState gameState)
        {
            return new CreateGameResponse { GameId = gameState.Id, Address = gameState.GetServerAddress(peer) };
        }

        protected virtual object GetJoinGameResponse(MasterClientPeer peer, GameState gameState)
        {
            return new JoinGameResponse { Address = gameState.GetServerAddress(peer) };
        }

        protected virtual object GetJoinRandomGameResponse(MasterClientPeer peer, GameState gameState)
        {
            return new JoinRandomGameResponse { GameId = gameState.Id, Address = gameState.GetServerAddress(peer) };
        }

        protected virtual DebugGameResponse GetDebugGameResponse(MasterClientPeer peer, GameState gameState)
        {
            return new DebugGameResponse
                {
                    Address = gameState.GetServerAddress(peer), 
                    Info = gameState.ToString()
                };
        }

        protected virtual OperationResponse HandleCreateGame(MasterClientPeer peer, OperationRequest operationRequest)
        {
            // validate the operation request
            OperationResponse response;
            var operation = new CreateGameRequest(peer.Protocol, operationRequest);
            if (OperationHelper.ValidateOperation(operation, log, out response) == false)
            {
                return response;
            }

            // if no gameId is specified by the client generate a unique id 
            if (string.IsNullOrEmpty(operation.GameId))
            {
                operation.GameId = Guid.NewGuid().ToString();
            }
           
            // try to create game
            GameState gameState;
            bool gameCreated;
            if (!this.TryCreateGame(operation, operation.GameId, false, operation.GameProperties, out gameCreated, out gameState, out response))
            {
                return response;
            }

            // add peer to game
            gameState.AddPeer(peer);

            this.ScheduleCheckJoinTimeOuts();

            // publish operation response
            var createGameResponse = this.GetCreateGameResponse(peer, gameState);
            return new OperationResponse(operationRequest.OperationCode, createGameResponse);
        }

        protected virtual OperationResponse HandleJoinGame(MasterClientPeer peer, OperationRequest operationRequest)
        {
            // validate operation
            var operation = new JoinGameRequest(peer.Protocol, operationRequest);
            OperationResponse response;
            if (OperationHelper.ValidateOperation(operation, log, out response) == false)
            {
                return response;
            }


            // try to find game by id
            GameState gameState;
            bool gameCreated = false;
            if (operation.JoinMode == JoinModes.JoinOnly && !this.Application.PluginTraits.AllowAsyncJoin)
            {
                // The client does not want to create the game if it does not exists.
                // In this case the game must have been created on the game server before it can be joined.
                if (this.GameList.TryGetGame(operation.GameId, out gameState) == false || gameState.HasBeenCreatedOnGameServer == false)
                {
                    return new OperationResponse
                    {
                        OperationCode = operationRequest.OperationCode, 
                        ReturnCode = (short)ErrorCode.GameIdNotExists, 
                        DebugMessage = LBErrorMessages.GameIdDoesNotExist
                    };
                }
            }
            else
            {
                // The client will create the game if it does not exists already.
                if (!this.GameList.TryGetGame(operation.GameId, out gameState))
                {
                    if (!this.TryCreateGame(operation, operation.GameId, true, operation.GameProperties, out gameCreated, out gameState, out response))
                    {
                        return response;
                    }
                }
            }

            if (gameState.IsUserInExcludeList(peer.UserId))
            {
                //ciao
                return new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = (short)ErrorCode.JoinFailedFoundExcludedUserId, 
                    DebugMessage = HiveErrorMessages.JoinFailedFoundExcludedUserId };

            }
            // ValidateGame checks isOpen and maxplayers 
            // and does not apply to new games & rejoins
            //var actorIsRejoining = operation.ActorNr != 0;
            //if (gameCreated == false && !actorIsRejoining)
            if (gameCreated == false && !operation.IsRejoining)
            {
                // check if max players of the game is already reached1
                if (gameState.MaxPlayer > 0 && gameState.PlayerCount >= gameState.MaxPlayer && !gameState.IsUserExpected(peer.UserId))
                {
                    return new OperationResponse
                    {
                        OperationCode = operationRequest.OperationCode, 
                        ReturnCode = (short)ErrorCode.GameFull, DebugMessage = LBErrorMessages.GameFull
                    };
                }

                // check if the game is open
                if (gameState.IsOpen == false)
                {
                    return new OperationResponse
                    {
                        OperationCode = operationRequest.OperationCode, 
                        ReturnCode = (short)ErrorCode.GameClosed, 
                        DebugMessage = LBErrorMessages.GameClosed
                    };
                }

                if (operation.CheckUserOnJoin && gameState.ContainsUser(peer.UserId))
                {
                    return new OperationResponse 
                    { 
                        OperationCode = operationRequest.OperationCode, 
                        ReturnCode = (short)ErrorCode.InternalServerError, 
                        DebugMessage = string.Format(LBErrorMessages.UserAlreadyJoined, peer.UserId, operation.JoinMode)
                    };
                }
            }

            string errMsg;
            if (!gameState.CheckSlots(peer.UserId, operation.AddUsers, out errMsg))
            {
                return new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = (short)ErrorCode.InternalServerError, DebugMessage = errMsg };
            }

            // add peer to game
            gameState.AddPeer(peer);
            gameState.AddSlots(operation);

            this.ScheduleCheckJoinTimeOuts();

            // publish operation response
            var joinResponse = this.GetJoinGameResponse(peer, gameState);
            return new OperationResponse(operationRequest.OperationCode, joinResponse);
        }

        protected virtual OperationResponse HandleJoinLobby(MasterClientPeer peer, JoinLobbyRequest operation, SendParameters sendParameters)
        {
            try
            {
                peer.GameChannelSubscription = null;

                if (operation.GameListCount > 0 && this.gameListLimit > 0)
                {
                    if (operation.GameListCount > this.gameListLimit)
                    {
                        operation.GameListCount = this.gameListLimit;
                    }
                }
              
                var subscription = this.GameList.AddSubscription(peer, operation.GameProperties, operation.GameListCount);
                peer.GameChannelSubscription = subscription;
                peer.SendOperationResponse(new OperationResponse(operation.OperationRequest.OperationCode), sendParameters);


                if (subscription != null)
                {
                    // publish game list to peer after the response has been sent
                    var gameList = subscription.GetGameList();

                    if (gameList.Count != 0)
                    {
                        var sb = new StringBuilder();
                        foreach (var game in gameList.Keys)
                        {
                            sb.AppendFormat("{0};", game);
                        }

                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("Game list is: {0}", sb.ToString());
                        }
                    }

                    var e = new GameListEvent { Data = gameList };
                    var eventData = new EventData((byte)EventCode.GameList, e);
                    peer.SendEvent(eventData, new SendParameters());
                }

                this.peers.Add(peer);
                return null;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        protected virtual OperationResponse HandleJoinRandomGame(MasterClientPeer peer, OperationRequest operationRequest)
        {
            // validate the operation request
            var operation = new JoinRandomGameRequest(peer.Protocol, operationRequest);
            OperationResponse response;
            if (OperationHelper.ValidateOperation(operation, log, out response) == false)
            {
                return response;
            }

            // try to find a match
            GameState game;
            string errorMessage;
            var result = this.GameList.TryGetRandomGame(operation, peer, out game, out errorMessage);
            if (result != ErrorCode.Ok)
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = "No match found";
                }

                response = new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = (short)result, DebugMessage = errorMessage};
                return response;
            }

            // match found, add peer to game and notify the peer
            game.AddPeer(peer);

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Found match: connectionId={0}, userId={1}, gameId={2}", peer.ConnectionId, peer.UserId, game.Id);
            }

            this.ScheduleCheckJoinTimeOuts();

            object joinResponse = this.GetJoinRandomGameResponse(peer, game);
            return new OperationResponse(operationRequest.OperationCode, joinResponse);
        }

        protected virtual OperationResponse HandleLeaveLobby(MasterClientPeer peer, OperationRequest operationRequest)
        {
            peer.GameChannelSubscription = null;

            this.GameList.RemoveSubscription(peer);
            if (this.peers.Remove(peer))
            {
                return new OperationResponse { OperationCode = operationRequest.OperationCode };
            }

            return new OperationResponse
            {
                OperationCode = operationRequest.OperationCode, 
                ReturnCode = (short)ErrorCode.Ok, 
                DebugMessage = LBErrorMessages.LobbyNotJoined,
            };
        }

        protected virtual OperationResponse HandleDebugGame(MasterClientPeer peer, OperationRequest operationRequest)
        {
            var operation = new DebugGameRequest(peer.Protocol, operationRequest);
            OperationResponse response; 
            if (OperationHelper.ValidateOperation(operation, log, out response) == false)
            {
                return response; 
            }

            GameState gameState;
            if (this.GameList.TryGetGame(operation.GameId, out gameState) == false)
            {
                return new OperationResponse
                    {
                        OperationCode = operationRequest.OperationCode, 
                        ReturnCode = (short)ErrorCode.GameIdNotExists, 
                        DebugMessage = LBErrorMessages.GameIdDoesNotExist
                    };
            }

            var debugGameResponse = this.GetDebugGameResponse(peer, gameState); 

            log.InfoFormat("DebugGame: {0}", debugGameResponse.Info);

            return new OperationResponse(operationRequest.OperationCode, debugGameResponse);
        }
        
        protected virtual void OnGameStateChanged(GameState gameState)
        {
        }

        protected virtual void OnRemovePeer(MasterClientPeer peer)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("peer removed from lobby:l:'{0}',p:'{1}',u:'{2}'", this, peer, peer.UserId);
            }
        }

        private void ScheduleCheckJoinTimeOuts()
        {
            if (this.checkJoinTimeoutSchedule == null)
            {
                this.checkJoinTimeoutSchedule = this.ExecutionFiber.Schedule(this.CheckJoinTimeOuts, (long)this.JoinTimeOut.TotalMilliseconds / 2);
            }
        }

        private void CheckJoinTimeOuts()
        {
            try
            {
                this.checkJoinTimeoutSchedule.Dispose();
                var joiningPlayersLeft = this.GameList.CheckJoinTimeOuts(this.JoinTimeOut);
                if (joiningPlayersLeft > 0)
                {
                    this.ExecutionFiber.Schedule(this.CheckJoinTimeOuts, (long)this.JoinTimeOut.TotalMilliseconds / 2);
                }
                else
                {
                    this.checkJoinTimeoutSchedule = null;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void HandleRemoveGameServer(IncomingGameServerPeer gameServer)
        {
            try
            {
                this.GameList.RemoveGameServer(gameServer);
                this.SchedulePublishGameChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void HandleRemoveGameState(string gameId)
        {
            try
            {
                GameState gameState;
                if (this.GameList.TryGetGame(gameId, out gameState) == false)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("HandleRemoveGameState: Game not found - gameId={0}", gameId);
                    }

                    return;
                }

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("HandleRemoveGameState: gameId={0}, joiningPlayers={1}", gameId, gameState.JoiningPlayerCount);
                }

                this.GameList.RemoveGameState(gameId);
                this.SchedulePublishGameChanges();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void HandleRemovePeer(MasterClientPeer peer)
        {
            try
            {
                peer.GameChannelSubscription = null;
                this.GameList.RemoveSubscription(peer);
                this.peers.Remove(peer);
                this.OnRemovePeer(peer);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void HandleUpdateGameState(UpdateGameEvent operation, IncomingGameServerPeer incomingGameServerPeer)
        {
            try
            {
                GameState gameState;

                if (this.GameList.UpdateGameState(operation, incomingGameServerPeer, out gameState) == false)
                {
                    return;
                }

                this.SchedulePublishGameChanges();

                this.OnGameStateChanged(gameState);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void SchedulePublishGameChanges()
        {
            if (this.schedule == null)
            {
                this.schedule = this.ExecutionFiber.Schedule(this.PublishGameChanges, this.gameChangesPublishInterval);
            }
        }

        private void PublishGameChanges()
        {
            try
            {
                this.schedule = null;
                this.GameList.PublishGameChanges();                
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private bool TryCreateGame(Operation operation, string gameId, bool createIfNotExists, Hashtable properties, out bool gameCreated, out GameState gameState, out OperationResponse errorResponse)
        {
            gameState = null;
            gameCreated = false;

            // try to get a game server instance from the load balancer            
            IncomingGameServerPeer gameServer;
            if (!this.Application.LoadBalancer.TryGetServer(out gameServer))
            {
                errorResponse = new OperationResponse(operation.OperationRequest.OperationCode)
                    {
                        ReturnCode = (short)ErrorCode.ServerFull,
                        DebugMessage = LBErrorMessages.FailedToGetServerInstance,
                    };

                return false;
            }

            // try to create or get game state
            if (createIfNotExists)
            {
                gameCreated = this.Application.GetOrCreateGame(gameId, this, (byte)this.MaxPlayersDefault, gameServer, out gameState);
            }
            else
            {
                if (!this.Application.TryCreateGame(gameId, this, (byte)this.MaxPlayersDefault, gameServer, out gameState))
                {
                    errorResponse = new OperationResponse(operation.OperationRequest.OperationCode)
                    {
                        ReturnCode = (short)ErrorCode.GameIdAlreadyExists,
                        DebugMessage = LBErrorMessages.GameAlreadyExist,
                    };

                    return false;
                }

                gameCreated = true;
            }
          

            if (properties != null)
            {
                bool changed;
                string debugMessage;

                if (!gameState.TrySetProperties(properties, out changed, out debugMessage))
                {
                    errorResponse = new OperationResponse(operation.OperationRequest.OperationCode)
                        {
                            ReturnCode = (short)ErrorCode.OperationInvalid,
                            DebugMessage = debugMessage
                        };
                    return false;
                }
            }

            try
            {
                this.GameList.AddGameState(gameState);
            }
            catch (Exception)
            {
                log.ErrorFormat("New game state:{0}", gameState.ToString());

                this.Application.RemoveGame(gameState.Id);
                gameCreated = false;

                GameState gameStateInList;
                if (this.GameList.TryGetGame(gameState.Id, out gameStateInList))
                {
                    log.ErrorFormat("Game state in list:{0}", gameStateInList.ToString());
                }
                else
                {
                    log.ErrorFormat("Game state {0} not found in list", gameState.Id);
                }
                throw;
            }

            this.SchedulePublishGameChanges();

            errorResponse = null;
            return true;
        }

        #endregion
    }
}