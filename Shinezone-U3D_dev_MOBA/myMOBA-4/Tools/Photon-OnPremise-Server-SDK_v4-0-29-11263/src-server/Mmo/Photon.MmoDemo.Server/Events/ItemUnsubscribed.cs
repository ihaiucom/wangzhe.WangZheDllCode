// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemUnsubscribed.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Client receive this event when an item exits an interest area.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Events
{
    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// Client receive this event when an item exits an interest area.
    /// </summary>
    public class ItemUnsubscribed
    {
        [DataMember(Code = (byte)ParameterCode.InterestAreaId)]
        public byte InterestAreaId { get; set; }

        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }
    }
}