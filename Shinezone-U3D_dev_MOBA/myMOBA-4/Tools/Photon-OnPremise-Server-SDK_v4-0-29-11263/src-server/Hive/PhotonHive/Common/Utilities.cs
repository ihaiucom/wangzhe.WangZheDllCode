// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Utilities type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Common
{
    using System.Collections;
    using System.Globalization;

    using Photon.Hive.Operations;

    /// <summary>
    /// A collection of methods useful in one or another context.
    /// </summary>
    public static class Utilities
    {
        private static readonly string amf3IsVisblePropertyKey = ((byte)GameParameter.IsVisible).ToString(CultureInfo.InvariantCulture);

        private static readonly string amf3IsOpenPropertyKey = ((byte)GameParameter.IsOpen).ToString(CultureInfo.InvariantCulture);

        private static readonly string amf3MaxPlayerPropertyKey = ((byte)GameParameter.MaxPlayers).ToString(CultureInfo.InvariantCulture);

        private static readonly string amf3PropertiesPropertyKey = ((byte)GameParameter.LobbyProperties).ToString(CultureInfo.InvariantCulture);

        private static readonly string amf3MasterClientIdPropertyKey = ((byte)GameParameter.MasterClientId).ToString(CultureInfo.InvariantCulture);

        private static readonly string amf3ExpectedUsersPropertyKey = ((byte)GameParameter.ExpectedUsers).ToString(CultureInfo.InvariantCulture);

        private static readonly string amf3NicknamePropertyKey = ((byte)ActorParameter.Nickname).ToString(CultureInfo.InvariantCulture);

        private static readonly string amf3IsInactivePropertyKey = ((byte)ActorParameter.IsInactive).ToString(CultureInfo.InvariantCulture);

        private static readonly string amf3UserIdPropertyKey = ((byte)ActorParameter.UserId).ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Converts well known properties sent by AS3/Flash clients - from string to byte-keys.
        /// </summary>
        /// <remarks>
        /// Check if peer is a flash (amf3) client because flash clients does not support byte keys in a hastable. 
        /// If a flash client likes to match a game with a specific 'MaxPlayer' value 'MaxPlayer' will be sent
        /// with the string key "255" and the max player value as int.
        /// </remarks>
        /// <param name="gameProps">A game properties hashtable.</param>
        /// <param name="actorProps">A actor properties hashtable.</param>
        public static void ConvertAs3WellKnownPropertyKeys(Hashtable gameProps, Hashtable actorProps)
        {
            // convert game properties
            if (gameProps != null && gameProps.Count > 0)
            {
                // well known property "is visible"
                if (gameProps.ContainsKey(amf3IsVisblePropertyKey))
                {
                    gameProps[(byte)GameParameter.IsVisible] = gameProps[amf3IsVisblePropertyKey];
                    gameProps.Remove(amf3IsVisblePropertyKey);
                }

                // well known property "is open"
                if (gameProps.ContainsKey(amf3IsOpenPropertyKey))
                {
                    gameProps[(byte)GameParameter.IsOpen] = gameProps[amf3IsOpenPropertyKey];
                    gameProps.Remove(amf3IsOpenPropertyKey);
                }

                // well known property "max players"
                if (gameProps.ContainsKey(amf3MaxPlayerPropertyKey))
                {
                    gameProps[(byte)GameParameter.MaxPlayers] = gameProps[amf3MaxPlayerPropertyKey];
                    gameProps.Remove(amf3MaxPlayerPropertyKey);
                }

                // well known property "props listed in lobby"
                if (gameProps.ContainsKey(amf3PropertiesPropertyKey))
                {
                    gameProps[(byte)GameParameter.LobbyProperties] = gameProps[amf3PropertiesPropertyKey];
                    gameProps.Remove(amf3PropertiesPropertyKey);
                }

                // well known property "master client id"
                if (gameProps.ContainsKey(amf3MasterClientIdPropertyKey))
                {
                    gameProps[(byte)GameParameter.MasterClientId] = gameProps[amf3MasterClientIdPropertyKey];
                    gameProps.Remove(amf3MasterClientIdPropertyKey);
                }

                // well known property "expected users"
                if (gameProps.ContainsKey(amf3ExpectedUsersPropertyKey))
                {
                    gameProps[(byte)GameParameter.ExpectedUsers] = gameProps[amf3ExpectedUsersPropertyKey];
                    gameProps.Remove(amf3ExpectedUsersPropertyKey);
                }
            }

            // convert actor properties (if any)
            if (actorProps != null && actorProps.Count > 0)
            {
                // well known property "PlayerName"
                if (actorProps.ContainsKey(amf3NicknamePropertyKey))
                {
                    actorProps[(byte)ActorParameter.Nickname] = actorProps[amf3NicknamePropertyKey];
                    actorProps.Remove(amf3NicknamePropertyKey);
                }

                // well known property "IsInactive" and "UserId"
                // can't be set by the client
                // will be removed in SetPropertiesHandler and JoinApplyGameStateChanges
            }
        }

        /// <summary>
        /// Converts well known properties sent by AS3/Flash clients - from string to byte-keys.
        /// </summary>
        /// <param name="gamePropertyKeys">The game properties list.</param>
        /// <param name="actorPropertyKeys">The actor properties list.</param>
        public static void ConvertAs3WellKnownPropertyKeys(IList gamePropertyKeys, IList actorPropertyKeys)
        {
            // convert game properties
            if (gamePropertyKeys != null && gamePropertyKeys.Count > 0)
            {
                // well known property "is visible"
                if (gamePropertyKeys.Contains(amf3IsVisblePropertyKey))
                {
                    gamePropertyKeys.Remove(amf3IsVisblePropertyKey);
                    gamePropertyKeys.Add((byte)GameParameter.IsVisible);
                }

                // well known property "is open"
                if (gamePropertyKeys.Contains(amf3IsOpenPropertyKey))
                {
                    gamePropertyKeys.Remove(amf3IsOpenPropertyKey);
                    gamePropertyKeys.Add((byte)GameParameter.IsOpen);
                }

                // well known property "max players"
                if (gamePropertyKeys.Contains(amf3MaxPlayerPropertyKey))
                {
                    gamePropertyKeys.Remove(amf3MaxPlayerPropertyKey);
                    gamePropertyKeys.Add((byte)GameParameter.MaxPlayers);
                }

                // well known property "props listed in lobby"
                if (gamePropertyKeys.Contains(amf3PropertiesPropertyKey))
                {
                    gamePropertyKeys.Remove(amf3PropertiesPropertyKey);
                    gamePropertyKeys.Add((byte)GameParameter.LobbyProperties);
                }

                // well known property "master client id"
                if (gamePropertyKeys.Contains(amf3MasterClientIdPropertyKey))
                {
                    gamePropertyKeys.Remove(amf3MasterClientIdPropertyKey);
                    gamePropertyKeys.Add((byte)GameParameter.MasterClientId);
                }

                // well known property "expected users"
                if (gamePropertyKeys.Contains(amf3ExpectedUsersPropertyKey))
                {
                    gamePropertyKeys.Remove(amf3ExpectedUsersPropertyKey);
                    gamePropertyKeys.Add((byte)GameParameter.ExpectedUsers);
                }
            }

            // convert actor properties (if any)
            if (actorPropertyKeys != null && actorPropertyKeys.Count > 0)
            {
                // well known property "PlayerName"
                if (actorPropertyKeys.Contains(amf3NicknamePropertyKey))
                {
                    actorPropertyKeys.Remove(amf3NicknamePropertyKey);
                    actorPropertyKeys.Add((byte)ActorParameter.Nickname);
                }

                // well known property "IsInactive"
                // can't be set by the client
                // will be removed in SetPropertiesHandler and JoinApplyGameStateChanges
            }
        }
    }
}
