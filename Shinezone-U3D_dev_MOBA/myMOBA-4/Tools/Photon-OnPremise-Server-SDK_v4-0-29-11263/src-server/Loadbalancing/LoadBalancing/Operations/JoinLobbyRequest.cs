// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinLobbyRequest.cs" company="">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Operations
{
    using System.Collections;

    using Photon.Hive.Common;
    using Photon.Hive.Operations;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using Photon.SocketServer.Rpc.Protocols;

    public class JoinLobbyRequest : Operation
    {
        public JoinLobbyRequest()
        {
        }

        public JoinLobbyRequest(IRpcProtocol protocol, OperationRequest operationRequest)
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

        [DataMember(Code = (byte)ParameterCode.GameCount, IsOptional = true)]
        public int GameListCount { get; set; }
 
        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable GameProperties { get; set; }

        [DataMember(Code = (byte)ParameterCode.LobbyName, IsOptional = true)]
        public string LobbyName { get; set; }

        [DataMember(Code = (byte)ParameterCode.LobbyType, IsOptional = true)]
        public byte LobbyType { get; set; }
    }
}
