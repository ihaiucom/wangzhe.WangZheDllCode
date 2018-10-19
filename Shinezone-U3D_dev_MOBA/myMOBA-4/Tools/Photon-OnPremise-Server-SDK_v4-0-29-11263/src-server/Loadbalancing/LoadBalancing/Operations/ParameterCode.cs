// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ParameterCode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Operations
{
    public enum ParameterCode : byte
    {
        // parameters inherited from lite
        GameId = Photon.Hive.Operations.ParameterKey.GameId,
        ActorNr = Photon.Hive.Operations.ParameterKey.ActorNr,
        TargetActorNr = Photon.Hive.Operations.ParameterKey.TargetActorNr,
        Actors = Photon.Hive.Operations.ParameterKey.Actors,
        Properties = Photon.Hive.Operations.ParameterKey.Properties,
        Broadcast = Photon.Hive.Operations.ParameterKey.Broadcast,
        ActorProperties = Photon.Hive.Operations.ParameterKey.ActorProperties,
        GameProperties = Photon.Hive.Operations.ParameterKey.GameProperties,
        Cache = Photon.Hive.Operations.ParameterKey.Cache,
        ReceiverGroup = Photon.Hive.Operations.ParameterKey.ReceiverGroup,
        Data = Photon.Hive.Operations.ParameterKey.Data,
        Code = Photon.Hive.Operations.ParameterKey.Code,
        Flush = Photon.Hive.Operations.ParameterKey.Flush,
        DeleteCacheOnLeave = Photon.Hive.Operations.ParameterKey.DeleteCacheOnLeave,
        Group = Photon.Hive.Operations.ParameterKey.Group,
        GroupsForRemove = Photon.Hive.Operations.ParameterKey.GroupsForRemove,
        GroupsForAdd = Photon.Hive.Operations.ParameterKey.GroupsForAdd,
        SuppressRoomEvents = Photon.Hive.Operations.ParameterKey.SuppressRoomEvents,
        EmptyRoomLiveTime = Photon.Hive.Operations.ParameterKey.EmptyRoomLiveTime,

        // load balancing project specific parameters
        Address = Photon.Hive.Operations.ParameterKey.Address,
        PeerCount = Photon.Hive.Operations.ParameterKey.PeerCount,
        GameCount = Photon.Hive.Operations.ParameterKey.GameCount,
        MasterPeerCount = Photon.Hive.Operations.ParameterKey.MasterPeerCount,
        UserId = Photon.Hive.Operations.ParameterKey.UserId,
        ApplicationId = Photon.Hive.Operations.ParameterKey.ApplicationId,
        Position = Photon.Hive.Operations.ParameterKey.Position,
        MatchMakingType = Photon.Hive.Operations.ParameterKey.Position,
        GameList = Photon.Hive.Operations.ParameterKey.GameList,
        Token = Photon.Hive.Operations.ParameterKey.Token,
        AppVersion = Photon.Hive.Operations.ParameterKey.AppVersion,
        NodeId = Photon.Hive.Operations.ParameterKey.NodeId,
        Info = Photon.Hive.Operations.ParameterKey.Info,
        ClientAuthenticationType = Photon.Hive.Operations.ParameterKey.ClientAuthenticationType,
        ClientAuthenticationParams = Photon.Hive.Operations.ParameterKey.ClientAuthenticationParams,
        CreateIfNotExists = Photon.Hive.Operations.ParameterKey.CreateIfNotExists,
        JoinType = Photon.Hive.Operations.ParameterKey.JoinMode,
        ClientAuthenticationData = Photon.Hive.Operations.ParameterKey.ClientAuthenticationData,
        LobbyName = Photon.Hive.Operations.ParameterKey.LobbyName,
        LobbyType = Photon.Hive.Operations.ParameterKey.LobbyType,
        LobbyStats = Photon.Hive.Operations.ParameterKey.LobbyStats,
        Region = Photon.Hive.Operations.ParameterKey.Region,
        Nickname = Photon.Hive.Operations.ParameterKey.Nickname,
        PluginName = Photon.Hive.Operations.ParameterKey.PluginName,
        PluginVersion = Photon.Hive.Operations.ParameterKey.PluginVersion,
        AddUsers = Photon.Hive.Operations.ParameterKey.AddUsers,
    }
}