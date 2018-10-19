// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyEchoResponse.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MyEchoResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MyApplication.Operations
{
    #region

    using Photon.SocketServer.Rpc;

    #endregion

    public class MyEchoResponse
    {
        [DataMember(Code = (byte)MyParameterCodes.Response, IsOptional = false)]
        public string Response { get; set; }
    }
}