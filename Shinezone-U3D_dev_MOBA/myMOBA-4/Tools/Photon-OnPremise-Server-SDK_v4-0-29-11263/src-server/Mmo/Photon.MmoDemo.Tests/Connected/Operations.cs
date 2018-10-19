// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Operations.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Tests.Connected
{
    using System.Collections;
    using System.Collections.Generic;

    using Photon.MmoDemo.Common;

    using SocketServer;

    public static class Operations
    {
        public static void AttachCamera(Client client, string itemId)
        {
            var data = new Dictionary<byte, object>();

            if (!string.IsNullOrEmpty(itemId))
            {
                data.Add((byte)ParameterCode.ItemId, itemId);
            }

            client.SendOperation((byte)OperationCode.AttachInterestArea, data, true);
        }

        public static void CreateWorld(Client client, string worldName, BoundingBox boundingBox, Vector tileDimensions)
        {
            var data = new Dictionary<byte, object>
                {
                    { (byte)ParameterCode.WorldName, worldName }, 
                    { (byte)ParameterCode.BoundingBox, boundingBox }, 
                    { (byte)ParameterCode.TileDimensions, tileDimensions }
                };
            client.SendOperation((byte)OperationCode.CreateWorld, data, true);
        }

        public static void DestroyItem(Client client, string itemId)
        {
            var data = new Dictionary<byte, object> { { (byte)ParameterCode.ItemId, itemId } };
            client.SendOperation((byte)OperationCode.DestroyItem, data, true);
        }

        public static void DetachCamera(Client client)
        {
            client.SendOperation((byte)OperationCode.DetachInterestArea, new Dictionary<byte, object>(), true);
        }

        public static void EnterWorld(
            Client client, string worldName, string username, Hashtable properties, Vector position, Vector viewDistanceEnter, Vector viewDistanceExit)
        {
            var data = new Dictionary<byte, object>
                {
                    { (byte)ParameterCode.WorldName, worldName }, 
                    { (byte)ParameterCode.Username, username }, 
                    { (byte)ParameterCode.Position, position }, 
                    { (byte)ParameterCode.ViewDistanceEnter, viewDistanceEnter }, 
                    { (byte)ParameterCode.ViewDistanceExit, viewDistanceExit }
                };
            if (properties != null)
            {
                data.Add((byte)ParameterCode.Properties, properties);
            }

            client.SendOperation((byte)OperationCode.EnterWorld, data, true);
        }

        public static void ExitWorld(Client client)
        {
            client.SendOperation((byte)OperationCode.ExitWorld, new Dictionary<byte, object>(), true);
        }

        public static void Move(Client client, string itemId, Vector position)
        {
            var data = new Dictionary<byte, object> { { (byte)ParameterCode.Position, position } };
            if (itemId != null)
            {
                data.Add((byte)ParameterCode.ItemId, itemId);
            }

            client.SendOperation((byte)OperationCode.Move, data, true);
        }

        public static void SetProperties(Client client, string itemId, Hashtable propertiesSet, ArrayList propertiesUnset)
        {
            var data = new Dictionary<byte, object>();
            if (propertiesSet != null)
            {
                data.Add((byte)ParameterCode.PropertiesSet, propertiesSet);
            }

            if (propertiesUnset != null)
            {
                data.Add((byte)ParameterCode.PropertiesUnset, propertiesUnset);
            }

            if (itemId != null)
            {
                data.Add((byte)ParameterCode.ItemId, itemId);
            }

            client.SendOperation((byte)OperationCode.SetProperties, data, true);
        }

        public static void SetViewDistance(Client client, Vector viewDistanceEnter, Vector viewDistanceExit)
        {
            var data = new Dictionary<byte, object>
                {
                    { (byte)ParameterCode.ViewDistanceEnter, viewDistanceEnter }, { (byte)ParameterCode.ViewDistanceExit, viewDistanceExit } 
                };
            client.SendOperation((byte)OperationCode.SetViewDistance, data, true);
        }

        public static void SpawnItem(Client client, string itemId, byte itemType, Vector position, Hashtable properties, bool subscribe)
        {
            var data = new Dictionary<byte, object>
                {
                    { (byte)ParameterCode.Position, position }, 
                    { (byte)ParameterCode.ItemId, itemId }, 
                    { (byte)ParameterCode.ItemType, itemType }, 
                    { (byte)ParameterCode.Subscribe, subscribe }
                };
            if (properties != null)
            {
                data.Add((byte)ParameterCode.Properties, properties);
            }

            client.SendOperation((byte)OperationCode.SpawnItem, data, true);
        }

        public static void SubscribeItem(Client client, string itemId, int? propertiesRevision)
        {
            var data = new Dictionary<byte, object> { { (byte)ParameterCode.ItemId, itemId } };
            if (propertiesRevision.HasValue)
            {
                data.Add((byte)ParameterCode.PropertiesRevision, propertiesRevision);
            }

            client.SendOperation((byte)OperationCode.SubscribeItem, data, true);
        }

        public static void UnsubscribeItem(Client client, string itemId)
        {
            var data = new Dictionary<byte, object> { { (byte)ParameterCode.ItemId, itemId } };

            client.SendOperation((byte)OperationCode.UnsubscribeItem, data, true);
        }
    }
}