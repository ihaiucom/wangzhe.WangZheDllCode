// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemGeneric.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Clients receive this event after executing operation RaiseGenericEvent />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Events
{
    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server.Operations;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// Clients receive this event after executing operation RaiseGenericEvent/>.
    /// </summary>
    public class ItemGeneric
    {
        [DataMember(Code = (byte)ParameterCode.CustomEventCode)]
        public byte CustomEventCode { get; set; }

        [DataMember(Code = (byte)ParameterCode.EventData, IsOptional = true)]
        public object EventData { get; set; }

        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }
    }
}