// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPluginManager.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Photon.Hive.Plugin
{
    public interface IPluginInstance
    {
        #region Properties

        IGamePlugin Plugin { get; }

        EnvironmentVersion Version { get; }

        #endregion
    }

    public interface IPluginManager
    {
        #region Public Methods

        IPluginInstance GetGamePlugin(IPluginHost sink, string pluginName);

        #endregion
    }
}