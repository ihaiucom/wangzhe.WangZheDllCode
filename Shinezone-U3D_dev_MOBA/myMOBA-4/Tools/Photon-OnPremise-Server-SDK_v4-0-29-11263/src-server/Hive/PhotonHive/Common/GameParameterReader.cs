// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameParameterReader.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.Hive.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Photon.Hive.Operations;

    /// <summary>
    /// Provides methods to read build in game properties from a hashtable.
    /// </summary>
    /// <remarks>
    /// Build in game properties in the load balancing project are stored as byte values. 
    /// Because some protocols used by photon (Flash, WebSockets) does not support byte values
    /// the properties will also be searched in the hashtable using there int representation.
    /// If an int representation is found it will be converted to the byte representation of 
    /// the game property.
    /// </remarks>
    public static class GameParameterReader
    {
        #region Public Methods

        public static bool TryReadBooleanParameter(Hashtable hashtable, GameParameter paramter, out bool? result, out object value)
        {
            result = null;

            if (!TryReadGameParameter(hashtable, paramter, out value))
            {
                return true;
            }

            if (value is bool)
            {
                result = (bool)value;
                return true;
            }

            return false;
        }

        public static bool TryReadByteParameter(Hashtable hashtable, GameParameter paramter, out byte? result, out object value)
        {
            result = null;

            if (!TryReadGameParameter(hashtable, paramter, out value))
            {
                return true;
            }

            if (value is byte)
            {
                result = (byte)value;
                return true;
            }

            if (value is int)
            {
                result = (byte)(int)value;
                hashtable[(byte)paramter] = result;
                return true;
            }

            if (value is double)
            {
                result = (byte)(double)value;
                hashtable[(byte)paramter] = result;
                return true;
            }

            return false;
        }

        public static bool TryGetProperties(Hashtable propertyTable, out byte? maxPlayer, out bool? isOpen, out bool? isVisible,
            out object[] properties, out string[] expectedUsers, out string debugMessage)
        {
            int? masterClientId;
            return TryGetProperties(propertyTable, out maxPlayer, out isOpen, 
                out isVisible, out properties, out masterClientId, out expectedUsers, out debugMessage);
        }

        public static bool TryGetProperties(Hashtable propertyTable, out byte? maxPlayer, out bool? isOpen, 
            out bool? isVisible, out object[] properties, out int? masterClientId, out string[] expectedUsers, out string debugMessage)
        {
            object value;
            isOpen = null;
            isVisible = null;
            properties = null;
            debugMessage = null;
            expectedUsers = null;
            masterClientId = null;
            if (TryReadByteParameter(propertyTable, GameParameter.MaxPlayers, out maxPlayer, out value) == false)
            {
                debugMessage = GetInvalidGamePropertyTypeMessage(GameParameter.MaxPlayers, typeof(byte), value);
                return false;
            }

            if (TryReadBooleanParameter(propertyTable, GameParameter.IsOpen, out isOpen, out value) == false)
            {
                debugMessage = GetInvalidGamePropertyTypeMessage(GameParameter.IsOpen, typeof(bool), value);
                return false;
            }

            if (TryReadBooleanParameter(propertyTable, GameParameter.IsVisible, out isVisible, out value) == false)
            {
                debugMessage = GetInvalidGamePropertyTypeMessage(GameParameter.IsVisible, typeof(bool), value);
                return false;
            }

            if (TryReadGameParameter(propertyTable, GameParameter.LobbyProperties, out value))
            {
                if (value != null && value is object[] == false)
                {
                    debugMessage = GetInvalidGamePropertyTypeMessage(GameParameter.LobbyProperties, typeof(object[]), value);
                    return false;
                }

                properties = (object[])value;
            }

            if (TryReadGameParameter(propertyTable, GameParameter.MasterClientId, out value))
            {
                if (value != null)
                {
                    if (value is int == false)
                    {
                        debugMessage = GetInvalidGamePropertyTypeMessage(GameParameter.MasterClientId, typeof (int), value);
                        return false;
                    }
                    masterClientId = (int)value;
                }
            }

            if (TryReadGameParameter(propertyTable, GameParameter.ExpectedUsers, out value))
            {
                if (value != null)
                {
                    if (value is string[] == false)
                    {
                        debugMessage = GetInvalidGamePropertyTypeMessage(GameParameter.ExpectedUsers, typeof(string[]), value);
                        return false;
                    }
                    expectedUsers = (string[])value;
                }
            }


            return true;
        }

        public static Hashtable GetLobbyGameProperties(Hashtable source, HashSet<object> list)
        {
            if (source == null || source.Count == 0)
            {
                return null;
            }

            Hashtable gameProperties;

            if (list != null)
            {
                // filter for game properties is set, only properties in the specified list 
                // will be reported to the lobby 
                gameProperties = new Hashtable(list.Count);

                foreach (object entry in list)
                {
                    if (source.ContainsKey(entry))
                    {
                        gameProperties.Add(entry, source[entry]);
                    }
                }
            }
            else
            {
                // if no filter is set for properties which should be listet in the lobby
                // all properties are send
                gameProperties = source;
                gameProperties.Remove((byte)GameParameter.MaxPlayers);
                gameProperties.Remove((byte)GameParameter.IsOpen);
                gameProperties.Remove((byte)GameParameter.IsVisible);
                gameProperties.Remove((byte)GameParameter.LobbyProperties);
            }

            return gameProperties;
        }


        #endregion

        #region Methods


        private static string GetInvalidGamePropertyTypeMessage(GameParameter parameter, Type expectedType, object value)
        {
            return string.Format(
                "Invalid type for property {0}. Expected type {1} but is {2}", parameter, expectedType, value == null ? "null" : value.GetType().ToString());
        }

        private static bool TryReadGameParameter(Hashtable hashtable, GameParameter paramter, out object result)
        {
            var byteKey = (byte)paramter;
            if (hashtable.ContainsKey(byteKey))
            {
                result = hashtable[byteKey];
                return true;
            }

            var intKey = (int)paramter;
            if (hashtable.ContainsKey(intKey))
            {
                result = hashtable[intKey];
                hashtable.Remove(intKey);
                hashtable[byteKey] = result;
                return true;
            }

            result = null;
            return false;
        }

        #endregion
    }
}