// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RpcResponse.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the RpcResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.WebRpc
{
    internal class WebRpcResponse
    {
        public byte ResultCode;

        public string Message;

        //public Dictionary<string, object> Data;
        public object Data;

        public override string ToString()
        {
            return string.Format("[ResultCode:{0};Message:'{1}']", this.ResultCode, this.Message);
        }
    }
}
