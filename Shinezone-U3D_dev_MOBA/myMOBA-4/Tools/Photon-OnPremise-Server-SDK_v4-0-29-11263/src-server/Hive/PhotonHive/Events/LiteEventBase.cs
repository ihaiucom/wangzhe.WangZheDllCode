// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteEventBase.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Base class implementation for all Lite events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Events
{
    using System;
    using Photon.Hive.Operations;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// Base class implementation for all Lite events.
    /// </summary>
    [Serializable]
    public abstract class HiveEventBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HiveEventBase"/> class. 
        /// </summary>
        /// <param name="actorNr">
        /// Actor number.
        /// </param>
        protected HiveEventBase(int actorNr)
        {
            this.ActorNr = actorNr;
        }

        /// <summary>
        /// Gets or sets the actor number of the sender.
        /// </summary>
        /// <value>The actor nr.</value>
        [DataMember(Code = (byte)ParameterKey.ActorNr)]
        public int ActorNr { get; set; }

        /// <summary>
        /// Gets or sets the event code.
        /// </summary>
        /// <value>The event code.</value>
        public byte Code { get; set; }
    }
}