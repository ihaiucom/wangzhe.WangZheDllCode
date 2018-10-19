// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client.WinGrid
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Windows.Forms;

    using ExitGames.Logging;
    using ExitGames.Logging.Log4Net;

    using log4net.Config;
    using Photon.MmoDemo.Common;

    internal static class Program
    {
        public static Settings GetDefaultSettings()
        {
            int boxesVertical = int.Parse(ConfigurationManager.AppSettings["GridTilesVertical"]);
            int boxesHorizontal = int.Parse(ConfigurationManager.AppSettings["GridTilesHorizontal"]);
            int edgeLengthVertical = int.Parse(ConfigurationManager.AppSettings["GridTileHeight"]);
            int edgeLengthHorizontal = int.Parse(ConfigurationManager.AppSettings["GridTileWidth"]);

            int intervalDraw = int.Parse(ConfigurationManager.AppSettings["DrawInterval"]);
            int intervalMove = int.Parse(ConfigurationManager.AppSettings["AutoMoveInterval"]);
            int intervalSend = int.Parse(ConfigurationManager.AppSettings["SendInterval"]);
            int velocity = int.Parse(ConfigurationManager.AppSettings["AutoMoveVelocity"]);
            bool autoMove = string.Compare("true", ConfigurationManager.AppSettings["AutoMove"], true) == 0;
            bool sendReliable = string.Compare("true", ConfigurationManager.AppSettings["SendReliable"], true) == 0;

            bool useTcp = string.Compare("true", ConfigurationManager.AppSettings["PhotonUseTcp"], true) == 0;
            string serverAddress = ConfigurationManager.AppSettings["PhotonServerAddress"];
            string applicationName = ConfigurationManager.AppSettings["PhotonApplicationName"];
            string worldName = ConfigurationManager.AppSettings["WorldName"];

            return new Settings
                {
                    // photon
                    ServerAddress = serverAddress, 
                    UseTcp = useTcp, 
                    ApplicationName = applicationName, 
                    
                    // grid
                    WorldName = worldName, 
                    TileDimensions = new Vector { X = edgeLengthVertical, Y = edgeLengthHorizontal }, 
                    GridSize = new Vector { X = boxesVertical * edgeLengthVertical, Y = boxesHorizontal * edgeLengthHorizontal }, 
                    
                    // game engine
                    DrawInterval = intervalDraw, 
                    AutoMoveInterval = intervalMove, 
                    SendInterval = intervalSend, 
                    SendReliable = sendReliable,
                    AutoMoveVelocity = velocity,
                    AutoMove = autoMove,
                };
        }

        /// <summary>
        /// The main entry point for the application.
        /// <summary>
        [STAThread]
        private static void Main()
        {
            LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            var configFileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));
            XmlConfigurator.Configure(configFileInfo);
            
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new WorldForm());
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
            }
        }
    }
}