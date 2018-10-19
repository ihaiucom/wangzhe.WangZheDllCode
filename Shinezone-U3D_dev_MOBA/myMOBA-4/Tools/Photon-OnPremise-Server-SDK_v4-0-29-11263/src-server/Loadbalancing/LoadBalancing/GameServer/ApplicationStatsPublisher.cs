namespace Photon.LoadBalancing.GameServer
{
    using System;

    using ExitGames.Concurrency.Fibers;
    using ExitGames.Logging;

    using Photon.LoadBalancing.ServerToServer.Events;
    using Photon.SocketServer;

    public class ApplicationStatsPublisher
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly GameApplication application;

        private readonly PoolFiber fiber;

        private readonly int publishIntervalMilliseconds = 1000;

        private IDisposable publishStatsSchedule;

        public ApplicationStatsPublisher(GameApplication application, int publishIntervalMilliseconds)
        {
            this.application = application;
            this.publishIntervalMilliseconds = publishIntervalMilliseconds;
            this.fiber = new PoolFiber();
            this.fiber.Start();
        }

        public int PeerCount { get; private set; }

        public int GameCount { get; private set; }

        public void IncrementPeerCount()
        {
            this.fiber.Enqueue(() => UpdatePeerCount(1));
        }

        public void DecrementPeerCount()
        {
            this.fiber.Enqueue(() => UpdatePeerCount(-1));
        }

        public void IncrementGameCount()
        {
            this.fiber.Enqueue(() => UpdateGameCount(1));
        }

        public void DecrementGameCount()
        {
            this.fiber.Enqueue(() => UpdateGameCount(-1));
        }

        private void UpdatePeerCount(int diff)
        {
            this.PeerCount += diff;
            this.OnStatsUpdated();
        }

        private void UpdateGameCount(int diff)
        {
            this.GameCount += diff;
            this.OnStatsUpdated();
        }

        private void OnStatsUpdated()
        {
            if (this.publishStatsSchedule != null)
            {
                return;
            }

            this.publishStatsSchedule = this.fiber.Schedule(this.PublishStats, this.publishIntervalMilliseconds);
        }

        private void PublishStats()
        {
            this.publishStatsSchedule = null;
            var e = new UpdateAppStatsEvent { PlayerCount = this.PeerCount, GameCount = this.GameCount };
            if (this.application.MasterServerConnection != null)
            {
                this.application.MasterServerConnection.SendEventIfRegistered(
                    new EventData((byte)ServerEventCode.UpdateAppStats, e), new SendParameters());
            }
        }
    }
}
