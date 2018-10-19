// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MmoInitialOperationHandler.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Сlient's Peer.CurrentOperationHandler immediately after connecting.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Server
{
    using System;

    using Photon.MmoDemo.Common;
    using Photon.MmoDemo.Server.Operations;
    using Photon.SocketServer;
    using Photon.SocketServer.Diagnostics;
    using Photon.MmoDemo.Server;
    using Photon.SocketServer.Rpc;

    using PhotonHostRuntimeInterfaces;

    /// <summary>
    /// Сlient's Peer.CurrentOperationHandler immediately after connecting.
    /// </summary>
    public class MmoInitialOperationHandler : IOperationHandler
    {
        private MmoPeer peer;
        public MmoInitialOperationHandler(MmoPeer peer)
        {
            this.peer = peer;
        }

        /// <summary>
        /// Handles all operations that are not allowed before operation EnterWorld is called.
        /// </summary>
        public static OperationResponse InvalidOperation(OperationRequest request)
        {
            return new OperationResponse(request.OperationCode)
                {
                    ReturnCode = (int)ReturnCode.InvalidOperation,
                    DebugMessage = "InvalidOperation: " + (OperationCode)request.OperationCode
                };
        }

        /// <summary>
        /// Expects operation RadarSubscribe and subscribes the peer to the Radar.
        /// Publishes an OperationResponse with error code ReturnCode.Ok if successful.
        /// </summary>
        public static OperationResponse OperationRadarSubscribe(PeerBase peer, OperationRequest request, SendParameters sendParameters)
        {
            var mmoPeer = (MmoPeer)peer;
            var operation = new RadarSubscribe(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            if (mmoPeer.RadarSubscription != null)
            {
                mmoPeer.RadarSubscription.Dispose();
                mmoPeer.RadarSubscription = null;
            }

            World world;
            if (WorldCache.Instance.TryGet(operation.WorldName, out world) == false)
            {
                return operation.GetOperationResponse((int)ReturnCode.WorldNotFound, "WorldNotFound");
            }

            mmoPeer.RadarSubscription = world.Radar.Channel.Subscribe(mmoPeer.RequestFiber, m => RadarChannel_OnItemEventMessage(peer, m));

            // set return values
            var responseObject = new RadarSubscribeResponse
                {
                    BoundingBox = world.Area,
                    TileDimensions = world.TileDimensions,
                    WorldName = world.Name
                };

            // send response before sending radar content
            var response = new OperationResponse(request.OperationCode, responseObject);
            peer.SendOperationResponse(response, sendParameters);

            // send complete radar content to client
            world.Radar.SendContentToPeer(mmoPeer);

            // response already sent
            return null;
        }

        /// <summary>
        /// Expects operation CreateWorld and adds a new World to the WorldCache.
        /// </summary>
        public OperationResponse OperationCreateWorld(PeerBase peer, OperationRequest request)
        {
            var operation = new CreateWorld(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            World world;
            MethodReturnValue result = WorldCache.Instance.TryCreate(
                operation.WorldName, operation.BoundingBox, operation.TileDimensions, out world)
                                           ? MethodReturnValue.Ok
                                           : MethodReturnValue.New((int)ReturnCode.WorldAlreadyExists, "WorldAlreadyExists");

            return operation.GetOperationResponse(result);
        }

        /// <summary>
        /// Expects operation EnterWorld and creates a new MmoActor with a new Item as avatar and a new MmoClientInterestArea. 
        /// </summary>
        /// <remarks>
        /// The MmoActor becomes the new Peer.CurrentOperationHandler.
        /// If another MmoActor with the same name exists he is disconnected.
        /// An OperationResponse with error code ReturnCode.Ok is published on success.
        /// </remarks>
        public OperationResponse OperationEnterWorld(PeerBase peer, OperationRequest request, SendParameters sendParameters)
        {
            var operation = new EnterWorld(peer.Protocol, request);
            if (!operation.IsValid)
            {
                return new OperationResponse(request.OperationCode) { ReturnCode = (int)ReturnCode.InvalidOperationParameter, DebugMessage = operation.GetErrorMessage() };
            }

            World world;
            if (WorldCache.Instance.TryGet(operation.WorldName, out world) == false)
            {
                return operation.GetOperationResponse((int)ReturnCode.WorldNotFound, "WorldNotFound");
            }

            var interestArea = new ClientInterestArea(peer, operation.InterestAreaId, world)
                {
                    ViewDistanceEnter = operation.ViewDistanceEnter,
                    ViewDistanceExit = operation.ViewDistanceExit
                };

            var actor = new MmoActorOperationHandler(peer, world, interestArea);
            var avatar = new Item(operation.Position, operation.Rotation, operation.Properties, actor, operation.Username, (byte)ItemType.Avatar, world);

            while (world.ItemCache.AddItem(avatar) == false)
            {
                Item otherAvatarItem;
                if (world.ItemCache.TryGetItem(avatar.Id, out otherAvatarItem))
                {
                    avatar.Dispose();
                    actor.Dispose();
                    interestArea.Dispose();

                    (((Item)otherAvatarItem).Owner).DisconnectByOtherPeer(this.peer, request, sendParameters);

                    // request continued later, no response here
                    return null;
                }
            }

            // init avatar
            actor.AddItem(avatar);
            actor.Avatar = avatar;

            ((Peer)peer).SetCurrentOperationHandler(actor);

            // set return values
            var responseObject = new EnterWorldResponse
                {
                    BoundingBox = world.Area,
                    TileDimensions = world.TileDimensions,
                    WorldName = world.Name
                };

            // send response; use item channel to ensure that this event arrives before any move or subscribe events
            var response = new OperationResponse(request.OperationCode, responseObject);
            sendParameters.ChannelId = Settings.ItemEventChannel;
            peer.SendOperationResponse(response, sendParameters);

            lock (interestArea.SyncRoot)
            {
                interestArea.AttachToItem(avatar);
                interestArea.UpdateInterestManagement();
            }

            avatar.Spawn(operation.Position);
            world.Radar.AddItem(avatar, operation.Position);

            // response already sent
            return null;
        }

        /// <summary>
        /// IOperationHandler implementation.
        /// Stops any further operation handling and disposes the peer's resources.
        /// </summary>
        public void OnDisconnect(PeerBase peer)
        {
            this.peer.SetCurrentOperationHandler(null);
            this.peer.Dispose();
        }

        /// <summary>
        /// IOperationHandler implementation.
        /// Disconnects the peer.
        /// </summary>
        public void OnDisconnectByOtherPeer(PeerBase peer)
        {
            // disconnect after any queued events are sent
            peer.RequestFiber.Enqueue(() => peer.RequestFiber.Enqueue(peer.Disconnect));
        }

        /// <summary>
        /// IOperationHandler implementation.
        /// </summary>
        public OperationResponse OnOperationRequest(PeerBase peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch ((OperationCode)operationRequest.OperationCode)
            {
                case OperationCode.CreateWorld:
                    {
                        return this.OperationCreateWorld(peer, operationRequest);
                    }

                case OperationCode.EnterWorld:
                    {
                        return this.OperationEnterWorld(peer, operationRequest, sendParameters);
                    }

                case OperationCode.RadarSubscribe:
                    {
                        return OperationRadarSubscribe(peer, operationRequest, sendParameters);
                    }

                case OperationCode.SubscribeCounter:
                    {
                        return CounterOperations.SubscribeCounter(peer, operationRequest);
                    }

                case OperationCode.UnsubscribeCounter:
                    {
                        return CounterOperations.UnsubscribeCounter(peer, operationRequest);
                    }

                case OperationCode.AddInterestArea:
                case OperationCode.AttachInterestArea:
                case OperationCode.DestroyItem:
                case OperationCode.DetachInterestArea:
                case OperationCode.ExitWorld:
                case OperationCode.GetProperties:
                case OperationCode.Move:
                case OperationCode.MoveInterestArea:
                case OperationCode.RemoveInterestArea:
                case OperationCode.SetProperties:
                case OperationCode.SetViewDistance:
                case OperationCode.SpawnItem:
                case OperationCode.SubscribeItem:
                case OperationCode.UnsubscribeItem:
                    {
                        return InvalidOperation(operationRequest);
                    }
            }

            return new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (int)ReturnCode.OperationNotSupported,
                    DebugMessage = "OperationNotSupported: " + operationRequest.OperationCode
                };
        }

        private static void RadarChannel_OnItemEventMessage(PeerBase peer, ItemEventMessage message)
        {
            // already in right fiber, we would use peer.SendEvent otherwise
            peer.SendEvent(message.EventData, message.SendParameters);
        }
    }
}