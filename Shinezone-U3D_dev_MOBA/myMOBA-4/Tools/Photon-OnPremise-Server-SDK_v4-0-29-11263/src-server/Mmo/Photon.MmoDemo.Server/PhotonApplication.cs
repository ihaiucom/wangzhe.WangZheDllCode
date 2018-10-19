// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonApplication.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This ApplicationBase subclass creates MmoPeers and has a CounterSamplePublisher that is used to send diagnostic values to the client.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server
{
    using System;
    using System.IO;

    using ExitGames.Diagnostics.Monitoring;
    using ExitGames.Logging;
    using ExitGames.Logging.Log4Net;

    using log4net.Config;

    using SocketServer;
    using SocketServer.Diagnostics;

    /// <summary>
    /// This ApplicationBase subclass creates MmoPeer and has a CounterSamplePublisher that is used to send diagnostic values to the client.
    /// </summary>
    public class PhotonApplication : ApplicationBase
    {
        /// <summary>
        /// Used to publish diagnostic counters.
        /// </summary>
        public static readonly CounterSamplePublisher CounterPublisher;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        static PhotonApplication()
        {
            CounterPublisher = new CounterSamplePublisher(1);
        }

        public static void Initialize()
        {
            PhotonApplication.registerTypes();            
            ////// counter event channel
            ////Settings.DiagnosticsEventChannel = 2;
            ////Settings.DiagnosticsEventReliability = Reliability.Reliable;
            CounterPublisher.AddCounter(new CpuUsageCounterReader(), "Cpu");
            
            // debug
            ////CounterPublisher.AddCounter("Msg sent", ItemMessage.CounterSend);
            ////CounterPublisher.AddCounter("Msg received", ItemMessage.CounterReceive);
            ////CounterPublisher.AddCounter("Events published", ItemEventMessage.CounterEventSend);

            // almost equal to Event/sec
            ////CounterPublisher.AddCounter("int. event msg rec.", ItemEventMessage.CounterEventReceive);
            CounterPublisher.Start();
        }

        private static void registerTypes()
        {
            Protocol.TryRegisterCustomType(typeof(Photon.MmoDemo.Common.Vector), (byte)Photon.MmoDemo.Common.Protocol.CustomTypeCodes.Vector, Photon.MmoDemo.Common.Protocol.SerializeVector, Photon.MmoDemo.Common.Protocol.DeserializeVector);
            Protocol.TryRegisterCustomType(typeof(Photon.MmoDemo.Common.BoundingBox), (byte)Photon.MmoDemo.Common.Protocol.CustomTypeCodes.BoundingBox, Photon.MmoDemo.Common.Protocol.SerializeBoundingBox, Photon.MmoDemo.Common.Protocol.DeserializeBoundingBox);
        }

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new MmoPeer(initRequest);
        }

        protected override void Setup()
        {
            // log4net
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");
            var configFileInfo = new FileInfo(Path.Combine(this.BinaryPath, "log4net.config"));
            if (configFileInfo.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(configFileInfo);
            }

            AppDomain.CurrentDomain.UnhandledException += AppDomain_OnUnhandledException;

            Initialize();
        }

        protected override void TearDown()
        {
        }

        /// <summary>
        /// Logs unhandled exceptions.
        /// </summary>
        private static void AppDomain_OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error(e.ExceptionObject);
        }
    }
}