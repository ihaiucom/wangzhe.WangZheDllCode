// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueEvent.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the QueueEvent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Events
{
    #region using directives

    using Photon.LoadBalancing.Operations;
    using Photon.SocketServer.Rpc;

    #endregion

    public class QueueEvent : DataContract
    {
        [DataMember(Code = (byte)ParameterCode.Position)]
        public int Position { get; set; }
    }
}