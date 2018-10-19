// -----------------------------------------------------------------------
// <copyright file="ServerState.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
// </summary>
// -----------------------------------------------------------------------

namespace Photon.Common.LoadBalancer.Common
{
    public enum ServerState
    {
        Normal = 0,

        OutOfRotation = 1,

        Offline = 2
    }
}