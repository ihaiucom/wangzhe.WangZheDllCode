// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemMessage.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Item messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ExitGames.Diagnostics.Counter;
using Photon.MmoDemo.Common;
using Photon.SocketServer;

namespace Photon.MmoDemo.Server
{
    /// <summary>
    /// This type of message is pubished by items sent through the item Item.DisposeChannel. 
    /// </summary>
    public sealed class ItemDisposedMessage
    {
        public ItemDisposedMessage(Item source)
        {
            this.Source = source;
        }
        public Item Source { get; private set; }
    }

    /// <summary>
    /// This message type contains EventData to be sent to clients.
    /// ItemEventMessages are published through the item Item.EventChannel.
    /// </summary>
    public class ItemEventMessage
    {
        public static readonly CountsPerSecondCounter CounterEventReceive = new CountsPerSecondCounter("ItemEventMessage.Receive");
        public static readonly CountsPerSecondCounter CounterEventSend = new CountsPerSecondCounter("ItemEventMessage.Send");
        public ItemEventMessage(Item source, IEventData eventData, SendParameters sendParameters)
        {
            this.Source = source;
            this.EventData = eventData;
            this.SendParameters = sendParameters;
        }
        public IEventData EventData { get; private set; }
        public SendParameters SendParameters { get; private set; }
        public Item Source { get; private set; }
    }

    /// <summary>
    /// This message contains the current position of the Item. 
    /// This type of message is published by items through the Item.PositionUpdateChannel. 
    /// </summary>
    public class ItemPositionMessage
    {
        public ItemPositionMessage(Item source, Vector position)
        {
            this.Source = source;
            this.Position = position;
        }
        public Vector Position { get; private set; }
        public Item Source { get; private set; }
    }

}
