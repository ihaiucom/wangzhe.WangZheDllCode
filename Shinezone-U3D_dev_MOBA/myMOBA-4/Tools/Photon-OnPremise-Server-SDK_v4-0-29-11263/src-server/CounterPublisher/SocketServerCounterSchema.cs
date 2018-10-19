// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocketServerCounterSchema.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SocketServerCounterSchema type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace Photon.CounterPublisher
{
    public static class SocketServerCounterSchema
    {
        public static class Common
        {
            #region Constants and Fields

            public static readonly string BuffersInUseCounter = "IO Buffers In Use";

            public static readonly string BuffersInUseMinusPerSecondCounter = "IO Buffers In Use -/sec";

            public static readonly string BuffersInUsePlusPerSecondCounter = "IO Buffers In Use +/sec";

            public static readonly string BytesInCounter = "Bytes in";

            public static readonly string BytesInPerSecondCounter = "Bytes in/sec";

            public static readonly string BytesOutCounter = "Bytes out";

            public static readonly string BytesOutPerSecondCounter = "Bytes out/sec";

            public static readonly string CategoryName = "Photon Socket Server";

            public static readonly string ConnectionsActiveCounter = "Connections Active";

            public static readonly string MessagesInCounter = "Messages in";

            public static readonly string MessagesInCounterPerSecond = "Messages in/sec";

            public static readonly string MessagesOutCounter = "Messages out";

            public static readonly string MessagesOutCounterPerSecond = "Messages out/sec";

            public static readonly string PeersTotalCounter = "Peers";

            public static readonly string SocketsInUseCounter = "Sockets In Use";

            public static readonly string SocketsInUseMinusPerSecondCounter = "Sockets In Use -/sec";

            public static readonly string SocketsInUsePlusPerSecondCounter = "Sockets In Use +/sec";

            public static readonly string TotalBuffersCounter = "IO Buffers Total";

            public static readonly string TotalBuffersMinusPerSecondCounter = "IO Buffers Total -/sec";

            public static readonly string TotalBuffersPlusPerSecondCounter = "IO Buffers Total +/sec";

            public static readonly string TotalPeersCounterAllTime = "Peers (all time)";

            public static readonly string TotalPeersCounterMinusPerSecond = "Peers -/sec";

            public static readonly string TotalPeersCounterPlusPerSecond = "Peers +/sec";

            public static readonly string TotalPeersDisconnectedByAppCounter = "Disconnected Peers (M)";

            public static readonly string TotalPeersDisconnectedByAppCounterAllTime = "Disconnected Peers (M) (all time)";

            public static readonly string TotalPeersDisconnectedByAppCounterMinusPerSecond = "Disconnected Peers (M) -/sec";

            public static readonly string TotalPeersDisconnectedByAppCounterPlusPerSecond = "Disconnected Peers (M) +/sec";

            public static readonly string TotalPeersDisconnectedByClientCounter = "Disconnected Peers (C)";

            public static readonly string TotalPeersDisconnectedByClientCounterAllTime = "Disconnected Peers (C) (all time)";

            public static readonly string TotalPeersDisconnectedByClientCounterMinusPerSecond = "Disconnected Peers (C) -/sec";

            public static readonly string TotalPeersDisconnectedByClientCounterPlusPerSecond = "Disconnected Peers (C) +/sec";

            public static readonly string TotalPeersDisconnectedByServerCounter = "Disconnected Peers (S)";

            public static readonly string TotalPeersDisconnectedByServerCounterAllTime = "Disconnected Peers (S) (all time)";

            public static readonly string TotalPeersDisconnectedByServerCounterMinusPerSecond = "Disconnected Peers (S) -/sec";

            public static readonly string TotalPeersDisconnectedByServerCounterPlusPerSecond = "Disconnected Peers (S) +/sec";

            public static readonly string TotalPeersDisconnectedByTimeoutCounter = "Disconnected Peers (T)";

            public static readonly string TotalPeersDisconnectedByTimeoutCounterAllTime = "Disconnected Peers (T) (all time)";

            public static readonly string TotalPeersDisconnectedByTimeoutCounterMinusPerSecond = "Disconnected Peers (T) -/sec";

            public static readonly string TotalPeersDisconnectedByTimeoutCounterPlusPerSecond = "Disconnected Peers (T) +/sec";

            public static readonly string TotalPeersDisconnectedByConnectTimeoutCounter = "Disconnected Peers (CT)";

            public static readonly string TotalPeersDisconnectedByConnectTimeoutCounterAllTime = "Disconnected Peers (CT) (all time)";

            public static readonly string TotalPeersDisconnectedByConnectTimeoutCounterMinusPerSecond = "Disconnected Peers (CT) -/sec";

            public static readonly string TotalPeersDisconnectedByConnectTimeoutCounterPlusPerSecond = "Disconnected Peers (CT) +/sec";

            public static readonly string TotalPeersDisconnectedCounter = "Disconnected Peers";

            public static readonly string TotalPeersDisconnectedCounterAllTime = "Disconnected Peers (all time)";

            public static readonly string TotalPeersDisconnectedCounterMinusPerSecond = "Disconnected Peers -/sec";

            public static readonly string TotalPeersDisconnectedCounterPlusPerSecond = "Disconnected Peers +/sec";

            public static readonly string TotalSocketsCounter = "Sockets Total";

            public static readonly string TotalSocketsMinusPerSecondCounter = "Sockets Total -/sec";

            public static readonly string TotalSocketsPlusPerSecondCounter = "Sockets Total +/sec";

            #endregion
        }

        public static class Enet
        {
            #region Constants and Fields

            public static readonly string ACKsInCounter = "Acknowledgements in";

            public static readonly string ACKsInPerSecondCounter = "Acknowledgements in/sec";

            public static readonly string ACKsOutCounter = "Acknowledgements out";

            public static readonly string ACKsOutPerSecondCounter = "Acknowledgements out/sec";

            public static readonly string CategoryName = "Photon Socket Server: ENet";

            public static readonly string ClientTimeoutDisconnects = "Client Timeout disconnects";

            public static readonly string ClientTimeoutDisconnectsPerSecondCounter = "Client Timeout disconnects/sec";

            public static readonly string CommandsInPerSecondCounter = "Commands in/sec";

            public static readonly string CommandsOutPerSecondCounter = "Commands out/sec";

            public static readonly string CommandsResentCounter = "Reliable Commands (out) resent";

            public static readonly string CommandsResentPerSecondCounter = "Reliable Commands (out) resent/sec";

            public static readonly string EnetTimerEventsPerSecondCounter = "Timer Events/sec";

            public static readonly string EnetTimersActiveCounter = "Timers Active";

            public static readonly string EnetTimersCancelledPerSecondCounter = "Timers Cancelled/sec";

            public static readonly string EnetTimersCreatedPerSecondCounter = "Timers Created/sec";

            public static readonly string EnetTimersDestroyedPerSecondCounter = "Timers Destroyed/sec";

            public static readonly string EnetTimersResetPerSecondCounter = "Timers Reset/sec";

            public static readonly string EnetTimersSetCounter = "Timers Set";

            public static readonly string EnetTimersSetPerSecondCounter = "Timers Set/sec";

            public static readonly string IncomingReliableCommandDroppedPerSecondCounter = "Reliable commands (in) dropped/sec";

            public static readonly string IncomingUnreliableCommandDroppedPerSecondCounter = "Unreliable commands (in) dropped/sec";

            public static readonly string PingsInCounter = "Pings in";

            public static readonly string PingsInPerSecondCounter = "Pings in/sec";

            public static readonly string PingsOutCounter = "Pings out";

            public static readonly string PingsOutPerSecondCounter = "Pings out/sec";

            public static readonly string RateLimitQueueBytesAddedPerSecondCounter = "Transmit Rate Limit Bytes Queued +/sec";

            public static readonly string RateLimitQueueBytesCounter = "Transmit Rate Limit Bytes Queued";

            public static readonly string RateLimitQueueBytesDiscardedCounter = "Transmit Rate Limit Bytes Discarded";

            public static readonly string RateLimitQueueBytesRemovedPerSecondCounter = "Transmit Rate Limit Bytes Queued -/sec";

            public static readonly string RateLimitQueueMessagesAddedPerSecondCounter = "Transmit Rate Limit Messages Queued +/sec";

            public static readonly string RateLimitQueueMessagesCounter = "Transmit Rate Limit Messages Queued";

            public static readonly string RateLimitQueueMessagesDiscardedCounter = "Transmit Rate Limit Messages Discarded";

            public static readonly string RateLimitQueueMessagesRemovedPerSecondCounter = "Transmit Rate Limit Messages Queued -/sec";

            public static readonly string ReliableCommandsInPerSecondCounter = "Reliable commands in/sec";

            public static readonly string ReliableCommandsOutPerSecondCounter = "Reliable commands out/sec";

            public static readonly string ReliableCommandsQueuedInCounter = "Reliable commands (in, fragments) queued";

            public static readonly string ReliableCommandsQueuedOutCounter = "Reliable commands (out) queued";

            public static readonly string TimeSpentInServerInCounter = "Time Spent In Server: In (ms)";

            //// public static readonly string timeSpentInServerInAverage = "Time Spent In Server: In (ms, Average)";
            //// public static readonly string timeSpentInServerInAverageBase = "Time Spent In Server: In (ms, Average Base Not Displayed)";
            public static readonly string TimeSpentInServerOutCounter = "Time Spent In Server: Out (ms)";

            public static readonly string TimeoutDisconnectCounter = "Timeout disconnects";

            public static readonly string TimeoutDisconnectPerSecondCounter = "Timeout disconnects/sec";

            public static readonly string UnreliableCommandsInPerSecondCounter = "Unreliable commands in/sec";

            public static readonly string UnreliableCommandsOutPerSecondCounter = "Unreliable commands out/sec";

            public static readonly string UnreliableCommandsThrottledPerSecondCounter = "Unreliable commands (out) throttled/sec";

            public static readonly string WindowLimitQueueBytesAddedPerSecondCounter = "Transmit Window Limit Bytes Queued +/sec";

            public static readonly string WindowLimitQueueBytesCounter = "Transmit Window Limit Bytes Queued";

            public static readonly string WindowLimitQueueBytesDiscardedCounter = "Transmit Window Limit Bytes Discarded";

            public static readonly string WindowLimitQueueBytesRemovedPerSecondCounter = "Transmit Window Limit Bytes Queued -/sec";

            public static readonly string WindowLimitQueueMessagesAddedPerSecondCounter = "Transmit Window Limit Messages Queued +/sec";

            public static readonly string WindowLimitQueueMessagesCounter = "Transmit Window Limit Messages Queued";

            public static readonly string WindowLimitQueueMessagesDiscardedCounter = "Transmit Window Limit Messages Discarded";

            public static readonly string WindowLimitQueueMessagesRemovedPerSecondCounter = "Transmit Window Limit Messages Queued -/sec";

            public static readonly string DatagramValidationFailuresCounter = "Datagram validation failures";

            public static readonly string DatagramValidationFailuresPerSecondCounter = "Datagram validation failures/sec";

            public static readonly string PortChangeCounter = "Port changes";

            public static readonly string PortChangePerSecondCounter = "Port changes/sec";

            #endregion
        }

        public static class Tcp
        {
            #region Constants and Fields

            public static readonly string CategoryName = "Photon Socket Server: TCP";

            public static readonly string TCPFlowControl25Counter = "TCP: Flow Control 25%";

            public static readonly string TCPFlowControl50Counter = "TCP: Flow Control 50%";

            public static readonly string TCPFlowControl75Counter = "TCP: Flow Control 75%";

            public static readonly string TCPFlowControlActiveCounter = "TCP: Flow Control Active";

            public static readonly string TCPFlowControlBufferFullCounter = "TCP: Flow Control Buffer Full Events";

            public static readonly string TCPFlowControlBufferQueueCounter = "TCP: Flow Control Buffer Queue";

            public static readonly string TCPFlowControlBufferQueueMinusPerSecondCounter = "TCP: Flow Control Buffer Queue -/sec";

            public static readonly string TCPFlowControlBufferQueuePlusPerSecondCounter = "TCP: Flow Control Buffer Queue +/sec";

            public static readonly string TCPPeersCounterAllTime = "TCP: Peers (all time)";

            public static readonly string TCPPeersCounterMinusPerSecond = "TCP: Peers -/sec";

            public static readonly string TCPPeersCounterPlusPerSecond = "TCP: Peers +/sec";

            public static readonly string TCPPeersDisconnectedByAppCounter = "TCP: Disconnected Peers (M)";

            public static readonly string TCPPeersDisconnectedByAppCounterAllTime = "TCP: Disconnected Peers (M) (all time)";

            public static readonly string TCPPeersDisconnectedByAppCounterMinusPerSecond = "TCP: Disconnected Peers (M) -/sec";

            public static readonly string TCPPeersDisconnectedByAppCounterPlusPerSecond = "TCP: Disconnected Peers (M) +/sec";

            public static readonly string TCPPeersDisconnectedByClientCounter = "TCP: Disconnected Peers (C)";

            public static readonly string TCPPeersDisconnectedByClientCounterAllTime = "TCP: Disconnected Peers (C) (all time)";

            public static readonly string TCPPeersDisconnectedByClientCounterMinusPerSecond = "TCP: Disconnected Peers (C) -/sec";

            public static readonly string TCPPeersDisconnectedByClientCounterPlusPerSecond = "TCP: Disconnected Peers (C) +/sec";

            public static readonly string TCPPeersDisconnectedByServerCounter = "TCP: Disconnected Peers (S)";

            public static readonly string TCPPeersDisconnectedByServerCounterAllTime = "TCP: Disconnected Peers (S) (all time)";

            public static readonly string TCPPeersDisconnectedByServerCounterMinusPerSecond = "TCP: Disconnected Peers (S) -/sec";

            public static readonly string TCPPeersDisconnectedByServerCounterPlusPerSecond = "TCP: Disconnected Peers (S) +/sec";

            public static readonly string TCPPeersDisconnectedByTimeoutCounter = "TCP: Disconnected Peers (T)";

            public static readonly string TCPPeersDisconnectedByTimeoutCounterAllTime = "TCP: Disconnected Peers (T) (all time)";

            public static readonly string TCPPeersDisconnectedByTimeoutCounterMinusPerSecond = "TCP: Disconnected Peers (T) -/sec";

            public static readonly string TCPPeersDisconnectedByTimeoutCounterPlusPerSecond = "TCP: Disconnected Peers (T) +/sec";

            public static readonly string TCPPeersDisconnectedByConnectTimeoutCounter = "TCP: Disconnected Peers (CT)";

            public static readonly string TCPPeersDisconnectedByConnectTimeoutCounterAllTime = "TCP: Disconnected Peers (CT) (all time)";

            public static readonly string TCPPeersDisconnectedByConnectTimeoutCounterMinusPerSecond = "TCP: Disconnected Peers (CT) -/sec";

            public static readonly string TCPPeersDisconnectedByConnectTimeoutCounterPlusPerSecond = "TCP: Disconnected Peers (CT) +/sec";

            public static readonly string TCPPeersDisconnectedCounter = "TCP: Disconnected Peers";

            public static readonly string TCPPeersDisconnectedCounterAllTime = "TCP: Disconnected Peers (all time)";

            public static readonly string TCPPeersDisconnectedCounterMinusPerSecond = "TCP: Disconnected Peers -/sec";

            public static readonly string TCPPeersDisconnectedCounterPlusPerSecond = "TCP: Disconnected Peers +/sec";

            public static readonly string TcpBytesInCounter = "TCP: Bytes in";

            public static readonly string TcpBytesInPerSecondCounter = "TCP: Bytes in/sec";

            public static readonly string TcpBytesOutCounter = "TCP: Bytes out";

            public static readonly string TcpBytesOutPerSecondCounter = "TCP: Bytes out/sec";

            public static readonly string TcpConnectionsActiveCounter = "TCP: Connections Active";

            public static readonly string TcpMessagesInCounter = "TCP: Messages in";

            public static readonly string TcpMessagesInCounterPerSecond = "TCP: Messages in/sec";

            public static readonly string TcpMessagesOutCounter = "TCP: Messages out";

            public static readonly string TcpMessagesOutCounterPerSecond = "TCP: Messages out/sec";

            public static readonly string TcpPeersCounter = "TCP: Peers";

            #endregion
        }

        public static class Threading
        {
            #region Constants and Fields

            public static readonly string BusinessLogicQueueAddPerSecondCounter = "Business Logic Queue +/sec";

            public static readonly string BusinessLogicQueueCounter = "Business Logic Queue";

            public static readonly string BusinessLogicQueueRemovePerSecondCounter = "Business Logic Queue -/sec";

            public static readonly string BusinessLogicThreadsActiveCounter = "Business Logic Threads Active";

            public static readonly string BusinessLogicThreadsEventsPerSecondCounter = "Business Logic Threads Events/sec";

            public static readonly string BusinessLogicThreadsProcessingCounter = "Business Logic Threads Processing";

            public static readonly string CategoryName = "Photon Socket Server: Threads and Queues";

            public static readonly string EnetQueueAddPerSecondCounter = "ENet Queue +/sec";

            public static readonly string EnetQueueCounter = "ENet Queue";

            public static readonly string EnetQueueRemovePerSecondCounter = "ENet Queue -/sec";

            public static readonly string EnetThreadsActiveCounter = "ENet Threads Active";

            public static readonly string EnetThreadsEventsPerSecondCounter = "ENet Threads Events/sec";

            public static readonly string EnetThreadsProcessingCounter = "ENet Threads Processing";

            public static readonly string EnetTimerThreadEventsPerSecondCounter = "ENet Timer Thread Events/sec";

            public static readonly string EnetTimerThreadsProcessingCounter = "ENet Timer Threads Processing";

            public static readonly string IoThreadsActiveCounter = "IO Threads Active";

            public static readonly string IoThreadsEventsPerSecondCounter = "IO Threads Events/sec";

            public static readonly string IoThreadsProcessingCounter = "IO Threads Processing";

            #endregion
        }

        public static class Udp
        {
            #region Constants and Fields

            public static readonly string CategoryName = "Photon Socket Server: UDP";

            public static readonly string DatagramsInCounter = "UDP: Datagrams in";

            public static readonly string DatagramsInPerSecondCounter = "UDP: Datagrams in/sec";

            public static readonly string DatagramsOutCounter = "UDP: Datagrams out";

            public static readonly string DatagramsOutPerSecondCounter = "UDP: Datagrams out/sec";

            public static readonly string PendingRecvsCounter = "UDP: Pending Recvs";

            public static readonly string UDPPeersCounterAllTime = "UDP: Peers (all time)";

            public static readonly string UDPPeersCounterMinusPerSecond = "UDP: Peers -/sec";

            public static readonly string UDPPeersCounterPlusPerSecond = "UDP: Peers +/sec";

            public static readonly string UDPPeersDisconnectedByAppCounter = "UDP: Disconnected Peers (M)";

            public static readonly string UDPPeersDisconnectedByAppCounterAllTime = "UDP: Disconnected Peers (M) (all time)";

            public static readonly string UDPPeersDisconnectedByAppCounterMinusPerSecond = "UDP: Disconnected Peers (M) -/sec";

            public static readonly string UDPPeersDisconnectedByAppCounterPlusPerSecond = "UDP: Disconnected Peers (M) +/sec";

            public static readonly string UDPPeersDisconnectedByClientCounter = "UDP: Disconnected Peers (C)";

            public static readonly string UDPPeersDisconnectedByClientCounterAllTime = "UDP: Disconnected Peers (C) (all time)";

            public static readonly string UDPPeersDisconnectedByClientCounterMinusPerSecond = "UDP: Disconnected Peers (C) -/sec";

            public static readonly string UDPPeersDisconnectedByClientCounterPlusPerSecond = "UDP: Disconnected Peers (C) +/sec";

            public static readonly string UDPPeersDisconnectedByServerCounter = "UDP: Disconnected Peers (S)";

            public static readonly string UDPPeersDisconnectedByServerCounterAllTime = "UDP: Disconnected Peers (S) (all time)";

            public static readonly string UDPPeersDisconnectedByServerCounterMinusPerSecond = "UDP: Disconnected Peers (S) -/sec";

            public static readonly string UDPPeersDisconnectedByServerCounterPlusPerSecond = "UDP: Disconnected Peers (S) +/sec";

            public static readonly string UDPPeersDisconnectedByTimeoutCounter = "UDP: Disconnected Peers (T)";

            public static readonly string UDPPeersDisconnectedByTimeoutCounterAllTime = "UDP: Disconnected Peers (T) (all time)";

            public static readonly string UDPPeersDisconnectedByTimeoutCounterMinusPerSecond = "UDP: Disconnected Peers (T) -/sec";

            public static readonly string UDPPeersDisconnectedByTimeoutCounterPlusPerSecond = "UDP: Disconnected Peers (T) +/sec";

            public static readonly string UDPPeersDisconnectedByConnectTimeoutCounter = "UDP: Disconnected Peers (CT)";

            public static readonly string UDPPeersDisconnectedByConnectTimeoutCounterAllTime = "UDP: Disconnected Peers (CT) (all time)";

            public static readonly string UDPPeersDisconnectedByConnectTimeoutCounterMinusPerSecond = "UDP: Disconnected Peers (CT) -/sec";

            public static readonly string UDPPeersDisconnectedByConnectTimeoutCounterPlusPerSecond = "UDP: Disconnected Peers (CT) +/sec";

            public static readonly string UDPPeersDisconnectedCounter = "UDP: Disconnected Peers";

            public static readonly string UDPPeersDisconnectedCounterAllTime = "UDP: Disconnected Peers (all time)";

            public static readonly string UDPPeersDisconnectedCounterMinusPerSecond = "UDP: Disconnected Peers -/sec";

            public static readonly string UDPPeersDisconnectedCounterPlusPerSecond = "UDP: Disconnected Peers +/sec";

            public static readonly string UdpBytesInCounter = "UDP: Bytes in";

            public static readonly string UdpBytesInPerSecondCounter = "UDP: Bytes in/sec";

            public static readonly string UdpBytesOutCounter = "UDP: Bytes out";

            public static readonly string UdpBytesOutPerSecondCounter = "UDP: Bytes out/sec";

            public static readonly string UdpConnectionsActiveCounter = "UDP: Connections Active";

            public static readonly string UdpMessagesInCounter = "UDP: Messages in";

            public static readonly string UdpMessagesInCounterPerSecond = "UDP: Messages in/sec";

            public static readonly string UdpMessagesOutCounter = "UDP: Messages out";

            public static readonly string UdpMessagesOutCounterPerSecond = "UDP: Messages out/sec";

            public static readonly string UdpPeersCounter = "UDP: Peers";

            #endregion
        }

        public class CLR
        {
            #region Constants and Fields

            public static readonly string CLRM2NBroadcastCounter = "CLR: M->N Broadcast";

            public static readonly string CLRM2NBroadcastCounterPerSecond = "CLR: M->N Broadcast/sec";

            public static readonly string CLRM2NCOMCounter = "CLR: M->N COM";

            public static readonly string CLRM2NCOMCounterPerSecond = "CLR: M->N COM/sec";

            public static readonly string CLRM2NCounter = "CLR: M->N Other";

            public static readonly string CLRM2NCounterPerSecond = "CLR: M->N Other/sec";

            public static readonly string CLRM2NSendCounter = "CLR: M->N Send";

            public static readonly string CLRM2NSendCounterPerSecond = "CLR: M->N Send/sec";

            public static readonly string CLRM2NTotalCounter = "CLR: M->N Total";

            public static readonly string CLRM2NTotalCounterPerSecond = "CLR: M->N Total/sec";

            public static readonly string CLRN2MCOMCounter = "CLR: N->M COM";

            public static readonly string CLRN2MCOMCounterPerSecond = "CLR: N->M COM/sec";

            public static readonly string CLRN2MCounter = "CLR: N->M Operations";

            public static readonly string CLRN2MCounterPerSecond = "CLR: N->M Operations/sec";

            public static readonly string CLRN2MTotalCounter = "CLR: N->M Total";

            public static readonly string CLRN2MTotalCounterPerSecond = "CLR: N->M Total/sec";

            public static readonly string CategoryName = "Photon Socket Server: CLR";

            #endregion
        }

        public class Policy
        {
            #region Constants and Fields

            public static readonly string CategoryName = "Photon Socket Server: Policy file";

            public static readonly string PolicyBytesInCounter = "Policy: Bytes in";

            public static readonly string PolicyBytesInPerSecondCounter = "Policy: Bytes in/sec";

            public static readonly string PolicyBytesOutCounter = "Policy: Bytes out";

            public static readonly string PolicyBytesOutPerSecondCounter = "Policy: Bytes out/sec";

            public static readonly string PolicyConnectionsActiveCounter = "Policy: Connections Active";

            public static readonly string PolicyFailedRequestsCounter = "Policy: Failed requests";

            public static readonly string PolicyFailedRequestsCounterPerSecond = "Policy: Failed requests/sec";

            public static readonly string PolicyObjectName = "Photon Socket Server: Policy file";

            public static readonly string PolicyPeersCounter = "Policy: Peers";

            public static readonly string PolicyPeersCounterAllTime = "Policy: Peers (all time)";

            public static readonly string PolicyPeersCounterMinusPerSecond = "Policy: Peers -/sec";

            public static readonly string PolicyPeersCounterPlusPerSecond = "Policy: Peers +/sec";

            public static readonly string PolicyPeersDisconnectedByAppCounter = "Policy: Disconnected Peers (M)";

            public static readonly string PolicyPeersDisconnectedByAppCounterAllTime = "Policy: Disconnected Peers (M) (all time)";

            public static readonly string PolicyPeersDisconnectedByAppCounterMinusPerSecond = "Policy: Disconnected Peers (M) -/sec";

            public static readonly string PolicyPeersDisconnectedByAppCounterPlusPerSecond = "Policy: Disconnected Peers (M) +/sec";

            public static readonly string PolicyPeersDisconnectedByClientCounter = "Policy: Disconnected Peers (C)";

            public static readonly string PolicyPeersDisconnectedByClientCounterAllTime = "Policy: Disconnected Peers (C) (all time)";

            public static readonly string PolicyPeersDisconnectedByClientCounterMinusPerSecond = "Policy: Disconnected Peers (C) -/sec";

            public static readonly string PolicyPeersDisconnectedByClientCounterPlusPerSecond = "Policy: Disconnected Peers (C) +/sec";

            public static readonly string PolicyPeersDisconnectedByServerCounter = "Policy: Disconnected Peers (S)";

            public static readonly string PolicyPeersDisconnectedByServerCounterAllTime = "Policy: Disconnected Peers (S) (all time)";

            public static readonly string PolicyPeersDisconnectedByServerCounterMinusPerSecond = "Policy: Disconnected Peers (S) -/sec";

            public static readonly string PolicyPeersDisconnectedByServerCounterPlusPerSecond = "Policy: Disconnected Peers (S) +/sec";

            public static readonly string PolicyPeersDisconnectedByTimeoutCounter = "Policy: Disconnected Peers (T)";

            public static readonly string PolicyPeersDisconnectedByTimeoutCounterAllTime = "Policy: Disconnected Peers (T) (all time)";

            public static readonly string PolicyPeersDisconnectedByTimeoutCounterMinusPerSecond = "Policy: Disconnected Peers (T) -/sec";

            public static readonly string PolicyPeersDisconnectedByTimeoutCounterPlusPerSecond = "Policy: Disconnected Peers (T) +/sec";

            public static readonly string PolicyPeersDisconnectedCounter = "Policy: Disconnected Peers";

            public static readonly string PolicyPeersDisconnectedCounterAllTime = "Policy: Disconnected Peers (all time)";

            public static readonly string PolicyPeersDisconnectedCounterMinusPerSecond = "Policy: Disconnected Peers -/sec";

            public static readonly string PolicyPeersDisconnectedCounterPlusPerSecond = "Policy: Disconnected Peers +/sec";

            #endregion
        }

        public class S2S
        {
            #region Constants and Fields

            public static readonly string CategoryName = "Photon Socket Server: S2S";

            public static readonly string S2SBytesInCounter = "S2S: Bytes in";

            public static readonly string S2SBytesInPerSecondCounter = "S2S: Bytes in/sec";

            public static readonly string S2SBytesOutCounter = "S2S: Bytes out";

            public static readonly string S2SBytesOutPerSecondCounter = "S2S: Bytes out/sec";

            public static readonly string S2SConnectionsActiveCounter = "S2S: Connections Active";

            public static readonly string S2SFlowControl25Counter = "S2S: Flow Control 25%";

            public static readonly string S2SFlowControl50Counter = "S2S: Flow Control 50%";

            public static readonly string S2SFlowControl75Counter = "S2S: Flow Control 75%";

            public static readonly string S2SFlowControlActiveCounter = "S2S: Flow Control Active";

            public static readonly string S2SFlowControlBufferFullCounter = "S2S: Flow Control Buffer Full Events";

            public static readonly string S2SFlowControlBufferQueueCounter = "S2S: Flow Control Buffer Queue";

            public static readonly string S2SFlowControlBufferQueueMinusPerSecondCounter = "S2S: Flow Control Buffer Queue -/sec";

            public static readonly string S2SFlowControlBufferQueuePlusPerSecondCounter = "S2S: Flow Control Buffer Queue +/sec";

            public static readonly string S2SMessagesInCounter = "S2S: Messages in";

            public static readonly string S2SMessagesInCounterPerSecond = "S2S: Messages in/sec";

            public static readonly string S2SMessagesOutCounter = "S2S: Messages out";

            public static readonly string S2SMessagesOutCounterPerSecond = "S2S: Messages out/sec";

            public static readonly string S2SPeersCounter = "S2S: Peers";

            public static readonly string S2SPeersCounterAllTime = "S2S: Peers (all time)";

            public static readonly string S2SPeersCounterMinusPerSecond = "S2S: Peers -/sec";

            public static readonly string S2SPeersCounterPlusPerSecond = "S2S: Peers +/sec";

            public static readonly string S2SPeersDisconnectedByAppCounter = "S2S: Disconnected Peers (M)";

            public static readonly string S2SPeersDisconnectedByAppCounterAllTime = "S2S: Disconnected Peers (M) (all time)";

            public static readonly string S2SPeersDisconnectedByAppCounterMinusPerSecond = "S2S: Disconnected Peers (M) -/sec";

            public static readonly string S2SPeersDisconnectedByAppCounterPlusPerSecond = "S2S: Disconnected Peers (M) +/sec";

            public static readonly string S2SPeersDisconnectedByClientCounter = "S2S: Disconnected Peers (C)";

            public static readonly string S2SPeersDisconnectedByClientCounterAllTime = "S2S: Disconnected Peers (C) (all time)";

            public static readonly string S2SPeersDisconnectedByClientCounterMinusPerSecond = "S2S: Disconnected Peers (C) -/sec";

            public static readonly string S2SPeersDisconnectedByClientCounterPlusPerSecond = "S2S: Disconnected Peers (C) +/sec";

            public static readonly string S2SPeersDisconnectedByServerCounter = "S2S: Disconnected Peers (S)";

            public static readonly string S2SPeersDisconnectedByServerCounterAllTime = "S2S: Disconnected Peers (S) (all time)";

            public static readonly string S2SPeersDisconnectedByServerCounterMinusPerSecond = "S2S: Disconnected Peers (S) -/sec";

            public static readonly string S2SPeersDisconnectedByServerCounterPlusPerSecond = "S2S: Disconnected Peers (S) +/sec";

            public static readonly string S2SPeersDisconnectedByTimeoutCounter = "S2S: Disconnected Peers (T)";

            public static readonly string S2SPeersDisconnectedByTimeoutCounterAllTime = "S2S: Disconnected Peers (T) (all time)";

            public static readonly string S2SPeersDisconnectedByTimeoutCounterMinusPerSecond = "S2S: Disconnected Peers (T) -/sec";

            public static readonly string S2SPeersDisconnectedByTimeoutCounterPlusPerSecond = "S2S: Disconnected Peers (T) +/sec";

            public static readonly string S2SPeersDisconnectedByConnectTimeoutCounter = "S2S: Disconnected Peers (CT)";

            public static readonly string S2SPeersDisconnectedByConnectTimeoutCounterAllTime = "S2S: Disconnected Peers (CT) (all time)";

            public static readonly string S2SPeersDisconnectedByConnectTimeoutCounterMinusPerSecond = "S2S: Disconnected Peers (CT) -/sec";

            public static readonly string S2SPeersDisconnectedByConnectTimeoutCounterPlusPerSecond = "S2S: Disconnected Peers (CT) +/sec";

            public static readonly string S2SPeersDisconnectedCounter = "S2S: Disconnected Peers";

            public static readonly string S2SPeersDisconnectedCounterAllTime = "S2S: Disconnected Peers (all time)";

            public static readonly string S2SPeersDisconnectedCounterMinusPerSecond = "S2S: Disconnected Peers -/sec";

            public static readonly string S2SPeersDisconnectedCounterPlusPerSecond = "S2S: Disconnected Peers +/sec";

            #endregion
        }
    }
}