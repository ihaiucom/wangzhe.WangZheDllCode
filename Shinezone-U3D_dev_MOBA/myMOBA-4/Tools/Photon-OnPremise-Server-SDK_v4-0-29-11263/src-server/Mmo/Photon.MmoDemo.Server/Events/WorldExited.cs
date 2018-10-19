// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorldExited.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
// Clients receive this event after exiting a world, 
// either because of operation ExitWorld or 
// because another client with the same username executes operation EnterWorld. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Events
{
    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server.Operations;
    using Photon.MmoDemo.Server;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// Clients receive this event after exiting a world, 
    /// either because of operation ExitWorld or 
    /// because another client with the same username executes operation EnterWorld. 
    /// </summary>
    public class WorldExited
    {
        [DataMember(Code = (byte)ParameterCode.WorldName)]
        public string WorldName { get; set; }
    }
}