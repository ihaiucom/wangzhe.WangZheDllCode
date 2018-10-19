using System.Collections.Generic;
using Photon.SocketServer;

namespace Photon.LoadBalancing.Operations
{
    using Photon.SocketServer.Rpc;

    public class GetLobbyStatsResponse : DataContract
    {
        public GetLobbyStatsResponse()
        {
        }

        public GetLobbyStatsResponse(IRpcProtocol protocol, IDictionary<byte, object> dataMembers)
            : base(protocol, dataMembers)
        {
        }

        [DataMember(Code = (byte)ParameterCode.LobbyName, IsOptional = true)]
        public string[] LobbyNames { get; set; }

        [DataMember(Code = (byte)ParameterCode.LobbyType, IsOptional = true)]
        public byte[] LobbyTypes { get; set; }

        [DataMember(Code = (byte)ParameterCode.PeerCount, IsOptional = false)]
        public int[] PeerCount { get; set; }

        [DataMember(Code = (byte)ParameterCode.GameCount, IsOptional = false)]
        public int[] GameCount { get; set; }
    }
}
