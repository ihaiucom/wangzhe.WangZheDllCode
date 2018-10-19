// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationHandlerBase.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Provides basic methods for <see cref="IOperationHandler" /> implementations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ExitGames.Logging;
using Photon.Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Photon.LoadBalancing.Common
{
    #region using directives

    

    #endregion

    /// <summary>
    ///   Provides basic methods for <see cref = "IOperationHandler" /> implementations.
    /// </summary>
    public abstract class OperationHandlerBase : IOperationHandler
    {
        #region Public Methods

        public static OperationResponse HandleInvalidOperation(Operation operation, ILogger logger)
        {
            string errorMessage = operation.GetErrorMessage();

            if (logger != null && logger.IsDebugEnabled)
            {
                logger.DebugFormat("Invalid operation: OpCode={0}; {1}", operation.OperationRequest.OperationCode, errorMessage);
            }

            return new OperationResponse(operation.OperationRequest.OperationCode)
            {
                ReturnCode = (short)ErrorCode.OperationInvalid, DebugMessage = errorMessage
            };
        }

        public static OperationResponse HandleUnknownOperationCode(OperationRequest operationRequest, ILogger logger)
        {
            if (logger != null && logger.IsDebugEnabled)
            {
                logger.DebugFormat("Unknown operation code: OpCode={0}", operationRequest.OperationCode);
            }

            return new OperationResponse(operationRequest.OperationCode)
            {
                ReturnCode = (short)ErrorCode.OperationInvalid, DebugMessage = LBErrorMessages.UnknownOperationCode
            };
        }

        public static bool ValidateOperation(Operation operation, ILogger logger, out OperationResponse response)
        {
            if (operation.IsValid)
            {
                response = null;
                return true;
            }

            response = HandleInvalidOperation(operation, logger);
            return false;
        }

        #endregion

        #region Implemented Interfaces

        #region IOperationHandler

        public virtual void OnDisconnect(PeerBase peer)
        {
        }

        public virtual void OnDisconnectByOtherPeer(PeerBase peer)
        {
        }

        public abstract OperationResponse OnOperationRequest(PeerBase peer, OperationRequest operationRequest, SendParameters sendParameters);

        #endregion

        #endregion
    }
}