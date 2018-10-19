// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemPropertiesSet.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//  Clients receive this event after executing operation SetProperties.
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
    /// Clients receive this event after executing operation SetProperties.
    /// </summary>
    public class ItemPropertiesSet
    {
        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }

        [DataMember(Code = (byte)ParameterCode.PropertiesRevision)]
        public int PropertiesRevision { get; set; }

        [DataMember(Code = (byte)ParameterCode.PropertiesSet, IsOptional = true)]
        public Hashtable PropertiesSet { get; set; }

        [DataMember(Code = (byte)ParameterCode.PropertiesUnset, IsOptional = true)]
        public ArrayList PropertiesUnset { get; set; }
    }
}