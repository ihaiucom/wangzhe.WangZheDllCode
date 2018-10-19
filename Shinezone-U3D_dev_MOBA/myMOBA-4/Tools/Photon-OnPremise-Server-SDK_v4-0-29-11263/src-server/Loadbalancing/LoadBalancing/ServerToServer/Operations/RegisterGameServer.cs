// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisterGameServer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the parameters which should be send from game server instances to
//   register at the master application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.ServerToServer.Operations
{
    #region using directives

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    #endregion

    /// <summary>
    ///   Defines the parameters which should be send from game server instances to 
    ///   register at the master application.
    /// </summary>
    public class RegisterGameServer : Operation
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "RegisterGameServer" /> class.
        /// </summary>
        /// <param name = "rpcProtocol">
        ///   The rpc Protocol.
        /// </param>
        /// <param name = "operationRequest">
        ///   The operation request.
        /// </param>
        public RegisterGameServer(IRpcProtocol rpcProtocol, OperationRequest operationRequest)
            : base(rpcProtocol, operationRequest)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "RegisterGameServer" /> class.
        /// </summary>
        public RegisterGameServer()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the public game server ip address.
        /// </summary>
        [DataMember(Code = 4, IsOptional = false)]
        public string GameServerAddress { get; set; }

        //[DataMember(Code = 5, IsOptional = true)]
        //public byte LocalNode { get; set; }

        /// <summary>
        ///   Gets or sets a unique server id.
        ///   This id is used to sync reconnects.
        /// </summary>
        [DataMember(Code = 3, IsOptional = false)]
        public string ServerId { get; set; }

        /// <summary>
        ///   Gets or sets the TCP port of the game server instance.
        /// </summary>
        /// <value>The TCP port.</value>
        [DataMember(Code = 2, IsOptional = true)]
        public int? TcpPort { get; set; }

        /// <summary>
        ///   Gets or sets the UDP port of the game server instance.
        /// </summary>
        /// <value>The UDP port.</value>
        [DataMember(Code = 1, IsOptional = true)]
        public int? UdpPort { get; set; }

        /// <summary>
        ///   Gets or sets the port of the game server instance used for WebSocket connections.
        /// </summary>
        [DataMember(Code = 6, IsOptional = true)]
        public int? WebSocketPort { get; set; }

        /// <summary>
        ///   Gets or sets the inital server state of the game server instance.
        /// </summary>
        [DataMember(Code = 7, IsOptional = true)]
        public int ServerState { get; set; }

        /// <summary>
        ///   Gets or sets the port of the game server instance used for rHTTP connections.
        /// </summary>
        [DataMember(Code = 8, IsOptional = true)]
        public int? HttpPort { get; set; }

        /// <summary>
        ///   Gets or sets the port of the game server instance used for secure WebSocket connections.
        /// </summary>
        [DataMember(Code = 9, IsOptional = true)]
        public int? SecureWebSocketPort { get; set; }

        /// <summary>
        ///   Gets or sets the path of the game server application instance used for rHTTP connections.
        /// </summary>
        [DataMember(Code = 10, IsOptional = true)]
        public string HttpPath { get; set; }

        /// <summary>
        ///   Gets or sets the port of the game server instance used for secure WebSocket connections.
        /// </summary>
        [DataMember(Code = 11, IsOptional = true)]
        public int? SecureHttpPort { get; set; }

        /// <summary>
        ///   Gets or sets the public game server ip address.
        /// </summary>
        [DataMember(Code = 12, IsOptional = true)]
        public string GameServerAddressIPv6 { get; set; }


        /// <summary>
        ///   Gets or sets the fully qualified public host name of the game server instance (used for WebSocket connections).
        /// </summary>
        [DataMember(Code = 13, IsOptional = true)]
        public string GameServerHostName { get; set; }
        #endregion
    }
}