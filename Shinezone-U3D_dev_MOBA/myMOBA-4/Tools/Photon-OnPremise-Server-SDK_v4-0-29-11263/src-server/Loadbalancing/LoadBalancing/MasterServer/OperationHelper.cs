// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationHelper.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Provides basic methods for <see cref="IOperationHandler" /> implementations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Common;
using Photon.LoadBalancing.Common;

namespace Photon.LoadBalancing.MasterServer
{
    #region using directives

    using ExitGames.Logging;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    #endregion

    /// <summary>
    ///   Provides static methods to validate operation requests and create
    ///   operation responses for invalid operation request.
    /// </summary>
    public static class OperationHelper
    {
        public static OperationResponse HandleInvalidOperation(Operation operation, ILogger logger)
        {
            string errorMessage = operation.GetErrorMessage();

            if (logger != null && logger.IsDebugEnabled)
            {
                logger.DebugFormat("Invalid operation: OpCode={0}; {1}", operation.OperationRequest.OperationCode, errorMessage);
            }

            return new OperationResponse { OperationCode = operation.OperationRequest.OperationCode, ReturnCode = (short)ErrorCode.OperationInvalid, DebugMessage = errorMessage };
        }

        public static OperationResponse HandleUnknownOperationCode(OperationRequest operationRequest, ILogger logger)
        {
            if (logger != null && logger.IsDebugEnabled)
            {
                logger.DebugFormat("Unknown operation code: OpCode={0}", operationRequest.OperationCode);
            }

            return new OperationResponse 
            { 
                OperationCode = operationRequest.OperationCode, 
                ReturnCode = (short)ErrorCode.OperationInvalid, 
                DebugMessage = LBErrorMessages.UnknownOperationCode 
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
    }
}