// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetPropertiesResponse.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GetPropertiesResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Operations
{
    #region

    using System.Collections;

    using Photon.SocketServer.Rpc;

    #endregion

    /// <summary>
    ///   Response for <see cref = "GetPropertiesRequest" />.
    /// </summary>
    public class GetPropertiesResponse
    {
        #region Properties

        /// <summary>
        ///   Gets or sets ActorProperties.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.ActorProperties, IsOptional = true)]
        public Hashtable ActorProperties { get; set; }

        /// <summary>
        ///   Gets or sets GameProperties.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable GameProperties { get; set; }

        #endregion
    }
}