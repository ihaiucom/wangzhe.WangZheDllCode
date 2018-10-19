// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Disconnected.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The disconnected.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.StarDust.Client.ConnectionStates.Lite
{
    internal class Disconnected : ConnectionStateBase
    {
        public static readonly Disconnected Instance = new Disconnected();
        
        // no special methods
    }
}