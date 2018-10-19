// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationHandlerDefault.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the OperationHandlerDefault type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Common;
using Photon.LoadBalancing.Common;

namespace Photon.LoadBalancing.Master.OperationHandler
{
    #region using directives

    using ExitGames.Logging;

    using Photon.LoadBalancing.MasterServer;
    using Photon.SocketServer;

    using OperationCode = Photon.LoadBalancing.Operations.OperationCode;

    #endregion

    public class OperationHandlerDefault : OperationHandlerBase
    {
        #region Constants and Fields

        public static readonly OperationHandlerDefault Instance = new OperationHandlerDefault();

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods

        public override OperationResponse OnOperationRequest(PeerBase peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            var clientPeer = (MasterClientPeer)peer;

            switch (operationRequest.OperationCode)
            {
                default:
                    return HandleUnknownOperationCode(operationRequest, log);

                case (byte)OperationCode.Authenticate:
                    return new OperationResponse(operationRequest.OperationCode)
                    {
                        ReturnCode = (short)ErrorCode.OperationDenied, DebugMessage = LBErrorMessages.AlreadyAuthenticated,
                    };

                case (byte)OperationCode.JoinLobby:
                    return clientPeer.HandleJoinLobby(operationRequest, sendParameters);

                case (byte)OperationCode.LeaveLobby:
                    return clientPeer.HandleLeaveLobby(operationRequest);

                case (byte)OperationCode.CreateGame:
                    return clientPeer.HandleCreateGame(operationRequest, sendParameters);

                case (byte)OperationCode.JoinGame:
                    return clientPeer.HandleJoinGame(operationRequest, sendParameters);

                case (byte)OperationCode.JoinRandomGame:
                    return clientPeer.HandleJoinRandomGame(operationRequest, sendParameters);

                case (byte)OperationCode.FindFriends:
                    return clientPeer.HandleFindFriends(operationRequest, sendParameters);

                case (byte)OperationCode.LobbyStats:
                    return clientPeer.HandleLobbyStatsRequest(operationRequest, sendParameters);

                case (byte)OperationCode.Rpc:
                    return clientPeer.HandleRpcRequest(operationRequest, sendParameters);
            }
        }
        #endregion
    }
}