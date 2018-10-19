// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReceiverGroup.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Possible groups of receivers for events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Operations
{
    /// <summary>
    ///   Possible groups of receivers for events.
    /// </summary>
    public enum ReceiverGroup
    {
        /// <summary>
        ///   Send to all actors but the sender.
        /// </summary>
        Others = 0, 

        /// <summary>
        ///   Send to all actors including the sender.
        /// </summary>
        All = 1, 

        /// <summary>
        ///   Send to the peer with the lowest actor number.
        /// </summary>
        MasterClient = 2
    }
}