// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActorParameter.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ActorParameter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Operations
{
    /// <summary>
    /// Well known actor properties (used as byte keys in actor-property hashtables).
    /// </summary>
    public enum ActorParameter : byte
    {
        Nickname = 255,
        IsInactive = 254,
        UserId = 253,
    }
}