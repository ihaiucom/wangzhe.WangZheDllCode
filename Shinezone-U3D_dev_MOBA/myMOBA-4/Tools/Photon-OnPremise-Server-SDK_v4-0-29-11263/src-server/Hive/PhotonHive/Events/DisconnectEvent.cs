// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisconnectEvent.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Events
{
    using Photon.Hive.Operations;

    public class DisconnectEvent : HiveEventBase
    {
        #region Constructors and Destructors

        public DisconnectEvent(int actorNr)
            : base(actorNr)
        {
            this.Code = (byte)EventCode.Disconnect;
        }

        #endregion
    }
}