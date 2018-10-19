// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomAuthenticationResultCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CustomAuthenticationResultCode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Common.Authentication.CustomAuthentication
{
    public class CustomAuthenticationResultCode
    {
        // return temporary data 
        public const byte Data = 0;

        public const byte Ok = 1;

        public const byte Failed = 2;

        public const byte ParameterInvalid = 3;
    }
}
