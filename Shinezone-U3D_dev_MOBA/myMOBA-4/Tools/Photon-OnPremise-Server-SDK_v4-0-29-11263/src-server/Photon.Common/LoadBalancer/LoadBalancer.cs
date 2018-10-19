// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadBalancer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LoadBalancer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using ExitGames.Logging;
using Photon.Common.LoadBalancer.Configuration;
using Photon.Common.LoadBalancer.LoadShedding;

namespace Photon.Common.LoadBalancer
{
    #region using directives

    

    #endregion

    /// <summary>
    ///   Represents a collection of server instances which can be accessed
    ///   randomly based on their current lod level. 
    /// </summary>
    /// <typeparam name = "TServer">
    ///   The type of the server instances.
    /// </typeparam>
    /// <remarks>
    /// Each server instance gets a weight assigned based on the current load level of that server. 
    /// The TryGetServer method gets a random server based on this weight. A server with a higher
    /// weight will be returned more often than a server with a lower weight.
    /// The default values for this weights are the following:
    /// 
    /// LoadLevel.Lowest  = 40
    /// LoadLevel.Low     = 30
    /// LoadLevel.Normal  = 20
    /// LoadLevel.High    = 10
    /// LoadLevel.Highest = 0
    /// 
    /// If there is for example one server for eac load level, the server with load level lowest 
    /// will be returned 50% of the times, the one with load level low 30% and so on. 
    /// </remarks>
    public class LoadBalancer<TServer>
    {
        #region Constants and Fields

        // ReSharper disable StaticFieldInGenericType
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        // ReSharper restore StaticFieldInGenericType

        // dictionary for fast server instance lookup
        private readonly Dictionary<TServer, ServerState> serverList;

        // stores the available server instances ordered by their weight
        private readonly LinkedList<ServerState> availableServers = new LinkedList<ServerState>();

        // list of the weights for each possible load level
        private int[] loadLevelWeights;

        // pseudo-random number generator for gettings a random server
        private readonly Random random;

        // stores the sum of the weights of all server instances
        private int totalWeight;

        // stores the sum of the load levels of all server instances
        // used to calculate the average load level
        private int totalWorkload;

