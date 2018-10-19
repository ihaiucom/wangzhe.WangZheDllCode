// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticateResponse.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AuthenticateResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Operations
{
    #region

    using Photon.Hive.Operations;
    using Photon.SocketServer.Rpc;

    #endregion

    public class AuthenticateResponse
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the queue position.
        ///   0 = The client passed the waiting queue 
        ///   > 0 = The server is currently full and the client has been enqueued in the waiting queue.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.Position, IsOptional = false)]
        public int QueuePosition { get; set; }

        #endregion
    }
}