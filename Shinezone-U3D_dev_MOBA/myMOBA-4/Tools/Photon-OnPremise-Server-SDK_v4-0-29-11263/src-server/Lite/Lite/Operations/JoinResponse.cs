// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JoinResponse.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the JoinResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Operations
{
    #region

    using System.Collections;

    using Photon.SocketServer.Rpc;

    #endregion

    /// <summary>
    ///   Response for <see cref = "JoinRequest" />.
    /// </summary>
    public class JoinResponse
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the actor number for the joined player.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.ActorNr)]
        public int ActorNr { get; set; }

        /// <summary>
        ///   Gets or sets the current actor properties for all existing actors in the game
        ///   that will be returned to the client in the operation response.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.ActorProperties, IsOptional = true)]
        public Hashtable CurrentActorProperties { get; set; }

        /// <summary>
        ///   Gets or sets the current game properties that will be returned 
        ///   to the client in the operation response.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.GameProperties, IsOptional = true)]
        public Hashtable CurrentGameProperties { get; set; }

        #endregion
    }
}