// -------------------------------------------------------------------------------------------
// <copyright file="MethodReturnValue.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Сombines an error code with a debug message. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server
{
    /// <summary>
    /// Сombines an error code with a debug message. 
    /// </summary>
    public struct MethodReturnValue
    {
        internal const string DebugMessageOk = "Ok";

        internal const short ErrorCodeOk = 0;

        // The ok value.
        private static readonly MethodReturnValue success = new MethodReturnValue { Error = ErrorCodeOk, Debug = DebugMessageOk };

        // Gets Ok value.
        public static MethodReturnValue Ok { get { return success; } }

        // Gets or sets DebugMessage.
        public string Debug { get; set; }

        // Gets or sets ReturnCode.
        public short Error { get; set; }

        public bool IsOk { get { return this.Error == ErrorCodeOk; } }

        public static MethodReturnValue New(short errorCode, string debug)
        {
            return new MethodReturnValue { Error = errorCode, Debug = debug };
        }
    }
}