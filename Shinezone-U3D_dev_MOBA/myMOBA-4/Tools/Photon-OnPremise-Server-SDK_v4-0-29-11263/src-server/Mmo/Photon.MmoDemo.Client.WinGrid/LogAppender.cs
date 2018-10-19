// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogAppender.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The log appender.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client.WinGrid
{
    using System;

    using log4net.Appender;
    using log4net.Core;

    public class LogAppender : AppenderSkeleton
    {
        public static event Action<string> OnLog;

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (OnLog != null)
            {
                OnLog(string.Format("{0,-5} {1}", loggingEvent.GetLoggingEventData().Level, loggingEvent.MessageObject));
            }
        }
    }
}