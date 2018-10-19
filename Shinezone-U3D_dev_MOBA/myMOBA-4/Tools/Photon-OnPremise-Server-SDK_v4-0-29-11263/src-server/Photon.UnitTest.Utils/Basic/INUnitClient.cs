using System;
using ExitGames.Client.Photon;

namespace Photon.UnitTest.Utils.Basic
{
    public interface INUnitClient : IDisposable
    {
        bool Connected { get; }
        string RemoteEndPoint { get; }

        void Connect(string serverAddress);
        void Disconnect();

        bool SendRequest(OperationRequest op);

        EventData WaitForEvent(int millisecodsWaitTime = ConnectPolicy.WaitTime);
        OperationResponse WaitForOperationResponse(int milliseconsWaitTime = ConnectPolicy.WaitTime);

        bool WaitForConnect(int timeout = ConnectPolicy.WaitTime);
        bool WaitForDisconnect(int timeout = ConnectPolicy.WaitTime);

        void EventQueueClear();
        void OperationResponseQueueClear();
    }
}
