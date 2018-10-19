namespace Photon.LoadBalancing.MasterServer.Lobby
{
    using System.Collections;
    using ExitGames.Logging;
    using Photon.SocketServer;

    public class AsyncRandomGameList : GameList
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        #region Constructors and Destructors

        public AsyncRandomGameList(AppLobby lobby)
            : base(lobby)
        {
        }

        #endregion

        public override Hashtable GetChangedGames()
        {
            return new Hashtable();
        }

        public override Hashtable GetAllGames(int maxCount)
        {
            return new Hashtable();
        }

        public override IGameListSubscription AddSubscription(PeerBase peer, Hashtable gamePropertyFilter, int maxGameCount)
        {
            return null;
        }

        public override void PublishGameChanges()
        {
            // nothing here
        }
    }
}
