// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeGroups.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangeGroups type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Operations
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
        /// Gets or sets an array of group identifiers the actor whants to join.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.GroupsForAdd, IsOptional = true)]
        public byte[] Add { get; set; }

        /// <summary>
        /// Gets or sets an array of group identifiers the actor whants to leave.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.GroupsForRemove, IsOptional = true)]
        public byte[] Remove { get; set; }
    }
}
