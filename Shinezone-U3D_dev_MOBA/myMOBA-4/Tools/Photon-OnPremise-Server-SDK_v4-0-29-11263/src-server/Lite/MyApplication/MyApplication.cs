// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyApplication.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Example application to show how to extend the Lite application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MyApplication
{
    using System.Reflection;

    using Lite;

    using log4net;

    using Photon.SocketServer;

    /// <summary>
    ///   Example application to show how to extend the Lite application.
    /// </summary>
    public class MyApplication : LiteApplication
    {
        #region Constants and Fields

        /// <summary>
        ///   An <see cref = "ILog" /> instance used to log messages to the log4net framework.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Methods

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new MyPeer(initRequest);
        }

        #endregion
    }
}