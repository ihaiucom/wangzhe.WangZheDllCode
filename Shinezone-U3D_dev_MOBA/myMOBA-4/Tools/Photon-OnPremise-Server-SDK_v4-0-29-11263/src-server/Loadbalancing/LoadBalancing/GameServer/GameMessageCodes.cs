// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameMessageCodes.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.GameServer
{
    /// <summary>
    /// GameMessagCodes define the type of a "Game" Message, the meaning and its content. Contains the enum values of the Lite GameMessageCode enum and extended Loadbalancing GameMessageCodes.
    /// Messages are used to communicate async with rooms and games.
    /// </summary>
    public enum GameMessageCodes
    {
        #region Lite Codes
        /// <summary>
        /// Message is an operatzion.
        /// </summary>
        Operation = 0,

        /// <summary>
        /// Message to remove peer from game.
        /// </summary>
        RemovePeerFromGame = 1,

        #endregion

        #region Loadbalancing codes
        /// <summary>
        /// Message to signal the state of the game to the Master Server.
        /// </summary>
        ReinitializeGameStateOnMaster = 2,

        /// <summary>
        /// Message to validate the game state 
        /// </summary>
        CheckGame = 3,

        /// <summary>
        /// Send to rooms to inform the peers that the server will go offline
        /// </summary>
        RaiseOfflineEvent = 4


        #endregion
    }
}
