// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemProperties.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The client receives this event after executing operation GetProperties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Events
{
    using System.Collections;

    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server.Operations;
    using Photon.MmoDemo.Server;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// The client receives this event after executing operation GetProperties.
    /// </summary>
    public class ItemProperties
    {
        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }

        [DataMember(Code = (byte)ParameterCode.PropertiesRevision)]
        public int PropertiesRevision { get; set; }

        [DataMember(Code = (byte)ParameterCode.PropertiesSet, IsOptional = true)]
        public Hashtable PropertiesSet { get; set; }
    }
}