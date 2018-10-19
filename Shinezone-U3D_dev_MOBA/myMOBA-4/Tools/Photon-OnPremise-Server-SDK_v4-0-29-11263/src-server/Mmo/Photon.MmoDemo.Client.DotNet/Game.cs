// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Game.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The game logic.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using ExitGames.Client.Photon;

    using Photon.MmoDemo.Common;
    using System.Collections;

    // The game logic.
    public partial class Game : IPhotonPeerListener
    {
        public static readonly Vector MoveDown = new Vector { X = 0, Y = -1 };

        public static readonly Vector MoveDownLeft = new Vector { X = -1, Y = -1 };

        public static readonly Vector MoveDownRight = new Vector { X = 1, Y = -1 };

        public static readonly Vector MoveLeft = new Vector { X = -1, Y = 0 };

        public static readonly Vector MoveRight = new Vector { X = 1, Y = 0 };

        public static readonly Vector MoveUp = new Vector { X = 0, Y = 1 };

        public static readonly Vector MoveUpLeft = new Vector { X = -1, Y = 1 };

        public static readonly Vector MoveUpRight = new Vector { X = 1, Y = 1 };

        private readonly MyItem avatar;

        private readonly Dictionary<byte, InterestArea> cameras = new Dictionary<byte, InterestArea>();

        private readonly Dictionary<string, Item> itemCache = new Dictionary<string, Item>();

        private readonly IGameListener listener;

        private readonly Settings settings;

        private int outgoingOperationCount;

        private PhotonPeer peer;

        public Game(IGameListener listener, Settings settings, string avatarName)
        {
            this.listener = listener;
            this.settings = settings;

            this.avatar = new MyItem(this, Guid.NewGuid().ToString(), ItemType.Avatar, avatarName);

            this.AddItem(this.Avatar);
            this.AddCamera(new InterestArea(0, this, this.avatar));
            this.WorldData = new WorldData
                {
                    BoundingBox = new BoundingBox
                    {
                        Min = new Vector(),
                        Max = new Vector(settings.GridSize.X, settings.GridSize.Y)
                    },
                    Name = this.settings.WorldName,
                    TileDimensions = this.settings.TileDimensions
                };
        }

        public MyItem Avatar
        {
            get
            {
                return this.avatar;
            }
        }

        public Dictionary<string, Item> Items
        {
            get
            {
                return this.itemCache;
            }
        }

        public IGameListener Listener
        {
            get
            {
                return this.listener;
            }
        }

        public PhotonPeer Peer
        {
            get
            {
                return this.peer;
            }
        }

        public Settings Settings
        {
            get
            {
                return this.settings;
            }
        }

        public bool WorldEntered
        {
            get; private set;
        }
        public WorldData WorldData { get; private set; }

        public void AddCamera(InterestArea camera)
        {
            this.cameras.Add(camera.Id, camera);
        }

        public void AddItem(Item item)
        {
            itemCache.Add(item.Id, item);
            this.listener.OnItemAdded(item);
        }

        public void Disconnect()
        {
            this.peer.Disconnect();
        }

        public void Initialize(PhotonPeer photonPeer)
        {
            this.peer = photonPeer;
            this.registerTypes();            
        }

        public void Connect()
        {
            this.peer.Connect(this.settings.ServerAddress, this.settings.ApplicationName);
        }

        private void registerTypes()
        {
            PhotonPeer.RegisterType(typeof(Vector), (byte)Common.Protocol.CustomTypeCodes.Vector, Common.Protocol.SerializeVector, Common.Protocol.DeserializeVector);
            PhotonPeer.RegisterType(typeof(BoundingBox), (byte)Common.Protocol.CustomTypeCodes.BoundingBox, Common.Protocol.SerializeBoundingBox, Common.Protocol.DeserializeBoundingBox);
        }

        public void EnterWorld()
        {
            var r = new Random();
            var d = this.WorldData.BoundingBox.Max - this.WorldData.BoundingBox.Max;
            var position = this.WorldData.BoundingBox.Min + new Vector { X = d.X * (float)r.NextDouble(), Y = d.Y * (float)r.NextDouble() };
            this.Avatar.SetPositions(position, position, Vector.Zero, Vector.Zero);

            Operations.EnterWorld(this, this.WorldData.Name, this.Avatar.Id, this.Avatar.BuildProperties(), this.Avatar.Position, this.Avatar.Rotation, this.Avatar.ViewDistanceEnter, this.Avatar.ViewDistanceExit);
        }

        public void OnCameraAttached(string itemId)
        {
            this.listener.OnCameraAttached(itemId);
        }

        public void OnCameraDetached()
        {
            this.listener.OnCameraDetached();
        }

        public void OnItemSpawned(string itemId)
        {
            this.listener.OnItemSpawned(itemId);
        }

        public void OnUnexpectedEventReceive(EventData @event)
        {
            this.listener.LogError(string.Format("{0}: unexpected event {1}", this.avatar.Text, @event.Code));
        }

        public void OnUnexpectedOperationError(OperationResponse operationResponse)
        {
            string message = string.Format(
                "{0}{1}: unexpected operation error: code = {2}, error = {3}, message = {4}",
                this.avatar.Text,
                this.WorldEntered ? "(in World)" : "",
                operationResponse.OperationCode,
                operationResponse.ReturnCode,
                operationResponse.DebugMessage
                );
            this.listener.LogError(message);
        }
       
        public void OnUnexpectedPhotonReturn(OperationResponse operationResponse)
        {
            this.listener.LogError(string.Format("{0}: unexpected return {1}", this.avatar.Text, operationResponse.OperationCode));
        }

        public bool RemoveCamera(byte cameraId)
        {
            return this.cameras.Remove(cameraId);
        }

        public bool RemoveItem(Item item)
        {
            var res = itemCache.Remove(item.Id);
            this.listener.OnItemRemoved(item);
            return res;
        }

        public void SendOperation(OperationCode operationCode, Dictionary<byte, object> parameter, bool sendReliable, byte channelId)
        {
            if (this.listener.IsDebugLogEnabled)
            {
                var builder = new StringBuilder();
                builder.AppendFormat("{0}: send operation {1}:", this.avatar.Id, operationCode);
                foreach (var entry in parameter)
                {
                    builder.AppendFormat(" {0}=", (ParameterCode)entry.Key);
                    if (entry.Value is float[])
                    {
                        builder.Append("float[");
                        foreach (float number in (float[])entry.Value)
                        {
                            builder.AppendFormat("{0:0.00},", number);
                        }

                        builder.Append("]");
                    }
                    else
                    {
                        builder.Append(entry.Value);
                    }
                }

                this.listener.LogDebug(builder.ToString());
            }

            peer.OpCustom((byte)operationCode, parameter, sendReliable, channelId);

            // avoid operation congestion (QueueOutgoingUnreliableWarning)
            this.outgoingOperationCount++;
            if (this.outgoingOperationCount > 10)
            {
                this.peer.SendOutgoingCommands();
                this.outgoingOperationCount = 0;
            }
        }

        public void SetConnected()
        {
            this.WorldEntered = false;
            this.listener.OnConnect();
        }

        public void SetDisconnected(StatusCode returnCode)
        {
            this.WorldEntered = false;
            this.listener.OnDisconnect(returnCode);
        }

        public void SetStateWorldEntered(WorldData worldData)
        {
            this.WorldData = worldData;
            this.WorldEntered = true;
            InterestArea camera;
            this.cameras.TryGetValue(0, out camera);
            camera.ResetViewDistance();
            this.Avatar.SetInterestAreaAttached(true);
            this.listener.OnWorldEntered();
        }

        public bool TryGetCamera(byte cameraId, out InterestArea camera)
        {
            return this.cameras.TryGetValue(cameraId, out camera);
        }

        public bool TryGetItem(string itemId, out Item item)
        {
            return itemCache.TryGetValue(itemId, out item);
        }

        public void Update()
        {
            this.peer.Service();
        }

        public void DebugReturn(DebugLevel debugLevel, string debug)
        {
            if (this.listener.IsDebugLogEnabled)
            {
                // we don't use debugLevel here - just log what's there
                this.listener.LogDebug(string.Concat(this.avatar.Id, ": ", debug));
            }
        }

        public void OnEvent(EventData ev)
        {
            if (this.listener.IsDebugLogEnabled)
            {
                var builder = new StringBuilder();
                builder.AppendFormat("{0}: received event {1}:", this.avatar.Id, (EventCode)ev.Code);
                foreach (var entry in ev.Parameters)
                {
                    builder.AppendFormat(" {0}=", (ParameterCode)entry.Key);
                    if (entry.Value is float[])
                    {
                        builder.Append("float[");
                        foreach (float number in (float[])entry.Value)
                        {
                            builder.AppendFormat("{0:0.00},", number);
                        }

                        builder.Append("]");
                    }
                    else
                    {
                        builder.Append(entry.Value);
                    }
                }

                this.listener.LogDebug(builder.ToString());
            }

            this.OnEventReceive(ev);
        }

        public void OnOperationResponse(OperationResponse response)
        {
            try
            {
                if (this.listener.IsDebugLogEnabled)
                {
                    this.listener.LogDebug(string.Format("{0}: received return {1}", this.avatar.Id, response.ReturnCode));
                }

                if (response.ReturnCode == 0)
                {
                    string itemId;
                    switch ((OperationCode)response.OperationCode)
                    {
                        case OperationCode.CreateWorld:
                            this.EnterWorld();
                            return;

                        case OperationCode.EnterWorld:
                            var worldData = new WorldData
                            {
                                Name = (string)response.Parameters[(byte)ParameterCode.WorldName],
                                BoundingBox = (BoundingBox)response.Parameters[(byte)ParameterCode.BoundingBox],
                                TileDimensions = (Vector)response.Parameters[(byte)ParameterCode.TileDimensions]
                            };
                            this.SetStateWorldEntered(worldData);
                            return;
                        case OperationCode.RemoveInterestArea:
                        case OperationCode.AddInterestArea:
                            return;

                        case OperationCode.AttachInterestArea:
                            itemId = (string)response[(byte)ParameterCode.ItemId];
                            this.OnCameraAttached(itemId);
                            return;

                        case OperationCode.DetachInterestArea:
                            this.OnCameraDetached();
                            return;

                        case OperationCode.SpawnItem:
                            itemId = (string)response[(byte)ParameterCode.ItemId];
                            this.OnItemSpawned(itemId);
                            return;

                        case OperationCode.RadarSubscribe:
                            return;
                    }
                }
                else
                {
                    switch ((OperationCode)response.OperationCode)
                    {
                        case OperationCode.EnterWorld:
                            Operations.CreateWorld(
                                this, this.WorldData.Name, this.WorldData.BoundingBox, this.WorldData.TileDimensions);
                            return;
                    }
                }

                this.OnUnexpectedOperationError(response);
            }
            catch (Exception e)
            {
                this.listener.LogError(e);
            }
        }

        public void OnStatusChanged(StatusCode returnCode)
        {
            try
            {
                if (this.listener.IsDebugLogEnabled)
                {
                    this.listener.LogDebug(string.Format("{0}: received callback {1}", this.avatar.Id, returnCode));
                }

                switch (returnCode)
                {
                    case StatusCode.Connect:
                        this.SetConnected();
                        this.EnterWorld();
                        break;

                    case StatusCode.Disconnect:
                    case StatusCode.DisconnectByServer:
                    case StatusCode.DisconnectByServerLogic:
                    case StatusCode.DisconnectByServerUserLimit:
                    case StatusCode.TimeoutDisconnect:
                        this.SetDisconnected(returnCode);
                        break;

                    default:
                        this.DebugReturn(DebugLevel.ERROR, returnCode.ToString());
                        break;
                }
            }
            catch (Exception e)
            {
                this.listener.LogError(e);
            }
        }

        public void OnMessage(object messages)
        {
            //
        }
    }
}