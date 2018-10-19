namespace Photon.Common
{
    public enum ErrorCode : short
    {
        InvalidRequestParameters = -6,
        ArgumentOutOfRange = -4,

        OperationDenied = -3,
        OperationInvalid = -2,
        InternalServerError = -1, 

        Ok = 0,

        InvalidAuthentication = 32767, // 0x7FFF, // codes start at short.MaxValue 
        GameIdAlreadyExists = 32766, // 0x7FFF - 1,
        GameFull = 32765, // 0x7FFF - 2,
        GameClosed = 32764, // 0x7FFF - 3,
        AlreadyMatched = 32763, // 0x7FFF - 4,
        ServerFull = 32762, // 0x7FFF - 5,
        UserBlocked = 32761, // 0x7FFF - 6,
        NoMatchFound = 32760, // 0x7FFF - 7,
        RedirectRepeat = 32759, // 0x7FFF - 8,
        GameIdNotExists = 32758, // 0x7FFF - 9,

        // for authenticate requests. Indicates that the max ccu limit has been reached
        MaxCcuReached = 32757, // 0x7FFF - 10,

        // for authenticate requests. Indicates that the application is not subscribed to this region / private cloud. 
        InvalidRegion = 32756, // 0x7FFF - 11,

        // for authenticate requests. Indicates that the call to the external authentication service failed.
        CustomAuthenticationFailed = 32755, // 0x7FFF - 12,

        AuthenticationTokenExpired = 32753, // 0x7FFF - 14,
        // for authenticate requests. Indicates that the call to the external authentication service failed.

        PluginReportedError = 32752, //0x7FFF - 15,
        PluginMismatch = 32751, // 0x7FFF - 16,

        JoinFailedPeerAlreadyJoined = 32750, // 0x7FFF - 17,
        JoinFailedFoundInactiveJoiner = 32749, // 0x7FFF - 18,
        JoinFailedWithRejoinerNotFound = 32748, // 0x7FFF - 19,
        JoinFailedFoundExcludedUserId = 32747, // 0x7FFF - 20,
        JoinFailedFoundActiveJoiner = 32746, // 0x7FFF - 21,

        HttpLimitReached = 32745, // 0x7FFF - 22,
        ExternalHttpCallFailed = 32744, // 0x7FFF - 23,
    }
}
