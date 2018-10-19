namespace Photon.Hive.Plugin
{
    /// <summary>
    /// Optional flags to be used in Photon client SDKs with Op RaiseEvent and Op SetProperties.
    /// Introduced mainly for webhooks 1.2 to control behavior of forwarded HTTP requests.
    /// </summary>
    public static class WebFlags
    {
        /// <summary>
        /// Indicates whether to forward HTTP request to web service or not.
        /// </summary>
        public const byte HttpForward = 0x01;
        /// <summary>
        /// Indicates whether to send AuthCookie of actor in the HTTP request to web service or not.
        /// </summary>
        public const byte SendAuthCookie = 0x02;
        /// <summary>
        /// <see cref="HttpForward"/> and <see cref="SendAuthCookie"/> combined.
        /// </summary>
        public const byte HttpForwardWithAuthCookie = HttpForward|SendAuthCookie;
        /// <summary>
        /// Indicates whether to send HTTP request synchronously or asynchronously to web service.
        /// </summary>
        public const byte SendSync = 0x04;
        /// <summary>
        /// Indicates whether to send serialized game state in HTTP request to web service or not.
        /// </summary>
        public const byte SendState = 0x08;

        /// <summary>
        /// Helper method to indicate if <see cref="HttpForward"/> flag is set or not.
        /// </summary>
        /// <param name="flags">Web flags parameter.</param>
        /// <returns>If <see cref="HttpForward"/> flag is set or not.</returns>
        public static bool ShouldHttpForward(byte flags)
        {
            return ((flags & HttpForward) != 0);
        }

        /// <summary>
        /// Helper method to indicate if <see cref="SendAuthCookie"/> flag is set or not.
        /// </summary>
        /// <param name="flags">Web flags parameter.</param>
        /// <returns>If <see cref="SendAuthCookie"/> flag is set or not.</returns>
        public static bool ShouldSendAuthCookie(byte flags)
        {
            return ((flags & SendAuthCookie) != 0);
        }

        /// <summary>
        /// Helper method to indicate if <see cref="SendSync"/> flag is set or not.
        /// </summary>
        /// <param name="flags">Web flags parameter.</param>
        /// <returns>If <see cref="SendSync"/> flag is set or not.</returns>
        public static bool ShouldSendSync(byte flags)
        {
            return ((flags & SendSync) != 0);
        }

        /// <summary>
        /// Helper method to indicate if <see cref="SendState"/> flag is set or not.
        /// </summary>
        /// <param name="flags">Web flags parameter.</param>
        /// <returns>If <see cref="SendState"/> flag is set or not.</returns>
        public static bool ShouldSendState(byte flags)
        {
            return ((flags & SendState) != 0);
        }
    }
}
