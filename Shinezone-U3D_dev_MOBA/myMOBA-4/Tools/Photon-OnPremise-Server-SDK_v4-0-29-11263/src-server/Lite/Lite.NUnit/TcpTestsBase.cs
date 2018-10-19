// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TcpTestsBase.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The tcp tests base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Tests
{
    using System;
    using System.Threading;

    using ExitGames.Logging;

    using NUnit.Framework;

    /// <summary>
    /// The tcp tests base.
    /// </summary>
    public abstract class TcpTestsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpTestsBase"/> class.
        /// </summary>
        protected TcpTestsBase()
        {
            this.WaitTime = 10000;
            this.AutoResetEventInit = new AutoResetEvent(false);
            this.AutoResetEventOperation = new AutoResetEvent(false);
            this.AutoResetEventEvent = new AutoResetEvent(false);
        }

        /// <summary>
        /// Gets or sets AutoResetEventEvent.
        /// </summary>
        protected AutoResetEvent AutoResetEventEvent { get; set; }

        /// <summary>
        /// Gets or sets AutoResetEventInit.
        /// </summary>
        protected AutoResetEvent AutoResetEventInit { get; set; }

        /// <summary>
        /// Gets or sets AutoResetEventOperation.
        /// </summary>
        protected AutoResetEvent AutoResetEventOperation { get; set; }

        /// <summary>
        /// Gets or sets WaitTime.
        /// </summary>
        protected int WaitTime { get; set; }

        /// <summary>
        /// The log elapsed time.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <param name="elapsedTime">
        /// The elapsed time.
        /// </param>
        /// <param name="numItems">
        /// The num items.
        /// </param>
        protected void LogElapsedTime(ILogger logger, string prefix, TimeSpan elapsedTime, long numItems)
        {
            if (logger.IsInfoEnabled)
            {
                logger.InfoFormat(
                    "{0}{1,10:N2} ms = {2,10:N5} ms/item = {3,10:N0} items/s", 
                    prefix, 
                    elapsedTime.TotalMilliseconds, 
                    elapsedTime.TotalMilliseconds / numItems, 
                    1000.0 / elapsedTime.TotalMilliseconds * numItems);
            }
        }

        /// <summary>
        /// The wait for event.
        /// </summary>
        protected void WaitForEvent()
        {
            if (this.AutoResetEventEvent.WaitOne(this.WaitTime) == false)
            {
                Assert.Fail("Didn't received event in expected time.");
            }
        }

        /// <summary>
        /// The wait for init response.
        /// </summary>
        protected void WaitForInitResponse()
        {
            if (this.AutoResetEventInit.WaitOne(this.WaitTime) == false)
            {
                Assert.Fail("Didn't received init response in expected time.");
            }
        }

        /// <summary>
        /// The wait for operation response.
        /// </summary>
        protected void WaitForOperationResponse()
        {
            if (this.AutoResetEventOperation.WaitOne(this.WaitTime) == false)
            {
                Assert.Fail("Didn't received operation response in expected time.");
            }
        }
    }
}