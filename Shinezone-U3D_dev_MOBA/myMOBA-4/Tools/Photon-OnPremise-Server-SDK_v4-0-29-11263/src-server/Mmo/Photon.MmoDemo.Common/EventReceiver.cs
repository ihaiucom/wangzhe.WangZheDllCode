// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventReceiver.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   All known event receivers.
//   Used for operation RaiseGenericEvent />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Common
{
    /// <summary>
    /// All known event receivers.
    /// Used for operation RaiseGenericEvent/>.
    /// </summary>
    public enum EventReceiver
    {
        ItemSubscriber = 1, 

        ItemOwner = 2
    }
}