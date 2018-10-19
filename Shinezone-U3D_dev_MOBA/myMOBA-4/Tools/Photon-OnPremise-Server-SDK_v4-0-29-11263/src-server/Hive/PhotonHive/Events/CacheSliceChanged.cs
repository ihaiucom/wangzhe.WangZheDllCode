// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertiesChangedEvent.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Implementation if the PropertiesChanged event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Events
{
    using System.Collections;

    using Photon.Hive.Operations;

    using Photon.SocketServer.Rpc;

    /// <summary>
    /// Implementation if the PropertiesChanged event.
    /// </summary>
    public class CacheSliceChanged : HiveEventBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheSliceChanged"/> class.
        /// </summary>
        /// <param name="actorNumber">
        /// The actor number.
        /// </param>
        public CacheSliceChanged(int actorNumber)
            : base(actorNumber)
        {
            this.Code = (byte)EventCode.CacheSliceChanged;
        }

        /// <summary>
        /// Gets or sets the slice that changed.
        /// </summary>
        [DataMember(Code = (byte)ParameterKey.CacheSliceIndex)]
        public int Slice { get; set; }
    }
}