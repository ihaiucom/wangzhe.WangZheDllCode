// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateGameRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CreateGameRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Operations
{
    #region using directives

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    #endregion

    public class CreateGameRequest : JoinGameRequest
    {
        public CreateGameRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        public CreateGameRequest()
        {
        }

        [DataMember(Code = (byte)ParameterKey.GameId, IsOptional = true)]
        public override string GameId { get; set; }
    }
}