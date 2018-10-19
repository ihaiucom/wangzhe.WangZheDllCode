// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameMessageCodes.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   GameMessagCodes define the type of a "LiteGame" Message, the meaning and its content.
//   Messages are used to communicate async with rooms and games.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Messages
{
    /// <summary>
    /// GameMessagCodes define the type of a "LiteGame" Message, the meaning and its content.
    /// Messages are used to communicate async with rooms and games.
    /// </summary>
    public enum GameMessageCodes : byte
    {
        /// <summary>
        /// Message is an operatzion.
        /// </summary>
        Operation = 0,

        /// <summary>
        /// Message to remove peer from game.
        /// </summary>
        RemovePeerFromGame = 1,
    }
}