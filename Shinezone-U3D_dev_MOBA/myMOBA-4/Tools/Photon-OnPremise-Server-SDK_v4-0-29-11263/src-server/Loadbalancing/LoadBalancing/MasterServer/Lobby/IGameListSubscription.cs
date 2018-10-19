// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGameListSubscibtion.cs" company="">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.MasterServer.Lobby
{
    using System.Collections;

    public interface IGameListSubscription
    {
        Hashtable GetGameList();
    }
}
