//-----------------------------------------------------------------------------
// <copyright file="RoomMessage.cs" company="Exit Games GmbH">
//     Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
// RoomMessage implementation.
// </summary>
//-----------------------------------------------------------------------------

namespace Photon.Hive.Messages
{
    /// <summary>
    /// Represents a message for rooms.
    /// </summary>
    public class RoomMessage : IMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomMessage"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public RoomMessage(byte action)
        {
            this.Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomMessage"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="message">The message.</param>
        public RoomMessage(byte action, object message)
            : this(action)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public object Message { get; private set; }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>The action.</value>
        public byte Action { get; private set; }
    }
}