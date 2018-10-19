// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntry.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Diagnostics.OperationLogging
{
    using System;

    public class LogEntry
    {
        public DateTime UtcCreated { get; set; }

        public string Action { get; set; }

        public string Message { get; set; }

        public LogEntry(DateTime utcCreated, string action, string message)
        {
            this.Action = action;
            this.UtcCreated = utcCreated;
            this.Message = message;   
        }

        public LogEntry(string action, string message)
            : this (DateTime.UtcNow, action, message)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}: {2}", this.UtcCreated, this.Action, this.Message); 
        }
    }
}
