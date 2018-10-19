// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Operations.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Tests.Disconnected
{
    using System.Collections;
    using System.Collections.Generic;

    using Photon.MmoDemo.Common;
    using Photon.SocketServer;
    
    public static class Operations
    {
        public static OperationRequest AttachCamera(string itemId)
        {
            var request = new OperationRequest { OperationCode = (byte)OperationCode.AttachInterestArea, Parameters = new Dictionary<byte, object>() };
            if (!string.IsNullOrEmpty(itemId))
            {
                request.Parameters.Add((byte)ParameterCode.ItemId, itemId);
            }

            return request;
        }

        public static OperationRequest CreateWorld(string worldName, BoundingBox boundingBox, Vector tileDimensions)
        {
            var request = new OperationRequest { OperationCode = (byte)OperationCode.CreateWorld, Parameters = new Dictionary<byte, object>() };
            request.Parameters.Add((byte)ParameterCode.WorldName, worldName);
            request.Parameters.Add((byte)ParameterCode.BoundingBox, boundingBox);
            request.Parameters.Add((byte)ParameterCode.TileDimensions, tileDimensions);
            return request;
        }

        public static OperationRequest DestroyItem(string itemId)
        {
            var request = new OperationRequest { OperationCode = (byte)OperationCode.DestroyItem, Parameters = new Dictionary<byte, object>() };
            request.Parameters.Add((byte)ParameterCode.ItemId, itemId);
            return request;
        }

        public static OperationRequest DetachCamera()
        {
            var request = new OperationRequest { OperationCode = (byte)OperationCode.DetachInterestArea, Parameters = new Dictionary<byte, object>() };
            return request;
        }

        public static OperationRequest EnterWorld(
            string worldName, string username, Hashtable properties, Vector position, Vector viewDistanceEnter, Vector viewDistanceExit)
        {
            var request = new OperationRequest { OperationCode = (byte)OperationCode.EnterWorld, Parameters = new Dictionary<byte, object>() };
            request.Parameters.Add((byte)ParameterCode.WorldName, worldName);
            request.Parameters.Add((byte)ParameterCode.Username, username);
            request.Parameters.Add((byte)ParameterCode.Position, position);
            request.Parameters.Add((byte)ParameterCode.ViewDistanceEnter, viewDistanceEnter);
            request.Parameters.Add((byte)ParameterCode.ViewDistanceExit, viewDistanceExit);
            if (properties != null)
            {
                request.Parameters.Add((byte)ParameterCode.Properties, properties);
            }

            return request;
        }

        public static OperationRequest ExitWorld()
        {
            var request = new OperationRequest { OperationCode = (byte)OperationCode.ExitWorld, Parameters = new Dictionary<byte, object>() };
            return request;
        }

        public static OperationRequest Move(string itemId, Vector position)
        {
            var request = new OperationRequest { OperationCode = (byte)OperationCode.Move, Parameters = new Dictionary<byte, object>() };
            request.Parameters.Add((byte)ParameterCode.Position, position);
            if (itemId != null)
            {
                request.Parameters.Add((byte)ParameterCode.ItemId, itemId);
            }

            return request;
        }

        public static OperationRequest SetProperties(string itemId, Hashtable propertiesSet, ArrayList propertiesUnset)
        {
            var request = new OperationRequest { OperationCode = (byte)OperationCode.SetProperties, Parameters = new Dictionary<byte, object>() };
            
            if (propertiesSet != null)
            {
                request.Parameters.Add((byte)ParameterCode.PropertiesSet, propertiesSet);
            }

            if (propertiesUnset != null)
            {
                request.Parameters.Add((byte)ParameterCode.PropertiesUnset, propertiesUnset);
            }

            if (itemId != null)
            {
                request.Parameters.Add((byte)ParameterCode.ItemId, itemId);
            }

            return request;
        }

        public static OperationRequest SetViewDistance(Vector viewDistanceEnter, Vector viewDistanceExit)
        {
            var request = new OperationRequest { OperationCode = (byte)OperationCode.SetViewDistance, Parameters = new Dictionary<byte, object>() };
            request.Parameters.Add((byte)ParameterCode.ViewDistanceEnter, viewDistanceEnter);
            request.Parameters.Add((byte)ParameterCode.ViewDistanceExit, viewDistanceExit);
            return request;
        }

        public static OperationRequest SpawnItem(string itemId, ItemType itemType, Vector position, Hashtable properties, bool subscribe)
        {
            var request = new OperationRequest { OperationCode = (byte)OperationCode.SpawnItem, Parameters = new Dictionary<byte, object>() };
            request.Parameters.Add((byte)ParameterCode.Position, position);
            request.Parameters.Add((byte)ParameterCode.ItemId, itemId);
            request.Parameters.Add((byte)ParameterCode.ItemType, (byte)itemType);
            request.Parameters.Add((byte)ParameterCode.Subscribe, subscribe);
            if (properties != null)
            {
                request.Parameters.Add((byte)ParameterCode.Properties, properties);
            }

            return request;
        }

        public static OperationRequest SubscribeItem(string itemId, int? propertiesRevision)
        {
            var request = new OperationRequest { OperationCode = (byte)OperationCode.SubscribeItem, Parameters = new Dictionary<byte, object>() };
            request.Parameters.Add((byte)ParameterCode.ItemId, itemId);
            if (propertiesRevision.HasValue)
            {
                request.Parameters.Add((byte)ParameterCode.PropertiesRevision, propertiesRevision);
            }

            return request;
        }

        public static OperationRequest UnsubscribeItem(string itemId)
        {
            var request = new OperationRequest { OperationCode = (byte)OperationCode.UnsubscribeItem, Parameters = new Dictionary<byte, object>() };
            request.Parameters.Add((byte)ParameterCode.ItemId, itemId);

            return request;
        }
    }
}