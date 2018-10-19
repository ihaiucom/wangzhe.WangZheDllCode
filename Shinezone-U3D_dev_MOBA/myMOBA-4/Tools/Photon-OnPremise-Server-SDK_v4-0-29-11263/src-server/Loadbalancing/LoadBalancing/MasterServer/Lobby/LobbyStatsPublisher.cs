
using Photon.Common;
using Photon.LoadBalancing.Common;

namespace Photon.LoadBalancing.MasterServer.Lobby
{
    using System;
    using System.Collections.Generic;

    using ExitGames.Concurrency.Fibers;
    using ExitGames.Logging;

    using Photon.LoadBalancing.Operations;
    using Photon.SocketServer;

    using OperationCode = Photon.LoadBalancing.Operations.OperationCode;
    using EventCode = Photon.LoadBalancing.Events.EventCode;

    public class LobbyStatsPublisher : IDisposable
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly PoolFiber fiber;

        private readonly LobbyFactory lobbyFactory;

        private readonly IDisposable schedule;

        private readonly HashSet<PeerBase> subscriber = new HashSet<PeerBase>();

        private readonly int maxLobbyStatsCount;

        public LobbyStatsPublisher(LobbyFactory lobbyFactory, int publishIntervalSeconds, int maxLobbyStatsCount)
        {
            this.lobbyFactory = lobbyFactory;
            this.maxLobbyStatsCount = maxLobbyStatsCount;
            this.subscriber = new HashSet<PeerBase>();

            this.fiber = new PoolFiber();
            this.fiber.Start();

            if (publishIntervalSeconds > 0)
            {
                this.schedule = this.fiber.ScheduleOnInterval(this.BroadcastStatisticEvent, 0, publishIntervalSeconds * 1000);
            }
        }

        public void Dispose()
        {
            if (schedule != null)
            {
                this.schedule.Dispose();
            }
            
            this.fiber.Dispose();
        }

        public void EnqueueGetStatsRequest(PeerBase peer, GetLobbyStatsRequest statsRequest, SendParameters sendParameter)
        {
            this.fiber.Enqueue(() => this.ExecuteGetStatsRequest(peer, statsRequest, sendParameter));
        }

        public void Subscribe(PeerBase peer)
        {
            this.fiber.Enqueue(() => this.ExecuteSubscribe(peer));
        }

        public void Unsubscribe(PeerBase peer)
        {
            this.fiber.Enqueue(() => this.ExecuteUnsubscribe(peer));
        }

