// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateServerEvent.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateServerEvent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.ServerToServer.Events
{
    #region using directives

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    #endregion

    public class UpdateServerEvent : DataContract
    {
        #region Constructors and Destructors

        public UpdateServerEvent(IRpcProtocol protocol, IEventData eventData)
            : base(protocol, eventData.Parameters)
        {
        }

        public UpdateServerEvent()
        {
        }

        #endregion

        #region Properties

        [DataMember(Code = (byte)ServerParameterCode.LoadIndex, IsOptional = false)]
        public byte LoadIndex { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.ServerState, IsOptional = true)]
        public int State { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.PeerCount, IsOptional = false)]
        public int PeerCount { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.GameCount, IsOptional = true)]
        public int GameCount { get; set; }

        #endregion
    }
}