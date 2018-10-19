// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteApplication.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Main photon application. This application is started from the photon server.
//   This class creates <see cref="LitePeer" />s for new clients.
//   Operation dispatch logic is handled by the <see cref="LitePeer" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite
{
    using System.IO;
    
    using ExitGames.Logging;
    using ExitGames.Logging.Log4Net;

    using Lite.Diagnostics;

    using log4net.Config;

    using Photon.SocketServer;
    using Photon.SocketServer.Diagnostics;

    /// <summary>
    /// Main photon application. This application is started from the photon server.
    /// This class creates <see cref="LitePeer"/>s for new clients.
    /// Operation dispatch logic is handled by the <see cref="LitePeer"/>. 
    /// </summary>
    public class LiteApplication : ApplicationBase
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Creates a <see cref="LitePeer"/> to handle <see cref="OperationRequest"/>s.
        /// </summary>
        /// <param name="initRequest">
        /// The initialization request.
        /// </param>
        /// <returns>
        /// A new <see cref="LitePeer"/> instance.
        /// </returns>
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new LitePeer(initRequest);
        }

        /// <summary>
        /// Application initializtion.
        /// </summary>
        protected override void Setup()
        {
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");

            // log4net
            string path = Path.Combine(this.BinaryPath, "log4net.config");
            var file = new FileInfo(path);
            if (file.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(file);
            }

            log.InfoFormat("Created application Instance: type={0}", Instance.GetType());

            Initialize();
        }

        protected void Initialize()
        {
            // counters for the photon dashboard
            CounterPublisher.DefaultInstance.AddStaticCounterClass(typeof(Counter), "Lite");

            Protocol.AllowRawCustomValues = true;
        }

        /// <summary>
        /// Called when the server shuts down.
        /// </summary>
        protected override void TearDown()
        {
        }
    }
}