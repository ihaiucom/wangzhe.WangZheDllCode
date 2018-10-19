using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;
using ExitGames.Logging;
using NUnit.Framework;
using Photon.Hive.Common.Lobby;
using Photon.Hive.Operations;
using Photon.LoadBalancing.Operations;
using Photon.UnitTest.Utils.Basic;
using ErrorCode = ExitGames.Client.Photon.LoadBalancing.ErrorCode;
using EventCode = ExitGames.Client.Photon.LoadBalancing.EventCode;
using EventData = ExitGames.Client.Photon.EventData;
using JoinMode = Photon.Hive.Operations.JoinModes;
using OperationCode = ExitGames.Client.Photon.LoadBalancing.OperationCode;
using ParameterCode = ExitGames.Client.Photon.LoadBalancing.ParameterCode;
using OperationResponse = ExitGames.Client.Photon.OperationResponse;
using OperationRequest = ExitGames.Client.Photon.OperationRequest;

namespace Photon.LoadBalancing.UnifiedClient
{
    public class UnifiedTestClient : UnifiedClientBase, IAuthSchemeClient
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        protected IAuthenticationScheme authenticationScheme;

        public UnifiedTestClient(INUnitClient client, IAuthenticationScheme authScheme)
            : base(client)
        {
            this.authenticationScheme = authScheme;
        }

        public void CheckThereIsErrorInfoEvent(int timeout = 3000)
        {
            EventData eventData;
            Assert.IsTrue(this.TryWaitForEvent(EventCode.ErrorInfo, timeout, out eventData), "Did not get expected error event");
            log.InfoFormat("Got expected error event with message {0}", eventData[ParameterCode.Info]);
        }

        public void CheckThereIsNoErrorInfoEvent(int timeout = 3000)
        {
            EventData eventData;
            Assert.IsFalse(this.TryWaitForEvent(EventCode.ErrorInfo, timeout, out eventData),
                "Got unexpected error event: '{0}'", eventData != null ? eventData[ParameterCode.Info] : null);
        }

        public OperationResponse Authenticate(string userId, Dictionary<byte, object> authParameter)
        {
            this.UserId = userId;
            var op = CreateAuthenticateRequest(userId);

            this.authenticationScheme.SetAuthenticateParameters(this, op.Parameters, authParameter);

            var response = this.SendRequestAndWaitForResponse(op);

            this.authenticationScheme.HandleAuthenticateResponse(this, response.Parameters);

            return response;
        }

        public GetLobbyStatsResponse GetLobbyStats(string[] lobbyNames, byte[] lobbyTypes, short expectedResult = ErrorCode.Ok)
        {
            var operationRequest = CreateOperationRequest(OperationCode.GetLobbyStats);
            if (lobbyNames != null)
            {
                operationRequest.Parameters.Add(ParameterCode.LobbyName, lobbyNames);
            }

            if (lobbyTypes != null)
            {
                operationRequest.Parameters.Add(ParameterCode.LobbyType, lobbyTypes);
            }

            var response = this.SendRequestAndWaitForResponse(operationRequest, expectedResult);
            return GetLobbyStatsResponse(response);
        }

