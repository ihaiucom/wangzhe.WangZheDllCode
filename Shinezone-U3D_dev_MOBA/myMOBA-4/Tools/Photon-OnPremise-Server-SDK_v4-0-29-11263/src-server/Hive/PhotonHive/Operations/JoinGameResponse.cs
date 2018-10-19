// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinGameResponse.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the JoinGameResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;

namespace Photon.Hive.Operations
{
    #region

    using Photon.SocketServer.Rpc;

    #endregion

    public class JoinGameResponse
    {
        #region Properties

        [DataMember(Code = (byte)ParameterKey.Address, IsOptional = false)]
        public string Address { get; set; }

        [DataMember(Code = (byte)ParameterKey.NodeId)]
        public byte NodeId { get; set; }

        public Hashtable GameProperties { get; set; }

        #endregion
    }
}