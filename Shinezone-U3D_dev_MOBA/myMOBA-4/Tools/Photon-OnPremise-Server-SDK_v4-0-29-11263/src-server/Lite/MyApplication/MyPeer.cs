// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MyPeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MyApplication
{
    using ExitGames.Logging;

    using Lite;
    using Lite.Caching;
    using Lite.Operations;

    using global::MyApplication.Operations;

    using Photon.SocketServer;

    using PhotonHostRuntimeInterfaces;

    public class MyPeer : LitePeer
    {
        #region Constants and Fields

        /// <summary>
        ///   An <see cref = "ILogger" /> instance used to log messages to the logging framework.
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors and Destructors

        public MyPeer(InitRequest initRequest)
            : base(initRequest)
        {
        }

        #endregion

        #region Methods

        protected override RoomReference GetRoomReference(JoinRequest joinRequest)
        {
            return MyGameCache.Instance.GetRoomReference(joinRequest.GameId, this);
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("OnOperationRequest. Code={0}", operationRequest.OperationCode);
            }

            switch (operationRequest.OperationCode)
            {
                case (byte)MyOperationCodes.EchoOperation:
                    {
                        // The echo operation one is handled immediately because it does not require the client to join a game.
                        var myEchoRequest = new MyEchoRequest(this.Protocol, operationRequest);
                        if (this.ValidateOperation(myEchoRequest, sendParameters) == false)
                        {
                            return;
                        }

                        var myEchoResponse = new MyEchoResponse { Response = myEchoRequest.Text };
                        var operationResponse = new OperationResponse(operationRequest.OperationCode, myEchoResponse);
                        this.SendOperationResponse(operationResponse, sendParameters);
                        break;
                    }

                default:
                    {
                        // for this example all other operations will handled by the base class
                        base.OnOperationRequest(operationRequest, sendParameters);
                        return;
                    }
            }
        }

        #endregion
    }
}