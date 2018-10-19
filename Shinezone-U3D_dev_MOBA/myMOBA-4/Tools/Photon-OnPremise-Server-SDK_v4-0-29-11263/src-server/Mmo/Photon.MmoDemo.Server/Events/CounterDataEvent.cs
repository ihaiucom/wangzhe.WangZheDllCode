// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CounterDataEvent.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Clients receive this event after executing operation SubscribeCounter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server.Events
{
    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server.Operations;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// Clients receive this event after executing operation SubscribeCounter.
    /// </summary>
    public class CounterDataEvent
    {
        [DataMember(Code = (byte)ParameterCode.CounterName)]
        public string Name { get; set; }

        [DataMember(Code = (byte)ParameterCode.CounterTimeStamps)]
        public long[] TimeStamps { get; set; }

        [DataMember(Code = (byte)ParameterCode.CounterValues)]
        public float[] Values { get; set; }
    }
}