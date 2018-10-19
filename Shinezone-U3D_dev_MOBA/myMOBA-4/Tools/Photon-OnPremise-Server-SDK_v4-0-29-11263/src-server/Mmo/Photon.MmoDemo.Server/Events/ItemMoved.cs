// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemMoved.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Clients receive this event after executing operation Move.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Events
{
    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server.Operations;
    using Photon.SocketServer.Rpc;

    using Photon.MmoDemo.Server;

    /// <summary>
    /// Clients receive this event after executing operation Move.
    /// </summary>
    public class ItemMoved
    {
        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }

        [DataMember(Code = (byte)ParameterCode.OldPosition)]
        public Vector OldPosition { get; set; }

        [DataMember(Code = (byte)ParameterCode.Position)]
        public Vector Position { get; set; }

        [DataMember(Code = (byte)ParameterCode.Rotation, IsOptional = true)]
        public Vector Rotation { get; set; }

        [DataMember(Code = (byte)ParameterCode.OldRotation, IsOptional = true)]
        public Vector OldRotation { get; set; }
    }
}