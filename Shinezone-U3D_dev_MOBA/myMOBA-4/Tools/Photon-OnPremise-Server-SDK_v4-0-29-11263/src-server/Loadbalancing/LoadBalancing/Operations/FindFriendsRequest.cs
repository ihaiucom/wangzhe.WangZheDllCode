
namespace Photon.LoadBalancing.Operations
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class FindFriendsRequest : Operation
    {
        public FindFriendsRequest()
        {
        }

        public FindFriendsRequest(IRpcProtocol protocol, OperationRequest operationRequest)
            : base(protocol, operationRequest)
        {
        }

        [DataMember(Code=1, IsOptional=false)]
        public string[] UserList { get; set; }
    }
}
