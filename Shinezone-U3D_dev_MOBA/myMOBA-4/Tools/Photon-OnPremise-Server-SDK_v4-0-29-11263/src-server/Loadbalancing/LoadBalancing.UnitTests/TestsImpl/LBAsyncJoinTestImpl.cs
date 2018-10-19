using System;
using System.Reflection;
using System.Threading;
using ExitGames.Client.Photon.LoadBalancing;
using NUnit.Framework;
using Photon.Hive.Operations;
using Photon.LoadBalancing.UnifiedClient;
using Photon.LoadBalancing.UnitTests.UnifiedServer;
using Photon.UnitTest.Utils.Basic;

namespace Photon.LoadBalancing.UnitTests.TestsImpl
{
    public abstract class LBAsyncJoinTestImpl : LoadBalancingUnifiedTestsBase
    {
        protected LBAsyncJoinTestImpl(ConnectPolicy policy) : base(policy)
        {
        }

        [Test]
        public void AsyncJoinToNonExistingGame()
        {
            UnifiedTestClient client = null;

            try
            {
                var roomName = GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);

                // try join game on master
                client = this.CreateMasterClientAndAuthenticate(Player1);
                client.JoinGame(roomName);

                // try join game on gameServer
                this.ConnectAndAuthenticate(client, this.GameServerAddress);
                client.JoinGame(roomName, ErrorCode.GameDoesNotExist);
            }
            finally
            {
                DisposeClients(client);
            }
        }

        [Test]
        public void AsyncJoinToExistingGame()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;

            try
            {
                var roomName = GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);

                client = this.CreateMasterClientAndAuthenticate(Player1);

                // try join game on gameServer
                this.ConnectAndAuthenticate(client, this.GameServerAddress);
                client.CreateGame(roomName);

                // try join game on master
                client2 = this.CreateMasterClientAndAuthenticate(Player2);
                client2.JoinGame(roomName);

                // try join game on gameServer
                this.ConnectAndAuthenticate(client2, this.GameServerAddress);
                client2.JoinGame(roomName);
            }
            finally
            {
                DisposeClients(client, client2);
            }
        }


        [Test]
        public void AsyncJoinToSavedGame()
        {
            UnifiedTestClient client = null;
            UnifiedTestClient client2 = null;

            try
            {
                var roomName = GenerateRandomizedRoomName(MethodBase.GetCurrentMethod().Name);

                client = this.CreateMasterClientAndAuthenticate(Player1);

                // try join game on gameServer
                this.ConnectAndAuthenticate(client, this.GameServerAddress);

                client.CreateRoom(roomName, new RoomOptions(), TypedLobby.Default, null, true, "SaveLoadStateTestPlugin");

                Thread.Sleep(1000);
                client.Disconnect();
                Thread.Sleep(1000);

                // try join game on master
                client2 = this.CreateMasterClientAndAuthenticate(Player2);
                client2.JoinGame(roomName);

                // try join game on gameServer
                this.ConnectAndAuthenticate(client2, this.GameServerAddress);
                var joinGameRequest = new JoinGameRequest
                {
                    GameId = roomName,
                    Plugins = new[] {"SaveLoadStateTestPlugin"}
                };
                client2.WaitTimeout = 3000000;
                client2.JoinGame(joinGameRequest);
            }
            finally
            {
                DisposeClients(client, client2);
            }
        }



        #region Methods

        private static string GenerateRandomizedRoomName(string roomName)
        {
            return roomName + Guid.NewGuid().ToString().Substring(0, 6);
        }

        #endregion

    }
}
