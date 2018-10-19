// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RadarUpdate.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Clients receive this event frequently after executing operation RadarSubscribe.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Events
{
    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server.Operations;
    using Photon.SocketServer.Rpc;

    using Photon.MmoDemo.Server;

    /// <summary>
    /// Clients receive this event frequently after executing operation RadarSubscribe.
    /// </summary>
    public class RadarUpdate
    {
        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }

        [DataMember(Code = (byte)ParameterCode.ItemType)]
        public byte ItemType { get; set; }

        [DataMember(Code = (byte)ParameterCode.Position)]
        public Vector Position { get; set; }

        // Remove item if true.
        [DataMember(Code = (byte)ParameterCode.Remove, IsOptional = true)]
        public bool Remove { get; set; }
    }

}