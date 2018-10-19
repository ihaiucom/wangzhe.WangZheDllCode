// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReturnCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   All known error codes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Common
{
    /// <summary>
    /// All known error codes.
    /// <summary>
    public enum ReturnCode
    {
        Ok = 0, 

        Fatal = 1, 

        ParameterOutOfRange = 51, 

        OperationNotSupported, 

        InvalidOperationParameter, 

        InvalidOperation, 

        ItemAccessDenied, 

        InterestAreaNotFound, 

        InterestAreaAlreadyExists, 

        WorldAlreadyExists = 101, 

        WorldNotFound, 

        ItemAlreadyExists, 

        ItemNotFound
    }
}