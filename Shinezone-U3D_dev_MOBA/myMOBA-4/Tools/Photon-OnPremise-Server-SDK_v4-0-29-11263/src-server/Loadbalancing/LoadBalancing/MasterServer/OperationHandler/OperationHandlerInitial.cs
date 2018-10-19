// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationHandlerInitial.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the OperationHandlerInitial type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.LoadBalancing.Common;

namespace Photon.LoadBalancing.Master.OperationHandler
{
    #region using directives

    using ExitGames.Logging;

    using Photon.LoadBalancing.MasterServer;
    using Photon.LoadBalancing.Operations;
    using Photon.SocketServer;

    #endregion

    public class OperationHandlerInitial : OperationHandlerBase
    {
        #region Constants and Fields

        public static readonly OperationHandlerInitial Instance = new OperationHandlerInitial();

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods

        public override OperationResponse OnOperationRequest(PeerBase peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                default:
                    return HandleUnknownOperationCode(operationRequest, log);

                case (byte)OperationCode.Authenticate:
                    return ((MasterClientPeer)peer).HandleAuthenticate(operationRequest, sendParameters);

                case (byte)OperationCode.CreateGame:
                case (byte)OperationCode.JoinGame:
                case (byte)OperationCode.JoinLobby:
                case (byte)OperationCode.JoinRandomGame:
                case (byte)OperationCode.FindFriends:
                case (byte)OperationCode.LobbyStats:
                case (byte)OperationCode.LeaveLobby:
                case (byte)OperationCode.DebugGame:
                case (byte)OperationCode.Rpc: 
                    return new OperationResponse(operationRequest.OperationCode)
                    {
                        ReturnCode = (int)Photon.Common.ErrorCode.OperationDenied, 
                        DebugMessage = LBErrorMessages.NotAuthorized
                    };
            }
        }

        #endregion
    }
}