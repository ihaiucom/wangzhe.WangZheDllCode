// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPluginManager.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Photon.Common.Plugins
{
    public interface IPluginManager<out IPlugin, in IHost>
    {
        #region Public Methods
        bool Initialized { get; }

        Type Type4Load { get; }

        string PluginPath { get; }

        IPlugin GetPlugin(IHost sink, string pluginName);

        #endregion
    }
}