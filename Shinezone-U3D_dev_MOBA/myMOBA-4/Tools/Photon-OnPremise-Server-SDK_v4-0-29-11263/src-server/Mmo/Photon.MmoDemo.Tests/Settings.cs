// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Tests
{
    public static class Settings
    {
        public static readonly string ApplicationId = "MmoDemo";

        public static readonly int ConnectTimeoutMilliseconds = 4000;

        public static readonly string ServerAddress = "127.0.0.1:5055";
        // public static readonly string ServerAddress = "127.0.0.1:4530";

        public static readonly bool UseTcp = false;

        public static readonly int WaitTime = 6000;
        public static readonly int WaitTimeMultiOp = 60000;
    }
}