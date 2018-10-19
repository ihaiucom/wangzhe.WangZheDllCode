// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedbackControlSystem.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FeedbackControlSystem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using ExitGames.Logging;
using Photon.Common.LoadBalancer.LoadShedding.Configuration;

namespace Photon.Common.LoadBalancer.LoadShedding
{
    internal sealed class FeedbackControlSystem : IFeedbackControlSystem, IDisposable
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        
        private readonly int maxCcu;

        private readonly string applicationRootPath; 

        private readonly FileSystemWatcher fileWatcher;

        private FeedbackControllerCollection controllerCollection;
        
        #endregion

        #region Constructors and Destructors

        public FeedbackControlSystem(int maxCcu, string applicationRootPath, string workLoadConfigFile)
        {
            this.maxCcu = maxCcu;
            this.applicationRootPath = applicationRootPath;

            this.Initialize(workLoadConfigFile);

            if (!string.IsNullOrEmpty(applicationRootPath) && Directory.Exists(applicationRootPath))
            {
                this.fileWatcher = new FileSystemWatcher(applicationRootPath, workLoadConfigFile);
                this.fileWatcher.Changed += this.ConfigFileChanged;
                this.fileWatcher.Created += this.ConfigFileChanged;
                this.fileWatcher.Deleted += this.ConfigFileChanged;
                this.fileWatcher.Renamed += this.ConfigFileChanged;
                this.fileWatcher.EnableRaisingEvents = true;
            }
        }

        #endregion

        #region Properties

        public FeedbackLevel Output
        {
            get
            {
                return this.controllerCollection.Output;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IFeedbackControlSystem

        public void SetBandwidthUsage(int bytes)
        {
            this.controllerCollection.SetInput(FeedbackName.Bandwidth, bytes);
        }

        public void SetCpuUsage(int cpuUsage)
        {
            this.controllerCollection.SetInput(FeedbackName.CpuUsage, cpuUsage);
        }

        public void SetOutOfRotation(bool isOutOfRotation)
        {
            this.controllerCollection.SetInput(FeedbackName.OutOfRotation, isOutOfRotation ? 1 : 0);
        }

        public void SetPeerCount(int peerCount)
        {
            this.controllerCollection.SetInput(FeedbackName.PeerCount, peerCount);
        }
        #endregion

        #endregion

        #region Methods

        private static List<FeedbackController> GetNonConfigurableControllers(int maxCcu)
        {
            Dictionary<FeedbackLevel, int> peerCountThresholds = maxCcu == 0
                                                                     ? new Dictionary<FeedbackLevel, int>()
                                                                     : new Dictionary<FeedbackLevel, int> {
                                                                             { FeedbackLevel.Lowest, 1 }, 
                                                                             { FeedbackLevel.Low, 2 }, 
                                                                             { FeedbackLevel.Normal, maxCcu / 2 }, 
                                                                             { FeedbackLevel.High, maxCcu * 8 / 10 }, 
                                                                             { FeedbackLevel.Highest, maxCcu }
                                                                         };
            var peerCountController = new FeedbackController(FeedbackName.PeerCount, peerCountThresholds, 0, FeedbackLevel.Lowest);

            return new List<FeedbackController> { peerCountController };
        }

        private void ConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            log.InfoFormat("Configuration file for Feedback Control System {0}\\{1} {2}. Reinitializing...", e.FullPath, e.Name, e.ChangeType);
            this.Initialize(e.Name);
        }

        private void Initialize(string workLoadConfigFile)
        {
            // CCU, Out-of-Rotation
            var allControllers = GetNonConfigurableControllers(this.maxCcu);

            // try to load feedback controllers from file: 

            string message;
            FeedbackControlSystemSection section;
            string filename = Path.Combine(this.applicationRootPath, workLoadConfigFile);
            
            if (!ConfigurationLoader.TryLoadFromFile(filename, out section, out message))
            {
                log.WarnFormat(
                    "Could not initialize Feedback Control System from configuration: Invalid configuration file {0}. Using default settings... ({1})", 
                    filename, 
                    message);
            }

            if (section != null)
            {
                // load controllers from config file.);
                foreach (FeedbackControllerElement controllerElement in section.FeedbackControllers)
                {
                    var dict = new Dictionary<FeedbackLevel, int>();
                    foreach (FeedbackLevelElement level in controllerElement.Levels)
                    {
                        dict.Add(level.Level, level.Value);
                    }

                    var controller = new FeedbackController(controllerElement.Name, dict, controllerElement.InitialInput, controllerElement.InitialLevel);

                    allControllers.Add(controller);
                }

                log.InfoFormat("Initialized FeedbackControlSystem with {0} controllers from config file.", section.FeedbackControllers.Count);
            }
            else
            {
                // default settings, in case no config file was found.
                allControllers.AddRange(DefaultConfiguration.GetDefaultControllers());
            }

            this.controllerCollection = new FeedbackControllerCollection(allControllers.ToArray());
        }

        #endregion

        public void Dispose()
        {
            if (this.fileWatcher != null)
            {
                this.fileWatcher.Dispose();
            }
        }
    }
}