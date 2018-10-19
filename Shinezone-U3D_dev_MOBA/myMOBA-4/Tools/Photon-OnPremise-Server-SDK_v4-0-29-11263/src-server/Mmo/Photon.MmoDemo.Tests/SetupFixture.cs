// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetupFixture.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   setup fixture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Tests
{
    using System.IO;

    using ExitGames.Logging;
    using ExitGames.Logging.Log4Net;

    using log4net.Config;

    using NUnit.Framework;

    using Photon.MmoDemo.Server;
    using Photon.MmoDemo.Tests.Disconnected;

    [SetUpFixture]
    public class SetupFixture
    {
        #region Constructors and Destructors

        public SetupFixture()
        {
            LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            var fi = new FileInfo("..\\..\\log4net.config");
            XmlConfigurator.ConfigureAndWatch(fi);

            LogManager.GetCurrentClassLogger().Info("SETUP");
            PhotonApplication.Initialize();
        }

        #endregion

        #region Methods

        private static void Main(params string[] args)
        {
            var setup = new SetupFixture();
            var test = new HeavyLoad();
            test.RunForTime();
        }

        #endregion
    }
}