
namespace Lite.Tests
{
    using System;
    using System.Collections;

    using Client;

    using ExitGames.Rpc.Operations;
    using ExitGames.Rpc.Protocol;

    using NUnit.Framework;

    using Operations;

    [TestFixture]
    public class LobbyTests
    {
        public LobbyTests()
        {
            this.WaitTime = 5000;
        }

        protected int WaitTime
        {
            get;
            set;
        }

        [Test]
        public void JoinGameWithLobby()
        {
            string roomName = CreateRandomRoomName();

            TestClient client = this.InitClient();

            OperationRequest request = new OperationRequest((short)OperationCodes.Join);
            request.Params.Add((short)ParameterKeys.GameId, roomName);
            request.Params.Add((short)LiteLobby.Operations.LobbyParameterKeys.LobbyId, "Mainlobby");
            client.SendOperationRequest(request, 0, true);
            OperationResponse response = client.WaitForOperationResponse(this.WaitTime);
            EventReceivedEventArgs eventArgs = client.WaitForEvent(this.WaitTime);

            // check operation params
            CheckDefaultOperationParams(response, OperationCodes.Join);
            CheckParam(response, ParameterKeys.ActorNr, 1);

            // check event params
            CheckDefaultEventParams(eventArgs, OperationCodes.Join, 1);
            CheckEventParamExists(eventArgs, ParameterKeys.Actors);

            // cleanup
            client.Close();
            client.Dispose();
        }

        protected IPhontonClient CreateClient()
        {
            return new RtsTcpClientV15Byte("LITELOBBY", new RpcClientVersion(1, 0, 0));
        }

        protected virtual TestClient InitClient()
        {
            IPhontonClient photonClient = this.CreateClient();
            TestClient client = new TestClient(photonClient);

            client.Connect("localhost", 4530);
            client.SendInit(0);

            if (client.WaitForInitResponse(this.WaitTime) == false)
            {
                Assert.Fail("Didn't received init response in expected time.");
            }

            return client;
        }

        protected static string CreateRandomRoomName()
        {
            return Guid.NewGuid().ToString();
        }

        #region check methods

        protected static void CheckJoinResponse(OperationResponse operationResponse, int expectedActorNumber)
        {
            CheckDefaultOperationParams(operationResponse, OperationCodes.Join);
            CheckParam(operationResponse, ParameterKeys.ActorNr, expectedActorNumber);
        }

        protected static void CheckJoinEvent(EventReceivedEventArgs eventArgs, int actorNumber)
        {
            CheckJoinEvent(eventArgs, actorNumber, null);
        }

        protected static void CheckJoinEvent(EventReceivedEventArgs eventArgs, int actorNumber, Hashtable expectedActorProperties)
        {
            CheckDefaultEventParams(eventArgs, OperationCodes.Join, actorNumber);
            CheckEventParamExists(eventArgs, ParameterKeys.Actors);

            if (expectedActorProperties != null)
            {
                CheckEventParamExists(eventArgs, ParameterKeys.ActorProperties);
                CheckEventParam(eventArgs, ParameterKeys.ActorProperties, expectedActorProperties);
            }
        }

        protected static void CheckDefaultOperationParams(OperationResponse response, OperationCodes operationCode)
        {
            CheckParamExists(response, ParameterKeys.ERR);
            CheckParamExists(response, ParameterKeys.DBG);

            int error = (int)response.Params[(short)Lite.Operations.ParameterKeys.ERR];
            Assert.AreEqual(0, error, string.Format("Response has Error. ERR={0}, DBG={1}", error, response.Params[(short)ParameterKeys.DBG]));

            CheckParam(response, ParameterKeys.Code, (short)operationCode);
        }

        protected static void CheckParamExists(OperationResponse response, ParameterKeys paramKey)
        {
            Assert.Contains((short)paramKey, response.Params.Keys, "Parameter '{0}' is missing in operation response.", paramKey);
        }

        protected static void CheckParam(OperationResponse response, ParameterKeys paramKey, object expectedValue)
        {
            CheckParamExists(response, paramKey);
            object value = response.Params[(short)paramKey];
            Assert.AreEqual(expectedValue, value, "Parameter '{0} has an unexpected value", paramKey);
        }

        protected static void CheckDefaultEventParams(EventReceivedEventArgs eventArgs, OperationCodes operationCode, int actorNumber)
        {
            CheckEventParam(eventArgs, ParameterKeys.Code, (byte)operationCode);
            CheckEventParam(eventArgs, ParameterKeys.ActorNr, actorNumber);
        }

        protected static void CheckDefaultEventParams(EventReceivedEventArgs eventArgs, EventCodes eventCode, int actorNumber)
        {
            CheckEventParam(eventArgs, ParameterKeys.Code, (byte)eventCode);
            CheckEventParam(eventArgs, ParameterKeys.ActorNr, actorNumber);
        }

        protected static void CheckEventParam(EventReceivedEventArgs eventArgs, ParameterKeys paramKey, object expectedValue)
        {
            CheckEventParamExists(eventArgs, paramKey);
            Assert.AreEqual(expectedValue, eventArgs.EventData[(short)paramKey], "Event param '{0}' has unexpected value", paramKey);
        }

        protected static void CheckEventParamExists(EventReceivedEventArgs eventArgs, ParameterKeys paramKey)
        {
            Assert.Contains((short)paramKey, eventArgs.EventData.Keys, "Parameter '{0}' is missing in event.", paramKey);
        }

        protected static void CheckEventParam(EventReceivedEventArgs eventArgs, object key, object expectedValue)
        {
            CheckEventParamExists(eventArgs, key);
            Assert.AreEqual(expectedValue, eventArgs.EventData[key], "Event param '{0}' has unexpected value", key);
        }

        protected static void CheckEventParamExists(EventReceivedEventArgs eventArgs, object key)
        {
            Assert.Contains(key, eventArgs.EventData.Keys, "Parameter '{0}' is missing in event.", key);
        }
        #endregion check methods

    }
}
