namespace Photon.Hive.Operations
{
    public class HiveErrorMessages
    {
        public const string OperationIsNotAllowedOnThisJoinStage = "Operation is not allowed on this join stage";

        public const string PeetNotJoinedToRoom = "Room not joined";

        public const string CacheSliceNoAviable = "Requested cache slice={0} not available.";

        public const string MaxTTLExceeded = "Can not create game with EmptyRoomTtl={0} max allowed is {1}.";

        public const string InvalidReceiverGroup = "Invalid ReceiverGroup ";

        public const string ActorNotFound = "Actor with number {0} not found.";

        public const string HttpForwardedOperationsLimitReached = "Limit ({0} per second) of operation with HttpForward flag are reached";

        public const string CantAddSlots = "Server can not add expected users to game";

        public const string UserAlreadyJoined = "Join failed: UserId '{0}' already joined the specified game (JoinMode={1}).";

        public const string GameIdDoesNotExist = "Game does not exists";

        public const string GameClosed = "Game closed";

        public const string GameFull = "Game full";

        public const string ReinitGameFailed = "Reinit game failed";

        public const string InvalidOperationCode = "Invalid operation code";

        public const string UserNotFound = "User does not exist in this game";

        public const string CanNotUseRejoinOrJoinIfPlayerExpected = "Expected users does not support JoinMode=2";

        public const string JoinFailedFoundExcludedUserId = "UserId found in excluded list";

        public const string GameAlreadyExist = "A game with the specified id already exist.";
    }
}
