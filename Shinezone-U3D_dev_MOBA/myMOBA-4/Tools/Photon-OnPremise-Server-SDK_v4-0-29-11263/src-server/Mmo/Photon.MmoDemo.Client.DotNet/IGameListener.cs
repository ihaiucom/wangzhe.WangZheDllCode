// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGameListener.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Interface for communication with client frontend.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client
{
    using System;

    using ExitGames.Client.Photon;
    using Photon.MmoDemo.Common;

    public interface IGameListener
    {
        bool IsDebugLogEnabled { get; }

        void LogDebug(object message);

        void LogError(object message);

        void LogInfo(object message);

        void OnCameraAttached(string itemId);

        void OnCameraDetached();

        void OnConnect();

        void OnDisconnect(StatusCode returnCode);

        void OnItemAdded(Item item);

        void OnItemRemoved(Item item);

        void OnItemSpawned(string itemId);

        void OnRadarUpdate(string itemId, ItemType itemType, Vector position, bool remove);

        void OnWorldEntered();
    }
}