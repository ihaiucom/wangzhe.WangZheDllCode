// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinRandomGameRequest.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the JoinRandomGameRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Operations
{
    #region using directives

    using System.Collections;

    using Photon.Hive.Common;
    using Photon.Hive.Operations;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using Photon.SocketServer.Rpc.Protocols;

    #endregion

    public class JoinRandomGameRequest : Operation
    {
        public JoinRandomGameRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
            if (!this.IsValid)
            {
                return;
            }

            // special handling for game properties send by AS3/Flash (Amf3 protocol) clients
            if (protocol.ProtocolType == ProtocolType.Amf3V16 || protocol.ProtocolType == ProtocolType.Json)
            {
                Utilities.ConvertAs3WellKnownPropertyKeys(this.GameProperties, null);
            }
        }

        public JoinRandomGameRequest()
        {
        }

        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable GameProperties { get; set; }

        [DataMember(Code = (byte)ParameterCode.MatchMakingType, IsOptional = true)]
        public byte JoinRandomType { get; set; }

        [DataMember(Code = (byte)ParameterKey.Data, IsOptional = true)]
        public string QueryData { get; set; }

        [DataMember(Code = (byte)ParameterCode.LobbyName, IsOptional = true)]
        public string LobbyName { get; set; }

        [DataMember(Code = (byte)ParameterCode.LobbyType, IsOptional = true)]
        public byte LobbyType { get; set; }

        [DataMember(Code = (byte)ParameterCode.AddUsers, IsOptional = true)]
        public string[] AddUsers { get; set; }
    }
}