        // watch files 
        private readonly FileSystemWatcher fileWatcher;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LoadBalancer{TServer}" /> class. Use default weights for each load level. 
        /// </summary>
        public LoadBalancer()
        {
            this.random = new Random(); 
           this.serverList = new Dictionary<TServer, ServerState>();
           this.loadLevelWeights = DefaultConfiguration.GetDefaultWeights();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LoadBalancer{TServer}" /> class.
        /// </summary>
        /// <param name="configFilePath">
        /// The full path (absolute or relative) to a config file that specifies a Weight for each LoadLevel. 
        /// The possible load levels and their values are defined  int the 
        /// <see cref="FeedbackLevel"/> enumeration. 
        /// See the LoadBalancing.config for an example.
        /// </param>
        public LoadBalancer(string configFilePath)
        {
            this.serverList = new Dictionary<TServer, ServerState>();
            this.random = new Random();

            this.InitializeFromConfig(configFilePath);

            string fullPath = Path.GetFullPath(configFilePath);
            string path = Path.GetDirectoryName(fullPath);
            if (path == null)
            {
                log.InfoFormat("Could not watch for configuration file. No path specified.");
                return;
            }

            string filter = Path.GetFileName(fullPath);
            if (filter == null)
            {
                log.InfoFormat("Could not watch for configuration file. No file specified.");
                return;
            }

            this.fileWatcher = new FileSystemWatcher(path, filter);
            this.fileWatcher.Changed += this.ConfigFileChanged;
            this.fileWatcher.Created += this.ConfigFileChanged;
            this.fileWatcher.Deleted += this.ConfigFileChanged;
            this.fileWatcher.Renamed += this.ConfigFileChanged;
            this.fileWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LoadBalancer{TServer}" /> class.
        /// </summary>
        /// <param name="loadLevelWeights">
        /// A list of weights which should be used for each available load level.
        /// This list must contain a value for each available load level and
        /// must be ordered by the load levels value. 
        /// The possible load levels and their values are defined  int the 
        /// <see cref="FeedbackLevel"/> enumeration.
        /// </param>
        public LoadBalancer(int[] loadLevelWeights)
        {
            if (loadLevelWeights == null)
            {
                throw new ArgumentNullException("loadLevelWeights");
            }

            const int feedbackLevelCount = (int)FeedbackLevel.Highest + 1;
            if (loadLevelWeights.Length != feedbackLevelCount)
            {
                throw new ArgumentOutOfRangeException(
                    "loadLevelWeights", 
                    string.Format(
                        "Parameter loadLevelWeights must have a length of {0}. One weight for each possible load level", 
                        feedbackLevelCount));
            }

            this.serverList = new Dictionary<TServer, ServerState>();
            this.random = new Random();
            this.loadLevelWeights = loadLevelWeights;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LoadBalancer{TServer}" /> class.
        /// </summary>
        /// <remarks>
        /// This overload is used for unit testing to provide a fixed seed for the 
        /// random number generator.
        /// </remarks>
        public LoadBalancer(int[] loadLevelWeights, int seed)
            : this(loadLevelWeights)
        {
            this.random = new Random(seed);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the average workload of all server instances.
        /// </summary>
        public FeedbackLevel AverageWorkload 
        {
            get
            {
                if (this.serverList.Count == 0)
                {
                    return 0;
                }

                return (FeedbackLevel)(int)Math.Round((double)this.totalWorkload / this.serverList.Count);
            }            
        }

        public bool HasAvailableServers
        {
            get
            {
                lock (this.serverList)
                {
                    return this.availableServers.Count != 0;
                }
            }
        }

        public int TotalWorkload
        {
            get
            {
                return this.totalWorkload; 
            }
        }

        public int TotalWeight
        {
            get
            {
                return this.totalWeight;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Attempts to add a server instance.
        /// </summary>
        /// <param name = "server">The server instance to add.</param>
        /// <param name = "loadLevel">The current workload of the server instance.</param>
        /// <returns>
        ///   True if the server instance was added successfully. If the server instance already exists, 
        ///   this method returns false.
        /// </returns>
        public bool TryAddServer(TServer server, FeedbackLevel loadLevel)
        {
            lock (this.serverList)
            {
                // check if the server instance was already added
                if (this.serverList.ContainsKey(server))
                {
                    log.WarnFormat("LoadBalancer already contains server {0}", server);
                    return false;
                }

                var serverState = new ServerState(server) 
                { 
                    LoadLevel = loadLevel, 
                    Weight = this.GetLoadLevelWeight(loadLevel) 
                };

                this.serverList.Add(server, serverState);

                if (serverState.Weight > 0)
                {
                    this.AddToAvailableServers(serverState);
                }

                this.UpdateTotalWorkload(FeedbackLevel.Lowest, loadLevel);

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Added server: workload={0}", loadLevel);
                }

                return true;
            }
        }

        /// <summary>
        ///   Tries to get a free server instance.
        /// </summary>
        /// <param name = "server">
        ///   When this method returns, contains an available server instance 
        ///   or null if no available server instances exists.
        /// </param>
        /// <returns>
        ///   True if a server instance with enough remaining workload is found; otherwise false.
        /// </returns>
        public bool TryGetServer(out TServer server)
        {
            FeedbackLevel workload;
            return this.TryGetServer(out server, out workload);
        }

        /// <summary>
        ///   Tries to get a free server instance.
        /// </summary>
        /// <param name = "server">
        ///   When this method returns, contains the server instance with the fewest workload
        ///   or null if no server instances exists.
        /// </param>
        /// <param name = "loadLevel">
        ///   When this method returns, contains an available server instance 
        ///   or null if no available server instances exists.
        /// </param>
        /// <returns>
        ///   True if a server instance with enough remaining workload is found; otherwise false.
        /// </returns>
        public bool TryGetServer(out TServer server, out FeedbackLevel loadLevel)
        {
            lock (this.serverList)
            {
                if (this.availableServers.Count == 0)
                {
                    loadLevel = FeedbackLevel.Highest;
                    server = default(TServer);
                    return false;
                }

                // Get a random weight between 0 and the sum of the weight of all server isntances
                var randomWeight = this.random.Next(this.totalWeight);
                int weight = 0;

                // Iterate through the server instances and add sum the weights of each instance.
                // If the sum of the weights is greater than the generated random value
                // the current server instance in the loop will be returned.
                // Using this method ensures that server instances with a higher weight will
                // be hit more often than one with a lower weihgt.
                var node = this.availableServers.First;
                while (node != null)
                {
                    weight += node.Value.Weight;
                    if (weight > randomWeight)
                    {
                        server = node.Value.Server;
                        loadLevel = node.Value.LoadLevel;
                        return true;
                    }

                    node = node.Next;
                }

                // this should never happen but better log out a warning and 
                // return an available server instance
                log.WarnFormat("Failed to get a server instance based on the weights");
                server = this.availableServers.First.Value.Server;
                loadLevel = this.availableServers.First.Value.LoadLevel;
                return true;
            }
        }

        /// <summary>
        ///   Tries to remove a server instance.
        /// </summary>
        /// <param name = "server">The server instance to remove.</param>
        /// <returns>
        ///   True if the server instance was removed successfully. 
        ///   If the server instance does not exists, this method returns false.
        /// </returns>
        public bool TryRemoveServer(TServer server)
        {
            lock (this.serverList)
            {
                ServerState serverState;
                if (this.serverList.TryGetValue(server, out serverState) == false)
                {
                    return false;
                }

                this.serverList.Remove(server);
                this.RemoveFromAvailableServers(serverState);

                this.UpdateTotalWorkload(serverState.LoadLevel, FeedbackLevel.Lowest);

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Removed server: workload={0}", serverState.LoadLevel);
                }

                return true;
            }
        }

        /// <summary>
        ///   Tries to update a server instance.
        /// </summary>
        /// <param name = "server">The server to update.</param>
        /// <param name = "newLoadLevel">The current workload of the server instance.</param>
        /// <returns>
        ///   True if the server instance was updated successfully. 
        ///   If the server instance does not exists, this method returns false.
        /// </returns>
        public bool TryUpdateServer(TServer server, FeedbackLevel newLoadLevel)
        {
            lock (this.serverList)
            {
                // check if server instance exits
                ServerState serverState;
                if (this.serverList.TryGetValue(server, out serverState) == false)
                {
                    return false;
                }

                // check if load level has changed
                if (serverState.LoadLevel == newLoadLevel)
                {
                    return true;
                }

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Updating server: oldWorkload={0}, newWorkload={1}", serverState.LoadLevel, newLoadLevel);
                }

                // apply new state
                this.UpdateTotalWorkload(serverState.LoadLevel, newLoadLevel);
                
                serverState.LoadLevel = newLoadLevel;
                var newWeight = this.GetLoadLevelWeight(newLoadLevel);

                // check if the weight for the server instance has changes
                // if it has not changed we don't have to update the list of available servers
                if (newWeight == serverState.Weight)
                {
                    return true;
                }

                this.RemoveFromAvailableServers(serverState);
                serverState.Weight = newWeight;

                if (serverState.Weight > 0)
                {
                    this.AddToAvailableServers(serverState);
                }

                return true;
            }
        }
   
        #endregion

        private void InitializeFromConfig(string configFilePath)
        {
            string message;
            LoadBalancerSection section;

            int[] weights = null;

            if (!ConfigurationLoader.TryLoadFromFile(configFilePath, out section, out message))
            {
                log.WarnFormat(
                    "Could not initialize LoadBalancer from configuration: Invalid configuration file {0}. Using default settings... ({1})",
                    configFilePath,
                    message);
            }

            if (section != null)
            {
                // load weights from config file & sort:
                var dict = new SortedDictionary<int, int>();
                foreach (LoadBalancerWeight weight in section.LoadBalancerWeights)
                {
                    dict.Add((int)weight.Level, weight.Value);
                }

                if (dict.Count == (int)FeedbackLevel.Highest + 1)
                {
                    weights = new int[dict.Count];
                    dict.Values.CopyTo(weights, 0);

                    log.InfoFormat("Initialized Load Balancer from configuration file: {0}", configFilePath);
                }
                else
                {
                    log.WarnFormat(
                        "Could not initialize LoadBalancer from configuration: {0} is invalid - expected {1} entries, but found {2}. Using default settings...",
                        configFilePath,
                        (int)FeedbackLevel.Highest + 1,
                        dict.Count);
                }
            }

            if (weights == null)
            {
                weights = DefaultConfiguration.GetDefaultWeights();
            }

            this.loadLevelWeights = weights;
        }

        private void UpdateTotalWorkload(FeedbackLevel oldLoadLevel, FeedbackLevel newLoadLevel)
        {
            this.totalWorkload -= (int)oldLoadLevel;
            this.totalWorkload += (int)newLoadLevel;
        }

        private int GetLoadLevelWeight(FeedbackLevel loadLevel)
        {
            return this.loadLevelWeights[(int)loadLevel]; 
        }

        private void AddToAvailableServers(ServerState serverState)
        {
            this.totalWeight += serverState.Weight;

            // find the first server with a lower weight and insert
            // the server before it to keep the list of available server 
            // instances sorted by weight.
            var node = this.availableServers.First;
            while (node != null)
            {
                if (node.Value.Weight <= serverState.Weight)
                {
                    serverState.Node = this.availableServers.AddBefore(node, serverState);
                    return;
                }

                node = node.Next;
            }

            // no server with a lower load level has been found
            // so simply add the server to the end of the available 
            // server list
            serverState.Node = this.availableServers.AddLast(serverState);
        }

        private void RemoveFromAvailableServers(ServerState serverState)
        {
            if (serverState.Node == null)
            {
                return;
            }

            this.totalWeight -= serverState.Weight;
            this.availableServers.Remove(serverState.Node);
            serverState.Node = null;
        }

        private void ConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            log.InfoFormat("Configuration file for LoadBalancer Weights {0}\\{1} {2}. Reinitializing...", e.FullPath, e.Name, e.ChangeType);
            
            this.InitializeFromConfig(e.FullPath);
            
            this.UpdateWeightForAllServers();
        }

        private void UpdateWeightForAllServers()
        {
            lock (this.serverList)
            {
                foreach (var server in this.serverList.Keys)
                {
                    // check if server instance exits
                    ServerState serverState;
                    if (this.serverList.TryGetValue(server, out serverState) == false)
                    {
                        continue;
                    }
                    
                    var newWeight = this.GetLoadLevelWeight(serverState.LoadLevel);

                    // check if the weight for the server instance has changes
                    // if it has not changed we don't have to update the list of available servers
                    if (newWeight == serverState.Weight)
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat("LoadBalancer Weight did NOT change for server {0}: loadLevel={1}, weight={2}", serverState.Server, serverState.LoadLevel, serverState.Weight);
                        }
                    }
                    else
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.DebugFormat(
                                "LoadBalancer Weight did change for server {0}: loadLevel={1}, oldWeight={2}, newWeight={3}",
                                serverState.Server,
                                serverState.LoadLevel,
                                serverState.Weight,
                                newWeight);
                        }

                        this.RemoveFromAvailableServers(serverState);
                        serverState.Weight = newWeight;

                        if (serverState.Weight > 0)
                        {
                            this.AddToAvailableServers(serverState);
                        }
                    }
                }
            }
        }

        private class ServerState
        {
            public ServerState(TServer server)
            {
                this.Server = server;
            }

            public TServer Server { get; private set; }

            public FeedbackLevel LoadLevel { get; set; }

            public int Weight { get; set; }

            public LinkedListNode<ServerState> Node { get; set; }
        }

        public void DumpState()
        {
            log.WarnFormat("LoadBalancer servers count {0}. Aviable server count {1}", 
                this.serverList.Count, 
                this.availableServers.Count);
        }
    }
}