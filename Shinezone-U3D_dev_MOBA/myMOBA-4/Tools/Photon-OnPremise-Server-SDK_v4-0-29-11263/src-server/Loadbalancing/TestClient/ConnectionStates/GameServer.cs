// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameServer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.LoadBalancing.TestClient.ConnectionStates
{
    using System;
    using System.Collections;
    using System.Net;
    using System.Threading;

    using ExitGames.Logging;

    using Photon.Hive.Operations;

    using Photon.LoadBalancing.Operations;
    using Photon.SocketServer;
    using Photon.SocketServer.ServerToServer;

    using OperationCode = Photon.LoadBalancing.Operations.OperationCode;

    public class GameServer
    {
        #region Fields / Constancts
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private TcpClient gameServerClient;
        
        private readonly AutoResetEvent resetEvent = new AutoResetEvent(false);

        private string gameId; 

        #endregion

        public void Start(string address, string gameName, string useCase)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("GAME: Connecting to game server at {0}", address);
            }

            string[] split = address.Split(':');
            IPAddress ipaddress = IPAddress.Parse(split[0]);
            int port = int.Parse(split[1]);

            var endPoint = new IPEndPoint(ipaddress, port);
            this.gameServerClient = new TcpClient();
            this.gameServerClient.ConnectError += this.OnGameClientConnectError;
            this.gameServerClient.ConnectCompleted += this.OnGameClientConnectCompleted;
            this.gameServerClient.OperationResponse += this.OnGameClientOperationResponse;
            this.gameServerClient.Event += OnGameClientEvent;
            this.gameServerClient.Connect(endPoint, "Game");

            this.gameId = gameName; 

            if (!this.resetEvent.WaitOne(2000, true))
            {
                log.Warn("Connect time out");
                return;
            }

            if (this.gameServerClient.Connected)
            {
                if (useCase == "Create")
                {
                    this.CreateGameOnGameServer();
                }
                else
                {
                    this.JoinGameOnGameServer();
                }
            }
        }

        private void CreateGameOnGameServer()
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("GAME: Create game {0}", this.gameId);
            }

            var operation = new CreateGameRequest { GameId = this.gameId };
            var request = new OperationRequest((byte)OperationCode.CreateGame, operation);
            this.gameServerClient.SendOperationRequest(request, new SendParameters());
        }

        private void JoinGameOnGameServer()
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("GAME: Joining game {0}", this.gameId);
            }

            var operation = new JoinGameRequest { GameId = this.gameId };
            var request = new OperationRequest((byte)OperationCode.JoinGame, operation);
            this.gameServerClient.SendOperationRequest(request, new SendParameters());
        }

        private void OnGameClientConnectCompleted(object sender, EventArgs e)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("GAME: Successfully connected to game server.");
            }

            this.resetEvent.Set();
        }

        private void OnGameClientConnectError(object sender, SocketErrorEventArgs e)
        {
            log.WarnFormat("GAME: Failed to connect to game server: error = {0}", e);
        }

        private void OnGameClientEvent(object sender, EventDataEventArgs e)
        {
            //Console.Write('r');
        }

        private void OnGameClientOperationResponse(object sender, OperationResponseEventArgs e)
        {
            if (e.OperationResponse.ReturnCode != 0)
            {
                log.WarnFormat(
                    "GAME: Received error response: code={0}, result={1}, msg={2}",
                    e.OperationResponse.OperationCode,
                    e.OperationResponse.ReturnCode,
                    e.OperationResponse.DebugMessage);
                return;
            }

            switch (e.OperationResponse.OperationCode)
            {
                case (byte)Operations.OperationCode.JoinRandomGame:
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("GAME: Successfully joined random game.");
                        }
                        break;
                    }
                case (byte)Operations.OperationCode.CreateGame:
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("GAME: Successfully created game.");
                        }
                        break;
                    }
                case (byte)Operations.OperationCode.JoinGame:
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("GAME: Successfully joined game.");
                        }
                        break;
                    }
                default:
                    {
                        log.WarnFormat("GAME: received response for unexpected operation: " + e.OperationResponse.OperationCode);
                        return;
                    }
            }

            if (log.IsDebugEnabled)
            {
                log.Debug("GAME: Sending random events.");
            }

            ThreadPool.QueueUserWorkItem(this.SendEvents);
        }

        public void Stop()
        {
            if (this.gameServerClient != null && this.gameServerClient.Connected)
            {
                this.gameServerClient.Disconnect();
            }
        }

        private void SendEvents(object state)
        {
            var rnd = new Random();
            while (!Console.KeyAvailable)
            {
                var data = new Hashtable();

                var operation = new RaiseEventRequest { EvCode = 100, Data = data };
                var request = new OperationRequest((byte)Photon.Hive.Operations.OperationCode.RaiseEvent, operation);

                //Console.Write('s');
                this.gameServerClient.SendOperationRequest(request, new SendParameters());
                
                Thread.Sleep(rnd.Next(500, 1000));
            }
        }
    }
}