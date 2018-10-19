// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateApplStatsEvent.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateApplicationStatsEvent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.ServerToServer.Events
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class UpdateAppStatsEvent : DataContract
    {
        public UpdateAppStatsEvent()
        {
        }

        public UpdateAppStatsEvent(IRpcProtocol protocol, IEventData eventData)
            : base(protocol, eventData.Parameters)
        {
        }

        [DataMember(Code = 0, IsOptional = false)]
        public int GameCount { get; set; }

        [DataMember(Code = 1, IsOptional = false)]
        public int PlayerCount { get; set; }      
    }
}