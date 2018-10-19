// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameEvents.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Game events methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using ExitGames.Client.Photon;
#if Unity
    using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif

    using Photon.MmoDemo.Common;

    public partial class Game
    {

        public void OnEventReceive(EventData eventData)
        {
            string itemId;
            Item item;
            switch ((EventCode)eventData.Code)
            {
                case EventCode.RadarUpdate:                    
                    this.Listener.OnRadarUpdate(
                        (string)eventData[(byte)ParameterCode.ItemId],
                        (ItemType)(byte)eventData[(byte)ParameterCode.ItemType],                        
                        (Vector)eventData[(byte)ParameterCode.Position],
                        (bool)eventData[(byte)ParameterCode.Remove]
                        );
                    return;

                case EventCode.ItemMoved:
                    itemId = (string)eventData[(byte)ParameterCode.ItemId];
                    if (this.TryGetItem(itemId, out item))
                    {
                        if (!item.IsMine)
                        {
                            Vector position = (Vector)eventData[(byte)ParameterCode.Position];
                            Vector oldPosition = (Vector)eventData[(byte)ParameterCode.OldPosition];
                            Vector rotation = (Vector)(eventData[(byte)ParameterCode.Rotation] ?? Vector.Zero);
                            Vector oldRotation = (Vector)(eventData[(byte)ParameterCode.OldRotation] ?? Vector.Zero);
                            item.SetPositions(position, oldPosition, rotation, oldRotation);
                        }
                    }
                    return;

                case EventCode.ItemDestroyed:
                    itemId = (string)eventData[(byte)ParameterCode.ItemId];
                    if (this.TryGetItem(itemId, out item))
                    {
                        item.IsDestroyed = this.RemoveItem(item);
                    }
                    return;

                case EventCode.ItemProperties:
                    HandleEventItemProperties(eventData.Parameters);
                    return;

                case EventCode.ItemPropertiesSet:
                    HandleEventItemPropertiesSet(eventData.Parameters);
                    return;

                case EventCode.ItemSubscribed: // item enters our interest area
                    HandleEventItemSubscribed(eventData.Parameters);
                    return;

                case EventCode.ItemUnsubscribed: // item exits our interest area
                    HandleEventItemUnsubscribed(eventData.Parameters);
                    return;

                case EventCode.WorldExited:
                    this.SetConnected();
                    return;
            }

            this.OnUnexpectedEventReceive(eventData);
        }

        private void HandleEventItemProperties(IDictionary eventData)
        {
            var itemId = (string)eventData[(byte)ParameterCode.ItemId];

            Item item;
            if (this.TryGetItem(itemId, out item))
            {
                item.PropertyRevisionLocal = (int)eventData[(byte)ParameterCode.PropertiesRevision];

                if (!item.IsMine)
                {
                    var propertiesSet = (Hashtable)eventData[(byte)ParameterCode.PropertiesSet];

                    item.SetColor((int)propertiesSet[Item.PropertyKeyColor]);
                    item.SetText((string)propertiesSet[Item.PropertyKeyText]);
                    item.SetInterestAreaAttached((bool)propertiesSet[Item.PropertyKeyInterestAreaAttached]);
                    item.SetInterestAreaViewDistance(
                        (Vector)propertiesSet[Item.PropertyKeyViewDistanceEnter], (Vector)propertiesSet[Item.PropertyKeyViewDistanceExit]);

                }
            }
        }

        private void HandleEventItemPropertiesSet(IDictionary eventData)
        {
            var itemId = (string)eventData[(byte)ParameterCode.ItemId];
            Item item;
            if (this.TryGetItem(itemId, out item))
            {
                item.PropertyRevisionLocal = (int)eventData[(byte)ParameterCode.PropertiesRevision];

                if (!item.IsMine)
                {
                    var propertiesSet = (Hashtable)eventData[(byte)ParameterCode.PropertiesSet];

                    if (propertiesSet.ContainsKey(Item.PropertyKeyColor))
                    {
                        item.SetColor((int)propertiesSet[Item.PropertyKeyColor]);
                    }

                    if (propertiesSet.ContainsKey(Item.PropertyKeyText))
                    {
                        item.SetText((string)propertiesSet[Item.PropertyKeyText]);
                    }

                    if (propertiesSet.ContainsKey(Item.PropertyKeyViewDistanceEnter))
                    {
                        var viewDistanceEnter = (Vector)propertiesSet[Item.PropertyKeyViewDistanceEnter];
                        item.SetInterestAreaViewDistance(viewDistanceEnter, (Vector)propertiesSet[Item.PropertyKeyViewDistanceExit]);
                    }

                    if (propertiesSet.ContainsKey(Item.PropertyKeyInterestAreaAttached))
                    {
                        item.SetInterestAreaAttached((bool)propertiesSet[Item.PropertyKeyInterestAreaAttached]);
                    }
                }
            }
        }

        private void HandleEventItemSubscribed(IDictionary eventData)
        {
            var itemType = (ItemType)(byte)eventData[(byte)ParameterCode.ItemType];
            var itemId = (string)eventData[(byte)ParameterCode.ItemId];
            Vector position = (Vector)eventData[(byte)ParameterCode.Position];
            var cameraId = (byte)eventData[(byte)ParameterCode.InterestAreaId];
            Vector rotation = eventData.Contains((byte)ParameterCode.Rotation) ? (Vector)eventData[(byte)ParameterCode.Rotation] : Vector.Zero;

            Item item;
            if (!this.TryGetItem(itemId, out item)) // register item first time seen 
            {
                item = new Item(this, itemId, itemType);
                this.AddItem(item);
                item.GetProperties();
            } 
            if (!item.IsMine)
            {
                item.PropertyRevisionRemote = (int)eventData[(byte)ParameterCode.PropertiesRevision];

                if (item.PropertyRevisionRemote != item.PropertyRevisionLocal)
                {
                    item.GetProperties();
                }
                item.SetPositions(position, position, rotation, rotation);
            }

            item.AddSubscribedInterestArea(cameraId);
        }

        private void HandleEventItemUnsubscribed(IDictionary eventData)
        {
            var itemId = (string)eventData[(byte)ParameterCode.ItemId];
            var cameraId = (byte)eventData[(byte)ParameterCode.InterestAreaId];

            Item item;
            if (this.TryGetItem(itemId, out item))
            {
                item.RemoveSubscribedInterestArea(cameraId);
            }
        }

        
    }
}