// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertiesChangedEvent.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Implementation if the PropertiesChanged event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Events
{
    using System.Collections;

    using Lite.Operations;

    using Photon.SocketServer.Rpc;

    /// <summary>
    /// Implementation if the PropertiesChanged event.
    /// </summary>
    public class PropertiesChangedEvent : LiteEventBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesChangedEvent"/> class.
        /// </summary>
        /// <param name="actorNumber">
        /// The actor number.
        /// </param>
        public PropertiesChangedEvent(int actorNumber)
            : base(actorNumber)
        {
            this.Code = (byte)EventCode.PropertiesChanged;
        }

        /// <summary>
        /// Gets or sets Properties.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.Properties)]
        public Hashtable Properties { get; set; }

        /// <summary>
        /// Gets or sets the number of the actor whos properties have been changed.
        /// A value of 0 indactes that game properties have been changed.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.TargetActorNr)]
        public int TargetActorNumber { get; set; }
    }
}