namespace Photon.Common.Authentication
{
    public static class ErrorMessages
    {
        /// <summary>
        ///     If appId is set to null.
        /// </summary>
        public const string AppIdMissing = "Application id not set";

        public const string EmptyAppId = "Empty application id";

        public const string InternalError = "Internal server error";

        public const string InvalidAppIdFormat = "Invalid application id format";

        public const string InvalidAppId = "Invalid application id";

        public const string AuthTokenMissing = "Authentication token is missing";

        public const string AuthTokenInvalid = "Invalid authentication token";

        public const string AuthTokenEncryptionInvalid = "Invalid authentication token encryption";

        public const string AuthTokenExpired = "Authentication token expired";

        public const string AuthTokenTypeNotSupported = "Authentication token type not supported";

        public const string ProtocolNotSupported = "Network protocol not supported";

        public const string EmptyUserId = "UserId is null or empty";

        public const string InvalidTypeForAuthData = "Invalid type for auth data";
    }
}