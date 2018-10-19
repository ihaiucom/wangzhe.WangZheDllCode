// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameListUpdateEvent.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GameListEvent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Events
{
    #region using directives

    using System.Collections;

    using Photon.LoadBalancing.Operations;
    using Photon.SocketServer.Rpc;

    #endregion

    public class GameListUpdateEvent 
    {
        [DataMember(Code = (byte)ParameterCode.GameList)]
        public Hashtable Data { get; set; }
    }
}