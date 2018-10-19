// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodesReader.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the NodesReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    using ExitGames.Logging;

    public class NodesReader : IDisposable
    {
        #region Constants and Fields

        private readonly string nodesFileName;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly string nodesFilePath;

        private readonly object syncRoot = new object();

        private readonly FileSystemWatcher watcher;

        private Dictionary<byte, IPAddress> nodes = new Dictionary<byte, IPAddress>();

        #endregion

        #region Constructors and Destructors

        public NodesReader(string nodesFilePath, string nodesFileName)
        {
            this.CurrentNodeId = 0;
            this.nodesFilePath = nodesFilePath;
            this.nodesFileName = nodesFileName;

            if (!string.IsNullOrEmpty(nodesFilePath) && Directory.Exists(nodesFilePath))
            {
                this.watcher = new FileSystemWatcher(this.nodesFilePath, this.nodesFileName);
            }
        }

        #endregion

        #region Events

        public event EventHandler<NodeEventArgs> NodeAdded;

        public event EventHandler<NodeEventArgs> NodeChanged;

        public event EventHandler<NodeEventArgs> NodeRemoved;

        #endregion

        #region Properties

        public byte CurrentNodeId { get; private set; }

        #endregion

        #region Public Methods

        public IPAddress GetIpAddress(byte nodeId)
        {
            lock (this.syncRoot)
            {
                IPAddress result;
                if (this.nodes.TryGetValue(nodeId, out result) == false)
                {
                    log.WarnFormat("Internal address for node {0} unknown; using loop back", nodeId);
                    result = IPAddress.Loopback;
                    this.nodes.Add(nodeId, result);
                    log.Info("Node added: " + nodeId + " = " + result);
                }

                return result;
            }
        }

        public void OnNodeAdded(NodeEventArgs e)
        {
            log4net.ThreadContext.Properties["AppId"] = "Global:NodesReader";

            EventHandler<NodeEventArgs> handler = this.NodeAdded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnNodeChanged(NodeEventArgs e)
        {
            log4net.ThreadContext.Properties["AppId"] = "Global:NodesReader";

            EventHandler<NodeEventArgs> handler = this.NodeChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnNodeRemoved(NodeEventArgs e)
        {
            log4net.ThreadContext.Properties["AppId"] = "Global:NodesReader";

            EventHandler<NodeEventArgs> handler = this.NodeRemoved;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Used by game server. The returned node id will be used by clients to initate a game server connection.
        /// </summary>
        /// <returns>The local node id OR 0</returns>
        public byte ReadCurrentNodeId()
        {
            string path = Path.Combine(this.nodesFilePath, this.nodesFileName);
            if (File.Exists(path))
            {
                this.ReadNodes(path);
            }

            return this.CurrentNodeId;
        }

        public void Start()
        {
            this.ImportNodes();

            if (this.watcher != null)
            {
                log.Info("Watching " + Path.Combine(this.nodesFilePath, this.nodesFileName));
                this.watcher.Changed += this.FileSystemWatcher_OnChanged;
                this.watcher.Created += this.FileSystemWatcher_OnCreated;
                this.watcher.Deleted += this.FileSystemWatcher_OnDeleted;
                this.watcher.EnableRaisingEvents = true;
            }
        }

        public void Stop()
        {
            if (this.watcher != null)
            {
                this.watcher.EnableRaisingEvents = false;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        public void Dispose()
        {
            if (this.watcher != null)
            {
                this.watcher.Dispose();
            }
        }

        #endregion

        #endregion

        #region Methods

        private void ImportNodes()
        {
            if (log.IsInfoEnabled)
            {
                log.Info("Import nodes");
            }

            string path = Path.Combine(this.nodesFilePath, this.nodesFileName);
            if (File.Exists(path))
            {
                Dictionary<byte, IPAddress> inputNodes = this.ReadNodes(path);
                this.UpdateNodes(inputNodes);
            }
            else
            {
                this.CurrentNodeId = 0;
                log.Warn(path + " does not exist, CurrentNodeId = " + this.CurrentNodeId);
            }
        }

        private void FileSystemWatcher_OnChanged(object sender, FileSystemEventArgs e)
        {
            Dictionary<byte, IPAddress> newEntries = this.ReadNodes(e.FullPath);
            this.UpdateNodes(newEntries);
        }

        private void FileSystemWatcher_OnCreated(object sender, FileSystemEventArgs e)
        {
            Dictionary<byte, IPAddress> newEntries = this.ReadNodes(e.FullPath);
            this.UpdateNodes(newEntries);
        }

        private void FileSystemWatcher_OnDeleted(object sender, FileSystemEventArgs e)
        {
            this.UpdateNodes(new Dictionary<byte, IPAddress>());
        }

        private Dictionary<byte, IPAddress> ReadNodes(string path)
        {
            if (log.IsInfoEnabled)
            {
                log.Info("Reading nodes");
            }

            var newEntries = new Dictionary<byte, IPAddress>();
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fileStream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    if (log.IsInfoEnabled)
                    {
                        log.InfoFormat("Node line read: {0}", line);
                    }

                    string[] split = line.Split(' ');
                    if (split.Length < 3)
                    {
                        log.Warn("Skipped invalid line format (expected [id] [IP] [Y/N])");
                        continue;
                    }

                    byte nodeId;
                    if (!byte.TryParse(split[0], out nodeId))
                    {
                        continue;
                    }

                    if (newEntries.ContainsKey(nodeId))
                    {
                        // duplicate entry
                        log.Warn("Skipped duplicate node id");
                        continue;
                    }

                    IPAddress address;
                    if (!IPAddress.TryParse(split[1], out address))
                    {
                        log.Warn("Skipped invalid line (wrong IP format)");
                        continue;
                    }

                    if (split[2] == "Y")
                    {
                        this.CurrentNodeId = nodeId;
                        log.Info("Local nodeId is " + this.CurrentNodeId);
                    }

                    newEntries.Add(nodeId, address);
                }
            }

            log.InfoFormat("Read {0} nodes from {1}", newEntries.Count, this.nodesFileName);
            return newEntries;
        }

        private void UpdateNodes(Dictionary<byte, IPAddress> newEntries)
        {
            if (log.IsInfoEnabled)
            {
                log.Info("Updating Nodes...");
            }

            lock (this.syncRoot)
            {
                foreach (KeyValuePair<byte, IPAddress> entry in this.nodes)
                {
                    IPAddress newAddress;
                    if (newEntries.TryGetValue(entry.Key, out newAddress))
                    {
                        // operator == returns false even though Equals returns true
                        if (newAddress.Equals(entry.Value) == false)
                        {
                            log.Info("Node changed: " + entry.Key + " = " + newAddress);
                            this.OnNodeChanged(new NodeEventArgs(entry.Key, newAddress));
                        }
                    }
                    else
                    {
                        // node removed
                        log.Info("Node removed: " + entry.Key + " = " + entry.Value);
                        this.OnNodeRemoved(new NodeEventArgs(entry.Key, null));
                    }
                }

                foreach (KeyValuePair<byte, IPAddress> entry in newEntries)
                {
                    if (!this.nodes.ContainsKey(entry.Key))
                    {
                        // node added
                        log.Info("Node added: " + entry.Key + " = " + entry.Value);
                        this.OnNodeAdded(new NodeEventArgs(entry.Key, entry.Value));
                    }
                }

                this.nodes = newEntries;
            }
        }

        #endregion

        public class NodeEventArgs : EventArgs
        {
            #region Constants and Fields

            private readonly IPAddress address;

            private readonly byte nodeId;

            #endregion

            #region Constructors and Destructors

            public NodeEventArgs(byte nodeId, IPAddress address)
            {
                this.address = address;
                this.nodeId = nodeId;
            }

            #endregion

            #region Properties

            public IPAddress Address
            {
                get
                {
                    return this.address;
                }
            }

            public byte NodeId
            {
                get
                {
                    return this.nodeId;
                }
            }

            #endregion
        }
    }
}