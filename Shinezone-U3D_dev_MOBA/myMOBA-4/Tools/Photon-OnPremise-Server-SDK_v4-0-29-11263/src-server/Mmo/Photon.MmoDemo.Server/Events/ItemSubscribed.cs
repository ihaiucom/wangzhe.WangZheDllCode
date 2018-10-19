// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemSubscribed.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Client receives this event when an item enters interest an area.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Events
{
    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server.Operations;
    using Photon.MmoDemo.Server;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// Client receives this event when an item enters interest an area.
    /// </summary>
    public class ItemSubscribed
    {
        [DataMember(Code = (byte)ParameterCode.InterestAreaId)]
        public byte InterestAreaId { get; set; }

        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }

        [DataMember(Code = (byte)ParameterCode.ItemType)]
        public byte ItemType { get; set; }

        [DataMember(Code = (byte)ParameterCode.Position)]
        public Vector Position { get; set; }

        [DataMember(Code = (byte)ParameterCode.Rotation, IsOptional = true)]
        public Vector Rotation { get; set; }

        [DataMember(Code = (byte)ParameterCode.PropertiesRevision)]
        public int PropertiesRevision { get; set; }
    }
}