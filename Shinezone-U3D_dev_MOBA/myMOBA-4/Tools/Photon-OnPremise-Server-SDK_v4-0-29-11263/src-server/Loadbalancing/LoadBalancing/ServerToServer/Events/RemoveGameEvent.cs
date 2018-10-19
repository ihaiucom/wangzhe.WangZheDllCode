// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveGameEvent.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoveGameEvent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.ServerToServer.Events
{
    #region using directives

    using Photon.LoadBalancing.Operations;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    #endregion

    public class RemoveGameEvent : DataContract
    {
        #region Constructors and Destructors

        public RemoveGameEvent()
        {
        }

        public RemoveGameEvent(IRpcProtocol protocol, IEventData eventData)
            : base(protocol, eventData.Parameters)
        {
        }

        #endregion

        #region Properties

        [DataMember(Code = (byte)ParameterCode.ApplicationId, IsOptional = true)]
        public string ApplicationId { get; set; }

        [DataMember(Code = (byte)ParameterCode.AppVersion, IsOptional = true)]
        public string ApplicationVersion { get; set; }

        [DataMember(Code = (byte)ParameterCode.GameId, IsOptional = false)]
        public string GameId { get; set; }

        #endregion
    }
}