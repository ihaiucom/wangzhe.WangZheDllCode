
namespace MyApplication
{
    using Lite;
    using Lite.Caching;

    using Operations;

    using Photon.SocketServer;

    public class MyGame : LiteGame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyGame"/> class.
        /// </summary>
        /// <param name="gameName">The name of the game.</param>
        /// <param name="roomCache">
        ///   The <see cref="RoomCacheBase"/> instance to which the room belongs.
        /// </param>
        public MyGame(string gameName, RoomCacheBase roomCache)
            : base(gameName, roomCache)
        {
        }

        /// <summary>
        /// Called for each operation in the execution queue.
        /// </summary>
        /// <param name="peer">The peer.</param>
        /// <param name="operationRequest">The operation request to execute.</param>
        /// <param name="sendParameters"></param>
        /// <remarks>
        /// ExecuteOperation is overriden to handle our custom operations.
        /// </remarks>
        protected override void ExecuteOperation(LitePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch ((MyOperationCodes)operationRequest.OperationCode)
            {
                case MyOperationCodes.GameOperation:
                    this.HandleMyGameOperation(peer, operationRequest, sendParameters);
                    break;

                default:
                    // all other operations will be handled by the LiteGame implementation
                    base.ExecuteOperation(peer, operationRequest, sendParameters);
                    break;
            }
        }

        private void HandleMyGameOperation(LitePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            var requestContract = new MyGameRequest(peer.Protocol, operationRequest);
            requestContract.OnStart();

            var responseContract = new MyGameResponse();
            responseContract.Response = "You are in game " + this.Name;

            var operationResponse = new OperationResponse(operationRequest.OperationCode, responseContract);
            peer.SendOperationResponse(operationResponse, sendParameters);

            requestContract.OnComplete();
        }
    }
}
