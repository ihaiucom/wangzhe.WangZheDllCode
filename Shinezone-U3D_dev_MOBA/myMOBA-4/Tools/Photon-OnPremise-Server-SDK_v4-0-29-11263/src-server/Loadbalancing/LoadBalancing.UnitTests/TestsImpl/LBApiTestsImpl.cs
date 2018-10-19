using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using NUnit.Framework;
using Photon.Hive.Common.Lobby;
using Photon.Hive.Operations;
using Photon.LoadBalancing.Operations;
using Photon.LoadBalancing.UnifiedClient;
using Photon.LoadBalancing.UnifiedClient.AuthenticationSchemes;
using Photon.LoadBalancing.UnitTests.UnifiedServer;
using Photon.UnitTest.Utils.Basic;
using ErrorCode = ExitGames.Client.Photon.LoadBalancing.ErrorCode;
using EventCode = ExitGames.Client.Photon.LoadBalancing.EventCode;
using OperationCode = ExitGames.Client.Photon.LoadBalancing.OperationCode;
using ParameterCode = ExitGames.Client.Photon.LoadBalancing.ParameterCode;

namespace Photon.LoadBalancing.UnitTests.UnifiedTests
{
    public abstract class LBApiTestsImpl : LoadBalancingUnifiedTestsBase
    {
        protected string GameNamePrefix = "ForwardPlugin2"; //string.Empty;

        protected LBApiTestsImpl(ConnectPolicy policy) : base(policy)
        {
        }

        static protected IAuthenticationScheme GetAuthScheme(string name)
        {
            if ("TokenLessAuthForOldClients" == name)
            {
                return new TokenLessAuthenticationScheme();
            }
            return new TokenAuthenticationScheme();
        }

        private int ranTestsCount;
        [SetUp]
        public void TestSetup()
        {
            ++ranTestsCount;
            this.WaitUntilEmptyGameList();
        }

        protected override void FixtureTearDown()
        {
            if (this.connectPolicy.IsInited && this.ranTestsCount > 1)
            {
                this.ApplicationStats();
            }
            base.FixtureTearDown();
        }

        [Test]
        public void ConnectTwice()
        {
            // master: 
            var client = this.CreateTestClient();
            this.ConnectToServer(client, this.MasterAddress);
            client.Disconnect();

            this.ConnectToServer(client, this.GameServerAddress);

            client.Disconnect();
            client.Dispose();

            //CheckGameListCount(0);
        }


        [Test]
        public void CreateGameTwice()
        {
            UnifiedTestClient masterClient = null;

            try
            {
                masterClient = this.CreateMasterClientAndAuthenticate(Player1);
                string roomName = this.GenerateRandomizedRoomName("CreateGameTwice_");
                masterClient.CreateGame(roomName);
                masterClient.CreateGame(roomName, ErrorCode.GameIdAlreadyExists);
            }
            finally
            {
                DisposeClients(masterClient);
            }
        }

        void LobbyTestBody(byte? type, short responseCode)
        {
            UnifiedTestClient masterClient = null;

            try
            {
                masterClient = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient.JoinLobby("tests", type, 0, responseCode);
            }
            finally
            {
                DisposeClients(masterClient);
            }
        }

        [Test]
        public void LobbyTypeTest()
        {
            this.LobbyTestBody(null, 0);
            this.LobbyTestBody(1, 0);
            this.LobbyTestBody(2, 0);
            this.LobbyTestBody(3, 0);
            this.LobbyTestBody(4, ErrorCode.InvalidOperation);
        }

        [Test]
        public void InvisibleGame()
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;

            try
            {
                string roomName = this.GenerateRandomizedRoomName("InvisibleGame_");

                // create room 
                client1 = this.CreateGameOnGameServer(Player1, roomName, null, 0, false, true, 0, null, null);

                // connect 2nd client to master
                client2 = this.CreateMasterClientAndAuthenticate(Player2);

                // try join random game should fail because the game is not visible
                var joinRandomGameRequest = new JoinRandomGameRequest { JoinRandomType = (byte)MatchmakingMode.FillRoom };
                client2.JoinRandomGame(joinRandomGameRequest, ErrorCode.NoRandomMatchFound);

                // join 2nd client on master - ok - and disconnect from master: 
                var joinGameRequest = new JoinGameRequest { GameId = roomName };
                var joinResponse = client2.JoinGame(joinGameRequest);
                client2.Disconnect();

                // join directly on GS - game full:
                this.ConnectAndAuthenticate(client2, joinResponse.Address, client2.UserId);
                client2.JoinGame(joinGameRequest);
            }
            finally
            {
                DisposeClients(client1, client2);
            }
        }

        [Test]
        public void ClosedGame()
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;

