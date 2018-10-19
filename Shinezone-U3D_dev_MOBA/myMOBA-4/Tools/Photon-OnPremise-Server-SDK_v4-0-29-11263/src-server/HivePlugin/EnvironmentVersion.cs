using System;

namespace Photon.Hive.Plugin
{
    /// <summary>
    /// Contains version of PhotonHivePlugin at build time and currently running
    /// </summary>
    public class EnvironmentVersion
    {
        /// <summary>
        /// version of PhotonHivePlugin provided by the host 
        /// </summary>
        public Version HostVersion;
        /// <summary>
        /// version of PhotonHivePlugin which was used to build plugin
        /// </summary>
        public Version BuiltWithVersion;
    }
}
