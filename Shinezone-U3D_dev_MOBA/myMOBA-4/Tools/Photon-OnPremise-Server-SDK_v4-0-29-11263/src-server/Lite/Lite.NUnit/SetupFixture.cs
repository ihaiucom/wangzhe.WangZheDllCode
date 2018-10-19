// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetupFixture.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The setup fixture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite.Tests
{
    using System.IO;

    using ExitGames.Logging;
    using ExitGames.Logging.Log4Net;

    using log4net.Config;

    using NUnit.Framework;

    /// <summary>
    /// The setup fixture.
    /// </summary>
    [SetUpFixture]
    public class SetupFixture
    {
        /// <summary>
        /// The setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            var fileInfo = new FileInfo("log4net.config");
            XmlConfigurator.Configure(fileInfo);
        }
    }
}