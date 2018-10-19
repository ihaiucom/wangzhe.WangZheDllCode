// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeerListener.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The peer listener.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Tests.Connected
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using ExitGames.Client.Photon;
    using ExitGames.Logging;

    using Photon.MmoDemo.Common;

    public class PeerListener : IPhotonPeerListener
    {
        #region Constants and Fields

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Action<PeerListener, bool> connectHandler;

        private readonly List<EventData> eventList = new List<EventData>();

        private readonly List<OperationResponse> responseList = new List<OperationResponse>();

        private readonly string username;

        private static long eventsReceived;

        private static long responseReceived;

        #endregion

        #region Constructors and Destructors

        public PeerListener(string username, Action<PeerListener, bool> connectHandler)
        {
            this.username = username;
            this.connectHandler = connectHandler;
        }

        #endregion

        #region Properties

        public static long EventsReceived
        {
            get
            {
                return Interlocked.Read(ref eventsReceived);
            }
        }

        public List<EventData> EventList
        {
            get
            {
                return this.eventList;
            }
        }

        public List<OperationResponse> ResponseList
        {
            get
            {
                return this.responseList;
            }
        }

        #endregion

        #region Public Methods

        public static void ResetStats()
        {
            Interlocked.Exchange(ref eventsReceived, 0);
            Interlocked.Exchange(ref responseReceived, 0);
        }

        #endregion

        #region Implemented Interfaces

        #region IPhotonPeerListener

        public void DebugReturn(DebugLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case DebugLevel.ALL:
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug(message);
                        }

                        break;
                    }

                case DebugLevel.INFO:
                    {
                        if (log.IsInfoEnabled)
                        {
                            log.Info(message);
                        }

                        break;
                    }

                case DebugLevel.ERROR:
                    {
                        if (log.IsErrorEnabled)
                        {
                            log.Error(message);
                        }

                        break;
                    }

                case DebugLevel.WARNING:
                    {
                        if (log.IsWarnEnabled)
                        {
                            log.Warn(message);
                        }

                        break;
                    }
            }
        }

        public void OnEvent(EventData ev)
        {
            // service() is executed in event fiber 
            this.AddEvent(ev);
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            this.AddResponse(operationResponse);
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            switch (statusCode)
            {
                case StatusCode.Connect:
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("PeerStatusCallback: Connect");
                        }

                        this.connectHandler(this, true);
                        return;
                    }

                case StatusCode.Disconnect:
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("PeerStatusCallback: Disconnect");
                        }

                        return;
                    }
            }

            log.ErrorFormat("PeerStatusCallback: {0}", statusCode);
        }

        public void OnMessage(object messages)
        {
            //
        }

        #endregion

        #endregion

        #region Methods

        private void AddEvent(EventData eventData)
        {
            Interlocked.Increment(ref eventsReceived);
            lock (this.eventList)
            {
                this.eventList.Add(eventData);
            }

            if (log.IsDebugEnabled)
            {
                if (eventData.Parameters.ContainsKey((byte)ParameterCode.ItemId))
                {
                    log.DebugFormat(
                        "{0} receives event, {1} total - code {2}, source {3}", 
                        this.username, 
                        this.eventList.Count, 
                        (EventCode)eventData.Code, 
                        eventData[(byte)ParameterCode.ItemId]);
                }
                else
                {
                    log.DebugFormat("{0} receives event, {1} total - code {2}", this.username, this.eventList.Count, (EventCode)eventData.Code);
                }
            }
        }

        private void AddResponse(OperationResponse response)
        {
            Interlocked.Increment(ref responseReceived);
            this.responseList.Add(response);

            if (response.ReturnCode != (int)ReturnCode.Ok)
            {
                log.ErrorFormat("ERR {0}, OP {1}, DBG {2}", (ReturnCode)(int)response.ReturnCode, (OperationCode)response.OperationCode, response.DebugMessage);
            }
            else if (log.IsDebugEnabled)
            {
                if (response.Parameters.ContainsKey((byte)ParameterCode.ItemId))
                {
                    log.DebugFormat(
                        "{0} receives response, {1} total - code {2}, source {3}", 
                        this.username, 
                        this.responseList.Count, 
                        (EventCode)response.OperationCode, 
                        response[(byte)ParameterCode.ItemId]);
                }
                else
                {
                    log.DebugFormat("{0} receives response, {1} total - code {2}", this.username, this.responseList.Count, (EventCode)response.OperationCode);
                }
            }
        }

        #endregion
    }
}