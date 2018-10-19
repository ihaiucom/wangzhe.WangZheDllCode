// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Application.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.CounterPublisher
{
    #region using directives

    using System.IO;

    using ExitGames.Logging;
    using ExitGames.Logging.Log4Net;

    using log4net.Config;

    using Photon.SocketServer;
    using Photon.SocketServer.Diagnostics;

    #endregion

    /// <summary>
    ///   The application.
    /// </summary>
    public class Application : ApplicationBase
    {
        /// <summary>
        ///   Returns null: No connections allowed.
        /// </summary>
        /// <param name = "initRequest">
        ///   The initialization request sent by the peer.
        /// </param>
        /// <returns>
        ///   Always null.
        /// </returns>
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return null;
        }

        /// <summary>
        ///   Provides SystemCounter and SocketServerCounter by default.
        /// </summary>
        protected override void Setup()
        {
            // log4net
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");
            string path = Path.Combine(this.BinaryPath, "log4net.config");
            var file = new FileInfo(path);
            if (file.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(file);
            }
            
            CounterPublisher.DefaultInstance.AddStaticCounterClass(typeof(SystemCounter), "System");
            CounterPublisher.DefaultInstance.AddStaticCounterClass(typeof(SocketServerCounter), "SocketServer");
        }

        /// <summary>
        ///   The tear down.
        /// </summary>
        protected override void TearDown()
        {
        }
    }
}