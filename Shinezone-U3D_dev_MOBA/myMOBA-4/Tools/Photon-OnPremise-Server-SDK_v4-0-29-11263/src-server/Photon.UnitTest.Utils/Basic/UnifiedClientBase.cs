using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using NUnit.Framework;

namespace Photon.UnitTest.Utils.Basic
{
    public abstract class UnifiedClientBase : IDisposable
    {
        public int WaitTimeout;

        protected UnifiedClientBase(INUnitClient client)
        {
            this.Client = client;
        }

        public string Token { get; set; }
        public string UserId { get; set; }

        public INUnitClient Client { get; private set; }

        public bool Connected { get { return this.Client.Connected; } }
        public string RemoteEndPoint { get { return this.Client.RemoteEndPoint; } }

        public void EventQueueClear()
        {
            if (this.Client != null)
            {
                this.Client.EventQueueClear();
            }
        }

        public void OperationResponseQueueClear()
        {
            if (this.Client != null)
            {
                this.Client.OperationResponseQueueClear();
            }
        }

        public virtual void Connect(string serverAddress)
        {
            if (this.Connected)
            {
                this.Disconnect();
            }

            this.Client.Connect(serverAddress);
            Assert.IsTrue(this.Client.WaitForConnect(ConnectPolicy.WaitTime),
                "Test was unable to connect to server (addr:{0}, for specified time ({1})",serverAddress, ConnectPolicy.WaitTime);
        }

        public void Disconnect()
        {
            if (this.Client != null)
            {
                this.Client.Disconnect();
                this.Client.WaitForDisconnect();
                this.Client.EventQueueClear();
                this.Client.OperationResponseQueueClear();
            }
        }

        public EventData WaitForEvent(int millisecodsWaitTime = ConnectPolicy.WaitTime)
        {
            if (this.Client != null)
            {
                return this.Client.WaitForEvent(millisecodsWaitTime);
            }
            throw new Exception("Client is not set");
        }

        public EventData WaitForEvent(byte eventCode, int millisecodsWaitTime = ConnectPolicy.WaitTime)
        {
            var timeout = Environment.TickCount + millisecodsWaitTime;
            while (Environment.TickCount < timeout)
            {
                var eventData = this.WaitForEvent(timeout - Environment.TickCount);
                if (eventCode == eventData.Code)
                {
                    return eventData;
                }
            }
            throw new TimeoutException();
        }

        public OperationResponse WaitForOperationResponse(int milliseconsWaitTime = ConnectPolicy.WaitTime)
        {
            if (this.Client != null)
            {
                return this.Client.WaitForOperationResponse(milliseconsWaitTime);
            }
            throw new Exception("Client is not set");
        }

        public bool SendRequest(OperationRequest op)
        {
            if (this.Client != null)
            {
                return this.Client.SendRequest(op);
            }
            return false;
        }

        public bool TryWaitForEvent(int waitTime, out EventData eventData)
        {
            eventData = null;
            try
            {
                eventData = this.WaitForEvent(waitTime);
            }
            catch (TimeoutException)
            {
                return false;
            }

            return true;
        }
        public bool TryWaitForEvent(byte eventCode, int waitTime, out EventData eventData)
        {
            eventData = null;
            try
            {
                eventData = WaitForEvent(eventCode, waitTime);
            }
            catch (TimeoutException)
            {
                return false;
            }

            return true;
        }


        public OperationResponse SendRequestAndWaitForResponse(OperationRequest request, short expectedResult = 0)
        {
            this.SendRequest(request);
            var response = this.WaitForOperationResponse(this.WaitTimeout);
            Assert.AreEqual(request.OperationCode, response.OperationCode);
            if (response.ReturnCode != expectedResult)
            {
                Assert.Fail("Request failed: opCode={0}, expected return code {1} but got returnCode={2}, msg={3}", request.OperationCode, expectedResult, response.ReturnCode, response.DebugMessage);
            }

            return response;
        }

        public void CheckThereIsNoEvent(byte eventCode, int timeout = 1000)
        {
            EventData eventData;
            Assert.IsFalse(this.TryWaitForEvent(eventCode, timeout, out eventData), "Got unexpected event {0}", eventCode);
        }

        public EventData CheckThereIsEvent(byte eventCode, int timeout = 1000)
        {
            EventData eventData;
            Assert.IsTrue(this.TryWaitForEvent(eventCode, timeout, out eventData), "Did not get expected event {0}", eventCode);
            return eventData;
        }

        protected static OperationRequest CreateOperationRequest(byte operationCode)
        {
            return new OperationRequest
            {
                OperationCode = operationCode,
                Parameters = new Dictionary<byte, object>()
            };
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (this.Client != null)
            {
                this.Client.Dispose();
                this.Client = null;
            }
        }
    }
}