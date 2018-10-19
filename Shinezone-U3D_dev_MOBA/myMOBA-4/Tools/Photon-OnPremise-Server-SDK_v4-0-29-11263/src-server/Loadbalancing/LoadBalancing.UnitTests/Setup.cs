
namespace Photon.LoadBalancing.UnitTests
{
    using System;
    using System.IO;

    using ExitGames.Logging.Log4Net;

    using log4net;
    using log4net.Config;

    using NUnit.Framework;

    using LogManager = ExitGames.Logging.LogManager;

    [SetUpFixture]
    public class SetupFixture
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Initializeing log4net ..");
            LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            GlobalContext.Properties["LogName"] = "TestLog.log";
            XmlConfigurator.ConfigureAndWatch(new FileInfo("tests_log4net.config"));
        }
    }
}
