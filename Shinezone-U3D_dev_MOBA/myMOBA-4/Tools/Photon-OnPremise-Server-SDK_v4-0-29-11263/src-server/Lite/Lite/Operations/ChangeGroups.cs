// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeGroups.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangeGroups type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Operations
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class ChangeGroups : Operation
    {
        public ChangeGroups(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        /// <summary>
        /// Gets or sets custom actor properties.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.GroupsForRemove, IsOptional = true)]
        public byte[] Remove { get; set; }

        [DataMember(Code = (byte)ParameterKey.GroupsForAdd, IsOptional = true)]
        public byte[] Add { get; set; }
    }
}
