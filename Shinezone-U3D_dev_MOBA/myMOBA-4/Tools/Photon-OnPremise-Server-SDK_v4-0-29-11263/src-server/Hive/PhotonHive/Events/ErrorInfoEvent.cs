// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisconnectEvent.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Events
{
    using Photon.Hive.Operations;
    using Photon.SocketServer.Rpc;

    public class ErrorInfoEvent : HiveEventBase
    {
        #region Constructors and Destructors

        public ErrorInfoEvent(string error)
            : base(0)
        {
            this.Code = (byte)EventCode.ErrorInfo;
            this.Info = error;
        }

        [DataMember(Code = (byte)ParameterKey.Info, IsOptional = true)]
        public string Info { get; set; }

        #endregion
    }
}