        private void ExecuteGetStatsRequest(PeerBase peer, GetLobbyStatsRequest statsRequest, SendParameters sendParameter)
        {
            try
            {
                OperationResponse errorresponse;
                if (this.ValidateRequest(statsRequest, out errorresponse) == false)
                {
                    peer.SendOperationResponse(errorresponse, sendParameter);
                    return;
                }


                var response = new GetLobbyStatsResponse();
                AppLobby[] lobbies;

                short returnCode = 0;
                string debugMessage = null;

                if (statsRequest.LobbyNames == null)
                {
                    lobbies = this.lobbyFactory.GetLobbies(this.maxLobbyStatsCount);
                    response = LobbyListToLobbyStatsData(lobbies);
                }
                else
                {
                    // Check if lobby stats limit is exceeded.
                    // If Limit is exceeded the complete list will be returned anyway but with the response result code set to -4.
                    // This behaviour might change in future versions.
                    if (this.maxLobbyStatsCount > 0 && statsRequest.LobbyNames.Length > this.maxLobbyStatsCount)
                    {
                        returnCode = (short)ErrorCode.ArgumentOutOfRange;
                        debugMessage = string.Format("LobbyStats limit of {0} exceeded", this.maxLobbyStatsCount);
                    }

                    lobbies = this.lobbyFactory.GetLobbies(statsRequest.LobbyNames, statsRequest.LobbyTypes);

                    var count = lobbies.Length;
                    response.PeerCount = new int[count];
                    response.GameCount = new int[count];

                    for (int i = 0; i < lobbies.Length; i++)
                    {
                        if (lobbies[i] != null)
                        {
                            response.PeerCount[i] = lobbies[i].PeerCount + lobbies[i].PlayerCount;
                            response.GameCount[i] = lobbies[i].GameCount;
                        }
                        else
                        {
                            response.PeerCount[i] = 0;
                            response.GameCount[i] = 0;
                        }
                    }
                }


                var operationResponse = new OperationResponse((byte)OperationCode.LobbyStats, response)
                    {
                        ReturnCode = returnCode,
                        DebugMessage = debugMessage
                    };

                peer.SendOperationResponse(operationResponse, sendParameter);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private bool ValidateRequest(GetLobbyStatsRequest request, out OperationResponse errorResponse)
        {
            errorResponse = null;

            if (request.LobbyNames == null)
            {
                return true;
            }

            if (request.LobbyTypes == null)
            {
                errorResponse = new OperationResponse
                {
                    OperationCode = request.OperationRequest.OperationCode,
                    ReturnCode = (short)ErrorCode.OperationInvalid,
                    DebugMessage = LBErrorMessages.LobbyTypesNotSet,
                };

                return false;
            }

            //if (this.maxLobbyStatsCount > 0 && request.LobbyNames.Length > this.maxLobbyStatsCount)
            //{
            //    errorResponse = new OperationResponse
            //    {
            //        OperationCode = (byte)OperationCode.LobbyStats,
            //        ReturnCode = (short)ErrorCode.ArgumentOutOfRange,
            //        DebugMessage = string.Format("LobbyStats limit of {0}  exceeded", this.maxLobbyStatsCount)
            //    };

            //    return false;
            //}

            if (request.LobbyNames.Length != request.LobbyTypes.Length)
            {
                errorResponse = new OperationResponse
                {
                    OperationCode = request.OperationRequest.OperationCode,
                    ReturnCode = (short)ErrorCode.OperationInvalid,
                    DebugMessage = LBErrorMessages.LobbyTypesLenDoNotMatchLobbyNames,
                };

                return false;
            }

            return true;
        }

        private void ExecuteSubscribe(PeerBase peer)
        {
            try
            {
                this.subscriber.Add(peer);
                this.PublishStatisticEvent(peer);
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Added subsciption: peerId={0}", peer.ConnectionId);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void ExecuteUnsubscribe(PeerBase peer)
        {
            try
            {
                if (this.subscriber.Remove(peer) == false)
                {
                    return;
                }

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Removed subsciption: peerId={0}", peer.ConnectionId);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void PublishStatisticEvent(PeerBase peer)
        {
            var lobbyList = this.lobbyFactory.GetLobbies(this.maxLobbyStatsCount);
            var lobbyStats = LobbyListToLobbyStatsData(lobbyList);
            var eventData = new EventData((byte)EventCode.LobbyStats, lobbyStats);
            peer.SendEvent(eventData, new SendParameters());
        }

        private void BroadcastStatisticEvent()
        {
            try
            {
                if (this.subscriber.Count <= 0)
                {
                    return;
                }

                if (log.IsDebugEnabled)
                {
                    log.Debug("Publishing lobby statistics");
                }

                var lobbyList = this.lobbyFactory.GetLobbies(this.maxLobbyStatsCount);
                var lobbyStats = LobbyListToLobbyStatsData(lobbyList);
                var eventData = new EventData((byte)EventCode.LobbyStats, lobbyStats);
                ApplicationBase.Instance.BroadCastEvent(eventData, this.subscriber, new SendParameters { Unreliable = true });
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private static GetLobbyStatsResponse LobbyListToLobbyStatsData(AppLobby[] lobbyList)
        {
            var lobbyStats = new GetLobbyStatsResponse();

            var count = lobbyList.Length;
            lobbyStats.LobbyNames = new string[count];
            lobbyStats.LobbyTypes = new byte[count];
            lobbyStats.PeerCount = new int[count];
            lobbyStats.GameCount = new int[count];

            for (int i = 0; i < lobbyList.Length; i++)
            {
                var item = lobbyList[i];
                lobbyStats.LobbyNames[i] = item.LobbyName;
                lobbyStats.LobbyTypes[i] = (byte)item.LobbyType;
                lobbyStats.PeerCount[i] = item.PeerCount + item.PlayerCount;
                lobbyStats.GameCount[i] = item.GameCount;
            }

            return lobbyStats;
        }

    }
}
