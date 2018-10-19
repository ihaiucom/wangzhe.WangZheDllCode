// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessage.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Messages
{
    /// <summary>
    /// Interface of a message.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets the action.
        /// </summary>
        byte Action { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        object Message { get; }
    }
}