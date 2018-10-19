namespace Lite.Tests.Disconnected
{
    using Photon.SocketServer;

    using PhotonHostRuntimeInterfaces;

    class TestLitePeer : LitePeer
    {
        public TestLitePeer(IRpcProtocol rpcProtocol, IPhotonPeer nativePeer)
            : this(new InitRequest(rpcProtocol, nativePeer))
        {
        }
        public TestLitePeer(InitRequest initRequest)
            : base(initRequest)
        {
            this.Initialize(initRequest);
        }
    }
}
