// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyType.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The property type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Operations
{
    using System;

    /// <summary>
    ///   The property type.
    /// </summary>
    [Flags]
    public enum PropertyType : byte
    {
        /// <summary>
        ///   The none.
        /// </summary>
        None = 0x00, 

        /// <summary>
        ///   The game.
        /// </summary>
        Game = 0x01, 

        /// <summary>
        ///   The actor.
        /// </summary>
        Actor = 0x02, 

        /// <summary>
        ///   The game and actor.
        /// </summary>
        GameAndActor = Game | Actor
    }
}