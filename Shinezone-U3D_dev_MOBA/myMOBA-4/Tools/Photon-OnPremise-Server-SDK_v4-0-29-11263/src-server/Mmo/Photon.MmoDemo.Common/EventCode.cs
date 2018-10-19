// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   All known event codes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Common
{
    /// <summary>
    /// All known event codes.
    /// <summary>
    public enum EventCode : byte
    {
        ItemDestroyed = 1, 

        ItemMoved, 

        ItemPropertiesSet, 

        WorldExited, 

        ItemSubscribed, 

        ItemUnsubscribed, 

        ItemProperties, 

        RadarUpdate,

        CounterData,

        ItemGeneric, 
    }
}