namespace Photon.Hive.Plugin
{
    /// <summary>
    /// Internal plugin errors codes.
    /// </summary>
    public static class ErrorCodes
    {
        /// <summary>
        /// Indicates that a callback process method was not called.
        /// </summary>
        public const short MissingCallProcessing = 0;
        /// <summary>
        /// Indicates that an unhandled exception has occured.
        /// </summary>
        public const short UnhandledException = 1;
        /// <summary>
        /// Indicates that an exeption has occured in an asynchronous callback.
        /// </summary>
        public const short AsyncCallbackException = 2;
        /// <summary>
        /// Indicates that preconditions of SetProperties were not met.
        /// </summary>
        public const short SetPropertiesPreconditionsFail = 3;
        /// <summary>
        /// Indicates that CAS ("Check And Swap") failed when setting updated properties.
        /// </summary>
        public const short SetPropertiesCASFail = 4;
        /// <summary>
        /// Indicates that an exception has occurred when updating properties.
        /// </summary>
        public const short SetPropertiesException = 5;
    }
}
