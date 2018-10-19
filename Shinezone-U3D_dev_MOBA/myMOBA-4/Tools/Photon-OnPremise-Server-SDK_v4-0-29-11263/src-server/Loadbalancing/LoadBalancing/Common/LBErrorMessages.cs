using Photon.Hive.Operations;

namespace Photon.LoadBalancing.Common
{
    public class LBErrorMessages : HiveErrorMessages
    {
        public const string LobbyNotExist = "Lobby does not exists";
        public const string CanNotCreateLobby = "Cannot create lobby";
        public const string LobbyTypesLenDoNotMatchLobbyNames = "LobbyTypes lenght does not match LobbyNames lenght";
        public const string LobbyTypesNotSet = "Lobby types not set";
        public const string FailedToGetServerInstance = "Failed to get server instance.";
        public const string LobbyNotJoined = "Lobby not joined";
        public const string UnknownOperationCode = "Unknown operation code";
        public const string NotAuthorized = "Not authorized";
        public const string Authenticating = "Already authenticating";
        public const string AlreadyAuthenticated = "Already authenticated";
        public const string RpcIsNotEnabled = "Rpc is not enabled";
        public const string RpcIsNotSetup = "Rpc Service isn't setup";
    }
}
