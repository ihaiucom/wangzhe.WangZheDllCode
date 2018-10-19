namespace Photon.Common.LoadBalancer
{
    using System;
    using System.IO;
    using System.IO.Pipes;
    using System.Threading;

    using ExitGames.Logging;

    using Photon.Common.LoadBalancer.Common;
    using Photon.Common.LoadBalancer.LoadShedding;

    public class ServerStateManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly string NamedPipeName;

        private string appOfflineFile;
        private FileSystemWatcher fileWatcher;
        private Timer checkOfflineTimer;
        private Thread pipeThread;
        private DateTime appOfflineDate;
        private readonly WorkloadController workloadController;
        private volatile bool stopListeningPipe;

        public Action<ServerState, ServerState, TimeSpan> OnNewServerState;
        private NamedPipeServerStream pipeServer;

        public ServerStateManager(WorkloadController workloadController, string pipeName = "GameServer")
        {
            this.workloadController = workloadController;
            this.NamedPipeName = pipeName;
        }

        public bool Start(string offlineFile)
        {
            this.appOfflineFile = offlineFile;

            var directory = Path.GetDirectoryName(this.appOfflineFile);
            var fileName = Path.GetFileName(this.appOfflineFile);
            if (directory == null || string.IsNullOrEmpty(fileName))
            {
                log.WarnFormat("Could not determin directory and fileName for server state file: {0}", this.appOfflineFile);
            }
            else
            {
                this.fileWatcher = new FileSystemWatcher(directory, fileName);
                this.fileWatcher.Changed += this.OnFileWatcherChanged;
                this.fileWatcher.Renamed += this.OnFileWatcherChanged;
                this.fileWatcher.Deleted += this.OnFileWatcherChanged;
                this.fileWatcher.Created += this.OnFileWatcherChanged;
                this.fileWatcher.EnableRaisingEvents = true;
                if (log.IsInfoEnabled)
                {
                    log.Info("Watching for application state file in " + this.appOfflineFile);
                }

                this.CheckAppOffline();
            }
            return true;
        }
        private void OnFileWatcherChanged(object sender, FileSystemEventArgs e)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Server state file changed: {0} - {1}", e.FullPath, e.ChangeType);
            }

            this.CheckAppOffline();
        }

        private void OnCheckAppOfflineTimer(object state)
        {
            this.CheckAppOffline();
        }

        public void CheckAppOffline()
        {
            try
            {
                if (this.checkOfflineTimer != null)
                {
                    this.checkOfflineTimer.Dispose();
                    this.checkOfflineTimer = null;
                }

                if (!File.Exists(this.appOfflineFile))
                {
                    this.workloadController.ServerState = ServerState.Normal;

                    if (log.IsInfoEnabled)
                    {
                        log.InfoFormat("Server state is set to online");
                    }

                    return;
                }


                string[] serverStateFileContent;
                try
                {
                    serverStateFileContent = File.ReadAllLines(this.appOfflineFile);
                }
                catch (IOException ioException)
                {
                    var hresult = System.Runtime.InteropServices.Marshal.GetHRForException(ioException);

                    // check for sharing violation
                    if ((hresult & 0xFFFF) == 0x20)
                    {
                        // the file is still in use
                        this.checkOfflineTimer = new Timer(this.OnCheckAppOfflineTimer, null, 1000, System.Threading.Timeout.Infinite);
                    }
                    else
                    {
                        log.Error(ioException);
                    }

                    return;
                }
                if (serverStateFileContent.Length == 0)
                {
                    log.ErrorFormat("ServerState file is empty. Current Server state {0} is unchanged.", this.workloadController.ServerState);
                    return;
                }

                var serverStateLine = serverStateFileContent[0];

                int requestedState;
                if (int.TryParse(serverStateLine, out requestedState) == false)
                {
                    log.WarnFormat("Invalid app state specified: {0}", serverStateLine);
                    return;
                }

                if ((ServerState)requestedState == this.workloadController.ServerState)
                {
                    return;
                }

                var offlineTime = TimeSpan.FromMinutes(15);

                var offlineTimeString = serverStateFileContent.Length > 1 ? serverStateFileContent[1] : string.Empty;
                if (!string.IsNullOrEmpty(offlineTimeString))
                {
                    if (TimeSpan.TryParse(offlineTimeString, out offlineTime) == false)
                    {
                        log.WarnFormat("Failed to parse offline time: {0}", offlineTimeString);
                        return;
                    }
                }

                if (log.IsInfoEnabled)
                {
                    log.InfoFormat("Server will be taken offline in {0}", offlineTime);
                }

                this.appOfflineDate = DateTime.UtcNow.Add(offlineTime);

                var oldState = this.workloadController.ServerState;
                this.workloadController.ServerState = (ServerState)requestedState;
                if (this.OnNewServerState != null)
                {
                    this.OnNewServerState(oldState, (ServerState)requestedState, offlineTime);
                }
            }

            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void ListenToPipe()
        {
            try
            {
                while (!this.stopListeningPipe)
                {
                    using (this.pipeServer = new NamedPipeServerStream(this.NamedPipeName, PipeDirection.InOut, 3))
                    {
                        log.InfoFormat("Waiting for named pipe client connection");
                        this.pipeServer.WaitForConnection();

                        log.InfoFormat("Named pipe client connected");

                        if (this.stopListeningPipe)
                        {
                            log.InfoFormat("we are stopping server. break connection");
                            break;
                        }
                        var sr = new BinaryReader(this.pipeServer);
                        var sw = new BinaryWriter(this.pipeServer);

                        try
                        {
                            var command = 0;
                            while (command != -1)
                            {
                                sw.Write((byte)this.workloadController.ServerState);
                                sw.Write(this.appOfflineDate.ToBinary());
                                sw.Flush();

                                command = this.pipeServer.ReadByte();
                                if (log.IsDebugEnabled)
                                {
                                    log.DebugFormat("Received named pipe command: {0}", command);
                                }

                                var oldState = this.workloadController.ServerState;
                                this.workloadController.ServerState = (ServerState)command;
                                this.appOfflineDate = DateTime.MinValue;
                                var offlineTime = TimeSpan.MinValue;

                                if (command == 2)
                                {
                                    var secondsToTakeOffline = sr.ReadInt32();
                                    offlineTime = TimeSpan.FromSeconds(secondsToTakeOffline);
                                    this.appOfflineDate = DateTime.UtcNow.Add(offlineTime);
                                }

                                //switch (command)
                                //{
                                //    case 0:
                                //        application.WorkloadController.ServerState = ServerState.Normal;
                                //        this.appOfflineDate = DateTime.MinValue;
                                //        if (oldState == ServerState.Offline)
                                //        {
                                //            application.MasterServerController.UpdateAllGameStates();
                                //        }
                                //        break;

                                //    case 1:
                                //        application.WorkloadController.ServerState = ServerState.OutOfRotation;
                                //        this.appOfflineDate = DateTime.MinValue;
                                //        if (oldState == ServerState.Offline)
                                //        {
                                //            application.MasterServerController.UpdateAllGameStates();
                                //        }
                                //        break;

                                //    case 2:
                                //        int secondsToTakeOffline = sr.ReadInt32();
                                //        var timeSpan = TimeSpan.FromSeconds(secondsToTakeOffline);
                                //        application.WorkloadController.ServerState = ServerState.Offline;
                                //        this.appOfflineDate = DateTime.UtcNow.Add(timeSpan);
                                //        this.RaiseOfflineEvent(timeSpan);
                                //        break;
                                //}

                                if (this.OnNewServerState != null)
                                {
                                    this.OnNewServerState(oldState, (ServerState)command, offlineTime);
                                }
                            }
                        }
                        catch (IOException e)
                        {
                            log.Error(e);
                        }
                        finally
                        {
                            this.pipeServer.Disconnect();
                            this.pipeServer.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public void StartListenPipe()
        {
            if (log.IsInfoEnabled)
            {
                log.Info("Watching for application state on named pipe");
            }

            this.pipeThread = new Thread(this.ListenToPipe)
            {
                IsBackground = true
            };
            this.pipeThread.Start();
        }

        public void StopListenPipe()
        {
            this.stopListeningPipe = true;
            if (this.pipeServer != null)
            {
                try
                {
                    using (var npcs = new NamedPipeClientStream(this.NamedPipeName))
                    {
                        npcs.Connect(100);
                    }
                }
                catch (System.TimeoutException)
                { }
            }

            if (this.pipeThread != null)
            {
                if (!this.pipeThread.Join(1000))
                {
                    this.pipeThread.Abort();
                }
            }
        }
    }
}