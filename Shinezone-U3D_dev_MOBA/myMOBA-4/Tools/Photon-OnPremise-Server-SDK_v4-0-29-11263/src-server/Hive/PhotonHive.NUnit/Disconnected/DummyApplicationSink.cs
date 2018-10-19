// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DummyApplicationSink.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Tests.Disconnected
{
    using System;

    using PhotonHostRuntimeInterfaces;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DummyApplicationSink : IPhotonApplicationSink
    {
        public bool BroadcastEvent(IPhotonPeer[] peerList, byte[] data, MessageReliablity reliability, byte channelId, MessageContentType messageContentType, out SendResults[] results)
        {

            results = new SendResults[peerList.Length];

            for (int i = 0; i < peerList.Length; i++)
            {
                results[i] = peerList[i].Send(data, reliability, channelId, messageContentType);
            }

            return true; 
        }

        public IPhotonPeer Connect(string ipAddress, ushort port, object userData)
        {
            return new DummyPeer(); 
        }

        public IPhotonPeer ConnectMux(string ipAddress, ushort port, object userData)
        {
            throw new NotImplementedException();
        }

        public IPhotonPeer ConnectENet(string ipAddress, ushort port, byte channelCount, object userData, object mtu)
        {
            throw new NotImplementedException();
        }

        public IPhotonPeer ConnectHixie76WebSocket(string ipAddress, ushort port, string url, string origin, object userData)
        {
            throw new NotImplementedException();
        }

        public IPhotonPeer ConnectWebSocket(string ipAddress, ushort port, WebSocketVersion version, string url, string subProtocols, object userData)
        {
            throw new NotImplementedException();
        }
    }
}
