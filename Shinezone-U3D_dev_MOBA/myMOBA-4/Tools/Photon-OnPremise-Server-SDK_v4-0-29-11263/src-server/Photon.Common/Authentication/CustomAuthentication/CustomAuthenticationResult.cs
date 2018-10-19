// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomAuthenticationResult.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CustomAuthenticationResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Common.Authentication.CustomAuthentication
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public class CustomAuthenticationResult
    { 
        [DataMember(IsRequired = true)]
        public byte ResultCode { get; set; }

        [DataMember(IsRequired = false)]
        public string Message { get; set; }

        [DataMember(IsRequired = false)]
        public string UserId { get; set; }

        [DataMember(IsRequired = false)]
        public string Nickname { get; set; }

        [DataMember(IsRequired = false)]
        public Dictionary<string, object> Data { get; set; }

        [Obsolete("Replaced by AuthCookie - only kept for backwards compatibility")]
        [DataMember(IsRequired = false)]
        public Dictionary<string, object> Secure { get; set; }

        [DataMember(IsRequired = false)]
        public Dictionary<string, object> AuthCookie { get; set; }
    }
}
