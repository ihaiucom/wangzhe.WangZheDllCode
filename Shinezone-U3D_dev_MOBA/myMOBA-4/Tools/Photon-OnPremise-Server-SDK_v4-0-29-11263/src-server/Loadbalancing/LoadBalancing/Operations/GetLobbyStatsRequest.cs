
namespace Photon.LoadBalancing.Operations
{
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    public class GetLobbyStatsRequest : Operation
    {
        public GetLobbyStatsRequest()
        {
        }

        public GetLobbyStatsRequest(IRpcProtocol protocol, OperationRequest request)
            :base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.LobbyName, IsOptional = true)]
        public string[] LobbyNames { get; set; }

        [DataMember(Code = (byte)ParameterCode.LobbyType, IsOptional = true)]
        public byte[] LobbyTypes { get; set; }
    }
}
