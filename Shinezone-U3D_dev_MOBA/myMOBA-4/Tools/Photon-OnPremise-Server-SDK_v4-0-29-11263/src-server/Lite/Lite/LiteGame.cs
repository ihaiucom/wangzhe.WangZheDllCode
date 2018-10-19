// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiteGame.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   A <see cref="Room" /> that supports the following requests:
//   <list type="bullet">
//   <item>
//   <see cref="JoinRequest" />
//   </item>
//   <item>
//   <see cref="RaiseEventRequest" />
//   </item>
//   <item>
//   <see cref="SetPropertiesRequest" />
//   </item>
//   <item>
//   <see cref="GetPropertiesRequest" />
//   </item>
//   <item>
//   <see cref="LeaveRequest" />
//   </item>
//   </list>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Lite
{
    #region using directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Lite.Caching;
    using Lite.Diagnostics.OperationLogging;
    using Lite.Events;
    using Lite.Messages;
    using Lite.Operations;

    using Photon.SocketServer;

    #endregion

    /// <summary>
    ///   A <see cref = "Room" /> that supports the following requests:
    ///   <list type = "bullet">
    ///     <item>
    ///       <see cref = "JoinRequest" />
    ///     </item>
    ///     <item>
    ///       <see cref = "RaiseEventRequest" />
    ///     </item>
    ///     <item>
    ///       <see cref = "SetPropertiesRequest" />
    ///     </item>
    ///     <item>
    ///       <see cref = "GetPropertiesRequest" />
    ///     </item>
    ///     <item>
    ///       <see cref = "LeaveRequest" />
    ///     </item>
    ///   </list>
    /// </summary>
    public class LiteGame : Room
    {
        #region Constants and Fields
        
        protected readonly LogQueue LogQueue;

        /// <summary> 
        ///   Contains <see cref = "EventCache" />s for all actors.
        /// </summary>
        protected readonly EventCacheDictionary actorEventCache = new EventCacheDictionary();

        protected readonly RoomEventCache eventCache = new RoomEventCache();

        protected readonly Dictionary<byte, ActorGroup> actorGroups = new Dictionary<byte, ActorGroup>();

        /// <summary>
        ///   The actor number counter is increase whenever a new <see cref = "Actor" /> joins the game.
        /// </summary>
        private int actorNumberCounter;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LiteGame" /> class.
        /// </summary>
        /// <param name = "gameName">
        ///   The name of the game.
        /// </param>
        /// <param name="roomCache">
        ///   The <see cref="RoomCacheBase"/> instance to which the room belongs.
        /// </param>
        /// <param name="emptyRoomLiveTime">
        ///   A value indicating how long the room instance will be keeped alive 
        ///   in the room cache after all peers have left the room.
        /// </param>

        public LiteGame(string gameName, RoomCacheBase roomCache, int emptyRoomLiveTime = 0)
            : base(gameName, roomCache, emptyRoomLiveTime)
        {
            this.LogQueue = new LogQueue("Game " + gameName, LogQueue.DefaultCapacity);
        }

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether cached events are automaticly deleted for 
        /// actors which are leaving a room.
        /// </summary>
        protected bool DeleteCacheOnLeave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if common room events (Join, Leave) will suppressed.
        /// </summary>
        protected bool SuppressRoomEvents { get; set; }

        #region Methods

        public override string ToString()
        {
            var sb = new StringBuilder(base.ToString());

            sb.AppendFormat("Events cached: {0}", this.eventCache.Count()); 
            sb.AppendLine();
            return sb.ToString(); 
        }

        /// <summary>
        ///   Called for each operation in the execution queue.
        ///   Every <see cref = "Room" /> has a queue of incoming operations to execute. 
        ///   Per game <see cref = "ExecuteOperation" /> is never executed multi-threaded, thus all code executed here has thread safe access to all instance members.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "operationRequest">
        ///   The operation request to execute.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected override void ExecuteOperation(LitePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            try
            {
                base.ExecuteOperation(peer, operationRequest, sendParameters);

                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Executing operation {0}", (OperationCode)operationRequest.OperationCode);
                }

                switch ((OperationCode)operationRequest.OperationCode)
                {
                    case OperationCode.Join:
                        {
                            var joinRequest = new JoinRequest(peer.Protocol, operationRequest);
                            if (peer.ValidateOperation(joinRequest, sendParameters) == false)
                            {
                                return;
                            }

                            if (this.LogQueue.Log.IsDebugEnabled)
                            {

                                this.LogQueue.Add(
                                    new LogEntry(
                                        "ExecuteOperation: " + (OperationCode)operationRequest.OperationCode,
                                        "Peer=" + peer.ConnectionId));
                            }

                            joinRequest.OnStart();
                            this.HandleJoinOperation(peer, joinRequest, sendParameters);
                            joinRequest.OnComplete();
                            break;
                        }

                    case OperationCode.Leave:
                        {
                            var leaveOperation = new LeaveRequest(peer.Protocol, operationRequest);
                            if (peer.ValidateOperation(leaveOperation, sendParameters) == false)
                            {
                                return;
                            }

                            if (this.LogQueue.Log.IsDebugEnabled)
                            {

                                this.LogQueue.Add(
                                    new LogEntry(
                                        "ExecuteOperation: " + (OperationCode)operationRequest.OperationCode,
                                        "Peer=" + peer.ConnectionId));
                            }

                            leaveOperation.OnStart();
                            this.HandleLeaveOperation(peer, leaveOperation, sendParameters);
                            leaveOperation.OnComplete();
                            break;
                        }

                    case OperationCode.RaiseEvent:
                        {
                            var raiseEventOperation = new RaiseEventRequest(peer.Protocol, operationRequest);
                            if (peer.ValidateOperation(raiseEventOperation, sendParameters) == false)
                            {
                                return;
                            }

                            raiseEventOperation.OnStart();
                            this.HandleRaiseEventOperation(peer, raiseEventOperation, sendParameters);
                            raiseEventOperation.OnComplete();
                            break;
                        }

                    case OperationCode.GetProperties:
                        {
                            var getPropertiesOperation = new GetPropertiesRequest(peer.Protocol, operationRequest);
                            if (peer.ValidateOperation(getPropertiesOperation, sendParameters) == false)
                            {
                                return;
                            }

                            getPropertiesOperation.OnStart();
                            this.HandleGetPropertiesOperation(peer, getPropertiesOperation, sendParameters);
                            getPropertiesOperation.OnComplete();
                            break;
                        }

                    case OperationCode.SetProperties:
                        {
                            var setPropertiesOperation = new SetPropertiesRequest(peer.Protocol, operationRequest);
                            if (peer.ValidateOperation(setPropertiesOperation, sendParameters) == false)
                            {
                                return;
                            }

                            setPropertiesOperation.OnStart();
                            this.HandleSetPropertiesOperation(peer, setPropertiesOperation, sendParameters);
                            setPropertiesOperation.OnComplete();
                            break;
                        }

                    case OperationCode.Ping:
                        {
                            peer.SendOperationResponse(new OperationResponse { OperationCode = operationRequest.OperationCode }, sendParameters);
                            break;
                        }

                    case OperationCode.ChangeGroups:
                        {
                            var changeGroupsOperation = new ChangeGroups(peer.Protocol, operationRequest);
                            if (peer.ValidateOperation(changeGroupsOperation, sendParameters) == false)
                            {
                                return;
                            }

                            changeGroupsOperation.OnStart();
                            this.HandleChangeGroupsOperation(peer, changeGroupsOperation, sendParameters);
                            changeGroupsOperation.OnComplete();
                            break;
                        }
                    default:
                        {
                            string message = string.Format("Unknown operation code {0}", (OperationCode)operationRequest.OperationCode);
                            peer.SendOperationResponse(
                                new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = -1, DebugMessage = message }, sendParameters);

                            if (Log.IsWarnEnabled)
                            {
                                Log.Warn(message);
                            }
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///   Gets the actor for a <see cref = "LitePeer" />.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <returns>
        ///   The actor for the peer or null if no actor for the peer exists (this should not happen).
        /// </returns>
        protected Actor GetActorByPeer(LitePeer peer)
        {
            Actor actor = this.Actors.GetActorByPeer(peer);
            if (actor == null)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Actor not found for peer: {0}", peer.ConnectionId);
                }
            }

            return actor;
        }

        /// <summary>
        ///   Handles the <see cref = "GetPropertiesRequest" /> operation: Sends the properties with the <see cref = "OperationResponse" />.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "getPropertiesRequest">
        ///   The operation to handle.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected virtual void HandleGetPropertiesOperation(LitePeer peer, GetPropertiesRequest getPropertiesRequest, SendParameters sendParameters)
        {
            var response = new GetPropertiesResponse();

            // check if game properties should be returned
            if ((getPropertiesRequest.PropertyType & (byte)PropertyType.Game) == (byte)PropertyType.Game)
            {
                response.GameProperties = this.Properties.GetProperties(getPropertiesRequest.GamePropertyKeys);
            }

            // check if actor properties should be returned
            if ((getPropertiesRequest.PropertyType & (byte)PropertyType.Actor) == (byte)PropertyType.Actor)
            {
                response.ActorProperties = new Hashtable();

                if (getPropertiesRequest.ActorNumbers == null)
                {
                    foreach (Actor actor in this.Actors)
                    {
                        Hashtable actorProperties = actor.Properties.GetProperties(getPropertiesRequest.ActorPropertyKeys);
                        response.ActorProperties.Add(actor.ActorNr, actorProperties);
                    }
                }
                else
                {
                    foreach (int actorNumber in getPropertiesRequest.ActorNumbers)
                    {
                        Actor actor = this.Actors.GetActorByNumber(actorNumber);
                        if (actor != null)
                        {
                            Hashtable actorProperties = actor.Properties.GetProperties(getPropertiesRequest.ActorPropertyKeys);
                            response.ActorProperties.Add(actorNumber, actorProperties);
                        }
                    }
                }
            }

            peer.SendOperationResponse(new OperationResponse(getPropertiesRequest.OperationRequest.OperationCode, response), sendParameters);
        }

        /// <summary>
        ///   Handles the <see cref = "JoinRequest" />: Joins a peer to a room and calls <see cref = "PublishJoinEvent" />.
        ///   Before a JoinOperation reaches this point (inside a room), the <see cref = "LitePeer" /> made 
        ///   sure that it is removed from the previous Room (if there was any).
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "joinRequest">
        ///   The join operation.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        /// <returns>
        ///   The newly created (joined) actor or null if the peer already joined.
        /// </returns>
        protected virtual Actor HandleJoinOperation(LitePeer peer, JoinRequest joinRequest, SendParameters sendParameters)
        {
            if (this.IsDisposed)
            {
                // join arrived after being disposed - repeat join operation                
                if (Log.IsWarnEnabled)
                {
                    Log.WarnFormat("Join operation on disposed game. GameName={0}", this.Name);
                }

                return null;
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Join operation from IP: {0} to port: {1}", peer.RemoteIP, peer.LocalPort);
            }

            // create an new actor
            Actor actor;
            if (this.TryAddPeerToGame(peer, joinRequest.ActorNr, out actor) == false)
            {
                peer.SendOperationResponse(
                    new OperationResponse
                        {
                            OperationCode = joinRequest.OperationRequest.OperationCode, 
                            ReturnCode = -1, 
                            DebugMessage = "Peer already joined the specified game."
                        }, 
                    sendParameters);
                return null;
            }

            // check if a room removal is in progress and cancel it if so
            if (this.RemoveTimer != null)
            {
                this.RemoveTimer.Dispose();
                this.RemoveTimer = null;
            }

            // set game properties for join from the first actor (game creator)
            if (this.Actors.Count == 1)
            {
                this.DeleteCacheOnLeave = joinRequest.DeleteCacheOnLeave;
                this.SuppressRoomEvents = joinRequest.SuppressRoomEvents;

                if (joinRequest.GameProperties != null)
                {
                    this.Properties.SetProperties(joinRequest.GameProperties);
                }
            }

            // set custom actor properties if defined
            if (joinRequest.ActorProperties != null)
            {
                actor.Properties.SetProperties(joinRequest.ActorProperties);
            }

            // set operation return values and publish the response
            var joinResponse = new JoinResponse { ActorNr = actor.ActorNr };

            if (this.Properties.Count > 0)
            {
                joinResponse.CurrentGameProperties = this.Properties.GetProperties();
            }

            foreach (Actor t in this.Actors)
            {
                if (t.ActorNr != actor.ActorNr && t.Properties.Count > 0)
                {
                    if (joinResponse.CurrentActorProperties == null)
                    {
                        joinResponse.CurrentActorProperties = new Hashtable();
                    }

                    Hashtable actorProperties = t.Properties.GetProperties();
                    joinResponse.CurrentActorProperties.Add(t.ActorNr, actorProperties);
                }
            }

            peer.SendOperationResponse(new OperationResponse(joinRequest.OperationRequest.OperationCode, joinResponse), sendParameters);

            // publish join event
            this.PublishJoinEvent(peer, joinRequest);

            this.PublishEventCache(peer);

            return actor;
        }

        /// <summary>
        ///   Handles the <see cref = "LeaveRequest" /> and calls <see cref = "RemovePeerFromGame" />.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "leaveRequest">
        ///   The operation.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected virtual void HandleLeaveOperation(LitePeer peer, LeaveRequest leaveRequest, SendParameters sendParameters)
        {
            this.RemovePeerFromGame(peer, leaveRequest);
            
            // is always reliable, so it gets a response
            peer.SendOperationResponse(new OperationResponse { OperationCode = leaveRequest.OperationRequest.OperationCode }, sendParameters);                
        }

        /// <summary>
        ///   Handles the <see cref = "RaiseEventRequest" />: Sends a <see cref = "CustomEvent" /> to actors in the room.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "raiseEventRequest">
        ///   The operation
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected virtual void HandleRaiseEventOperation(LitePeer peer, RaiseEventRequest raiseEventRequest, SendParameters sendParameters)
        {
            // get the actor who send the operation request
            Actor actor = this.GetActorByPeer(peer);
            if (actor == null)
            {
                return;
            }

            sendParameters.Flush = raiseEventRequest.Flush;

            if (raiseEventRequest.Cache == (byte)CacheOperation.RemoveFromRoomCache)
            {
                this.eventCache.RemoveEvents(raiseEventRequest);
                var response = new OperationResponse(raiseEventRequest.OperationRequest.OperationCode) { ReturnCode = 0 };
                peer.SendOperationResponse(response, sendParameters);
                return;
            }
            
            if (raiseEventRequest.Cache == (byte)CacheOperation.RemoveFromCacheForActorsLeft)
            {
                var currentActorNumbers = this.Actors.GetActorNumbers();
                this.eventCache.RemoveEventsForActorsNotInList(currentActorNumbers);
                var response = new OperationResponse(raiseEventRequest.OperationRequest.OperationCode) { ReturnCode = 0 };
                peer.SendOperationResponse(response, sendParameters);
                return;
            }

            // publish the custom event
            var customEvent = new CustomEvent(actor.ActorNr, raiseEventRequest.EvCode, raiseEventRequest.Data);

            bool updateEventCache = false;
            IEnumerable<Actor> recipients;

            if (raiseEventRequest.Actors != null && raiseEventRequest.Actors.Length > 0)
            {
                recipients = this.Actors.GetActorsByNumbers(raiseEventRequest.Actors);
            }
            else if (raiseEventRequest.Group != 0)
            {
                ActorGroup group;
                if (this.actorGroups.TryGetValue(raiseEventRequest.Group, out group))
                {
                    recipients = group.GetExcludedList(actor);
                }
                else
                {
                    return;
                }
            }
            else
            {
                switch ((ReceiverGroup)raiseEventRequest.ReceiverGroup)
                {
                    case ReceiverGroup.All:
                        recipients = this.Actors;
                        updateEventCache = true;
                        break;

                    case ReceiverGroup.Others:
                        recipients = this.Actors.GetExcludedList(actor);
                        updateEventCache = true;
                        break;

                    case ReceiverGroup.MasterClient:
                        recipients = new[] { this.Actors[0] };
                        break;

                    default:
                        peer.SendOperationResponse(
                            new OperationResponse
                            {
                                OperationCode = raiseEventRequest.OperationRequest.OperationCode,
                                ReturnCode = -1,
                                DebugMessage = "Invalid ReceiverGroup " + raiseEventRequest.ReceiverGroup
                            },
                            sendParameters);
                        return;
                }
            }

            if (updateEventCache && raiseEventRequest.Cache != (byte)CacheOperation.DoNotCache)
            {
                string msg;
                if (!this.UpdateEventCache(actor, raiseEventRequest, out msg))
                {
                    peer.SendOperationResponse(
                        new OperationResponse
                        {
                            OperationCode = raiseEventRequest.OperationRequest.OperationCode,
                            ReturnCode = -1,
                            DebugMessage = msg
                        },
                        sendParameters);
                    return;
                }
            }

            this.PublishEvent(customEvent, recipients, sendParameters);
        }

        /// <summary>
        ///   Handles the <see cref = "SetPropertiesRequest" /> and sends event <see cref = "PropertiesChangedEvent" /> to all <see cref = "Actor" />s in the room.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "setPropertiesRequest">
        ///   The <see cref = "SetPropertiesRequest" /> operation to handle.
        /// </param>
        /// <param name = "sendParameters">
        ///   The send Parameters.
        /// </param>
        protected virtual void HandleSetPropertiesOperation(LitePeer peer, SetPropertiesRequest setPropertiesRequest, SendParameters sendParameters)
        {
            // check if peer has joined this room instance
            var sender = this.GetActorByPeer(peer);
            if (sender == null)
            {
                var response = new OperationResponse
                    {
                        OperationCode = setPropertiesRequest.OperationRequest.OperationCode,
                        ReturnCode = -1,
                        DebugMessage = "Room not joined"
                    };

                peer.SendOperationResponse(response, sendParameters);
                return;
            }

            if (setPropertiesRequest.ActorNumber > 0)
            {
                Actor actor = this.Actors.GetActorByNumber(setPropertiesRequest.ActorNumber);
                if (actor == null)
                {
                    peer.SendOperationResponse(
                        new OperationResponse
                            {
                                OperationCode = setPropertiesRequest.OperationRequest.OperationCode, 
                                ReturnCode = -1, 
                                DebugMessage = string.Format("Actor with number {0} not found.", setPropertiesRequest.ActorNumber)
                            }, 
                        sendParameters);
                    return;
                }

                actor.Properties.SetProperties(setPropertiesRequest.Properties);
            }
            else
            {
                this.Properties.SetProperties(setPropertiesRequest.Properties);
            }

            peer.SendOperationResponse(new OperationResponse { OperationCode = setPropertiesRequest.OperationRequest.OperationCode }, sendParameters);

            // if the optional paramter Broadcast is set a EvPropertiesChanged
            // event will be send to room actors
            if (setPropertiesRequest.Broadcast)
            {
                Actor actor = this.Actors.GetActorByPeer(peer);
                IEnumerable<Actor> recipients = this.Actors.GetExcludedList(actor);
                var propertiesChangedEvent = new PropertiesChangedEvent(actor.ActorNr)
                    {
                       TargetActorNumber = setPropertiesRequest.ActorNumber, Properties = setPropertiesRequest.Properties 
                    };

                this.PublishEvent(propertiesChangedEvent, recipients, sendParameters);
            }
        }

        protected virtual void HandleChangeGroupsOperation(LitePeer peer, ChangeGroups changeGroupsRequest, SendParameters sendParameters)
        {
            // get the actor who send the operation request
            Actor actor = this.GetActorByPeer(peer);
            if (actor == null)
            {
                return;
            }

            actor.RemoveGroups(changeGroupsRequest.Remove);

            if (changeGroupsRequest.Add != null)
            {
                if (changeGroupsRequest.Add.Length > 0)
                {
                    foreach (var groupId in changeGroupsRequest.Add)
                    {
                        ActorGroup group;
                        if (!this.actorGroups.TryGetValue(groupId, out group))
                        {
                            group = new ActorGroup(groupId);
                            this.actorGroups.Add(groupId, group);
                        }
                        actor.AddGroup(group);
                    }
                }
                else
                {
                    foreach (var group in this.actorGroups.Values)
                    {
                        actor.AddGroup(group);
                    }
                }
            }
        }

        /// <summary>
        ///   Processes a game message. Messages are used for internal communication.
        ///   Per default only <see cref = "GameMessageCodes.RemovePeerFromGame">message RemovePeerFromGame</see> is handled, 
        ///   a message that is sent when a player leaves a game due to disconnect or due to a subsequent join to a different game.
        /// </summary>
        /// <param name = "message">
        ///   Message to process.
        /// </param>
        protected override void ProcessMessage(IMessage message)
        {
            try
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("ProcessMessage {0}", message.Action);
                }

                switch ((GameMessageCodes)message.Action)
                {
                    case GameMessageCodes.RemovePeerFromGame:
                        var peer = (LitePeer)message.Message;
                        this.RemovePeerFromGame(peer, null);
                        if (this.LogQueue.Log.IsDebugEnabled)
                        {

                            this.LogQueue.Add(
                                new LogEntry(
                                    "ProcessMessage: " + (GameMessageCodes)message.Action, "Peer=" + peer.ConnectionId));
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///   Sends all cached events to a peer.
        /// </summary>
        /// <param name = "litePeer">
        ///   The lite peer that receives the events.
        /// </param>
        protected void PublishEventCache(LitePeer litePeer)
        {
            var @event = new CustomEvent(0, 0, null);
            foreach (KeyValuePair<int, EventCache> entry in this.actorEventCache)
            {
                int actor = entry.Key;
                EventCache cache = entry.Value;
                @event.ActorNr = actor;
                foreach (KeyValuePair<byte, Hashtable> eventEntry in cache)
                {
                    @event.Code = @eventEntry.Key;
                    @event.Data = @eventEntry.Value;

                    var eventData = new EventData(@event.Code, @event);
                    litePeer.SendEvent(eventData, new SendParameters());
                }
            }

            foreach (CustomEvent customEvent in this.eventCache)
            {
                var eventData = new EventData(customEvent.Code, customEvent);
                litePeer.SendEvent(eventData, new SendParameters());
            }
        }

        /// <summary>
        ///   Sends a <see cref = "JoinEvent" /> to all <see cref = "Actor" />s.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        /// <param name = "joinRequest">
        ///   The join request.
        /// </param>
        protected virtual void PublishJoinEvent(LitePeer peer, JoinRequest joinRequest)
        {
            if (this.SuppressRoomEvents)
            {
                return;
            }

            Actor actor = this.GetActorByPeer(peer);
            if (actor == null)
            {
                return;
            }

            // generate a join event and publish to all actors in the room
            var joinEvent = new JoinEvent(actor.ActorNr, this.Actors.GetActorNumbers().ToArray());

            if (joinRequest.BroadcastActorProperties)
            {
                joinEvent.ActorProperties = joinRequest.ActorProperties;
            }

            this.PublishEvent(joinEvent, this.Actors, new SendParameters());
        }

        /// <summary>
        ///   Sends a <see cref = "LeaveEvent" /> to all <see cref = "Actor" />s.
        /// </summary>
        /// <param name = "actor">
        ///   The actor which sents the event.
        /// </param>
        /// <param name = "leaveRequest">
        /// The <see cref="LeaveRequest"/> sent by the peer or null if the peer have been disconnected without sending a leave request.
        /// </param>
        protected virtual void PublishLeaveEvent(Actor actor, LeaveRequest leaveRequest)
        {
            if (this.SuppressRoomEvents)
            {
                return;
            }

            if (this.Actors.Count > 0 && actor != null)
            {
                IEnumerable<int> actorNumbers = this.Actors.GetActorNumbers();
                var leaveEvent = new LeaveEvent(actor.ActorNr, actorNumbers.ToArray());
                this.PublishEvent(leaveEvent, this.Actors, new SendParameters());
            }
        }

        /// <summary>
        ///   Removes a peer from the game. 
        ///   This method is called if a client sends a <see cref = "LeaveRequest" /> or disconnects.
        /// </summary>
        /// <param name = "peer">
        ///   The <see cref = "LitePeer" /> to remove.
        /// </param>
        /// <param name="leaveRequest">
        /// The <see cref="LeaveRequest"/> sent by the peer or null if the peer have been disconnected without sending a leave request.
        /// </param>
        /// <returns>
        ///   The actor number of the removed actor. 
        ///   If the specified peer does not exists -1 will be returned.
        /// </returns>
        protected virtual int RemovePeerFromGame(LitePeer peer, LeaveRequest leaveRequest)
        {
            Actor actor = this.Actors.RemoveActorByPeer(peer);
            if (actor == null)
            {
                if (Log.IsWarnEnabled)
                {
                    Log.WarnFormat("RemovePeerFromGame - Actor to remove not found for peer: {0}", peer.ConnectionId);
                }

                return -1;
            }

            this.actorEventCache.RemoveEventCache(actor.ActorNr);

            if (this.DeleteCacheOnLeave)
            {
                this.eventCache.RemoveEventsByActor(actor.ActorNr);
            }

            // raise leave event
            this.PublishLeaveEvent(actor, leaveRequest);

            return actor.ActorNr;
        }

        /// <summary>
        ///   Tries to add a <see cref = "LitePeer" /> to this game instance.
        /// </summary>
        /// <param name = "peer">
        ///   The peer to add.
        /// </param>
        /// <param name = "actor">
        ///   When this method returns this out param contains the <see cref = "Actor" /> associated with the <paramref name = "peer" />.
        /// </param>
        /// <returns>
        ///   Returns true if no actor exists for the specified peer and a new actor for the peer has been successfully added. 
        ///   The actor parameter is set to the newly created <see cref = "Actor" /> instance.
        ///   Returns false if an actor for the specified peer already exists. 
        ///   The actor paramter is set to the existing <see cref = "Actor" /> for the specified peer.
        /// </returns>
        protected virtual bool TryAddPeerToGame(LitePeer peer, int actorNr, out Actor actor)
        {
            // check if the peer already exists in this game
            actor = this.Actors.GetActorByPeer(peer);
            if (actor != null)
            {
                return false;
            }

            if (actorNr != 0)
            {
                actor = this.Actors.GetActorByNumber(actorNr);
                if (actor != null)
                {
                    return false;
                }
            }

            // create new actor instance 
            actor = new Actor(peer);
            if (actorNr != 0)
            {
                actor.ActorNr = actorNr;
            }
            else
            {
                this.actorNumberCounter++;
                actor.ActorNr = this.actorNumberCounter;
            }
            this.Actors.Add(actor);

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Actor added: {0} to game: {1}", actor.ActorNr, this.Name);
            }

            return true;
        }

        /// <summary>
        ///   Helper method of <see cref = "HandleRaiseEventOperation" />.
        ///   Stores an event for new actors.
        /// </summary>
        /// <param name = "actor">
        ///   The actor.
        /// </param>
        /// <param name = "raiseEventRequest">
        ///   The raise event request.
        /// </param>
        /// <param name="msg">
        ///   Contains an error message if the method returns false.
        /// </param>
        /// <returns>
        ///   True if <see cref = "RaiseEventRequest.Cache" /> is valid.
        /// </returns>
        protected bool UpdateEventCache(Actor actor, RaiseEventRequest raiseEventRequest, out string msg)
        {
            msg = null;
            CustomEvent customEvent;

            switch (raiseEventRequest.Cache)
            {
                case (byte)CacheOperation.DoNotCache:
                    return true;

                case (byte)CacheOperation.AddToRoomCache:
                    customEvent = new CustomEvent(actor.ActorNr, raiseEventRequest.EvCode, raiseEventRequest.Data);
                    this.eventCache.AddEvent(customEvent);
                    return true;

                case (byte)CacheOperation.AddToRoomCacheGlobal:
                    customEvent = new CustomEvent(0, raiseEventRequest.EvCode, raiseEventRequest.Data);
                    this.eventCache.AddEvent(customEvent);
                    return true;
            }

            // cache operations for the actor event cache currently only working with hashtable data
            Hashtable eventData;
            if (raiseEventRequest.Data == null || raiseEventRequest.Data is Hashtable)
            {
                eventData = (Hashtable)raiseEventRequest.Data;
            }
            else
            {
                msg = string.Format("Cache operation '{0}' requires a Hashtable as event data.", raiseEventRequest.Cache);
                return false;
            }


            switch (raiseEventRequest.Cache)
            {
                case (byte)CacheOperation.MergeCache:
                    this.actorEventCache.MergeEvent(actor.ActorNr, raiseEventRequest.EvCode, eventData);
                    return true;

                case (byte)CacheOperation.RemoveCache:
                    this.actorEventCache.RemoveEvent(actor.ActorNr, raiseEventRequest.EvCode);
                    return true;

                case (byte)CacheOperation.ReplaceCache:
                    this.actorEventCache.ReplaceEvent(actor.ActorNr, raiseEventRequest.EvCode, eventData);
                    return true;

                default:
                msg = string.Format("Unknown cache operation '{0}'.", raiseEventRequest.Cache);
                    return false;
            }
        }

        #endregion
    }
}