namespace Photon.Hive.Plugin.WebHooks
{
    // Note: renaming Fields will break scripts and tests
    internal class WebhooksResponse
    {
        #region Constructors and Destructors

        public WebhooksResponse()
        {
            this.ResultCode = 0xFF;
        }

        #endregion

        #region Public Properties

        public string Data { get; set; }

        public string Message { get; set; }

        public byte ResultCode { get; set; }

        public SerializableGameState State { get; set; }

        #endregion
    }
}