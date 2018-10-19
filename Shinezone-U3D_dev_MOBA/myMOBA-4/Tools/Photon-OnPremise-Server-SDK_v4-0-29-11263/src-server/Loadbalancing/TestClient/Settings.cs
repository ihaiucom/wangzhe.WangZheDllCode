// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.TestClient
{
    using System.Configuration;

    public class Settings
    {
        /// <summary>
        ///   The server address.
        /// </summary>
        public static string ServerAddress = ConfigurationManager.AppSettings["ServerAddress"];

        /// <summary>
        ///   The number of clients per game.
        /// </summary>
        public static byte NumClientsPerGame = byte.Parse(ConfigurationManager.AppSettings["NumClientsPerGame"]);

        /// <summary>
        ///   The number of games.
        /// </summary>
        public static byte NumGames = byte.Parse(ConfigurationManager.AppSettings["NumGames"]);

    }
}
