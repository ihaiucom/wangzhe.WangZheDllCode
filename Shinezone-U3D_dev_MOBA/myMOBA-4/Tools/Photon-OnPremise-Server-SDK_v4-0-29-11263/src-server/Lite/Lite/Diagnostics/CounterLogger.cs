// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CounterLogger.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CounterLogger type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Diagnostics
{
    using System;
    using System.Threading;

    using ExitGames.Logging;

    /// <summary>
    /// Logs the most intersting counters into a log file
    /// </summary>
    public class CounterLogger : IDisposable
    {
        /// <summary>
        /// Get logger for the counter log file.
        /// </summary>
        private static readonly ILogger counterLog = LogManager.GetLogger("PerformanceCounter");

        /// <summary>
        /// Get logger for debug out.
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Log interval is set to 5 seconds.
        /// </summary>
        private const int LogIntervalMs = 5000;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static readonly CounterLogger instance = new CounterLogger();

        // ReSharper disable UnaccessedField.Local

        /// <summary>
        /// Timer used to trigger log output
        /// </summary>
        private Timer timer;

        // ReSharper restore UnaccessedField.Local

        /// <summary>
        /// Prevents a default instance of the <see cref="CounterLogger"/> class from being created.
        /// </summary>
        private CounterLogger()
        {
        }

        /// <summary>
        /// Gets an sigelton instance of the <see cref="CounterLogger"/> class.
        /// </summary>
        /// <value>The instance.</value>
        public static CounterLogger Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Starts the log output.
        /// </summary>
        public void Start()
        {
            if (counterLog.IsDebugEnabled)
            {
                if (log.IsInfoEnabled)
                {
                    log.Info("Starting counter logger");
                }

                //var callback = new TimerCallback(LogCounter);
                //this.timer = new Timer(callback, null, 0, LogIntervalMs);
            }
            else
            {
                if (log.IsInfoEnabled)
                {
                    log.Info("Counter logger not started.");
                }
            }
        }

        /// <summary>
        /// Callback to write counter values to a log counter.
        /// </summary>
        /// <param name="state">State value.</param>
        //private static void LogCounter(object state)
        //{
        //    counterLog.InfoFormat(
        //        "# Sessions = {0:N2}", Photon.SocketServer.Diagnostics.PhotonCounter.SessionCount.GetNextValue());
        //    counterLog.InfoFormat(
        //        "OperationReceive/s = {0:N2}",
        //        Photon.SocketServer.Diagnostics.PhotonCounter.OperationReceivePerSec.GetNextValue());
        //    counterLog.InfoFormat(
        //        "OperationsResponse/s = {0:N2}",
        //        Photon.SocketServer.Diagnostics.PhotonCounter.OperationResponsePerSec.GetNextValue());
        //    counterLog.InfoFormat(
        //        "EventsSent/s = {0:N2}",
        //        Photon.SocketServer.Diagnostics.PhotonCounter.EventSentPerSec.GetNextValue());
        //    counterLog.InfoFormat(
        //        "Average Execution Time = {0:N2}\n",
        //        Photon.SocketServer.Diagnostics.PhotonCounter.AverageOperationExecutionTime.GetNextValue());
        //}

        public void Dispose()
        {
            var t = this.timer;
            if (t != null)
            {
                t.Dispose();
                this.timer = null;
            }
        }
    }
}