        public CreateGameResponse CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby, Hashtable playerProperties, bool onGameServer, short expectedResult = ErrorCode.Ok)
        {
            return this.CreateRoom(roomName, roomOptions, lobby, playerProperties, onGameServer, null, expectedResult);
        }

        public CreateGameResponse CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby, 
            Hashtable playerProperties, bool onGameServer, string pluginName, short expectedResult = ErrorCode.Ok)
        {
            var createGameRequest = new CreateGameRequest
            {
                GameId = roomName,
                LobbyName = lobby.Name,
                LobbyType = (byte)lobby.Type,
            };
            if (!string.IsNullOrEmpty(pluginName))
            {
                createGameRequest.Plugins = new[] {pluginName};
            }
            if (onGameServer)
            {
                createGameRequest.ActorProperties = playerProperties;
                createGameRequest.BroadcastActorProperties = true;

                RoomOptionsToOpParameters(createGameRequest, roomOptions);
            }

            return this.CreateGame(createGameRequest, expectedResult);
        }

        public CreateGameResponse CreateGame(string gameId, short expectedResult = ErrorCode.Ok)
        {
            var createGameRequest = new CreateGameRequest { GameId = gameId };
            return this.CreateGame(createGameRequest, expectedResult);
        }

        public CreateGameResponse CreateGame(string gameId, bool isVisible, bool isOpen, byte maxPlayer, short expectedResult = ErrorCode.Ok)
        {
            var createGameRequest = new CreateGameRequest
            {
                GameId = gameId,
                GameProperties = new Hashtable()
            };

            createGameRequest.GameProperties[GameParameter.IsVisible] = isVisible;
            createGameRequest.GameProperties[GameParameter.IsOpen] = isOpen;
            createGameRequest.GameProperties[GameParameter.MaxPlayers] = maxPlayer;

            return this.CreateGame(createGameRequest, expectedResult);
        }

        public CreateGameResponse CreateGame(string gameId, bool isVisible, bool isOpen, byte maxPlayer, Hashtable gameProperties, string[] lobbyProperties, Hashtable playerProperties, short expectedResult = ErrorCode.Ok)
        {
            if (gameProperties == null)
            {
                gameProperties = new Hashtable();
            }

            var createGameRequest = new CreateGameRequest
            {
                GameId = gameId,
                GameProperties = gameProperties,
                ActorProperties = playerProperties
            };

            createGameRequest.GameProperties[GameParameter.IsVisible] = isVisible;
            createGameRequest.GameProperties[GameParameter.IsOpen] = isOpen;
            createGameRequest.GameProperties[GameParameter.MaxPlayers] = maxPlayer;

            if (lobbyProperties != null)
            {
                createGameRequest.GameProperties[GameParameter.LobbyProperties] = lobbyProperties;
            }

            return this.CreateGame(createGameRequest, expectedResult);
        }

        public CreateGameResponse CreateGame(CreateGameRequest createGameRequest, short expectedResult = ErrorCode.Ok)
        {
            var request = CreateOperationRequest(OperationCode.CreateGame);

            JoinRequestToDictionary(createGameRequest, request);


            var response = this.SendRequestAndWaitForResponse(request, expectedResult);
            return GetCreateGameResponse(response);
        }

        public JoinGameResponse JoinRoom(string roomName, Hashtable playerProperties,
            int actorId, RoomOptions roomOptions, bool createIfNotExists, bool onGameServer, short errorCode = ErrorCode.Ok)
        {
            var joinRequest = new JoinGameRequest
            {
                GameId = roomName,
                ActorNr = actorId,
                JoinMode =
                    createIfNotExists
                        ? Photon.Hive.Operations.JoinModes.JoinOnly
                        : Photon.Hive.Operations.JoinModes.CreateIfNotExists
            };

            if (onGameServer)
            {
                joinRequest.ActorProperties = playerProperties;
                joinRequest.BroadcastActorProperties = true;

                RoomOptionsToOpParameters(joinRequest, roomOptions);
            }

            return this.JoinGame(joinRequest, errorCode);
        }

        public JoinGameResponse JoinGame(string gameId, short expectedResult = ErrorCode.Ok)
        {
            var joinRequest = new JoinGameRequest { GameId = gameId };
            return this.JoinGame(joinRequest, expectedResult);
        }

        public JoinGameResponse JoinGame(JoinGameRequest joinRequest, short expectedResult = ErrorCode.Ok)
        {
            var request = CreateOperationRequest(OperationCode.JoinGame);

            JoinRequestToDictionary(joinRequest, request);

            var operationResponse = this.SendRequestAndWaitForResponse(request, expectedResult);
            return GetJoinGameResponse(operationResponse);
        }

        public JoinRandomGameResponse JoinRandomGame(Hashtable gameProperties, byte maxPlayers, Hashtable actorProperties,
            MatchmakingMode mode, string lobbyName, AppLobbyType lobbyType, string sqlLobbyFilter, short errorCode = ErrorCode.Ok)
        {
            var request = new JoinRandomGameRequest
            {
                GameProperties = new Hashtable(),
                JoinRandomType = (byte)mode,
                LobbyType = (byte)lobbyType,
                LobbyName = lobbyName,
            };
            if (maxPlayers > 0)
            {
                request.GameProperties.Add(GamePropertyKey.MaxPlayers, maxPlayers);
            }
            return this.JoinRandomGame(request, errorCode);
        }

        public JoinRandomGameResponse JoinRandomGame(JoinRandomGameRequest request, short expectedResult, params string[] expectedRoomNames)
        {
            var operationRequest = CreateOperationRequest(OperationCode.JoinRandomGame);

            if (request.GameProperties != null)
            {
                operationRequest.Parameters[ParameterCode.GameProperties] = request.GameProperties;
            }

            if (request.QueryData != null)
            {
                operationRequest.Parameters[ParameterCode.Data] = request.QueryData;
            }

            if (request.JoinRandomType != 0)
            {
                operationRequest.Parameters[ParameterCode.MatchMakingType] = request.JoinRandomType;
            }

            if (request.LobbyName != null)
            {
                operationRequest.Parameters[ParameterCode.LobbyName] = request.LobbyName;
            }

            if (request.LobbyType != 0)
            {
                operationRequest.Parameters[ParameterCode.LobbyType] = request.LobbyType;
            }

            var response = this.SendRequestAndWaitForResponse(operationRequest, expectedResult);
            var joinRandomResponse = GetJoinRandomGameResponse(response);
            if (expectedResult != ErrorCode.Ok)
            {
                return joinRandomResponse;
            }

            if (expectedRoomNames == null || expectedRoomNames.Length == 0)
            {
                return joinRandomResponse;
            }

            foreach (var id in expectedRoomNames)
            {
                if (id == joinRandomResponse.GameId)
                {
                    return joinRandomResponse;
                }
            }

            Assert.Fail("Unexpected game on join random: gameId={0}", joinRandomResponse.GameId);
            return joinRandomResponse;
        }

        public OperationResponse JoinRandomGame(Hashtable gameProperties, string query, short expectedResult, string lobbyName = null, byte? lobbyType = null, params string[] expectedRoomNames)
        {
            var operationRequest = CreateOperationRequest(OperationCode.JoinRandomGame);

            if (gameProperties != null)
            {
                operationRequest.Parameters[ParameterCode.GameProperties] = gameProperties;
            }

            if (query != null)
            {
                operationRequest.Parameters[ParameterCode.Data] = query;
            }

            if (lobbyName != null)
            {
                operationRequest.Parameters[ParameterCode.LobbyName] = lobbyName;
            }

            if (lobbyType.HasValue)
            {
                operationRequest.Parameters[ParameterCode.LobbyType] = lobbyType.Value;
            }

            var response = this.SendRequestAndWaitForResponse(operationRequest, expectedResult);
            if (expectedResult != ErrorCode.Ok)
            {
                return response;
            }

            string gameId;
            if (!TryGetParameter(response, ParameterCode.RoomName, out gameId))
            {
                Assert.Fail("GameId is missing in join random response");
            }

            if (expectedRoomNames == null || expectedRoomNames.Length == 0)
            {
                return response;
            }

            foreach (var id in expectedRoomNames)
            {
                if (id == gameId)
                {
                    return response;
                }
            }

            Assert.Fail("Unexpected game on join random: gameId={0}", gameId);
            return response;
        }

        public OperationResponse LeaveGame(bool IsInActive = false)
        {
            var request = new OperationRequest
            {
                OperationCode = OperationCode.Leave,
                Parameters = new Dictionary<byte, object>
                {
                    {ParameterCode.IsComingBack, IsInActive}
                }
            };
            return this.SendRequestAndWaitForResponse(request);
        }

        public OperationResponse JoinLobby(string lobbyName = null, byte? lobbyType = null, int maxGameCount = 0, short expectedResult = 0)
        {
            var operationRequest = CreateOperationRequest(OperationCode.JoinLobby);
            if (lobbyName != null)
            {
                operationRequest.Parameters[ParameterCode.LobbyName] = lobbyName;
            }

            if (lobbyType.HasValue)
            {
                operationRequest.Parameters[ParameterCode.LobbyType] = lobbyType.Value;
            }

            if (maxGameCount != 0)
            {
                operationRequest.Parameters[ParameterCode.GameCount] = maxGameCount;
            }

            return this.SendRequestAndWaitForResponse(operationRequest, expectedResult);
        }

        public OperationResponse FindFriends(string[] userIds, out bool[] onlineStates, out string[] userStates)
        {
            var request = CreateFindfriendsRequest(userIds);
            var response = this.SendRequestAndWaitForResponse(request);

            Assert.IsTrue(TryGetParameter(response, 1, out onlineStates));
            Assert.IsTrue(TryGetParameter(response, 2, out userStates));

            Assert.AreEqual(userIds.Length, onlineStates.Length);
            Assert.AreEqual(userIds.Length, userStates.Length);

            return response;
        }

        /// <summary>
        /// Calls OpJoinLobby(string name, LobbyType lobbyType).
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// If the operation could be sent (requires connection).
        /// </returns>
        public virtual bool OpJoinLobby()
        {
            return this.OpJoinLobby(string.Empty, AppLobbyType.Default);
        }

        /// <summary>
        /// Joins the lobby on the Master Server, where you get a list of RoomInfos of currently open rooms.
        ///             This is an async request which triggers a OnOperationResponse() call.
        /// 
        /// </summary>
        /// <param name="lobbyName">The lobby join to.</param>
        /// <param name="lobbyType">type of lobby</param>
        /// <returns>
        /// If the operation could be sent (has to be connected).
        /// </returns>
        public virtual bool OpJoinLobby(string lobbyName, AppLobbyType lobbyType)
        {
            var request = CreateOperationRequest(OperationCode.JoinLobby);
            request.Parameters[ParameterCode.LobbyName] = lobbyName;
            request.Parameters[ParameterCode.LobbyType] = (byte)lobbyType;

            this.SendRequestAndWaitForResponse(request);
            return true;
        }

        public virtual bool OpLeaveLobby()
        {
            var request = CreateOperationRequest(OperationCode.LeaveLobby);

            return this.SendRequest(request);
        }

        /// <summary>
        /// Sets properties of a room.
        /// Internally this uses OpSetProperties, which can be used to either set room or player properties.
        /// </summary>
        /// <param name="gameProperties"></param>
        /// <returns>If the operation could be sent (has to be connected).</returns>
        public OperationResponse OpSetPropertiesOfRoom(Hashtable gameProperties)
        {
            return this.OpSetPropertiesOfRoom(gameProperties, false);
        }

        /// <summary>
        /// Sets properties of a room.
        /// Internally this uses OpSetProperties, which can be used to either set room or player properties.
        /// </summary>
        /// <param name="gameProperties"></param>
        /// <param name="webForward"></param>
        /// <returns>If the operation could be sent (has to be connected).</returns>
        public OperationResponse OpSetPropertiesOfRoom(Hashtable gameProperties, bool webForward)
        {
            var request = CreateOperationRequest(OperationCode.SetProperties);
            request.Parameters.Add(ParameterCode.Properties, gameProperties);
            request.Parameters.Add(ParameterCode.Broadcast, true);

            if (webForward)
            {
                request.Parameters[ParameterCode.EventForward] = true;
            }

            return this.SendRequestAndWaitForResponse(request);
        }

        public GetPropertiesResponse GetActorsProperties()
        {
            var request = CreateOperationRequest(OperationCode.GetProperties);
            request.Parameters.Add(ParameterCode.Properties, (byte)PropertyType.Actor);


            var response = this.SendRequestAndWaitForResponse(request);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Parameters);
            Assert.AreEqual(response.OperationCode, OperationCode.GetProperties);

            return CreateGetPropertiesResponse(response);
        }

        protected static void JoinRequestToDictionary(JoinGameRequest joinRequest, OperationRequest request)
        {
            if (joinRequest.GameId != null)
            {
                request.Parameters[ParameterCode.RoomName] = joinRequest.GameId;
            }

            if (joinRequest.LobbyName != null)
            {
                request.Parameters[ParameterCode.LobbyName] = joinRequest.LobbyName;
            }

            if (joinRequest.LobbyType != 0)
            {
                request.Parameters[ParameterCode.LobbyType] = joinRequest.LobbyType;
            }

            if (joinRequest.GameProperties != null)
            {
                request.Parameters[ParameterCode.GameProperties] = joinRequest.GameProperties;
            }

            if (joinRequest.ActorProperties != null)
            {
                request.Parameters[ParameterCode.PlayerProperties] = joinRequest.ActorProperties;
                request.Parameters[ParameterCode.Broadcast] = true;
            }

            if (joinRequest.BroadcastActorProperties)
            {
                request.Parameters[ParameterCode.Broadcast] = true;
            }

            if (joinRequest.DeleteCacheOnLeave)
            {
                request.Parameters[ParameterCode.CleanupCacheOnLeave] = joinRequest.DeleteCacheOnLeave;
            }

            if (joinRequest.CheckUserOnJoin)
            {
                request.Parameters[ParameterCode.CheckUserOnJoin] = joinRequest.CheckUserOnJoin;
            }

            if (joinRequest.PlayerTTL > 0 || joinRequest.PlayerTTL == -1)
            {
                request.Parameters[ParameterCode.PlayerTTL] = joinRequest.PlayerTTL;
            }

            if (joinRequest.EmptyRoomLiveTime != 0)
            {
                request.Parameters[ParameterCode.EmptyRoomTTL] = joinRequest.EmptyRoomLiveTime;
            }

            if (joinRequest.JoinMode != JoinMode.JoinOnly)
            {
                request.Parameters[ParameterCode.JoinMode] = joinRequest.JoinMode;
            }

            if (joinRequest.Plugins != null)
            {
                request.Parameters[ParameterCode.Plugins] = joinRequest.Plugins;
            }

            if (joinRequest.CacheSlice != null)
            {
                request.Parameters[ParameterCode.CacheSliceIndex] = joinRequest.CacheSlice;
            }

            if (joinRequest.ActorNr > 0)
            {
                request.Parameters[ParameterCode.ActorNr] = joinRequest.ActorNr;
            }

            if (joinRequest.AddUsers != null && joinRequest.AddUsers.Length > 0)
            {
                request.Parameters[ParameterCode.Add] = joinRequest.AddUsers;
            }

            if (joinRequest.SuppressRoomEvents)
            {
                request.Parameters[ParameterCode.SuppressRoomEvents] = true;
            }

            if (joinRequest.ForceRejoin)
            {
                request.Parameters[229] = true;
            }
        }

        protected static void RoomOptionsToOpParameters(JoinGameRequest request, RoomOptions roomOptions)
        {
            if (roomOptions == null)
            {
                roomOptions = new RoomOptions();
            }

            var gameProperties = new Hashtable();
            gameProperties[GamePropertyKey.IsOpen] = roomOptions.IsOpen;
            gameProperties[GamePropertyKey.IsVisible] = roomOptions.IsVisible;
            gameProperties[GamePropertyKey.PropsListedInLobby] = roomOptions.CustomRoomPropertiesForLobby ?? new string[0];
            gameProperties.MergeStringKeys(roomOptions.CustomRoomProperties);
            if (roomOptions.MaxPlayers > 0)
            {
                gameProperties[GamePropertyKey.MaxPlayers] = roomOptions.MaxPlayers;
            }
            request.GameProperties = gameProperties;


            request.DeleteCacheOnLeave = roomOptions.CleanupCacheOnLeave;

            if (roomOptions.CheckUserOnJoin)
            {
                request.CheckUserOnJoin = true;   //TURNBASED
            }

            if (roomOptions.PlayerTtl > 0 || roomOptions.PlayerTtl == -1)
            {
                request.PlayerTTL = roomOptions.PlayerTtl;   //TURNBASED
            }
            if (roomOptions.EmptyRoomTtl > 0)
            {
                request.EmptyRoomLiveTime = roomOptions.EmptyRoomTtl;   //TURNBASED
            }
        }

        protected static OperationRequest CreateAuthenticateRequest(string userId)
        {
            var op = CreateOperationRequest(OperationCode.Authenticate);
            op.Parameters.Add(ParameterCode.UserId, userId);
            return op;
        }

        protected static OperationRequest CreateFindfriendsRequest(string[] userIdList)
        {
            var op = CreateOperationRequest(OperationCode.FindFriends);
            op.Parameters.Add(1, userIdList);
            return op;
        }

        protected static CreateGameResponse GetCreateGameResponse(OperationResponse op)
        {
            var res = new CreateGameResponse
            {
                GameId = GetParameter<string>(op, ParameterCode.RoomName, true),
                Address = GetParameter<string>(op, ParameterCode.Address, true)
            };
            return res;
        }

        protected static JoinGameResponse GetJoinGameResponse(OperationResponse op)
        {
            var res = new JoinGameResponse
            {
                Address = GetParameter<string>(op, ParameterCode.Address, true),
                GameProperties = GetParameter<Hashtable>(op, ParameterCode.GameProperties, true),
            };
            return res;
        }

        protected static GetLobbyStatsResponse GetLobbyStatsResponse(OperationResponse op)
        {
            var res = new GetLobbyStatsResponse
            {
                LobbyNames = GetParameter<string[]>(op, ParameterCode.LobbyName, true),
                LobbyTypes = GetParameter<byte[]>(op, ParameterCode.LobbyType, true),
                PeerCount = GetParameter<int[]>(op, ParameterCode.PeerCount, true),
                GameCount = GetParameter<int[]>(op, ParameterCode.GameCount, true)
            };
            return res;
        }

        protected static JoinRandomGameResponse GetJoinRandomGameResponse(OperationResponse op)
        {
            var res = new JoinRandomGameResponse
            {
                GameId = GetParameter<string>(op, ParameterCode.RoomName, false),
                Address = GetParameter<string>(op, ParameterCode.Address, false),
            };
            return res;
        }

        protected static GetPropertiesResponse CreateGetPropertiesResponse(OperationResponse response)
        {
            var result = new GetPropertiesResponse();
            if (response.Parameters.ContainsKey(ParameterCode.PlayerProperties))
            {
                result.ActorProperties = (Hashtable)response[ParameterCode.PlayerProperties];
            }

            if (response.Parameters.ContainsKey(ParameterCode.GameProperties))
            {
                result.GameProperties = (Hashtable)response[ParameterCode.GameProperties];
            }

            return result;
        }

        protected static bool TryGetParameter<T>(OperationResponse response, byte parameterCode, out T value)
        {
            value = default(T);

            if (response.Parameters == null)
            {
                return false;
            }

            object temp;
            if (response.Parameters.TryGetValue(parameterCode, out temp) == false)
            {
                return false;
            }

            Assert.IsInstanceOf<T>(temp);
            value = (T)temp;
            return true;
        }

        protected static T GetParameter<T>(OperationResponse response, byte parameterCode, bool isOptional)
        {
            T value;
            if (!TryGetParameter(response, parameterCode, out value))
            {
                if (isOptional == false && response.ReturnCode == (short)Photon.Common.ErrorCode.Ok)
                {
                    Assert.Fail("Parameter {0} is missing in operation response {1}", parameterCode, (Photon.LoadBalancing.Operations.OperationCode)response.OperationCode);
                }

                return default(T);
            }

            return value;
        }
    }
}
