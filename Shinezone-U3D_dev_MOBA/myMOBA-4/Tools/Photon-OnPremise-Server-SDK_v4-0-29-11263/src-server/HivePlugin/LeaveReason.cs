namespace Photon.Hive.Plugin
{
    /// <summary>
    /// Code of the reasons why a peer may leave a room.
    /// </summary>
    public static class LeaveReason
    {
        /// <summary>
        /// Indicates that the client called Disconnect()
        /// </summary>
        public const byte ClientDisconnect = 0;
        
        /// <summary>
        ///  Indicates that client has timed-out server. This is valid only when using UDP/ENET.
        /// </summary>
        public const byte ClientTimeoutDisconnect = 1;
        
        /// <summary>
        /// Indicates client is too slow to handle data sent.
        /// </summary>
        public const byte ManagedDisconnect = 2;
        
        /// <summary>
        /// Indicates low level protocol error which can be caused by data corruption.
        /// </summary>
        public const byte ServerDisconnect = 3;
        
        /// <summary>
        /// Indicates that the server has timed-out client. 
        /// </summary>
        public const byte TimeoutDisconnect = 4;

        /// <summary>
        /// TBD: Not used currently.
        /// </summary>
        public const byte ConnectTimeout = 5;

        /// <summary>
        /// TBD: Not used currently.
        /// </summary>
        public const byte SwitchRoom = 100;
        
        /// <summary>
        /// Indicates that the client called OpLeave().
        /// </summary>
        public const byte LeaveRequest = 101;

        /// <summary>
        /// Indicates that the inactive actor timed-out, meaning the PlayerTtL of the room expired for that actor. 
        /// </summary>
        public const byte PlayerTtlTimedOut = 102;

        /// <summary>
        /// Indicates a very unusual scenario where the actor did not send anything to Photon Servers for 5 minutes. 
        /// Normally peers timeout long before that but Photon does a check for every connected peer's timestamp of 
        /// the last exchange with the servers (called LastTouch) every 5 minutes.
        /// </summary>
        public const byte PeerLastTouchTimedout = 103;

        /// <summary>
        /// Indicates that the actor was removed from ActorList by a plugin.
        /// </summary>
        public const byte PluginRequest = 104;

        /// <summary>
        /// Indicates an internal error in a plugin implementation.
        /// </summary>
        public const byte PluginFailedJoin = 105;

        /// <summary>
        /// Stringify the leave reason code
        /// </summary>
        /// <param name="reason">Leave reason code</param>
        /// <returns>readable form of the leave reason</returns>
        public static string ToString(int reason)
        {
            switch (reason)
            {
                case ClientDisconnect:
                {
                    return "ClientDisconnect";
                }
                case ClientTimeoutDisconnect:
                {
                    return "ClientTimeoutDisconnect";
                }
                case ManagedDisconnect:
                {
                    return "ManagedDisconnect";
                }
                case ServerDisconnect:
                {
                    return "ServerDisconnect";
                }
                case TimeoutDisconnect:
                {
                    return "TimeoutDisconnect";
                }
                case ConnectTimeout:
                {
                    return "ConnectTimeout";
                }
                case SwitchRoom:
                {
                    return "SwitchRoom";
                }
                case LeaveRequest:
                {
                    return "LeaveRequest";
                }
                case PlayerTtlTimedOut:
                {
                    return "PlayerTtlTimedOut";
                }
                case PeerLastTouchTimedout:
                {
                    return "PeerLastTouchTimedout";
                }
                case PluginRequest:
                {
                    return "PluginRequest";
                }
                case PluginFailedJoin:
                {
                    return "PluginFailedJoin";
                }
                default:
                {
                    return "Unknown:" + reason;
                }
            }
        }
    }
}