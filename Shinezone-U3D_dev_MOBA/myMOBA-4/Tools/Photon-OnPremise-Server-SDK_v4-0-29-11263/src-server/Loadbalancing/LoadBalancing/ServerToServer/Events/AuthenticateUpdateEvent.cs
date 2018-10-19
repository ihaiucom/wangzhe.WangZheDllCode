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

    public class AuthenticateUpdateEvent : DataContract
    {
        #region Constructors and Destructors

        public AuthenticateUpdateEvent()
        {
        }

        public AuthenticateUpdateEvent(IRpcProtocol protocol, IEventData eventData)
            : base(protocol, eventData.Parameters)
        {
        }

        #endregion

        #region Properties

        [DataMember(Code = (byte)ParameterCode.ApplicationId, IsOptional = false)]
        public string ApplicationId { get; set; }

        [DataMember(Code = (byte)ParameterCode.GameList, IsOptional = false)]
        public bool Data { get; set; }

        [DataMember(Code = 1, IsOptional = true)]
        public bool IsClientAuthenticationRequired { get; set; }

        [DataMember(Code = 2, IsOptional = true)]
        public bool IsClientAuthenticationEnabled { get; set; }

        [DataMember(Code = 3, IsOptional = true)]        
        public bool HasExternalApi { get; set; }

        #endregion
    }
}