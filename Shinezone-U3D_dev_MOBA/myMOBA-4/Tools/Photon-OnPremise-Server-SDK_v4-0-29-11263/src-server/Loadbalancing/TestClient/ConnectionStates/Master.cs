// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Master.cs" company="Exit Games GmbH">
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
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Threading;

    using ExitGames.Logging;

    using Photon.Hive.Operations;

    using Photon.LoadBalancing.Operations;
    using Photon.SocketServer;
    using Photon.SocketServer.ServerToServer;

    public class Master
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        // local: 
        private readonly IPEndPoint masterEndPoint = new IPEndPoint(IPAddress.Parse(Settings.ServerAddress), 4530);

        private readonly AutoResetEvent resetEvent = new AutoResetEvent(false);

        private TcpClient masterClient;

        private GameServer gameServerConnection = new GameServer();

        private string gameId;

        private int actorNumber; 

        public void Start(string gameId, int actorNr)
        {
            this.actorNumber = actorNr; 
            this.ConnectToMaster();
            if (!this.resetEvent.WaitOne(2000, true))
            {
                log.Warn("Failed to connect to master.");
             
                return;
            }

            if (this.masterClient.Connected == false)
            {
                log.Warn("Failed to connect to master.");
                return;
            }
            
            // get game list: 
            this.Authenticate();
        }

        public void Stop()
        {
            if (this.masterClient.Connected)
            {
                masterClient.Disconnect();
            }

            if (this.gameServerConnection != null)
            {
                this.gameServerConnection.Stop(); 
            }
        }

        private void ConnectToMaster()
        {
            this.masterClient = new TcpClient();

            if (log.IsDebugEnabled)
            {
                log.Debug("MASTER: Connecting to master server at " + masterEndPoint + " ..");
            }

            this.masterClient.ConnectCompleted += this.OnMasterClientConnectCompleted;
            this.masterClient.ConnectError += this.OnMasterClientConnectError;
            this.masterClient.OperationResponse += this.OnMasterClientOperationResponse;
            this.masterClient.Event += this.OnMasterClientEvent;
            this.masterClient.Connect(this.masterEndPoint, "Master");
        }

        private void OnMasterClientConnectCompleted(object sender, EventArgs e)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("MASTER: Successfully connected to master");
            }

            this.resetEvent.Set();
        }

        private void OnMasterClientConnectError(object sender, SocketErrorEventArgs e)
        {
            log.WarnFormat("MASTER: Connect to master failed: err={0}", e.SocketError);

            this.resetEvent.Set();
        }

        private void Authenticate()
        {
            var request = new OperationRequest
            {
                OperationCode = (byte)Operations.OperationCode.Authenticate
            };

            this.masterClient.SendOperationRequest(request, new SendParameters());
        }

        private void JoinLobby()
        {
            var request = new OperationRequest
                {
                    OperationCode =  (byte)Operations.OperationCode.JoinLobby
                };

            this.masterClient.SendOperationRequest(request, new SendParameters());
        }

        private void CreateGame(string id)
        {
            this.gameId = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("MASTER: Creating game: id={0}", this.gameId);
            }

            var request = new OperationRequest
                {
                    OperationCode = (byte)Operations.OperationCode.CreateGame,
                    Parameters = new Dictionary<byte, object> { { (byte)ParameterCode.GameId, this.gameId } }
                };

            byte maxPlayersProperty = 1;
            var gameProperties = new Hashtable { { maxPlayersProperty, (byte)4 } };
            request.Parameters.Add((byte)ParameterKey.GameProperties, gameProperties);

            this.masterClient.SendOperationRequest(request, new SendParameters());
        }

        private void JoinRandom(Hashtable properties)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("MASTER: Joining random game ...");
            }

            var operation = new JoinRandomGameRequest { GameProperties = properties };
            var request = new OperationRequest((byte)Operations.OperationCode.JoinRandomGame, operation);

            this.masterClient.SendOperationRequest(request, new SendParameters());
        }

        private void OnMasterClientOperationResponse(object sender, OperationResponseEventArgs e)
        {
            if (e.OperationResponse.ReturnCode != 0)
            {
                log.WarnFormat(
                    "MASTER: Received error response: opCode={0}, err={1}, msg={2}",
                    e.OperationResponse.OperationCode,
                    e.OperationResponse.ReturnCode,
                    e.OperationResponse.DebugMessage);
                return;
            }

            string address;

            switch (e.OperationResponse.OperationCode)
            {
                case (byte)Operations.OperationCode.Authenticate:
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("MASTER: Authenticate succeeded.");
                    }
                    this.JoinLobby();
                    break; 

                case (byte)Operations.OperationCode.JoinRandomGame:
                    address = e.OperationResponse.Parameters[(byte)ParameterCode.Address] as string;
                    this.gameId = e.OperationResponse.Parameters[(byte)ParameterCode.GameId] as string;
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("MASTER: Join random response: address={0}, gameId={1}", address, this.gameId);
                    }
                    this.masterClient.Disconnect();
                    this.gameServerConnection.Start(address, this.gameId, "Join");
                    break;

                case (byte)Operations.OperationCode.CreateGame:
                    address = e.OperationResponse.Parameters[(byte)ParameterCode.Address] as string;
                    this.gameId = e.OperationResponse.Parameters[(byte)ParameterCode.GameId] as string;
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("MASTER: Create game response: address={0}, gameId={1}", address, this.gameId);
                    }
                    this.masterClient.Disconnect();
                    this.gameServerConnection.Start(address, this.gameId, "Create");

                    break;

                case (byte)Operations.OperationCode.JoinLobby:

                    if (this.actorNumber == 0)
                    {
                        this.CreateGame(gameId);
                    }
                    else
                    {
                        this.JoinRandom(null);
                    }

                    break; 
            }
        }

        private void OnMasterClientEvent(object sender, EventDataEventArgs e)
        {
            var eventToString = new StringBuilder();

            if (e.EventData.Code == (byte)LoadBalancing.Events.EventCode.GameList)
            {
                var gameList = (Hashtable) e.EventData.Parameters[(byte)ParameterCode.GameList]; 
                log.InfoFormat("MASTER: GameList event received. Currently: {0} Games visible.", gameList.Count);
            }

            else
            {
                foreach (var data in e.EventData.Parameters)
                {
                    eventToString.AppendFormat("{0} -> {1};", data.Key, data.Value);
                }

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("MASTER: Received Event {0}: {1}", e.EventData.Code, eventToString);
                }
            }
        }
    }
}
