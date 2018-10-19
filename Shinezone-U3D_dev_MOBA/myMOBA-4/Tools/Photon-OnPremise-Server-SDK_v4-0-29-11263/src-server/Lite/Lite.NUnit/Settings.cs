// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Settings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Tests
{
    using System.Configuration;

    /// <summary>
    /// The settings.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Gets the port.
        /// </summary>
        public static int Port
        {
            get
            {
                return UseTcp ? 4530 : 5055;
            }
        }

        /// <summary>
        /// Gets the ServerAddress.
        /// </summary>
        public static string ServerAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["ServerAddress"];
            }
        }

        /// <summary>
        /// Gets a value indicating whether UseTcp.
        /// </summary>
        public static bool UseTcp
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["UseTcp"]);
            }
        }

        /// <summary>
        /// Gets Loops.
        /// </summary>
        public static int Loops
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["Loops"]);
            }
        }

        /// <summary>
        /// Gets Clients.
        /// </summary>
        public static int Clients
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["Clients"]);
            }
        }
    }
}