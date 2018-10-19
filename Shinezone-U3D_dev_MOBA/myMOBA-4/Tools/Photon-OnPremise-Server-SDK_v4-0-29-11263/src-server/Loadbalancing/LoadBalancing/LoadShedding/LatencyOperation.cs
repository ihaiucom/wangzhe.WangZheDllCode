// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LatencyOperation.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LatencyOperation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.LoadShedding
{
    using System.Collections.Generic;

    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class LatencyOperation : DataContract
    {
        #region Constructors and Destructors

        public LatencyOperation(IRpcProtocol protocol, Dictionary<byte, object> @params)
            : base(protocol, @params)
        {
        }

        public LatencyOperation()
        {
        }

        #endregion

        #region Properties

        [DataMember(Code = 10)]
        public long? SentTime { get; set; }

        #endregion
    }
}