            try
            {
                // create the game
                string roomName = this.GenerateRandomizedRoomName("ClosedGame_");
                client1 = this.CreateGameOnGameServer(Player1, roomName, null, 0, true, false, 0, null, null);


                // join 2nd client on master - closed: 
                client2 = this.CreateMasterClientAndAuthenticate(Player2);
                var joinGameRequest = new JoinGameRequest { GameId = roomName };
                client2.JoinGame(joinGameRequest, ErrorCode.GameClosed);
                client2.Disconnect();

                // join directly on GS - game closed: 
                this.ConnectAndAuthenticate(client2, client1.RemoteEndPoint, client2.UserId);
                client2.JoinGame(roomName, ErrorCode.GameClosed);
            }
            finally
            {
                DisposeClients(client1, client2);
            }
        }

        [Test]
        public void UserIdIsNotNull()
        {
            // master: 
            var client = (UnifiedTestClient)this.CreateTestClient();
            try
            {
                this.ConnectToServer(client, this.MasterAddress);
                var response = client.Authenticate(null, new Dictionary<byte, object>());
                Assert.IsNotNull(response.Parameters[ParameterCode.UserId]);
                client.Disconnect();
                client.Dispose();

                client = (UnifiedTestClient) this.CreateTestClient();
                this.ConnectToServer(client, this.MasterAddress);

                response = client.Authenticate("", new Dictionary<byte, object>());
                Assert.IsNotNull(response.Parameters[ParameterCode.UserId]);
                client.Disconnect();

            }
            finally
            {
                DisposeClients(client);
            }
        }

        [Test]
        public void AuthResponseTest()
        {
            // master: 
            var client = (UnifiedTestClient)this.CreateTestClient();
            try
            {
                this.ConnectToServer(client, this.MasterAddress);
                var response = client.Authenticate(null, new Dictionary<byte, object>());
                Assert.IsNotNull(response.Parameters[ParameterCode.UserId]);
                client.Disconnect();

                this.ConnectToServer(client, this.MasterAddress);
                var parameters = new Dictionary<byte, object>()
                {
                    {ParameterCode.Secret, response.Parameters[ParameterCode.Secret]},
                };

                var response2 = client.Authenticate("", parameters);

                Assert.AreEqual(2, response2.Parameters.Count);
                Assert.AreNotEqual(response.Parameters[ParameterCode.Secret], response2.Parameters[ParameterCode.Secret]);
                client.Disconnect();

            }
            finally
            {
                DisposeClients(client);
            }
        }

        [Test]
        public void MaxPlayers()
        {
            UnifiedTestClient masterClient = null;
            UnifiedTestClient gameClient1 = null;

            try
            {
                string roomName = this.GenerateRandomizedRoomName("MaxPlayers_");
                gameClient1 = this.CreateGameOnGameServer("GameClient", roomName, null, 0, true, true, 1, null, null);

                // join 2nd client on master - full: 
                masterClient = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient.JoinGame(roomName, ErrorCode.GameFull);

                // join random 2nd client on master - full: 
                var joinRequest = new JoinRandomGameRequest();
                masterClient.JoinRandomGame(joinRequest, ErrorCode.NoRandomMatchFound);
                joinRequest.JoinRandomType = (byte)MatchmakingMode.SerialMatching;
                masterClient.JoinRandomGame(joinRequest, ErrorCode.NoRandomMatchFound);
                joinRequest.JoinRandomType = (byte)MatchmakingMode.RandomMatching;
                masterClient.JoinRandomGame(joinRequest, ErrorCode.NoRandomMatchFound);
                masterClient.Disconnect();

                // join directly on GS: 
                this.ConnectAndAuthenticate(masterClient, gameClient1.RemoteEndPoint, masterClient.UserId);
                masterClient.JoinGame(roomName, ErrorCode.GameFull);
            }
            finally
            {
                DisposeClients(masterClient, gameClient1);
            }
        }

        [Test]
        public void LobbyGameListEvents()
        {
            // previous tests could just have leaved games on the game server
            // so there might be AppStats or GameListUpdate event in schedule.
            // Just wait a second so this events can be published before starting the test
            Thread.Sleep(1100);

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;

            try
            {
                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);

                Assert.IsTrue(masterClient1.OpJoinLobby());
                var ev = masterClient1.WaitForEvent(EventCode.GameList, 1000 + ConnectPolicy.WaitTime);
                Assert.AreEqual(EventCode.GameList, ev.Code);
                var gameList = (Hashtable)ev.Parameters[ParameterCode.GameList];
                this.CheckGameListCount(0, gameList);

                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);

                Assert.IsTrue(masterClient2.OpJoinLobby());
                ev = masterClient2.WaitForEvent(EventCode.GameList);
                Assert.AreEqual(EventCode.GameList, ev.Code);
                gameList = (Hashtable)ev.Parameters[ParameterCode.GameList];
                this.CheckGameListCount(0, gameList);

                // join lobby again: 
                masterClient1.OperationResponseQueueClear();
                Assert.IsTrue(masterClient1.OpJoinLobby());

                // wait for old app stats event
                masterClient2.CheckThereIsEvent(EventCode.AppStats, 10000);

                masterClient1.EventQueueClear();
                masterClient2.EventQueueClear();

                // open game
                string roomName = "LobbyGamelistEvents_1_" + Guid.NewGuid().ToString().Substring(0, 6);
                this.CreateRoomOnGameServer(masterClient1, roomName);

                // in order to get updates from gs on master server
                Thread.Sleep(1000);

                var timeout = Environment.TickCount + 10000;

                bool gameListUpdateReceived = false;
                bool appStatsReceived = false;

                while (Environment.TickCount < timeout && (!gameListUpdateReceived || !appStatsReceived))
                {
                    try
                    {
                        ev = masterClient2.WaitForEvent(1000);

                        if (ev.Code == EventCode.AppStats)
                        {
                            appStatsReceived = true;
                            Assert.AreEqual(1, ev[ParameterCode.GameCount]);
                        }
                        else if (ev.Code == EventCode.GameListUpdate)
                        {
                            gameListUpdateReceived = true;
                            var roomList = (Hashtable)ev.Parameters[ParameterCode.GameList];
                            this.CheckGameListCount(1, roomList);

                            Assert.IsTrue(roomList.ContainsKey(roomName), "Room not found in game list");

                            var room = (Hashtable)roomList[roomName];
                            Assert.IsNotNull(room);
                            Assert.AreEqual(3, room.Count);

                            Assert.IsNotNull(room[GamePropertyKey.IsOpen], "IsOpen");
                            Assert.IsNotNull(room[GamePropertyKey.MaxPlayers], "MaxPlayers");
                            Assert.IsNotNull(room[GamePropertyKey.PlayerCount], "PlayerCount");

                            Assert.AreEqual(true, room[GamePropertyKey.IsOpen]);
                            Assert.AreEqual(0, room[GamePropertyKey.MaxPlayers]);
                            Assert.AreEqual(1, room[GamePropertyKey.PlayerCount]);
                        }
                    }
                    catch (TimeoutException)
                    {
                    }
                }

                Assert.IsTrue(gameListUpdateReceived, "GameListUpdate event received");
                Assert.IsTrue(appStatsReceived, "AppStats event received");


                masterClient1.SendRequestAndWaitForResponse(new OperationRequest { OperationCode = (byte)Hive.Operations.OperationCode.Leave });
                masterClient1.Disconnect();

                gameListUpdateReceived = false;
                appStatsReceived = false;

                timeout = Environment.TickCount + 10000;
                while (Environment.TickCount < timeout && (!gameListUpdateReceived || !appStatsReceived))
                {
                    try
                    {
                        ev = masterClient2.WaitForEvent(1000);

                        if (ev.Code == EventCode.AppStats)
                        {
                            appStatsReceived = true;
                            Assert.AreEqual(0, ev[ParameterCode.GameCount]);
                        }

                        if (ev.Code == EventCode.GameListUpdate)
                        {
                            gameListUpdateReceived = true;

                            var roomList = (Hashtable)ev.Parameters[ParameterCode.GameList];

                            // count may be greater than one because games from previous tests are 
                            // being removed
                            //Assert.AreEqual(1, roomList.Count); 
                            Assert.IsTrue(roomList.ContainsKey(roomName));
                            var room = (Hashtable)roomList[roomName];
                            Assert.IsNotNull(room);

                            Assert.AreEqual(1, room.Count);
                            Assert.IsNotNull(room[GamePropertyKey.Removed], "Removed");
                            Assert.AreEqual(true, room[GamePropertyKey.Removed]);
                        }
                    }
                    catch (TimeoutException)
                    {
                    }
                }

                Assert.IsTrue(gameListUpdateReceived, "GameListUpdate event received");
                Assert.IsTrue(appStatsReceived, "AppStats event received");

                // leave lobby
                masterClient2.OpLeaveLobby();

                gameListUpdateReceived = false;
                appStatsReceived = false;

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);

                roomName = this.GenerateRandomizedRoomName("LobbyGamelistEvents_2_");

                this.CreateRoomOnGameServer(masterClient1, roomName);

                timeout = Environment.TickCount + 10000;

                while (Environment.TickCount < timeout && (!gameListUpdateReceived || !appStatsReceived))
                {
                    try
                    {
                        ev = masterClient2.WaitForEvent(1000);

                        if (ev.Code == EventCode.AppStats)
                        {
                            appStatsReceived = true;
                            Assert.AreEqual(1, ev[ParameterCode.GameCount]);
                        }

                        if (ev.Code == EventCode.GameListUpdate)
                        {
                            gameListUpdateReceived = true;
                        }
                    }
                    catch (TimeoutException)
                    {
                    }

                }
                Assert.IsFalse(gameListUpdateReceived, "GameListUpdate event received");
                Assert.IsTrue(appStatsReceived, "AppStats event received");
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2);
            }
        }

        [Test]
        public void JoinNotExistingGame()
        {
            UnifiedTestClient client = null;

            try
            {
                string roomName = this.GenerateRandomizedRoomName("JoinNoMatchFound_");

                // try join game on master
                client = this.CreateMasterClientAndAuthenticate(Player1);
                client.JoinGame(roomName, ErrorCode.GameDoesNotExist);
                client.Disconnect();

                // try join game on gameServer
                this.ConnectAndAuthenticate(client, this.GameServerAddress, client.UserId);
                client.JoinGame(roomName, ErrorCode.GameDoesNotExist);
                client.Disconnect();
            }
            finally
            {
                DisposeClients(client);
            }
        }

        [Test]
        public void JoinWithEmptyPluginListTest()
        {
            UnifiedTestClient masterClient1 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    Plugins = new string[0],
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);

                // client 1: try to join a game on master which does not exists (create if not exists) 
                var joinResponse1 = masterClient1.JoinGame(joinRequest);
                masterClient1.Disconnect();

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

            }
            finally
            {
                DisposeClients(masterClient1);
            }
        }

        [Test]
        public void CreateWithEmptyPluginListTest()
        {
            UnifiedTestClient masterClient1 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);
                var joinRequest = new CreateGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    Plugins = new string[0],
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);

                // client 1: try to join a game on master which does not exists (create if not exists) 
                var joinResponse1 = masterClient1.CreateGame(joinRequest);
                masterClient1.Disconnect();

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.CreateGame(joinRequest);

            }
            finally
            {
                DisposeClients(masterClient1);
            }
        }

        [Test]
        public void JoinCreateIfNotExists()
        {
            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;

            try
            {
                string roomName = this.GenerateRandomizedRoomName("JoinCreateIfNotExists_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
//                masterClient2 = this.CreateMasterClientAndAuthenticate(Player3);

                // client 1: try to join a game on master which does not exists (create if not exists) 
                var joinResponse1 = masterClient1.JoinGame(joinRequest);
                masterClient1.Disconnect();

                // client 2: try to randomjoin a game which exists but is not created on the game server
                masterClient2.JoinRandomGame(new JoinRandomGameRequest(), ErrorCode.NoRandomMatchFound);

                // client 2: try to join (name) a game which exists but is not created on the game server
                var joinResponse2 = masterClient2.JoinGame(joinRequest);
                Assert.AreEqual(joinResponse1.Address, joinResponse2.Address);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, this.GameServerAddress, masterClient1.UserId);
                masterClient1.JoinGame(joinRequest);

                joinRequest.JoinMode = Photon.Hive.Operations.JoinModes.RejoinOrJoin;
                // client 2: try to join a game which exists and is created on the game server
                joinResponse2 = masterClient2.JoinGame(joinRequest);
                Assert.AreEqual(joinResponse1.Address, joinResponse2.Address);
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2);
            }
        }

        [Test]
        public void JoinCreateIfNotExistsLobbyProps()
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;

            try
            {
                // connect to master server
                client1 = this.CreateMasterClientAndAuthenticate(Player1);
                client1.JoinLobby();

                client2 = this.CreateMasterClientAndAuthenticate(Player2);
                client2.JoinLobby();

                string roomName = this.GenerateRandomizedRoomName("JoinCreateIfNotExistsLobby_");

                var gameProps1 = new Hashtable { { (byte)250, new object[] { "A" } } };

                var gameProps2 = new Hashtable { { "A", 1 }, { "B", 2 } };


                // join game with CreateIfNotExists parameter set
                var joinRequest = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>()
                };
                joinRequest.Parameters[ParameterCode.RoomName] = roomName;
                joinRequest.Parameters[ParameterCode.JoinMode] = Hive.Operations.JoinModes.CreateIfNotExists;

                var response = client1.SendRequestAndWaitForResponse(joinRequest);

                client2.EventQueueClear();

                // try to join not existing game on the game server
                this.ConnectAndAuthenticate(client1, (string)response.Parameters[ParameterCode.Address], client1.UserId);
                client1.SendRequestAndWaitForResponse(joinRequest);

                // wait for the game list update 
                var ev = client2.WaitForEvent(15000);
                if (ev.Code == 226)
                {
                    // app stats received first
                    ev = client2.WaitForEvent(15000);
                }

                Console.WriteLine("EventCode: {0}", ev.Code);

                // set properties for lobby and properties in two requests send in one package
                var op = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>()
                };

                op.Parameters[ParameterCode.Properties] = gameProps1;
                client1.SendRequest(op);

                op.Parameters[ParameterCode.Properties] = gameProps2;
                client1.SendRequest(op);


                // wait for the game list update event
                ev = client2.WaitForEvent(EventCode.GameListUpdate);
                Assert.IsTrue(ev.Parameters.ContainsKey(ParameterCode.GameList));
                var gameList = ev.Parameters[ParameterCode.GameList] as Hashtable;
                Assert.IsNotNull(gameList);
                Assert.IsTrue(gameList.ContainsKey(roomName));
                var gameProperties = gameList[roomName] as Hashtable;
                Assert.IsNotNull(gameProperties);
                Assert.IsTrue(gameProperties.ContainsKey("A"));
                Assert.IsFalse(gameProperties.ContainsKey("B"));
            }
            finally
            {
                DisposeClients(client1, client2);
            }
        }

        [Test]
        public void JoinOnGameServer()
        {
            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;

            try
            {
                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);

                // create game
                string roomName = this.GenerateRandomizedRoomName("JoinOnGameServer_");
                var createResponse = masterClient1.CreateGame(roomName);

                // join on master while the first client is not yet on GS:
                masterClient2.JoinGame(roomName, ErrorCode.GameDoesNotExist);

                // move 1st client to GS: 
                masterClient1.Disconnect();

                var player1Properties = new Hashtable { { "Name", Player1 } };
                var createRequest = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>()
                };
                createRequest.Parameters[ParameterCode.RoomName] = roomName;
                createRequest.Parameters[ParameterCode.Broadcast] = true;
                createRequest.Parameters[ParameterCode.PlayerProperties] = player1Properties;

                this.ConnectAndAuthenticate(masterClient1, createResponse.Address, masterClient1.UserId);
                masterClient1.SendRequestAndWaitForResponse(createRequest);

                // get own join event: 
                var ev = masterClient1.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(1, ev.Parameters[ParameterCode.ActorNr]);
                var ActorProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(Player1, ActorProperties["Name"]);

                // in order to send game state from gs to ms
                Thread.Sleep(100);

                // join 2nd client on master - ok: 
                var joinResponse = masterClient2.JoinGame(roomName);

                // disconnect and move 2nd client to GS: 
                masterClient2.Disconnect();

                var player2Properties = new Hashtable { { "Name", Player2 } };
                var joinRequest = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>()
                };
                joinRequest.Parameters[ParameterCode.RoomName] = roomName;
                joinRequest.Parameters[ParameterCode.Broadcast] = true;
                joinRequest.Parameters[ParameterCode.PlayerProperties] = player2Properties;

                this.ConnectAndAuthenticate(masterClient2, joinResponse.Address, masterClient2.UserId);
                masterClient2.SendRequestAndWaitForResponse(joinRequest);

                ev = masterClient1.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(2, ev.Parameters[ParameterCode.ActorNr]);
                ActorProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(Player2, ActorProperties["Name"]);

                ev = masterClient2.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(2, ev.Parameters[ParameterCode.ActorNr]);
                ActorProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(Player2, ActorProperties["Name"]);

                // TODO: continue implementation
                // raise event, leave etc.        
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2);
            }
        }

        [Test]
        public void JoinOnGameServerWithoutAuth()
        {
            UnifiedTestClient masterClient1 = null;

            try
            {
                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);

                // create game
                var roomName = this.GenerateRandomizedRoomName("JoinOnGameServer_");
                var createResponse = masterClient1.CreateGame(roomName);

                // move 1st client to GS: 
                masterClient1.Disconnect();

                var player1Properties = new Hashtable { { "Name", Player1 } };
                var createRequest = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>()
                };
                createRequest.Parameters[ParameterCode.RoomName] = roomName;
                createRequest.Parameters[ParameterCode.Broadcast] = true;
                createRequest.Parameters[ParameterCode.PlayerProperties] = player1Properties;


                this.ConnectToServer(masterClient1, createResponse.Address);

                masterClient1.SendRequestAndWaitForResponse(createRequest, ErrorCode.OperationNotAllowedInCurrentState);

            }
            finally
            {
                DisposeClients(masterClient1);
            }
        }

        [Test]
        public void JoinDisconnectRejoin()
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;

            try
            {
                client1 = this.CreateMasterClientAndAuthenticate(Player1);
                client2 = this.CreateMasterClientAndAuthenticate(Player2);

                // create game
                string roomName = this.GenerateRandomizedRoomName("JoinTwice");
                var createResponse = client1.CreateGame(roomName);

                // join on master while the first client is not yet on GS:
                client2.JoinGame(roomName, ErrorCode.GameDoesNotExist);

                var player1Properties = new Hashtable { { "Name", Player1 } };
                var createRequest = new OperationRequest
                {
                    OperationCode = OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object>()
                };
                createRequest.Parameters[ParameterCode.RoomName] = roomName;
                createRequest.Parameters[ParameterCode.Broadcast] = true;
                createRequest.Parameters[ParameterCode.PlayerProperties] = player1Properties;

                // move first client to GS: 
                this.ConnectAndAuthenticate(client1, createResponse.Address, Player1);
                client1.SendRequestAndWaitForResponse(createRequest);

                // get own join event: 
                var ev = client1.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(1, ev.Parameters[ParameterCode.ActorNr]);
                var ActorProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(Player1, ActorProperties["Name"]);

                Thread.Sleep(100);
                // join 2nd client on master - ok: 
                var joinResponse = client2.JoinGame(roomName);

                var player2Properties = new Hashtable { { "Name", Player2 } };

                var joinRequest = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>()
                };
                joinRequest.Parameters[ParameterCode.RoomName] = roomName;
                joinRequest.Parameters[ParameterCode.Broadcast] = true;
                joinRequest.Parameters[ParameterCode.PlayerProperties] = player2Properties;


                // move second client to GS: 
                this.ConnectAndAuthenticate(client2, joinResponse.Address, Player2);
                client2.SendRequestAndWaitForResponse(joinRequest);

                ev = client1.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(2, ev.Parameters[ParameterCode.ActorNr]);
                ActorProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(Player2, ActorProperties["Name"]);

                ev = client2.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(2, ev.Parameters[ParameterCode.ActorNr]);
                ActorProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(Player2, ActorProperties["Name"]);

                client2.LeaveGame();

                // get leave event on client1: 
                ev = client1.WaitForEvent();
                Assert.AreEqual(EventCode.Leave, ev.Code);
                Assert.AreEqual(2, ev.Parameters[ParameterCode.ActorNr]);

                // join again on GS - ok - disconnect and move to GS: 
                this.ConnectAndAuthenticate(client2, this.MasterAddress, Player2);
                joinResponse = client2.JoinGame(roomName);

                this.ConnectAndAuthenticate(client2, joinResponse.Address, Player2);
                client2.SendRequestAndWaitForResponse(joinRequest);

                // get join event on client1: 
                ev = client2.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(3, ev.Parameters[ParameterCode.ActorNr]);
                ActorProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(Player2, ActorProperties["Name"]);

                // TODO: continue implementation
                // raise event, leave etc.        
            }
            finally
            {
                DisposeClients(client1, client2);
            }
        }

        [Test]
        public void JoinRandomNoMatchFound()
        {
            UnifiedTestClient masterClient = null;

            try
            {
                masterClient = this.CreateMasterClientAndAuthenticate(Player1);

                masterClient.JoinRandomGame(new Hashtable(), 0, new Hashtable(),
                    MatchmakingMode.FillRoom, string.Empty, AppLobbyType.Default, null, ErrorCode.NoRandomMatchFound);
            }
            finally
            {
                DisposeClients(masterClient);
            }
        }

        [Test]
        public void JoinRandomOnGameServer()
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;

            try
            {
                client1 = this.CreateMasterClientAndAuthenticate(Player1);
                client2 = this.CreateMasterClientAndAuthenticate(Player2);

                // create
                string roomName = "JoinRandomOnGameServer_" + Guid.NewGuid().ToString().Substring(0, 6);

                var operationResponse = client1.CreateGame(roomName, true, true, 0);

                var gameServerAddress1 = operationResponse.Address;
                Console.WriteLine("Match on GS: " + gameServerAddress1);

                // join on master while the first client is not yet on GS:
                client2.JoinRandomGame(new Hashtable(), 0, new Hashtable(), MatchmakingMode.FillRoom,
                    string.Empty, AppLobbyType.Default, null, ErrorCode.NoRandomMatchFound);

                // move 1st client to GS: 
                this.ConnectAndAuthenticate(client1, gameServerAddress1, client1.UserId);

                var player1Properties = new Hashtable { { "Name", Player1 } };

                client1.CreateGame(roomName, true, true, 0, null, null, player1Properties);

                // get own join event: 
                var ev = client1.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(1, ev.Parameters[ParameterCode.ActorNr]);
                var ActorProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(Player1, ActorProperties["Name"]);

                Thread.Sleep(100);
                // join 2nd client on master - ok: 
                var opResponse = client2.JoinRandomGame(new Hashtable(), 0, new Hashtable(), MatchmakingMode.FillRoom, string.Empty, AppLobbyType.Default, null);

                var gameServerAddress2 = opResponse.Address;
                Assert.AreEqual(gameServerAddress1, gameServerAddress2);

                var roomName2 = operationResponse.GameId;
                Assert.AreEqual(roomName, roomName2);

                // disconnect and move 2nd client to GS: 
                this.ConnectAndAuthenticate(client2, gameServerAddress2, client2.UserId);

                // clean up - just in case: 
                client1.OperationResponseQueueClear();
                client2.OperationResponseQueueClear();

                client1.EventQueueClear();
                client2.EventQueueClear();

                // join 2nd client on GS: 
                var player2Properties = new Hashtable { { "Name", Player2 } };

                var request = new JoinGameRequest
                {
                    ActorProperties = player2Properties,
                    GameId = roomName,
                    ActorNr = 0,
                    JoinMode = 0,
                };

                client2.JoinGame(request);

                ev = client1.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(2, ev.Parameters[ParameterCode.ActorNr]);
                ActorProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(Player2, ActorProperties["Name"]);

                ev = client2.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(2, ev.Parameters[ParameterCode.ActorNr]);
                ActorProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(Player2, ActorProperties["Name"]);

                // disocnnect 2nd client
                client2.Disconnect();

                ev = client1.WaitForEvent();
                Assert.AreEqual(EventCode.Leave, ev.Code);
                Assert.AreEqual(2, ev.Parameters[ParameterCode.ActorNr]);

                // TODO: continue implementation
                // raise event, leave etc.        
            }
            finally
            {
                DisposeClients(client1, client2);
            }
        }

        [Test]
        public void JoinJoinLeaveLeaveFastRejoinTest()
        {
            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;

//            this.WaitTimeout = 300000;

            try
            {
                string roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    EmptyRoomLiveTime = 2500,
                    PlayerTTL = int.MaxValue,
                    Plugins = new []{"Webhooks"}
                };

                this.WaitTimeout = 20000;
                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                this.ConnectAndAuthenticate(masterClient1, this.GameServerAddress, masterClient1.UserId);
                masterClient1.JoinGame(joinRequest);

                Thread.Sleep(100);

                joinRequest.JoinMode = Photon.Hive.Operations.JoinModes.JoinOnly;
                // client 2: try to join a game which exists and is created on the game server

                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                var joinResponse2 = masterClient2.JoinGame(joinRequest);
                Assert.AreEqual(joinResponse1.Address, joinResponse2.Address);

                this.ConnectAndAuthenticate(masterClient2, joinResponse2.Address);

                masterClient2.JoinGame(joinRequest);

                Thread.Sleep(100);

                masterClient2.Disconnect();

                Thread.Sleep(100);
                masterClient1.Disconnect();

                Thread.Sleep(500);

                this.ConnectAndAuthenticate(masterClient2, this.MasterAddress);
                var joinRequest2 = new JoinGameRequest
                {
                    GameId = roomName,
                    ActorNr = 2,
                    JoinMode = (byte)JoinMode.JoinOrRejoin,
                };
                var response = masterClient2.JoinGame(joinRequest2);

                this.ConnectAndAuthenticate(masterClient2, response.Address);
                masterClient2.JoinGame(joinRequest2);

                Thread.Sleep(500);

                masterClient2.Disconnect();

                Thread.Sleep(5000);
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2);
            }
        }

        static private bool RepetitiveCheck(Func<bool> checkFunc, int times)
        {
            var i = 0;
            while (i++ <= times && !checkFunc())
            {
                if (i == times)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool CheckAppStatEvent(UnifiedTestClient client, int masterPeerCount, int peerCount, int gameCount)
        {
            var appStatEvent = client.WaitForEvent(EventCode.AppStats, 10000);
            client.EventQueueClear();

            if (peerCount != (int)appStatEvent.Parameters[ParameterCode.PeerCount])
            {
                return false;
            }

            Assert.AreEqual(EventCode.AppStats, appStatEvent.Code, "Event Code");
            Assert.AreEqual(masterPeerCount, appStatEvent.Parameters[ParameterCode.MasterPeerCount], "Peer Count on Master");
            Assert.AreEqual(peerCount, appStatEvent.Parameters[ParameterCode.PeerCount], "Peer Count on GS");
            Assert.AreEqual(gameCount, appStatEvent.Parameters[ParameterCode.GameCount], "Game Count");
            return true;
        }

        [Test]
        public void ApplicationStats()
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;
            UnifiedTestClient client3 = null;

            const int repeatChecksCount = 5;
            try
            {
                string roomName = "ApplicationStats_" + Guid.NewGuid().ToString().Substring(0, 6);
                // in order to clean up all previous peers on server
                Thread.Sleep(3500);

                System.Console.WriteLine("-----------creating of client 1------------------------");
                // create clients
                client1 = this.CreateMasterClientAndAuthenticate(Player1);
                System.Console.WriteLine("-----------creating of client 2------------------------");
                client2 = this.CreateMasterClientAndAuthenticate(Player2);
                System.Console.WriteLine("-----------creating of client 3------------------------");
                client3 = this.CreateMasterClientAndAuthenticate(Player3);
                System.Console.WriteLine("-----------creation finished. getting stats------------------------");

                Thread.Sleep(500);
                // app stats
                Func<bool> cond = () => CheckAppStatEvent(client3, 3, 0, 0);
                Assert.IsTrue(RepetitiveCheck(cond, repeatChecksCount));

                System.Console.WriteLine("------------------client 1 creates game-----------------");

                // create a game on the game server
                this.CreateRoomOnGameServer(client1, true, true, 10, roomName);

                // app stats: 
                cond = () => CheckAppStatEvent(client3, 2, 1, 1);
                Assert.IsTrue(RepetitiveCheck(cond, repeatChecksCount));

                System.Console.WriteLine("-------------client 2 joins random game ----------------------");
                // join random game
                var joinRequest = new JoinRandomGameRequest { GameProperties = new Hashtable(), JoinRandomType = (byte)MatchmakingMode.FillRoom };
                var joinResponse = client2.JoinRandomGame(joinRequest, ErrorCode.Ok);

                //                Assert.AreEqual(client1.RemoteEndPoint, joinResponse.Address);
                Assert.AreEqual(joinResponse.GameId, roomName);

                System.Console.WriteLine("-------------client 2 connects to game server ----------------------");
                this.ConnectAndAuthenticate(client2, joinResponse.Address, client2.UserId);
                client2.JoinGame(roomName);

                // app stats: 
                cond = () => CheckAppStatEvent(client3, 1, 2, 1);
                Assert.IsTrue(RepetitiveCheck(cond, repeatChecksCount));


                System.Console.WriteLine("-------------client 1 and 2 leaving ----------------------");
                client2.LeaveGame();
                client1.LeaveGame();


                System.Console.WriteLine("-------------client 3 waits for updated stats ----------------------");

                // app stats: 
                cond = () => CheckAppStatEvent(client3, 1, 2, 0);
                Assert.IsTrue(RepetitiveCheck(cond, repeatChecksCount));

            }
            finally
            {
                DisposeClients(client1, client2, client3);
            }
        }

        static void Assert_IsOneOf(int[] expectedValues, int actual, string message)
        {
            if (expectedValues.Any(expected => expected == actual))
            {
                return;
            }

            Assert.Fail("{2} Expected one of '{0}', but got {1}", string.Join(",", expectedValues), actual, message);
        }

        [Test]
        public void NegativePeersCountBugTest() //https://app.asana.com/0/199189943394/36771544536765
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;

            var gameName = MethodBase.GetCurrentMethod().Name;
            var gameName2 = MethodBase.GetCurrentMethod().Name + "_second_game";
            var authParams = new Dictionary<byte, object>
            {
                {(byte)ParameterKey.LobbyStats, true}
            };
            
            try
            {
                //1.
                client1 = this.CreateMasterClientAndAuthenticate(Player1, authParams);

                Thread.Sleep(500);
                //3.
                var lobbyStatsResponse = client1.GetLobbyStats(null, null);
                Assert_IsOneOf(new int[] { 0 }, lobbyStatsResponse.PeerCount[0], "Wrong peers count.");
                Assert_IsOneOf(new int[] { 0 }, lobbyStatsResponse.GameCount[0], "Wrong games count.");

                //4.
                var createGame = new CreateGameRequest
                {
                    GameId = gameName,
                    EmptyRoomLiveTime = 4000,
                    PlayerTTL = 10000000,
                };
                var response = client1.CreateGame(createGame);

                this.ConnectAndAuthenticate(client1, response.Address);

                client1.CreateGame(createGame);

                Thread.Sleep(1000);
                //5.
                client1.Disconnect();

                Thread.Sleep(1000);
                //6.
                client2 = this.CreateMasterClientAndAuthenticate(Player2, authParams);

                //7.
                lobbyStatsResponse = client2.GetLobbyStats(null, null);
                Assert_IsOneOf(new int[] { 1 }, lobbyStatsResponse.PeerCount[0], "Wrong peers count.");
                Assert_IsOneOf(new int[] { 1 }, lobbyStatsResponse.GameCount[0], "Wrong games count.");


                //8.
                createGame = new CreateGameRequest
                {
                    GameId = gameName2,
                    JoinMode = (byte)JoinMode.CreateIfNotExists,
                    PlayerTTL = 10000000,
                };
                var joinResponse = client2.CreateGame(createGame);

                this.ConnectAndAuthenticate(client2, joinResponse.Address);

                client2.CreateGame(createGame);

                Thread.Sleep(300);
                //9.
                client2.LeaveGame(true);

                this.ConnectAndAuthenticate(client2, this.MasterAddress, authParams);

                //10.
                lobbyStatsResponse = client2.GetLobbyStats(null, null);
                Assert_IsOneOf(new int[] { 1 }, lobbyStatsResponse.PeerCount[0], "Wrong peers count.");
                Assert_IsOneOf(new int[] { 1 }, lobbyStatsResponse.GameCount[0], "Wrong games count.");

                //11.
                client2.Disconnect();

                //12.
                Thread.Sleep(1500);

                //13.
                client1 = this.CreateMasterClientAndAuthenticate(Player1, authParams);

                Thread.Sleep(1500);
                //14.
                lobbyStatsResponse = client1.GetLobbyStats(null, null);
                Assert_IsOneOf(new int[] { 0 }, lobbyStatsResponse.PeerCount[0], "Wrong peers count.");
                Assert_IsOneOf(new int[] { 0 }, lobbyStatsResponse.GameCount[0], "Wrong games count.");

            }
            finally
            {
                DisposeClients(client1, client2);
            }
        }

        [Test]
        public void BroadcastProperties()
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;


            try
            {
                client1 = this.CreateMasterClientAndAuthenticate(Player1);
                client2 = this.CreateMasterClientAndAuthenticate(Player2);

                client1.EventQueueClear();
                client2.EventQueueClear();

                client1.OperationResponseQueueClear();
                client2.OperationResponseQueueClear();

                // open game
                string roomName = "BroadcastProperties_" + Guid.NewGuid().ToString().Substring(0, 6);

                var player1Properties = new Hashtable { { "Name", Player1 } };

                var gameProperties = new Hashtable();
                gameProperties["P1"] = 1;
                gameProperties["P2"] = 2;

                var lobbyProperties = new[] { "L1", "L2", "L3" };

                var createResponse = client1.CreateRoom(
                    roomName,
                    new RoomOptions
                    {
                        CustomRoomProperties = gameProperties,
                        CustomRoomPropertiesForLobby = lobbyProperties
                    }, TypedLobby.Default, player1Properties, false);

                var gameServerAddress1 = createResponse.Address;
                Console.WriteLine("Created room " + roomName + " on GS: " + gameServerAddress1);

                // move 1st client to GS: 
                this.ConnectAndAuthenticate(client1, gameServerAddress1, client1.UserId);

                client1.CreateRoom(
                    roomName,
                    new RoomOptions
                    {
                        CustomRoomProperties = gameProperties,
                        CustomRoomPropertiesForLobby = lobbyProperties
                    }, TypedLobby.Default, player1Properties, true);

                // move 2nd client to GS: 
                this.ConnectAndAuthenticate(client2, gameServerAddress1, client2.UserId);

                var player2Properties = new Hashtable { { "Name", Player2 } };

                var joinResponse = client2.JoinRoom(roomName, player2Properties, 0, new RoomOptions(), false, true);

                var room = joinResponse.GameProperties;
                Assert.IsNotNull(room);
                Assert.AreEqual(7, room.Count);

                Assert.IsNotNull(room[GamePropertyKey.IsOpen], "IsOpen");
                Assert.IsNotNull(room[GamePropertyKey.IsVisible], "IsVisisble");
                Assert.IsNotNull(room[GamePropertyKey.PropsListedInLobby], "PropertiesInLobby");
                Assert.IsNotNull(room["P1"], "P1");
                Assert.IsNotNull(room["P2"], "P2");


                Assert.AreEqual(true, room[GamePropertyKey.IsOpen], "IsOpen");
                Assert.AreEqual(true, room[GamePropertyKey.IsVisible], "IsVisisble");
                //TODO Replace to GamePropertyKey
                Assert.AreEqual(1, room[GamePropertyKey.MasterClientId], "MasterClientId");
                Assert.AreEqual(3, ((string[])room[GamePropertyKey.PropsListedInLobby]).Length, "PropertiesInLobby");
                Assert.AreEqual("L1", ((string[])room[GamePropertyKey.PropsListedInLobby])[0], "PropertiesInLobby");
                Assert.AreEqual("L2", ((string[])room[GamePropertyKey.PropsListedInLobby])[1], "PropertiesInLobby");
                Assert.AreEqual("L3", ((string[])room[GamePropertyKey.PropsListedInLobby])[2], "PropertiesInLobby");
                Assert.AreEqual(1, room["P1"], "P1");
                Assert.AreEqual(2, room["P2"], "P2");

                // set properties: 
                var setProperties = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>()
                };

                setProperties.Parameters[ParameterCode.Broadcast] = true;
                setProperties.Parameters[ParameterCode.Properties] = new Hashtable { { "P3", 3 }, { "P1", null }, { "P2", 20 } };

                var setPropResponse = client1.SendRequestAndWaitForResponse(setProperties);
                Assert.AreEqual(OperationCode.SetProperties, setPropResponse.OperationCode);
                Assert.AreEqual(ErrorCode.Ok, setPropResponse.ReturnCode, setPropResponse.DebugMessage);

                var ev = client2.WaitForEvent(EventCode.PropertiesChanged);

                room = (Hashtable)ev.Parameters[ParameterCode.Properties];
                Assert.IsNotNull(room);
                Assert.AreEqual(3, room.Count);

                Assert.IsNull(room["P1"], "P1");
                Assert.IsNotNull(room["P2"], "P2");
                Assert.IsNotNull(room["P3"], "P3");

                Assert.AreEqual(null, room["P1"], "P1");
                Assert.AreEqual(20, room["P2"], "P2");
                Assert.AreEqual(3, room["P3"], "P3");

                var getProperties = new OperationRequest { OperationCode = OperationCode.GetProperties, Parameters = new Dictionary<byte, object>() };
                getProperties.Parameters[ParameterCode.Properties] = PropertyType.Game;

                var getPropResponse = client2.SendRequestAndWaitForResponse(getProperties);

                Assert.AreEqual(OperationCode.GetProperties, getPropResponse.OperationCode);
                Assert.AreEqual(ErrorCode.Ok, getPropResponse.ReturnCode, getPropResponse.DebugMessage);

                room = (Hashtable)getPropResponse.Parameters[ParameterCode.GameProperties];
                Assert.IsNotNull(room);
                Assert.AreEqual(8, room.Count);

                Assert.IsNotNull(room[GamePropertyKey.IsOpen], "IsOpen");
                Assert.IsNotNull(room[GamePropertyKey.IsVisible], "IsVisisble");
                Assert.IsNotNull(room[GamePropertyKey.PropsListedInLobby], "PropertiesInLobby");
                Assert.IsNull(room["P1"], "P1");
                Assert.IsNotNull(room["P2"], "P2");
                Assert.IsNotNull(room["P3"], "P3");


                Assert.AreEqual(true, room[GamePropertyKey.IsOpen], "IsOpen");
                Assert.AreEqual(true, room[GamePropertyKey.IsVisible], "IsVisisble");
                Assert.AreEqual(3, ((string[])room[GamePropertyKey.PropsListedInLobby]).Length, "PropertiesInLobby");
                Assert.AreEqual("L1", ((string[])room[GamePropertyKey.PropsListedInLobby])[0], "PropertiesInLobby");
                Assert.AreEqual("L2", ((string[])room[GamePropertyKey.PropsListedInLobby])[1], "PropertiesInLobby");
                Assert.AreEqual("L3", ((string[])room[GamePropertyKey.PropsListedInLobby])[2], "PropertiesInLobby");
                Assert.AreEqual(null, room["P1"], "P1");
                Assert.AreEqual(20, room["P2"], "P2");
                Assert.AreEqual(3, room["P3"], "P3");
            }
            finally
            {
                DisposeClients(client1, client2);
            }
        }

        [Test]
        public void SetPropertiesForLobby()
        {

            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;

            try
            {
                client1 = this.CreateMasterClientAndAuthenticate(Player1);

                Assert.IsTrue(client1.OpJoinLobby());
                var ev = client1.WaitForEvent(EventCode.GameList);
                Assert.AreEqual(EventCode.GameList, ev.Code);
                var gameList = (Hashtable)ev.Parameters[ParameterCode.GameList];
                this.CheckGameListCount(0, gameList);

                client2 = this.CreateMasterClientAndAuthenticate(Player2);

                Assert.IsTrue(client2.OpJoinLobby());
                ev = client2.WaitForEvent(EventCode.GameList);
                Assert.AreEqual(EventCode.GameList, ev.Code);
                gameList = (Hashtable)ev.Parameters[ParameterCode.GameList];
                this.CheckGameListCount(0, gameList);

                client1.EventQueueClear();
                client2.EventQueueClear();

                client1.OperationResponseQueueClear();
                client2.OperationResponseQueueClear();

                // open game
                string roomName = "SetPropertiesForLobby_" + Guid.NewGuid().ToString().Substring(0, 6);

                var player1Properties = new Hashtable();
                player1Properties.Add("Name", Player1);

                var gameProperties = new Hashtable();
                gameProperties["P1"] = 1;
                gameProperties["P2"] = 2;

                gameProperties["L1"] = 1;
                gameProperties["L2"] = 2;


                var lobbyProperties = new string[] { "L1", "L2", "L3" };

                var createRoomResponse = client1.CreateRoom(
                    roomName,
                    new RoomOptions
                    {
                        CustomRoomProperties = gameProperties,
                        CustomRoomPropertiesForLobby = lobbyProperties
                    }, TypedLobby.Default, player1Properties,
                    false);

                var gameServerAddress1 = createRoomResponse.Address;
                Console.WriteLine("Created room " + roomName + " on GS: " + gameServerAddress1);

                // move 1st client to GS: 
                this.ConnectAndAuthenticate(client1, gameServerAddress1, client1.UserId);

                client1.CreateRoom(
                    roomName,
                    new RoomOptions
                    {
                        CustomRoomProperties = gameProperties,
                        CustomRoomPropertiesForLobby = lobbyProperties
                    },
                    TypedLobby.Default,
                    player1Properties,
                    true);

                // get own join event: 
                ev = client1.WaitForEvent();
                Assert.AreEqual(EventCode.Join, ev.Code);
                Assert.AreEqual(1, ev.Parameters[ParameterCode.ActorNr]);

                var actorList = (int[])ev.Parameters[ParameterCode.ActorList];
                Assert.AreEqual(1, actorList.Length);
                Assert.AreEqual(1, actorList[0]);

                var ActorProperties = ((Hashtable)ev.Parameters[ParameterCode.PlayerProperties]);
                Assert.AreEqual(Player1, ActorProperties["Name"]);

                ev = client2.WaitForEvent(EventCode.GameListUpdate);

                Hashtable roomList = null;
                // we have this loop in order to protect test from unexpected update, which we get because of other tests
                var exitLoop = false;
                while (!exitLoop)
                {
                    roomList = (Hashtable)ev.Parameters[ParameterCode.GameList];

                    Assert.GreaterOrEqual(roomList.Count, 1);

                    if (roomList[roomName] == null)
                    {
                        ev = client2.WaitForEvent(EventCode.GameListUpdate, 12 * ConnectPolicy.WaitTime);
                    }
                    else
                    {
                        exitLoop = true;
                    }
                }
                var room = (Hashtable)roomList[roomName];
                Assert.IsNotNull(room);
                Assert.AreEqual(5, room.Count);

                Assert.IsNotNull(room[GamePropertyKey.IsOpen], "IsOpen");
                Assert.IsNotNull(room[GamePropertyKey.MaxPlayers], "MaxPlayers");
                Assert.IsNotNull(room[GamePropertyKey.PlayerCount], "PlayerCount");
                Assert.IsNotNull(room["L1"], "L1");
                Assert.IsNotNull(room["L2"], "L2");


                Assert.AreEqual(true, room[GamePropertyKey.IsOpen], "IsOpen");
                Assert.AreEqual(0, room[GamePropertyKey.MaxPlayers], "MaxPlayers");
                Assert.AreEqual(1, room[GamePropertyKey.PlayerCount], "PlayerCount");
                Assert.AreEqual(1, room["L1"], "L1");
                Assert.AreEqual(2, room["L2"], "L2");

                client1.OpSetPropertiesOfRoom(new Hashtable { { "L3", 3 }, { "L1", null }, { "L2", 20 } });


                ev = client2.WaitForEvent(EventCode.GameListUpdate);

                roomList = (Hashtable)ev.Parameters[ParameterCode.GameList];
                Assert.AreEqual(1, roomList.Count);

                room = (Hashtable)roomList[roomName];
                Assert.IsNotNull(room);
                Assert.AreEqual(5, room.Count);

                Assert.IsNotNull(room[GamePropertyKey.IsOpen], "IsOpen");
                Assert.IsNotNull(room[GamePropertyKey.MaxPlayers], "MaxPlayers");
                Assert.IsNotNull(room[GamePropertyKey.PlayerCount], "PlayerCount");
                Assert.IsNotNull(room["L2"], "L2");
                Assert.IsNotNull(room["L3"], "L3");

                Assert.AreEqual(true, room[GamePropertyKey.IsOpen], "IsOpen");
                Assert.AreEqual(0, room[GamePropertyKey.MaxPlayers], "MaxPlayers");
                Assert.AreEqual(1, room[GamePropertyKey.PlayerCount], "PlayerCount");
                Assert.AreEqual(20, room["L2"], "L2");
                Assert.AreEqual(3, room["L3"], "L3");

                client1.SendRequestAndWaitForResponse(new OperationRequest { OperationCode = OperationCode.Leave });
            }
            finally
            {
                DisposeClients(client1, client2, client1);
            }
        }

        [Test]
        public void SuppressRoomEvents()
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName("SuppressRoomEvents_");

                client1 = this.CreateMasterClientAndAuthenticate(Player1);
                client2 = this.CreateMasterClientAndAuthenticate(Player2);

                var createGameResponse = client1.CreateGame(roomName, true, true, 4);

                // switch client 1 to GS 
                this.ConnectAndAuthenticate(client1, createGameResponse.Address, client1.UserId);

                var createRequest = new OperationRequest { OperationCode = OperationCode.CreateGame, Parameters = new Dictionary<byte, object>() };
                createRequest.Parameters.Add(ParameterCode.RoomName, createGameResponse.GameId);
                createRequest.Parameters.Add((byte)Operations.ParameterCode.SuppressRoomEvents, true);
                client1.SendRequestAndWaitForResponse(createRequest);

                this.ConnectAndAuthenticate(client2, createGameResponse.Address, client1.UserId);
                client2.JoinGame(roomName);

                EventData eventData;
                Assert.IsFalse(client1.TryWaitForEvent(EventCode.Join, ConnectPolicy.WaitTime, out eventData));

                client1.Dispose();
                Assert.IsFalse(client2.TryWaitForEvent(EventCode.Leave, ConnectPolicy.WaitTime, out eventData));

            }
            finally
            {
                DisposeClients(client1, client2);
            }
        }

        [Test]
        public void MatchByProperties()
        {

            UnifiedTestClient masterClient = null;
            UnifiedTestClient gameClient = null;

            try
            {
                // create game on the game server
                string roomName = this.GenerateRandomizedRoomName("BroadcastProperties_");

                var gameProperties = new Hashtable();
                gameProperties["P1"] = 1;
                gameProperties["P2"] = 2;
                gameProperties["L1"] = 1;
                gameProperties["L2"] = 2;
                gameProperties["L3"] = 3;

                var lobbyProperties = new string[] { "L1", "L2", "L3" };

                gameClient = this.CreateGameOnGameServer(Player1, roomName, null, 0, true, true, 0, gameProperties, lobbyProperties);

                // test matchmaking
                masterClient = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient.EventQueueClear();
                masterClient.OperationResponseQueueClear();

                var joinRequest = new JoinRandomGameRequest
                {
                    JoinRandomType = (byte)MatchmakingMode.FillRoom,
                    GameProperties = new Hashtable()
                };


                joinRequest.GameProperties.Add("N", null);
                masterClient.OperationResponseQueueClear();
                masterClient.JoinRandomGame(joinRequest, ErrorCode.NoRandomMatchFound);

                joinRequest.GameProperties.Clear();
                joinRequest.GameProperties.Add("L1", 5);
                masterClient.OperationResponseQueueClear();
                masterClient.JoinRandomGame(joinRequest, ErrorCode.NoRandomMatchFound);

                joinRequest.GameProperties.Clear();
                joinRequest.GameProperties.Add("L1", 1);
                joinRequest.GameProperties.Add("L2", 1);
                masterClient.OperationResponseQueueClear();
                masterClient.JoinRandomGame(joinRequest, ErrorCode.NoRandomMatchFound);

                joinRequest.GameProperties.Clear();
                joinRequest.GameProperties.Add("L1", 1);
                joinRequest.GameProperties.Add("L2", 2);
                masterClient.OperationResponseQueueClear();
                masterClient.JoinRandomGame(joinRequest, ErrorCode.Ok);

                gameClient.LeaveGame();
            }
            finally
            {
                DisposeClients(masterClient, gameClient);
            }
        }

        [Test]
        public void MatchmakingTypes()
        {
            UnifiedTestClient masterClient = null;
            var gameClients = new UnifiedTestClient[3];
            var roomNames = new string[3];
            try
            {
                // create games on game server
                for (int i = 0; i < gameClients.Length; i++)
                {
                    roomNames[i] = this.GenerateRandomizedRoomName("MatchmakingTypes_" + i + "_");
                    var createGameRequest = new CreateGameRequest { GameId = roomNames[i] };
                    gameClients[i] = this.CreateGameOnGameServer("Player" + i, createGameRequest);
                }

                // fill room - 3x: 
                masterClient = this.CreateMasterClientAndAuthenticate(Player1);
                var joinRandomRequest = new JoinRandomGameRequest { JoinRandomType = (byte)MatchmakingMode.FillRoom };

                masterClient.JoinRandomGame(joinRandomRequest, ErrorCode.Ok, roomNames[0]);
                masterClient.JoinRandomGame(joinRandomRequest, ErrorCode.Ok, roomNames[0]);
                masterClient.JoinRandomGame(joinRandomRequest, ErrorCode.Ok, roomNames[0]);


                // serial matching - 4x: 
                joinRandomRequest = new JoinRandomGameRequest { JoinRandomType = (byte)MatchmakingMode.SerialMatching };
                masterClient.JoinRandomGame(joinRandomRequest, ErrorCode.Ok, roomNames[1]);
                masterClient.JoinRandomGame(joinRandomRequest, ErrorCode.Ok, roomNames[2]);
                masterClient.JoinRandomGame(joinRandomRequest, ErrorCode.Ok, roomNames[0]);
                masterClient.JoinRandomGame(joinRandomRequest, ErrorCode.Ok, roomNames[1]);

                for (int i = 0; i < gameClients.Length; i++)
                {
                    gameClients[i].LeaveGame();
                }
            }
            finally
            {
                DisposeClients(masterClient);
                DisposeClients(gameClients);
            }
        }

        [Test]
        public void FiendFriends()
        {
            var userIds = new string[] { "User1", "User2", "User3" };

            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;
            UnifiedTestClient client3 = null;

            try
            {

                bool[] onlineStates;
                string[] userStates;

                // connect first client 
                client1 = this.CreateMasterClientAndAuthenticate(userIds[0]);
                client1.FindFriends(userIds, out onlineStates, out userStates);
                Assert.AreEqual(true, onlineStates[0]);
                Assert.AreEqual(false, onlineStates[1]);
                Assert.AreEqual(false, onlineStates[2]);
                Assert.AreEqual(string.Empty, userStates[0]);
                Assert.AreEqual(string.Empty, userStates[1]);
                Assert.AreEqual(string.Empty, userStates[2]);

                // connect second client 
                client2 = this.CreateMasterClientAndAuthenticate(userIds[1]);
                client1.FindFriends(userIds, out onlineStates, out userStates);
                Assert.AreEqual(true, onlineStates[0]);
                Assert.AreEqual(true, onlineStates[1]);
                Assert.AreEqual(false, onlineStates[2]);
                Assert.AreEqual(string.Empty, userStates[0]);
                Assert.AreEqual(string.Empty, userStates[1]);
                Assert.AreEqual(string.Empty, userStates[2]);

                // connect third client and create game on game server
                client3 = this.CreateMasterClientAndAuthenticate(userIds[2]);
                var response = client3.CreateGame("FiendFriendsGame1");

                this.ConnectAndAuthenticate(client3, response.Address, userIds[2]);
                client3.CreateGame("FiendFriendsGame1");

                client1.FindFriends(userIds, out onlineStates, out userStates);
                Assert.AreEqual(true, onlineStates[0]);
                Assert.AreEqual(true, onlineStates[1]);
                Assert.AreEqual(true, onlineStates[2]);
                Assert.AreEqual(string.Empty, userStates[0]);
                Assert.AreEqual(string.Empty, userStates[1]);
                Assert.AreEqual("FiendFriendsGame1", userStates[2]);
                client1.EventQueueClear();

                // disconnect client2 and client3
                client2.Disconnect();
                client3.Disconnect();

                // wait some time until disconnect of client 3 was reported to game server
                Thread.Sleep(300);

                client1.FindFriends(userIds, out onlineStates, out userStates);
                Assert.AreEqual(true, onlineStates[0]);
                Assert.AreEqual(false, onlineStates[1], "MasterClient2 disconencted, but not shown as offlien");
                Assert.AreEqual(false, onlineStates[2], "GameClient3 disconnected, but was not published to master in time");
                Assert.AreEqual(string.Empty, userStates[0]);
                Assert.AreEqual(string.Empty, userStates[1]);
                Assert.AreEqual(string.Empty, userStates[2]);

            }
            finally
            {
                DisposeClients(client1, client2, client3);
            }
        }

        #region Lobby Tests

        [Test]
        public void SqlLobbyMatchmaking()
        {
            UnifiedTestClient masterClient = null;
            UnifiedTestClient[] gameClients = null;

            try
            {
                const string lobbyName = "SqlLobby1";
                const byte lobbyType = 2;

                gameClients = new UnifiedTestClient[3];

                for (int i = 0; i < gameClients.Length; i++)
                {
                    var gameProperties = new Hashtable();
                    switch (i)
                    {
                        case 1:
                            gameProperties.Add("C0", 10);
                            break;

                        case 2:
                            gameProperties.Add("C0", "Map1");
                            break;
                    }

                    var roomName = "SqlLobbyMatchmaking" + i;
                    gameClients[i] = this.CreateGameOnGameServer("GameClient" + i, roomName, lobbyName, lobbyType, true, true, 0, gameProperties,
                        null);
                }


                masterClient = this.CreateMasterClientAndAuthenticate("Tester");

                // client didn't joined lobby so all requests without 
                // a lobby specified should not return a match
                masterClient.JoinRandomGame(null, null, ErrorCode.NoRandomMatchFound);
                masterClient.JoinRandomGame(null, "C0=10", ErrorCode.NoRandomMatchFound);

                // specifing the lobbyname and type should give some matches
                masterClient.JoinLobby(lobbyName, lobbyType);
                masterClient.JoinRandomGame(null, null, ErrorCode.Ok, lobbyName, lobbyType);
                masterClient.JoinRandomGame(null, "C0=1", ErrorCode.NoRandomMatchFound, lobbyName, lobbyType);
                masterClient.JoinRandomGame(null, "C0<10", ErrorCode.NoRandomMatchFound, lobbyName, lobbyType);
                masterClient.JoinRandomGame(null, "C0>10", ErrorCode.Ok, lobbyName, lobbyType, "SqlLobbyMatchmaking2");
                masterClient.JoinRandomGame(null, "C0=10", ErrorCode.Ok, lobbyName, lobbyType, "SqlLobbyMatchmaking1");
                masterClient.JoinRandomGame(null, "C0>0", ErrorCode.Ok, lobbyName, lobbyType, "SqlLobbyMatchmaking1");
                masterClient.JoinRandomGame(null, "C0<20", ErrorCode.Ok, lobbyName, lobbyType, "SqlLobbyMatchmaking1");
                masterClient.JoinRandomGame(null, "C0='Map2'", ErrorCode.NoRandomMatchFound, lobbyName, lobbyType);
                masterClient.JoinRandomGame(null, "C0='Map1'", ErrorCode.Ok, lobbyName, lobbyType, "SqlLobbyMatchmaking2");

                // join client to lobby. Matches could be found without 
                // specifing the lobby
                masterClient.JoinLobby(lobbyName, lobbyType);
                masterClient.JoinRandomGame(null, null, ErrorCode.Ok);
                masterClient.JoinRandomGame(null, "C0=1", ErrorCode.NoRandomMatchFound);
                masterClient.JoinRandomGame(null, "C0<10", ErrorCode.NoRandomMatchFound);
                masterClient.JoinRandomGame(null, "C0>10", ErrorCode.Ok, null, null, "SqlLobbyMatchmaking2");
                masterClient.JoinRandomGame(null, "C0=10", ErrorCode.Ok);
                masterClient.JoinRandomGame(null, "C0>0", ErrorCode.Ok, null, null, "SqlLobbyMatchmaking1");
                masterClient.JoinRandomGame(null, "C0<20", ErrorCode.Ok, null, null, "SqlLobbyMatchmaking1");
                masterClient.JoinRandomGame(null, "C0='Map2'", ErrorCode.NoRandomMatchFound);
                masterClient.JoinRandomGame(null, "C0='Map1'", ErrorCode.Ok, null, null, "SqlLobbyMatchmaking2");

                // invalid sql should return error
                var joinResponse = masterClient.JoinRandomGame(null, "GRTF", ErrorCode.InvalidOperationCode);
                Assert.AreEqual(ErrorCode.InvalidOperationCode, joinResponse.ReturnCode);
            }
            finally
            {
                DisposeClients(masterClient);
                DisposeClients(gameClients);
            }
        }

        [Test]
        public void SqlLobbyPropertiesUpdateBug()
        {
            UnifiedTestClient masterClient = null;
            UnifiedTestClient gameClients = null;

            try
            {
                const string lobbyName = "SqlLobby1";
                const byte lobbyType = 2;

                var gameProperties = new Hashtable {{"C0", 10}};
                var roomName = MethodBase.GetCurrentMethod().Name;
                gameClients = this.CreateGameOnGameServer("GameClient", roomName, lobbyName, lobbyType, true, true, 0, gameProperties, null);

                var setPrpertiesRequest = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Properties, new Hashtable {{"C0", 1}}}
                    }
                };
                gameClients.SendRequestAndWaitForResponse(setPrpertiesRequest);

                masterClient = this.CreateMasterClientAndAuthenticate("Tester");

                //// client didn't joined lobby so all requests without 
                //// a lobby specified should not return a match
                //masterClient.JoinRandomGame(null, null, ErrorCode.NoRandomMatchFound);
                //masterClient.JoinRandomGame(null, "C0=10", ErrorCode.NoRandomMatchFound);

                //// specifing the lobbyname and type should give some matches
                //masterClient.JoinLobby(lobbyName, lobbyType);
                masterClient.JoinRandomGame(null, null, ErrorCode.Ok, lobbyName, lobbyType);
                masterClient.JoinRandomGame(null, "C0=1", ErrorCode.Ok, lobbyName, lobbyType);
                masterClient.JoinRandomGame(null, "C0=10", ErrorCode.NoRandomMatchFound, lobbyName, lobbyType, roomName);

                // join client to lobby. Matches could be found without 
                // specifing the lobby
                //masterClient.JoinLobby(lobbyName, lobbyType);
                //masterClient.JoinRandomGame(null, null);
                //masterClient.JoinRandomGame(null, "C0=1", ErrorCode.NoRandomMatchFound);
                //masterClient.JoinRandomGame(null, "C0<10", ErrorCode.NoRandomMatchFound);
                //masterClient.JoinRandomGame(null, "C0>10", ErrorCode.Ok, null, null, "SqlLobbyMatchmaking2");
                //masterClient.JoinRandomGame(null, "C0=10");
                //masterClient.JoinRandomGame(null, "C0>0", ErrorCode.Ok, null, null, "SqlLobbyMatchmaking1");
                //masterClient.JoinRandomGame(null, "C0<20", ErrorCode.Ok, null, null, "SqlLobbyMatchmaking1");
                //masterClient.JoinRandomGame(null, "C0='Map2'", ErrorCode.NoRandomMatchFound);
                //masterClient.JoinRandomGame(null, "C0='Map1'", ErrorCode.Ok, null, null, "SqlLobbyMatchmaking2");

                // invalid sql should return error
                //var joinResponse = masterClient.JoinRandomGame(null, "GRTF", ErrorCode.InvalidOperationCode);
                //Assert.AreEqual(ErrorCode.InvalidOperationCode, joinResponse.ReturnCode);
            }
            finally
            {
                DisposeClients(masterClient);
                DisposeClients(gameClients);
            }
        }

        [Test]
        public void SqlLobbyMaxPlayersNoFilter()
        {
            UnifiedTestClient masterClient = null;
            UnifiedTestClient gameClient1 = null;
            UnifiedTestClient gameClient2 = null;

            const string lobbyName = "SqlLobbyMaxPlayers";
            const byte lobbyType = 2;

            try
            {
                string roomName = this.GenerateRandomizedRoomName("SqlLobbyMaxPlayers_");
                gameClient1 = this.CreateGameOnGameServer(Player1, roomName, lobbyName, lobbyType, true, true, 1, null, null);

                // join 2nd client on master - full: 
                masterClient = this.CreateMasterClientAndAuthenticate("Tester");

                masterClient.JoinRandomGame(null, null, ErrorCode.NoRandomMatchFound);
                masterClient.JoinRandomGame(null, "C0=10", ErrorCode.NoRandomMatchFound);

                // specifing the lobbyname and type should give some matches
                masterClient.JoinLobby(lobbyName, lobbyType);
                masterClient.JoinGame(roomName, ErrorCode.GameFull);

                // join random 2nd client on master - full: 
                var joinRequest = new JoinRandomGameRequest();
                masterClient.JoinRandomGame(joinRequest, ErrorCode.NoRandomMatchFound);
                joinRequest.JoinRandomType = (byte) MatchmakingMode.SerialMatching;
                masterClient.JoinRandomGame(joinRequest, ErrorCode.NoRandomMatchFound);
                joinRequest.JoinRandomType = (byte) MatchmakingMode.RandomMatching;
                masterClient.JoinRandomGame(joinRequest, ErrorCode.NoRandomMatchFound);
                masterClient.Dispose();

                // join directly on GS: 
                gameClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                this.ConnectAndAuthenticate(gameClient2, gameClient1.RemoteEndPoint, gameClient2.UserId);
                gameClient2.JoinGame(roomName, ErrorCode.GameFull);
            }
            finally
            {
                DisposeClients(masterClient, gameClient1, gameClient2);
            }
        }

        [Test]
        public void SqlLobbyPlayerCountChanged()
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;
            UnifiedTestClient client3 = null;
            UnifiedTestClient client4 = null;

            const string lobbyName = "SqlLobbyPlayerCountChanging";
            const byte lobbyType = 2;

            try
            {
                string roomName = this.GenerateRandomizedRoomName("SqlLobbyPlayerCountChanging_");
                client1 = this.CreateGameOnGameServer("Client1", roomName, lobbyName, lobbyType, true, true, 3, null, null);


                // join 2nd client 
                client2 = this.CreateMasterClientAndAuthenticate("Client2");
                client2.JoinLobby(lobbyName, lobbyType);
                var response = client2.JoinGame(roomName);

                // ok
                this.ConnectAndAuthenticate(client2, response.Address, client2.UserId);
                client2.JoinGame(roomName);

                Thread.Sleep(500);

                // ok
                client3 = this.CreateMasterClientAndAuthenticate("Client3");
                client3.JoinLobby(lobbyName, lobbyType);
                response = client3.JoinGame(roomName);

                this.ConnectAndAuthenticate(client3, response.Address, client3.UserId);
                client3.JoinGame(roomName);

                // test with client #4 
                client4 = this.CreateMasterClientAndAuthenticate("Client4");
                client4.JoinLobby(lobbyName, lobbyType);

                // ok
                this.ConnectAndAuthenticate(client4, response.Address, client4.UserId);

                // join directly - without any prior join on Master: 
                client4.JoinGame(roomName, ErrorCode.GameFull);

                client3.Disconnect();
                Thread.Sleep(500);

                this.ConnectAndAuthenticate(client4, response.Address);
                // now succeed:
                client4.JoinGame(roomName);
            }
            finally
            {
                DisposeClients(client1, client2, client3, client4);
            }
        }

        [Test]
        public void SqlLobbyMaxPlayersWithFilter()
        {

            UnifiedTestClient gameClient1 = null;

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;

            const string lobbyName = "SqlLobbyMaxPlayers";
            const byte lobbyType = 2;

            try
            {
                string roomName = this.GenerateRandomizedRoomName("SqlLobbyMaxPlayers_");
                var gameProperties = new Hashtable();
                gameProperties["C0"] = 10;
                gameProperties["C5"] = "Name";

                gameClient1 = this.CreateGameOnGameServer(Player1, roomName, lobbyName, lobbyType, true, true, 2, gameProperties, null);

                masterClient1 = this.CreateMasterClientAndAuthenticate("Tester1");
                masterClient2 = this.CreateMasterClientAndAuthenticate("Tester2");

                // join 2nd client on master - no matches without lobby:
                masterClient1.JoinRandomGame(null, null, ErrorCode.NoRandomMatchFound);
                masterClient1.JoinRandomGame(null, "C0=10", ErrorCode.NoRandomMatchFound);

                // specifing the lobbyname and type should give some matches
                masterClient1.JoinLobby(lobbyName, lobbyType);
                masterClient2.JoinLobby(lobbyName, lobbyType);


                // join random - with filter:
                var joinRequest = new JoinRandomGameRequest {QueryData = "C0=10"};
                masterClient1.JoinRandomGame(joinRequest, ErrorCode.Ok);
                masterClient2.JoinRandomGame(joinRequest, ErrorCode.NoRandomMatchFound);


                // join directly on GS: 
                this.ConnectAndAuthenticate(masterClient1, gameClient1.RemoteEndPoint, masterClient1.UserId);
                masterClient1.JoinGame(roomName);

                masterClient2.JoinGame(roomName, ErrorCode.GameFull);

                // disconnect second client
                gameClient1.LeaveGame();
                gameClient1.Dispose();
                Thread.Sleep(500); // give the app lobby some time to update the game state

                masterClient2.JoinRandomGame(joinRequest, ErrorCode.Ok);

            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, gameClient1);
            }
        }

        [Test]
        public void SqlLobbyMaxPlayer()
        {
            UnifiedTestClient masterClient = null;
            UnifiedTestClient gameClient = null;

            try
            {
                const string lobbyName = "SqlMaxPlayerLobby";
                const string roomName = "SqlMaxPlayer";
                const string roomName2 = "SqlMaxPlayer2";
                const string roomName3 = "SqlMaxPlayer3";
                const byte lobbyType = 2;

                var customRoomProperties = new Hashtable();
                customRoomProperties["C0"] = 1;

                var propsToListInLobby = new string[customRoomProperties.Count];
                propsToListInLobby[0] = "C0";


                gameClient = this.CreateGameOnGameServer(Player1, roomName, lobbyName, lobbyType, true, true, null, customRoomProperties,
                    propsToListInLobby);
                masterClient = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient.JoinRandomGame(null, "C0>0", ErrorCode.Ok, lobbyName, lobbyType);
                masterClient.Disconnect();
                gameClient.Disconnect();

                gameClient = this.CreateGameOnGameServer(Player1, roomName2, lobbyName, lobbyType, true, true, 2, customRoomProperties,
                    propsToListInLobby);
                masterClient = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient.JoinRandomGame(null, "C0>0", ErrorCode.Ok, lobbyName, lobbyType);
                masterClient.Disconnect();
                gameClient.Disconnect();

                gameClient = this.CreateGameOnGameServer(Player1, roomName3, lobbyName, lobbyType, true, true, 4, customRoomProperties,
                    propsToListInLobby);
                masterClient = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient.JoinRandomGame(null, "C0>0", ErrorCode.Ok, lobbyName, lobbyType);
                masterClient.Disconnect();
                gameClient.Disconnect();

            }
            finally
            {
                DisposeClients(masterClient, gameClient);
            }
        }

        [Test]
        [Explicit("Very long running test")]
        public void SqlLobbyMaxPlayersWithFilterJoinTimeout()
        {
            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;

            UnifiedTestClient gameClient1 = null;

            const string lobbyName = "SqlLobbyMaxPlayers";
            const byte lobbyType = 2;

            try
            {
                var roomName = this.GenerateRandomizedRoomName("SqlLobbyMaxPlayers_");
                var gameProperties = new Hashtable();
                gameProperties["C0"] = 10;
                gameProperties["C5"] = "Name";

                gameClient1 = this.CreateGameOnGameServer(null, roomName, lobbyName, lobbyType, true, true, 2, gameProperties, null);

                var joinRequest = new JoinRandomGameRequest {QueryData = "C0=10"};

                // join first client
                masterClient1 = this.CreateMasterClientAndAuthenticate("Tester1");
                masterClient1.JoinLobby(lobbyName, lobbyType);
                masterClient1.JoinRandomGame(joinRequest, (short) Photon.Common.ErrorCode.Ok);

                // join second client
                // should fail because first client is still connecting to the game server
                masterClient2 = this.CreateMasterClientAndAuthenticate("Tester2");
                masterClient2.JoinLobby(lobbyName, lobbyType);
                masterClient2.JoinRandomGame(joinRequest, (short) Photon.Common.ErrorCode.NoMatchFound);
                masterClient2.Dispose();

                // wait for join timeout (default is currently 15 seconds)
                Thread.Sleep(30000);

                // join second client
                // should work because first client has timed out connecting to the game server
                masterClient2 = this.CreateMasterClientAndAuthenticate("Tester2");
                masterClient2.JoinLobby(lobbyName, lobbyType);
                masterClient2.JoinRandomGame(joinRequest, (short) Photon.Common.ErrorCode.Ok);
                masterClient2.Dispose();
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, gameClient1);
            }
        }

        [Test]
        public void LobbyStatistics()
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;

            try
            {
                var authParameter = new Dictionary<byte, object> {{(byte) Operations.ParameterCode.LobbyStats, true}};

                // authenticate client and check if the Lobbystats event will be received
                // Remarks: The event cannot be checked for a specific lobby count because
                // previous tests may have created lobbies. 
                client1 = this.CreateMasterClientAndAuthenticate(Player1, authParameter);
                var lobbyStatsEvent = client1.WaitForEvent((byte) Events.EventCode.LobbyStats);
                Assert.AreEqual((byte) Events.EventCode.LobbyStats, lobbyStatsEvent.Code);

                // Join to a new lobby and check if the new lobby will listet
                // for new clients
                var lobbyName = this.GenerateRandomizedRoomName("LobbyStatisticTest");
                const byte lobbyType = 2;
                client1.JoinLobby(lobbyName, lobbyType);

                client2 = this.CreateMasterClientAndAuthenticate(Player2, authParameter);
                lobbyStatsEvent = client2.WaitForEvent((byte) Events.EventCode.LobbyStats);
                Assert.AreEqual((byte) Events.EventCode.LobbyStats, lobbyStatsEvent.Code);

                object temp;
                lobbyStatsEvent.Parameters.TryGetValue((byte) Operations.ParameterCode.LobbyName, out temp);
                var lobbyNames = GetParameter<string[]>(lobbyStatsEvent.Parameters, (byte) Operations.ParameterCode.LobbyName, "LobbyNames");
                var lobbyTypes = GetParameter<byte[]>(lobbyStatsEvent.Parameters, (byte) Operations.ParameterCode.LobbyType, "LobbyTypes");
                var peerCounts = GetParameter<int[]>(lobbyStatsEvent.Parameters, (byte) Operations.ParameterCode.PeerCount, "PeerCount");
                var gameCounts = GetParameter<int[]>(lobbyStatsEvent.Parameters, (byte) Operations.ParameterCode.GameCount, "GameCount");

                Assert.AreEqual(lobbyNames.Length, lobbyTypes.Length, "LobbyType count differs from LobbyName count");
                Assert.AreEqual(lobbyNames.Length, peerCounts.Length, "PeerCount count differs from LobbyName count");
                Assert.AreEqual(lobbyNames.Length, gameCounts.Length, "GameCount count differs from LobbyName count");

                var lobbyIndex = Array.IndexOf(lobbyNames, lobbyName);
                Assert.GreaterOrEqual(lobbyIndex, 0, "Lobby not found in statistics");
                Assert.AreEqual(lobbyType, lobbyTypes[lobbyIndex], "Wrong lobby type");
                Assert.AreEqual(1, peerCounts[lobbyIndex], "Wrong peer count");
                Assert.AreEqual(0, gameCounts[lobbyIndex], "Wrong game count");

                client2.Dispose();
                client2 = null;

                // create a new game for the lobby
                var gameName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);
                var createGameResponse = client1.CreateGame(gameName);
                this.ConnectAndAuthenticate(client1, createGameResponse.Address, client1.UserId);
                client1.CreateGame(gameName);

                // give the game server some time to report the game to the master server
                Thread.Sleep(100);

                // check if new game is listed in lobby statistics
                client2 = this.CreateMasterClientAndAuthenticate(Player2, authParameter);
                lobbyStatsEvent = client2.WaitForEvent((byte) Events.EventCode.LobbyStats);
                Assert.AreEqual((byte) Events.EventCode.LobbyStats, lobbyStatsEvent.Code);

                lobbyStatsEvent.Parameters.TryGetValue((byte) Operations.ParameterCode.LobbyName, out temp);
                lobbyNames = GetParameter<string[]>(lobbyStatsEvent.Parameters, (byte) Operations.ParameterCode.LobbyName, "LobbyNames");
                lobbyTypes = GetParameter<byte[]>(lobbyStatsEvent.Parameters, (byte) Operations.ParameterCode.LobbyType, "LobbyTypes");
                peerCounts = GetParameter<int[]>(lobbyStatsEvent.Parameters, (byte) Operations.ParameterCode.PeerCount, "PeerCount");
                gameCounts = GetParameter<int[]>(lobbyStatsEvent.Parameters, (byte) Operations.ParameterCode.GameCount, "GameCount");

                Assert.AreEqual(lobbyNames.Length, lobbyTypes.Length, "LobbyType count differs from LobbyName count");
                Assert.AreEqual(lobbyNames.Length, peerCounts.Length, "PeerCount count differs from LobbyName count");
                Assert.AreEqual(lobbyNames.Length, gameCounts.Length, "GameCount count differs from LobbyName count");

                lobbyIndex = Array.IndexOf(lobbyNames, lobbyName);
                Assert.GreaterOrEqual(lobbyIndex, 0, "Lobby not found in statistics");
                Assert.AreEqual(lobbyType, lobbyTypes[lobbyIndex], "Wrong lobby type");
                Assert.AreEqual(1, peerCounts[lobbyIndex], "Wrong peer count");
                Assert.AreEqual(1, gameCounts[lobbyIndex], "Wrong game count");
            }
            finally
            {
                DisposeClients(client1, client2);
            }
        }

        [Test]
        public void LobbyCreateTest()
        {
            if (this.IsOnline)
            {
                Assert.Ignore("Test mostly applicable to offline version of tests");
            }
            UnifiedTestClient client = null;
            const string LobbyName1 = "lobby1";
            const string LobbyName2 = "lobby2";
            const string LobbyName3 = "looby3";
            const string LobbyName4 = "looby4";

            const string GameName = "LobbyCreateTest";
            try
            {
                client = this.CreateMasterClientAndAuthenticate(Player1);

                var lobResponse = client.GetLobbyStats(null, null);
                var lobbiesCount = lobResponse.LobbyNames.Length;

                client.JoinLobby(LobbyName1);
                lobResponse = client.GetLobbyStats(null, null);
                Assert.AreEqual(++lobbiesCount, lobResponse.LobbyNames.Length);
                Assert.Contains(LobbyName1, lobResponse.LobbyNames);
                Assert.AreEqual(0, lobResponse.LobbyTypes[1]);

                client.JoinLobby(LobbyName1, 0);
                lobResponse = client.GetLobbyStats(null, null);
                Assert.AreEqual(lobbiesCount, lobResponse.LobbyNames.Length);
                Assert.Contains(LobbyName1, lobResponse.LobbyNames);
                Assert.AreEqual(0, lobResponse.LobbyTypes[1]);

                client.JoinLobby(LobbyName2, 1);
                lobResponse = client.GetLobbyStats(null, null);
                Assert.AreEqual(++lobbiesCount, lobResponse.LobbyNames.Length);
                Assert.Contains(LobbyName2, lobResponse.LobbyNames);
                Assert.AreEqual(1, lobResponse.LobbyTypes[lobResponse.LobbyTypes.Length - 1]);

                var createGame = new CreateGameRequest
                {
                    GameId = GameName,
                    LobbyName = LobbyName3,
                };

                client.CreateGame(createGame);

                lobResponse = client.GetLobbyStats(null, null);
                Assert.AreEqual(++lobbiesCount, lobResponse.LobbyNames.Length);
                Assert.Contains(LobbyName3, lobResponse.LobbyNames);
                Assert.AreEqual(0, lobResponse.LobbyTypes[lobResponse.LobbyTypes.Length - 1]);

                createGame = new CreateGameRequest
                {
                    GameId = GameName,
                    LobbyName = LobbyName4,
                };

                client.CreateGame(createGame, ErrorCode.GameIdAlreadyExists);

                lobResponse = client.GetLobbyStats(null, null);
                Assert.AreEqual(++lobbiesCount, lobResponse.LobbyNames.Length);
            }
            finally
            {
                DisposeClients(client);
            }
        }

        [Test]
        [Explicit("LobbyStatsPublishInterval property of game server settings must be set to 1 for this test to run")]
        public void LobbyStatisticsPublish()
        {
            UnifiedTestClient client = null;

            try
            {
                var authParameter = new Dictionary<byte, object>();
                authParameter.Add((byte) Operations.ParameterCode.LobbyStats, true);

                client = this.CreateMasterClientAndAuthenticate(null, authParameter);
                client.WaitForEvent((byte) Events.EventCode.LobbyStats);

                int count = 0;
                while (count < 3)
                {
                    client.WaitForEvent((byte) Events.EventCode.LobbyStats, 2000);
                    count++;
                }

            }
            finally
            {
                DisposeClients(client);
            }
        }

        [Test]
        public void LobbyStatisticsRequest()
        {
            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;

            try
            {
                var lobbyName = this.GenerateRandomizedRoomName("LobbyStatisticsRequest1");
                const byte lobbyType = 0;
                var lobbyName2 = this.GenerateRandomizedRoomName("LobbyStatisticsRequest2");
                const byte lobbyType2 = 2;
                const string roomName = "TestRoom";

                // join lobby on master
                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient1.JoinLobby(lobbyName, lobbyType);
                masterClient1.WaitForEvent((byte) Events.EventCode.GameList);

                // get stats for all lobbies
                var response = masterClient1.GetLobbyStats(null, null);
                var expectedLobbyNames = new string[] {lobbyName};
                var expectedLobbyTypes = new byte[] {lobbyType};
                var expectedPeerCount = new int[] {1};
                var expectedGameCount = new int[] {0};
                this.VerifyLobbyStatisticsFullList(response, expectedLobbyNames, expectedLobbyTypes, expectedPeerCount, expectedGameCount);

                // get stats for specific lobbies (the last second should not exists and return 0 for game and peer count)
                var lobbyNames = new string[] {lobbyName, lobbyName, lobbyName2};
                var lobbyTypes = new byte[] {lobbyType, lobbyType2, lobbyType2};

                response = masterClient1.GetLobbyStats(lobbyNames, lobbyTypes);
                expectedPeerCount = new int[] {1, 0, 0};
                expectedGameCount = new int[] {0, 0, 0};
                this.VerifyLobbyStatisticsList(response, expectedPeerCount, expectedGameCount);

                // join lobby on master with second client
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient2.JoinLobby(lobbyName, lobbyType);
                masterClient2.WaitForEvent((byte) Events.EventCode.GameList);

                // check if peer count has been updated
                response = masterClient2.GetLobbyStats(lobbyNames, lobbyTypes);
                expectedPeerCount = new int[] {2, 0, 0};
                expectedGameCount = new int[] {0, 0, 0};
                this.VerifyLobbyStatisticsList(response, expectedPeerCount, expectedGameCount);
                masterClient2.Disconnect();

                // join second client to another lobby 
                var masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);
                masterClient3.JoinLobby(lobbyName2, lobbyType2);
                masterClient3.WaitForEvent((byte) Events.EventCode.GameList);

                // check if peer count has been updated
                response = masterClient3.GetLobbyStats(lobbyNames, lobbyTypes);
                expectedPeerCount = new int[] {1, 0, 1};
                expectedGameCount = new int[] {0, 0, 0};
                this.VerifyLobbyStatisticsList(response, expectedPeerCount, expectedGameCount);

                // create game on master server
                var createGameResponse = masterClient3.CreateGame(roomName);
                masterClient3.Disconnect();

                // there should be on player in lobby3 even if the game is not created on the game server 
                response = masterClient1.GetLobbyStats(lobbyNames, lobbyTypes);
                expectedPeerCount = new int[] {1, 0, 1};
                expectedGameCount = new int[] {0, 0, 1};
                this.VerifyLobbyStatisticsList(response, expectedPeerCount, expectedGameCount);

                // create the game on the game server
                this.ConnectAndAuthenticate(masterClient3, createGameResponse.Address, masterClient3.UserId);
                masterClient3.CreateGame(roomName);
                Thread.Sleep(100); // give game server some time to report game update to master

                // check if peer and game count have been updated
                response = masterClient1.GetLobbyStats(lobbyNames, lobbyTypes);
                expectedPeerCount = new int[] {1, 0, 1};
                expectedGameCount = new int[] {0, 0, 1};
                this.VerifyLobbyStatisticsList(response, expectedPeerCount, expectedGameCount);

                // join second client to the game
                this.ConnectAndAuthenticate(masterClient2, createGameResponse.Address, masterClient3.UserId);
                masterClient2.JoinGame(roomName);
                Thread.Sleep(100);

                response = masterClient1.GetLobbyStats(lobbyNames, lobbyTypes);
                expectedPeerCount = new int[] {1, 0, 2};
                expectedGameCount = new int[] {0, 0, 1};
                this.VerifyLobbyStatisticsList(response, expectedPeerCount, expectedGameCount);

                // leave game on the game server
                masterClient3.LeaveGame();
                masterClient3.Dispose();
                Thread.Sleep(100); // give game server some time to report game update to master

                response = masterClient1.GetLobbyStats(lobbyNames, lobbyTypes);
                expectedPeerCount = new int[] {1, 0, 1};
                expectedGameCount = new int[] {0, 0, 1};
                this.VerifyLobbyStatisticsList(response, expectedPeerCount, expectedGameCount);

                // remove game on the game server
                masterClient2.LeaveGame();
                masterClient2.Dispose();
                Thread.Sleep(1000);

                response = masterClient1.GetLobbyStats(lobbyNames, lobbyTypes);
                expectedPeerCount = new int[] {1, 0, 0};
                expectedGameCount = new int[] {0, 0, 0};
                this.VerifyLobbyStatisticsList(response, expectedPeerCount, expectedGameCount);

                // check invalid operations
                lobbyNames = new string[] {lobbyName};
                lobbyTypes = new byte[] {lobbyType, lobbyType2, lobbyType2};
                masterClient1.GetLobbyStats(lobbyNames, lobbyTypes, ErrorCode.InvalidOperationCode);
                masterClient1.GetLobbyStats(lobbyNames, null, ErrorCode.InvalidOperationCode);

            }
            finally
            {
                DisposeClients(masterClient1, masterClient2);
            }
        }

        [TestCase(AppLobbyType.Default)]
        [TestCase(AppLobbyType.SqlLobby)]
