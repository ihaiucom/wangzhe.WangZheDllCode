// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MmoActorOperationHandler.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Сlient's Peer.CurrentOperationHandler after entering a world.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server
{
    using System;
    using System.Collections;

    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server.Events;
    using Photon.MmoDemo.Server.Operations;
    using Photon.SocketServer;
    using Photon.MmoDemo.Server;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// Сlient's Peer.CurrentOperationHandler after entering a world.
    /// </summary>
    public sealed class MmoActorOperationHandler : MmoActor, IOperationHandler
    {
        public MmoActorOperationHandler(PeerBase peer, World world, InterestArea interestArea)
            : base(peer, world)
        {
            this.AddInterestArea(interestArea);
        }

        /// <summary>
        /// Handles operations CreateWorld and EnterWorld.
        /// </summary>
        public static OperationResponse InvalidOperation(OperationRequest request)
        {
            string debugMessage = "InvalidOperation: " + (OperationCode)request.OperationCode;
            return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperation, DebugMessage = debugMessage };
        }

        /// <summary>
        /// Handles operation AddInterestArea: Creates a new InterestArea and optionally attaches it to an existing Item.
        /// </summary>
        public OperationResponse OperationAddInterestArea(PeerBase peer, OperationRequest request, SendParameters sendParameters)
        {
            var operation = new AddInterestArea(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            InterestArea interestArea;
            if (this.TryGetInterestArea(operation.InterestAreaId, out interestArea))
            {
                return operation.GetOperationResponse((int)ReturnCode.InterestAreaAlreadyExists, "InterestAreaAlreadyExists");
            }

            interestArea = new ClientInterestArea(this.Peer, operation.InterestAreaId, this.World);
            this.AddInterestArea(interestArea);

            // attach interestArea to item            
            if (string.IsNullOrEmpty(operation.ItemId) == false)
            {
                Item item;

                bool actorItem = this.TryGetItem(operation.ItemId, out item);
                if (actorItem)
                {
                    // we are already in the item thread, invoke directly
                    return ItemOperationAddInterestArea(item, operation, interestArea);
                }
                else
                {
                    if (this.World.ItemCache.TryGetItem(operation.ItemId, out item) == false)
                    {
                        return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
                    }
                    else
                    {
                        // second parameter (peer) allows us to send an error event to the client (in case of an error)
                        item.Fiber.Enqueue(() => this.ExecItemOperation(() => ItemOperationAddInterestArea(item, operation, interestArea), sendParameters));
                        // send response later
                        return null;
                    }
                }
            }
            else
            {
                // free floating interestArea
                lock (interestArea.SyncRoot)
                {
                    interestArea.Position = operation.Position;
                    interestArea.ViewDistanceEnter = operation.ViewDistanceEnter;
                    interestArea.ViewDistanceExit = operation.ViewDistanceExit;
                    interestArea.UpdateInterestManagement();
                }

                return operation.GetOperationResponse(MethodReturnValue.Ok);
            }
        }

        /// <summary>
        /// Handles operation AttachInterestArea: Attaches an existing InterestArea to an existing Item.
        /// </summary>
        public OperationResponse OperationAttachInterestArea(PeerBase peer, OperationRequest request, SendParameters sendParameters)
        {
            var operation = new AttachInterestArea(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            InterestArea interestArea;
            if (this.TryGetInterestArea(operation.InterestAreaId, out interestArea) == false)
            {
                return operation.GetOperationResponse((int)ReturnCode.InterestAreaNotFound, "InterestAreaNotFound");
            }

            Item item;
            bool actorItem;
            if (string.IsNullOrEmpty(operation.ItemId))
            {
                item = this.Avatar;
                actorItem = true;

                // set return vaues
                operation.ItemId = item.Id;
            }
            else
            {
                actorItem = this.TryGetItem(operation.ItemId, out item);
                
            }

            if (actorItem)
            {
                // we are already in the item thread, invoke directly
                return this.ItemOperationAttachInterestArea(item, operation, interestArea, sendParameters);
            }
            else
            {
                // search world cache just to see if item exists at all
                if (this.World.ItemCache.TryGetItem(operation.ItemId, out item) == false)
                {
                    return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
                }
                else
                {
                    // second parameter (peer) allows us to send an error event to the client (in case of an error)
                    item.Fiber.Enqueue(() => this.ExecItemOperation(() => this.ItemOperationAttachInterestArea(item, operation, interestArea, sendParameters), sendParameters));

                    // response is sent later
                    return null;
                }
            }
        }

        /// <summary>
        /// Handles operation DestroyItem: Destroys an existing Item. 
        /// </summary>
        public OperationResponse OperationDestroyItem(PeerBase peer, OperationRequest request, SendParameters sendParameters)
        {
            var operation = new DestroyItem(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            Item item;
            bool actorItem = this.TryGetItem(operation.ItemId, out item);
            
            if (actorItem)
            {
                // we are already in the item thread, invoke directly
                return this.ItemOperationDestroy(item, operation);
            }
            else 
            {
                // search world cache just to see if item exists at all
                if (this.World.ItemCache.TryGetItem(operation.ItemId, out item) == false)
                {
                    return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
                }
                else
                {
                    // second parameter (peer) allows us to send an error event to the client (in case of an error)
                    // error ItemAccessDenied or ItemNotFound will be returned
                    item.Fiber.Enqueue(() => this.ExecItemOperation(() => this.ItemOperationDestroy(item, operation), sendParameters));

                    // operation is continued later
                    return null;
                }
            }
        }

        /// <summary>
        /// Handles operation DetachInterestArea: Detaches an existing InterestArea from an Item.
        /// </summary>
        public OperationResponse OperationDetachInterestArea(PeerBase peer, OperationRequest request)
        {
            var operation = new DetachInterestArea(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            InterestArea interestArea;
            if (this.TryGetInterestArea(operation.InterestAreaId, out interestArea) == false)
            {
                return operation.GetOperationResponse((int)ReturnCode.InterestAreaNotFound, "InterestAreaNotFound");
            }

            lock (interestArea.SyncRoot)
            {
                interestArea.Detach();
            }

            return operation.GetOperationResponse(MethodReturnValue.Ok);
        }

        /// <summary>
        /// Handles operation ExitWorld: Sends event WorldExited to the client, disposes the actor and replaces the peer's Peer.CurrentOperationHandler with the MmoPeer itself.
        /// </summary>
        public OperationResponse OperationExitWorld(PeerBase peer, OperationRequest request)
        {
            var operation = new Operation();
            operation.OnStart();

            this.ExitWorld();

            // don't send response
            operation.OnComplete();
            return null;
        }

        /// <summary>
        /// Handles operation GetProperties: Sends event ItemProperties to the client.
        /// </summary>
        public OperationResponse OperationGetProperties(PeerBase peer, OperationRequest request, SendParameters sendParameters)
        {
            var operation = new GetProperties(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            Item item;
            bool actorItem = this.TryGetItem(operation.ItemId, out item);
            if (actorItem == false)
            {
                if (this.World.ItemCache.TryGetItem(operation.ItemId, out item) == false)
                {
                    return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
                }
            }

            if (actorItem)
            {
                // we are already in the item thread, invoke directly
                return this.ItemOperationGetProperties(item, operation);
            }
            else
            {
                // second parameter (peer) allows us to send an error event to the client (in case of an error)
                item.Fiber.Enqueue(() => this.ExecItemOperation(() => this.ItemOperationGetProperties(item, operation), sendParameters));

                // operation is continued later
                return null;
            }
        }

        /// <summary>
        /// Handles operation Move: Move the items and ultimately sends event ItemMoved to other clients.
        /// </summary>
        public OperationResponse OperationMove(PeerBase peer, OperationRequest request, SendParameters sendParameters)
        {
            var operation = new Move(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            Item item;
            if (string.IsNullOrEmpty(operation.ItemId))
            {
                item = this.Avatar;

                // set return values
                operation.ItemId = item.Id;
            }
            else if (this.TryGetItem(operation.ItemId, out item) == false)
            {
                return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            return this.ItemOperationMove((Item)item, operation, sendParameters);
        }

        /// <summary>
        /// Handles operation MoveInterestArea: Moves one of the actor's InterestArea.
        /// </summary>
        public OperationResponse OperationMoveInterestArea(PeerBase peer, OperationRequest request)
        {
            var operation = new MoveInterestArea(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            InterestArea interestArea;
            if (this.TryGetInterestArea(operation.InterestAreaId, out interestArea))
            {
                lock (interestArea.SyncRoot)
                {
                    interestArea.Position = operation.Position;
                    interestArea.UpdateInterestManagement();
                }

                // don't send response
                return null;
            }

            return operation.GetOperationResponse((int)ReturnCode.InterestAreaNotFound, "InterestAreaNotFound");
        }

        /// <summary>
        /// Handles operation RemoveInterestArea: Removes one of the actor's InterestAreas.
        /// </summary>
        public OperationResponse OperationRemoveInterestArea(PeerBase peer, OperationRequest request)
        {
            var operation = new RemoveInterestArea(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            InterestArea interestArea;
            if (this.TryGetInterestArea(operation.InterestAreaId, out interestArea))
            {
                lock (interestArea.SyncRoot)
                {
                    interestArea.Detach();
                    interestArea.Dispose();
                }

                this.RemoveInterestArea(operation.InterestAreaId);
                return operation.GetOperationResponse(MethodReturnValue.Ok);
            }

            return operation.GetOperationResponse((int)ReturnCode.InterestAreaNotFound, "InterestAreaNotFound");
        }

        /// <summary>
        /// Handles operation SetProperties: Sets the Item.Properties of an Item and ultimately sends event ItemPropertiesSet to other clients.
        /// </summary>
        public OperationResponse OperationSetProperties(PeerBase peer, OperationRequest request, SendParameters sendParameters)
        {
            var operation = new SetProperties(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            Item item;
            if (string.IsNullOrEmpty(operation.ItemId))
            {
                item = this.Avatar;

                // set return values
                operation.ItemId = item.Id;
            }
            else if (this.TryGetItem(operation.ItemId, out item) == false)
            {
                return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            return this.ItemOperationSetProperties(item, operation, sendParameters);
        }

        /// <summary>
        /// Handles operation SetViewDistance: Changes the subscribe and unsubscribe radius for an InterestArea.
        /// </summary>
        public OperationResponse OperationSetViewDistance(PeerBase peer, OperationRequest request)
        {
            var operation = new SetViewDistance(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            InterestArea interestArea;
            if (this.TryGetInterestArea(operation.InterestAreaId, out interestArea) == false)
            {
                return operation.GetOperationResponse((int)ReturnCode.InterestAreaNotFound, "InterestAreaNotFound");
            }

            lock (interestArea.SyncRoot)
            {
                interestArea.ViewDistanceEnter = operation.ViewDistanceEnter;
                interestArea.ViewDistanceExit = operation.ViewDistanceExit;
                interestArea.UpdateInterestManagement();
            }

            // don't send response
            return null;
        }

        /// <summary>
        /// Handles operation SpawnItem: Creates a new Item and optionally subscribes an InterestArea to it.
        /// </summary>
        public OperationResponse OperationSpawnItem(PeerBase peer, OperationRequest request)
        {
            var operation = new SpawnItem(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            var item = new Item(operation.Position, operation.Rotation, operation.Properties, this, operation.ItemId, operation.ItemType, this.World);
            if (this.World.ItemCache.AddItem(item))
            {
                this.AddItem(item);
                return this.ItemOperationSpawn(item, operation);
            }

            item.Dispose();
            return operation.GetOperationResponse((int)ReturnCode.ItemAlreadyExists, "ItemAlreadyExists");
        }

        /// <summary>
        /// Handles operation SubscribeItem: Manually subscribes item (does not affect interest area updates).
        /// The client receives event ItemSubscribed on success.
        /// </summary>
        /// <remarks>        
        /// If the submitted SubscribeItem.PropertiesRevision is null or smaller than the item Item.PropertiesRevision event ItemProperties is sent to the client.        
        /// </remarks>
        public OperationResponse OperationSubscribeItem(PeerBase peer, OperationRequest request, SendParameters sendParameters)
        {
            var operation = new SubscribeItem(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();

            Item item;
            bool actorItem = this.TryGetItem(operation.ItemId, out item);
            if (actorItem == false)
            {
                if (this.World.ItemCache.TryGetItem(operation.ItemId, out item) == false)
                {
                    return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
                }
            }

            if (actorItem)
            {
                // we are already in the item thread, invoke directly
                return this.ItemOperationSubscribeItem(item, operation);
            }
            else
            {
                // second parameter (peer) allows us to send an error event to the client (in case of an error)
                item.Fiber.Enqueue(() => this.ExecItemOperation(() => this.ItemOperationSubscribeItem(item, operation), sendParameters));

                // operation continues later
                return null;
            }
        }

        /// <summary>
        /// Handles operation UnsubscribeItem: manually unsubscribes an existing InterestArea from an existing Item.
        /// The client receives event ItemUnsubscribed on success.
        /// </summary>
        public OperationResponse OperationUnsubscribeItem(PeerBase peer, OperationRequest request, SendParameters sendParameters)
        {
            var operation = new UnsubscribeItem(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();

            Item item;
            bool actorItem = this.TryGetItem(operation.ItemId, out item);
            if (actorItem == false)
            {
                if (this.World.ItemCache.TryGetItem(operation.ItemId, out item) == false)
                {
                    return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
                }
            }

            this.interestItems.UnsubscribeItem(item);
            
            if (actorItem)
            {
                // we are already in the item thread, invoke directly
                return this.ItemOperationUnsubscribeItem(item, operation);
            }
            else
            {
                // second parameter (peer) allows us to send an error event to the client (in case of an error)
                item.Fiber.Enqueue(() => this.ExecItemOperation(() => this.ItemOperationUnsubscribeItem(item, operation), sendParameters));

                // operation continues later
                return null;
            }
        }

        /// <summary>
        ///   Handles operation RaiseGenericEvent. Sends event ItemGeneric to an Item owner or the subscribers of an Item />.
        /// </summary>
        public OperationResponse OperationRaiseGenericEvent(PeerBase peer, OperationRequest request, SendParameters sendParameters)
        {
            var operation = new RaiseGenericEvent(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            operation.OnStart();
            Item item;
            bool actorItem = true;
            if (this.TryGetItem(operation.ItemId, out item) == false)
            {
                if (this.World.ItemCache.TryGetItem(operation.ItemId, out item) == false)
                {
                    return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
                }

                actorItem = false;
            }

            if (actorItem)
            {
                // we are already in the item thread, invoke directly
                return ItemOperationRaiseGenericEvent(item, operation, sendParameters);
            }

            // second parameter (peer) allows us to send an error event to the client (in case of an error)
            item.Fiber.Enqueue(() => this.ExecItemOperation(() => ItemOperationRaiseGenericEvent(item, operation, sendParameters), sendParameters));

            // operation continued later
            return null;
        }


        /// <summary>
        /// Disposes the actor, stops any further operation handling and disposes the Peer.
        /// </summary>
        public void OnDisconnect(PeerBase peer)
        {
            this.Dispose();
            ((Peer)peer).SetCurrentOperationHandler(null);
            peer.Dispose();
        }

        /// <summary>
        /// Kicks the actor from the world (event WorldExited is sent to the client) and then disconnects the client.
        /// </summary>
        /// <remarks>
        /// Called by DisconnectByOtherPeer after being enqueued to the PeerBase.RequestFiber.
        /// It kicks the actor from the world (event WorldExited) and then continues the original request by calling the original peer's OnOperationRequest method.        
        /// </remarks>
        public void OnDisconnectByOtherPeer(PeerBase otherPeer, OperationRequest otherRequest, SendParameters sendParameters)
        {
            this.ExitWorld();

            // disconnect peer after the exit world event is sent
            this.Peer.RequestFiber.Enqueue(() => this.Peer.RequestFiber.Enqueue(this.Peer.Disconnect));

            // continue execution of other request
            PeerHelper.InvokeOnOperationRequest(otherPeer, otherRequest, sendParameters);
        }

        /// <summary>
        /// Enqueues OnDisconnectByOtherPeer to the PeerBase.RequestFiber.
        /// </summary>
        /// <remarks>
        /// This method is intended to be used to disconnect a user's peer if he connects with multiple clients while the application logic wants to allow just one.
        /// </remarks>
        public void DisconnectByOtherPeer(PeerBase otherPeer, OperationRequest otherRequest, SendParameters sendParameters)
        {
            this.Peer.RequestFiber.Enqueue(() => this.OnDisconnectByOtherPeer(otherPeer, otherRequest, sendParameters));
        }

        public OperationResponse OnOperationRequest(PeerBase peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch ((OperationCode)operationRequest.OperationCode)
            {
                case OperationCode.AddInterestArea:
                    return this.OperationAddInterestArea(peer, operationRequest, sendParameters);

                case OperationCode.AttachInterestArea:
                    return this.OperationAttachInterestArea(peer, operationRequest, sendParameters);

                case OperationCode.DestroyItem:
                    return this.OperationDestroyItem(peer, operationRequest, sendParameters);

                case OperationCode.DetachInterestArea:
                    return this.OperationDetachInterestArea(peer, operationRequest);

                case OperationCode.ExitWorld:
                    return this.OperationExitWorld(peer, operationRequest);

                case OperationCode.GetProperties:
                    return this.OperationGetProperties(peer, operationRequest, sendParameters);

                case OperationCode.Move:
                    return this.OperationMove(peer, operationRequest, sendParameters);

                case OperationCode.MoveInterestArea:
                    return this.OperationMoveInterestArea(peer, operationRequest);

                case OperationCode.RemoveInterestArea:
                    return this.OperationRemoveInterestArea(peer, operationRequest);

                case OperationCode.SetProperties:
                    return this.OperationSetProperties(peer, operationRequest, sendParameters);

                case OperationCode.SetViewDistance:
                    return this.OperationSetViewDistance(peer, operationRequest);

                case OperationCode.SpawnItem:
                    return this.OperationSpawnItem(peer, operationRequest);

                case OperationCode.SubscribeItem:
                    return this.OperationSubscribeItem(peer, operationRequest, sendParameters);

                case OperationCode.UnsubscribeItem:
                    return this.OperationUnsubscribeItem(peer, operationRequest, sendParameters);

                case OperationCode.RadarSubscribe:
                    return MmoInitialOperationHandler.OperationRadarSubscribe(peer, operationRequest, sendParameters);

                case OperationCode.SubscribeCounter:
                    return CounterOperations.SubscribeCounter(peer, operationRequest);

                case OperationCode.UnsubscribeCounter:
                    return CounterOperations.SubscribeCounter(peer, operationRequest);

                case OperationCode.RaiseGenericEvent:
                    return this.OperationRaiseGenericEvent(peer, operationRequest, sendParameters);

                case OperationCode.CreateWorld:
                case OperationCode.EnterWorld:
                    return InvalidOperation(operationRequest);
            }

            return new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (int)ReturnCode.OperationNotSupported,
                    DebugMessage = "OperationNotSupported: " + operationRequest.OperationCode
                };
        }

        private static OperationResponse ItemOperationAddInterestArea(Item item, AddInterestArea operation, InterestArea interestArea)
        {
            if (item.Disposed)
            {
                return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            lock (interestArea.SyncRoot)
            {
                interestArea.AttachToItem(item);
                interestArea.ViewDistanceEnter = operation.ViewDistanceEnter;
                interestArea.ViewDistanceExit = operation.ViewDistanceExit;
                interestArea.UpdateInterestManagement();
            }

            operation.OnComplete();
            return operation.GetOperationResponse(MethodReturnValue.Ok);
        }

        private MethodReturnValue CheckAccess(Item item)
        {
            if (item.Disposed)
            {
                return MethodReturnValue.New((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            if (((Item)item).GrantWriteAccess(this))
            {
                return MethodReturnValue.Ok;
            }

            return MethodReturnValue.New((int)ReturnCode.ItemAccessDenied, "ItemAccessDenied");
        }

        /// <summary>
        /// Executes an item operation and returns an error response in case an exception occurs.
        /// </summary>
        private void ExecItemOperation(Func<OperationResponse> operation, SendParameters sendParameters)
        {
            OperationResponse result = operation();
            if (result != null)
            {
                this.Peer.SendOperationResponse(result, sendParameters);
            }
        }

        private void ExitWorld()
        {
            var worldExited = new WorldExited { WorldName = ((World)this.World).Name };
            this.Dispose();

            // set initial handler
            ((MmoPeer)this.Peer).SetInitialOperationhandler();            

            var eventData = new EventData((byte)EventCode.WorldExited, worldExited);

            // use item channel to ensure that this event arrives in correct order with move/subscribe events
            this.Peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });
        }

        private OperationResponse ItemOperationAttachInterestArea(
            Item item, AttachInterestArea operation, InterestArea interestArea, SendParameters sendParameters)
        {
            if (item.Disposed)
            {
                return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            lock (interestArea.SyncRoot)
            {
                interestArea.Detach();
                interestArea.AttachToItem(item);
                interestArea.UpdateInterestManagement();
            }

            // use item channel to ensure that this event arrives before any move or subscribe events
            OperationResponse response = operation.GetOperationResponse(MethodReturnValue.Ok);
            sendParameters.ChannelId = Settings.ItemEventChannel;
            this.Peer.SendOperationResponse(response, sendParameters);

            operation.OnComplete();
            return null;
        }

        private OperationResponse ItemOperationDestroy(Item item, DestroyItem operation)
        {
            MethodReturnValue result = this.CheckAccess(item);
            if (result.IsOk)
            {
                item.Destroy();
                item.Dispose();
                this.RemoveItem(item);

                item.World.ItemCache.RemoveItem(item.Id);
                var eventInstance = new ItemDestroyed { ItemId = item.Id };
                var eventData = new EventData((byte)EventCode.ItemDestroyed, eventInstance);
                this.Peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });

                // no response, event is sufficient
                operation.OnComplete();
                return null;
            }

            return operation.GetOperationResponse(result);
        }

        private OperationResponse ItemOperationGetProperties(Item item, GetProperties operation)
        {
            if (item.Disposed)
            {
                return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            if (item.Properties != null)
            {
                if (operation.PropertiesRevision.HasValue == false || operation.PropertiesRevision.Value != item.PropertiesRevision)
                {
                    var properties = new ItemProperties
                        {
                            ItemId = item.Id,
                            PropertiesRevision = item.PropertiesRevision,
                            PropertiesSet = new Hashtable(item.Properties)
                        };

                    var eventData = new EventData((byte)EventCode.ItemProperties, properties);
                    this.Peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });
                }
            }

            // no response sent
            operation.OnComplete();
            return null;
        }

        private OperationResponse ItemOperationMove(Item item, Move operation, SendParameters sendParameters)
        {
            // should always be OK
            MethodReturnValue result = this.CheckAccess(item);
            if (result.IsOk)
            {
                // save previous for event
                Vector oldPosition = item.Position;
                Vector oldRotation = item.Rotation;

                // move
                item.Rotation = operation.Rotation;
                item.Move(operation.Position);

                // send event
                var eventInstance = new ItemMoved
                    {
                        ItemId = item.Id,
                        OldPosition = oldPosition,
                        Position = operation.Position,
                        Rotation = operation.Rotation,
                        OldRotation = oldRotation
                    };

                var eventData = new EventData((byte)EventCode.ItemMoved, eventInstance);
                sendParameters.ChannelId = Settings.ItemEventChannel;
                var message = new ItemEventMessage(item, eventData, sendParameters);
                item.EventChannel.Publish(message);

                // no response sent
                operation.OnComplete();
                return null;
            }

            return operation.GetOperationResponse(result);
        }

        private OperationResponse ItemOperationSetProperties(Item item, SetProperties operation, SendParameters sendParameters)
        {
            MethodReturnValue result = this.CheckAccess(item);
            if (result.IsOk)
            {
                item.SetProperties(operation.PropertiesSet, operation.PropertiesUnset);
                var eventInstance = new ItemPropertiesSet
                    {
                        ItemId = item.Id,
                        PropertiesRevision = item.PropertiesRevision,
                        PropertiesSet = operation.PropertiesSet,
                        PropertiesUnset = operation.PropertiesUnset
                    };

                var eventData = new EventData((byte)EventCode.ItemPropertiesSet, eventInstance);
                sendParameters.ChannelId = Settings.ItemEventChannel;
                var message = new ItemEventMessage(item, eventData, sendParameters);
                item.EventChannel.Publish(message);

                // no response sent
                operation.OnComplete();
                return null;
            }

            return operation.GetOperationResponse(result);
        }

        private OperationResponse ItemOperationSpawn(Item item, SpawnItem operation)
        {
            // this should always return Ok
            MethodReturnValue result = this.CheckAccess(item);

            if (result.IsOk)
            {
                item.Rotation = operation.Rotation;
                item.Spawn(operation.Position);
                ((World)this.World).Radar.AddItem(item, operation.Position);
            }

            operation.OnComplete();
            return operation.GetOperationResponse(result);
        }

        private OperationResponse ItemOperationSubscribeItem(Item item, SubscribeItem operation)
        {
            if (item.Disposed)
            {
                return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            this.interestItems.SubscribeItem(item);

            var subscribeEvent = new ItemSubscribed
            {
                ItemId = item.Id,
                ItemType = item.Type,
                Position = item.Position,
                PropertiesRevision = item.PropertiesRevision,
                Rotation = item.Rotation
            };

            var eventData = new EventData((byte)EventCode.ItemSubscribed, subscribeEvent);
            this.Peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });

            if (operation.PropertiesRevision.HasValue == false || operation.PropertiesRevision.Value != item.PropertiesRevision)
            {
                var properties = new ItemPropertiesSet
                    {
                        ItemId = item.Id,
                        PropertiesRevision = item.PropertiesRevision,
                        PropertiesSet = new Hashtable(item.Properties)
                    };
                var propEventData = new EventData((byte)EventCode.ItemPropertiesSet, properties);
                this.Peer.SendEvent(propEventData, new SendParameters { ChannelId = Settings.ItemEventChannel });
            }

            // don't send response
            operation.OnComplete();
            return null;
        }

        private OperationResponse ItemOperationUnsubscribeItem(Item item, UnsubscribeItem operation)
        {
            if (item.Disposed)
            {
                return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            this.interestItems.UnsubscribeItem(item);

            var unsubscribeEvent = new ItemUnsubscribed { ItemId = item.Id };

            var eventData = new EventData((byte)EventCode.ItemUnsubscribed, unsubscribeEvent);
            this.Peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });

            // don't send response
            operation.OnComplete();
            return null;
        }

        private static OperationResponse ItemOperationRaiseGenericEvent(Item item, RaiseGenericEvent operation, SendParameters sendParameters)
        {
            if (item.Disposed)
            {
                return operation.GetOperationResponse((int)ReturnCode.ItemNotFound, "ItemNotFound");
            }

            var eventInstance = new ItemGeneric
            {
                ItemId = item.Id,
                CustomEventCode = operation.CustomEventCode,
                EventData = operation.EventData
            };

            var eventData = new EventData((byte)EventCode.ItemGeneric, eventInstance);
            sendParameters.Unreliable = (Reliability)operation.EventReliability == Reliability.Unreliable;
            sendParameters.ChannelId = Settings.ItemEventChannel;
            switch (operation.EventReceiver)
            {
                case (byte)EventReceiver.ItemOwner:
                    {
                        item.Owner.Peer.SendEvent(eventData, sendParameters);
                        break;
                    }

                case (byte)EventReceiver.ItemSubscriber:
                    {
                        var message = new ItemEventMessage(item, eventData, sendParameters);
                        item.EventChannel.Publish(message);
                        break;
                    }

                default:
                    {
                        return operation.GetOperationResponse((int)ReturnCode.ParameterOutOfRange, "Invalid EventReceiver " + operation.EventReceiver);
                    }
            }

            // no response
            operation.OnComplete();
            return null;
        }


    }
}