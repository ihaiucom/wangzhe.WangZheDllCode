// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateGameEvent.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateGameEvent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Hive.Plugin;

namespace Photon.LoadBalancing.ServerToServer.Events
{
    #region using directives

    using System.Collections;

    using Photon.Hive.Operations;

    using Photon.LoadBalancing.Operations;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    #endregion

    public class UpdateGameEvent : DataContract
    {
        #region Constructors and Destructors

        public UpdateGameEvent()
        {
        }

        public UpdateGameEvent(IRpcProtocol protocol, IEventData eventData)
            : base(protocol, eventData.Parameters)
        {
        }

        #endregion

        #region Properties

        [DataMember(Code = (byte)ParameterCode.PeerCount, IsOptional = true)]
        public byte ActorCount { get; set; }

        [DataMember(Code = (byte)ParameterCode.ApplicationId, IsOptional = true)]
        public string ApplicationId { get; set; }

        [DataMember(Code = (byte)ParameterCode.AppVersion, IsOptional = true)]
        public string ApplicationVersion { get; set; }

        [DataMember(Code = (byte)ParameterCode.GameId, IsOptional = false)]
        public string GameId { get; set; }

        [DataMember(Code = (byte)ParameterCode.LobbyName, IsOptional = true)]
        public string LobbyId { get; set; }

        [DataMember(Code = (byte)ParameterCode.LobbyType, IsOptional = true)]
        public byte LobbyType { get; set; }

        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable GameProperties { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.NewUsers, IsOptional = true)]
        public string[] NewUsers { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.RemovedUsers, IsOptional = true)]
        public string[] RemovedUsers { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.InactiveUsers, IsOptional = true)]
        public string[] InactiveUsers { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.ExcludedUsers, IsOptional = true)]
        public ExcludedActorInfo[] ExcludedUsers { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.ExpectedUsers, IsOptional = true)]
        public string[] ExpectedUsers { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.FailedToAdd, IsOptional = true)]
        public string[] FailedToAdd { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.Reinitialize, IsOptional = true)]
        public bool Reinitialize { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.MaxPlayer, IsOptional = true)]
        public byte? MaxPlayers { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.IsOpen, IsOptional = true)]
        public bool? IsOpen { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.IsVisible, IsOptional = true)]
        public bool? IsVisible { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.LobbyPropertyFilter, IsOptional = true)]
        public object[] PropertyFilter { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.InactiveCount, IsOptional = true)]
        public byte InactiveCount { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.IsPersistent, IsOptional = true)]
        public bool? IsPersistent { get; set; }

        [DataMember(Code = (byte)ServerParameterCode.CheckUserIdOnJoin, IsOptional = true)]
        public bool? CheckUserIdOnJoin { get; set; }


        #endregion
    }
}