//        [TestCase(AppLobbyType.AsyncRandomLobby)]
        [TestCase(AppLobbyType.ChannelLobby)]
        public void LobbyJoinLobbyGameCountLimitTest(AppLobbyType appLobbyType)
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("this tests requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;

            try
            {
                var lobbyName = this.GenerateRandomizedRoomName("LobbyStatisticsRequest1");
                var lobbyType = (byte)appLobbyType;
                const string roomName = "LobbyJoinLobbyGameCountLimitTest_TestRoom";

                // join lobby on master
                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient1.JoinLobby(lobbyName, lobbyType);
                masterClient1.WaitForEvent((byte)Events.EventCode.GameList);

                this.CreateRoomOnGameServer(masterClient1, roomName + Player1);

                // join lobby on master with second client
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient2.JoinLobby(lobbyName, lobbyType);
                masterClient2.WaitForEvent((byte)Events.EventCode.GameList);

                this.CreateRoomOnGameServer(masterClient2, roomName + Player2);

                // join third client to another lobby 
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);

                const int maxGamesCountInList = 1;
                masterClient3.JoinLobby(lobbyName, lobbyType, maxGamesCountInList);
                var ev = masterClient3.WaitForEvent((byte)Events.EventCode.GameList);

                var gameList = (Hashtable) ev[ParameterCode.GameList];

                Assert.AreEqual(maxGamesCountInList, gameList.Count);


            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3);
            }
        }

        [Test]
        public void InActiveInGameDoNotGetThisGameAsRandom_DefaultLobbyTest()
        {
            InActiveInGameDoNotGetThisGameAsRandomTestBody(AppLobbyType.Default);
        }

        [Test]
        public void InActiveInGameDoNotGetThisGameAsRandom_SQLLobbyTest()
        {
            InActiveInGameDoNotGetThisGameAsRandomTestBody(AppLobbyType.SqlLobby);
        }

        public void InActiveInGameDoNotGetThisGameAsRandomTestBody(AppLobbyType lobbyType)
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            const string LobbyName = "Lobby";
            try
            {
                var roomName = this.GenerateRandomizedRoomName("InActiveInGameDoNotGetThisGameAsRandomTest_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = 1000,
                    EmptyRoomLiveTime = 3000,
                    LobbyType = (byte)lobbyType,
                    LobbyName = LobbyName,
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address, masterClient1.UserId);
                masterClient1.JoinGame(joinRequest);

                masterClient1.LeaveGame(true);

                Thread.Sleep(200);

                this.ConnectAndAuthenticate(masterClient1, this.MasterAddress, masterClient1.UserId);

                masterClient1.JoinRandomGame(new Hashtable(), 0, new Hashtable(),
                    MatchmakingMode.FillRoom, LobbyName, lobbyType, null, ErrorCode.NoRandomMatchFound);

                Thread.Sleep(1200);

                var response = masterClient1.JoinRandomGame(new Hashtable(), 0, new Hashtable(),
                    MatchmakingMode.FillRoom, LobbyName, lobbyType, null);

                Assert.AreEqual(roomName, response.GameId);

                this.ConnectAndAuthenticate(masterClient1, response.Address, masterClient1.UserId);
                masterClient1.JoinRoom(roomName, null, 0, new RoomOptions(), false, true, ErrorCode.Ok);

                Thread.Sleep(200);

                masterClient1.LeaveGame(false);

                Thread.Sleep(200);

                this.ConnectAndAuthenticate(masterClient1, this.MasterAddress, masterClient1.UserId);

                masterClient1.JoinRandomGame(new Hashtable(), 0, new Hashtable(),
                    MatchmakingMode.FillRoom, LobbyName, lobbyType, null);

                Assert.AreEqual(roomName, response.GameId);
            }
            finally
            {
                DisposeClients(masterClient1);
            }
        }

        #endregion

        [Test]
        public void EmptyRoomLiveTime()
        {
            UnifiedTestClient gameClient = null;

            try
            {
                string gameId = this.GenerateRandomizedRoomName("EmptyRoomLiveTime");

                var createGameRequest = new CreateGameRequest
                {
                    GameId = gameId,
                    EmptyRoomLiveTime = 1000
                };

                gameClient = this.CreateMasterClientAndAuthenticate(Player1);
                var response = gameClient.CreateGame(createGameRequest);

                this.ConnectAndAuthenticate(gameClient, response.Address);
                gameClient.CreateGame(createGameRequest);

                // in order to give server some time to update data about game on master server
                Thread.Sleep(100);
                gameClient.Disconnect();
                this.ConnectAndAuthenticate(gameClient, response.Address);

                // Rejoin the game. The game should be still in the room cache
                gameClient.JoinGame(gameId);
                gameClient.LeaveGame();

                gameClient.Disconnect();
                this.ConnectAndAuthenticate(gameClient, response.Address);

                // Rejoin the game again. Second clients leave should not have set the emty room live time to zero.
                gameClient.JoinGame(gameId);
                gameClient.LeaveGame();

                gameClient.Disconnect();
                this.ConnectAndAuthenticate(gameClient, response.Address);
                Thread.Sleep(2500);
                // Rejoin the game. The game should not be in the room cache anymore
                gameClient.JoinGame(gameId, ErrorCode.GameDoesNotExist);
            }
            finally
            {
                DisposeClients(gameClient);
            }
        }

        [Test]
        public void CheckPluginMismatch()
        {
            if (!this.UsePlugins)
            {
                Assert.Ignore("test needs plugin support");
            }
            UnifiedTestClient masterClient = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName("CheckPluginMismatch_");
                var createRequest = new CreateGameRequest
                {
                    GameId = roomName,
                    Plugins = new[] { "WrongPlugin" },
                };
                masterClient = this.CreateMasterClientAndAuthenticate(Player1);
                var response = masterClient.CreateGame(createRequest);
                masterClient.Disconnect();

                this.ConnectAndAuthenticate(masterClient, response.Address, masterClient.UserId);
                masterClient.CreateGame(createRequest, 32751);// TODO replace numbers with constant after client lib update

                masterClient.Disconnect();
            }
            finally
            {
                DisposeClients(masterClient);
            }
        }

        [Test]
        public void RandomGameMaxPlayerTest()
        {
            UnifiedTestClient masterClient = null;
            UnifiedTestClient masterClient2 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName("RandomGameMaxPlayerTest_");
                var createRequest = new CreateGameRequest
                {
                    GameId = roomName,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 6}
                    }
                };

                masterClient = this.CreateMasterClientAndAuthenticate(Player1);
                var response = masterClient.CreateGame(createRequest);
                this.ConnectAndAuthenticate(masterClient, response.Address, Player1);
                masterClient.CreateGame(createRequest);
                Thread.Sleep(300);// wait while game is created on gameserver

                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient2.JoinRandomGame(null, 3, null, MatchmakingMode.FillRoom, null, AppLobbyType.Default, "", ErrorCode.NoRandomMatchFound);
                masterClient2.JoinRandomGame(null, 3, null, MatchmakingMode.RandomMatching, null, AppLobbyType.Default, "", ErrorCode.NoRandomMatchFound);
                masterClient2.JoinRandomGame(null, 3, null, MatchmakingMode.SerialMatching, null, AppLobbyType.Default, "", ErrorCode.NoRandomMatchFound);
                masterClient2.JoinRandomGame(null, 6, null, MatchmakingMode.FillRoom, null, AppLobbyType.Default, "");
                masterClient2.JoinRandomGame(null, 6, null, MatchmakingMode.SerialMatching, null, AppLobbyType.Default, "");
                masterClient2.JoinRandomGame(null, 6, null, MatchmakingMode.RandomMatching, null, AppLobbyType.Default, "");
            }
            finally
            {
                DisposeClients(masterClient);
                DisposeClients(masterClient2);
            }
        }

        [Test]
        public void GameStateTest()
        {
            if (!this.UsePlugins)
            {
                Assert.Ignore("test needs plugin support");
            }
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("this tests requires userId to be set");
            }

            UnifiedTestClient masterClient = null;
            UnifiedTestClient masterClient2 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName("GameStateTest_");
                var myRoomOptions = new RoomOptions
                {
                    PlayerTtl = int.MaxValue,
                    MaxPlayers = 4,
                    IsOpen = true,
                    IsVisible = true,
                    CustomRoomProperties = new Hashtable
                    {
                        {"prop1Key", "prop1Val"},
                        {"prop2Key", "prop2Val"},
                        {"lobby3Key", "lobby3Val"},
                        {"lobby4Key", "lobby4Val"},
                        {"map_name", "mymap"}
                    },
                    CustomRoomPropertiesForLobby = new string[] { "lobby3Key", "lobby4Key" },
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1)
                };

                var customProperties = new Hashtable { { "player_id", "12345" } };

                masterClient = this.CreateMasterClientAndAuthenticate(Player1);
                var response = masterClient.CreateGame(roomName);
                this.ConnectAndAuthenticate(masterClient, response.Address, Player1);

                masterClient.CreateRoom(roomName, myRoomOptions, TypedLobby.Default, customProperties, true, "SaveLoadStateTestPlugin");
                Thread.Sleep(300);// wait while game is created on gameserver

                // send messages to fill up cache
                FillEventsCache(masterClient);

                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);

                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                };
                var jgResponse = masterClient2.JoinGame(joinRequest);

                this.ConnectAndAuthenticate(masterClient2, jgResponse.Address, Player2);

                var customProperties2 = new Hashtable { { "player_id", "__12345" } };
                joinRequest.ActorProperties = customProperties2;
                var jgr = masterClient2.JoinGame(joinRequest);
                var gameProperties = jgr.GameProperties;

                Thread.Sleep(300);

                masterClient.Disconnect();
                masterClient2.Disconnect();

                Thread.Sleep(6000);

                this.ConnectAndAuthenticate(masterClient, this.MasterAddress);
                this.ConnectAndAuthenticate(masterClient2, this.MasterAddress);

                joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 6},
                    },
                    ActorNr = 2,
                    Plugins = new string[] { "SaveLoadStateTestPlugin" },
                    JoinMode = (byte)JoinMode.RejoinOnly
                };

                // we join second player first in order to get MasterClientId == 2
                jgResponse = masterClient2.JoinGame(joinRequest);
                this.ConnectAndAuthenticate(masterClient2, jgResponse.Address, Player2);

                jgResponse = masterClient2.JoinGame(joinRequest);

                gameProperties[GamePropertyKey.MasterClientId] = 2;
                Assert.AreEqual(gameProperties, jgResponse.GameProperties);

                Thread.Sleep(100);
                joinRequest.ActorNr = 1;
                jgResponse = masterClient.JoinGame(joinRequest);
                this.ConnectAndAuthenticate(masterClient, jgResponse.Address, Player1);

                jgResponse = masterClient.JoinGame(joinRequest);
                Assert.AreEqual(gameProperties, jgResponse.GameProperties);

                Thread.Sleep(10);
                var gapResponse = masterClient.GetActorsProperties();
                Assert.IsNotNull(gapResponse.ActorProperties);
                Assert.IsNotNull(gapResponse.ActorProperties[1]);
                Assert.IsNotNull(gapResponse.ActorProperties[2]);

                Assert.AreEqual(customProperties, gapResponse.ActorProperties[1]);
                Assert.AreEqual(customProperties2, gapResponse.ActorProperties[2]);

                Assert.IsNotNull(masterClient2.WaitForEvent(3, 1000));
            }
            finally
            {
                DisposeClients(masterClient);
                DisposeClients(masterClient2);
            }
        }

        #region MasterClientId tests

        [Test]
        public void MasterClientIdChangeJoinRejoinTest()
        {
            UnifiedTestClient client1 = null, client2 = null;
            var gameName = MethodBase.GetCurrentMethod().Name;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(gameName);
                var createRequest = new CreateGameRequest
                {
                    GameId = roomName,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 6},
                    },
                    PlayerTTL = 5000,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1)
                };

                client1 = this.CreateMasterClientAndAuthenticate(Player1);
                var response = client1.CreateGame(createRequest);
                this.ConnectAndAuthenticate(client1, response.Address, Player1);
                client1.CreateGame(createRequest);
                Thread.Sleep(300); // wait while game is created on gameserver

                client2 = this.CreateMasterClientAndAuthenticate(Player2);
                var joinGameResponse = this.ConnectClientToGame(client2, roomName);
                Assert.AreEqual(1, (int) joinGameResponse.GameProperties[GamePropertyKey.MasterClientId]);

                client1.LeaveGame(true);

                var ev = client2.WaitForEvent(EventCode.Leave);
                Assert.AreEqual(2, (int) ev[ParameterCode.MasterClientId]);

                client1.Disconnect();
                this.ConnectAndAuthenticate(client1, this.MasterAddress, client1.UserId, reuseToken: string.IsNullOrEmpty(this.Player1));
                joinGameResponse = this.ConnectClientToGame(client1, roomName, 1);
                Assert.AreEqual(2, (int) joinGameResponse.GameProperties[GamePropertyKey.MasterClientId]);

                client2.Disconnect();
                ev = client1.WaitForEvent(EventCode.Leave);
                Assert.AreEqual(1, (int) ev[ParameterCode.MasterClientId]);

                // rejoin
                this.ConnectAndAuthenticate(client2, this.MasterAddress, client2.UserId, reuseToken: string.IsNullOrEmpty(this.Player2));
                joinGameResponse = this.ConnectClientToGame(client2, roomName, 2);
                Assert.AreEqual(1, (int) joinGameResponse.GameProperties[GamePropertyKey.MasterClientId]);
            }
            finally
            {
                DisposeClients(client1);
                DisposeClients(client2);
            }
        }

        [Test]
        public void MasterClientIdCASUpdate()
        {
            UnifiedTestClient client1 = null, client2 = null, client3 = null;
            var gameName = MethodBase.GetCurrentMethod().Name;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(gameName);
                var createRequest = new CreateGameRequest
                {
                    GameId = roomName,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 6},
                    },
                    PlayerTTL = 5000,
                };

                client1 = this.CreateMasterClientAndAuthenticate(Player1);
                var response = client1.CreateGame(createRequest);
                this.ConnectAndAuthenticate(client1, response.Address, Player1);
                client1.CreateGame(createRequest);
                Thread.Sleep(300); // wait while game is created on gameserver

                client2 = this.CreateMasterClientAndAuthenticate(Player2);
                var joinGameResponse = this.ConnectClientToGame(client2, roomName);
                Assert.AreEqual(1, (int) joinGameResponse.GameProperties[GamePropertyKey.MasterClientId]);

                // we send wrong requests
                // we will try to set master client id for unexisting client
                var request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {
                            ParameterCode.Properties, new Hashtable {{GameParameter.MasterClientId, 7}}
                        },
                        {ParameterCode.ExpectedValues, new Hashtable {{GameParameter.MasterClientId, 1}}}
                    }
                };

                // error expected
                client1.SendRequestAndWaitForResponse(request, ErrorCode.InvalidOperationCode);

                // we will try to set master client id with wrong current value
                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Properties, new Hashtable {{GameParameter.MasterClientId, 2}}},
                        {ParameterCode.ExpectedValues, new Hashtable {{GameParameter.MasterClientId, 2}}}
                    }
                };

                client1.SendRequestAndWaitForResponse(request, ErrorCode.InvalidOperationCode);

                // now correct request
                request = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,
                    Parameters = new Dictionary<byte, object>
                    {
                        {
                            ParameterCode.Properties, new Hashtable {{GameParameter.MasterClientId, 2}}
                        },
                        {ParameterCode.ExpectedValues, new Hashtable {{GameParameter.MasterClientId, 1}}},
                    }
                };

                client1.SendRequestAndWaitForResponse(request);

                var ev = client2.WaitForEvent(EventCode.PropertiesChanged);
                var properties = (Hashtable) ev[ParameterCode.Properties];
                Assert.IsNotNull(properties);
                Assert.AreEqual(2, (int) properties[GamePropertyKey.MasterClientId]);

                client3 = this.CreateMasterClientAndAuthenticate("Player3");
                joinGameResponse = this.ConnectClientToGame(client3, roomName);
                Assert.AreEqual(2, (int) joinGameResponse.GameProperties[GamePropertyKey.MasterClientId]);

            }
            finally
            {
                DisposeClients(client1);
                DisposeClients(client2);
                DisposeClients(client3);
            }
        }

        [Test]
        public void RaiseEventMasterClientReciverNullRefTest()
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;
            UnifiedTestClient client3 = null;
            try
            {
                client1 = this.CreateMasterClientAndAuthenticate(this.Player1);
                client2 = this.CreateMasterClientAndAuthenticate(this.Player2);
                client3 = this.CreateMasterClientAndAuthenticate(this.Player3);

                var roomName = MethodBase.GetCurrentMethod().Name;
                var createRoomRequest = new CreateGameRequest
                {
                    GameId = roomName,
                    SuppressRoomEvents = true,
                };
                var createGameResponse = client1.CreateGame(createRoomRequest);

                this.ConnectAndAuthenticate(client1, createGameResponse.Address);

                client1.CreateGame(createRoomRequest);

                Thread.Sleep(100);

                var joinRoomResponse = client2.JoinGame(roomName);

                this.ConnectAndAuthenticate(client2, joinRoomResponse.Address);

                client2.JoinGame(roomName);

                this.ConnectAndAuthenticate(client3, joinRoomResponse.Address);
                client3.JoinGame(roomName);

                client1.Disconnect();

                Thread.Sleep(100);

                var request = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Code, (byte) 1},
                        {ParameterCode.ReceiverGroup, (byte)2},
                        {ParameterCode.Data, new Hashtable{{0, 1}}}
                    }
                };

                client2.EventQueueClear();
                client3.SendRequest(request);

                client2.WaitForEvent((byte)1);
            }
            finally
            {
                DisposeClients(client1, client2, client3);
            }
        }

        #endregion

        #region Publishing of UserId Tests

        [Test]
        public void PublishUserIdTest()
        {
            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;
            UnifiedTestClient masterClient4 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, roomName},
                        {ParameterCode.JoinMode, JoinModes.CreateIfNotExists},
                        {ParameterCode.CheckUserOnJoin, !string.IsNullOrEmpty(this.Player1)},
                        {(byte) ParameterKey.PublishUserId, true},
                        {ParameterCode.Broadcast, true},
                    }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);
                masterClient4 = this.CreateMasterClientAndAuthenticate(Player3 != null ? "Player4" : null);

                var joinResponse = masterClient1.SendRequestAndWaitForResponse(joinRequest);
                var address = (string) joinResponse[ParameterCode.Address];

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, address);
                masterClient1.SendRequestAndWaitForResponse(joinRequest);

                //var actorProperties = (Hashtable)joinResponse[ParameterCode.PlayerProperties];
                var joinEvent = masterClient1.WaitForEvent(EventCode.Join);

                var actorProperties = (Hashtable) joinEvent[ParameterCode.PlayerProperties];
                var userId = (string) actorProperties[(byte) ActorParameter.UserId];
                if (string.IsNullOrEmpty(this.Player1))
                {
                    Assert.IsFalse(string.IsNullOrEmpty(userId));
                }
                else
                {
                    Assert.AreEqual(this.Player1, userId);
                }


                joinResponse = masterClient2.SendRequestAndWaitForResponse(joinRequest);
                address = (string) joinResponse[ParameterCode.Address];

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient2, address);
                joinResponse = masterClient2.SendRequestAndWaitForResponse(joinRequest);

                actorProperties = (Hashtable) joinResponse[ParameterCode.PlayerProperties];
                var actor0Properties = (Hashtable) actorProperties[1];
                userId = (string) actor0Properties[(byte) ActorParameter.UserId];
                if (string.IsNullOrEmpty(this.Player1))
                {
                    Assert.IsFalse(string.IsNullOrEmpty(userId));
                }
                else
                {
                    Assert.AreEqual(this.Player1, userId);
                }

                joinEvent = masterClient1.WaitForEvent(EventCode.Join);

                actorProperties = (Hashtable) joinEvent[ParameterCode.PlayerProperties];
                userId = (string) actorProperties[(byte) ActorParameter.UserId];

                if (string.IsNullOrEmpty(this.Player2))
                {
                    Assert.IsFalse(string.IsNullOrEmpty(userId));
                }
                else
                {
                    Assert.AreEqual(this.Player2, userId);
                }
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3, masterClient4);
            }
        }

        [Test]
        public void NotPublishUserIdTest()
        {
            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;
            UnifiedTestClient masterClient4 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new OperationRequest
                {
                    OperationCode = OperationCode.JoinGame,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.RoomName, roomName},
                        {ParameterCode.JoinMode, JoinModes.CreateIfNotExists},
                        {ParameterCode.CheckUserOnJoin, !string.IsNullOrEmpty(this.Player1)},
                        {ParameterCode.Broadcast, true},
                    }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);
                masterClient4 = this.CreateMasterClientAndAuthenticate(Player3 != null ? "Player4" : null);

                var joinResponse = masterClient1.SendRequestAndWaitForResponse(joinRequest);
                var address = (string) joinResponse[ParameterCode.Address];

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, address);
                masterClient1.SendRequestAndWaitForResponse(joinRequest);

                //var actorProperties = (Hashtable)joinResponse[ParameterCode.PlayerProperties];
                var joinEvent = masterClient1.WaitForEvent(EventCode.Join);

                var actorProperties = (Hashtable) joinEvent[ParameterCode.PlayerProperties];
                Assert.IsNull(actorProperties);

                joinResponse = masterClient2.SendRequestAndWaitForResponse(joinRequest);
                address = (string) joinResponse[ParameterCode.Address];

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient2, address);
                joinResponse = masterClient2.SendRequestAndWaitForResponse(joinRequest);

                actorProperties = (Hashtable) joinResponse[ParameterCode.PlayerProperties];
                Assert.IsNull(actorProperties);

                joinEvent = masterClient1.WaitForEvent(EventCode.Join);

                actorProperties = (Hashtable) joinEvent[ParameterCode.PlayerProperties];
                Assert.IsNull(actorProperties);
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3, masterClient4);
            }
        }


        #endregion

        #region Join mode tests

        [Test]
        public void FastReJoinTest()
        {
            //if (string.IsNullOrEmpty(this.Player1))
            //{
            //    Assert.Ignore("This test requires userId to be set");
            //}

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient12 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName("FastReJoinTest_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = 1000,
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address, masterClient1.UserId);
                masterClient1.JoinGame(joinRequest);

                // client 2: try to join a game which exists and is created on the game server
                var joinResponse2 = masterClient2.JoinGame(joinRequest);
                Assert.AreEqual(joinResponse1.Address, joinResponse2.Address);

                this.ConnectAndAuthenticate(masterClient2, joinResponse2.Address, masterClient2.UserId);
                masterClient2.JoinGame(joinRequest);

                //              masterClient12 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient12 = (UnifiedTestClient) this.CreateTestClient();
                masterClient12.UserId = Player1;
                masterClient12.Token = masterClient1.Token;
                this.ConnectAndAuthenticate(masterClient12, joinResponse1.Address, masterClient12.UserId);
                joinRequest.ActorNr = 1;
                masterClient12.JoinGame(joinRequest);

                Thread.Sleep(1200);
                Assert.IsFalse(masterClient1.Connected, "masterClient1 is expected to be disconnected by server.");
            }
            finally
            {
                DisposeClients(masterClient1);
                DisposeClients(masterClient2);
                DisposeClients(masterClient12);
            }
        }

        [Test]
        public void FastJoinOnMasterWithSameNameTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient12 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName("FastReJoinTest_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = 1000,
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address, masterClient1.UserId);
                masterClient1.JoinGame(joinRequest);

                // client 2: try to join a game which exists and is created on the game server
                var joinResponse2 = masterClient2.JoinGame(joinRequest);
                Assert.AreEqual(joinResponse1.Address, joinResponse2.Address);

                this.ConnectAndAuthenticate(masterClient2, joinResponse2.Address, masterClient2.UserId);
                masterClient2.JoinGame(joinRequest);

                masterClient12 = this.CreateMasterClientAndAuthenticate(Player1);

                var joinRandomGame = new JoinRandomGameRequest();
                masterClient12.JoinRandomGame(joinRandomGame, ErrorCode.NoRandomMatchFound);
            }
            finally
            {
                DisposeClients(masterClient1);
                DisposeClients(masterClient2);
                DisposeClients(masterClient12);
            }
        }

        [Test]
        public void ReJoinFullRoomTest()
        {
            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;
            UnifiedTestClient masterClient4 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName("ReJoinFullRoomTest_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 3}
                    }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);
                masterClient4 = this.CreateMasterClientAndAuthenticate(Player3 != null ? "Player4" : null);

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address, masterClient1.UserId);
                masterClient1.JoinGame(joinRequest);

                var joinEvent = masterClient1.WaitForEvent(EventCode.Join);

                // client 2: try to join a game which exists and is created on the game server
                var joinResponse2 = masterClient2.JoinGame(joinRequest);
                Assert.AreEqual(joinResponse1.Address, joinResponse2.Address);

                this.ConnectAndAuthenticate(masterClient2, joinResponse2.Address, masterClient2.UserId);
                masterClient2.JoinGame(joinRequest);

                masterClient1.LeaveGame(true);

                // client 3: try to join a game which exists and is created on the game server
                joinResponse2 = masterClient3.JoinGame(joinRequest);
                Assert.AreEqual(joinResponse1.Address, joinResponse2.Address);

                this.ConnectAndAuthenticate(masterClient3, joinResponse2.Address, masterClient3.UserId);
                masterClient3.JoinGame(joinRequest);

                // client 4: try to join a game which exists and is created on the game server
                masterClient4.JoinGame(joinRequest, ErrorCode.GameFull);

                joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.RejoinOnly,
                    ActorNr = this.Player1 == null ? (int) joinEvent[ParameterCode.ActorNr] : -1,
                };

                Thread.Sleep(200);

                this.ConnectAndAuthenticate(masterClient1, this.MasterAddress, masterClient1.UserId,
                    reuseToken: string.IsNullOrEmpty(this.Player1));
                joinResponse1 = masterClient1.JoinGame(joinRequest);

                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address, masterClient1.UserId);
                masterClient1.JoinGame(joinRequest);

                // leave and wait while client will be disconnected from game
                masterClient1.LeaveGame(true);

                Thread.Sleep(200);

                // change join mode and rejoin again. should succeed
                joinRequest.JoinMode = JoinModes.RejoinOrJoin;

                this.ConnectAndAuthenticate(masterClient1, this.MasterAddress, masterClient1.UserId,
                    reuseToken: string.IsNullOrEmpty(this.Player1));
                joinResponse1 = masterClient1.JoinGame(joinRequest);

                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address, masterClient1.UserId);
                masterClient1.JoinGame(joinRequest);
            }
            finally
            {
                DisposeClients(masterClient1);
                DisposeClients(masterClient2);
                DisposeClients(masterClient3);
                DisposeClients(masterClient4);
            }
        }

        [Test]
        public void ReJoinFullRoomTestJapanVersion()
        {
            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;
            UnifiedTestClient masterClient4 = null;
            UnifiedTestClient masterClient5 = null;

            const int TestPlayerTtl = 5000;
            try
            {
                var roomName = this.GenerateRandomizedRoomName("ReJoinFullRoomTestJapan_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = TestPlayerTtl,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 4}
                    }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);
                masterClient4 = this.CreateMasterClientAndAuthenticate(Player3 != null ? "Player4" : null);
                masterClient5 = this.CreateMasterClientAndAuthenticate(Player3 != null ? "Player5" : null);

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

                masterClient1.WaitForEvent(EventCode.Join);

                // client 2: try to join a game which exists and is created on the game server
                var joinResponse2 = masterClient2.JoinGame(joinRequest);
                Assert.AreEqual(joinResponse1.Address, joinResponse2.Address);

                this.ConnectAndAuthenticate(masterClient2, joinResponse2.Address);
                masterClient2.JoinGame(joinRequest);

                // client 3: try to join a game which exists and is created on the game server
                joinResponse2 = masterClient3.JoinGame(joinRequest);
                Assert.AreEqual(joinResponse1.Address, joinResponse2.Address);

                this.ConnectAndAuthenticate(masterClient3, joinResponse2.Address);
                masterClient3.JoinGame(joinRequest);

                // client 4: try to join a game which exists and is created on the game server
                masterClient4.JoinGame(joinRequest);
                this.ConnectAndAuthenticate(masterClient4, joinResponse2.Address);
                masterClient4.JoinGame(joinRequest);

                masterClient5.JoinGame(joinRequest, ErrorCode.GameFull);

                Thread.Sleep(1200);

                masterClient1.Disconnect();

                masterClient4.WaitForEvent(EventCode.Leave, 200);
                // wait little longer than TestPlayerTtl to get finanl leave
                masterClient4.WaitForEvent(EventCode.Leave, TestPlayerTtl + 200);

                Thread.Sleep(200);
                joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.CreateIfNotExists,
                };

                this.ConnectAndAuthenticate(masterClient1, this.MasterAddress, masterClient1.UserId,
                    reuseToken: string.IsNullOrEmpty(this.Player1));
                joinResponse1 = masterClient1.JoinGame(joinRequest);
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address, masterClient1.UserId);
                masterClient1.JoinGame(joinRequest);
            }
            finally
            {
                DisposeClients(masterClient1);
                DisposeClients(masterClient2);
                DisposeClients(masterClient3);
                DisposeClients(masterClient4);
                DisposeClients(masterClient5);
            }
        }

        [Test]
        public void RejoinOrJoinFailsIfGameNotExistTest()
        {
            UnifiedTestClient masterClient1 = null;

            try
            {
                var roomName = MethodBase.GetCurrentMethod().Name;
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.RejoinOrJoin,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);

                var createResponse = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, createResponse.Address);

                masterClient1.JoinGame(joinRequest);
            }
            finally
            {
                DisposeClients(masterClient1);
            }
        }

        [Test]
        public void RejoinOnlyFailsIfGameNotExistTest()
        {
            UnifiedTestClient masterClient1 = null;

            try
            {
                var roomName = MethodBase.GetCurrentMethod().Name;
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.RejoinOnly,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);

                var createResponse = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, createResponse.Address);

                masterClient1.JoinGame(joinRequest, ErrorCode.GameDoesNotExist);
            }
            finally
            {
                DisposeClients(masterClient1);
            }
        }

        [Test]
        public void JoinOnlyFailsIfGameNotExistTest()
        {
            UnifiedTestClient masterClient1 = null;

            try
            {
                var roomName = MethodBase.GetCurrentMethod().Name;
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.JoinOnly,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);

                masterClient1.JoinGame(joinRequest, ErrorCode.GameDoesNotExist);
            }
            finally
            {
                DisposeClients(masterClient1);
            }
        }

        [Test]
        public void RejoinExceedsMaxPlayerTest()
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;
            try
            {
                client1 = this.CreateMasterClientAndAuthenticate(Player1);
                client2 = this.CreateMasterClientAndAuthenticate(Player2);

                var GameName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);

                var request = new CreateGameRequest
                {
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    GameId = GameName,
                    GameProperties = new Hashtable
                    {
                        {GameParameter.MaxPlayers, 1}
                    }
                };

                var response = client1.CreateGame(request);

                this.ConnectAndAuthenticate(client1, response.Address);

                client1.CreateGame(request);


                var joinRequest = new JoinGameRequest
                {
                    GameId = GameName,
                    JoinMode = (byte)JoinMode.JoinOrRejoin,
                };

                var joinResponse = client2.JoinGame(joinRequest);
                this.ConnectAndAuthenticate(client2, joinResponse.Address);

                client2.JoinGame(joinRequest, ErrorCode.GameFull);

                this.ConnectAndAuthenticate(client2, this.MasterAddress);

                joinRequest.JoinMode = (byte)JoinMode.RejoinOnly;
                joinResponse = client2.JoinGame(joinRequest);
                this.ConnectAndAuthenticate(client2, joinResponse.Address);
                client2.JoinGame(joinRequest, (short)Photon.Common.ErrorCode.JoinFailedWithRejoinerNotFound);
            }
            finally
            {
                DisposeClients(client1, client2);
            }
        }

        [Test]
        public void ReJoinModeTest()
        {
            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName("ReJoinModeTest_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = 1000,
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address, masterClient1.UserId);
                masterClient1.JoinGame(joinRequest);

                var joinEvent = masterClient1.WaitForEvent(EventCode.Join);

                // client 2: try to join a game which exists and is created on the game server
                var joinResponse2 = masterClient2.JoinGame(joinRequest);
                Assert.AreEqual(joinResponse1.Address, joinResponse2.Address);

                this.ConnectAndAuthenticate(masterClient2, joinResponse2.Address, masterClient2.UserId);
                masterClient2.JoinGame(joinRequest);

                masterClient1.LeaveGame(true);

                Thread.Sleep(200);

                joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.RejoinOnly,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = 1000,
                    ActorNr = (int)joinEvent[ParameterCode.ActorNr],
                };

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address, masterClient1.UserId);
                masterClient1.JoinGame(joinRequest);

                // leave and wait while client will be disconnected from game
                masterClient1.LeaveGame(true);
                Thread.Sleep(1200);

                // and try to rejoin this game again. should fail
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address, masterClient1.UserId);
                masterClient1.JoinGame(joinRequest, (short)Photon.Common.ErrorCode.JoinFailedWithRejoinerNotFound);

                // change join mode and rejoin again. should succeed
                joinRequest.JoinMode = JoinModes.RejoinOrJoin;

                joinRequest.ActorNr = 0;
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address, masterClient1.UserId);
                masterClient1.JoinGame(joinRequest);
            }
            finally
            {
                DisposeClients(masterClient1);
                DisposeClients(masterClient2);
            }
        }

        [Test]
        public void ReJoinModeTest2()
        {
            this.WaitTimeout = 10000;
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;
            try
            {
                client1 = this.CreateMasterClientAndAuthenticate(Player1);
                client2 = this.CreateMasterClientAndAuthenticate(Player2);

                var GameName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);

                var request = new CreateGameRequest
                {
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    GameId = GameName,
                    GameProperties = new Hashtable
                    {
                        {GameParameter.MaxPlayers, 2}
                    }
                };

                var response = client1.CreateGame(request);

                this.ConnectAndAuthenticate(client1, response.Address);

                client1.CreateGame(request);


                var joinRequest = new JoinGameRequest
                {
                    GameId = GameName,
                    JoinMode = (byte)JoinMode.RejoinOnly,
                };

                var joinResponse = client2.JoinGame(joinRequest);
                this.ConnectAndAuthenticate(client2, joinResponse.Address);

                client2.JoinGame(joinRequest, (short)Photon.Common.ErrorCode.JoinFailedWithRejoinerNotFound);

                this.ConnectAndAuthenticate(client2, this.MasterAddress);
                joinResponse = client2.JoinGame(joinRequest);
                this.ConnectAndAuthenticate(client2, joinResponse.Address);

                joinRequest.JoinMode = (byte)JoinMode.JoinOrRejoin;
                client2.JoinGame(joinRequest);
            }
            finally
            {
                DisposeClients(client1, client2);
            }
        }

        [TestCase(JoinMode.Default)]
        [TestCase(JoinMode.CreateIfNotExists)]
        [TestCase(JoinMode.JoinOrRejoin)]
        [TestCase(JoinMode.RejoinOnly)]
        public void JoinDuringCloseTest(JoinMode joinMode)
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;
            try
            {
                client1 = this.CreateMasterClientAndAuthenticate(Player1);
                client2 = this.CreateMasterClientAndAuthenticate(Player2);

                var GameName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);

                var request = new CreateGameRequest
                {
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    GameId = GameName,
                    GameProperties = new Hashtable
                    {
                        {GameParameter.MaxPlayers, 2}
                    },
                    Plugins = new string[] { "LongOnClosePlugin"}
                };

                var response = client1.CreateGame(request);

                this.ConnectAndAuthenticate(client1, response.Address);

                client1.CreateGame(request);

                Thread.Sleep(100);

                client1.LeaveGame();

                var joinRequest = new JoinGameRequest
                {
                    GameId = GameName,
                    JoinMode = (byte)joinMode,
                };

                var joinResponse = client2.JoinGame(joinRequest);
                this.ConnectAndAuthenticate(client2, joinResponse.Address);

                var errorCode = joinMode == JoinMode.CreateIfNotExists || joinMode == JoinMode.JoinOrRejoin ? 0 : ErrorCode.GameDoesNotExist;
                client2.JoinGame(joinRequest, (short)errorCode);
            }
            finally
            {
                DisposeClients(client1, client2);
            }
        }

        [TestCase(JoinMode.Default)]
        [TestCase(JoinMode.CreateIfNotExists)]
        [TestCase(JoinMode.JoinOrRejoin)]
        [TestCase(JoinMode.RejoinOnly)]
        public void JoinDuringCloseWithPersistenceTest(JoinMode joinMode)
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;
            try
            {
                client1 = this.CreateMasterClientAndAuthenticate(Player1);
                client2 = this.CreateMasterClientAndAuthenticate(Player2);

                var GameName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);

                var request = new CreateGameRequest
                {
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    GameId = GameName,
                    GameProperties = new Hashtable
                    {
                        {GameParameter.MaxPlayers, 2}
                    },
                    Plugins = new string[] { "LongOnClosePluginWithPersistence" }
                };

                var response = client1.CreateGame(request);

                this.ConnectAndAuthenticate(client1, response.Address);

                client1.CreateGame(request);

                Thread.Sleep(100);

                client1.LeaveGame();

                var joinRequest = new JoinGameRequest
                {
                    GameId = GameName,
                    JoinMode = (byte)joinMode,
                };

                var joinResponse = client2.JoinGame(joinRequest);
                this.ConnectAndAuthenticate(client2, joinResponse.Address);

                var errorCode = joinMode == JoinMode.RejoinOnly ? 32748 : 0;
                client2.JoinGame(joinRequest, (short)errorCode);
            }
            finally
            {
                DisposeClients(client1, client2);
            }
        }

        [Test]
        public void ForceRejoinTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("this tests requires userId to be set");
            }

            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;
            UnifiedTestClient client3 = null;
            try
            {
                client1 = this.CreateMasterClientAndAuthenticate(Player1);
                client2 = this.CreateMasterClientAndAuthenticate(Player2);
                client3 = this.CreateMasterClientAndAuthenticate(Player2);

                var GameName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);

                var request = new CreateGameRequest
                {
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    GameId = GameName,
                    PlayerTTL = 100000,
                    GameProperties = new Hashtable
                    {
                        {GameParameter.MaxPlayers, 2}
                    }
                };

                var response = client1.CreateGame(request);

                this.ConnectAndAuthenticate(client1, response.Address);

                client1.CreateGame(request);

                var joinRequest = new JoinGameRequest
                {
                    GameId = GameName,
                    JoinMode = (byte)JoinMode.JoinOrRejoin,
                };

                var joinResponse = client2.JoinGame(joinRequest);
                this.ConnectAndAuthenticate(client2, joinResponse.Address);

                client2.JoinGame(joinRequest);

                joinRequest = new JoinGameRequest
                {
                    GameId = GameName,
                    JoinMode = (byte)JoinMode.RejoinOnly,
                    ForceRejoin = true,
                    ActorNr = 2,
                };

                joinResponse = client3.JoinGame(joinRequest);
                this.ConnectAndAuthenticate(client3, joinResponse.Address);

                client3.JoinGame(joinRequest);

                Thread.Sleep(100);
                Assert.IsFalse(client2.Connected);
            }
            finally
            {
                DisposeClients(client1, client2, client3);
            }
        }

        [Test]
        [Ignore("just instead of writing test client")]
        public void CacheClearTest()
        {
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;
            try
            {
                client1 = this.CreateMasterClientAndAuthenticate(Player1);
                client2 = this.CreateMasterClientAndAuthenticate(Player2);

                var GameName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);

                var request = new CreateGameRequest
                {
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    GameId = GameName,
                    GameProperties = new Hashtable
                    {
                        {GameParameter.MaxPlayers, 2}
                    }
                };

                var response = client1.CreateGame(request);

                this.ConnectAndAuthenticate(client1, response.Address);

                client1.CreateGame(request);

                Thread.Sleep(100);

                var joinRequest = new JoinGameRequest
                {
                    GameId = GameName,
                    JoinMode = (byte)JoinMode.RejoinOnly,
                };

                var joinResponse = client2.JoinGame(joinRequest);
                this.ConnectAndAuthenticate(client2, joinResponse.Address);

                client2.JoinGame(joinRequest, ErrorCode.InternalServerError);
                

                this.ConnectAndAuthenticate(client2, joinResponse.Address);
                joinRequest.JoinMode = (byte)JoinMode.JoinOrRejoin;
                client2.JoinGame(joinRequest);

                Thread.Sleep(100);
                client1.LeaveGame();
                client2.LeaveGame();
            }
            finally
            {
                DisposeClients(client1, client2);
            }
            Thread.Sleep(3180000);
        }

        #endregion

        #region Slot Reservation Tests

        [Test]
        public void Slots_SimpleSlotReservationTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;
            UnifiedTestClient masterClient4 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 3}
                    },
                    AddUsers = new []{ this.Player2, this.Player3}
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);
                masterClient4 = this.CreateMasterClientAndAuthenticate("Player4");

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

                masterClient1.WaitForEvent(EventCode.Join);

                Thread.Sleep(100);
                var lobbyStatsResponse = masterClient4.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);

                var joinRequest2 = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.JoinOnly,
                    CheckUserOnJoin = true,
                };

                joinResponse1 = masterClient2.JoinGame(joinRequest2);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient2, joinResponse1.Address);
                masterClient2.JoinGame(joinRequest2);

                masterClient4.JoinGame(joinRequest2, ErrorCode.GameFull);

                joinRequest2.JoinMode = Photon.Hive.Operations.JoinModes.RejoinOrJoin;
                this.ConnectAndAuthenticate(masterClient4, this.MasterAddress);
                joinResponse1 = masterClient4.JoinGame(joinRequest2);
                this.ConnectAndAuthenticate(masterClient4, joinResponse1.Address);
                masterClient4.JoinGame(joinRequest2, ErrorCode.GameFull);

                joinRequest2.JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists;
                this.ConnectAndAuthenticate(masterClient4, this.MasterAddress);
                masterClient4.JoinGame(joinRequest2, ErrorCode.GameFull);

                joinRequest2.JoinMode = Photon.Hive.Operations.JoinModes.JoinOnly;
                masterClient4.JoinGame(joinRequest2, ErrorCode.GameFull);

                this.ConnectAndAuthenticate(masterClient4, this.MasterAddress);
                masterClient4.JoinRandomGame(new JoinRandomGameRequest(), ErrorCode.NoRandomMatchFound);
            }
            finally 
            {
                DisposeClients(masterClient1, masterClient2, masterClient3, masterClient4);
            }
        }

        [Test]
        public void Slots_CreateGameInvalidSlotsCountTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 2}
                    },
                    AddUsers = new[] { this.Player2, this.Player3 }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);

                masterClient1.JoinGame(joinRequest, ErrorCode.InvalidOperation);
            }
            finally
            {
                DisposeClients(masterClient1);
            }
        }

        [Test]
        public void Slots_CreateGameAndJoinNoMaxUserLimitTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    AddUsers = new[] { this.Player2, this.Player3 }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate("Player7");

                var response = masterClient1.JoinGame(joinRequest);

                this.ConnectAndAuthenticate(masterClient1, response.Address);
                masterClient1.JoinGame(joinRequest);

                joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    AddUsers = new[] { "Player4", "Player5" }
                };

                response = masterClient2.JoinGame(joinRequest);

                this.ConnectAndAuthenticate(masterClient2, response.Address);
                masterClient2.JoinGame(joinRequest);

            }
            finally
            {
                DisposeClients(masterClient1, masterClient2);
            }
        }

        [Test]
        public void Slots_EmptySlotNameTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = true,
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 2}
                    },
                    AddUsers = new[] { string.Empty }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);

                masterClient1.JoinGame(joinRequest, ErrorCode.InvalidOperation);
            }
            finally
            {
                DisposeClients(masterClient1);
            }
        }

        [Test]
        public void Slots_LobbyStatsTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 3}
                    },
                    AddUsers = new[] { this.Player2, this.Player3 }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

                masterClient1.WaitForEvent(EventCode.Join);

                Thread.Sleep(100);
                var lobbyStatsResponse = masterClient3.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);

                var joinRequest2 = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.JoinOnly,
                    CheckUserOnJoin = true,
                };

                joinResponse1 = masterClient2.JoinGame(joinRequest2);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient2, joinResponse1.Address);
                masterClient2.JoinGame(joinRequest2);

                Thread.Sleep(100);
                lobbyStatsResponse = masterClient3.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3);
            }
        }

        [Test]
        public void Slots_SlotReservationOnJoinTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 3}
                    },
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

                masterClient1.WaitForEvent(EventCode.Join);

                Thread.Sleep(100);
                var lobbyStatsResponse = masterClient3.GetLobbyStats(null, null);
                Assert.AreEqual(1, lobbyStatsResponse.PeerCount[0]);

                var joinRequest2 = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = true,
                    AddUsers = new[] { this.Player3 }
                };

                joinResponse1 = masterClient2.JoinGame(joinRequest2);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient2, joinResponse1.Address);
                masterClient2.JoinGame(joinRequest2);

                masterClient1.WaitForEvent(EventCode.PropertiesChanged);

                Thread.Sleep(100);
                lobbyStatsResponse = masterClient3.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3);
            }
        }

        [Test]
        public void Slots_TooManySlotsOnJoinTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 3}
                    },
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

                masterClient1.WaitForEvent(EventCode.Join);

                Thread.Sleep(100);
                var lobbyStatsResponse = masterClient3.GetLobbyStats(null, null);
                Assert.AreEqual(1, lobbyStatsResponse.PeerCount[0]);

                var joinRequest2 = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = true,
                    AddUsers = new[] { this.Player3, "Player4", "Player5" }
                };

                masterClient2.JoinGame(joinRequest2, ErrorCode.InternalServerError);


                Thread.Sleep(100);
                lobbyStatsResponse = masterClient3.GetLobbyStats(null, null);
                Assert.AreEqual(1, lobbyStatsResponse.PeerCount[0]);
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3);
            }
        }

        [Test]
        public void Slots_DifferentJoinModeTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 3}
                    },
                    AddUsers = new[] { this.Player2, this.Player3 }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

                masterClient1.WaitForEvent(EventCode.Join);

                var joinRequest2 = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = true,
                };

                Thread.Sleep(100);
                masterClient2.JoinGame(joinRequest2);

                joinRequest2.JoinMode = Photon.Hive.Operations.JoinModes.RejoinOrJoin;

                joinResponse1 = masterClient2.JoinGame(joinRequest2);

                this.ConnectAndAuthenticate(masterClient2, joinResponse1.Address);
                masterClient2.JoinGame(joinRequest2, ErrorCode.OperationNotAllowedInCurrentState);

                joinRequest2.JoinMode = Photon.Hive.Operations.JoinModes.RejoinOnly;

                joinResponse1 = masterClient3.JoinGame(joinRequest2);

                this.ConnectAndAuthenticate(masterClient3, joinResponse1.Address);
                masterClient3.JoinGame(joinRequest2, (short)Photon.Common.ErrorCode.JoinFailedWithRejoinerNotFound);

            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3);
            }
        }

        [Test]
        public void Slots_RepeatingNamesInDifferentRequestsTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;
            UnifiedTestClient masterClient4 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 6}
                    },
                    AddUsers = new[] { this.Player2, this.Player3 }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);
                masterClient4 = this.CreateMasterClientAndAuthenticate(Player3 != null ? "Player4" : null);

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

                masterClient1.WaitForEvent(EventCode.Join);

                Thread.Sleep(100);
                var lobbyStatsResponse = masterClient4.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);

                var joinRequest2 = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.JoinOnly,
                    CheckUserOnJoin = true,
                    AddUsers = new []{this.Player3, "Player4"}
                };

                joinResponse1 = masterClient2.JoinGame(joinRequest2);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient2, joinResponse1.Address);
                masterClient2.JoinGame(joinRequest2);

                Thread.Sleep(100);
                lobbyStatsResponse = masterClient4.GetLobbyStats(null, null);
                Assert.AreEqual(4, lobbyStatsResponse.PeerCount[0]);
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3, masterClient4);
            }
        }

        [Test]
        public void Slots_SetPropertiesTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 4}
                    },

                    AddUsers = new []{ this.Player2, this.Player3, "Player4"}
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

                masterClient1.WaitForEvent(EventCode.Join);

                Thread.Sleep(100);
                var lobbyStatsResponse = masterClient3.GetLobbyStats(null, null);
                Assert.AreEqual(4, lobbyStatsResponse.PeerCount[0]);

                joinRequest.JoinMode = JoinModes.JoinOnly;
                joinRequest.AddUsers = null;
                masterClient2.JoinGame(joinRequest);

                this.ConnectAndAuthenticate(masterClient2, joinResponse1.Address);
                masterClient2.JoinGame(joinRequest);

                var setPropertiesRequest = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,

                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ExpectedValues, new Hashtable{{(byte) GameParameter.ExpectedUsers, new[] {this.Player2, this.Player3, "Player4"}}}},
                        {ParameterCode.Properties, new Hashtable{{(byte) GameParameter.ExpectedUsers, new [] {this.Player2}}}}
                    }
                };

                Thread.Sleep(10);
                masterClient2.SendRequestAndWaitForResponse(setPropertiesRequest);

                masterClient1.WaitForEvent(EventCode.PropertiesChanged);

                Thread.Sleep(100);
                lobbyStatsResponse = masterClient3.GetLobbyStats(null, null);
                Assert.AreEqual(2, lobbyStatsResponse.PeerCount[0]);
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3);
            }
        }

        [Test]
        public void Slots_SetPropertiesFailedCASTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 3}
                    },

                    AddUsers = new[] { this.Player2, this.Player3 }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

                masterClient1.WaitForEvent(EventCode.Join);

                Thread.Sleep(100);
                var lobbyStatsResponse = masterClient3.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);

                joinRequest.JoinMode = JoinModes.JoinOnly;
                joinRequest.AddUsers = null;
                masterClient2.JoinGame(joinRequest);

                this.ConnectAndAuthenticate(masterClient2, joinResponse1.Address);
                masterClient2.JoinGame(joinRequest);

                var setPropertiesRequest = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,

                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ExpectedValues, new Hashtable{{(byte) GameParameter.ExpectedUsers, new[] {this.Player2}}}},
                        {ParameterCode.Properties, new Hashtable{{(byte) GameParameter.ExpectedUsers, new [] {this.Player2}}}}
                    }
                };

                Thread.Sleep(10);
                masterClient2.SendRequestAndWaitForResponse(setPropertiesRequest, ErrorCode.InvalidOperation);

                EventData ev;
                Assert.IsFalse(masterClient1.TryWaitForEvent(EventCode.PropertiesChanged, 1000, out ev));

                Thread.Sleep(100);
                lobbyStatsResponse = masterClient3.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3);
            }
        }

        [Test]
        public void Slots_SetPropertiesCheckFailTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 3}
                    },

                    AddUsers = new[] { this.Player2, this.Player3 }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

                masterClient1.WaitForEvent(EventCode.Join);

                Thread.Sleep(100);
                var lobbyStatsResponse = masterClient3.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);

                joinRequest.JoinMode = JoinModes.JoinOnly;
                joinRequest.AddUsers = null;
                masterClient2.JoinGame(joinRequest);

                this.ConnectAndAuthenticate(masterClient2, joinResponse1.Address);
                masterClient2.JoinGame(joinRequest);

                var setPropertiesRequest = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,

                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ExpectedValues, new Hashtable{{(byte) GameParameter.ExpectedUsers, new[] {this.Player2, this.Player3}}}},
                        {ParameterCode.Properties, new Hashtable{{(byte) GameParameter.ExpectedUsers, new [] {this.Player2, this.Player3, "Player4"}}}}
                    }
                };

                Thread.Sleep(10);
                masterClient2.SendRequestAndWaitForResponse(setPropertiesRequest, ErrorCode.InvalidOperation);

                EventData ev;
                Assert.IsFalse(masterClient1.TryWaitForEvent(EventCode.PropertiesChanged, 1000, out ev));

                Thread.Sleep(100);
                lobbyStatsResponse = masterClient3.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3);
            }
        }

        // we check that Slots are prefered souces of expected useres
        [Test]
        public void Slots_CreateGameUsingBothTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;
            UnifiedTestClient masterClient4 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 3},
                        {GameParameter.ExpectedUsers, new[] { this.Player2, this.Player3 }}
                    },
                    AddUsers = new[] { this.Player2 }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);
                masterClient4 = this.CreateMasterClientAndAuthenticate("Player4");

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

                masterClient1.WaitForEvent(EventCode.Join);

                Thread.Sleep(100);
                var lobbyStatsResponse = masterClient4.GetLobbyStats(null, null);
                Assert.AreEqual(2, lobbyStatsResponse.PeerCount[0]);

                var joinRequest2 = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.JoinOnly,
                    CheckUserOnJoin = true,
                };

                joinResponse1 = masterClient2.JoinGame(joinRequest2);

                // client 2: connect to GS and try to join game where it is expected
                this.ConnectAndAuthenticate(masterClient2, joinResponse1.Address);
                masterClient2.JoinGame(joinRequest2);

                var setPropertiesRequest = new OperationRequest
                {
                    OperationCode = OperationCode.SetProperties,

                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.ExpectedValues, new Hashtable{{(byte) GameParameter.ExpectedUsers, new[] {this.Player2}}}},
                        {ParameterCode.Properties, new Hashtable{{(byte) GameParameter.ExpectedUsers, new [] {this.Player3}}}}
                    }
                };

                Thread.Sleep(10);
                masterClient2.SendRequestAndWaitForResponse(setPropertiesRequest);

            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3, masterClient4);
            }
        }

        [Test]
        public void Slots_JoinRandomGameWithSlotsTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;
            UnifiedTestClient masterClient4 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 6}
                    },
                    AddUsers = new[] { this.Player2, this.Player3 }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);
                masterClient4 = this.CreateMasterClientAndAuthenticate("Player4");

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

                masterClient1.WaitForEvent(EventCode.Join);

                Thread.Sleep(100);
                var lobbyStatsResponse = masterClient4.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);

                var joinRequest2 = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.JoinOnly,
                    CheckUserOnJoin = true,
                };

                joinResponse1 = masterClient2.JoinGame(joinRequest2);

                this.ConnectAndAuthenticate(masterClient2, joinResponse1.Address);
                masterClient2.JoinGame(joinRequest2);

                masterClient4.JoinRandomGame(new JoinRandomGameRequest
                                            {
                                                AddUsers = new []{"Player5", "Player6", "Player7"}
                                            }, ErrorCode.Ok);

                masterClient4.JoinRandomGame(new JoinRandomGameRequest
                {
                    AddUsers = new[] { "Player5", "Player6", "Player7", "Player8" }
                }, ErrorCode.NoRandomMatchFound);
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3, masterClient4);
            }
        }

        [Test]
        public void Slots_SlotReservationForActiveUserTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;
            UnifiedTestClient masterClient4 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 4}
                    },
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);
                masterClient4 = this.CreateMasterClientAndAuthenticate("Player4");

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

                masterClient1.WaitForEvent(EventCode.Join);

                Thread.Sleep(100);

                var joinRequest2 = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.JoinOnly,
                    CheckUserOnJoin = true,
                };

                joinResponse1 = masterClient2.JoinGame(joinRequest2);

                this.ConnectAndAuthenticate(masterClient2, joinResponse1.Address);
                masterClient2.JoinGame(joinRequest2);

                joinRequest2.AddUsers = new[] {this.Player2};
                joinResponse1 = masterClient4.JoinGame(joinRequest2);
                this.ConnectAndAuthenticate(masterClient4, joinResponse1.Address);
                masterClient4.JoinGame(joinRequest2);
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3, masterClient4);
            }
        }

        [Test]
        public void Slots_RejoinOrJoinOnlyModeTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;
            UnifiedTestClient masterClient4 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 0}
                    },
                    AddUsers = new[] { this.Player2, this.Player3 }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);
                masterClient4 = this.CreateMasterClientAndAuthenticate("Player4");

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

                masterClient1.WaitForEvent(EventCode.Join);

                Thread.Sleep(100);
                var lobbyStatsResponse = masterClient4.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);

                var joinRequest2 = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.RejoinOrJoin,
                    CheckUserOnJoin = true,
                };

                masterClient2.JoinGame(joinRequest2);

                this.ConnectAndAuthenticate(masterClient2, joinResponse1.Address);
                masterClient2.JoinGame(joinRequest2, ErrorCode.OperationNotAllowedInCurrentState);

                Thread.Sleep(100);
                lobbyStatsResponse = masterClient4.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3, masterClient4);
            }
        }

        [Test]
        public void Slots_EveryTeamMateReservesSlotsTest()
        {
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires userId to be set");
            }

            UnifiedTestClient masterClient1 = null;
            UnifiedTestClient masterClient2 = null;
            UnifiedTestClient masterClient3 = null;
            UnifiedTestClient masterClient4 = null;

            try
            {
                var roomName = this.GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name + "_");
                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = true,
                    PlayerTTL = int.MaxValue,
                    GameProperties = new Hashtable
                    {
                        {GamePropertyKey.MaxPlayers, 3}
                    },
                    AddUsers = new[] { this.Player2, this.Player3 }
                };

                masterClient1 = this.CreateMasterClientAndAuthenticate(Player1);
                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);
                masterClient3 = this.CreateMasterClientAndAuthenticate(Player3);
                masterClient4 = this.CreateMasterClientAndAuthenticate("Player4");

                var joinResponse1 = masterClient1.JoinGame(joinRequest);

                // client 1: connect to GS and try to join not existing game on the game server (create if not exists)
                this.ConnectAndAuthenticate(masterClient1, joinResponse1.Address);
                masterClient1.JoinGame(joinRequest);

                masterClient1.WaitForEvent(EventCode.Join);

                Thread.Sleep(100);
                var lobbyStatsResponse = masterClient4.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);

                var joinRequest2 = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.JoinOnly,
                    CheckUserOnJoin = true,
                    AddUsers = new[] { this.Player1, this.Player3 }
                };

                joinResponse1 = masterClient2.JoinGame(joinRequest2);

                Thread.Sleep(100);
                lobbyStatsResponse = masterClient4.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);

                this.ConnectAndAuthenticate(masterClient2, joinResponse1.Address);
                masterClient2.JoinGame(joinRequest2);

                Thread.Sleep(100);
                lobbyStatsResponse = masterClient4.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);

                joinResponse1 = masterClient3.JoinGame(joinRequest2);

                this.ConnectAndAuthenticate(masterClient3, joinResponse1.Address);
                masterClient3.JoinGame(joinRequest2);

                Thread.Sleep(100);
                lobbyStatsResponse = masterClient4.GetLobbyStats(null, null);
                Assert.AreEqual(3, lobbyStatsResponse.PeerCount[0]);
            }
            finally
            {
                DisposeClients(masterClient1, masterClient2, masterClient3, masterClient4);
            }
        }

        #endregion

        #region Expiration of Player Ttl Tests

        [Test]
        public void PlayerTtlTimeExpiredWhileGameInStorageTest()
        {
            if (!this.UsePlugins)
            {
                Assert.Ignore("test needs plugin support");
            }
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("this tests requires userId to be set");
            }

            UnifiedTestClient masterClient = null;
            UnifiedTestClient masterClient2 = null;

            const int playerTtl = 5000;
            try
            {
                var roomName = this.GenerateRandomizedRoomName("PlayerTtlTimeTest_");
                var myRoomOptions = new RoomOptions
                {
                    PlayerTtl = playerTtl,
                    MaxPlayers = 4,
                    IsOpen = true,
                    IsVisible = true,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1)
                };

                masterClient = this.CreateMasterClientAndAuthenticate(Player1);
                var response = masterClient.CreateGame(roomName);
                this.ConnectAndAuthenticate(masterClient, response.Address, Player1);

                masterClient.CreateRoom(roomName, myRoomOptions, TypedLobby.Default, null, true, "SaveLoadStateTestPlugin");
                Thread.Sleep(300); // wait while game is created on gameserver

                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);

                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                };
                var jgResponse = masterClient2.JoinGame(joinRequest);

                this.ConnectAndAuthenticate(masterClient2, jgResponse.Address, Player2);

                Thread.Sleep(300);
                masterClient2.JoinGame(joinRequest);

                var joinEvent2 = masterClient2.WaitForEvent(EventCode.Join);
                masterClient2.LeaveGame(true); // leave game, but stay inactive there

                Thread.Sleep(3000);

                DisposeClients(masterClient2);
                masterClient.LeaveGame(true);

                Thread.Sleep(2000);

                this.ConnectAndAuthenticate(masterClient, response.Address, Player1);

                joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.RejoinOnly,
                    Plugins = new string[] { "SaveLoadStateTestPlugin" },
                };

                masterClient.JoinGame(joinRequest);
                Thread.Sleep(100);

                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);

                this.ConnectAndAuthenticate(masterClient2, response.Address, Player2);
                Thread.Sleep(300);

                joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.RejoinOnly,
                    ActorNr = (int) joinEvent2[ParameterCode.ActorNr],
                };

                masterClient2.JoinGame(joinRequest, (short)Photon.Common.ErrorCode.JoinFailedWithRejoinerNotFound);
            }
            finally
            {
                DisposeClients(masterClient);
                DisposeClients(masterClient2);
            }
        }

        [Test]
        public void PlayerTtlTimeExpiredAfterReloadingTest()
        {
            if (!this.UsePlugins)
            {
                Assert.Ignore("test needs plugin support");
            }
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("this tests requires userId to be set");
            }

            UnifiedTestClient masterClient = null;
            UnifiedTestClient masterClient2 = null;

            const int playerTtl = 5000;
            try
            {
                var roomName = this.GenerateRandomizedRoomName("PlayerTtlTimeTest_");
                var myRoomOptions = new RoomOptions
                {
                    PlayerTtl = playerTtl,
                    MaxPlayers = 4,
                    IsOpen = true,
                    IsVisible = true,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1)
                };

                masterClient = this.CreateMasterClientAndAuthenticate(Player1);
                var response = masterClient.CreateGame(roomName);
                this.ConnectAndAuthenticate(masterClient, response.Address, Player1);

                masterClient.CreateRoom(roomName, myRoomOptions, TypedLobby.Default, null, true, "SaveLoadStateTestPlugin");
                Thread.Sleep(300); // wait while game is created on gameserver

                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);

                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                };
                var jgResponse = masterClient2.JoinGame(joinRequest);

                this.ConnectAndAuthenticate(masterClient2, jgResponse.Address, Player2);

                Thread.Sleep(300);
                masterClient2.JoinGame(joinRequest);

                var joinEvent2 = masterClient2.WaitForEvent(EventCode.Join);
                masterClient2.LeaveGame(true); // leave game, but stay inactive there

                Thread.Sleep(1000);

                DisposeClients(masterClient2);
                masterClient.LeaveGame(true);

                Thread.Sleep(1000);

                this.ConnectAndAuthenticate(masterClient, response.Address, Player1);

                joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.RejoinOnly,
                    Plugins = new string[] { "SaveLoadStateTestPlugin" },
                };

                masterClient.JoinGame(joinRequest);
                Thread.Sleep(3000);

                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);

                this.ConnectAndAuthenticate(masterClient2, response.Address, Player2);
                Thread.Sleep(300);

                joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.RejoinOnly,
                    ActorNr = (int) joinEvent2[ParameterCode.ActorNr],
                };

                masterClient2.JoinGame(joinRequest, (short)Photon.Common.ErrorCode.JoinFailedWithRejoinerNotFound);
            }
            finally
            {
                DisposeClients(masterClient);
                DisposeClients(masterClient2);
            }
        }

        [Test]
        public void PlayerTtlTimeNotExpiredTest()
        {
            if (!this.UsePlugins)
            {
                Assert.Ignore("test needs plugin support");
            }

            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("this tests requires userId to be set");
            }

            UnifiedTestClient masterClient = null;
            UnifiedTestClient masterClient2 = null;

            const int playerTtl = 5000;
            try
            {
                var roomName = this.GenerateRandomizedRoomName("PlayerTtlTimeTest_");
                var myRoomOptions = new RoomOptions
                {
                    PlayerTtl = playerTtl,
                    MaxPlayers = 4,
                    IsOpen = true,
                    IsVisible = true,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1)
                };

                masterClient = this.CreateMasterClientAndAuthenticate(Player1);
                var response = masterClient.CreateGame(roomName);
                this.ConnectAndAuthenticate(masterClient, response.Address, Player1);

                masterClient.CreateRoom(roomName, myRoomOptions, TypedLobby.Default, null, true, "SaveLoadStateTestPlugin");
                Thread.Sleep(300); // wait while game is created on gameserver

                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);

                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                };
                var jgResponse = masterClient2.JoinGame(joinRequest);

                this.ConnectAndAuthenticate(masterClient2, jgResponse.Address, Player2);

                Thread.Sleep(300);
                masterClient2.JoinGame(joinRequest);

                var joinEvent2 = masterClient2.WaitForEvent(EventCode.Join);
                masterClient2.LeaveGame(true); // leave game, but stay inactive there

                Thread.Sleep(1000);

                DisposeClients(masterClient2);
                masterClient.LeaveGame(true);

                Thread.Sleep(1000);

                this.ConnectAndAuthenticate(masterClient, response.Address);

                joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.RejoinOnly,
                    Plugins = new string[] { "SaveLoadStateTestPlugin" },
                };

                masterClient.JoinGame(joinRequest);
                Thread.Sleep(100);

                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);

                this.ConnectAndAuthenticate(masterClient2, response.Address, Player2);
                Thread.Sleep(300);

                joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.RejoinOnly,
                };

                masterClient2.JoinGame(joinRequest);
            }
            finally
            {
                DisposeClients(masterClient);
                DisposeClients(masterClient2);
            }
        }

        [Test]
        public void PlayerTtlTimeExpiredForFistPlayerBeforeReloadingTest()
        {
            if (!this.UsePlugins)
            {
                Assert.Ignore("test needs plugin support");
            }
            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("this tests requires userId to be set");
            }

            UnifiedTestClient masterClient = null;
            UnifiedTestClient masterClient2 = null;

            const int playerTtl = 3000;
            try
            {
                var roomName = this.GenerateRandomizedRoomName("PlayerTtlTimeTest_");
                var myRoomOptions = new RoomOptions
                {
                    PlayerTtl = playerTtl,
                    MaxPlayers = 4,
                    IsOpen = true,
                    IsVisible = true,
                    EmptyRoomTtl = 1000,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1)
                };

                masterClient = this.CreateMasterClientAndAuthenticate(Player1);
                var response = masterClient.CreateGame(roomName);
                this.ConnectAndAuthenticate(masterClient, response.Address, Player1);

                masterClient.CreateRoom(roomName, myRoomOptions, TypedLobby.Default, null, true, "SaveLoadStateTestPlugin");
                Thread.Sleep(300); // wait while game is created on gameserver

                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);

                var joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                };
                var jgResponse = masterClient2.JoinGame(joinRequest);

                this.ConnectAndAuthenticate(masterClient2, jgResponse.Address, Player2);

                Thread.Sleep(300);
                masterClient2.JoinGame(joinRequest);

                var joinEvent2 = masterClient2.WaitForEvent(EventCode.Join);
                masterClient2.LeaveGame(true); // leave game, but stay inactive there

                DisposeClients(masterClient2);
                masterClient.LeaveGame(true);

                Thread.Sleep(3500);

                this.ConnectAndAuthenticate(masterClient, response.Address, Player1);

                var createRequest = new CreateGameRequest
                {
                    GameId = roomName,
                    JoinMode = Photon.Hive.Operations.JoinModes.CreateIfNotExists,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    PlayerTTL = int.MaxValue,
                    ActorNr = 1,
                    Plugins = new[] { "SaveLoadStateTestPlugin" }
                };

                masterClient.CreateGame(createRequest, (short)Photon.Common.ErrorCode.JoinFailedWithRejoinerNotFound); //TBD - client needs new errocodes
                //Thread.Sleep(100);

                masterClient2 = this.CreateMasterClientAndAuthenticate(Player2);

                this.ConnectAndAuthenticate(masterClient2, response.Address, Player2);
                Thread.Sleep(300);

                joinRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    JoinMode = JoinModes.RejoinOnly,
                    ActorNr = (int) joinEvent2[ParameterCode.ActorNr],
                };

                masterClient2.JoinGame(joinRequest, (short)Photon.Common.ErrorCode.JoinFailedWithRejoinerNotFound);
            }
            finally
            {
                DisposeClients(masterClient);
                DisposeClients(masterClient2);
            }
        }

        #endregion

        #region Bann tests

        [Test]
        public void BanPlayerTest()
        {
            if (!this.UsePlugins)
            {
                Assert.Ignore("test needs plugin support");
            }

            if (string.IsNullOrEmpty(this.Player1))
            {
                Assert.Ignore("This test requires user id to be set");
            }
            UnifiedTestClient client1 = null;
            UnifiedTestClient client2 = null;

            var gameName = MethodBase.GetCurrentMethod().Name;

            try
            {
                client1 = this.CreateMasterClientAndAuthenticate(this.Player1);

                var createGame = new CreateGameRequest
                {
                    GameId = gameName,
                    CheckUserOnJoin = !string.IsNullOrEmpty(this.Player1),
                    Plugins = new string[] { "BanTestPlugin"},
                };

                var response = client1.CreateGame(createGame);

                this.ConnectAndAuthenticate(client1, response.Address);

                client1.CreateGame(createGame);

                Thread.Sleep(300);
                client2 = this.CreateMasterClientAndAuthenticate(this.Player2);

                var joinGame = new JoinGameRequest
                {
                    GameId = gameName,
                };

                var joinGameResponse = client2.JoinGame(joinGame);

                this.ConnectAndAuthenticate(client2, joinGameResponse.Address);

                client2.JoinGame(joinGame);

                //all joined lets ban

                var raiseEventData = new Hashtable
                {
                    {0, true},
                    {1, 2}
                };

                var operation = new OperationRequest
                {
                    OperationCode = OperationCode.RaiseEvent,
                    Parameters = new Dictionary<byte, object>
                    {
                        {ParameterCode.Data, raiseEventData}
                    }
                };

                client1.SendRequest(operation);

                Thread.Sleep(1500);
                // ha ha ha, we banned him
                Assert.IsFalse(client2.Connected);


                //client2 tries rejoin immediately and ... fails
                this.ConnectAndAuthenticate(client2, joinGameResponse.Address);
                joinGame.JoinMode = (byte)JoinMode.JoinOrRejoin;
                client2.JoinGame(joinGame, (short)Photon.Common.ErrorCode.JoinFailedFoundExcludedUserId);

                //reconnect to master and rejoin through master and ... fails
                this.ConnectAndAuthenticate(client2, this.MasterAddress);

                client2.JoinGame(joinGame, (short)Photon.Common.ErrorCode.JoinFailedFoundExcludedUserId);

                // try to create game
                client2.CreateGame(createGame, ErrorCode.GameIdAlreadyExists);

                joinGame.JoinMode = (byte)JoinMode.CreateIfNotExists;
                client2.JoinGame(joinGame, (short)Photon.Common.ErrorCode.JoinFailedFoundExcludedUserId);

                client2.JoinRandomGame(null, 0, null, MatchmakingMode.RandomMatching, string.Empty, AppLobbyType.Default, null, ErrorCode.NoRandomMatchFound);
            }
            finally
            {
                DisposeClients(client1, client2);
            }
        }
        #endregion

        #region Helpers

        private JoinGameResponse ConnectClientToGame(UnifiedTestClient client, string roomName, int actorNr = 0)
        {

            var joinRequest = new JoinGameRequest
            {
                GameId = roomName,
            };
            if (actorNr > 0)
            {
                joinRequest.ActorNr = actorNr;
            }
            // request to master
            var jgResponse = client.JoinGame(joinRequest);

            // connect to GS
            this.ConnectAndAuthenticate(client, jgResponse.Address, client.UserId);

            // request to GS
            return client.JoinGame(joinRequest);
        }

        private static void FillEventsCache(UnifiedTestClient masterClient)
        {
            // put events in slice == 0
            masterClient.SendRequest(new OperationRequest
            {
                OperationCode = OperationCode.RaiseEvent,
                Parameters = new Dictionary<byte, object>
                {
                    {ParameterCode.Code, (byte) 1},
                    {ParameterCode.Cache, (byte)EventCaching.AddToRoomCache},
                }
            });

            // increment slice
            masterClient.SendRequest(new OperationRequest
            {
                OperationCode = OperationCode.RaiseEvent,
                Parameters = new Dictionary<byte, object>
                {
                    {ParameterCode.Cache, (byte)EventCaching.SliceIncreaseIndex},
                    {ParameterCode.Code, (byte) 2},
                }
            });

            // put events to slice == 1
            masterClient.SendRequest(new OperationRequest
            {
                OperationCode = OperationCode.RaiseEvent,
                Parameters = new Dictionary<byte, object>
                {
                    {ParameterCode.Code, (byte) 3},
                    {ParameterCode.Cache, (byte) EventCaching.AddToRoomCache},
                }
            });
        }

        private void WaitUntilEmptyGameList(int timeout = 5000)
        {
            UnifiedTestClient client = null;
            var time = 0;

            while (time < timeout)
            {
                Hashtable gameList;
                try
                {
                    client = this.CreateMasterClientAndAuthenticate("GameCheckUser");
                    client.JoinLobby();

                    var ev = client.WaitForEvent((byte)Events.EventCode.GameList);
                    Assert.AreEqual((byte)Events.EventCode.GameList, ev.Code);
                    gameList = (Hashtable)ev.Parameters[ParameterCode.GameList];

                }
                finally
                {
                    DisposeClients(client);
                }

                int openGames = 0;
                foreach (DictionaryEntry item in gameList)
                {
                    var gameProperties = (Hashtable)item.Value;
                    if (!gameProperties.ContainsKey(GamePropertyKey.Removed))
                    {
                        openGames++;
                    }
                }

                if (openGames == 0)
                {
                    return;
                }
                Thread.Sleep(100);
                time += 150;
            }

            Assert.Fail("Timeout {0} ms expired. Server still has games", timeout);
        }

        private void CheckGameListCount(int expectedGameCount, Hashtable gameList = null)
        {
            if (gameList == null)
            {
                UnifiedTestClient client = null;

                try
                {
                    client = this.CreateMasterClientAndAuthenticate(Player1);
                    client.JoinLobby();

                    var ev = client.WaitForEvent((byte)Photon.LoadBalancing.Events.EventCode.GameList);
                    Assert.AreEqual((byte)Photon.LoadBalancing.Events.EventCode.GameList, ev.Code);
                    gameList = (Hashtable)ev.Parameters[ParameterCode.GameList];
                }
                finally
                {
                    DisposeClients(client);
                }
            }

            int openGames = 0;
            foreach (DictionaryEntry item in gameList)
            {
                var gameProperties = (Hashtable)item.Value;
                if (!gameProperties.ContainsKey(GamePropertyKey.Removed))
                {
                    openGames++;
                }
            }

            if (expectedGameCount > 0 && openGames == 0)
            {
                Assert.Fail("Expected {0} games listed in lobby, but got: 0", expectedGameCount);
            }

            if (openGames != expectedGameCount)
            {
                var gameNames = new string[gameList.Count];
                gameList.Keys.CopyTo(gameNames, 0);
                var msg = string.Format(
                    "Expected {0} open games, but got {1}: {2}", expectedGameCount, openGames, string.Join(",", gameNames));
                Assert.Fail(msg);
            }
        }

        private void CreateRoomOnGameServer(UnifiedTestClient masterClient, string roomName)
        {
            this.CreateRoomOnGameServer(masterClient, true, true, 0, roomName);
        }

        private void CreateRoomOnGameServer(
            UnifiedTestClient masterClient,
            bool isVisible,
            bool isOpen,
            byte maxPlayers,
            string roomName)
        {

            var createGameResponse = masterClient.CreateGame(roomName, isVisible, isOpen, maxPlayers);

            this.ConnectAndAuthenticate(masterClient, createGameResponse.Address, masterClient.UserId);
            masterClient.CreateGame(roomName, true, true, maxPlayers);

            // get own join event: 
            var ev = masterClient.WaitForEvent();
            Assert.AreEqual(EventCode.Join, ev.Code);
            Assert.AreEqual(1, ev.Parameters[ParameterCode.ActorNr]);
        }

        private UnifiedTestClient CreateGameOnGameServer(
            string userName,
            string roomName,
            string lobbyName,
            byte lobbyType,
            bool? isVisible,
            bool? isOpen,
            byte? maxPlayer,
            Hashtable gameProperties,
            string[] lobbyProperties, int RoomTTL = 0)
        {
            var createRequest = new CreateGameRequest
            {
                GameId = roomName,
                GameProperties = gameProperties,
                LobbyName = lobbyName,
                LobbyType = lobbyType,
                EmptyRoomLiveTime = RoomTTL
            };

            if (createRequest.GameProperties == null)
            {
                createRequest.GameProperties = new Hashtable();
            }

            if (isVisible.HasValue)
            {
                createRequest.GameProperties[GamePropertyKey.IsVisible] = isVisible.Value;
            }

            if (isOpen.HasValue)
            {
                createRequest.GameProperties[GamePropertyKey.IsOpen] = isOpen.Value;
            }

            if (maxPlayer.HasValue)
            {
                createRequest.GameProperties[GamePropertyKey.MaxPlayers] = maxPlayer.Value;
            }

            if (lobbyProperties != null)
            {
                createRequest.GameProperties[GamePropertyKey.PropsListedInLobby] = lobbyProperties;
            }


            return this.CreateGameOnGameServer(userName, createRequest);
        }

        private UnifiedTestClient CreateGameOnGameServer(string userName, CreateGameRequest createRequest)
        {
            UnifiedTestClient client = null;
            var gameCreated = false;

            try
            {
                client = this.CreateMasterClientAndAuthenticate(userName);
                var response = client.CreateGame(createRequest);

                this.ConnectAndAuthenticate(client, response.Address, userName);
                client.CreateGame(createRequest);
                gameCreated = true;

                // in order to give server some time to update data about game on master server
                Thread.Sleep(100);
            }
            finally
            {
                if (!gameCreated)
                {
                    DisposeClients(client);
                }
            }

            return client;
        }

        private string GenerateRandomizedRoomName(string roomName)
        {
            return (string.IsNullOrEmpty(this.GameNamePrefix) ? string.Empty : this.GameNamePrefix + "_") + roomName + Guid.NewGuid().ToString().Substring(0, 6);
        }

        private static T GetParameter<T>(Dictionary<byte, object> parameterDict, byte parameterCode, string parameterName = null)
        {
            string paramText;
            if (string.IsNullOrEmpty(parameterName))
            {
                paramText = parameterCode.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                paramText = string.Format("{0} ({1})", parameterName, parameterCode);
            }

            object value;
            if (parameterDict.TryGetValue(parameterCode, out value) == false)
            {

                Assert.Fail("{0} parameter is missing", paramText);
            }

            Assert.IsInstanceOf<T>(value, "{0} parameter has wrong type.", paramText);
            return (T)value;
        }

        private void VerifyLobbyStatisticsFullList(GetLobbyStatsResponse response, string[] expectedLobbyNames, byte[] expectedLobbyTypes, int[] expectedPeerCount, int[] expectedGameCount)
        {
            // verify that all parameters are set when getting all lobby stats
            Assert.IsNotNull(response.LobbyNames, "LobbyNames missing");
            Assert.IsNotNull(response.LobbyTypes, "LobbyTypes missing");
            Assert.IsNotNull(response.LobbyNames, "PeerCount missing");
            Assert.IsNotNull(response.LobbyTypes, "GameCount missing");

            // verify that count of all parameters are equal
            Assert.AreEqual(response.LobbyNames.Length, response.LobbyTypes.Length, "LobbyTypes count does not match LobbyNames count");
            Assert.AreEqual(response.LobbyNames.Length, response.PeerCount.Length, "PeerCount count does not match LobbyNames count");
            Assert.AreEqual(response.LobbyNames.Length, response.GameCount.Length, "GameCount count does not match LobbyNames count");

            // try to find expected lobbies
            for (int i = 0; i < expectedLobbyNames.Length; i++)
            {
                int lobbyIndex = -1;
                for (int j = 0; j < response.LobbyNames.Length; j++)
                {
                    if (response.LobbyNames[j] == expectedLobbyNames[i] && response.LobbyTypes[j] == expectedLobbyTypes[i])
                    {
                        lobbyIndex = j;
                        break;
                    }
                }

                Assert.GreaterOrEqual(lobbyIndex, 0, "Lobby not found in statistics: name={0}, type={1}", expectedLobbyNames[i], expectedLobbyTypes[i]);
                Assert.AreEqual(expectedPeerCount[i], response.PeerCount[lobbyIndex], "Unexpected peer count");
                Assert.AreEqual(expectedGameCount[i], response.GameCount[lobbyIndex], "Unexpected game count");
            }
        }

        private void VerifyLobbyStatisticsList(GetLobbyStatsResponse response, int[] expectedPeerCount, int[] expectedGameCount)
        {
            // verify that all parameters are set when getting all lobby stats
            Assert.IsNull(response.LobbyNames, "LobbyNames are unexpected ");
            Assert.IsNull(response.LobbyTypes, "LobbyTypes are unexpected ");
            Assert.IsNotNull(response.PeerCount, "PeerCount missing");
            Assert.IsNotNull(response.GameCount, "GameCount missing");

            // verify that count of all parameters are equal
            Assert.AreEqual(expectedPeerCount, response.PeerCount, "Unexpected PeerCounts");
            Assert.AreEqual(expectedGameCount, response.GameCount, "Unexpected GameCounts");
        }

        #endregion
    }
}